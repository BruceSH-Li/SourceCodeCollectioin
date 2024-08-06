using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-14 12:44
    /// 描 述：流转箱类型物料绑定实体类
    /// </summary>
    [TableAttribute("mes_TransferTypeMaterialBanding")]
    public class TransferTypeMaterialBandingEntity : IEntity<TransferTypeMaterialBandingEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 流转箱类型
        /// </summary>
        /// <returns></returns>
        public string F_TransferType { get; set; }
        /// <summary>
        /// 物料Id
        /// </summary>
        /// <returns></returns>
        public string F_MaterialId { get; set; }
    }
}
