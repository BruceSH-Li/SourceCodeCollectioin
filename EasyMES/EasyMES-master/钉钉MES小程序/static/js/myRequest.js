const app = getApp();

const myRequest = {

  //通用查询接口，仅返回success成功才进行自定义回调
  get(url, data) {
    return new Promise((resolve, reject) => {
      dd.showLoading();
      dd.httpRequest({
        headers: {
          // "Content-Type": "application/json",
          "WC-Token": app.globalData .apitoken
        },
        url: app.globalData .apiurl + url,
        method: 'GET',
        data: data,//参数附在url后或写成data对象
        dataType: 'json',
        success: function (res) {

          if (res.data.state == 'error' && res.data.code == 21333) {
            dd.alert({
              title: '提示',
              content: '你的登入信息已过期，请重新登入',
              buttonText: '重新登入',
              success: () => {
                app.getUserInfo();
                // get(url,data);
              }
            });
          } else if (res.data.state == 'success') {
            resolve(res);//查询成功自定义回调
          } else {
            dd.alert({ title: '后台查询失败', content: res.data.message, buttonText: '好的', });
          }
        },
        fail: function (res) {
          reject(res);
          dd.alert({ title: '前台提交失败', content: res.errorMessage + ":" + res.status, buttonText: '我知道了', });
        },
        complete: function (res) {
          dd.hideLoading();
        }
      });
    })

  },

  //通用提交接口，提交成功返回上级页面
  post(url, data) {
    return new Promise((resolve, reject) => {
      dd.httpRequest({
        headers: {
          "Content-Type": "application/json",
          "WC-Token": app.globalData .apitoken
        },
        url: app.globalData .apiurl + url,
        method: 'POST',
        data: data,
        dataType: 'json',
        success: function (res) {
          resolve(res);
          console.log(res);
          if (res.data.state == 'error' && res.data.code == 21333) {

            dd.alert({
              title: '提示',
              content: '你的登入信息已过期，请重新登入',
              buttonText: '重新登入',
              success: () => {
                app.getUserInfo();
              }
            });
          } else {

            dd.alert({
              title: res.data.state == 'success' ? '提交成功' : '后台提交失败',
              content: res.data.state == 'success' ? '提交成功' : res.data.message,
              buttonText: '好的',
              success: () => {
                if (res.data.state == 'success') {
                  dd.navigateBack();
                }
              }
            });
          }

        },
        fail: function (res) {
          reject(res);
          dd.alert({ title: '前台提交失败', content: res.errorMessage + ":" + res.status, buttonText: '我知道了', });
        }

      });
    })
  },

  //提交成功后无返回，可自定义回调，必要时可配合webutil.js中的节流函数减少重复提交问题
  postWithoutBack(url, data) {
    return new Promise((resolve, reject) => {
      dd.httpRequest({
        headers: {
          "Content-Type": "application/json",
          "WC-Token": app.globalData .apitoken
        },
        url: app.globalData .apiurl + url,
        method: 'POST',
        data: data,
        dataType: 'json',
        success: function (res) {
          // resolve(res);
          console.log(res);
          if (res.data.state == 'error' && res.data.code == 21333) {

            dd.alert({
              title: '提示',
              content: '你的登入信息已过期，请重新登入',
              buttonText: '重新登入',
              success: () => {
                app.getUserInfo();
              }
            });
          } else {

            dd.alert({
              title: res.data.state == 'success' ? '提交成功' : '后台提交失败',
              content: res.data.state == 'success' ? '提交成功' : res.data.message,
              buttonText: '好的',
              success: () => {
                if (res.data.state == 'success') {
                  // dd.navigateBack();
                  resolve(res);
                }
              }
            });
          }

        },
        fail: function (res) {
          reject(res);
          dd.alert({ title: '前台提交失败', content: res.errorMessage + ":" + res.status, buttonText: '我知道了', });
        }

      });
    })
  },
}

module.exports = myRequest;