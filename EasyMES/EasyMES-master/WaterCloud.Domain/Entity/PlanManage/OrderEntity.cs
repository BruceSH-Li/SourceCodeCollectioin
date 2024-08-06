using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.PlanManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-07-21 14:57
    /// 描 述：订单管理实体类
    /// </summary>
    [TableAttribute("mes_Order")]
    public class OrderEntity : IEntity<OrderEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string F_OrderCode { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        public string F_Customer { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime? F_PlanStartTime { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime? F_PlanEndTime { get; set; }
        /// <summary>
        /// 天数
        /// </summary>
        public int? F_DayNum { get; set; }
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
        /// 需求数量
        /// </summary>
        [NotMapped]
        public float? F_NeedNum { get; set; }
        [NotMapped]
        public List<OrderDetailEntity> list { get; set; }
        public bool? F_IsFinish { get; set; }
        /// <summary>
        /// 预期结束时间
        /// </summary>
        public DateTime? F_PredictOverTime { get; set; }
        /// <summary>
        /// 实际完成时间
        /// </summary>
        public DateTime? F_ActualOverTime { get; set; }
        
    }
}
