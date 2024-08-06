Page({
  data: {
    cacheKey: 'settings',
    taskslider: '5',
    taskInfoSwitch: true
  },
  onLoad() {
    var _settings = dd.getStorageSync({ key: this.data.cacheKey });
    this.data.settings=_settings;
    if (_settings.data) {
      this.setData({
        taskslider: _settings.data.taskslider,
        taskInfoSwitch:_settings.data.taskInfoSwitch
      });
    }
  },

  slider2change(e) {
    var _settings = this.data.settings;
    if (_settings.data) {
      _settings.data['taskslider'] = e.detail.value;
    } else {
      _settings.data = { 'taskslider': e.detail.value };
    }
    dd.setStorageSync({ key: this.data.cacheKey, data: _settings.data });
  },

  taskInfoSwitchChange(e) {
    var _settings = this.data.settings;
    if (_settings.data) {
      _settings.data['taskInfoSwitch'] = e.detail.value;
    } else {
      _settings.data = { 'taskInfoSwitch': e.detail.value };
    }
    dd.setStorageSync({ key: this.data.cacheKey, data: _settings.data });
  }




});
