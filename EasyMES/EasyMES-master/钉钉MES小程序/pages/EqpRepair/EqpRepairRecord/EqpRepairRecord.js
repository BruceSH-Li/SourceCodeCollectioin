var app = getApp();
var myRequest = require('/static/js/myRequest.js');
import { debounce, throttle } from '/static/js/webutil.js';

Page({
  data: {
    tabs: [
      {
        title: '',
        subTitle: '',
        number: '',
        showBadge: false,
        badge: {
          arrow: false,
          stroke: true,
        },
      },

      // {
      //   title: '待接单',
      //   subTitle: '',
      //   number: '0',
      //   showBadge: true,
      //   badge: {
      //     arrow: false,
      //     stroke: true,
      //   },
      // },
      // {
      //   title: '维修中',
      //   subTitle: '',
      //   number: '0',
      //   showBadge: true,
      //   badge: {
      //     arrow: false,
      //     stroke: true,
      //   },
      // },
      // {
      //   title: '待确认',
      //   subTitle: '',
      //   number: '0',
      //   showBadge: true,
      //   badge: {
      //     arrow: true,
      //   },
      // },

    ],
    activeTab: 0,

    typeCapsule: false,
    typeHasSubTitle: true,

    contentHeight: [
      { name: 'has', value: '是' },
      { name: 'hasnt', value: '否', checked: true },
    ],

    userid: "",
    username: "",
    IsAdminRole: false,
    noAdminRole: true,
    query: [],

    showSearchButton: false,

    keyValue: "",

    currentPage: 1, // 设置加载的第几次，默认是第一次 
    pageSize: 5, // 每页条数    
    hasMoreData: true,
    Nodatadispaly: false

  },

  handleTabClick({ index, tabsName }) {
    this.setData({
      [tabsName]: index,
    });
  },
  handleTabChange({ index, tabsName }) {
    this.setData({
      [tabsName]: index,
    });
  },

  handleInput(value) {
    this.setData({
      keyValue: value,
      showSearchButton: true
    });
  },

  // handleInput: debounce(function (value) {
  //   console.log(value);
  //   this.setData({
  //     keyValue: value,
  //     showSearchButton: true
  //   });
  // }),

  handleClear() {
    this.setData({
      keyValue: '',
      showSearchButton: false
    });
  },
  handleCancel() {
    this.setData({
      keyValue: '',
      showSearchButton: false
    });
  },
  // handleSubmit(value) {
  //   this.setData({
  //     currentPage: 1,
  //     query: [],
  //     Nodatadispaly: false
  //   })
  //   this.getRepairInfoList();
  // },

  handleSubmit: throttle(function () {
    this.setData({
      currentPage: 1,
      query: [],
      Nodatadispaly: false
    })
    this.getRepairInfoList();
  }, 3000),

  onLoad(query) {

    if (query.activeTab) {
      this.setData({
        activeTab: parseInt(query.activeTab)
      })
    }

    dd.getStorage({
      key: 'userInfo',
      success: (res) => {
        this.setData({
          userid: res.data.userid,
          username: res.data.username
        })
        this.getRepairInfoList();
      }
    })



  },

  onShow() {

  },

  DetailRepair(e) {
    var RepairId = e.currentTarget.dataset.F_RepairId
    dd.navigateTo({            //保留当前页面，跳转到应用内的某个指定页面
      url: "/pages/EqpRepair/EqpRepairDetail/EqpRepairDetail?RepairId=" + RepairId + "&type=record"
    })
  },


  getRepairInfoList: function (code) {
    var that = this;
    myRequest.get("/api/EqpRepair/getAllRepairInfo?ddUserId=" + that.data.userid + "&currentPage=" + that.data.currentPage + "&pageSize=" + that.data.pageSize + "&keyValue=" + encodeURIComponent(that.data.keyValue)).then((res) => {
      var result = res.data.data;
      if (that.data.query.length == 0 && result.length == 0) {
        that.setData({
          Nodatadispaly: true
        })
        return;
      }
      if (result != null && result.length < that.data.pageSize) {
        that.setData({
          // noAdminRole: !result.IsAdminRole
          query: that.data.query.concat(result),
          hasMoreData: false
        });
      } else {
        that.setData({
          query: that.data.query.concat(result),
          hasMoreData: true,
          currentPage: that.data.currentPage + 1
        })
      }






    })
  },

  onReachBottom: function () {
    // console.log("12131");
    if (this.data.hasMoreData) {
      this.getRepairInfoList();

    }
  },

  onPullDownRefresh() {
    this.setData({
      query: [],
      currentPage: 1,
      Nodatadispaly: false
    })
    this.getRepairInfoList();
    dd.stopPullDownRefresh();
  },

});
