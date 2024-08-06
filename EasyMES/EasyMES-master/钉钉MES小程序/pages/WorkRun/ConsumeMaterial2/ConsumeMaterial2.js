var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    eqpName: "",
    TargetTransferCode: "",
    num: null,
    description: "",
    objectArray: [],
    arrIndex: 0,
    hidden: true,    
    objectEqpArray: [],
    arrEqpIndex: 0,
    usehidden:true,
    numHidden:true,
    readyhidden:true,
  },
  onLoad() { 
    myRequest.get("/api/WorkRun/GetUsingEqpList").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          this.setData({
            eqpName:res.data.data[0].F_EqpName,
            storageHidden: true,
            objectEqpArray: res.data.data,
          });
          if (!!this.data.eqpName)
          {
              this.GetUseMaterial(this.data.eqpName);
          }
      } else {
        dd.alert({ title: '提示', content: "无使用中的设备", buttonText: '我知道了', });
      }
    })  
  },
  bindObjPickerEqpChange(e) {
    this.setData({
      arrEqpIndex: e.detail.value,
      eqpName:this.data.objectEqpArray[e.detail.value].F_EqpName,
      hidden: true,
      arrIndex: 0,
      usehidden:true
    });
    if (!!this.data.eqpName)
    {
        this.GetUseMaterial(this.data.eqpName);
    }
  },
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
  Scan_transferCode(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          transferCode: res.code
        });
        if (!!this.data.transferCode)
        {
            this.GetStorageByCode(this.data.eqpName);
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
        this.GetStorageByCode(this.data.eqpName);
    }   
  },
  onSubmit:throttle(function(e) {
    var data=this.data;
    if(this.data.useentity.F_MaterialCode!=this.data.storageEntity.F_MaterialCode){
        dd.alert({ title: '提示', content: "所需物料不匹配！", buttonText: '我知道了', });
        return false;
    }
    this.data.TargetTransferCode=e.detail.value.TargetTransferCode;
    this.data.num=e.detail.value.num;
    myRequest.post("/api/WorkRun/ConsumeMaterial", JSON.stringify(data)).then();
  }),
  GetUseMaterial: function (eqpName) {
    var that = this;
    var api=encodeURI("/api/WorkRun/GetUseMaterial?eqpName=" + eqpName);
    myRequest.get(api).then((res) => {
      if (res.data.count > 0) {
        that.setData({
          objectArray: res.data.data,
          useentity:res.data.data[0],
          usehidden:false
        });
        this.GetStorageByMaterial(this.data.objectArray[0].F_MaterialCode, this.data.eqpName);
      } else {
        dd.alert({ title: '提示', content: "不存在和此设备匹配的物料信息！", buttonText: '我知道了', });
      }
    })
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      hidden: true,
      useentity:this.data.objectArray[e.detail.value],
      usehidden:false,
    });
    this.GetStorageByMaterial(this.data.objectArray[e.detail.value].F_MaterialCode, this.data.eqpName);
  },
  bindObjPickerOutChange(e) {
    this.setData({
        objectBoxArray: this.data.objectOutArray[e.detail.value].list,
        storageEntity:this.data.objectOutArray[e.detail.value].list[0],
        locationCode:this.data.objectOutArray[e.detail.value].list[0].F_LocationCode,
        transferCode:this.data.objectOutArray[e.detail.value].list[0].F_TransferBoxCode,
        num: this.data.objectOutArray[e.detail.value].list[0].F_Num,
        arrBoxIndex: 0,
        arrOutIndex: e.detail.value,
        hidden: false
    });
  },
  GetStorageByMaterial: function (code, eqpName,isall=false) {
    var that = this;
    var api=encodeURI("/api/Common/GetStorageListByMaterial?eqpName=" + eqpName + "&code=" + code+"&isAll="+isall);
    myRequest.get(api).then((res) => {
      if (res.data.count>0) {
        that.setData({
          readyEntity:res.data.data[0].list[0],
          readyhidden: false
        });
      } else {
        that.setData({
          readyEntity: {},
          readyhidden: true
        });
        dd.alert({ title: '提示', content: "不存在和此物料匹配的库存信息！", buttonText: '我知道了', });
      }
    })
  },
  GetStorageByCode: function () {
    var that=this;
    var code = this.data.transferCode;
    var api=encodeURI("/api/Common/GetStorageByCode?code=" + code);
    myRequest.get(api).then((res) => {
      if (!!res.data.data) {
        that.setData({
          storageEntity:res.data.data,
          hidden: false,
          num:res.data.data.F_Num
        });
      } else {
        that.setData({
          storageEntity: {},
          hidden: true,
          num:0
        });
        dd.alert({ title: '提示', content: "不存在和此物料匹配的库存信息！", buttonText: '我知道了', });
      }
    })
  }
});
