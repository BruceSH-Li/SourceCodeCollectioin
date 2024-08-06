using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-17 08:33
    /// 描 述：设备区域绑定实体类
    /// </summary>
    [TableAttribute("mes_EqpAreaBanding")]
    public class EqpAreaBandingEntity : IEntity<EqpAreaBandingEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 流转箱编号
        /// </summary>
        /// <returns></returns>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 库区Id
        /// </summary>
        /// <returns></returns>
        public string F_AreaId { get; set; }
    }
}
