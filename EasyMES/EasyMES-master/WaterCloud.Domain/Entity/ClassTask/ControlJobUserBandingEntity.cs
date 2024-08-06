﻿using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.ClassTask
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-03 13:55
    /// 描 述：信息设置实体类
    /// </summary>
    [TableAttribute("mes_ControlJobUserBanding")]
    public class ControlJobUserBandingEntity : IEntity<ControlJobUserBandingEntity>
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 0补货，1入库，2出库，3产出上架，4移库，5作业开始，6作业结束,7领用退回
        /// </summary>
        public int? F_JobType { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string F_UserId { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string F_EqpId { get; set; }
        public string F_EqpName { get; set; }
        public string F_UserName { get; set; }
    }
}
