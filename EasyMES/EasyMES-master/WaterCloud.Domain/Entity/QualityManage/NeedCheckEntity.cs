using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.QualityManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-05-21 13:50
    /// 描 述：质检单管理实体类
    /// </summary>
    [TableAttribute("mes_NeedCheck")]
    public class NeedCheckEntity : IEntity<NeedCheckEntity>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        [AutoIncrement]
        public long F_Id { get; set; }
        public string F_SourceId { get; set; }
        /// <summary>
        /// 检验标识
        /// </summary>
        public bool? F_IsCheck { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 检验类型
        /// </summary>
        public int? F_CheckType { get; set; }
        /// <summary>
        /// 流转箱编号
        /// </summary>
        public string F_TransferBoxCode { get; set; }
        /// <summary>
        /// 料号Id
        /// </summary>
        public string F_MaterialId { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        public string F_MaterialCode { get; set; }
        /// <summary>
        /// 料号名称
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
        /// 库位号
        /// </summary>
        public string F_LocationCode { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string F_EqpName { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 检验数量
        /// </summary>
        public float? F_Num { get; set; }
        /// <summary>
        /// 报废数量
        /// </summary>
        [NotMapped]
        public float? F_ScapNum { get; set; }
        /// <summary>
        /// 质检信息
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 二次检验标识
        /// </summary>
        public bool? F_IsTwoCheck { get; set; }
        /// <summary>
        /// 是否二次检验
        /// </summary>
        public bool? F_IsNeedTwoCheck { get; set; }
        /// <summary>
        /// 检验完成时间
        /// </summary>
        public DateTime? F_FinishTime { get; set; }
        /// <summary>
        /// 二次检验完成时间
        /// </summary>
        public DateTime? F_TwoFinishTime { get; set; }
        /// <summary>
        /// 退回数量
        /// </summary>
        [NotMapped]
        public float? F_ReturnNum { get; set; }
        /// <summary>
        /// 可疑数量
        /// </summary>
        [NotMapped]
        public float? F_OtherNum { get; set; }
        [NotMapped]
        public List<ScapMaterialEntity> list { get; set; }
    }
}
