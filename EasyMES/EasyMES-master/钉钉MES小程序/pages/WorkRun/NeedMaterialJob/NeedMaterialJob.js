var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';

Page({
  data: {
    objectArray: [],
    eqpName: "",
    materialCode: "",
    num: null,
    isShow: false,
    arrIndex: 0,
    objectEqpArray: [],
    arrEqpIndex: 0,
    containerNum:null,
    usehidden:true

  },
  onLoad() { 
    myRequest.get("/api/WorkRun/GetUsingEqpList").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          this.setData({
            eqpName:res.data.data[0].F_EqpName,
            storageHidden: true,
            objectEqpArray: res.data.data,
          });
          this.GetWorkOrder(this.data.eqpName);
          this.GetUseMaterial(this.data.eqpName);
      } else {
        dd.alert({ title: '提示', content: "无使用中的设备", buttonText: '我知道了', });
      }
    })  
  },
  bindObjPickerEqpChange(e) {
    this.setData({
      arrEqpIndex: e.detail.value,
      eqpName:this.data.objectEqpArray[e.detail.value].F_EqpName,
      hidden: true,
      usehidden:true,
      arrIndex: 0,
    });
    this.GetWorkOrder(this.data.eqpName);
    this.GetUseMaterial(this.data.eqpName);
  },
  // Scan_materialCode(event) {
  //   dd.scan({
  //     type: 'qr',
  //     success: (res) => {
  //       this.setData({
  //         materialCode: res.code
  //       });
  //     }
  //   });
  // },
  onSubmit:throttle(function(e) {
    this.data.num = e.detail.value.num;
    // if(this.data.num>this.data.containerNum)
    // {
    //     dd.alert({ title: '提示', content: "申请数量超出物料容器值", buttonText: '我知道了', });
    //     return false;
    // }
    myRequest.post("/api/WorkRun/NeedMaterialJob", JSON.stringify(this.data)).then();

  }),
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
            dd.alert({ title: '提示', content: "不存在和此设备匹配的信息！", buttonText: '我知道了', });
          }

        }
        else {
          dd.alert({ title: '后台查询失败', content: res.data.message, buttonText: '我知道了', });
        }
      },
      fail: function (res) {
        dd.alert({ title: '查询失败', content: res.errorMessage + ":" + res.status, buttonText: '我知道了', });
      }
    })
  },
  GetUseMaterial: function (code) {
    var that = this;
        var api=encodeURI( app.globalData .apiurl + "/api/WorkRun/GetUseMaterial?eqpName=" + code);
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
              objectArray: res.data.data,
              materialCode:res.data.data[0].F_MaterialCode,
              num:res.data.data[0].F_ContainerNum,
              containerNum:res.data.data[0].F_ContainerNum,
              useentity:res.data.data[0],
              usehidden:false

            });

          } else {
            // dd.alert({ title: '提示', content: "不存在和此设备匹配的信息！", buttonText: '我知道了', });
          }

        }
        else {
          dd.alert({ title: '后台查询失败', content: res.data.message, buttonText: '我知道了', });
        }
      },
      fail: function (res) {
        dd.alert({ title: '查询失败', content: res.errorMessage + ":" + res.status, buttonText: '我知道了', });
      }
    })
  },

  bindObjPickerChange(e) {
    console.log(e.detail.value);
    this.setData({
      arrIndex: e.detail.value,
      materialCode:this.data.objectArray[e.detail.value].F_MaterialCode,
      num: this.data.objectArray[e.detail.value].F_ContainerNum,
      containerNum:this.data.objectArray[e.detail.value].F_ContainerNum,
      useentity:this.data.objectArray[e.detail.value],
      usehidden:false
    });
  }

});
