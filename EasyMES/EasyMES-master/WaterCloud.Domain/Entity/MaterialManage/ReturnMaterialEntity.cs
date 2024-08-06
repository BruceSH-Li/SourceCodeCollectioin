using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-16 12:57
    /// 描 述：设备物料使用实体类
    /// </summary>
    [TableAttribute("mes_ReturnMaterial")]
    public class ReturnMaterialEntity : IEntity<ReturnMaterialEntity>
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
        /// <summary>
        /// 工序Id
        /// </summary>
        /// <returns></returns>
        public string F_ProcessId { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        /// <returns></returns>
        public string F_ProcessName { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        /// <returns></returns>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        /// <returns></returns>
        public string F_EqpName { get; set; }
        /// <summary>
        /// 工单Id
        /// </summary>
        /// <returns></returns>
        public string F_WorkOrderId { get; set; }
        /// <summary>
        /// 工单编号
        /// </summary>
        /// <returns></returns>
        public string F_WorkOrderCode { get; set; }
        /// <summary>
        /// 执行顺序
        /// </summary>
        /// <returns></returns>
        public int? F_WorkOrderRunSort { get; set; }
        public string F_LocationCode { get; set; }
    }
}
