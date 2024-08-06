var app = getApp();
import { getTimeStamp } from '/static/js/webutil.js'
Page({
  data: {
    TaskJobs: [],
    now:new Date()
  },
  onLoad() {
    var that = this;
    that.getTaskJob();
  },
  onShow() {
    var that = this;
    that.getTaskJob();
  },
  onPullDownRefresh() {
    console.log('onPullDownRefresh', new Date());
    this.getTaskJob();
  },
  DetailTaskJob(e) {
    this.data.TaskJobs.forEach(element => {
      if (element.F_JobCode == e.currentTarget.dataset.jobCode)
      {
        if(element.F_JobState == 0){
          dd.navigateTo({
            url: '/pages/TaskJob/DoTaskJob/DoTaskJob?jobCode=' + e.currentTarget.dataset.jobCode
          });
        }
        return;
      }   
    });
  },

  getTaskJob: function () {
    var that = this;
    dd.showLoading();
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/TaskJob/GetUnDoTaskJob",
      method: 'GET',
      data: JSON.stringify(null),
      dataType: 'json',
      success: (res) => {
        if (res.data.state == 'success') {
          that.setData({
            TaskJobs: res.data.data,
            JobCount: res.data.count
          });


        }
        else if (res.data.state == 'error' && res.data.code == 21333) {
          dd.alert({
            title: '提示',
            content: '你的登入信息已过期，请重新登入',
            buttonText: '重新登入',
            success: () => {
              // app.getUserInfo();
              // that.getTaskJob();
              dd.removeStorageSync({ key: 'userInfo' });
              dd.reLaunch({
                url: "/pages/index/index"
              });

            }
          });
        }
        else {
          dd.alert({ title: '获取失败', content: res.data.message, buttonText: '我知道了' });
        }
      },
      fail: function (res) {
        dd.alert({ title: '获取失败', content: res.errorMessage+":"+res.status, buttonText: '我知道了' });

      },
      complete: function (res) {
        dd.stopPullDownRefresh();
        dd.hideLoading();
      }

    });

  }

});
