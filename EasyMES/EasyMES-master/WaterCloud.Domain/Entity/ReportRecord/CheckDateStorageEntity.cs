using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.ReportRecord
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-07-20 14:08
    /// 描 述：盘点表实体类
    /// </summary>
    [TableAttribute("mes_CheckDateStorage")]
    public class CheckDateStorageEntity : IEntity<CheckDateStorageEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 盘点类型
        /// </summary>
        public int? F_CheckStorageType { get; set; }
        /// <summary>
        /// 盘点日期
        /// </summary>
        public DateTime? F_CheckDate { get; set; }
        /// <summary>
        /// 班别（A白班，B夜班）
        /// </summary>
        public string F_ClassNum { get; set; }
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
        /// 上一班数量
        /// </summary>
        public float? F_BeginNum { get; set; }
        /// <summary>
        /// 当前数量
        /// </summary>
        public float? F_CurrentNum { get; set; }
        /// <summary>
        /// 当前托数
        /// </summary>
        public int? F_CurrentPallet { get; set; }
        /// <summary>
        /// 入库数量
        /// </summary>
        public float? F_InNum { get; set; }
        /// <summary>
        /// 出库数量
        /// </summary>
        public float? F_OutNum { get; set; }
        public string F_Description { get; set; }
    }
}
