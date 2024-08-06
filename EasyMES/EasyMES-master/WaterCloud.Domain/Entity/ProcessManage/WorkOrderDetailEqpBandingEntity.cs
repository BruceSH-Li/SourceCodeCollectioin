using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.ProcessManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-09 12:39
    /// 描 述：工单明细设备绑定实体类
    /// </summary>
    [TableAttribute("mes_WorkDetailEqpBanding")]
    public class WorkOrderDetailEqpBandingEntity : IEntity<WorkOrderDetailEqpBandingEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 工单明细Id
        /// </summary>
        /// <returns></returns>
        public string F_WorkOrderDetailId { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        /// <returns></returns>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        /// <returns></returns>
        public int? F_EqpType { get; set; }
        /// <summary>
        /// 设备用途
        /// </summary>
        /// <returns></returns>
        public int? F_EqpUse { get; set; }
        /// <summary>
        /// 模块数
        /// </summary>
        /// <returns></returns>
        public int? F_MouldNum { get; set; }
        /// <summary>
        /// 是否主设备
        /// </summary>
        /// <returns></returns>
        public bool? F_IsMaster { get; set; }
    }
}
