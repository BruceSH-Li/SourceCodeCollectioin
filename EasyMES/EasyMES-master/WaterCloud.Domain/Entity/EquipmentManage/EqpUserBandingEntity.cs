using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-17 08:33
    /// 描 述：设备用户绑定实体类
    /// </summary>
    [TableAttribute("mes_EqpUserBanding")]
    public class EqpUserBandingEntity : IEntity<EqpUserBandingEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 设备id
        /// </summary>
        /// <returns></returns>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        /// <returns></returns>
        public string F_UserId { get; set; }
    }
}
