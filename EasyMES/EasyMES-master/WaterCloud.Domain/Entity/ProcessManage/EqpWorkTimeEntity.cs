using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.ProcessManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-04-07 09:52
    /// 描 述：设备值班时间实体类
    /// </summary>
    [TableAttribute("mes_EqpWorkTime")]
    public class EqpWorkTimeEntity : IEntity<EqpWorkTimeEntity>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        [AutoIncrement]
        public long F_Id { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? F_StartTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? F_EndTime { get; set; }
        /// <summary>
        /// 计算以半小时算
        /// </summary>
        public float? F_TimeNum { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public string F_UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string F_UserName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        
    }
}
