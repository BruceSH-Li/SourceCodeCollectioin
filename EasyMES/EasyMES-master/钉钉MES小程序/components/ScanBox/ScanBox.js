Component({
  mixins: [],
  props: {
    // label: {
    //   type: String,
    //   value: ""
    // },

    //表单提交参数名
    name: {
      type: String,
      value: ""
    },
    
    onScanCallBack: (data) => { },//父页面自定义回调
    onSearchCallBack: (data) => { }

  },
  data: {
    scanCode: ''
  },
  didMount() {
    var a = this.props.value;
    // debugger;
    if (!!this.props.value) {
      this.setData({
        scanCode: this.props.value
      })
    }
  },
  didUpdate() { },
  didUnmount() { },

  methods: {
    //监听输入框输入事件
    onSearch: function (event) {
      var that = this;
      var timer = setTimeout(() => {
        if (event.detail.value == that.data.scanCode) return;
        return new Promise((resolve, reject) => {
          that.setData({
            scanCode: event.detail.value
          });
          if (!event.detail.value) return;
          that.props.onSearchCallBack(event.detail.value);
        })
      }, 100);//延迟100ms，如果值未变化则直接返回，不执行回调

    },
    //监听扫码事件
    onScan: function (event) {
      var that = this;
      return new Promise((resolve, reject) => {
        dd.scan({
          type: 'qr',
          success: (res) => {
            that.setData({
              scanCode: res.code,
              // storageHidden: true//父页面详细信息的隐藏和显示可写在自定义回调里面
            });
            if (!res.code) return;
            that.props.onScanCallBack(res.code);
          }
        });
      })
    }
  },
});
