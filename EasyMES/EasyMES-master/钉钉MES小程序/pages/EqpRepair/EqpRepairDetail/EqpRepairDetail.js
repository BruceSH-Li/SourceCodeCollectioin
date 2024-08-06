var app = getApp();
var myRequest = require('/static/js/myRequest.js');
Page({
  data: {
    repairInfo: {},
    CurrentManName: "",
    CurrentMan: "",
    type: "",
    objectManArray: [],
    // arrManIndex: 0,

    ManList: [],

    selectValue: '', // 选择的值
    selectShow: false, // 是否显示级联组件
    // rawCitiesData: [], // mock的级联数据


    images: [],
    uploadedImages: [],
    imageWidth: 100,

    processData: [{
      name: '报修',
      start: '#fff',
      end: '#EFF3F6',
      icon: '/static/images/ProcessUndo.png',
      state: 1
    },
    {
      name: '已派工',
      start: '#EFF3F6',
      end: '#EFF3F6',
      icon: '/static/images/ProcessUndo.png',
      state: 2
    },
    {
      name: '开始维修',
      start: '#EFF3F6',
      end: '#EFF3F6',
      icon: '/static/images/ProcessUndo.png',
      state: 3
    },
    {
      name: '维修结束',
      start: '#EFF3F6',
      end: '#EFF3F6',
      icon: '/static/images/ProcessUndo.png',
      state: 4
    },
    {
      name: '已确认',
      start: '#EFF3F6',
      end: '#fff',
      icon: '/static/images/ProcessUndo.png',
      state: 99
    }]

  },
  onLoad(query) {
    var that = this;

    dd.setNavigationBar({
      title: "维修工单明细"
    });

    dd.getSystemInfo({
      success: function (res) {
        that.setData({
          imageWidth: res.screenWidth / 4 - 10
        })
      }
    })

    var RepairId = query.RepairId;
    var type = query.type;

    that.getRepairInfoByRepairId(RepairId, type);

    if (type != "record") {
      that.getManList();
    }



    dd.getStorage({
      key: 'userInfo',
      success: (res) => {
        that.setData({
          CurrentManName: res.data.username,
          CurrentMan: res.data.userid,
          type: type
        })
      }
    });




  },

  onShow() {
    this.setData({
      ManList: []
    })
  },

  /**
 * 点击输入框显示级联组件
 * @method openSelect
 */
  openSelect() {
    this.setData({
      selectShow: true,
      objectManArray: this.data.objectManArray
    });
  },
  /**
   * 关闭级联组件
   * @method closeSelect
   */
  closeSelect() {
    this.setData({ selectShow: false });
  },
  /**
   * 点击确认触发的事件
   * @method onSelectSuccess
   * @param {*} result
   */
  onSelectSuccess(result) {
    let selectValue = {};

    var that = this;
    if (result) selectValue = result.pop();

    // if (e.detail.value == this.data.arrManIndex) return;
    if (this.data.repairInfo.repariManlist) {
      var repairingarr = this.data.repairInfo.repariManlist.map(function (o) { return o.F_RepairMan });
      if (repairingarr.indexOf(selectValue.code) >= 0) {
        dd.alert({ title: '提示', content: "该人员已派工，无需重复派工！", buttonText: '我知道了', });
        return;
      }
    }
    var arr = this.data.ManList.map(function (o) { return o.F_RepairMan });
    if (arr.indexOf(selectValue.code) >= 0) return;
    var json = {
      F_RepairMan: selectValue.code,
      F_RepairManName: selectValue.name
    };

    that.data.ManList.push(json);

    that.setData({
      // arrManIndex: e.detail.value,
      ManList: that.data.ManList
    });

  },

  setProcessIcon: function () {
    var that = this;
    var processArr = that.data.processData

    for (var i = 0; i < processArr.length; i++) {
      if (processArr[i].state < that.data.repairInfo.F_Status) {
        processArr[i].icon = "/static/images/ProcessDone.png";
        processArr[i].start = "#45B2FE";
        processArr[i].end = "#45B2FE";
      } else if (processArr[i].state == that.data.repairInfo.F_Status) {
        processArr[i].icon = "/static/images/Processing.png";
        processArr[i].start = "#45B2FE";
        processArr[i].end = "#EFF3F6";
      } else {
        processArr[i].icon = "/static/images/ProcessUndo.png";
        processArr[i].start = "#EFF3F6";
        processArr[i].end = "#EFF3F6";
      }

    }

    processArr[0].start = "#fff"
    processArr[4].end = "#fff"
    that.setData({
      processData: processArr
    })
  },

  ToRepairFlow: function () {

    dd.navigateTo({
      url: "/pages/EqpRepair/EqpRepairFlow/EqpRepairFlow?repairId=" + this.data.repairInfo.F_Id + "&status=" + this.data.repairInfo.F_Status
    })
  },




  previewImage: function (event) {        // 预览图集
    var that = this;
    dd.previewImage({
      urls: that.data.images,
      current: event.target.dataset.index

    });

  },




  getRepairInfoByRepairId: function (code, type) {

    myRequest.get("/api/EqpRepair/getRepairInfoByRepairId?Repairid=" + code).then((res) => {
      var imageslist = [];
      var imageslistNew = [];
      if (res.data.data.F_PhotoPath) {
        imageslist = res.data.data.F_PhotoPath.split(";");
        imageslistNew = imageslist.map(x => app.globalData .PhotoUrl + x.replace(/\\/g, "/"))
      }

      this.setData({
        repairInfo: res.data.data,
        // images:["http://localhost:8082/upload/"+imageslist[0].replace(/\\/g,"/")]
        images: imageslistNew
      });

      if (type == "record") {
        this.setProcessIcon();
      }


    })
  },

  getManList: function (code) {
    myRequest.get("/api/EqpRepair/getManList").then((res) => {
      this.setData({
        objectManArray: res.data.data,

      });


    })
  },


  onSubmit(e) {
    //   //提交图片
    var that = this;
    // var data = that.data;
    if (that.data.ManList.length == 0) {
      dd.alert({ title: '提示', content: "请选择维修人！", buttonText: '我知道了', });
      return;
    }

    var data = {
      type: that.data.type,
      repairId: that.data.repairInfo.F_Id,
      currentMan: that.data.CurrentMan,
      currentManName: that.data.CurrentManName,
      repairManList: that.data.ManList
    }
    myRequest.postWithoutBack("/api/EqpRepair/UpdateEqpRepairInfo", JSON.stringify(data)).then((res) => {
      dd.redirectTo({
        url: "/pages/EqpRepair/EqpRepairInfo/EqpRepairInfo?activeTab=0"
      })
    });


  }

});
