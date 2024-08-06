var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    instorageHidden: true
  },
  onLoad() { },
  Scan_transferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          transferCode: res.code,
          instorageHidden: true
        });
        if (!res.code) return;
        this.GetInStorageBandingByCode(this.data.transferCode);
      }
    });
  },
    search(event) {
    this.setData({
      transferCode: event.detail.value,
      instorageHidden: true
    });
    if (!!this.data.transferCode)
    {
      this.GetInStorageBandingByCode(this.data.transferCode);
    }
  },
  GetInStorageBandingByCode(code)
  {
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
                  F_MaterialBatch: res.data.data.F_MaterialBatch,
                  F_Num: res.data.data.F_Num,
                  F_BandingUserName: res.data.data.F_BandingUserName,
                  instorageHidden: false
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
  onSubmit:throttle(function(e) {
    myRequest.post("/api/InStorage/InStorageCancleBanding?code="+e.detail.value.transferCode).then();

  })
});
