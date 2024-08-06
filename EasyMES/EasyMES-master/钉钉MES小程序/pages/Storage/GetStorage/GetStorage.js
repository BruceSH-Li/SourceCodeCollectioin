var app = getApp();
var timer;
var myRequest = require('/static/js/myRequest.js');
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
        this.GetCheckoutInfo(res.code);


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
        this.GetCheckoutInfo(this.data.transferCode);
      }
    }, 100);
  },

  GetCheckoutInfo: function (code) {
    myRequest.get("/api/Checkout/GetInfoByCode", { "code": code }).then((res) => {
      this.setData({
        entity: res.data.data,
        storageHidden: false
      });
    })
  }

});
