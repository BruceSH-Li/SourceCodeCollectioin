var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    locationCode: "",
    hidden: true,
    arrlocationIndex: 0,
    arrOutIndex: 0,
    objectEqpArray: [],
    arrEqpIndex: 0,
    arrIndex: 0,
  },
  onLoad() { 
  },
    Scan_transferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          transferCode: res.code,
          hidden: true
        });
        if (!res.code) return;
        this.GetOutputMaterialBandingByCode(res.code);
      }
    });
  },
  search(event) {
    this.setData({
      transferCode: event.detail.value,
      hidden: true
    });
    if (!!this.data.transferCode)
    {
        this.GetOutputMaterialBandingByCode(this.data.transferCode);
    }
  },
    GetOutputMaterialBandingByCode: function (code) {
    var that = this;
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/WorkRun/GetOutputMaterialBandingByCode?code=" + code,
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
              entity: res.data.data,
              hidden: false,
              eqpName:res.data.data.F_EqpName,
            });
           this.GetLocation();

          } else {
            dd.alert({ title: '提示', content: "不存在和流转箱匹配的信息！", buttonText: '我知道了', });
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
  GetLocation() {
    var api=encodeURI("/api/Common/GetLocationList?eqpName=" + this.data.eqpName+"&materialCode="+this.data.entity.F_MaterialCode);
    myRequest.get(api).then((res) => {
      if (res.data.data && res.data.data.length > 0) {
        this.setData({
          objectOutArray:res.data.data,
          objectlocationArray: res.data.data[0].list,
          locationCode:res.data.data[0].list[0].F_LocationCode,
          arrlocationIndex: 0,
          arrOutIndex: 0,
        });
      } else {
        dd.alert({ title: '提示', content: "无可用库位", buttonText: '我知道了', });
      }
    })    
  },
  GetAllLocation() {
    var api=encodeURI("/api/Common/GetLocationList?eqpName=" +  this.data.eqpName+"&materialCode="+this.data.entity.F_MaterialCode+"&isAll=true");
    myRequest.get(api).then((res) => {
      if (res.data.data && res.data.data.length > 0) {
        this.setData({
          objectOutArray:res.data.data,
          objectlocationArray: res.data.data[0].list,
          locationCode:res.data.data[0].list[0].F_LocationCode,
          arrlocationIndex: 0,
          arrOutIndex: 0,
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
  bindObjPickerOutChange(e) {
    this.setData({
      arrOutIndex: e.detail.value,
      objectlocationArray: this.data.objectOutArray[e.detail.value].list,
      locationCode:this.data.objectOutArray[e.detail.value].list[0].F_LocationCode,
      arrlocationIndex: 0,
    });
  },
  onSubmit:throttle(function(e) {
    myRequest.post("/api/WorkRun/OutputMaterialLocation", JSON.stringify(this.data)).then();

  }),
});
