using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-01 14:31
    /// 描 述：物料设备绑定实体类
    /// </summary>
    [TableAttribute("mes_MaterialEqpBanding")]
    public class MaterialEqpBandingEntity : IEntity<MaterialEqpBandingEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 料号主键-1
        /// </summary>
        /// <returns></returns>
        [Required(ErrorMessage = "物料不能为空")]
        public string F_MaterialId { get; set; }
        /// <summary>
        /// 设备主键-n
        /// </summary>
        /// <returns></returns>
        [Required(ErrorMessage = "设备不能为空")]
        public string F_EqpId { get; set; }
        /// <summary>
        /// 设备用途
        /// </summary>
        /// <returns></returns>
        [Required(ErrorMessage = "设备用途不能为空")]
        public int? F_EqpUse { get; set; }

        /// <summary>
        /// 料号描述
        /// </summary>
        [NotMapped]
        public string MaterialDesc { get; set; }
    }
}
