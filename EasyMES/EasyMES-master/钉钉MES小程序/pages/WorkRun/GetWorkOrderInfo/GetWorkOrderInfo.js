var app = getApp();
var myRequest = require('/static/js/myRequest.js');
Page({
  data: {
    eqpName: "",
    materialCode: "",
    num: 0,
    isShow: false,    
    objectEqpArray: [],
    arrEqpIndex: 0,
  },
  onLoad() { 
    myRequest.get("/api/WorkRun/GetUsingEqpList").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          this.setData({
            eqpName:res.data.data[0].F_EqpName,
            isShow: true,
            objectEqpArray: res.data.data,
          });
          if (!!this.data.eqpName)
          {
              this.GetWorkOrder(this.data.eqpName);
          }
      } else {
        dd.alert({ title: '提示', content: "无使用中的设备", buttonText: '我知道了', });
      }
    })  
  },
  bindObjPickerEqpChange(e) {
    this.setData({
      arrEqpIndex: e.detail.value,
      eqpName:this.data.objectEqpArray[e.detail.value].F_EqpName,
      isShow: true
    });
    if (!!this.data.eqpName)
    {
        this.GetWorkOrder(this.data.eqpName);
    }
  },
  toggle() {
    this.setData({
      isShow: !this.data.isShow
    })
  },
  GetWorkOrder: function (code) {
    var that = this;
    var api=encodeURI(app.globalData .apiurl + "/api/WorkRun/GetWorkOrder?eqpName=" + code);
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: api,
      method: 'GET',
      dataType: 'json',
      success: (res) => {
        if (res.data.state == 'error' && res.data.code == 21333) {
          dd.alert({
            title: '提示',
            content: '你的登入信息已过期，请重新登入',
            buttonText: '重新登入',
            success: () => {
              app.getUserInfo();

            }
          });
        } else if (res.data.state == 'success') {
          if (res.data.count > 0) {
            that.setData({
              workOrderList: res.data.data,
              isShow: true
            });

          } else {
            that.setData({
              workOrderList: [],
              isShow: true
            });
          }

        }
        else {
          dd.alert({ title: '后台查询失败', content: res.data.message, buttonText: '我知道了', });
        }
      },
      fail: function (res) {
        dd.alert({ title: '查询失败', content: res.errorMessage+":"+res.status, buttonText: '我知道了', });
      }
    })
  },


});
