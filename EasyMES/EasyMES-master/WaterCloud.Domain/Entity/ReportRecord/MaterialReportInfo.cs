using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.ReportRecord
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-02-05 09:38
    /// 描 述：物料追溯类
    /// </summary>
    public class MaterialReportInfo
    {
        public string id { get; set; }
        /// <summary>
        /// 记录类型(0入库,1入库绑定,2产出,3变动增加,4消耗产出,5出库,6变动减少,7不良)
        /// </summary>
        public int? logType { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string materialCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string materialName { get; set; }
        /// <summary>
        /// 批号
        /// </summary>
        public string materialBatch { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public float? num { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? sortCode { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 入库批号
        /// </summary>
        public string oldMaterialBatch { get; set; }
        public string eqpName { get; set; }
        public string processName { get; set; }

    }
}

