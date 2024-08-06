using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.PlanManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-07-21 14:57
    /// 描 述：订单明细实体类
    /// </summary>
    [TableAttribute("mes_OrderDetail")]
    public class OrderDetailEntity : IEntity<OrderDetailEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 订单Id
        /// </summary>
        public string F_OrderId { get; set; }
        /// <summary>
        /// 料号Id
        /// </summary>
        public string F_MaterialId { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        [NotMapped]
        public string F_MaterialCode { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        [NotMapped]
        public string F_MaterialName { get; set; }
        /// <summary>
        /// 料号单位
        /// </summary>
        [NotMapped]
        public string F_MaterialUnit { get; set; }
        /// <summary>
        /// 料号分类
        /// </summary>
        [NotMapped]
        public int? F_MaterialType { get; set; }
        /// <summary>
        /// 需求数量
        /// </summary>
        public float? F_NeedNum { get; set; }
        [NotMapped]
        public float? F_DoneNum { get; set; }
    }
}
