using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.ProcessManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-04-07 16:06
    /// 描 述：值班绑定表实体类
    /// </summary>
    [TableAttribute("mes_WorkDetailTimeBanding")]
    public class WorkDetailTimeBandingEntity : IEntity<WorkDetailTimeBandingEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        [AutoIncrement]
        public long F_Id { get; set; }
        /// <summary>
        /// 明细id
        /// </summary>
        public string F_WorkOrderDetailId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? F_EqpWorkTimeId { get; set; }
    }
}
