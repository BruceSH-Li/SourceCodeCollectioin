using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-10 15:47
    /// 描 述：设备停机实体类
    /// </summary>
    [TableAttribute("mes_EqpStopRecord")]
    public class EqpStopRecordEntity : IEntity<EqpStopRecordEntity>, ICreationAudited, IModificationAudited
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 1：故障停机，2：计划内停机 ，3:急停停机
        /// </summary>
        public int? F_StopType { get; set; }
        /// <summary>
        /// 计划停机开始时间
        /// </summary>
        public DateTime? F_PlanStopStartTime { get; set; }
        /// <summary>
        /// 计划停机结束时间
        /// </summary>
        public DateTime? F_PlanStopEndTime { get; set; }
        /// <summary>
        /// 停机原因分类：1人2机3料4法5环
        /// </summary>
        public string F_StopReasonType { get; set; }
        /// <summary>
        /// 停机原因
        /// </summary>
        public string F_StopReason { get; set; }
        /// <summary>
        /// 实际停机开始时间
        /// </summary>
        public DateTime? F_RealStopStartTime { get; set; }
        /// <summary>
        /// 实际停机结束时间
        /// </summary>
        public DateTime? F_RealStopEndTime { get; set; }
        /// <summary>
        /// 解决措施
        /// </summary>
        public string F_Solution { get; set; }
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

        public bool? F_DeleteMark { get; set; }
        public bool? F_EnabledMark { get; set; }
        public string F_EqpName { get; set; }
        public string F_IsRecovery { get; set; }

        public string F_Shift { get; set; }
        public string F_Day { get; set; }
        public string F_RepeatWeekday { get; set; }

    }

    public class CallBackData
    {
        public EMERGENCY_STOP emergency_stop { set; get; } = new EMERGENCY_STOP();
        public int xiaocheok { set; get; }
    }

    public class EMERGENCY_STOP
    {
        public string id { set; get; }
        public string eqpid { set; get; }

        public string addtime { set; get; }

        public string reason { set; get; }

        public string is_recovery { set; get; }
        public string recovery_time { set; get; }


    }

}
