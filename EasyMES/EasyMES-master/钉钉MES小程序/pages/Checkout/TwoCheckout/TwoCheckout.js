var app = getApp();
var timer;
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    batchCode: "",
    isBack:0,
    propList: [],  // 列表数据
    storageHidden: true,
    arrTypeIndex:0,
    objectTypeArray:[],
    arrIndex:0,
    objectArray:[],
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
    myRequest.get("/api/Checkout/GetTwoAllInfo").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          that.setData({
            storageHidden: true,
            objectTypeArray: res.data.data,
            arrTypeIndex: 0,
            arrIndex: 0,
            objectArray: res.data.data[0].list,
            batchCode:res.data.data[0].list[0].F_BatchCode,
            propList: [],  // 列表数据
          });
          if (!!that.data.batchCode)
          {
            that.GetCheckoutInfo(that.data.batchCode); 
          }
      } else {
        that.setData({
          storageHidden: true,
          objectTypeArray: [],
          arrTypeIndex: 0,
          arrIndex: 0,
          objectArray: [],
          batchCode:"",
          propList: [],  // 列表数据
        });
        dd.alert({ title: '提示', content: "无需要检验的物料", buttonText: '我知道了', });
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
  onSubmit:throttle(function(e) {
    var that=this;
    var data=that.data;
    data.num=e.detail.value.num;
    myRequest.postWithoutBack("/api/Checkout/TwoCheckoutForm", JSON.stringify(data)).then((res)=>{
      that.onLoad();
    });
  }),
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
