using System;
using Chloe.Annotations;

namespace WaterCloud.Domain.DingTalkManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-11-21 09:52
    /// 描 述：应用配置实体类
    /// </summary>
    [TableAttribute("mob_DingTalkConfig")]
    public class DingTalkConfigEntity : IEntity<DingTalkConfigEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ColumnAttribute("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int? F_DingType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool? F_IsWebLogin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_AppName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_RedirectUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_AppId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_AppKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_AppSecret { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_AgentId { get; set; }
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
        /// 
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
        public string F_CreatorUserName { get; set; }
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
        /// 
        /// </summary>
        /// <returns></returns>
        public bool? F_IsDingInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_Url { get; set; }
        public bool? F_IsBoardLogin { get; set; }
    }
}
