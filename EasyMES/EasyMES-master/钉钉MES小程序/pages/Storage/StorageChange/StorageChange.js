var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    changeNum: null,
    newTransferCode: "",
    newLocationCode: "",
    description: "",
    storageHidden: true,
    hidden: true,
    arrlocationIndex: 0,
    arrOutIndex: 0,
  },
  onLoad() { },
  GetLocation() {
    var api=encodeURI("/api/Common/GetLocationListByLocation?locationCode=" + this.data.F_LocationCode+"&materialCode="+this.data.F_MaterialCode);
    myRequest.get(api).then((res) => {
      if (res.data.data && res.data.data.length > 0) {
        this.setData({
          objectOutArray:res.data.data,
          objectlocationArray: res.data.data[0].list,
          newLocationCode:res.data.data[0].list[0].F_LocationCode,
          arrlocationIndex: 0,
          arrOutIndex: 0,
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
          objectOutArray:res.data.data,
          objectlocationArray: res.data.data[0].list,
          newLocationCode:res.data.data[0].list[0].F_LocationCode,
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
      newLocationCode:this.data.objectlocationArray[e.detail.value].F_LocationCode,
    });
  },
  bindObjPickerOutChange(e) {
    this.setData({
      arrOutIndex: e.detail.value,
      objectlocationArray: this.data.objectOutArray[e.detail.value].list,
      newLocationCode:this.data.objectOutArray[e.detail.value].list[0].F_LocationCode,
      arrlocationIndex: 0,
    });
  },
  Scan_transferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          transferCode: res.code,
          storageHidden: true
        });

        if (!res.code) return;
        GetStorageByCode(res.code);


      }
    });
  },
    search(event) {
    this.setData({
      transferCode: event.detail.value,
      storageHidden: true
    });
    if (!!this.data.transferCode)
    {
      this.GetStorageByCode(this.data.transferCode);
    }
  },
  GetStorageByCode(code){
    dd.httpRequest({
          headers: {
            "Content-Type": "application/json",
            "WC-Token": app.globalData .apitoken
          },
          url: app.globalData .apiurl + "/api/Common/GetStorageByCode?code=" + code,
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
                  F_MaterialCode: res.data.data.F_MaterialCode,
                  F_MaterialName: res.data.data.F_MaterialName,
                  F_MaterialBatch: res.data.data.F_MaterialBatch,
                  changeNum: res.data.data.F_Num,
                  F_LocationCode: res.data.data.F_LocationCode,
                  newLocationCode:res.data.data.F_LocationCode,
                  transferCode:  res.data.data.F_TransferboxCode,
                  storageHidden: false
                });



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
  Scan_newTransferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          newTransferCode: res.code,
          hidden:false
        });
      }
    });
  },
  newsearch(event) {
    this.setData({
      newTransferCode: event.detail.value,
    });
    if (!!this.data.newTransferCode)
    {
        this.setData({
          hidden:false
        });
    }
  },
  onSubmit:throttle(function(e) {
    var data=e.detail.value;
    data.changeType = 0;
    data.newLocationCode=this.data.newLocationCode;
    myRequest.post("/api/Common/StorageChange", JSON.stringify(data)).then();

  })
});
