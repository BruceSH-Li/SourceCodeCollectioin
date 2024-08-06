using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-15 15:11
    /// 描 述：入库表实体类
    /// </summary>
    [TableAttribute("mes_OutStorageInfo")]
    public class OutStorageInfoEntity : IEntity<OutStorageInfoEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        [AutoIncrement]
        public long F_Id { get; set; }
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
        /// 数量
        /// </summary>
        /// <returns></returns>
        public float? F_Num { get; set; }
        public string F_LocationCode { get; set; }
        /// <summary>
        /// 出库单id
        /// </summary>
        /// <returns></returns>
        public string F_OutStorageId { get; set; }
        [NotMapped]
        public string F_OutStorageCode { get; set; }
        /// <summary>
        /// 装箱容器
        /// </summary>
        public string F_TargetTransferCode { get; set; }
        /// <summary>
        /// 绑定人id
        /// </summary>
        /// <returns></returns>
        public string F_BandingUserId { get; set; }
        public string F_BandingUserName { get; set; }
        /// <summary>
        /// 绑定时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_BandingTime { get; set; }
        /// <summary>
        /// 临时标识未装箱
        /// </summary>
        public bool? F_IsTemp { get; set; }
        [NotMapped]
        public string TargetTransferCode { get; set; }

    }
}
