using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterCloud.Domain.ProcessManage
{
    [TableAttribute("mes_WorkOrder")]
    public class WorkOrderExtend : WorkOrderEntity
    {
        /// <summary>
        /// 料号编号
        /// </summary>
        /// <returns></returns>
        public string F_MaterialCode { get; set; }
        /// <summary>
        /// 料号名称
        /// </summary>
        /// <returns></returns>
        public string F_MaterialName { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        /// <returns></returns>
        public string F_MaterialDescription { get; set; }
        /// <summary>
        /// 料号单位
        /// </summary>
        /// <returns></returns>
        public string F_MaterialUnit { get; set; }
        /// <summary>
        /// 料号规格
        /// </summary>
        /// <returns></returns>
        public string F_MaterialSize { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        /// <returns></returns>
        public string F_CreatorUserName { get; set; }
        public string classNum { get; set; }
        public DateTime? planDate { get; set; }
        /// <summary>
        /// 子订单数量
        /// </summary>
        /// <returns></returns>
        public int? F_OrderNum { get; set; }
        public List<WorkOrderDetailExtend> details { get; set; }

    }
}
