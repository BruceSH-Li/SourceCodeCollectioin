using Chloe.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-08 12:32
    /// 描 述：点检项目实体类
    /// </summary>
    [TableAttribute("mes_EqpPqcCheckItem")]
    public class EqpPqcCheckItemEntity : IEntity<EqpPqcCheckItemEntity>, ICreationAudited, IModificationAudited, IDeleteAudited
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int? F_Sequence { get; set; }
        /// <summary>
        /// 点检项目
        /// </summary>
        public string F_CheckItem { get; set; }
        /// <summary>
        /// 检验基准
        /// </summary>
        public string F_CheckStand { get; set; }
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
    }
}
