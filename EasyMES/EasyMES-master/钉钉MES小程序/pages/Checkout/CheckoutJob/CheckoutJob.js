var app = getApp();
var timer;
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    storageHidden: true
  },
  onLoad() { },
  Scan_transferCode(event) {
    clearTimeout(timer);
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          transferCode: res.code,
          storageHidden: true
        });

        if (!res.code) return;
        this.GetStorageByCode(res.code);


      }
    });
  },
  search(event) {
    timer = setTimeout(() => {
      if (event.detail.value == this.data.transferCode) return;
      this.setData({
        transferCode: event.detail.value,
        storageHidden: true
      });
      if (!!this.data.transferCode) {
        this.GetStorageByCode(this.data.transferCode);
      }
    }, 100);
  },
  onSubmit:throttle(function(e) {
    var that=this;
    myRequest.postWithoutBack("/api/Checkout/NeedCheckJob?code="+e.detail.value.transferCode).then((res)=>{
      that.setData({
        transferCode: "",
        storageHidden: true
      });
    });
  }),
  GetStorageByCode: function (code) {
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/Checkout/GetInfoByCode?code=" + code,
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
          this.setData({
            entity: res.data.data,
            storageHidden: false
          });

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
