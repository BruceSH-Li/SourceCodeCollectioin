using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-01 14:31
    /// 描 述：物料管理实体类
    /// </summary>
    [TableAttribute("mes_Material")]
    public class MaterialEntity : IEntity<MaterialEntity>,ICreationAudited,IModificationAudited,IDeleteAudited
    {
        /// <summary>
        /// 料号id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 料号编号
        /// </summary>
        /// <returns></returns>
        public string F_MaterialCode { get; set; }
        /// <summary>
        /// 料号名称
        /// </summary>
        /// <returns></returns>
        public string F_MaterialName { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        /// <returns></returns>
        public string F_MaterialDescription { get; set; }
        /// <summary>
        /// 料号单位
        /// </summary>
        /// <returns></returns>
        public string F_MaterialUnit { get; set; }
        /// <summary>
        /// 料号规格
        /// </summary>
        /// <returns></returns>
        public string F_MaterialSize { get; set; }
        /// <summary>
        /// 料号材质
        /// </summary>
        /// <returns></returns>
        public string F_MaterialBy { get; set; }
        /// <summary>
        /// 料号分类
        /// </summary>
        /// <returns></returns>
        public int? F_MaterialType { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        /// <returns></returns>
        public string F_MaterialPic { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        /// <returns></returns>
        public string F_Customer { get; set; }
        /// <summary>
        /// 客户品号
        /// </summary>
        /// <returns></returns>
        public string F_CustomerProNO { get; set; }
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
        [NotMapped]
        //tablecheck字段
        public bool LAY_CHECKED { get; set; }
        [NotMapped]
        public float? F_Num { get; set; }
        [NotMapped]
        public string listData { get; set; }
        [NotMapped]
        public string F_ProcessId { get; set; }
        [NotMapped]
        public string F_SonMaterialId { get; set; }
        [NotMapped]
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 零件cpk标准Json字符串
        /// </summary>
        public string F_CpkStData { get; set; }
        /// <summary>
        /// 参考模型
        /// </summary>
        public string F_ModelRefer { get; set; }
        /// <summary>
        /// 是否需要检验
        /// </summary>
        public bool? F_NeedCheckout { get; set; }
        /// <summary>
        /// 预警值
        /// </summary>
        public int? F_WarmNum { get; set; }
        /// <summary>
        /// 容器值
        /// </summary>
        public int? F_ContainerNum { get; set; }
        public int? F_ReceiveNum { get; set; }

        /// <summary>
        /// 批号前缀
        /// </summary>
        [NotMapped]
        public string F_BatchPrefix { get; set; }
        /// <summary>
        /// 批次类型(0、年月日班，1、年月日时分秒)
        /// </summary>
        [NotMapped]
        public int? F_BatchType { get; set; }
        [NotMapped]
        public float? F_DoneNum { get; set; }
        [NotMapped]
        public float? F_StorageNum { get; set; }
        [NotMapped]
        public float? F_ReturnNum { get; set; }
        [NotMapped]
        public float? F_BandingNum { get; set; }

        //[NotMapped]
        //public List<CpkStDataEntity> oCpkStDataEntity { get; set; }
    }

    public class CpkStDataEntity
    {
        /// <summary>
        /// 分析字段
        /// </summary>
        public string field_name { get; set; }
        /// <summary>
        /// 标准值
        /// </summary>
        public string st { get; set; }
        /// <summary>
        /// 上偏差
        /// </summary>
        public string st_tu { get; set; }
        /// <summary>
        /// 下偏差
        /// </summary>
        public string st_tl { get; set; }
        /// <summary>
        /// 图像
        /// </summary>
        public string st_pic { get; set; }
    }

}
