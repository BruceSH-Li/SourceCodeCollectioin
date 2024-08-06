using System.Collections.Generic;
using WaterCloud.Domain.MaterialManage;

namespace WaterCloud.Domain.ClassTask
{
    public class ControlJobExtend : ControlJobEntity
    {
        /// <summary>
        /// 需求设备
        /// </summary>
        /// <returns></returns>
        public string F_NeedEqpName { get; set; }
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
        /// 完成数量
        /// </summary>
        /// <returns></returns>
        public float? F_FinishNum { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        /// <returns></returns>
        public float? F_NeedDoNum { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        public float? F_Num { get; set; }
        /// <summary>
        /// 完成绑定数量
        /// </summary>
        /// <returns></returns>
        public float? F_BandingNum { get; set; }
        /// <summary>
        /// 剩余绑定数量
        /// </summary>
        /// <returns></returns>
        public float? F_NeedBandingNum { get; set; }
        /// <summary>
        /// 入库单编号
        /// </summary>
        /// <returns></returns>
        public string F_InStorageCode { get; set; }
        /// <summary>
        /// 出库单编号
        /// </summary>
        /// <returns></returns>
        public string F_OutStorageCode { get; set; }
        /// <summary>
        /// 流转箱编号
        /// </summary>
        /// <returns></returns>
        public string F_TransferBoxCode { get; set; }
        /// <summary>
        /// 库位编号
        /// </summary>
        /// <returns></returns>
        public string F_LocationCode { get; set; }
        /// <summary>
        /// 库存列表
        /// </summary>
        public List<StorageEntity> storages { get; set; }
        /// <summary>
        /// 补货列表
        /// </summary>
        public List<EqpMaterialUseEntity> uses { get; set; }
        /// <summary>
        /// 绑定列表
        /// </summary>
        public List<InStorageInfoEntity> instorageTemps { get; set; }
        /// <summary>
        /// 上架列表
        /// </summary>
        public List<InStorageInfoEntity> instorages { get; set; }
        /// <summary>
        /// 下架列表
        /// </summary>
        public List<OutStorageInfoEntity> outstorages { get; set; }
        /// <summary>
        /// 库位列表
        /// </summary>
        public List<LocationEntity> locations { get; set; }

    }
}
