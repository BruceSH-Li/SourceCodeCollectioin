const app = getApp();
var myRequest = require('/static/js/myRequest.js');
Page({
  data: {
    batchCode: "",
    propList: [],  // 列表数据
    description:"",
    descriptionId:"",
    filePath:"",
    arrTypeIndex:0,
    objectTypeArray:[],
    arrIndex:0,
    objectArray:[],
    imageHidden:true,
  },
  onLoad(query) {
    // 页面显示
    var that = this;
    that.setData({
      batchCode: query.batchCode,
    })
    if(!!query.batchCode)
    {
      dd.getStorage({
        key: query.batchCode,
        success: (res) => {
          if (res.data) {
            that.setData({
              propList: res.data.list,
            })
          }
        }
      });
    }
    myRequest.get("/api/Checkout/GetDescriptionInfo").then((res) => {
      if (res.data.data && res.data.data.length>0)
      {
          this.setData({
            storageHidden: true,
            objectTypeArray: res.data.data,
            arrIndex: 0,
            objectArray: res.data.data[0].list,
            description:res.data.data[0].list[0].F_Description,
            descriptionId:res.data.data[0].list[0].F_Id,
          });
      } else {
        dd.alert({ title: '提示', content: "无需要检验的物料", buttonText: '我知道了', });
      }
    })     
  },
  AddScapImage() {
    dd.chooseImage({
      count: 1,
      success: (res) => {
        this.setData({
          filePath: res.filePaths[0],
          imageHidden:false,
        });
      },
    });
  },
  onSubmit(e) {
    if(!!this.data.filePath)
    {
      dd.uploadFile({
          headers: {
            "Content-Type": "application/json",
            "WC-Token": app.globalData .apitoken
          },
          url: app.globalData .apiurl+"/api/Checkout/PhotoSaveIndex",
          fileType: 'image',
          fileName: 'employee_photo',
          filePath: this.data.filePath,
          success: (res) => {
            var image={};
            image.F_FilePath=JSON.parse(res.data).file;
            image.F_Description=this.data.description;
            image.F_DescriptionId=this.data.descriptionId;
            for (let index = 0; index < this.data.propList.length; index++) {
              if  (this.data.propList[index].F_DescriptionId==image.F_DescriptionId)
              {
                return false;
              }
            }
            this.data.propList.push(image);
            dd.setStorage({
              key: this.data.batchCode,
              data: {
                  list: this.data.propList,
                },
            });
            dd.navigateBack();
          },
      });  
    }
    else  
    {
        var image={};
        image.F_FilePath="";
        image.F_Description=this.data.description;
        image.F_DescriptionId=this.data.descriptionId;
        for (let index = 0; index < this.data.propList.length; index++) {
          if  (this.data.propList[index].F_DescriptionId==image.F_DescriptionId)
          {
            return false;
          }
        }
        this.data.propList.push(image);
        dd.setStorage({
          key: this.data.batchCode,
          data: {
              list: this.data.propList,
            },
        });
        dd.navigateBack();
    }
  },
  LookImage(event) {
    dd.previewImage({
      current: 0,
      urls: [
        event.target.dataset.attr,
      ],
    });
  },
  bindObjPickerChange(e) {
    this.setData({
      arrIndex: e.detail.value,
      description:this.data.objectArray[e.detail.value].F_Description,
      descriptionId:this.data.objectArray[e.detail.value].F_Id,
    });
  },
  bindObjPickerTypeChange(e) {
    this.setData({
      arrTypeIndex: e.detail.value,
      objectArray: this.data.objectTypeArray[e.detail.value].list,
      description:this.data.objectTypeArray[e.detail.value].list[0].F_Description,
      descriptionId:this.data.objectTypeArray[e.detail.value].list[0].F_Id,
      arrIndex: 0,
    });
  },
});
