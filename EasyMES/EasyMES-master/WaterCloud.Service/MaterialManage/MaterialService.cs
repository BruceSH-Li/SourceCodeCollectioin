using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Service.SystemManage;
using WaterCloud.Domain.ProcessManage;
using System.IO;
using WaterCloud.Domain.EquipmentManage;

namespace WaterCloud.Service.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-01 14:31
    /// 描 述：物料管理服务类
    /// </summary>
    public class MaterialService : DataFilterService<MaterialEntity>, IDenpendency
    {
        public ItemsDataService itemsApp { get; set; }
        private string cacheKey = "watercloud_materialdata_";

        public MaterialService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<MaterialEntity>> GetList(string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_MaterialCode.Contains(keyword) || t.F_MaterialName.Contains(keyword)).ToList();
            }
            return cachedata.Where(t => t.F_DeleteMark == false).OrderByDescending(t => t.F_CreatorTime).ToList();
        }
        public async Task<List<MaterialEntity>> GetList(Pagination pagination, string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_MaterialCode.Contains(keyword) || t.F_MaterialName.Contains(keyword)).ToList();
            }
            return cachedata.Where(t => t.F_DeleteMark == false).OrderByDescending(t => t.F_CreatorTime).Skip((pagination.page-1)*pagination.rows).Take(pagination.rows).ToList();
        }
        public async Task<List<MaterialEntity>> GetLookList(string keyword = "")
        {
            var query =repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_MaterialCode.Contains(keyword) || u.F_MaterialName.Contains(keyword));
            }
            query = GetDataPrivilege("u","",query);
            return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        }
        public async Task<List<MaterialEntity>> GetLookList(Pagination pagination, string keyword = "")
        {
            //获取数据权限
            var query = repository.IQueryable().Where(u => u.F_DeleteMark == false&& u.F_MaterialType != 2);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_MaterialCode.Contains(keyword) || u.F_MaterialName.Contains(keyword));
            }
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }
        public async Task<List<MaterialEntity>> GetLookList(SoulPage<MaterialEntity> pagination,string keyword = "",string id="" ,int? type=null)
        {
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> enabledTemp = new Dictionary<string, string>();
            enabledTemp.Add("有效", "1");
            enabledTemp.Add("无效", "0");
            dic.Add("F_EnabledMark", enabledTemp);
            Dictionary<string, string> checkoutTemp = new Dictionary<string, string>();
            checkoutTemp.Add("是", "1");
            checkoutTemp.Add("否", "0");
            dic.Add("F_NeedCheckout", checkoutTemp);
            var setList = await itemsApp.GetItemList("Mes_MaterialType");
            Dictionary<string, string> materialTypeTemp = new Dictionary<string, string>();
            foreach (var item in setList)
            {
                materialTypeTemp.Add(item.F_ItemName, item.F_ItemCode);
            }
            dic.Add("F_MaterialType", materialTypeTemp);
            pagination = ChangeSoulData(dic, pagination);
            //获取数据权限
            var query = repository.IQueryable().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_MaterialCode.Contains(keyword) || u.F_MaterialName.Contains(keyword));
            }
            if (type!=null)
            {
				query = query.Where(u => u.F_MaterialType == type);
			}
            if(!string.IsNullOrEmpty(id))
            {
                query= query.Where(u=>u.F_Id==id);
            }
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<MaterialEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }

        public async Task<List<MaterialEntity>> GetUseMaterialList(string eqpName, string keyword)
        {
            var query1 = uniwork.IQueryable<EquipmentEntity>(a=>a.F_EqpName==eqpName)
                .InnerJoin<WorkOrderDetailEqpBandingEntity>((a,b) => b.F_EqpId == a.F_Id)
                .InnerJoin<WorkOrderDetailEntity>((a, b,c) => b.F_WorkOrderDetailId == c.F_Id && c.F_WorkOrderState == 1)
                .InnerJoin<WorkOrderDetailProcessBandingEntity>((a,b,c,d)=>c.F_Id==d.F_WorkOrderDetailId)
                .InnerJoin<BomFormEntity>((a, b, c,d,e) => e.F_MaterialId == c.F_MaterialId&&e.F_ProcessId==d.F_ProcessId && e.F_BomType == 1)
                //.InnerJoin<MaterialEntity>((a, b, c, d,e) => d.F_SonMaterialId == e.F_Id)
                .Select((a, b, c, d,e) => e);

            var query=query1.InnerJoin<MaterialEntity>((d, e) => d.F_SonMaterialId == e.F_Id)
                                .Select((d, e) => e);

            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_MaterialCode.Contains(keyword) || u.F_MaterialName.Contains(keyword));
            }
            var data = query.Where(t => t.F_DeleteMark == false).OrderByDesc(t => t.F_CreatorTime).ToList();
			foreach (var item in data)
			{
                var uses = uniwork.IQueryable<EquipmentEntity>(a => a.F_EqpName == eqpName)
                .InnerJoin<WorkOrderDetailEqpBandingEntity>((a, b) => b.F_EqpId == a.F_Id)
                .InnerJoin<WorkOrderDetailEntity>((a, b, c) => b.F_WorkOrderDetailId == c.F_Id && c.F_WorkOrderState == 1)
                .InnerJoin<EqpMaterialUseEntity>((a, b, c, d) => c.F_WorkOrderId == d.F_WorkOrderId && a.F_Id == d.F_EqpId && d.F_MaterialId == item.F_Id)
                .Select((a, b, c, d) => d).ToList();
                var usesids = uses.Select(a => a.F_Id).ToList();
                var otheruses= uniwork.IQueryable<EquipmentEntity>(a => a.F_EqpName == eqpName)
                .InnerJoin<WorkOrderDetailEqpBandingEntity>((a, b) => b.F_EqpId == a.F_Id)
                .InnerJoin<WorkOrderDetailEntity>((a, b, c) => b.F_WorkOrderDetailId == c.F_Id && c.F_WorkOrderState == 1)
                .InnerJoin<EqpMaterialUseEntity>((a, b, c, d) => !usesids.Contains(d.F_Id) && a.F_Id == d.F_EqpId && d.F_MaterialId == item.F_Id && d.F_DoneNum != d.F_Num)
                .Select((a, b, c, d) => d).ToList();
                item.F_Num = uses.Sum(a => a.F_Num)??0;
                item.F_Num += otheruses.Sum(a => a.F_Num)??0;
                item.F_DoneNum = uses.Sum(a => a.F_DoneNum)??0+ otheruses.Sum(a => a.F_Num) ?? 0;
            }
            return data;
        }
        public async Task<List<MaterialEntity>> GetNeedMaterialList(string eqpId)
        {
            var query = uniwork.IQueryable<WorkOrderDetailEqpBandingEntity>(b => b.F_EqpId == eqpId)
                .InnerJoin<WorkOrderDetailEntity>((b, c) => b.F_WorkOrderDetailId == c.F_Id && c.F_WorkOrderState == 1)
                .InnerJoin<WorkOrderDetailProcessBandingEntity>((b, c, d) => c.F_Id == d.F_WorkOrderDetailId)
                .InnerJoin<BomFormEntity>((b, c, d,e) => e.F_MaterialId == c.F_MaterialId&&e.F_ProcessId==d.F_ProcessId && e.F_BomType == 1)
                .InnerJoin<MaterialEntity>((b, c, d, e,f) => e.F_SonMaterialId == f.F_Id)
                .Select((b, c, d, e,f) => f);
            return query.Where(t => t.F_DeleteMark == false).ToList();
        }
        public async Task<List<MaterialEntity>> GetOutMaterialList(string eqpName, string keyword)
        {
            var query = uniwork.IQueryable<EquipmentEntity>(a => a.F_EqpName == eqpName)
                .InnerJoin<WorkOrderDetailEqpBandingEntity>((a, b) => b.F_EqpId == a.F_Id)
                .InnerJoin<WorkOrderDetailEntity>((a, b,c) => b.F_WorkOrderDetailId == c.F_Id && c.F_WorkOrderState == 1)
                .InnerJoin<MaterialEntity>((a, b, c,d) => c.F_MaterialId == d.F_Id)
                .Select((a, b, c,d) => new MaterialEntity { 
                  F_BatchPrefix=a.F_BatchPrefix,
                  F_BatchType=a.F_BatchType,
                  F_CpkStData=d.F_CpkStData,
                  F_CreatorTime=d.F_CreatorTime,
                  F_CreatorUserId=d.F_CreatorUserId,
                  F_Customer=d.F_Customer,
                  F_CustomerProNO=d.F_CustomerProNO,
                  F_DeleteMark=d.F_DeleteMark,
                  F_Description=d.F_Description,
                  F_EnabledMark=d.F_EnabledMark,
                  F_Id=d.F_Id,
                  F_MaterialBy=d.F_MaterialBy,
                  F_MaterialCode=d.F_MaterialCode,
                  F_MaterialDescription=d.F_MaterialDescription,
                  F_MaterialName=d.F_MaterialName,
                  F_MaterialPic=d.F_MaterialPic,
                  F_MaterialSize=d.F_MaterialSize,
                  F_MaterialUnit=d.F_MaterialUnit,
                  F_MaterialType=d.F_MaterialType,
                  F_ModelRefer=d.F_ModelRefer,
                  F_NeedCheckout=d.F_NeedCheckout,
                  F_ContainerNum=d.F_ContainerNum,
                  F_ReceiveNum=d.F_ReceiveNum,
                });
            return query.Where(t => t.F_DeleteMark == false).ToList();
        }
        public async Task<List<MaterialEntity>> GetOutMaterialList(string eqpId)
        {
            var query = uniwork.IQueryable<EquipmentEntity>(a => a.F_Id == eqpId)
                .InnerJoin<WorkOrderDetailEqpBandingEntity>((a, b) => b.F_EqpId == a.F_Id)
                .InnerJoin<WorkOrderDetailEntity>((a, b, c) => b.F_WorkOrderDetailId == c.F_Id && c.F_WorkOrderState == 1)
                .InnerJoin<MaterialEntity>((a, b, c, d) => c.F_MaterialId == d.F_Id)
                .Select((a, b, c, d) => new MaterialEntity
                {
                    F_BatchPrefix = a.F_BatchPrefix,
                    F_BatchType = a.F_BatchType,
                    F_CpkStData = d.F_CpkStData,
                    F_CreatorTime = d.F_CreatorTime,
                    F_CreatorUserId = d.F_CreatorUserId,
                    F_Customer = d.F_Customer,
                    F_CustomerProNO = d.F_CustomerProNO,
                    F_DeleteMark = d.F_DeleteMark,
                    F_Description = d.F_Description,
                    F_EnabledMark = d.F_EnabledMark,
                    F_Id = d.F_Id,
                    F_MaterialBy = d.F_MaterialBy,
                    F_MaterialCode = d.F_MaterialCode,
                    F_MaterialDescription = d.F_MaterialDescription,
                    F_MaterialName = d.F_MaterialName,
                    F_MaterialPic = d.F_MaterialPic,
                    F_MaterialSize = d.F_MaterialSize,
                    F_MaterialUnit = d.F_MaterialUnit,
                    F_MaterialType = d.F_MaterialType,
                    F_ModelRefer = d.F_ModelRefer,
                    F_NeedCheckout = d.F_NeedCheckout,
                    F_ContainerNum = d.F_ContainerNum,
                    F_ReceiveNum = d.F_ReceiveNum,
                });
            return query.Where(t => t.F_DeleteMark == false).ToList();
        }
        #endregion

        public async Task<MaterialEntity> GetLookForm(object keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            //if (cachedata != null && !string.IsNullOrEmpty(cachedata.F_CpkStData))
            //    cachedata.oCpkStDataEntity = cachedata.F_CpkStData.ToList<CpkStDataEntity>();
            return GetFieldsFilterData(cachedata);
        }

        #region 提交数据
        public async Task SubmitForm(MaterialEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.Create();
                await repository.Insert(entity);
                await CacheHelper.Remove(cacheKey + "list");
            }
            else
            {
                //已经开始使用就不能修改名称
                if (uniwork.IQueryable<InStorageEntity>(a => a.F_MaterialId == keyValue).Count() > 0|| uniwork.IQueryable<WorkOrderDetailEntity>(a => a.F_MaterialId == keyValue).Count() > 0)
                {
                    entity.F_MaterialCode = null;
                }
                entity.Modify(keyValue);
                await repository.Update(entity);
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
            foreach (var item in ids)
            {
            await CacheHelper.Remove(cacheKey + item);
            }
            await CacheHelper.Remove(cacheKey + "list");
        }
        public async Task<List<MaterialEntity>> CheckFile(string fileFullName)
        {
            if (!FileHelper.IsExcel(fileFullName))
            {
                throw new Exception("文件不是有效的Excel文件!");
            }
            //文件解析
            var list = new ExcelHelper<MaterialEntity>().ImportFromExcel(fileFullName);
            //删除文件
            File.Delete(fileFullName);
            foreach (var item in list)
            {
                item.F_Id = Utils.GuId();
                item.F_EnabledMark = true;
                item.F_DeleteMark = false;
                item.F_ContainerNum = item.F_ContainerNum ?? 125; ;
                item.F_ReceiveNum = item.F_ReceiveNum ?? 2; ;
                List<string> str = new List<string>();
                if (string.IsNullOrEmpty(item.F_MaterialCode))
                {
                    item.F_EnabledMark = false;
                    item.ErrorMsg = "编号不存在";
                    continue;
                }
                else if (repository.IQueryable(a => a.F_MaterialCode == item.F_MaterialCode).Count() > 0 || list.Where(a => a.F_MaterialCode == item.F_MaterialCode).Count() > 1)
                {
                    str.Add("编号重复");
                    item.F_EnabledMark = false;
                }
                if (string.IsNullOrEmpty(item.F_MaterialName))
                {
                    str.Add("名称不存在");
                    item.F_EnabledMark = false;
                }
                if (item.F_MaterialType==null)
                {
                    str.Add("物料分类不存在");
                    item.F_EnabledMark = false;
                }
                if (item.F_MaterialUnit == null)
                {
                    str.Add("物料单位不存在");
                    item.F_EnabledMark = false;
                }
                if (item.F_EnabledMark == false)
                {
                    item.ErrorMsg = string.Join(',', str.ToArray());
                }
            }
            return list;
        }

        public async Task ImportForm(List<MaterialEntity> filterList)
        {
            foreach (var item in filterList)
            {
                item.Create();
            }
            await uniwork.Insert(filterList);
        }
        #endregion

    }
}
