using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.ProcessManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-03 09:47
    /// 描 述：工艺设备绑定实体类
    /// </summary>
    [TableAttribute("mes_ProcessEqpBanding")]
    public class ProcessEqpBandingEntity : IEntity<ProcessEqpBandingEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 工序主键-1
        /// </summary>
        /// <returns></returns>
        public string F_ProcessId { get; set; }
        /// <summary>
        /// 设备主键-1
        /// </summary>
        /// <returns></returns>
        public string F_EqpId { get; set; }
    }
}
