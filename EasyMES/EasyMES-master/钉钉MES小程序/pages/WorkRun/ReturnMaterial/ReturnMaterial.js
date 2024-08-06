var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    eqpName: "",
    transferCode: "",
    locationCode: "",
    description: "",
    newTransferCode: "",
    hidden: true,
    objectEqpArray: [],
    arrEqpIndex: 0,
    arrIndex: 0,
    arrlocationIndex: 0,
    arrOutIndex: 0,
  },
  onLoad() { 
    myRequest.get("/api/WorkRun/GetHaveUseEqpList").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          this.setData({
            eqpName:res.data.data[0].F_EqpName,
            storageHidden: true,
            objectEqpArray: res.data.data,
          });
          if ((!this.data.eqpName)) return;
          this.GetUseTransferList(this.data.eqpName);
      } else {
        dd.alert({ title: '提示', content: "无使用中的设备", buttonText: '我知道了', });
      }
    })  
  },
  GetLocation() {
    var api=encodeURI("/api/Common/GetLocationListByLocation?locationCode=" + (!!this.data.entity.F_LocationCode?this.data.entity.F_LocationCode:"RA101")+"&materialCode="+this.data.entity.F_MaterialCode);
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
    var api=encodeURI("/api/Common/GetLocationListByLocation?locationCode=" +  (!!this.data.entity.F_LocationCode?this.data.entity.F_LocationCode:"RA101")+"&materialCode="+this.data.entity.F_MaterialCode+"&isAll=true");
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
  bindObjPickerEqpChange(e) {
    this.setData({
      arrEqpIndex: e.detail.value,
      eqpName:this.data.objectEqpArray[e.detail.value].F_EqpName,
      arrIndex:0,
      hidden: true
    });
    if ((!this.data.eqpName)) return;
    this.GetUseTransferList(this.data.eqpName);
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      transferCode:this.data.objectArray[e.detail.value].F_TransferBoxCode,
      hidden: true
    });
    if ((!this.data.eqpName)||(!this.data.transferCode)) return;
        this.GetConsumeMaterialByCode(this.data.transferCode, this.data.eqpName);
  },
  Scan_locationCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          locationCode: res.code
        });
      }
    });
  },
  Scan_newTransferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          newTransferCode: res.code
        });
      }
    });
  },
  onSubmit:throttle(function(e) {
    var data=this.data;
    myRequest.post("/api/WorkRun/ReturnMaterial", JSON.stringify(data)).then();

  }),
  GetConsumeMaterialByCode: function (transfercode, eqpName) {
    var that = this;
    var api=encodeURI(app.globalData .apiurl + "/api/WorkRun/GetConsumeMaterialByCode?code=" + transfercode + "&eqpName=" + eqpName);
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
              // onload();
            }
          });
        } else if (res.data.state == 'success') {
          if (res.data.data) {
            that.setData({
              entity: res.data.data,
              hidden: false
            });
          that.GetLocation();

          } else {
            dd.alert({ title: '提示', content: "不存在与此设备和流转箱匹配的信息！", buttonText: '我知道了', });
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
  GetUseTransferList: function (eqpName) {
    var that = this;
    var api=encodeURI(app.globalData .apiurl + "/api/WorkRun/GetUseTransferList?eqpName=" + eqpName);
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url:api,
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
          if (res.data.data && res.data.data.length>0) {
            that.setData({
              transferCode:res.data.data[0].F_TransferBoxCode,
              storageHidden: true,
              objectArray: res.data.data,
            });
            that.GetConsumeMaterialByCode(that.data.transferCode, that.data.eqpName);

          } else {
            dd.alert({ title: '提示', content: "不存在与此设备和流转箱匹配的信息！", buttonText: '我知道了', });
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
