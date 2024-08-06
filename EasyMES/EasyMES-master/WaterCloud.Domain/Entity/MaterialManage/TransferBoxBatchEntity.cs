using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-17 14:19
    /// 描 述：流转箱打印实体类
    /// </summary>
    [TableAttribute("mes_TransferBoxBatch")]
    public class TransferBoxBatchEntity : IEntity<TransferBoxBatchEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 容器条码名称
        /// </summary>
        public string F_BatchName { get; set; }
        /// <summary>
        /// 容器最大数
        /// </summary>
        public int? F_MaxNum { get; set; }
        /// <summary>
        /// 条码当前编号
        /// </summary>
        public string F_BatchCode { get; set; }
        public long? F_BatchNum { get; set; }
        public bool? F_IsCheck { get; set; }
    }
}
