using System;
using Chloe.Annotations;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-11-30 14:03
    /// 描 述：设备清单实体类
    /// </summary>
    [TableAttribute("mes_Equipment")]
    public class EquipmentEntity : IEntity<EquipmentEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        /// <returns></returns>
        public string F_EqpName { get; set; }
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
        /// 设备描述
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
        public int? F_EqpState { get; set; }
        /// <summary>
        /// 财产所属
        /// </summary>
        /// <returns></returns>
        public int? F_EqpOwner { get; set; }
        /// <summary>
        /// 接受日期
        /// </summary>
        /// <returns></returns>
        public DateTime? F_UseDate { get; set; }
        /// <summary>
        /// 预计使用次数
        /// </summary>
        /// <returns></returns>
        public int? F_PlanUserNum { get; set; }
        /// <summary>
        /// 已使用次数
        /// </summary>
        /// <returns></returns>
        public int? F_UsedNum { get; set; }
        /// <summary>
        /// 保养周期
        /// </summary>
        /// <returns></returns>
        public int? F_MaintainNum { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        /// <returns></returns>
        public int? F_EqpType { get; set; }
        /// <summary>
        /// 是否需要保养
        /// </summary>
        /// <returns></returns>
        public bool? F_IsMaintain { get; set; }
        /// <summary>
        /// 是否加载流动设备
        /// </summary>
        /// <returns></returns>
        public bool? F_IsOtherEqp { get; set; }
        /// <summary>
        /// 保养周期类型
        /// </summary>
        /// <returns></returns>
        public int? F_MaintainType { get; set; }
        /// <summary>
        /// 设备用途
        /// </summary>
        /// <returns></returns>
        public int? F_EqpUse { get; set; }
        /// <summary>
        /// 模具模数
        /// </summary>
        /// <returns></returns>
        public int? F_MoldingNum { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        /// <returns></returns>
        public string F_CompanyName { get; set; }
        /// <summary>
        /// 模具类型
        /// </summary>
        /// <returns></returns>
        public int? F_MouldType { get; set; }
        [NotMapped]
        //tablecheck字段
        public bool LAY_CHECKED { get; set; }
        [NotMapped]
        public bool? F_IsMaster { get; set; }
        [NotMapped]
        /// <summary>
        /// 模块数
        /// </summary>
        /// <returns></returns>
        public int? F_MouldNum { get; set; }
        /// <summary>
        /// 模具号
        /// </summary>
        public int? F_DieNum { get; set; }
        /// <summary>
        /// PLC状态
        /// </summary>
        public int? F_PlcState { get; set; }
        /// <summary>
        /// 批号前缀
        /// </summary>
        /// <returns></returns>
        public string F_BatchPrefix { get; set; }
        /// <summary>
        /// 批次类型(0、年月日班，1、年月日时分秒)
        /// </summary>
        public int? F_BatchType { get; set; }
        /// <summary>
        /// 产出数量限制
        /// </summary>
        public int? F_OutTransferNum { get; set; }
        /// <summary>
        /// 固定计划停机时间(min)
        /// </summary>
        public int? F_PlanStopTime { get; set; }
        /// <summary>
        /// 生产节拍(s)
        /// </summary>
        public float? F_Beat { get; set; }
        /// <summary>
        /// 默认OEE(%)
        /// </summary>
        public int? F_OEE { get; set; }

        [NotMapped]
        public int? userCount { get; set; }

        [NotMapped]
        public string userName { get; set; }
    }
}
