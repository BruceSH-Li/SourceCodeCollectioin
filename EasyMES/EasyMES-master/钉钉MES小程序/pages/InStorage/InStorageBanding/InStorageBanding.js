var app = getApp();
var myRequest = require('/static/js/myRequest.js');
var until = require('/static/js/webutil.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    materialCode: "",
    materialName: "",
    num: null,
    description: "",
    inStorageCode: "",
    instorageHidden: true,
    arrIndex: 0,
  },
  onLoad() { 
      // this.setData({
      //   materialBatch: until.formatTime(new Date(),"YYYYMMDDhhmm"),
      // });
      myRequest.get("/api/InStorage/GetInStorageList").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          this.setData({
            inStorageCode:res.data.data[0].F_InStorageCode,
            instorageHidden: true,
            objectArray: res.data.data,
          });
          this.GetInStorageByCode(this.data.inStorageCode);
      } else {
        dd.alert({ title: '提示', content: "无入库订单", buttonText: '我知道了', });
      }
    })  
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      inStorageCode:this.data.objectArray[e.detail.value].F_InStorageCode,
      hidden: true
    });
    this.GetInStorageByCode(this.data.inStorageCode);
  },
  Scan_transferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          transferCode: res.code
        });
      }
    });
  },
  Scan_materialBatch(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          materialBatch: res.code
        });
      }
    });
  },
  onSubmit:throttle(function(e) {
    var data=this.data;
    data.num=e.detail.value.num;
    data.materialBatch=e.detail.value.materialBatch;
    data.transferCode=e.detail.value.transferCode;
    myRequest.post("/api/InStorage/InStorageBanding",JSON.stringify(data)).then();
  }),

  GetInStorageByCode:function(code){
    var that=this;
     dd.httpRequest({
          headers: {
            "Content-Type": "application/json",
            "WC-Token": app.globalData .apitoken
          },
          url: app.globalData .apiurl + "/api/InStorage/GetUnInStorageByCode?keyword=" + code,
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
                  // onload();
                }
              });
            } else if (res.data.state == 'success') {
              if (res.data.data) {
                that.setData({
                  inStorageCode:res.data.data.F_InStorageCode,
                  plannum: res.data.data.F_Num,
                  unbandingnum: res.data.data.F_Num - res.data.data.F_DoneNum,
                  materialCode:res.data.data.F_MaterialCode,
                  materialName:res.data.data.F_MaterialName,
                  num:res.data.data.F_ContainerNum,
                  instorageHidden: false
                });




              } else {
                dd.alert({ title: '提示', content: "不存在和此入库单号匹配的信息！", buttonText: '我知道了', });
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
  }



});
