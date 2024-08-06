var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    outStorageCode: "",
    objectArray: [],
    arrIndex: 0,
    objectOutArray: [],
    arrOutIndex: 0,
    storageHidden: true,
    arrlocationIndex: 0,
    arrCodeIndex: 0,
  },
onLoad() { 
    myRequest.get("/api/OutStorage/GetOutStorageBandingList").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          this.setData({
            outStorageCode:res.data.data[0].F_OutStorageCode,
            storageHidden: true,
            objectOutArray: res.data.data,
          });
          this.GetOutStorageBandingByCode(res.data.data[0].F_OutStorageCode);
      } else {
        dd.alert({ title: '提示', content: "无出库订单", buttonText: '我知道了', });
      }
    })  
  },
    GetLocation() {
    var api=encodeURI("/api/Common/GetLocationListByLocation?locationCode=" + this.data.F_LocationCode+"&materialCode="+this.data.F_MaterialCode);
    myRequest.get(api).then((res) => {
      if (res.data.data && res.data.data.length > 0) {
        this.setData({
          objectCodeArray:res.data.data,
          objectlocationArray: res.data.data[0].list,
          locationCode:res.data.data[0].list[0].F_LocationCode,
          arrlocationIndex: 0,
          arrCodeIndex: 0,
        });
      } else {
        dd.alert({ title: '提示', content: "无可用库位", buttonText: '我知道了', });
      }
    })    
  },
  GetAllLocation() {
    var api=encodeURI("/api/Common/GetLocationListByLocation?locationCode=" +  this.data.F_LocationCode+"&materialCode="+this.data.F_MaterialCode+"&isAll=true");
    myRequest.get(api).then((res) => {
      if (res.data.data && res.data.data.length > 0) {
        this.setData({
          objectCodeArray:res.data.data,
          objectlocationArray: res.data.data[0].list,
          locationCode:res.data.data[0].list[0].F_LocationCode,
          arrlocationIndex: 0,
          arrCodeIndex: 0,
        });
      } else {
        dd.alert({ title: '提示', content: "无可用库位", buttonText: '我知道了', });
      }
    })    
  },
    bindObjPickerlocationChange(e) {
    this.setData({
      arrlocationIndex: e.detail.value,
      locationCode:this.data.objectlocationArray[e.detail.value].F_LocationCode,
    });
  },
  bindObjPickerCodeChange(e) {
    this.setData({
      arrCodeIndex: e.detail.value,
      objectlocationArray: this.data.objectCodeArray[e.detail.value].list,
      locationCode:this.data.objectCodeArray[e.detail.value].list[0].F_LocationCode,
      arrlocationIndex: 0,
    });
  },
  bindObjPickerOutChange(e) {
    this.setData({
      arrOutIndex: e.detail.value,
      outStorageCode:this.data.objectOutArray[e.detail.value].F_OutStorageCode,
      hidden: true
    });
    this.GetOutStorageBandingByCode(this.data.outStorageCode);
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      transferCode:this.data.objectArray[e.detail.value].F_TransferBoxCode,
      F_MaterialCode: this.data.objectArray[e.detail.value].F_MaterialCode,
      F_MaterialName: this.data.objectArray[e.detail.value].F_MaterialName,
      F_MaterialBatch: this.data.objectArray[e.detail.value].F_MaterialBatch,
      F_Num:this.data.objectArray[e.detail.value].F_Num,
      storageHidden: false
    });
  },
  onSubmit:throttle(function(e) {
    myRequest.post("/api/OutStorage/OutStorageCancle", JSON.stringify(this.data)).then();

  }),
  GetOutStorageBandingByCode: function (outStorageCode) {
    var that = this;
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/OutStorage/GetUnBandingInfoByCode?code=" + outStorageCode,
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
              objectArray: res.data.data,
            });
            if (res.data.data.length>0)
            {
              that.setData({
                F_MaterialCode: this.data.objectArray[0].F_MaterialCode,
                F_MaterialName: this.data.objectArray[0].F_MaterialName,
                F_MaterialBatch: this.data.objectArray[0].F_MaterialBatch,
                transferCode: this.data.objectArray[0].F_TransferBoxCode,
                F_Num:this.data.objectArray[0].F_Num,
                F_LocationCode:this.data.objectArray[0].F_LocationCode,
                storageHidden: false
              });
              that.GetLocation();
            } 
            else  
            {
               dd.alert({ title: '提示', content: "不存在与此出库单匹配的绑定信息！", buttonText: '我知道了', });
            }
          } else {
            dd.alert({ title: '提示', content: "不存在与此出库单和流转箱匹配的信息！", buttonText: '我知道了', });
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
  }
});
