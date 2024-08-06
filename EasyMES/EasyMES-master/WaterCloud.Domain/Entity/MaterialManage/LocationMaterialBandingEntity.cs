using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-04-15 12:25
    /// 描 述：库位物料绑定表实体类
    /// </summary>
    [TableAttribute("mes_LocationMaterialBanding")]
    public class LocationMaterialBandingEntity : IEntity<LocationMaterialBandingEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 库位id
        /// </summary>
        public string F_LocationId { get; set; }
        /// <summary>
        /// 物料id
        /// </summary>
        public string F_MaterialId { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public string F_MaterialCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string F_MaterialName { get; set; }
    }
}
