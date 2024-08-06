App({
  globalData: {
    apiurl:"",
    userid: "",
    username: "",
    apitoken: "",
    mainurl: "",    
    PhotoUrl:""

  },
  onLaunch(options) {
    // 第一次打开
    // options.query == {number:1}
    //console.info(options.query.corpId);
    // this.autoUpdate();
    var that=this;
    dd.getStorage({
      key: 'userInfo',
      success: (res) => {
        if (res.data) {
          that.globalData.apiUrl = !!res.data.apiUrl?res.data.apiUrl:that.globalData.apiUrl;
          that.globalData.mainurl = !!res.data.apiUrl?res.data.apiUrl:that.globalData.apiUrl;
          that.globalData.PhotoUrl = !!res.data.PhotoUrl?res.data.PhotoUrl:that.globalData.apiUrl;
        }
      }
    });
  },
  onShow(options) {
    // 从后台被 scheme 重新打开
    // options.query == {number:1}

  },
  onHide() {
    // 隐藏
  },
  onError(msg) {
    console.log(msg);
    
  },

  getUserInfo: function () {
    dd.showLoading({ content: '登入中' });
    var that = this;
    return new Promise(function (resolve, reject) {
      dd.getAuthCode({
        success: res => {
          var authCode = res.authCode;

          dd.httpRequest({
            url: that.globalData.apiurl + "/api/User/DingLogin?code=" + authCode,
            method: 'POST',
            // data: { jobCode: query.jobCode },
            dataType: 'json',
            success: (res) => {
              if (res.data.state == 'success') {
                dd.showToast({ content: '登入成功', icon: 'success', duration: 1000 });
                dd.setStorage({
                  key: 'userInfo',
                  data: {
                    userid: res.data.data.userid,
                    username: res.data.data.username,
                    apitoken: res.data.data.apitoken,
                    mainurl: that.globalData.apiurl,
                    apiurl:that.globalData.apiurl,
                    PhotoUrl:that.globalData.PhotoUrl,
                  },
                  // success: function () {
                  //   dd.alert({ content: '写入成功' });
                  // }
                });

                that.globalData.userid = res.data.data.userid;
                that.globalData.username = res.data.data.username;
                that.globalData.apitoken = res.data.data.apitoken;
                that.globalData.mainurl = that.globalData.apiurl;


                resolve({
                  userid: res.data.data.userid,
                  username: res.data.data.username
                });

              } else if (res.data.state = 'error' && res.data.code == 1000) {
                dd.alert({
                  title: '提示', content: res.data.message, buttonText: '好的',
                  success: () => {
                    dd.reLaunch({ url: "/pages/User/DingBanding/DingBanding" });
                  }
                });
              }

              else {
                dd.alert({ title: '登入失败，请重试', content: res.data.message, buttonText: '好的' });
              }
            },
            fail: function (res) {
              dd.alert({ title: '获取失败', content: res.status+":"+res.errorMessage+":"+res.status, buttonText: '我知道了', });
            },
            complete: function (res) {
              dd.hideLoading();

            }
          });





        },
        fail: function (res) {
          dd.alert({ content: res.errorMessage+":"+res.status });
        }
      });

    }).catch((e) => { });

  },

  autoUpdate: function () {
    var self = this;
    console.info(dd.SDKVersion);
    // 获取小程序更新机制兼容
    if (dd.canIUse('getUpdateManager')) {
      const updateManager = dd.getUpdateManager()
      //1. 检查小程序是否有新版本发布
      updateManager.onCheckForUpdate(function (res) {
        // 请求完新版本信息的回调
        if (res.hasUpdate) {
          //检测到新版本，需要更新，给出提示
          dd.confirm({
            title: '更新提示',
            content: '检测到新版本，是否下载新版本并重启小程序？',
            success: function (res) {
              if (res.confirm == true) {
                //2. 用户确定下载更新小程序，小程序下载及更新静默进行
                self.downLoadAndUpdate(updateManager)
              } else if (res.confirm == false) {
                //用户点击取消按钮的处理，如果需要强制更新，则给出二次弹窗，如果不需要，则这里的代码都可以删掉了
                dd.confirm({
                  title: '温馨提示~',
                  content: '本次版本更新涉及到新的功能添加，旧版本无法正常访问的哦~',
                  // showCancel:false,//隐藏取消按钮
                  confirmButtonText: "确定更新",//只保留确定更新按钮
                  success: function (res) {
                    if (res.confirm) {
                      //下载新版本，并重新应用
                      self.downLoadAndUpdate(updateManager)
                    }
                  }
                })
              }
            }
          })
        }
      })
    } else {
      // 如果希望用户在最新版本的客户端上体验您的小程序，可以这样子提示
      dd.alert({
        title: '提示',
        content: '当前微信版本过低，无法使用该功能，请升级到最新微信版本后重试。'
      })
    }
  },
  /**
   * 下载小程序新版本并重启应用
   */
  downLoadAndUpdate: function (updateManager) {
    var self = this
    dd.showLoading();
    //静默下载更新小程序新版本
    updateManager.onUpdateReady(function () {
      dd.hideLoading()
      //新的版本已经下载好，调用 applyUpdate 应用新版本并重启
      updateManager.applyUpdate()
    })
    updateManager.onUpdateFailed(function () {
      // 新的版本下载失败
      dd.alert({
        title: '已经有新版本了哟~',
        content: '新版本已经上线啦~，请您删除当前小程序，重新搜索打开哟~',
      })
    })
  },

});
