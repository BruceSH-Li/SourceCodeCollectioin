using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.ClassTask
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-01-15 14:18
    /// 描 述：待执行任务实体类
    /// </summary>
    [TableAttribute("mes_ControlJob")]
    public class ControlJobEntity : IEntity<ControlJobEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 调度编号
        /// </summary>
        /// <returns></returns>
        public string F_JobCode { get; set; }
        /// <summary>
        /// 调度信息
        /// </summary>
        /// <returns></returns>
        public string F_JobInfo { get; set; }
        /// <summary>
        /// 是否发送计划人员
        /// </summary>
        /// <returns></returns>
        public bool? F_IsToPlanMan { get; set; }
        /// <summary>
        /// 是否通知
        /// </summary>
        /// <returns></returns>
        public bool? F_IsNotice { get; set; }
        /// <summary>
        /// 优先级（从大到小执行）
        /// </summary>
        /// <returns></returns>
        public int? F_Priority { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_DoneTime { get; set; }
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
        /// 调度类型（0补货，1入库，2出库，3产出上架，4移库，5作业开始，6作业结束,7领用退回）
        /// </summary>
        /// <returns></returns>
        public int? F_JobType { get; set; }
        /// <summary>
        /// 需求id
        /// </summary>
        /// <returns></returns>
        public string F_NeedId { get; set; }
        public float? F_NeedNum { get; set; }
        /// <summary>
        /// 需求设备id
        /// </summary>
        /// <returns></returns>
        public string F_NeedEqpId { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_FinishTime { get; set; }
        /// <summary>
        /// 需求时间
        /// </summary>
        public DateTime? F_NeedTime { get; set; }
        /// <summary>
        /// 任务状态（0等待，1执行，2完成,3关闭）
        /// </summary>
        /// <returns></returns>
        public int? F_JobState { get; set; }
        /// <summary>
        /// 超时
        /// </summary>
        [NotMapped]
        public bool? F_IsOutTime { get; set; }
        [NotMapped]
        public string F_MaterialName { get; set; }
    }
}
