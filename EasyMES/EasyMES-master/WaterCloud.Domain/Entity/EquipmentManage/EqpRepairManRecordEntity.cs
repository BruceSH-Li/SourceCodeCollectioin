using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-16 13:57
    /// 描 述：派工维修实体类
    /// </summary>
    [TableAttribute("mes_EqpRepairManRecord")]
    public class EqpRepairManRecordEntity : IEntity<EqpRepairManRecordEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 设备维修记录表主键
        /// </summary>
        public string F_RepairId { get; set; }
        /// <summary>
        /// 维修人Id
        /// </summary>
        public string F_RepairMan { get; set; }
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
        /// 维修结果
        /// </summary>
        public string F_RepairResult { get; set; }
        /// <summary>
        /// 状态：1,派工，2，维修中，99，完成
        /// </summary>
        public string F_Status { get; set; }
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
        /// 维修人姓名
        /// </summary>
        public string F_RepairManName { get; set; }

        [NotMapped]
        public double repairhour { get; set; }
    }
}
