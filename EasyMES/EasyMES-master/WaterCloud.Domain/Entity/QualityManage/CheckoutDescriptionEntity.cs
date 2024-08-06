using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.QualityManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-04-23 15:23
    /// 描 述：质检参数实体类
    /// </summary>
    [TableAttribute("mes_CheckoutDescription")]
    public class CheckoutDescriptionEntity : IEntity<CheckoutDescriptionEntity>,ICreationAudited
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string F_CreatorUserId { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int? F_Layers { get; set; }
        /// <summary>
        /// 次数
        /// </summary>
        public long? F_ClickNum { get; set; }
    }

    public class PlatoEntity
    { 
        public string name { get; set; }
        public int? Qty { get; set; }
        public float? accumRate { get; set; }
    }
}
