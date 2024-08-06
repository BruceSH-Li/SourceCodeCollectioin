using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-16 13:37
    /// 描 述：入库管理实体类
    /// </summary>
    [TableAttribute("mes_InStorage")]
    public class InStorageEntity : IEntity<InStorageEntity>,ICreationAudited, IModificationAudited
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 入库单编号
        /// </summary>
        /// <returns></returns>
        public string F_InStorageCode { get; set; }
        /// <summary>
        /// 料号Id
        /// </summary>
        /// <returns></returns>
        public string F_MaterialId { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        /// <returns></returns>
        public string F_MaterialCode { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        /// <returns></returns>
        public string F_MaterialName { get; set; }
        /// <summary>
        /// 料号单位
        /// </summary>
        /// <returns></returns>
        public string F_MaterialUnit { get; set; }
        /// <summary>
        /// 料号分类
        /// </summary>
        /// <returns></returns>
        public int? F_MaterialType { get; set; }
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
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        /// <returns></returns>
        public string F_CreatorUserId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        public float? F_Num { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        /// <returns></returns>
        public float? F_DoneNum { get; set; }
        /// <summary>
        /// 不良品
        /// </summary>
        public float? F_BadNum { get; set; }
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
        public string F_BatchJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int? F_InStorageState { get; set; }
        public DateTime? F_PlanTime { get; set; }
        public string F_CarNo { get; set; }
        [NotMapped]
        public float? F_StorageNum { get; set; }
        [NotMapped]
        public List<InStorageInfoEntity> list { get; set; }
        [NotMapped]
        public int? F_ContainerNum { get; set; }
        public bool? F_IsUserEdit { get; set; }
    }
}
