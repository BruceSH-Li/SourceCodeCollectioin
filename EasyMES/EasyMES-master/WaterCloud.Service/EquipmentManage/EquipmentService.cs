using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Service.SystemManage;

namespace WaterCloud.Service.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-11-30 14:03
    /// 描 述：设备清单服务类
    /// </summary>
    public class EquipmentService : DataFilterService<EquipmentEntity>, IDenpendency
    {
        private string cacheKey = "watercloud_equipmentdata_";
		private ItemsDataService itemsApp;
		public EquipmentService(IDbContext context) : base(context)
        {
			itemsApp = new ItemsDataService(context);
		}
        #region 获取数据
        public async Task<List<EquipmentEntity>> GetList(string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_EqpName.Contains(keyword) || t.F_Description.Contains(keyword)).ToList();
            }
            return cachedata.Where(t => t.F_DeleteMark == false && t.F_EnabledMark == true).OrderByDescending(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<EquipmentEntity>> GetLookList(string keyword = "")
        {
            var query = GetDataPrivilege("u").Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_EqpName.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            query = GetDataPrivilege("u","", query);
            return query.OrderBy("F_EqpType asc,F_EqpName asc").ToList();
        }

        public async Task<List<EquipmentEntity>> GetLookList(SoulPage<EquipmentEntity> pagination,string keyword = "",string id="")
        {
            //获取数据权限
            var query = repository.IQueryable().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_EqpName.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            if(!string.IsNullOrEmpty(id))
            {
                query= query.Where(u=>u.F_Id==id);
            }
            query = GetDataPrivilege("u","", query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<EquipmentEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        public async Task<EquipmentEntity> GetLookForm(object keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        #endregion

        #region 提交数据
        public async Task SubmitForm(EquipmentEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.F_EqpState = 0;
                entity.F_OutTransferNum = entity.F_OutTransferNum==null?1: entity.F_OutTransferNum;
                entity.F_PlanStopTime = entity.F_PlanStopTime == null ? 0 : entity.F_PlanStopTime;
                entity.Create();
                await repository.Insert(entity);
                await CacheHelper.Remove(cacheKey + "list");
            }
            else
            {
                //已经开始使用就不能修改名称
                if (uniwork.IQueryable<WorkOrderDetailEqpBandingEntity>(a => a.F_EqpId == keyValue).Count() > 0)
                {
                    entity.F_EqpName = null;
                }
                entity.Modify(keyValue);
                uniwork.BeginTrans();
                await repository.Update(entity);
                var list = uniwork.IQueryable<EqpMaterialBandingEntity>(a => a.F_EqpId == keyValue).ToList();
				var classNums = await itemsApp.GetItemList("Mes_ClassNumber");
				var classStartTime = TimeSpan.Parse(classNums.FirstOrDefault().F_Description.Split("-")[0]).TotalMinutes;
				var classEndTime = TimeSpan.Parse(classNums.FirstOrDefault().F_Description.Split("-")[1]).TotalMinutes;
                var totalMin = 720;
                if (classEndTime> classStartTime)
                {
                    totalMin = (int)(classEndTime - classStartTime);
				}
                else
                {
					totalMin = (int)(classEndTime + 24 * 60- classStartTime);
				}
				foreach (var item in list)
                {
                    double v1= (totalMin - (entity.F_PlanStopTime ?? 0)) *60 * (entity.F_OEE ?? 0);
                    double v2 = (double)entity.F_Beat * 100;
                    item.F_Num = (float)Utils.Round(v1 / v2, 0);
                    if (list.Where(a=>a.F_ProduceType==1).Count()>0 && item.F_ProduceType==1)
                    {
                        v2 = (double)entity.F_Beat * 100 * list.Where(a => a.F_ProduceType == 1).Count();
                        item.F_Num = (float)Utils.Round(v1 / v2, 0);
                    }
                    await uniwork.Update<EqpMaterialBandingEntity>(a => a.F_Id == item.F_Id, a => new EqpMaterialBandingEntity
                    {
                        F_Num = item.F_Num
                    });
                }
                uniwork.Commit();
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

        public async Task<List<EqpMaterialBandingEntity>> GetMaterialListJson(string keyValue)
        {
            var data = uniwork.IQueryable<EqpMaterialBandingEntity>(a => a.F_EqpId == keyValue).ToList();
            return data;
        }
        public async Task<List<MaterialEntity>> GetMaterialJson(string keyValue)
        {
            var data = uniwork.IQueryable<ProcessEqpBandingEntity>(b=> keyValue == b.F_EqpId).InnerJoin<BomFormEntity>((b,c)=>b.F_ProcessId==c.F_ProcessId).
                InnerJoin<MaterialEntity>((b,c,d)=>c.F_MaterialId==d.F_Id).Select((b,c,d)=>d).Distinct().ToList();
            return data;
        }
        public async Task SubmitMaterialForm(List<EqpMaterialBandingEntity> listData, string eqpId)
        {
            var eqp = await uniwork.FindEntity<EquipmentEntity>(eqpId);
			var classNums = await itemsApp.GetItemList("Mes_ClassNumber");
			var classStartTime = TimeSpan.Parse(classNums.FirstOrDefault().F_Description.Split("-")[0]).TotalMinutes;
			var classEndTime = TimeSpan.Parse(classNums.FirstOrDefault().F_Description.Split("-")[1]).TotalMinutes;
			var totalMin = 720;
			if (classEndTime > classStartTime)
			{
				totalMin = (int)(classEndTime - classStartTime);
			}
			else
			{
				totalMin = (int)(classEndTime + 24 * 60 - classStartTime);
			}
			foreach (var item in listData)
            {
                item.F_Id = Utils.GuId();
                if (item.F_Priority <= 0)
                {
                    throw new Exception("优先级不能小于0");
                }
                item.F_EqpId = eqpId;
                var material = await uniwork.FindEntity<MaterialEntity>(item.F_MaterialId);
                item.F_MaterialCode = material.F_MaterialCode;
                item.F_MaterialName = material.F_MaterialName;
                item.F_MaterialType = material.F_MaterialType;
                item.F_MaterialUnit = material.F_MaterialUnit;
				if (eqp.F_Beat != null && eqp.F_Beat != 0)
                {
                    double v1 = (totalMin - (eqp.F_PlanStopTime ?? 0)) * 60 * (eqp.F_OEE ?? 0);
                    double v2 = (double)eqp.F_Beat * 100;
                    item.F_Num = (float)Utils.Round(v1 / v2, 0);
                    if (listData.Where(a => a.F_ProduceType == 1).Count() > 0 && item.F_ProduceType == 1)
                    {
                        v2 = (double)eqp.F_Beat * 100 * listData.Where(a => a.F_ProduceType == 1).Count();
                        item.F_Num = (float)Utils.Round(v1 / v2, 0);
                    }
                }
                else
                {
                    throw new Exception("请去设备中添加节拍参数");
                }
            }
            uniwork.BeginTrans();
            await uniwork.Delete<EqpMaterialBandingEntity>(a => a.F_EqpId == eqpId);
            await uniwork.Insert(listData);
            uniwork.Commit();
        }
        #endregion

    }
}
