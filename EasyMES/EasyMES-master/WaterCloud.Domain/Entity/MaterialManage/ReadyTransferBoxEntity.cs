using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-07-05 14:52
    /// 描 述：预绑定流转箱实体类
    /// </summary>
    [TableAttribute("mes_ReadyTransferBox")]
    public class ReadyTransferBoxEntity : IEntity<ReadyTransferBoxEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 流转箱编号
        /// </summary>
        public string F_TransferBoxCode { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_CreatorTime { get; set; }
    }
}
