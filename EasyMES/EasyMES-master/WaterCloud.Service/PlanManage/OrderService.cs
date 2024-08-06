using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.PlanManage;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Service.MaterialManage;
using System.Net.Http;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.SystemSecurity;
using WaterCloud.Service.SystemManage;

namespace WaterCloud.Service.PlanManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-07-21 14:57
    /// 描 述：订单管理服务类
    /// </summary>
    public class OrderService : DataFilterService<OrderEntity>, IDenpendency
    {
        private StorageService storageApp;
        private ItemsDataService itemsApp;
		public OrderService(IDbContext context, IHttpClientFactory httpClientFactory) : base(context)
        {
            storageApp = new StorageService(context, httpClientFactory);
			itemsApp = new ItemsDataService(context);
		}
        #region 获取数据
        public async Task<List<OrderEntity>> GetList(string keyword = "")
        {
            var data = IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(t => t.F_OrderCode.Contains(keyword)
                || t.F_Description.Contains(keyword));
            }
            return data.Where(t => t.F_DeleteMark == false).OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<OrderEntity>> GetLookList(string keyword = "")
        {
            var query = IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.F_OrderCode.Contains(keyword)
                || t.F_Description.Contains(keyword));
            }
            //权限过滤
            query = GetDataPrivilege("u", "", query);
            return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<MaterialEntity>> GetMaterialList(string keyword)
        {
            //获取数据权限
            var list = uniwork.IQueryable<MaterialEntity>(a => a.F_DeleteMark == false && a.F_EnabledMark==true && a.F_MaterialType >0);
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(t => t.F_MaterialCode.Contains(keyword) || t.F_MaterialName.Contains(keyword));
            }
            return list.ToList();
        }

        public async Task<List<OrderEntity>> GetLookList(SoulPage<OrderEntity> pagination,string keyword = "",string id="",bool isFinish = true)
        {
            var query = IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.F_OrderCode.Contains(keyword)
                || t.F_Description.Contains(keyword));
            }
            if(!string.IsNullOrEmpty(id))
            {
                query= query.Where(u=>u.F_Id==id);
            }
            if (!isFinish)
            {
                query = query.Where(u => u.F_IsFinish == false);
            }
            //权限过滤
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        private IQuery<OrderEntity> IQueryable()
        {
            var details = uniwork.IQueryable<OrderDetailEntity>().GroupBy(a => a.F_OrderId).Select(a => new { a.F_OrderId, F_NeedNum = Sql.Sum(a.F_NeedNum) });
            var query = repository.IQueryable().InnerJoin(details, (a, b) => a.F_Id == b.F_OrderId)
                .Select((a, b) => new OrderEntity
                {
                    F_CreatorTime=a.F_CreatorTime,
                    F_CreatorUserId=a.F_CreatorUserId,
                    F_Customer=a.F_Customer,
                    F_DayNum=a.F_DayNum,
                    F_DeleteMark=a.F_DeleteMark,    
                    F_DeleteTime=a.F_DeleteTime,
                    F_DeleteUserId=a.F_DeleteUserId,
                    F_Description=a.F_Description,
                    F_EnabledMark=a.F_EnabledMark,
                    F_Id=a.F_Id,
                    F_LastModifyTime=a.F_LastModifyTime,
                    F_LastModifyUserId=a.F_LastModifyUserId,
                    F_NeedNum=b.F_NeedNum,
                    F_OrderCode=a.F_OrderCode,
                    F_PlanEndTime=a.F_PlanEndTime,
                    F_PlanStartTime=a.F_PlanStartTime,
                    F_IsFinish=a.F_IsFinish,
                    F_PredictOverTime=a.F_PredictOverTime,
                    F_ActualOverTime=a.F_ActualOverTime,
                });
            return query;
        }

        public async Task<OrderEntity> GetForm(string keyValue)
        {
            var data = IQueryable().Where(a => a.F_Id == keyValue).FirstOrDefault();
            var outstorages = uniwork.IQueryable<OutStorageEntity>(a => a.F_EnabledMark == true && a.F_OrderId != null).GroupBy(a => a.F_OrderId).AndBy(a => a.F_MaterialId)
                .Select(a => new { a.F_OrderId, a.F_MaterialId, F_Num = Sql.Sum(a.F_DoneNum) });
            data.list = uniwork.IQueryable<OrderDetailEntity>(a => a.F_OrderId == keyValue)
                .InnerJoin<MaterialEntity>((a, b) => a.F_MaterialId == b.F_Id)
                .LeftJoin(outstorages, (a, b, c) => a.F_OrderId == c.F_OrderId && a.F_MaterialId == c.F_MaterialId)
                .Select((a, b, c) => new OrderDetailEntity
                {
                    F_Id = a.F_Id,
                    F_MaterialCode = b.F_MaterialCode,
                    F_MaterialId = a.F_MaterialId,
                    F_MaterialName = b.F_MaterialName,
                    F_MaterialType = b.F_MaterialType,
                    F_MaterialUnit = b.F_MaterialUnit,
                    F_NeedNum = a.F_NeedNum,
                    F_OrderId = a.F_OrderId,
                    F_DoneNum = c.F_MaterialId == null ? 0 : c.F_Num
                }).ToList();
            return data;
        }
        #endregion

        public async Task<OrderEntity> GetLookForm(string keyValue)
        {
            var data = IQueryable().Where(a => a.F_Id == keyValue).FirstOrDefault();
            var outstorages = uniwork.IQueryable<OutStorageEntity>(a => a.F_EnabledMark == true && a.F_OrderId != null).GroupBy(a => a.F_OrderId).AndBy(a => a.F_MaterialId)
                .Select(a => new { a.F_OrderId, a.F_MaterialId, F_Num = Sql.Sum(a.F_DoneNum) });
            data.list = uniwork.IQueryable<OrderDetailEntity>(a => a.F_OrderId == keyValue)
                .InnerJoin<MaterialEntity>((a, b) => a.F_MaterialId == b.F_Id)
                .LeftJoin(outstorages, (a, b, c) => a.F_OrderId == c.F_OrderId && a.F_MaterialId == c.F_MaterialId)
                .Select((a, b, c) => new OrderDetailEntity
                {
                    F_Id = a.F_Id,
                    F_MaterialCode = b.F_MaterialCode,
                    F_MaterialId = a.F_MaterialId,
                    F_MaterialName = b.F_MaterialName,
                    F_MaterialType = b.F_MaterialType,
                    F_MaterialUnit = b.F_MaterialUnit,
                    F_NeedNum = a.F_NeedNum,
                    F_OrderId = a.F_OrderId,
                    F_DoneNum = c.F_MaterialId == null ? 0 : c.F_Num
                }).ToList();
            return GetFieldsFilterData(data);
        }

        #region 提交数据
        public async Task SubmitForm(OrderEntity entity, string keyValue)
        {
            if(string.IsNullOrEmpty(keyValue))
            {
                    //初始值添加
                entity.F_DeleteMark = false;
                entity.F_EnabledMark = false;
                entity.F_IsFinish = false;
                entity.F_PredictOverTime = null;
                entity.Create();
                var lastdate = repository.IQueryable().OrderByDesc(a => a.F_PlanEndTime).FirstOrDefault();
                if (lastdate!=null && lastdate.F_PlanEndTime > entity.F_PlanStartTime)
                {
                    throw new Exception("订单时间重合，无法创建");
                }
                foreach (var item in entity.list)
                {
                    item.F_Id = Utils.GuId();
                    item.F_OrderId = entity.F_Id;
                }
                uniwork.BeginTrans();
                await repository.Insert(entity);
                await uniwork.Insert(entity.list);
                uniwork.Commit();
            }
            else
            {
                    //此处需修改
                entity.Modify(keyValue);
                uniwork.BeginTrans();
                await repository.Update(entity);
                if (entity.F_EnabledMark == true)
                {
                    foreach (var item in entity.list)
                    {
                        await uniwork.Update<OrderDetailEntity>(a => a.F_Id == item.F_Id, a => new OrderDetailEntity
                        {
                            F_NeedNum = item.F_NeedNum
                        });
                        //数量变动
                    }
                }
                else
                {
                    foreach (var item in entity.list)
                    {
                        item.F_Id = Utils.GuId();
                        item.F_OrderId = entity.F_Id;
                    }
                    await uniwork.Delete<OrderDetailEntity>(a => a.F_OrderId == entity.F_Id);
                    await uniwork.Insert(entity.list);
                }
                uniwork.Commit();
            }
        }
        //删除订单
        public async Task DeleteForm(string keyValue)
        {
            if (repository.IQueryable(a => keyValue == a.F_Id && a.F_EnabledMark == true).Count() > 0)
            {
                if (uniwork.IQueryable<OutStorageEntity>(a => keyValue == a.F_Id && a.F_OutStorageState > 0).Count() > 0)
                {
                    throw new Exception("订单已经开始执行，无法删除");
                }
            }
            uniwork.BeginTrans();
            await repository.Delete(a => keyValue == a.F_Id);
            await uniwork.Delete<OutStorageEntity>(a => keyValue == a.F_Id);
            uniwork.Commit();
        }
        //结案订单
        public async Task OverForm(string keyValue)
        {
            if (repository.IQueryable(a => keyValue == a.F_Id && a.F_EnabledMark == true && a.F_IsFinish == true).Count() > 0)
            {
                throw new Exception("订单已经开始结案，无法结案");
            }
            await repository.Update(a => keyValue == a.F_Id && a.F_EnabledMark == true && a.F_IsFinish == false, a => new OrderEntity
            {
                F_IsFinish=true,
                F_ActualOverTime=DateTime.Now.Date
            });
        }
        //启用订单,重新排计划
        public async Task EnabledAndResetForm(string keyValue = "")
        {
            var ordernotes = repository.IQueryable(a => a.F_EnabledMark == true && a.F_IsFinish == false && a.F_PlanEndTime > DateTime.Now.Date).OrderBy(a => a.F_PlanStartTime).ToList();
            if (!string.IsNullOrEmpty(keyValue) && ordernotes.Where(a => a.F_Id == keyValue).Count() == 0)
            {
                var ordertemp = await repository.FindEntity(keyValue);
                ordernotes.Add(ordertemp);
            }
            var orderIds = ordernotes.Select(a => a.F_Id).ToList();
            //获取当前天
            DateTime currentdate = DateTime.Now.Date;
			//班别
			var classNums = await itemsApp.GetItemList("Mes_ClassNumber");
			var classStartTime = TimeSpan.Parse(classNums.FirstOrDefault().F_Description.Split("-")[0]);
			var currentTime = DateTime.Now.TimeOfDay;
			if (TimeSpan.Compare(currentTime, classStartTime) < 0)
			{
				currentdate = currentdate.AddDays(-1);
            }
            //到期的订单先结案
            var overnote = ordernotes.Where(a => a.F_PlanEndTime == DateTime.Now.Date.AddDays(-1)).FirstOrDefault();
            if (overnote != null)
            {
                ordernotes = ordernotes.Where(a => a.F_Id != overnote.F_Id).ToList();
                await uniwork.Update<OrderEntity>(a => overnote.F_Id == a.F_Id && a.F_EnabledMark == true && a.F_IsFinish == false, a => new OrderEntity
                {
                    F_IsFinish = true,
                    F_ActualOverTime = DateTime.Now.Date
                });
                await uniwork.Update<OutStorageEntity>(a => overnote.F_Id == a.F_OrderId && a.F_OutStorageState == 0 && a.F_MaterialType == 2 && a.F_PlanTime > DateTime.Now.Date, a => new OutStorageEntity
                {
                    F_OrderId = null
                });
                var otherorder = ordernotes.OrderBy(a => a.F_PlanStartTime).FirstOrDefault();
                if (otherorder != null)
                {
                    await uniwork.Update<OutStorageEntity>(a => a.F_OutStorageState == 0 && a.F_MaterialType==2 && a.F_OrderId == null && a.F_PlanTime > DateTime.Now.Date, a => new OutStorageEntity
                    {
                        F_OrderId = otherorder.F_Id
                    });
                }
            }
            //采购计划、交付计划、生产计划
            List<InStorageEntity> inStorages = new List<InStorageEntity>();
            List<OutStorageEntity> outStorages = new List<OutStorageEntity>();
            List<WorkPlanEntity> workPlans = new List<WorkPlanEntity>();
            //订单数量
            var details = uniwork.IQueryable<OrderDetailEntity>(a => orderIds.Contains(a.F_OrderId)).ToList();
            foreach (var item in details)
            {
                item.F_DoneNum = 0;
            }
            var doneouts = uniwork.IQueryable<OutStorageEntity>(a => (orderIds.Contains(a.F_OrderId) && a.F_OutStorageState > 0) || a.F_PlanTime == currentdate).ToList();
            //更新订单完成数量
            foreach (var item in doneouts)
            {
                var orderdetai = details.Where(a => a.F_OrderId == item.F_OrderId && a.F_MaterialId == item.F_MaterialId).FirstOrDefault();
                if (orderdetai != null)
                {
                    orderdetai.F_DoneNum += item.F_DoneNum ?? 0;
                }
            }
            //预期库存
            SortedDictionary<DateTime, List<MaterialEntity>> planStorage = new SortedDictionary<DateTime, List<MaterialEntity>>();
            //计算当前库存
            //安全库存值
            var currentStorages = await storageApp.GetCurrentClassNumStorage();
            //获取采购计划
            var inPlans = uniwork.IQueryable<InStorageEntity>(a => a.F_PlanTime >= currentdate).ToList();
            //获取交付计划
            var outPlans = uniwork.IQueryable<OutStorageEntity>(a => a.F_PlanTime >= currentdate).ToList();
            //获取生产计划
            var producePlans = uniwork.IQueryable<WorkPlanEntity>(a => a.F_Date >= currentdate).ToList();
            //获取安全库存
            var warmMaterials = uniwork.IQueryable<MaterialEntity>(a => a.F_EnabledMark == true && a.F_DeleteMark == false).OrderBy("F_MaterialType desc").ToList();
            //Bom表
            var allBoms = uniwork.IQueryable<BomFormEntity>(a => a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_BomType == 1).ToList();
            //设备物料绑定
            var eqpbandings = uniwork.IQueryable<EqpMaterialBandingEntity>().InnerJoin<EquipmentEntity>((a, b) => a.F_EqpId == b.F_Id)
                        .Select((a, b) => new { a.F_MaterialId, a.F_MaterialCode, a.F_MaterialName, a.F_MaterialType, a.F_MaterialUnit, a.F_ProduceType, a.F_Priority, a.F_Num, a.F_EqpId, b.F_EqpName })
                        .OrderBy("F_MaterialType desc,F_EqpName asc").ToList();
            //不生产的产品
            var candones = eqpbandings.Select(a => a.F_MaterialId).Distinct().ToList();
            //需求物料总表
            List<MaterialEntity> needMaterials = new List<MaterialEntity>();
            //订单需求物料总表
            List<MaterialEntity> orderMaterials = new List<MaterialEntity>();
            DateTime lasterDate = currentdate.AddDays(1);
            //今天的计划加进计划库存中
            var tempinplan = inPlans.Where(a => a.F_PlanTime < currentdate.AddDays(1) && a.F_InStorageState <= 2).ToList();
            var tempoutplan = outPlans.Where(a => a.F_PlanTime < currentdate.AddDays(1) && a.F_OutStorageState <= 2).ToList();
			if (TimeSpan.Compare(currentTime, classStartTime) < 0)
            {
                tempinplan = inPlans.Where(a => a.F_PlanTime < currentdate.AddDays(2) && a.F_InStorageState <= 2).ToList();
                tempoutplan = outPlans.Where(a => a.F_PlanTime < currentdate.AddDays(2) && a.F_OutStorageState <= 2).ToList();
            }
            var tempproduceplan = producePlans.Where(a => a.F_Date >= currentdate && a.F_Date < currentdate.AddDays(1)).ToList();
			for (int i = 0; i < classNums.Count(); i++)
			{
				var tempStartTime = TimeSpan.Parse(classNums[i].F_Description.Split("-")[0]);
				var tempEndTime = TimeSpan.Parse(classNums[i].F_Description.Split("-")[1]);
				if (TimeSpan.Compare(currentTime, tempStartTime) > 0 && TimeSpan.Compare(tempEndTime, currentTime) > 0)
				{
					for (int j = 0; j < i; j++)
					{
						tempproduceplan = tempproduceplan.Where(a => a.F_ClassNum != classNums[j].F_ItemCode).ToList();
					}
				}
			}
			foreach (var item in tempinplan)
            {
                var temp = currentStorages.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
                temp.F_Num += item.F_Num - item.F_DoneNum;
            }
            foreach (var item in tempoutplan)
            {
                var temp = currentStorages.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
                temp.F_Num -= item.F_Num - item.F_DoneNum;
            }
            foreach (var item in tempproduceplan)
            {
                var temp = currentStorages.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
                temp.F_Num += item.F_PlanNum - item.F_DoneNum;
                //消耗的物料
                var boms = allBoms.Where(a => a.F_MaterialId == temp.F_Id).ToList();
                foreach (var bom in boms)
                {
                    var tempson = currentStorages.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault();
                    tempson.F_Num -= bom.F_Num * (item.F_PlanNum - item.F_DoneNum);
                }
            }
            //判断预期库存是否满足交付，如果满足则只创建交付计划
            bool isOK = true;
            //删除计划
            List<string> deleteIn = new List<string>();
            List<string> deleteProduce = new List<string>();
            List<string> deleteOut = new List<string>();
            //明天之后的计划全删掉
            deleteIn = uniwork.IQueryable<InStorageEntity>(a => a.F_PlanTime >= lasterDate && a.F_InStorageState == 0 && a.F_IsUserEdit == false).Select(a => a.F_Id).ToList();
            deleteOut = uniwork.IQueryable<OutStorageEntity>(a => a.F_PlanTime >= lasterDate && a.F_OutStorageState == 0 && a.F_IsUserEdit == false).Select(a => a.F_Id).ToList();
            deleteProduce = uniwork.IQueryable<WorkPlanEntity>(a => a.F_Date >= lasterDate && a.F_IsUserEdit == false).Select(a => a.F_Id).ToList();
            //未执行的计划
            var olddeleteIn = uniwork.IQueryable<InStorageEntity>(a => a.F_PlanTime < lasterDate && a.F_DoneNum == 0 && a.F_InStorageState == 0).Select(a => a.F_Id).ToList();
            var olddeleteOut = uniwork.IQueryable<OutStorageEntity>(a => a.F_PlanTime < lasterDate && a.F_DoneNum == 0 && a.F_OutStorageState == 0).Select(a => a.F_Id).ToList();
            deleteIn.AddRange(olddeleteIn);
            deleteOut.AddRange(olddeleteOut);
            var peopleProduce = uniwork.IQueryable<WorkPlanEntity>(a => a.F_Date >= lasterDate && a.F_IsUserEdit == true).ToList();
            var peopleIn = uniwork.IQueryable<InStorageEntity>(a => a.F_PlanTime >= lasterDate && a.F_IsUserEdit == true).ToList();
            var peopleOut = uniwork.IQueryable<OutStorageEntity>(a => a.F_PlanTime >= lasterDate && a.F_IsUserEdit == true).ToList();
            //从lasterDate开始做计划
            int doneday = 0;
            for (int i = 0; i < 100; i++)
            {
                //获取订单库存
                orderMaterials = await GetMaterialByOrder(details.Where(a=>a.F_DoneNum!=a.F_NeedNum).ToList(), warmMaterials, allBoms);
                needMaterials = await GetMaterialByNeed(warmMaterials, allBoms);
                var tempStorages = await GetMaterialByStorage(currentStorages, warmMaterials, allBoms);
                foreach (var item in needMaterials)
                {
                    if (item.F_MaterialType > 0 && !candones.Contains(item.F_Id))
                    {
                        item.F_Num = 0;
                        continue;
                    }
                    var orderMaterial = orderMaterials.Where(a => a.F_Id == item.F_Id).FirstOrDefault();
                    var tempStorage = tempStorages.Where(a => a.F_Id == item.F_Id).FirstOrDefault();
                    if (orderMaterial != null)
                    {
                        item.F_Num += orderMaterial.F_Num;
                    }
                    if (tempStorage != null)
                    {
                        item.F_Num -= tempStorage.F_Num;
                    }
                }
                //整托交付
                //制定交付计划
                //逐个创建交付计划
                var ordernote = ordernotes.Where(a => a.F_IsFinish == false).FirstOrDefault();
                if (ordernote != null && ordernote.F_PlanStartTime <= lasterDate.AddDays(i))
                {
                    foreach (var item in details.Where(a => a.F_OrderId == ordernote.F_Id))
                    {
                        var storage = currentStorages.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
                        if (peopleOut.Where(a => a.F_PlanTime == lasterDate.AddDays(i) && a.F_MaterialId == item.F_Id && a.F_OrderId == item.F_OrderId).Count() > 0)
                        {
                            storage.F_Num -= peopleOut.Where(a => a.F_PlanTime == lasterDate.AddDays(i - 1) && a.F_MaterialId == item.F_Id && a.F_OrderId == item.F_OrderId).Sum(a => a.F_Num) ?? 0;
                            item.F_DoneNum += peopleOut.Where(a => a.F_PlanTime == lasterDate.AddDays(i - 1) && a.F_MaterialId == item.F_Id && a.F_OrderId == item.F_OrderId).Sum(a => a.F_Num) ?? 0;
                            continue;
                        }
                        if (item.F_DoneNum >= item.F_NeedNum)
                        {
                            continue;
                        }
                        if (storage.F_Num <= 0)
                        {
                            continue;
                        }
						var tempMaterial = warmMaterials.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
						int tempnum = (int)item.F_NeedNum / (ordernote.F_DayNum ?? 0);
                        int palletnum = tempnum / (int)tempMaterial.F_ContainerNum;
                        var tempout = new OutStorageEntity();
                        tempout.Create();
                        tempout.F_DoneNum = 0;
                        tempout.F_EnabledMark = false;
                        if (lasterDate.AddDays(i) == DateTime.Now.Date)
                        {
                            tempout.F_EnabledMark = true;
                        }
                        tempout.F_MaterialId = item.F_MaterialId;
                        tempout.F_Num = palletnum * (int)tempMaterial.F_ContainerNum;
                        if (item.F_NeedNum - item.F_DoneNum < tempout.F_Num)
                        {
                            tempout.F_Num = item.F_NeedNum - item.F_DoneNum;
                        }
                        if (storage.F_Num < tempout.F_Num)
                        {
                            palletnum = (int)storage.F_Num / (int)tempMaterial.F_ContainerNum;
                            tempout.F_Num = palletnum * (int)tempMaterial.F_ContainerNum;
                        }
                        storage.F_Num -= tempout.F_Num;
                        item.F_DoneNum += tempout.F_Num;
                        tempout.F_IsUserEdit = false;
                        tempout.F_OrderId = item.F_OrderId;
                        tempout.F_MaterialCode = tempMaterial.F_MaterialCode;
                        tempout.F_MaterialName = tempMaterial.F_MaterialName;
                        tempout.F_MaterialType = tempMaterial.F_MaterialType;
                        tempout.F_MaterialUnit = tempMaterial.F_MaterialUnit;
                        tempout.F_OrderId = item.F_OrderId;
                        tempout.F_OutStorageCode = "ON-" + lasterDate.AddDays(i).ToString("yyyyMMdd") + new Random().Next(0, 1000000);
                        tempout.F_OutStorageState = 0;
                        tempout.F_PlanTime = lasterDate.AddDays(i);
                        if (tempout.F_Num>0)
                        {
                            outStorages.Add(tempout);
                        }

                    }
                    if (details.Where(a => a.F_OrderId == ordernote.F_Id && a.F_NeedNum > a.F_DoneNum).Count() == 0)
                    {
                        ordernote.F_IsFinish = true;
                        ordernote.F_PredictOverTime = lasterDate.AddDays(i);
                    }
                }

                if (needMaterials.Where(a => a.F_MaterialType > 0 && a.F_Num > 0).Count() == 0 && ordernotes.Where(a => a.F_IsFinish == false).Count() == 0)
                {
                    break;
                }
                foreach (var classNum in classNums)
                {
					//制定生产计划 白班
					//根据订单产品获取需要生产的设备
					var products = needMaterials.Where(a => a.F_MaterialType > 0 && a.F_Num > 0).Select(a => a.F_Id).ToList();
					//人为修改的生产计划
					var peopleProduceClass = peopleProduce.Where(a => a.F_Date == lasterDate.AddDays(i) && a.F_ClassNum == classNum.F_ItemCode && a.F_IsUserEdit == true).ToList();
					var peopleProduceIds = peopleProduceClass.Select(a => a.F_EqpId).ToList();
					var eqpbanding = eqpbandings.Where(a => products.Contains(a.F_MaterialId)).ToList();
					//订单需要的并且不满足安全库存的优先生产
					List<WorkPlanEntity> halfPlans = new List<WorkPlanEntity>();
					foreach (var item in eqpbanding)
					{
						//增加人为修改的部分
						var peopleProducetemp = peopleProduceClass.Where(a => a.F_EqpId == item.F_EqpId && a.F_MaterialId == item.F_MaterialId).FirstOrDefault();
						var produceStorage = currentStorages.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
						if (peopleProducetemp != null)
						{
							produceStorage.F_Num += peopleProducetemp.F_PlanNum;
							var tempBoms = allBoms.Where(a => a.F_MaterialId == item.F_MaterialId).ToList();
							var tempBomIds = tempBoms.Select(a => a.F_SonMaterialId).ToList();
							var tempcurrentStorages = currentStorages.Where(a => tempBomIds.Contains(a.F_Id)).ToList();
							foreach (var thisStorage in tempcurrentStorages)
							{
								var thisBom = tempBoms.Where(a => a.F_SonMaterialId == thisStorage.F_Id).FirstOrDefault();
								thisStorage.F_Num -= item.F_Num * thisBom.F_Num;
							}
							halfPlans.Add(peopleProducetemp);
						}
						if (halfPlans.Where(a => a.F_EqpId == item.F_EqpId).Count() > 0 || peopleProduceClass.Where(a => a.F_EqpId == item.F_EqpId).Count() > 0)
						{
							continue;
						}
						//对比独立生产的物料，库存小于安全库存，库存大于安全库存，以最少的为准
						bool firstProduce = true;
						//安全库存需要
						List<string> warms = new List<string>();
						//订单库存需要
						List<string> orders = new List<string>();
						foreach (var banding in eqpbanding.Where(a => a.F_EqpId == item.F_EqpId))
						{
							var warmMaterial = warmMaterials.Where(a => a.F_Id == banding.F_MaterialId).FirstOrDefault();
							var orderMaterial = orderMaterials.Where(a => a.F_Id == banding.F_MaterialId).FirstOrDefault();
							if (orderMaterial == null)
							{
								continue;
							}
							var storage = currentStorages.Where(a => a.F_Id == banding.F_MaterialId).FirstOrDefault();
							var itemBoms = allBoms.Where(a => a.F_MaterialId == item.F_MaterialId).ToList();
							var itemBomIds = itemBoms.Select(a => a.F_SonMaterialId).ToList();
							var itemStorages = currentStorages.Where(a => itemBomIds.Contains(a.F_Id)).ToList();
							bool isproduce = true;
							foreach (var thisStorage in itemStorages)
							{
								var thisBom = itemBoms.Where(a => a.F_SonMaterialId == thisStorage.F_Id).FirstOrDefault();
								if (thisStorage.F_MaterialType == 1 && thisStorage.F_Num < item.F_Num * thisBom.F_Num)
								{
									isproduce = false;
								}
							}
							if (isproduce == true)
							{
								if (storage.F_Num < warmMaterial.F_WarmNum)
								{
									warms.Add(banding.F_MaterialId);
								}
								if (storage.F_Num < orderMaterial.F_Num)
								{
									orders.Add(banding.F_MaterialId);
								}
							}
						}
						if (!orders.Contains(item.F_MaterialId) && orders.Count() > 0)
						{
							firstProduce = false;
						}
						if (orders.Contains(item.F_MaterialId) && orders.Count() > 1 && !warms.Contains(item.F_MaterialId) && warms.Count() > 0)
						{
							firstProduce = false;
						}
						if (orders.Contains(item.F_MaterialId) && orders.Count() > 1 && warms.Contains(item.F_MaterialId) && warms.Count() > 1)
						{
							var storage = currentStorages.Where(a => warms.Contains(a.F_Id)).OrderBy(a => a.F_Num).FirstOrDefault();
							if (storage.F_Id != item.F_MaterialId)
							{
								firstProduce = false;
							}
						}
						if (firstProduce == false)
						{
							continue;
						}
						var thisBoms = allBoms.Where(a => a.F_MaterialId == item.F_MaterialId).ToList();
						var thisBomIds = thisBoms.Select(a => a.F_SonMaterialId).ToList();
						var thisStorages = currentStorages.Where(a => thisBomIds.Contains(a.F_Id)).ToList();
						foreach (var thisStorage in thisStorages)
						{
							var thisBom = thisBoms.Where(a => a.F_SonMaterialId == thisStorage.F_Id).FirstOrDefault();
							thisStorage.F_Num -= item.F_Num * thisBom.F_Num;
						}
						WorkPlanEntity workplan = new WorkPlanEntity();
						workplan.Create();
						workplan.F_ClassNum = classNum.F_ItemCode;
						workplan.F_Date = lasterDate.AddDays(i);
						workplan.F_DoneNum = 0;
						workplan.F_PlanNum = item.F_Num;
						workplan.F_WorkPlanType = 0;
						workplan.F_MaterialId = item.F_MaterialId;
						workplan.F_EqpId = item.F_EqpId;
						workplan.F_EqpName = item.F_EqpName;
						workplan.F_EnabledMark = true;
						workplan.F_IsUserEdit = false;
						workplan.F_DeleteMark = false;
						workPlans.Add(workplan);
						if (item.F_ProduceType == 0)
						{
							halfPlans.Add(workplan);
						}
						//增加当前库存
						produceStorage = currentStorages.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
						produceStorage.F_Num += item.F_Num;
						//扣减需求当前库存
						var needMaterial = needMaterials.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
						needMaterial.F_Num -= item.F_Num;
					}
				}
                if (lasterDate.AddDays(i - 1) >= DateTime.Now.Date)
                {
                    //制定采购计划
                    var needpurchases = currentStorages.Where(a => a.F_MaterialType == 0 && a.F_Num < a.F_WarmNum).ToList();
                    foreach (var item in needpurchases)
                    {
                        if (peopleIn.Where(a => a.F_PlanTime == lasterDate.AddDays(i - 1) && a.F_MaterialId == item.F_Id).Count() > 0)
                        {
                            item.F_Num += peopleIn.Where(a => a.F_PlanTime == lasterDate.AddDays(i - 1) && a.F_MaterialId == item.F_Id).Sum(a => a.F_Num) ?? 0;
                            continue;
                        }
                        InStorageEntity instorage = new InStorageEntity();
                        instorage.F_MaterialType = item.F_MaterialType;
                        instorage.F_MaterialCode = item.F_MaterialCode;
                        instorage.F_MaterialId = item.F_Id;
                        instorage.F_MaterialName = item.F_MaterialName;
                        instorage.F_MaterialUnit = item.F_MaterialUnit;
                        instorage.F_EnabledMark = false;
                        if (lasterDate.AddDays(i - 1) == DateTime.Now.Date)
                        {
                            instorage.F_EnabledMark = true;
                        }
                        instorage.Create();
                        instorage.F_BadNum = 0;
                        instorage.F_DoneNum = 0;
                        int palet = (int)((item.F_WarmNum - item.F_Num) / item.F_ContainerNum);
                        if ((item.F_WarmNum - item.F_Num) % item.F_ContainerNum > 0)
                        {
                            palet = palet + 1;
                        }
                        instorage.F_Num = item.F_ContainerNum * palet;
                        instorage.F_IsUserEdit = false;
                        instorage.F_PlanTime = lasterDate.AddDays(i - 1);
                        instorage.F_InStorageState = 0;
                        instorage.F_InStorageCode = "IN-" + lasterDate.AddDays(i - 1).ToString("yyyyMMdd") + new Random().Next(0, 1000000);
                        if (instorage.F_Num > 0)
                        {
                            inStorages.Add(instorage);
                        }
                        item.F_Num += instorage.F_Num;
                    }
                }
                //增加计划库存
                planStorage.Add(lasterDate.AddDays(i), currentStorages.ToJson().ToObject<List<MaterialEntity>>());
            }
            var deletedetails = uniwork.IQueryable<WorkOrderEntity>(a => deleteProduce.Contains(a.F_WorkPlanId))
                .InnerJoin<WorkOrderDetailEntity>((a, b) => a.F_Id == b.F_WorkOrderId).Select((a, b) => b.F_Id).ToList();
            //是否创建计划
            var dbJobEntity = await uniwork.IQueryable<OpenJobEntity>().Where(a=>a.F_FileName== "WaterCloud.Service.AutoJob.AutoUpdatePlanJob").FirstOrDefaultAsync();
            bool isStart = true;
            if (dbJobEntity == null || dbJobEntity.F_EnabledMark == false)
			{
                isStart = false;
            }
            uniwork.BeginTrans();
            //删除计划
            await uniwork.Delete<InStorageEntity>(a => deleteIn.Contains(a.F_Id) && a.F_InStorageState == 0);
            await uniwork.Delete<WorkPlanEntity>(a => deleteProduce.Contains(a.F_Id) && a.F_DoneNum == 0);
            await uniwork.Delete<OutStorageEntity>(a => deleteOut.Contains(a.F_Id) && a.F_OutStorageState == 0);
            await uniwork.Delete<WorkOrderEntity>(a => deleteProduce.Contains(a.F_Id) && a.F_DoneNum == 0);
            await uniwork.Delete<WorkOrderDetailEntity>(a => deletedetails.Contains(a.F_Id) && a.F_DoneNum == 0);
            //创建计划
            if (inStorages.Count > 0 && isStart)
            {
                await uniwork.Insert(inStorages);
            }
            if (outStorages.Count > 0 && isStart)
            {
                await uniwork.Insert(outStorages);
            }
            if (workPlans.Count > 0 && isStart)
            {
                await uniwork.Insert(workPlans);
            }
            //今日的订单结案
            await uniwork.Update<OrderEntity>(a => a.F_PlanEndTime==DateTime.Now.Date && a.F_EnabledMark == true && a.F_IsFinish == false, a => new OrderEntity
            {
                F_IsFinish = true,
                F_ActualOverTime = DateTime.Now.Date
            });
            //启用今天的计划
            await uniwork.Update<InStorageEntity>(a => a.F_PlanTime == DateTime.Now.Date, a => new InStorageEntity
            {
                F_EnabledMark = true,
            });
            await uniwork.Update<OutStorageEntity>(a =>a.F_PlanTime == DateTime.Now.Date, a => new OutStorageEntity
            {
                F_EnabledMark = true,
            });
            //更新预期完成时间
            foreach (var item in ordernotes)
            {
                await repository.Update(a => a.F_Id==item.F_Id, a => new OrderEntity
                {
                    F_EnabledMark = true,
                    F_PredictOverTime=item.F_PredictOverTime
                });
            }
            uniwork.Commit();
        }
        //根据订单获取所需总物料
        private async Task<List<MaterialEntity>> GetMaterialByOrder(List<OrderDetailEntity> details, List<MaterialEntity> materials, List<BomFormEntity> allBoms)
        {
            List<MaterialEntity> list = new List<MaterialEntity>();
            List<OrderDetailEntity> temps = new List<OrderDetailEntity>();
            foreach (var item in details)
            {
                var temp = temps.Where(a => a.F_MaterialId == item.F_MaterialId).FirstOrDefault();
                if (temp == null)
                {
                    temps.Add(item.ToJson().ToObject<OrderDetailEntity>());
                }
                else
                {
                    temp.F_NeedNum += temp.F_NeedNum;
                    temp.F_DoneNum += temp.F_DoneNum;
                }
            }
            foreach (var item in temps)
            {
                //订单物料添加
                var detail = materials.Where(a=>a.F_Id==item.F_MaterialId).FirstOrDefault().ToJson().ToObject<MaterialEntity>();
                detail.F_Num = item.F_NeedNum-item.F_DoneNum;
                list.Add(detail);
               //订单所需物料
               var boms = allBoms.Where(a => a.F_MaterialId == item.F_MaterialId).ToList();
                foreach (var bom in boms)
                {
                    var temp = list.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault();
                    if (temp == null)
                    {
                        temp = materials.Where(a=>a.F_Id==bom.F_SonMaterialId).FirstOrDefault().ToJson().ToObject<MaterialEntity>();
                        temp.F_Num = 0;
                        list.Add(temp);
                    }
                    temp.F_Num += bom.F_Num * detail.F_Num;
                    await GetMaterialByOrder(temp.F_Id, (bom.F_Num * detail.F_Num) ??0, list, materials, allBoms);
                }
            }
            return list;
        }
        private async Task<List<MaterialEntity>> GetMaterialByStorage(List<MaterialEntity> details, List<MaterialEntity> materials, List<BomFormEntity> allBoms)
        {
            List<MaterialEntity> list = new List<MaterialEntity>();
            foreach (var item in details)
            {
                //物料添加
                var material = materials.Where(a => a.F_Id == item.F_Id).FirstOrDefault().ToJson().ToObject<MaterialEntity>();
                var entitytemp = list.Where(a => a.F_Id == item.F_Id).FirstOrDefault();
                if (entitytemp != null)
                {
                    entitytemp.F_Num += item.F_Num;
                }
                else
                {
                    material.F_Num = item.F_Num;
                    list.Add(material);
                }
                //订单所需物料
                var boms = allBoms.Where(a => a.F_MaterialId == item.F_Id).ToList();
                foreach (var bom in boms)
                {
                    var temp = list.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault();
                    if (temp == null)
                    {
                        temp = materials.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault().ToJson().ToObject<MaterialEntity>();
                        temp.F_Num = 0;
                        list.Add(temp);
                    }
                    temp.F_Num += bom.F_Num * item.F_Num;
                    await GetMaterialByOrder(temp.F_Id, (bom.F_Num * item.F_Num) ?? 0, list, materials, allBoms);
                }
            }
            return list;
        }
        private async Task<List<MaterialEntity>> GetMaterialByNeed(List<MaterialEntity> materials, List<BomFormEntity> allBoms)
        {
            List<MaterialEntity> list = new List<MaterialEntity>();
            foreach (var item in materials)
            {
                //物料添加
                var detail = item.ToJson().ToObject<MaterialEntity>();
                var entitytemp = list.Where(a => a.F_Id == item.F_Id).FirstOrDefault();
                if (entitytemp != null)
                {
                    entitytemp.F_Num += detail.F_WarmNum;
                }
                else
                {
                    detail.F_Num = detail.F_WarmNum;
                    list.Add(detail);
                }
                //订单所需物料
                var boms = allBoms.Where(a => a.F_MaterialId == item.F_Id).ToList();
                foreach (var bom in boms)
                {
                    var temp = list.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault();
                    if (temp == null)
                    {
                        temp = materials.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault().ToJson().ToObject<MaterialEntity>();
                        temp.F_Num = 0;
                        list.Add(temp);
                    }
                    temp.F_Num += bom.F_Num * detail.F_WarmNum;
                    await GetMaterialByOrder(temp.F_Id, (bom.F_Num * detail.F_WarmNum) ?? 0, list, materials, allBoms);
                }
            }
            return list;
        }
        //递归获取所需物料
        private async Task GetMaterialByOrder(string materialId,float num,List<MaterialEntity> list, List<MaterialEntity> materials, List<BomFormEntity> allBoms)
        {
            var boms = allBoms.Where(a => a.F_MaterialId == materialId).ToList();
            foreach (var bom in boms)
            {
                var temp = list.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault();
                if (temp == null)
                {
                    temp = materials.Where(a=>a.F_Id==bom.F_SonMaterialId).FirstOrDefault().ToJson().ToObject<MaterialEntity>();
                    temp.F_Num = 0;
                    list.Add(temp.ToJson().ToObject<MaterialEntity>());
                }
                temp.F_Num += bom.F_Num * num;
                await GetMaterialByOrder(temp.F_Id, (bom.F_Num * num) ?? 0, list, materials, allBoms);
            }
        }
        #endregion

    }
}
