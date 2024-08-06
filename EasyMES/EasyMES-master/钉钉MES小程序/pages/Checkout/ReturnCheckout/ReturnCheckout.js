var app = getApp();
var timer;
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    batchCode: "",
    propList:[],  // 列表数据
    storageHidden: true,
    arrIndex:0,
    objectArray:[],
    arrTypeIndex:0,
    objectTypeArray:[],
    hidden:true
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
      myRequest.get("/api/Checkout/GetReturnInfo").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          that.setData({
            storageHidden: true,
            objectTypeArray:res.data.data,
            objectArray: res.data.data[that.data.arrTypeIndex].list,
            batchCode:res.data.data[that.data.arrTypeIndex].list[0].F_BatchCode
          });
          if (!!that.data.batchCode)
          {
            that.GetCheckoutInfo(that.data.batchCode); 
          }
          if (!!this.data.transferCode) {
            this.GetReturnInfo(this.data.transferCode);
          }
      } else {
        dd.alert({ title: '提示', content: "无需要返库的物料", buttonText: '我知道了', });
      }
    })  
   },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      batchCode:this.data.objectArray[e.detail.value].F_BatchCode,
    });
    if (!!this.data.batchCode)
    {   
      this.GetCheckoutInfo(this.data.batchCode); 
    }
  },
  bindObjPickerTypeChange(e) {
    this.setData({
      arrTypeIndex: e.detail.value,
      objectArray: this.data.objectTypeArray[e.detail.value].list,
      batchCode:this.data.objectTypeArray[e.detail.value].list[0].F_BatchCode,
      arrIndex: 0,
    });
    if (!!this.data.batchCode)
    {
      this.GetCheckoutInfo(this.data.batchCode); 
    }
  },
  Scan_transferCode(event) {
    clearTimeout(timer);
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          transferCode: res.code
        });
        if (!!this.data.transferCode) {
          this.GetReturnInfo(this.data.transferCode);
        }
      }
    });
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
  transfersearch(event) {
    timer = setTimeout(() => {
      if (event.detail.value == this.data.transferCode) return;
      this.setData({
        transferCode: event.detail.value
      });
      if (!!this.data.transferCode) {
        this.GetReturnInfo(this.data.transferCode);
      }
    }, 100);
  },
  onSubmit:throttle(function(e) {
    var that=this;
    var data=that.data;
    data.num=e.detail.value.num;
    data.transferCode=e.detail.value.transferCode;
    data.isProduce=e.detail.value.isProduce=="1"?true:false;
    myRequest.postWithoutBack("/api/Checkout/ReturnCheckout", JSON.stringify(data)).then((res)=>{
      that.setData({
        batchCode: "",
        propList:[],  // 列表数据
        storageHidden: true,
        arrIndex:0,
        objectArray:[],
      })  
      that.onLoad();
    });
  }),
  GetCheckoutInfo: function (code) {
    myRequest.get("/api/Checkout/GetReturnScapMaterialInfoByCode", { "code": code }).then((res) => {
      this.setData({
        entity: res.data.data,
        propList: res.data.data.list,  // 列表数据
        storageHidden: false,
        num:res.data.data.F_OtherNum
      });
    })
  },

  GetReturnInfo: function (code) {
    myRequest.get("/api/Checkout/GetInfoByCode", { "code": code }).then((res) => {
      this.setData({
        returnentity: res.data.data,
        hidden: false
      });
    })
  }

});
