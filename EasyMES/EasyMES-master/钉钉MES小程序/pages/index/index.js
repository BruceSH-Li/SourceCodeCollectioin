import { checkNetStatus } from '/static/js/webutil.js';
var app = getApp();
Page({
  data: {
    username: app.globalData .username,
    userid: app.globalData .userid,
    mainurl: app.globalData .mainurl,
    menuList: null,
    arrIndex:0,
    objectArray:[
      {
        name:'水之云',
        apiurl:'http://106.75.169.226:8082',
        PhotoUrl:'http://106.75.169.226:8082/upload/'
      },
      {
        name:'测试工厂',
        apiurl:'http://localhost:8082',
        PhotoUrl:'http://localhost:8082/upload/'
      }
    ],
  },
  bringToFront(e) {
    const { value } = e.target.dataset;
    const list = this.data.list.concat();
    const index = list.indexOf(value);
    if (index !== -1) {
      list.splice(index, 1);
      list.unshift(value);
      this.setData({ list });
    }
  },
  bindObjPickerChange(e) {
    if(e.detail.value!=this.data.arrIndex)
    {
      this.setData({
        arrIndex: e.detail.value,
        apiurl:this.data.objectArray[e.detail.value].apiurl,
        PhotoUrl:this.data.objectArray[e.detail.value].PhotoUrl,
      });
      dd.setStorage({
        key: 'userInfo',
        data: {
          username: null,
          userid: null,
          apiurl:this.data.apiurl,
          PhotoUrl:this.data.PhotoUrl,
        },
      });
      app.globalData.apiurl = this.data.apiurl;
      app.globalData.PhotoUrl = this.data.PhotoUrl,
      this.Login();
    } 
  },
  onLoad(query) {
    // return;
    // 页面加载
    //console.info(`Page onLoad with query: ${JSON.stringify(query)}`);
    var that = this;
    that.getLoadInfo();

    // 判断网络状态，无法访问Internet则直接返回
    // checkNetStatus().then((res) => {
    //   if (!res) {
    //     dd.showToast({ content: '当前网络不可用，请联网后下拉刷新', icon: 'success', duration: 3000 });
    //     return;
    //   }
    //   that.getLoadInfo();
    // }).catch((res) => {
    //   dd.showToast({ content: '当前网络不可用，请联网后下拉刷新1', icon: 'success', duration: 3000 });
    //   return;
    // });


  },
  onReady() {
    // 页面加载完成



  },
  onShow() {
    // 页面显示
    var that = this;
    dd.getStorage({
      key: 'userInfo',
      success: (res) => {
        if (res.data) {
          that.setData({
            username: res.data.username,
            userid: res.data.userid,
            mainurl: res.data.mainurl
          })
          if (that.data.userid) {
            // that.getTaskCount();
          }
          // app.globalData .apitoken = res.data.apitoken;
        }

      }

    });
  },
  onHide() {
    // 页面隐藏
  },
  onUnload() {
    // 页面被关闭
    var that = this;
    clearInterval(that.data.inter);
  },
  onTitleClick() {
    // 标题被点击
  },
  onPullDownRefresh() {
    // 页面被下拉
    this.onLoad();
    dd.stopPullDownRefresh();
  },
  onReachBottom() {
    // 页面被拉到底部
  },
  onShareAppMessage() {
    // 返回自定义分享信息
    return {
      title: 'My App',
      desc: 'My App description',
      path: 'pages/index/index'
    };
  },

  //登入按钮
  Login() {
    //     dd.reLaunch({url:'/pages/User/DingBanding/DingBanding'});
    // return;
    var that = this;
    app.getUserInfo().then(function (obj) {
      that.setData({
        username: obj.username,
        userid: obj.userid,
        mainurl: obj.mainurl
      });

      if (that.data.userid) {
        that.getTaskCount();
        that.getRoleMenu();
      }

    });

  },

  //定时获取任务
  startInter: function () {
    var that = this;
    clearInterval(that.data.inter);
    var timeout = 5 * 60 * 1000;
    var _settings = dd.getStorageSync({ key: "settings" });
    if (_settings.data) {
      timeout = _settings.data.taskslider * 60 * 1000;
      console.log(timeout);
    }

    if (!timeout) timeout = 5 * 60 * 1000;

    that.data.inter = setInterval(
      function () {
        // TODO 你需要无限循环执行的任务
        console.log('setInterval 每过' + timeout + '秒执行一次任务');
        that.getTaskCount();
        // that.startInter();
        // console.log('setInterval 每过' + timeout + '秒执行一次任务')
      }, timeout);
  },

  getTaskCount: function () {
    console.log('定时任务执行中')
    var that = this;
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
          if (res.data.count != 0) {
            dd.setTabBarBadge({ index: 1, text: res.data.count.toString() });
          }
          else
          {
            dd.setTabBarBadge({ index: 1, text: '0' });
          }
          that.startInter();

        }
        else if (res.data.state == 'error' && res.data.code == 21333) {

          that.setData({
            userid: null,
            username: null
          })
          dd.alert({
            title: '提示',
            content: '你的登入信息已过期，请重新登入',
            buttonText: '好的',
            success: () => {
              // app.getUserInfo();
            }
          });
          clearInterval(that.data.inter);
        }
        else {
          that.setData({
            userid: null,
            username: null
          })
          dd.alert({ title: '获取失败', content: res.data.message, buttonText: '我知道了' });
          clearInterval(that.data.inter);
        }
      },
      fail: function (res) {
        that.setData({
          userid: null,
          username: null
        })
        dd.alert({ title: '获取失败', content: res.errorMessage + ":" + res.status, buttonText: '我知道了' });
        clearInterval(that.data.inter);
      },
      complete: function (res) {
        dd.hideLoading();

      }

    });

  },

  getRoleMenu: function () {
    var that = this;
    dd.httpRequest({
      headers: {
        "Content-Type": "application/json",
        "WC-Token": app.globalData .apitoken
      },
      url: app.globalData .apiurl + "/api/User/GetMenuList",
      method: 'GET',
      data: JSON.stringify(null),
      dataType: 'json',
      success: (res) => {
        if (res.data.state == 'success') {
          if (res.data.count != 0) {
            that.setData({
              menuList: res.data.data,
            })
          }
        }
      }
    });
  },
  getLoadInfo() {
    var that = this;
    dd.getStorage({
      key: 'userInfo',
      success: (res) => {
        if (res.data && res.data.userid) {
          that.setData({
            username: res.data.username,
            userid: res.data.userid,
            mainurl: res.data.mainurl,
            apiurl:res.data.apiurl,
            PhotoUrl:res.data.PhotoUrl,
          })
          app.globalData.apitoken = res.data.apitoken;
          app.globalData.apiurl = !!res.data.apiurl?res.data.apiurl:that.data.objectArray[0].apiurl;
          app.globalData.PhotoUrl = !!res.data.PhotoUrl?res.data.PhotoUrl:that.data.objectArray[0].PhotoUrl;;
          for(var i=0;i<that.data.objectArray.length;i++)
          {
            if(that.data.objectArray[i].apiurl == res.data.apiurl)
            {
              that.setData({
                arrIndex: i
              })
            }
          }
          if (that.data.userid) {
            that.getTaskCount();
            that.getRoleMenu();
          }
        }
        else {
          app.globalData.apiurl =that.data.objectArray[0].apiurl;
          app.globalData.PhotoUrl = that.data.objectArray[0].PhotoUrl;;
          app.getUserInfo().then(function (obj) {
            that.setData({
              username: obj.username,
              userid: obj.userid,
              mainurl: obj.mainurl
            });
            if (that.data.userid) {
              that.getTaskCount();
              that.getRoleMenu();
            }

          });

        }
      }

    });
  }

});

