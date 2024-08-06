using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.QualityManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-05-21 13:39
    /// 描 述：质量缺陷录入实体类
    /// </summary>
    [TableAttribute("mes_ScapImage")]
    public class ScapImageEntity : IEntity<ScapImageEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        [AutoIncrement]
        public long F_Id { get; set; }
        /// <summary>
        /// 报废id
        /// </summary>
        public long? F_ScapId { get; set; }
        public string F_FilePath { get; set; }
        /// <summary>
        /// 报废信息
        /// </summary>
        public string F_Description { get; set; }
        public string F_DescriptionId { get; set; }
    }
}
