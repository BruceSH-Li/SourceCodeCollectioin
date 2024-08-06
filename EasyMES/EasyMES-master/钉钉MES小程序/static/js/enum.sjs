function JobType(num) {
  switch (num) {
    case 0:
      return "设备领料";
    case 1:
      return "物料入库";
    case 2:
      return "产品出库";
    case 3:
      return "产出上架";
    case 4:
      return "物料变动";
    case 5:
      return "工单作业";
    case 6:
      return "工单结束";
    case 7:
      return "物料退回";
    case 8:
      return "常规质检";
    case 9:
      return "二次质检";
    default:
      return "";
  }

}

//库存来源
function MaterialSourceType(num) {
  switch (num) {
    case 0:
      return "入库";
    case 1:
      return "领用";
    case 2:
      return "产出";
    case 3:
      return "库存";
    default:
      return "";
  }

}
//检验类型
function CheckoutType(num) {
  switch (num) {
    case 0:
      return "入库检验";
    case 1:
      return "领用检验";
    case 2:
      return "产出检验";
    case 3:
      return "库存检验";
    case 4:
      return "出库检验";
    default:
      return "";
  }

}
//检验类型
function ScapMaterial(num) {
  switch (num) {
    case 0:
      return "可疑";
    case 1:
      return "报废";
    case 2:
      return "正常";
    default:
      return "";
  }

}

//故障分类
function ProblemType(num) {
  switch (num) {
    case 1:
      return "人";
    case 2:
      return "机";
    case 3:
      return "料";
    case 4:
      return "法";
    case 5:
      return "环";
    default:
      return "";
  }

}

//故障等级
function ProblemClass(num) {
  switch (num) {
    case "1":
      return "低级";
    case "2":
      return "中级";
    case "3":
      return "高级";
    default:
      return "";
  }

}

//维修单状态
function RepairStatus(num) {
  switch (num) {
    case "1":
      return "待派工";
    case "2":
      return "待接单";
    case "3":
      return "维修中";
    case "4":
      return "待确认";
    case "99":
      return "已完成";
    default:
      return "";
  }

}

//维修单状态颜色
function RepairStatusColor(num) {
  switch (num) {
    case "1":
      return "red";
    case "2":
      return "blue";
    case "3":
      return "yellow";
    case "4":
      return "orange";
    case "99":
      return "green";
    default:
      return "";
  }

}

//维修人状态
function RepairManStatus(num) {
  switch (num) {
    case "1":
      return "待接单";
    case "2":
      return "维修中";
    case "99":
      return "维修结束";
    default:
      return "";
  }

}

// var ProblemTypeJson = [
//   {
//     F_ProblemType: 1,
//     F_ProblemTypeDesc: "人"
//   },
//   {
//     F_ProblemType: 2,
//     F_ProblemTypeDesc: "机"
//   },
//   {
//     F_ProblemType: 3,
//     F_ProblemTypeDesc: "料"
//   },
//   {
//     F_ProblemType: 4,
//     F_ProblemTypeDesc: "法"
//   },
//   {
//     F_ProblemType: 5,
//     F_ProblemTypeDesc: "环"
//   }
// ]




export default {
  JobType: JobType,
  MaterialSourceType: MaterialSourceType,
  CheckoutType: CheckoutType,
  ScapMaterial: ScapMaterial,
  // ProblemTypeJson: ProblemTypeJson
  ProblemType: ProblemType,
  ProblemClass: ProblemClass,
  RepairManStatus: RepairManStatus,
  RepairStatus: RepairStatus,
  RepairStatusColor:RepairStatusColor
}