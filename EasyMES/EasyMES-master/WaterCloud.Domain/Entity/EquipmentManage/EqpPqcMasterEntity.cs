using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-08 15:52
    /// 描 述：EqpPqcMasterEntity实体类
    /// </summary>
    [TableAttribute("mes_EqpPqcMaster")]
    public class EqpPqcMasterEntity : IEntity<EqpPqcMasterEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 点检月份
        /// </summary>
        public string F_CheckMonth { get; set; }
        /// <summary>
        /// 班别
        /// </summary>
        public string F_Shift { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Remark { get; set; }
    }
}
