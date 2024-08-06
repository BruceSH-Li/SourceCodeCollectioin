var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';
Page({
  data: {
    TaskJob: {},
    now:new Date()
  },
  onLoad(query) {
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/TaskJob/GetTaskJobForm",
      method: 'GET',
      data: { jobCode: query.jobCode },
      // data:{jobCode:'CJ_202102191002330'},
      dataType: 'json',
      success: (res) => {
        if (res.data.state == 'success') {
          this.setData({
            TaskJob: res.data.data
          });
        }
        else if (res.data.state == 'error' && res.data.code == 21333) {
          dd.alert({
            title: '提示',
            content: '你的登入信息已过期，请重新登入',
            buttonText: '重新登入',
            success: () => {
              app.getUserInfo();
              dd.navigateBack();
            }
          });
        }
      },
      fail: function (res) {
        dd.alert({ title: '获取失败', content: res.errorMessage + ":" + res.status, buttonText: '我知道了', });
      }

    });
  },
  onSubmit:throttle(function(e) {
    myRequest.post("/api/TaskJob/DoTaskJob", '\'' + this.data.TaskJob.F_JobCode + '\'').then();

  })
});
