using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-03 13:55
    /// 描 述：Bom结构实体类
    /// </summary>
    [TableAttribute("mes_BomForm")]
    public class BomFormEntity : IEntity<BomFormEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
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
        public string F_MaterialId { get; set; }
        /// <summary>
        /// 组成主键-n
        /// </summary>
        /// <returns></returns>
        public string F_SonMaterialId { get; set; }
        /// <summary>
        /// Bom类型（0erp,1mes）
        /// </summary>
        /// <returns></returns>
        public int? F_BomType { get; set; }
        /// <summary>
        /// 工序主键
        /// </summary>
        /// <returns></returns>
        public string F_ProcessId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        public Single? F_Num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool? F_DeleteMark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool? F_EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        public string F_Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_CreatorUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? F_LastModifyTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_LastModifyUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? F_DeleteTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string F_DeleteUserId { get; set; }
    }
}
