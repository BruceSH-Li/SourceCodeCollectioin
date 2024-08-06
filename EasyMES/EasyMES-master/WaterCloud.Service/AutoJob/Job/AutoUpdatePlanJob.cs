using Chloe;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Domain.PlanManage;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Domain.QualityManage;
using WaterCloud.Service.SystemManage;

namespace WaterCloud.Service.AutoJob
{
    public class AutoUpdatePlanJob : IJobTask
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IDbContext _context;
		private ItemsDataService itemsApp;
		public AutoUpdatePlanJob(IDbContext context)
        {
            _hostingEnvironment = GlobalContext.HostingEnvironment;
            _context = context;
			itemsApp = new ItemsDataService(context);
		}
        public async Task<AlwaysResult> Start()
        {
            AlwaysResult obj = new AlwaysResult();
            try
            {
                var ordernotes = _context.Query<OrderEntity>(a => a.F_EnabledMark == true && a.F_IsFinish == false).OrderBy(a => a.F_PlanStartTime).ToList();
                if (ordernotes.Count() == 0)
                {
                    throw new Exception("没有订单需要执行APS");
                }
                var orderIds = ordernotes.Select(a => a.F_Id).ToList();
                //获取当前天
                DateTime currentdate = DateTime.Now.Date;
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
                    _context.Update<OrderEntity>(a => overnote.F_Id == a.F_Id && a.F_EnabledMark == true && a.F_IsFinish == false, a => new OrderEntity
                    {
                        F_IsFinish = true,
                        F_ActualOverTime = DateTime.Now.Date
                    });
                    _context.Update<OutStorageEntity>(a => overnote.F_Id == a.F_OrderId && a.F_MaterialType == 2 && a.F_OutStorageState == 0 && a.F_PlanTime > DateTime.Now.Date, a => new OutStorageEntity
                    {
                        F_OrderId = null
                    });
                    var otherorder = ordernotes.OrderBy(a => a.F_PlanStartTime).FirstOrDefault();
                    if (otherorder!=null)
                    {
                        _context.Update<OutStorageEntity>(a => a.F_OutStorageState == 0 && a.F_MaterialType == 2 && a.F_OrderId == null &&a.F_PlanTime > DateTime.Now.Date, a => new OutStorageEntity
                        {
                            F_OrderId=otherorder.F_Id
                        });
                    }
                }
                //采购计划、交付计划、生产计划
                List<InStorageEntity> inStorages = new List<InStorageEntity>();
                List<OutStorageEntity> outStorages = new List<OutStorageEntity>();
                List<WorkPlanEntity> workPlans = new List<WorkPlanEntity>();
                //订单数量
                var details = _context.Query<OrderDetailEntity>(a => orderIds.Contains(a.F_OrderId)).ToList();
                foreach (var item in details)
                {
                    item.F_DoneNum = 0;
                }
                var doneouts = _context.Query<OutStorageEntity>(a => (orderIds.Contains(a.F_OrderId) && a.F_OutStorageState > 0) || a.F_PlanTime == currentdate).ToList();
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
                var currentStorages = await GetCurrentClassNumStorage();
                //获取采购计划
                var inPlans = _context.Query<InStorageEntity>(a => a.F_PlanTime >= currentdate).ToList();
                //获取交付计划
                var outPlans = _context.Query<OutStorageEntity>(a => a.F_PlanTime >= currentdate).ToList();
                //获取生产计划
                var producePlans = _context.Query<WorkPlanEntity>(a => a.F_Date >= currentdate).ToList();
                //获取安全库存
                var warmMaterials = _context.Query<MaterialEntity>(a => a.F_EnabledMark == true && a.F_DeleteMark == false).OrderBy("F_MaterialType desc").ToList();
                //Bom表
                var allBoms = _context.Query<BomFormEntity>(a => a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_BomType == 1).ToList();
                //设备物料绑定
                var eqpbandings = _context.Query<EqpMaterialBandingEntity>().InnerJoin<EquipmentEntity>((a, b) => a.F_EqpId == b.F_Id)
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
                var tempinplan = inPlans.Where(a => a.F_PlanTime >= currentdate && a.F_PlanTime < currentdate.AddDays(1) && a.F_InStorageState < 2).ToList();
                var tempoutplan = outPlans.Where(a => a.F_PlanTime >= currentdate && a.F_PlanTime < currentdate.AddDays(1) && a.F_OutStorageState < 2).ToList();
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
                deleteIn = _context.Query<InStorageEntity>(a => a.F_PlanTime >= lasterDate && a.F_InStorageState == 0 && a.F_IsUserEdit == false).Select(a => a.F_Id).ToList();
                deleteOut = _context.Query<OutStorageEntity>(a => a.F_PlanTime >= lasterDate && a.F_OutStorageState == 0 && a.F_IsUserEdit == false).Select(a => a.F_Id).ToList();
                deleteProduce = _context.Query<WorkPlanEntity>(a => a.F_Date >= lasterDate && a.F_IsUserEdit == false).Select(a => a.F_Id).ToList();
                //未执行的计划
                var olddeleteIn = _context.Query<InStorageEntity>(a => a.F_PlanTime < lasterDate && a.F_DoneNum == 0 && a.F_InStorageState == 0).Select(a => a.F_Id).ToList();
                var olddeleteOut = _context.Query<OutStorageEntity>(a => a.F_PlanTime < lasterDate && a.F_DoneNum == 0 && a.F_OutStorageState == 0).Select(a => a.F_Id).ToList();
                deleteIn.AddRange(olddeleteIn);
                deleteOut.AddRange(olddeleteOut);
                var peopleProduce = _context.Query<WorkPlanEntity>(a => a.F_Date >= lasterDate && a.F_IsUserEdit == true).ToList();
                var peopleIn = _context.Query<InStorageEntity>(a => a.F_PlanTime >= lasterDate && a.F_IsUserEdit == true).ToList();
                var peopleOut = _context.Query<OutStorageEntity>(a => a.F_PlanTime >= lasterDate && a.F_IsUserEdit == true).ToList();
                //从lasterDate开始做计划
                for (int i = 0; i < 100; i++)
                {
                    //获取订单库存
                    orderMaterials = await GetMaterialByOrder(details.Where(a => a.F_DoneNum != a.F_NeedNum).ToList(), warmMaterials, allBoms);
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
                            int tempnum = (int)item.F_NeedNum / (ordernote.F_DayNum ?? 0);
                            int palletnum = tempnum / 22;
                            var tempout = new OutStorageEntity();
                            var tempMaterial = warmMaterials.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
                            tempout.Create();
                            tempout.F_DoneNum = 0;
                            tempout.F_EnabledMark = false;
                            if (lasterDate.AddDays(i) == DateTime.Now.Date)
                            {
                                tempout.F_EnabledMark = true;
                            }
                            tempout.F_MaterialId = item.F_MaterialId;
                            tempout.F_Num = palletnum * 22;
                            if (item.F_NeedNum - item.F_DoneNum < tempout.F_Num)
                            {
                                tempout.F_Num = item.F_NeedNum - item.F_DoneNum;
                            }
                            if (storage.F_Num < tempout.F_Num)
                            {
                                palletnum = (int)storage.F_Num / 22;
                                tempout.F_Num = palletnum * 22;
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
                            if (tempout.F_Num > 0)
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
                            if (peopleIn.Where(a => a.F_PlanTime == lasterDate.AddDays(i-1) && a.F_MaterialId == item.F_Id).Count() > 0)
                            {
                                item.F_Num += peopleIn.Where(a => a.F_PlanTime == lasterDate.AddDays(i - 1) && a.F_MaterialId == item.F_Id).Sum(a=>a.F_Num)??0;
                                continue;
                            }
                            InStorageEntity instorage = new InStorageEntity();
                            instorage.F_Id = Utils.GuId();
                            instorage.F_CreatorTime = DateTime.Now;
                            instorage.F_CreatorUserId = GlobalContext.SystemConfig.SysemUserId;
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
                var deletedetails = _context.Query<WorkOrderEntity>(a => deleteProduce.Contains(a.F_WorkPlanId))
                        .InnerJoin<WorkOrderDetailEntity>((a, b) => a.F_Id == b.F_WorkOrderId).Select((a, b) => b.F_Id).ToList();
                //删除计划
                _context.Delete<InStorageEntity>(a => deleteIn.Contains(a.F_Id) && a.F_InStorageState == 0);
                _context.Delete<WorkPlanEntity>(a => deleteProduce.Contains(a.F_Id) && a.F_DoneNum == 0);
                _context.Delete<OutStorageEntity>(a => deleteOut.Contains(a.F_Id) && a.F_OutStorageState == 0);
                _context.Delete<WorkOrderEntity>(a => deleteProduce.Contains(a.F_Id) && a.F_DoneNum == 0);
                _context.Delete<WorkOrderDetailEntity>(a => deletedetails.Contains(a.F_Id) && a.F_DoneNum == 0);
                //创建计划
                if (inStorages.Count > 0)
                {
                    _context.InsertRange(inStorages);
                }
                if (outStorages.Count > 0)
                {
                    _context.InsertRange(outStorages);
                }
                if (workPlans.Count > 0)
                {
                    _context.InsertRange(workPlans);
                }
                //启用今天的计划
                _context.Update<InStorageEntity>(a => a.F_PlanTime == DateTime.Now.Date, a => new InStorageEntity
                {
                    F_EnabledMark = true,
                });
                _context.Update<OutStorageEntity>(a => a.F_PlanTime == DateTime.Now.Date, a => new OutStorageEntity
                {
                    F_EnabledMark = true,
                });
                //更新预期完成时间
                foreach (var item in ordernotes)
                {
                    _context.Update<OrderEntity>(a => a.F_Id==item.F_Id, a => new OrderEntity
                    {
                        F_EnabledMark = true,
                        F_PredictOverTime = item.F_PredictOverTime
                    });
                }
                obj.state = ResultType.success.ToString();
                obj.message = "APS更新成功！";
            }
            catch (Exception ex)
            {
                obj.state = ResultType.error.ToString();
                obj.message = "APS更新失败！" + ex.Message;
            }
            return obj;
        }
        //根据订单获取所需总物料
        private async Task<List<MaterialEntity>> GetMaterialByOrder(List<OrderDetailEntity> details, List<MaterialEntity> materials, List<BomFormEntity> allBoms)
        {
            List<MaterialEntity> list = new List<MaterialEntity>();
            List<OrderDetailEntity> temps = new List<OrderDetailEntity>();
            foreach (var item in details)
            {
                var temp = temps.Where(a => a.F_MaterialId == item.F_MaterialId).FirstOrDefault();
                if (temp==null)
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
                var detail = materials.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault().ToJson().ToObject<MaterialEntity>();
                detail.F_Num = item.F_NeedNum - item.F_DoneNum;
                list.Add(detail);
                //订单所需物料
                var boms = allBoms.Where(a => a.F_MaterialId == item.F_MaterialId).ToList();
                foreach (var bom in boms)
                {
                    var temp = list.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault();
                    if (temp == null)
                    {
                        temp = materials.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault().ToJson().ToObject<MaterialEntity>();
                        temp.F_Num = 0;
                        list.Add(temp);
                    }
                    temp.F_Num += bom.F_Num * detail.F_Num;
                    await GetMaterialByOrder(temp.F_Id, (bom.F_Num * detail.F_Num) ?? 0, list, materials, allBoms);
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
        private async Task GetMaterialByOrder(string materialId, float num, List<MaterialEntity> list, List<MaterialEntity> materials, List<BomFormEntity> allBoms)
        {
            var boms = allBoms.Where(a => a.F_MaterialId == materialId).ToList();
            foreach (var bom in boms)
            {
                var temp = list.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault();
                if (temp == null)
                {
                    temp = materials.Where(a => a.F_Id == bom.F_SonMaterialId).FirstOrDefault().ToJson().ToObject<MaterialEntity>();
                    temp.F_Num = 0;
                    list.Add(temp.ToJson().ToObject<MaterialEntity>());
                }
                temp.F_Num += bom.F_Num * num;
                await GetMaterialByOrder(temp.F_Id, (bom.F_Num * num) ?? 0, list, materials, allBoms);
            }
        }
        private async Task<List<MaterialEntity>> GetCurrentClassNumStorage()
        {
            DateTime checkdate = DateTime.Now.Date;
            DateTime starttime = DateTime.Now.Date;
            DateTime endtime = DateTime.Now.Date;
            string classNum = "";
			var classNums = await itemsApp.GetItemList("Mes_ClassNumber");
			var classStartTime = TimeSpan.Parse(classNums.FirstOrDefault().F_Description.Split("-")[0]);
			var tempStartTime = classStartTime.TotalMinutes;
			var tempEndTime = TimeSpan.Parse(classNums[0].F_Description.Split("-")[1]).TotalMinutes;
			var currentTime = DateTime.Now.TimeOfDay;
			if (classNums.Count() == 1)
            {
				classNum = classNums[0].F_ItemCode;
				starttime = DateTime.Now.Date.AddMinutes(tempStartTime);
				endtime = DateTime.Now.Date.AddMinutes(tempEndTime);
			}
            else
            {
				if (TimeSpan.Compare(currentTime, classStartTime) < 0)
				{
					checkdate = checkdate.AddDays(-1);
				}
				tempEndTime = tempStartTime;
                for (int j = 0; j < classNums.Count(); j++)
                {
                    var startTime = TimeSpan.Parse(classNums[j].F_Description.Split("-")[0]).TotalMinutes;
                    var endTime = TimeSpan.Parse(classNums[j].F_Description.Split("-")[1]).TotalMinutes;
                    if (endTime > startTime)
                    {
                        tempEndTime += endTime - startTime;
                    }
                    else
                    {
                        tempEndTime += endTime + 24 * 60 - startTime;
                    }
                    if (DateTime.Now > checkdate.AddMinutes(tempStartTime) && DateTime.Now <= checkdate.AddMinutes(tempEndTime))
                    {
                        classNum = classNums[j].F_ItemCode;
                        starttime = checkdate.AddMinutes(tempStartTime);
                        endtime = checkdate.AddMinutes(tempEndTime);
                        break;
                    }
                    tempStartTime += tempEndTime;
                }
            }
			var materials = _context.Query<MaterialEntity>(a => a.F_EnabledMark == true && a.F_DeleteMark == false).OrderByDesc(a => a.F_MaterialType).ToList();
            foreach (var item in materials)
            {
                var CurrentNum = _context.Query<StorageEntity>(a => a.F_MaterialId == item.F_Id && a.F_IsCheckout != false).Sum(a => a.F_Num) ?? 0;
                var instoragenote = _context.Query<InStorageEntity>(a => a.F_MaterialId == item.F_Id && a.F_PlanTime == checkdate && a.F_InStorageState <= 2).ToList();
                var innoteids = instoragenote.Select(a => a.F_Id).ToList();
                var tempinstorage = _context.Query<InStorageInfoEntity>(a => a.F_IsTemp == true && a.F_MaterialId == item.F_Id && !innoteids.Contains(a.F_InStorageId)).Sum(a => a.F_Num) ?? 0;
                var outstoragenote = _context.Query<OutStorageEntity>(a => a.F_MaterialId == item.F_Id && a.F_PlanTime == checkdate && a.F_OutStorageState <= 2).ToList();
                item.F_Num = CurrentNum + tempinstorage;
                item.F_StorageNum = CurrentNum + tempinstorage;
                item.F_ReturnNum = 0;
                item.F_DoneNum = 0;
            }
            return materials;
        }
    }
}
