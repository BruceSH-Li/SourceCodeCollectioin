﻿using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.SystemManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-09-09 13:49
    /// 描 述：报表模板历史实体类
    /// </summary>
    [TableAttribute("sys_ReportTemplateHis")]
    public class ReportTemplateHisEntity : IEntity<ReportTemplateHisEntity>,ICreationAudited
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 模板地址
        /// </summary>
        public string F_PrintTemplatePath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_CreatorUserId { get; set; }
        /// <summary>
        /// 模板类型 0json 1xlsx
        /// </summary>
        public int? F_PrintTemplateType { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        public string F_PrintTemplateJson { get; set; }

    }
}
