var app = getApp();
var timer;
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    batchCode: "",
    isBack:0,
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
  onSubmit:throttle(function(e) {
    var that=this;
    var data=that.data;
    data.num=e.detail.value.num;
    myRequest.postWithoutBack("/api/Checkout/ChangeScapState", JSON.stringify(data)).then((res)=>{
      that.setData({
        batchCode: "",
        propList: [],  // 列表数据
        storageHidden: true
      });
    });
  }),
  LookImage(event) {
    var that=this;
    dd.previewImage({
      current: 0,
      urls: [
       that.data.mainurl+ event.target.dataset.attr,
      ],
    });
  },
  radioChange(e) {
    this.setData({
      isBack: e.detail.value,
    });
  },
  GetCheckoutInfo: function (code) {
    myRequest.get("/api/Checkout/GetScapMaterialInfoByCode", { "code": code,"isTemp":false }).then((res) => {
      this.setData({
        entity: res.data.data,
        propList: res.data.data.list,  // 列表数据
        storageHidden: false,
        num:res.data.data.F_OtherNum
      });
    })
  }

});
