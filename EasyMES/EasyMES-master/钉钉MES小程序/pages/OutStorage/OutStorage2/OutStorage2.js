var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    num: 0,
    description: "",
    outStorageCode: "",
    storageHidden: true,
    entityHidden: true,
    arrIndex: 0,
    numHidden:true,
    readyhidden:true,
  },
  onLoad() { 
    myRequest.get("/api/OutStorage/GetOutStorageList").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          this.setData({
            outStorageCode:res.data.data[0].F_OutStorageCode,
            storageHidden: true,
            objectArray: res.data.data,
          });
          this.GetOutStorageByCode(res.data.data[0].F_OutStorageCode);
      } else {
        dd.alert({ title: '提示', content: "无出库订单", buttonText: '我知道了', });
      }
    })  
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      outStorageCode:this.data.objectArray[e.detail.value].F_OutStorageCode,
      storageHidden: true
    });
    this.GetOutStorageByCode(this.data.objectArray[e.detail.value].F_OutStorageCode);
  },
  onSubmit:throttle(function(e) {
    if (Number(this.data.num)>this.data.unOutStorageNum)
    {
        dd.alert({ title: '提示', content: "数量大于所需数量，无法下架！", buttonText: '我知道了', });
        return false;
    }
    if(this.data.F_MaterialCode == this.data.entity.F_MaterialCode && this.data.F_MaterialCode)
    {
      myRequest.post("/api/OutStorage/OutStorage", JSON.stringify(this.data)).then();
    }
    else  
    {
      dd.alert({ title: '提示', content: "所需物料不一致，无法出库！", buttonText: '我知道了', });
    }    
  }),
  searchTargetTransferCode(event) {
    this.setData({
      TargetTransferCode: event.detail.value,
    });
  },
  numChange(event) {
    if(Number(event.detail.value)<this.data.num)
    {
      this.setData({
        numHidden:false
    });
    }  
    this.setData({
        num:event.detail.value
    });
  },
    Scan_transferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          transferCode: res.code
        });
        if (!!this.data.transferCode)
        {
            this.GetStorageByCode();
        }
      }
    });
  },
  search(event) {
    this.setData({
      transferCode: event.detail.value,
    });
    if (!!this.data.transferCode)
    {
        this.GetStorageByCode();
    }   
  },
   GetStorageByCode: function () {
    var that=this;
    var code = this.data.transferCode;
    var api=encodeURI("/api/Common/GetStorageByCode?code=" + code);
    myRequest.get(api).then((res) => {
      if (!!res.data.data) {
        that.setData({
          entity:res.data.data,
          entityHidden: false,
          num:res.data.data.F_Num
        });
      } else {
        that.setData({
          entity: {},
          entityHidden: true,
          num:0
        });
        dd.alert({ title: '提示', content: "不存在和此物料匹配的库存信息！", buttonText: '我知道了', });
      }
    })
  },
  Scan_TargetTransferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          TargetTransferCode: res.code
        });
      }
    });
  },
  GetAllStorage: function ()
  {
    this.GetStorageCode(true);
  },
  GetStorageBtn: function()
  {
    this.GetStorageCode(false);
  },
  GetStorageCode: function (isAll=false)
  {
    var code=this.data.F_MaterialCode;
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/Common/GetStorageListByMaterial?code=" + code+"&isAll=" + isAll,
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
          if (res.data.data&&res.data.data.length>0) {
            this.setData({
              readyEntity:res.data.data[0].list[0],
              readyHidden: false,
            });
          } else {
            dd.alert({ title: '提示', content: "不存在和此出库单匹配的信息！", buttonText: '我知道了', });
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
  GetOutStorageByCode: function (code) {
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/OutStorage/GetUnOutStorageByCode?keyword=" + code,
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
              outStorageCode: res.data.data.F_OutStorageCode,
              F_OutStorageCode: res.data.data.F_OutStorageCode,
              F_MaterialCode: res.data.data.F_MaterialCode,
              F_MaterialName: res.data.data.F_MaterialName,
              F_Num: res.data.data.F_Num,
              unOutStorageNum: res.data.data.F_Num - res.data.data.F_DoneNum,
              storageHidden: false
            });
            this.GetStorageCode();


          } else {
            dd.alert({ title: '提示', content: "不存在和此出库单匹配的信息！", buttonText: '我知道了', });
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
