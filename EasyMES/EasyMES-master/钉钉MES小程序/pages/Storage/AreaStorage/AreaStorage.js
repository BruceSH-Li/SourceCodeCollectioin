var app = getApp();
var myRequest = require('/static/js/myRequest.js');
var until = require('/static/js/webutil.js');
Page({
  data: {
    storageHidden: true,
    arrIndex:0
  },
  onLoad() { 
    myRequest.get("/api/Common/GetMaterialList").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          this.setData({
            storageEntity:res.data.data[0],
            storageHidden: false,
            objectArray: res.data.data,
          });
      } else {
        dd.alert({ title: '提示', content: "无物料信息", buttonText: '我知道了', });
      }
    })  
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      storageHidden: false,
      storageEntity:this.data.objectArray[e.detail.value]
    });
  },

});
