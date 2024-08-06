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
    objectBoxArray: [],
    arrBoxIndex: 0,
    arrOutIndex: 0,
    usehidden:true,
    numHidden:true,
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
  bindObjPickerBoxChange(e) {
    this.setData({
      arrBoxIndex: e.detail.value,
      locationCode:this.data.objectBoxArray[e.detail.value].F_LocationCode,
      transferCode:this.data.objectBoxArray[e.detail.value].F_TransferBoxCode,
      hidden: false,
      storageEntity:this.data.objectBoxArray[e.detail.value],
      num: this.data.objectBoxArray[e.detail.value].F_Num,
    });
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
  onSubmit:throttle(function(e) {
    var data=this.data;
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
      usehidden:false
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
          objectBoxArray: res.data.data[0].list,
          storageEntity:res.data.data[0].list[0],
          locationCode:res.data.data[0].list[0].F_LocationCode,
          transferCode:res.data.data[0].list[0].F_TransferBoxCode,
          num: res.data.data[0].list[0].F_Num,
          arrBoxIndex: 0,
          objectOutArray: res.data.data,
          arrOutIndex: 0,
          hidden: false
        });
      } else {
        that.setData({
          objectBoxArray: [],
          objectOutArray: [],
          storageEntity: {},
          locationCode:"",
          transferCode:"",
          arrBoxIndex: 0,
          arrOutIndex: 0,
          num: 0,
          hidden: true
        });
        dd.alert({ title: '提示', content: "不存在和此物料匹配的库存信息！", buttonText: '我知道了', });
      }
    })
  },
  GetStorage: function () {
    var that = this;
    that.GetStorageByMaterial(that.data.objectArray[that.data.arrIndex].F_MaterialCode,that.data.eqpName);    
  },
  GetAllStorage: function () {
    var that = this;
    that.GetStorageByMaterial(that.data.objectArray[that.data.arrIndex].F_MaterialCode,that.data.eqpName,true);    
  },
});
