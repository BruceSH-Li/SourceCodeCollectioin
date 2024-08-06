using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.SystemManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-01-14 13:23
    /// 描 述：打印模板实体类
    /// </summary>
    [TableAttribute("sys_PrintTemplate")]
    public class PrintTemplateEntity : IEntity<PrintTemplateEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        /// <returns></returns>
        public string F_PrintTemplateName { get; set; }
        /// <summary>
        /// 模板地址
        /// </summary>
        /// <returns></returns>
        public string F_PrintTemplatePath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool? F_DeleteMark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool? F_EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        public string F_Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_CreatorUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? F_LastModifyTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_LastModifyUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? F_DeleteTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_DeleteUserId { get; set; }
        /// <summary>
        /// 模板描述
        /// </summary>
        /// <returns></returns>
        public string F_PrintTemplateDescription { get; set; }
        [NotMapped]
        public List<PrintTemplateHisEntity> list { get; set; }
    }
}
