var app = getApp();
var myRequest = require('/static/js/myRequest.js');
var until = require('/static/js/webutil.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    transferCode: "",
    materialCode: "",
    num: null,
    eqpName: "",
    objectArray: [],
    arrIndex: 0,
    description: "",    
    objectEqpArray: [],
    arrEqpIndex: 0,
    hidden:true,
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
      hidden: true
    });
    if (!!this.data.eqpName)
    {
        this.GetUseMaterial(this.data.eqpName);
    }
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
            this.GetOutputMaterialBandingByCode(this.data.transferCode);
        }
      }
    });
  },
  search(event) {
    this.setData({
      transferCode: event.detail.value,
      hidden: true
    });
    if (!!this.data.transferCode)
    {
        this.GetOutputMaterialBandingByCode(this.data.transferCode);
    }
  },
  Scan_materialBatch(event) {
    dd.scan({
      type: 'qr',
      success: (res) => {
        this.setData({
          materialBatch: res.code
        });
      }
    });
  },
  GetUseMaterial: function (eqpName) {
    var that = this;
    var api=encodeURI("/api/WorkRun/GetOutMaterial?eqpName=" + eqpName);
    myRequest.get(api).then((res) => {
      if (res.data.count > 0) {
        var materialBatch = "";
        if(res.data.data[0].F_BatchType == 1)
        {
            materialBatch = res.data.data[0].F_BatchPrefix +"-"+ until.formatTime(new Date(),"yyMMDDhhmmss");
        }
        else
        {
          var api=encodeURI("/api/WorkRun/GetBatch");
          myRequest.get(api).then((resit) =>{
            materialBatch=resit
          })
        }
        that.setData({
          objectArray: res.data.data,
          materialCode: res.data.data[0].F_MaterialCode,
          num:res.data.data[0].F_ContainerNum,
          materialBatch: materialBatch,
        });
      } else {
        dd.alert({ title: '提示', content: "不存在和此设备匹配的信息！", buttonText: '我知道了', });
      }
    })
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      materialCode: this.data.objectArray[e.detail.value].F_MaterialCode,
      num: this.data.objectArray[e.detail.value].F_ContainerNum
    });
  },
  onSubmit:throttle(function(e) {
    var data=this.data;
    data.num=e.detail.value.num;
    data.transferCode=e.detail.value.transferCode;
    myRequest.post("/api/WorkRun/OutputMaterial", JSON.stringify(data)).then();

  }),
    GetOutputMaterialBandingByCode: function (code) {
    var that = this;
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/WorkRun/GetOutputMaterialBandingByCode?code=" + code,
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
            that.setData({
              entity: res.data.data,
              hidden: false
            });


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
