var app = getApp();
var timer;
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    batchCode: "",
    propList: [],  // 列表数据
    storageHidden: true
  },
  onLoad() {
    var that=this;
    dd.getStorage({
        key: 'userInfo',
        success: (res) => {
          if (res.data) {
            that.setData({
              mainurl: res.data.mainurl,
            })
          }
        }
      });

   },
  Scan_batchCode(event) {
    clearTimeout(timer);
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          batchCode: res.code,
          storageHidden: true
        });

        if (!res.code) return;
        this.GetCheckoutInfo(res.code);


      }
    });
  },
  search(event) {
    timer = setTimeout(() => {
      if (event.detail.value == this.data.batchCode) return;
      this.setData({
        batchCode: event.detail.value,
        storageHidden: true
      });
      if (!!this.data.batchCode) {
        this.GetCheckoutInfo(this.data.batchCode);
      }
    }, 100);
  },
  LookImage(event) {
    var that=this;
    dd.previewImage({
      current: 0,
      urls: [
       that.data.mainurl+ event.target.dataset.attr,
      ],
    });
  },
  onSubmit:throttle(function(e) {
    var that=this;
    myRequest.postWithoutBack("/api/Checkout/CheckoutCancle?code="+e.detail.value.batchCode).then((res)=>{
      that.setData({
        batchCode: "",
        propList: [],  // 列表数据
        storageHidden: true
      })
    });
  }),
  GetCheckoutInfo: function (code) {
    myRequest.get("/api/Checkout/GetScapMaterialInfoByCode", { "code": code,"isTemp":true }).then((res) => {
      this.setData({
        entity: res.data.data,
        propList: res.data.data.list,  // 列表数据
        storageHidden: false
      });
    })
  }

});
