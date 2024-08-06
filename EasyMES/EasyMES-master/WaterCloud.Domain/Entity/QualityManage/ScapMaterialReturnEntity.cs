using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.QualityManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-07-13 15:34
    /// 描 述：质检回退实体类
    /// </summary>
    [TableAttribute("mes_ScapMaterialReturn")]
    public class ScapMaterialReturnEntity : IEntity<ScapMaterialReturnEntity>
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
        /// 可疑物料Id
        /// </summary>
        public long? F_ScapMaterialId { get; set; }
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
        /// 退回数量
        /// </summary>
        public float? F_Num { get; set; }
        /// <summary>
        /// 退回流转箱编号
        /// </summary>
        public string F_TransferBoxCode { get; set; }
        /// <summary>
        /// 退回料号批次
        /// </summary>
        public string F_BackMaterialBatch { get; set; }
        public bool? F_IsProduce { get; set; }

    }
}
