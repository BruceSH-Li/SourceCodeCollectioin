var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    instorageHidden: true,
    areaArrIndex: 0,
    arrIndex: 0,
    arrOutIndex: 0,
    arrAreaIndex: 0,
  },
  onLoad() { 
  
  },
  GetArea() {
    var code=this.data.F_MaterialType;
    myRequest.get("/api/Common/GetAreaList?type="+code).then((res) => {
      if (res.data.data) {
        if (res.data.data.length>0)
        {
          this.setData({
            objectAreaArray: res.data.data,
            F_AreaCode:res.data.data[this.data.arrAreaIndex].F_AreaCode
          });
          this.GetLocation();
        }
      } else {
        dd.alert({ title: '提示', content: "无库区", buttonText: '我知道了', });
      }
    }) 
  },
  GetLocation() {
    var code=this.data.F_AreaCode;
    var api=encodeURI("/api/Common/GetLocationList?areaCode=" + code+"&materialCode="+this.data.F_MaterialCode);
    myRequest.get(api).then((res) => {
      if (res.data.data && res.data.data.length > 0) {
        this.setData({
          objectOutArray:res.data.data,
          objectArray: res.data.data[0].list,
          locationCode:res.data.data[0].list[0].F_LocationCode,
          arrIndex: 0,
          arrOutIndex: 0,
        });
      } else {
        dd.alert({ title: '提示', content: "无可用库位", buttonText: '我知道了', });
      }
    })    
  },
  GetAllLocation() {
    var code=this.data.F_AreaCode;
    var api=encodeURI("/api/Common/GetLocationList?areaCode=" + code+"&isAll=true"+"&materialCode="+this.data.F_MaterialCode);
    myRequest.get(api).then((res) => {
      if (res.data.data && res.data.data.length > 0) {
        this.setData({
          objectOutArray:res.data.data,
          objectArray: res.data.data[0].list,
          locationCode:res.data.data[0].list[0].F_LocationCode,
          arrIndex: 0,
          arrOutIndex: 0,
        });
      } else {
        dd.alert({ title: '提示', content: "无可用库位", buttonText: '我知道了', });
      }
    })    
  },
  Scan_transferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          transferCode: res.code,
          instorageHidden: true
        });

        if (!res.code) return;
        this.GetInStorageBandingByCode(res.code);
      }
    });
  },
  search(event) {
    this.setData({
      transferCode: event.detail.value,
      instorageHidden: true
    });
    if (!!this.data.transferCode) {
      this.GetInStorageBandingByCode(this.data.transferCode);
    }
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      locationCode:this.data.objectArray[e.detail.value].F_LocationCode,
      hidden: true
    });
  },
  bindObjPickerOutChange(e) {
    this.setData({
      arrOutIndex: e.detail.value,
      objectArray: this.data.objectOutArray[e.detail.value].list,
      hidden: true,
      locationCode:this.data.objectOutArray[e.detail.value].list[0].F_LocationCode,
      arrIndex: 0,
    });
  },
  bindObjPickerAreaChange(e) {
    this.setData({
      arrAreaIndex: e.detail.value,
      hidden: true
    });
    this.GetLocation();
  },
  GetInStorageBandingByCode(code) {
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/InStorage/GetInStorageBandingByCode?code=" + code,
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
            this.setData({
              F_InStorageCode: res.data.data.F_InStorageCode,
              F_MaterialCode: res.data.data.F_MaterialCode,
              F_MaterialName: res.data.data.F_MaterialName,
              F_MaterialType: res.data.data.F_MaterialType,
              F_MaterialBatch: res.data.data.F_MaterialBatch,
              F_Num: res.data.data.F_Num,
              F_BandingUserName: res.data.data.F_BandingUserName,
              instorageHidden: false
            });
            this.GetArea();
          } else {
            dd.alert({ title: '提示', content: "不存在和此流转箱匹配的信息！", buttonText: '我知道了', });
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
  onSubmit:throttle(function(e) {
    var data=e.detail.value;
    data.locationCode=this.data.locationCode;
    myRequest.post("/api/InStorage/InStorageLocation", JSON.stringify(data)).then();
  })
});
