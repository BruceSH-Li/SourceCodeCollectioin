using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WaterCloud.Domain.EquipmentManage;

namespace WaterCloud.Domain.ProcessManage
{
    public class WorkPlanExtend
    {
        public string F_EqpName { get; set; }
        /// <summary>
        /// 计划类型(0生产，1维护,2仓库)
        /// </summary>
        public int? F_WorkPlanType { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public float? F_PlanNum { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public float? F_DoneNum { get; set; }
        public string F_MaterialName { get; set; }
        public EquipmentEntity eqp { get; set; }
        /// <summary>
        /// 入的数量
        /// </summary>
        public List<WorkPlanEntity> inlist { get; set; }
        /// <summary>
        /// 出的数量
        /// </summary>
        public List<WorkPlanEntity> outlist { get; set; }

    }
}
