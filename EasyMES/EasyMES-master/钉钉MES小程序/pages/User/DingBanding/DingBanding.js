var md5=require('/static/js/md5.js');
var app = getApp();
Page({
  data: {
  },
  onLoad() { },


  onSubmit(e) {
    // dd.reLaunch({
    //   url: "/pages/index/index"
    // });
    // return;

    dd.getAuthCode({
      success: res => {
        var authCode = res.authCode;
        e.detail.value.code = authCode;
        e.detail.value.password=md5.hex(e.detail.value.password);
        console.info(e.detail.value.password);
        
        dd.showLoading({ content: '绑定中' });
        dd.httpRequest({
          headers: {
            "Content-Type": "application/json",
            // "WC-Token": app.globalData .apitoken
          },
          url: app.globalData .apiurl + "/api/User/DingBandingUser",
          method: 'POST',
          data: JSON.stringify(e.detail.value),
          dataType: 'json',
          success: function (res) {
            console.log(res);
            if (res.data.state == 'success') {
              dd.showToast({ content: '绑定成功，即将跳转', icon: 'success', duration: 1000 });
              dd.setStorageSync({
                key: 'userInfo',
                data: {
                  userid: res.data.data.userid,
                  username: res.data.data.username,
                  apitoken: res.data.data.apitoken,
                  apiurl:app.globalData.apiurl,
                  PhotoUrl:app.globalData.PhotoUrl,
                },
              });
              app.globalData .apitoken = res.data.apitoken;
              dd.reLaunch({
                url: "/pages/index/index"
              });
            }
            else {
              dd.alert({
                title: res.data.state == 'success' ? '提交成功' : '提交失败',
                content: res.data.state == 'success' ? '提交成功' : res.data.message,
                buttonText: '好的',
                // success: () => {
                //   if (res.data.state == 'success') {
                //     dd.navigateBack();
                //   }
                // }
              });
            }
          },
          fail: function (res) {

            dd.alert({ title: '提交失败', content: res.errorMessage+":"+res.status, buttonText: '我知道了', });
          },
          complete: function (res) {
            dd.hideLoading();

          }

        });
      }
    })
  },


  inputFocus: function (e) {
    if (e.target.id == 'userid') {
      this.setData({
        'userid_focus': true
      });
    } else if (e.target.id == 'passwd') {
      this.setData({
        'passwd_focus': true
      });
    }
  },
  inputBlur: function (e) {
    if (e.target.id == 'userid') {
      this.setData({
        'userid_focus': false
      });
    } else if (e.target.id == 'passwd') {
      this.setData({
        'passwd_focus': false
      });
    }
  },
});
