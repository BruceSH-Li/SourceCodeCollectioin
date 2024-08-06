var app = getApp();
var myRequest = require('/static/js/myRequest.js');
Page({
  data: {
    newList: [],
    status:0
  },
  onLoad(query) {

    var repairId = query.repairId;
    var status=query.status;
    
    this.getRepairFlowByRepairId(repairId,status);
  },

  getRepairFlowByRepairId: function (code,status) {
    var that = this;
    myRequest.get("/api/EqpRepair/getRepairFlowByRepairId?repairId=" + code).then((res) => {
      this.setData({
        newList:res.data.data,
        status:status
      })
    })
  }
});
