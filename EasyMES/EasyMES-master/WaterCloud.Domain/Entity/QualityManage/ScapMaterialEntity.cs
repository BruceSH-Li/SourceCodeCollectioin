using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.QualityManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-05-21 13:57
    /// 描 述：不良物料管理实体类
    /// </summary>
    [TableAttribute("mes_ScapMaterial")]
    public class ScapMaterialEntity : IEntity<ScapMaterialEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        [AutoIncrement]
        public long F_Id { get; set; }
        /// <summary>
        /// 条码编号
        /// </summary>
        public string F_BatchCode { get; set; }
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
        /// 料号批次
        /// </summary>
        public string F_MaterialBatch { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        public string F_CreatorUserId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string F_CreatorUserName { get; set; }
        /// <summary>
        /// 检验id
        /// </summary>
        public long? F_CheckId { get; set; }
        /// <summary>
        /// 检验类型
        /// </summary>
        public int? F_CheckType { get; set; }
        /// <summary>
        /// 检验完成
        /// </summary>
        public bool? F_IsCheck { get; set; }
        /// <summary>
        /// 临时表示还在常规质检
        /// </summary>
        public bool? F_IsTemp { get; set; }
        /// <summary>
        /// 检验数量
        /// </summary>
        public float? F_Num { get; set; }
        /// <summary>
        /// 报废数量
        /// </summary>
        public float? F_ScapNum { get; set; }
        /// <summary>
        /// 退回数量
        /// </summary>
        public float? F_ReturnNum { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        [NotMapped]
        public float? F_OtherNum { get; set; }
        [NotMapped]
        public List<ScapImageEntity> list { get; set; }
    }
}
