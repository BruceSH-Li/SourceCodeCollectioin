var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferBoxCode: "",
    batchCode: "",
    descriptionHidden: true,
    storageHidden: true,
    propList: [],  // 列表数据
    arrTypeIndex:0,
    objectTypeArray:[],
    arrIndex:0,
    objectArray:[],
    btnHidden:true,
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
    myRequest.get("/api/Checkout/GetAllInfo").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          that.setData({
            storageHidden: true,
            objectTypeArray: res.data.data,
            arrIndex: 0,
            objectArray: res.data.data[0].list,
            transferBoxCode:res.data.data[0].list[0].F_TransferBoxCode
          });
          if (!!that.data.transferBoxCode)
          {
            that.GetCheckoutInfo(that.data.transferBoxCode); 
          }
      } else {
        dd.alert({ title: '提示', content: "无需要检验的物料", buttonText: '我知道了', });
      }
    })  
  },
  onShow() {
    // 页面显示
    var that = this;
    if (!!that.data.batchCode)
    {
      dd.getStorage({
        key: that.data.batchCode,
        success: (res) => {
          if (res.data) {
            that.setData({
              propList: res.data.list,
            })
          }
        }
      });
    }
  },
  onHide() {
    dd.setStorage({
      key: this.data.batchCode,
      data: {
          list: this.data.propList,
        },
    });
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      transferBoxCode:this.data.objectArray[e.detail.value].F_TransferBoxCode,
    });
    if (!!this.data.transferBoxCode)
    {   
      this.GetCheckoutInfo(this.data.transferBoxCode); 
    }
  },
  bindObjPickerTypeChange(e) {
    this.setData({
      arrTypeIndex: e.detail.value,
      objectArray: this.data.objectTypeArray[e.detail.value].list,
      transferBoxCode:this.data.objectTypeArray[e.detail.value].list[0].F_TransferBoxCode,
      arrIndex: 0,
    });
    if (!!this.data.transferBoxCode)
    {
      this.GetCheckoutInfo(this.data.transferBoxCode); 
    }

  },
  Scan_batchCode(event) {
    var that = this;
    dd.scan({
      type: 'qr',
      success: (res) => {
        that.setData({
          batchCode: res.code,
          btnHidden: false
        });
        if (!!that.data.batchCode)
        {
          dd.getStorage({
            key: that.data.batchCode,
            success: (res) => {
              if (res.data) {
                that.setData({
                  propList: res.data.list,
                })
              }
              else
              {
                that.setData({
                  propList: [],
                })
              }
            }
          });
        }
      },
      fail: function(res){
        that.setData({
          propList: [],
        })
      }
    });
  },
  search(event) {
    if (!!event.detail.value)
    {
      this.setData({
        batchCode: event.detail.value,
        btnHidden: false
      });
      dd.getStorage({
        key: this.data.batchCode,
        success: (res) => {
          if (res.data) {
            this.setData({
              propList: res.data.list,
            })
          }
          else
          {
            this.setData({
              propList: [],
            })
          }
        },
        fail: function(res){
          this.setData({
            propList: [],
          })
        }
      });
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
  AddScapImage() {
      dd.navigateTo({
        url: '/pages/Checkout/AddScapImage/AddScapImage?batchCode='+this.data.batchCode
      })
  },
  LookScapImage(event) {
    dd.previewImage({
      current: 0,
      urls: [event.target.dataset.attr],
    });
  },
  DeleteScapImage(event) {
    var that = this;
    dd.confirm({
      title: '提示', content: "是否删除缺陷信息", confirmButtonText: '确定',cancelButtonText:'取消',
      success: (result) => {
        if (result.confirm)
        {
          that.data.propList.splice(event.target.dataset.attr, 1);
          that.setData({
            propList:that.data.propList,
          });
        }
      }
    });
  },
  onSubmit:throttle(function(e) {
    var that=this;
    if(!that.data.batchCode)
    {
      dd.confirm({
          title: '提示', content: "检验完成确认？", confirmButtonText: '确定',cancelButtonText:'取消',
          success: (result) => {
            if (result.confirm)
            {
              myRequest.post("/api/Checkout/CheckoutForm", JSON.stringify(that.data)).then();
            }
        }
      });
    }
    else  
    {
        if(that.data.propList.length == 0)
        {
          dd.alert({ title: '提示', content: "请录入缺陷", buttonText: '我知道了', });
          return false;
        }
        var data = that.data;
        data.num=e.detail.value.num;
        myRequest.postWithoutBack("/api/Checkout/CheckoutForm", JSON.stringify(data)).then((res)=>{
            that.GetCheckoutInfo(data.transferBoxCode); 
            that.setData({
              batchCode: '',
              descriptionHidden: true,
              propList: [],
            })
        });
      }
  }),
  GetCheckoutInfo: function (code) {
    myRequest.get("/api/Checkout/GetWaitCheckoutInfoByCode", { "code": code }).then((res) => {
      if (res.data.data) {
        this.setData({
          entity:res.data.data,
          storageHidden: false
        });

      } else {
        dd.alert({ title: '提示', content: "不存在和此流转箱匹配的信息！", buttonText: '我知道了', });
      }
    })

  }

});
