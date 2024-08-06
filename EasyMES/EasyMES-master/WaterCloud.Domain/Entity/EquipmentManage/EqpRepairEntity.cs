using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-15 12:37
    /// 描 述：设备维修记录表实体类
    /// </summary>
    [TableAttribute("mes_EqpRepair")]
    public class EqpRepairEntity : IEntity<EqpRepairEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 维修单号
        /// </summary>
        public string F_RepairNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 故障分类
        /// </summary>
        public int? F_ProblemType { get; set; }
        /// <summary>
        /// 故障等级：低级，中级，高级
        /// </summary>
        public string F_ProblemClass { get; set; }
        /// <summary>
        /// 故障描述
        /// </summary>
        public string F_ProblemDesc { get; set; }
        /// <summary>
        /// 故障图片路径
        /// </summary>
        public string F_PhotoPath { get; set; }
        /// <summary>
        /// 报修人ID
        /// </summary>
        public string F_CallRepairMan { get; set; }
        /// <summary>
        /// 报修时间
        /// </summary>
        public DateTime? F_CallRepairTime { get; set; }
        /// <summary>
        /// 派工时间
        /// </summary>
        public DateTime? F_DistributeTime { get; set; }
        /// <summary>
        /// 维修开始时间
        /// </summary>
        public DateTime? F_StartRepairTime { get; set; }
        /// <summary>
        /// 维修结束时间
        /// </summary>
        public DateTime? F_EndRepairTime { get; set; }
        /// <summary>
        /// 状态：1,报修；2，已派工；3,维修中；4，确认中，5，关闭
        /// </summary>
        public string F_Status { get; set; }
        /// <summary>
        /// 确认人ID,同报修人
        /// </summary>
        public string F_ConfirmMan { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime? F_ConfirmTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? F_DeleteMark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? F_EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_CreatorUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_LastModifyUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? F_DeleteTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_DeleteUserId { get; set; }

        public string F_EqpName { get; set; }
        public string F_CallRepairManName { get; set; }

        [NotMapped]
        public List<EqpRepairManRecordEntity> repariManlist { get; set; } = new List<EqpRepairManRecordEntity>();

    }
}
