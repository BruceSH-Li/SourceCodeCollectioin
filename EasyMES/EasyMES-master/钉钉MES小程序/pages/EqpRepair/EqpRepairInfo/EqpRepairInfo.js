var app = getApp();
var myRequest = require('/static/js/myRequest.js');
Page({
  data: {
    tabs: [
      {
        title: '待派工',
        subTitle: '',
        number: '0',
        showBadge: true,
        badge: {
          arrow: false,
          stroke: true,
        },
      },

      {
        title: '待接单',
        subTitle: '',
        number: '0',
        showBadge: true,
        badge: {
          arrow: false,
          stroke: true,
        },
      },
      {
        title: '维修中',
        subTitle: '',
        number: '0',
        showBadge: true,
        badge: {
          arrow: false,
          stroke: true,
        },
      },
      {
        title: '待确认',
        subTitle: '',
        number: '0',
        showBadge: true,
        badge: {
          arrow: true,
        },
      },

    ],
    activeTab: 0,

    typeCapsule: true,
    typeHasSubTitle: true,

    contentHeight: [
      { name: 'has', value: '是' },
      { name: 'hasnt', value: '否', checked: true },
    ],

    userid: "",
    username: "",
    IsAdminRole: false,
    noAdminRole: true,
    KPGquery: [],
    JDquery: [],
    BXquery: [],
    WXquery: [],
    QRquery: []

  },

  handleTabClick({ index, tabsName }) {
    this.setData({
      [tabsName]: index,
    });
  },
  handleTabChange({ index, tabsName }) {
    this.setData({
      [tabsName]: index,
    });
  },

  onLoad(query) {

    if (query.activeTab) {
      this.setData({
        activeTab: parseInt(query.activeTab)
      })
    }

    dd.getStorage({
      key: 'userInfo',
      success: (res) => {
        this.setData({
          userid: res.data.userid,
          username: res.data.username
        })
        this.getRepairInfoList();
      }
    })



  },

  onShow() {

  },

  DetailRepair(e) {
    var RepairId = e.currentTarget.dataset.F_RepairId
    dd.navigateTo({            //保留当前页面，跳转到应用内的某个指定页面
      url: "/pages/EqpRepair/EqpRepairDetail/EqpRepairDetail?RepairId=" + RepairId
    })
  },


  updateStatus(e) {
    var that = this;
    console.log(e.currentTarget.dataset.F_RepairId);
    console.log(e.currentTarget.dataset.type);
    var type = e.currentTarget.dataset.type;
    var RepairId = e.currentTarget.dataset.F_RepairId
    if (type == "PG") {
      dd.navigateTo({            //保留当前页面，跳转到应用内的某个指定页面
        url: "/pages/EqpRepair/EqpRepairDetail/EqpRepairDetail?type=" + type + "&RepairId=" + RepairId
      })
    }
    else if (type == "PGJD"||type=="ZZJD") {
      dd.confirm({
        title: '提示', content: "确认接单？", confirmButtonText: '确定', cancelButtonText: '取消',
        success: (result) => {
          if (result.confirm) {
            var data = {
              repairId: RepairId,
              type: type,
              currentMan: that.data.userid,
              currentManName: that.data.username,
              // repairManList:[{F_RepairMan:]
            }

            myRequest.postWithoutBack("/api/EqpRepair/UpdateEqpRepairInfo", JSON.stringify(data)).then((res) => {
              dd.redirectTo({
                url: "/pages/EqpRepair/EqpRepairInfo/EqpRepairInfo?activeTab=1"
              })

            });
          }
        }
      });
    }
    else if(type=="WX"){
      dd.prompt({
        title: '提示', message: "维修完成？", okButtonText: '确定', cancelButtonText: '取消',
        placeholder: '请输入维修结果',
        success: (result) => {
          if (result.ok) {
            var data = {
              repairId: RepairId,
              type: type,
              currentMan: that.data.userid,
              currentManName: that.data.username,
              repairResult:result.inputValue
              // repairManList:[{F_RepairMan:]
            }

            myRequest.postWithoutBack("/api/EqpRepair/UpdateEqpRepairInfo", JSON.stringify(data)).then((res) => {
              dd.redirectTo({
                url: "/pages/EqpRepair/EqpRepairInfo/EqpRepairInfo?activeTab=2"
              })

            });
          }
        }
      });
    }
    else if(type=="QR"){
      dd.confirm({
        title: '提示', content: "确认维修完成？", confirmButtonText: '确定', cancelButtonText: '取消',
        success: (result) => {
          if (result.confirm) {
            var data = {
              repairId: RepairId,
              type: type,
              currentMan: that.data.userid,
              currentManName: that.data.username,
              // repairManList:[{F_RepairMan:]
            }

            myRequest.postWithoutBack("/api/EqpRepair/UpdateEqpRepairInfo", JSON.stringify(data)).then((res) => {
              dd.redirectTo({
                url: "/pages/EqpRepair/EqpRepairInfo/EqpRepairInfo?activeTab=3"
              })

            });
          }
        }
      });
    }


  },

  getRepairInfoList: function (code) {
    var that = this;
    myRequest.get("/api/EqpRepair/getRepairInfoByDDUserId?ddUserId=" + that.data.userid).then((res) => {
      var result = res.data.data;
      that.setData({
        noAdminRole: !result.IsAdminRole
      });

      //派工
      if (result.KPGquery && result.KPGquery.length > 0) {
        that.setData({
          'tabs[0].number': result.KPGquery.length,
          KPGquery: result.KPGquery
        })
      } else {
        that.setData({
          pgNodatadispaly: true
        })
      }

      //接单
      if (result.JDquery && result.JDquery.length > 0) {
        that.setData({
          'tabs[1].number': result.JDquery.length,
          JDquery: result.JDquery
        })
      } else {
        that.setData({
          jdNodatadispaly: true
        })
      }

      if (result.BXquery && result.BXquery.length > 0) {
        that.setData({
          BXquery: result.BXquery
        })
      }

      //维修中
      if (result.WXquery && result.WXquery.length > 0) {
        that.setData({
          'tabs[2].number': result.WXquery.length,
          WXquery: result.WXquery
        })
      } else {
        that.setData({
          wxNodatadispaly: true
        })
      }
      //确认
      if (result.QRquery && result.QRquery.length > 0) {
        that.setData({
          'tabs[3].number': result.QRquery.length,
          QRquery: result.QRquery
        })
      } else {
        that.setData({
          qrNodatadispaly: true
        })
      }


    })
  },



});
