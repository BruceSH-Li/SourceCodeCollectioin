using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-17 08:31
    /// 描 述：库位管理实体类
    /// </summary>
    [TableAttribute("mes_Location")]
    public class LocationEntity : IEntity<LocationEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 库区id
        /// </summary>
        /// <returns></returns>
        public string F_AreaId { get; set; }
        /// <summary>
        /// 库区编号
        /// </summary>
        /// <returns></returns>
        public string F_AreaCode { get; set; }
        /// <summary>
        /// 库位编号
        /// </summary>
        /// <returns></returns>
        public string F_LocationCode { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        /// <returns></returns>
        public int? F_SortCode { get; set; }
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
        /// 
        /// </summary>
        /// <returns></returns>
        public bool? F_LocationState { get; set; }
        [NotMapped]
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public string F_MaterialCode { get; set; }
        public string F_MaterialName { get; set; }
        public int? F_LocationType { get; set; }
    }
}
