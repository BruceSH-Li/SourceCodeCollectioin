using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.ProcessManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-15 10:07
    /// 描 述：生产计划实体类
    /// </summary>
    [TableAttribute("mes_WorkPlan")]
    public class WorkPlanEntity : IEntity<WorkPlanEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 料号Id
        /// </summary>
        public string F_MaterialId { get; set; }

        public string F_ProcessId { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public float? F_PlanNum { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public float? F_DoneNum { get; set; }
        /// <summary>
        /// 计划类型(0生产，1维护)
        /// </summary>
        public int? F_WorkPlanType { get; set; }
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
        /// <summary>
        /// 
        /// </summary>
        public DateTime F_Date { get; set; }
        /// <summary>
        /// 班别
        /// </summary>
        public string F_ClassNum { get; set; }
        public string F_EqpId{ get; set; }
        public string F_EqpName { get; set; }
        /// <summary>
        /// 料号编号
        /// </summary>
        [NotMapped]
        public string F_MaterialCode { get; set; }
        /// <summary>
        /// 料号名称
        /// </summary>
        [NotMapped]
        public string F_MaterialName { get; set; }
        /// <summary>
        /// 料号单位
        /// </summary>
        [NotMapped]
        public string F_MaterialUnit { get; set; }
        /// <summary>
        /// 料号分类
        /// </summary>
        [NotMapped]
        public int? F_MaterialType { get; set; }
        [NotMapped]
        public List<WorkPlanTemp> listData { get; set; }
        public bool? F_IsUserEdit { get; set; }
        [NotMapped]
        public int? piecenum { get; set; }
    }
    public class WorkPlanTemp
    {
        /// <summary>
        /// Id
        /// </summary>
        public string F_Id { get; set; }
        /// 
        public string F_MaterialId { get; set; }

        public string F_ProcessId { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public float? F_PlanNum { get; set; }

    }
}
