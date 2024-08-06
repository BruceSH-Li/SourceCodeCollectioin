var app = getApp();
var myRequest = require('/static/js/myRequest.js');
Page({
  data: {
    F_CallRepairMan: "",
    F_CallRepairManName: "",

    objectEqpArray: [],
    arrEqpIndex: 0,
    F_EqpId: "",
    F_EqpName: "",

    objectProblemTypeArray: [
      {
        F_ProblemType: 1,
        F_ProblemTypeDesc: "人"
      },
      {
        F_ProblemType: 2,
        F_ProblemTypeDesc: "机"
      },
      {
        F_ProblemType: 3,
        F_ProblemTypeDesc: "料"
      },
      {
        F_ProblemType: 4,
        F_ProblemTypeDesc: "法"
      },
      {
        F_ProblemType: 5,
        F_ProblemTypeDesc: "环"
      }
    ],
    arrProblemTypeIndex: 0,
    F_ProblemType: 1,


    objectProblemClassArray: [
      {
        F_ProblemClass: 1,
        F_ProblemClassDesc: "低级"
      },
      {
        F_ProblemClass: 2,
        F_ProblemClassDesc: "中级"
      },
      {
        F_ProblemClass: 3,
        F_ProblemClassDesc: "高级"
      },
    ],
    arrProblemClassIndex: 0,
    F_ProblemClass: 1,

    images: [],
    uploadedImages: [],
    imageWidth: 100,


  },
  onLoad() {
    var that = this;
    that.getEqpList();
    dd.setNavigationBar({
      title: "设备报修"
    });

    dd.getSystemInfo({
      success: function (res) {
        that.setData({
          imageWidth: res.screenWidth / 4 - 10
        })
      }
    })

    dd.getStorage({
      key: 'userInfo',
      success: (res) => {
        that.setData({
          F_CallRepairManName: res.data.username,
          F_CallRepairMan: res.data.userid,
        })
      }
    })


  },

  chooseImage: function () {        // 选择图片
    var that = this;
    dd.chooseImage({
      sizeType: ['original', 'compressed'],

      sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有

      success: function (res) {                // 返回选定照片的本地文件路径列表，tempFilePath可以作为img标签的src属性显示图片

        var tempFilePaths = res.tempFilePaths;
        // console.log(tempFilePaths);

        that.setData({
          images: that.data.images.concat(tempFilePaths)

        });

      }

    })

  },
  previewImage: function (event) {        // 预览图集
    var that = this;
    dd.previewImage({
      urls: that.data.images,
      current:event.target.dataset.index
    });

  },
  delete: function (e) {
    var that = this;
    var index = e.currentTarget.dataset.index;
    var images = that.data.images;

    images.splice(index, 1);

    that.setData({
      images: images

    });

  },





  getEqpList: function (code) {
    //api/EqpRepair/getEqplist
    myRequest.get("/api/EqpRepair/getEqplist").then((res) => {

      this.setData({
        F_EqpId: res.data.data[0].F_Id,
        F_EqpName: res.data.data[0].F_EqpName,
        objectEqpArray: res.data.data,
      });
      // if (!this.data.eqpId) return;
      // this.gethotdata(this.data.eqpId);

    })
  },

  bindObjPickerEqpChange(e) {
    if (e.detail.value == this.data.arrEqpIndex) return;
    this.setData({
      arrEqpIndex: e.detail.value,
      F_EqpId: this.data.objectEqpArray[e.detail.value].F_Id,
      F_EqpName: this.data.objectEqpArray[e.detail.value].F_EqpName,
    });
    // this.gethotdata(this.data.eqpId);

  },

  bindObjPickerProblemTypeChange(e) {
    if (e.detail.value == this.data.arrProblemTypeIndex) return;
    this.setData({
      arrProblemTypeIndex: e.detail.value,
      F_ProblemType: this.data.objectProblemTypeArray[e.detail.value].F_ProblemType,
      // F_ProblemTypeDesc: this.data.objectProblemTypeArray[e.detail.value].F_ProblemTypeDesc,
    });
    // this.gethotdata(this.data.eqpId);

  },

  bindObjPickerProblemClassChange(e) {
    if (e.detail.value == this.data.arrProblemClassIndex) return;
    this.setData({
      arrProblemClassIndex: e.detail.value,
      F_ProblemClass: this.data.objectProblemClassArray[e.detail.value].F_ProblemClass,
      // F_ProblemClassDesc: this.data.objectProblemClassArray[e.detail.value].F_ProblemClassDesc,
    });


  },

  uploadimagesFun: function (tempFilePath) {
    var that = this;
    return new Promise((resolve, reject) => {
      //上传图片
      dd.uploadFile({
        url: app.globalData .apiurl + "/api/HotlineData/AppPhotoSave",
        filePath: tempFilePath,
        fileName: 'test.jpeg',
        fileType: 'image',

        success: (res) => {
          console.log(res);
          var uploadedImages = that.data.uploadedImages;
          if (!!res.data) {
            var r = JSON.parse(res.data);
            if (r.error == 0) {

              uploadedImages.push(r.files[0]);
              that.setData({
                uploadedImages: uploadedImages
              })

              resolve();
            }
            else {
              dd.alert({ title: '后台提交失败', content: res.data.message, buttonText: '好的', });
              return;
            }
          }

        }

      })

    })

  },

  onSubmit(e) {
    //提交图片
    var that = this;
    this.setData({
      uploadedImages: []
    });

    if (!e.detail.value.F_ProblemDesc) {
      dd.alert({ title: '前台提交失败', content: "故障描述不能为空！", buttonText: '好的', });
      return;
    }

    let asyncFun = [];
    that.data.images.forEach(function (tempFilePath) {
      asyncFun.push(that.uploadimagesFun(tempFilePath))
    })
    Promise.all(
      asyncFun

    ).then(() => {

      //上传数据
      var data = that.data;
      data.F_PhotoPath = data.uploadedImages.join(';');
      data.F_ProblemDesc = e.detail.value.F_ProblemDesc;
      myRequest.post("/api/EqpRepair/EqpCallRepair", JSON.stringify(data)).then();
    });

  }

});
