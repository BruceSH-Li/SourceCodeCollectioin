using System.Collections.Generic;
using WaterCloud.Domain.MaterialManage;

namespace WaterCloud.Domain.ProcessManage
{
    public class WorkOrderDetailExtend : WorkOrderDetailEntity
    {
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
        /// 料号单位
        /// </summary>
        /// <returns></returns>
        public string F_MaterialUnit { get; set; }
        /// <summary>
        /// 料号分类
        /// </summary>
        /// <returns></returns>
        public int? F_MaterialType { get; set; }
        /// <summary>
        /// <summary>
        /// 创建人
        /// </summary>
        /// <returns></returns>
        public string F_CreatorUserName { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        /// <returns></returns>
        public string F_EqpId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        /// <returns></returns>
        public string F_EqpName { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        /// <returns></returns>
        public string F_EqpState { get; set; }
        /// <summary>
        /// 设备列表
        /// </summary>
        /// <returns></returns>
        public string listData { get; set; }
        /// <summary>
        /// 设备列表
        /// </summary>
        /// <returns></returns>
        public List<OutPutInfoEntity> list { get; set; }
        /// <summary>
        /// 合格率
        /// </summary>
        /// <returns></returns>
        public float? PassRate { get; set; }
        /// <summary>
        /// 完成率
        /// </summary>
        /// <returns></returns>
        public float? FinishRate { get; set; }
        public string EqpType { get; set; }
        public List<EqpWorkTimeEntity> userTimeList { get; set; }

        public int? piecenum { get; set; }
    }
}
