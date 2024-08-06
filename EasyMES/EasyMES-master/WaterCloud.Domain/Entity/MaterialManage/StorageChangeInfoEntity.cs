using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-31 08:56
    /// 描 述：库存变动信息实体类
    /// </summary>
    [TableAttribute("mes_StorageChangeInfo")]
    public class StorageChangeInfoEntity : IEntity<StorageChangeInfoEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        [AutoIncrement]
        public long F_Id { get; set; }
        public string F_StorageId { get; set; }
        public int? F_ChangeType { get; set; }
        /// <summary>
        /// 流转箱编号
        /// </summary>
        /// <returns></returns>
        public string F_TransferBoxCode { get; set; }
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
        /// 料号批次
        /// </summary>
        /// <returns></returns>
        public string F_MaterialBatch { get; set; }
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
        public string F_CreatorUserName { get; set; }
        /// <summary>
        /// 转移数量
        /// </summary>
        /// <returns></returns>
        public float? F_Num { get; set; }
        /// <summary>
        /// 库位编号
        /// </summary>
        /// <returns></returns>
        public string F_LocationCode { get; set; }
        /// <summary>
        /// 新库位编号
        /// </summary>
        /// <returns></returns>
        public string F_NewLocationCode { get; set; }
        /// <summary>
        /// 新流转箱编号
        /// </summary>
        /// <returns></returns>
        public string F_NewTransferBoxCode { get; set; }
        [NotMapped]
        public float? F_ChangeNum { get; set; }
    }
}
