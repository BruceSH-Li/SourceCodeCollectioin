using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-16 13:38
    /// 描 述：出库管理实体类
    /// </summary>
    [TableAttribute("mes_OutStorage")]
    public class OutStorageEntity : IEntity<OutStorageEntity>,ICreationAudited, IModificationAudited
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 出库单编号
        /// </summary>
        /// <returns></returns>
        public string F_OutStorageCode { get; set; }
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
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? F_LastModifyTime { get; set; }
        public DateTime? F_PlanTime { get; set; }
        public string F_CarNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_LastModifyUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int? F_OutStorageState { get; set; }
        [NotMapped]
        public List<OutStorageInfoEntity> list { get; set; }
        [NotMapped]
        /// <summary>
        /// 绑定数量
        /// </summary>
        /// <returns></returns>
        public float? F_StorageNum { get; set; }
        [NotMapped]
        public bool? F_IsOk { get; set; }
        [NotMapped]
        public string F_OrderCode { get; set; }
        public string F_OrderId { get; set; }
        public bool? F_IsUserEdit { get; set; }

    }
}
