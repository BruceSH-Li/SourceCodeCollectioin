using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-07-21 15:14
    /// 描 述：设备物料绑定实体类
    /// </summary>
    [TableAttribute("mes_EqpMaterialBanding")]
    public class EqpMaterialBandingEntity : IEntity<EqpMaterialBandingEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 料号Id
        /// </summary>
        public string F_MaterialId { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        public string F_MaterialCode { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        public string F_MaterialName { get; set; }
        /// <summary>
        /// 料号单位
        /// </summary>
        public string F_MaterialUnit { get; set; }
        /// <summary>
        /// 料号分类
        /// </summary>
        public int? F_MaterialType { get; set; }
        /// <summary>
        /// 产量数量
        /// </summary>
        public float? F_Num { get; set; }
        /// <summary>
        /// 生产模式(0独立，1共享)
        /// </summary>
        public int? F_ProduceType { get; set; }
        /// <summary>
        /// 从大到小
        /// </summary>
        public int? F_Priority { get; set; }
        
    }
}
