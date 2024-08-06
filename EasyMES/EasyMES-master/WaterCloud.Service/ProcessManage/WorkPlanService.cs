using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Service.SystemManage;

namespace WaterCloud.Service.ProcessManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-15 10:07
    /// 描 述：生产计划服务类
    /// </summary>
    public class WorkPlanService : DataFilterService<WorkPlanEntity>, IDenpendency
    {
		private ItemsDataService itemsApp;
		public WorkPlanService(IDbContext context) : base(context)
        {
			itemsApp = new ItemsDataService(context);
		}
        #region 获取数据
        public async Task<List<WorkPlanEntity>> GetList(string keyword = "")
        {
            var data = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(t => t.F_Description.Contains(keyword));
            }
            return data.Where(t => t.F_DeleteMark == false).OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<WorkPlanEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_Description.Contains(keyword));
            }
             //权限过滤
             query = GetDataPrivilege("u", "", query);
            return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<object> GetLookList(string startDay, int CountDay = 7)
        {
            var code = 0;
            DateTime startTime;
            if (string.IsNullOrEmpty(startDay))
            {
                startTime = DateTime.Now.Date;
            }
			else
			{
                startTime = DateTime.Parse(startDay);
            }
            DateTime endTime= startTime.AddDays(CountDay);
            var datas = IQueryable().Where(a => a.F_Date>=startTime && a.F_Date < endTime).ToList();
            var datat = uniwork.IQueryable<EquipmentEntity>(t => t.F_ParentId=="0" && t.F_EnabledMark == true && t.F_DeleteMark == false&&t.F_EqpType==0).OrderBy(t=>t.F_EqpName).ToList();
            var data = new List<Object>();
			var classNums = await itemsApp.GetItemList("Mes_ClassNumber");
			var classStartTime = TimeSpan.Parse(classNums.FirstOrDefault().F_Description.Split("-")[0]);
			var currentTime = DateTime.Now.TimeOfDay;
			foreach (var item in datat)
			{
                Dictionary<string,string> temp = new Dictionary<string, string>();
                temp["F_EqpId"] = item.F_Id;
                temp["F_EqpName"] = item.F_EqpName;
                for (int i = 0; i < CountDay; i++)
                {
                    foreach (var classNum in classNums)
                    {
						temp[startTime.AddDays(i).ToString("yyyy/MM/dd") + classNum.F_ItemCode] = "";
						if (datas.Where(a => a.F_EqpId == item.F_Id && startTime.AddDays(i) == a.F_Date && a.F_ClassNum == classNum.F_ItemCode).Count() > 0)
						{
							var entitys = datas.Where(a => a.F_EqpId == item.F_Id && startTime.AddDays(i) == a.F_Date && a.F_ClassNum == classNum.F_ItemCode).ToList();
							foreach (var entity in entitys)
							{
								if (entity.F_WorkPlanType == 0)
								{
									temp[startTime.AddDays(i).ToString("yyyy/MM/dd") + classNum.F_ItemCode] += entity.F_MaterialName + ",计划数量" + entity.F_PlanNum + ",实际数量" + entity.F_DoneNum + ";";
								}
								else
								{
									temp[startTime.AddDays(i).ToString("yyyy/MM/dd") + classNum.F_ItemCode] = "";
								}
							}
						}
						else
						{
							temp[startTime.AddDays(i).ToString("yyyy/MM/dd") + classNum.F_ItemCode] = "";
						}
					}
                }

                data.Add(temp);
			}

            var cols = new List<string>();
            for (int i = 0; i < CountDay; i++)
            {
                foreach (var classNum in classNums)
                {
                    cols.Add(startTime.AddDays(i).ToString("yyyy/MM/dd") + classNum.F_ItemCode);
                }
            }
            return new
            {
                data,
                cols
            };
        }

		public async Task<List<MaterialEntity>> GetAddListJson(string eqpId)
        {
            var query = uniwork.IQueryable<MaterialEntity>().Where(u => u.F_DeleteMark == false && u.F_MaterialType != 0);
            if (!string.IsNullOrEmpty(eqpId))
            {
                query = uniwork.IQueryable<EqpMaterialBandingEntity>(a => a.F_EqpId == eqpId)
                    .InnerJoin<MaterialEntity>((a, b) => a.F_MaterialId == b.F_Id && b.F_DeleteMark == false && b.F_MaterialType != 0)
                    .Select((a, b) => b);
            }
            var bandings = uniwork.IQueryable<EqpMaterialBandingEntity>(a => a.F_EqpId == eqpId).ToList();
            var list = query.ToList();
            foreach (var item in bandings)
            {
                var material = list.Where(a => a.F_Id == item.F_MaterialId).FirstOrDefault();
                material.F_Num = item.F_Num;
            }
            return list;
        }

        public async Task<List<ProcessFlowEntity>> GetOpListJson(string eqpId)
        {
            var query = uniwork.IQueryable<ProcessEqpBandingEntity>()
                .InnerJoin<ProcessFlowEntity>((a, b) => a.F_ProcessId == b.F_Id && b.F_EnabledMark == true && b.F_DeleteMark == false);
                
            if (!string.IsNullOrEmpty(eqpId))
            {
                query = query.Where((a, b) => a.F_EqpId == eqpId);
            }

            var result = query.Select((a, b) => b).ToList();
            return result;
        }


        public async Task<WorkPlanEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<WorkPlanEntity> GetLookForm(string eqpId,string currentdate,string classnum)
        {
            DateTime now = DateTime.Parse(currentdate).Date;
            var data = IQueryable().Where(a => a.F_Date == now && classnum == a.F_ClassNum && a.F_EqpId == eqpId).ToList();
            var entity = data.FirstOrDefault();
            var eqp = uniwork.IQueryable<EqpMaterialBandingEntity>(a=>a.F_EqpId == eqpId && a.F_ProduceType==0).OrderByDesc(a=>a.F_Priority).ToList();
            var processEqp = uniwork.IQueryable<ProcessEqpBandingEntity>(a => a.F_EqpId == eqpId).ToList();
            if (entity != null)
            {
                entity.listData = new List<WorkPlanTemp>();
                foreach (var item in data.Where(a => a.F_WorkPlanType == 0))
                {
                    WorkPlanTemp temp = new WorkPlanTemp();
                    temp.F_Id = Utils.GuId();
                    temp.F_MaterialId = item.F_MaterialId;
                    temp.F_ProcessId = item.F_ProcessId;
                    temp.F_PlanNum = item.F_PlanNum;
                    entity.listData.Add(temp);
                }

            }
            else
            {
                entity = new WorkPlanEntity();
                entity.F_WorkPlanType = 1;
            }
            entity.F_Date = now;
            entity.F_ClassNum = classnum;
            entity.F_EqpId = eqpId;
            entity.F_MaterialId = eqp.Count()==1?eqp.FirstOrDefault().F_MaterialId :null;
            entity.F_ProcessId = processEqp.Count == 1 ? processEqp.FirstOrDefault().F_ProcessId : null;
            return GetFieldsFilterData(entity);
        }

		private IQuery<WorkPlanEntity> IQueryable()
		{
            var query = repository.IQueryable()
                .LeftJoin<MaterialEntity>((a, b) => a.F_MaterialId == b.F_Id)
                .Select((a, b) => new WorkPlanEntity
                {
                    F_EnabledMark = a.F_EnabledMark,
                    F_EqpId = a.F_EqpId,
                    F_EqpName = a.F_EqpName,
                    F_ClassNum = a.F_ClassNum,
                    F_CreatorTime = a.F_CreatorTime,
                    F_CreatorUserId = a.F_CreatorUserId,
                    F_Date = a.F_Date,
                    F_DeleteMark = a.F_DeleteMark,
                    F_DeleteTime = a.F_DeleteTime,
                    F_DeleteUserId = a.F_DeleteUserId,
                    F_Description = a.F_Description,
                    F_DoneNum = a.F_DoneNum,
                    F_Id = a.F_Id,
                    F_LastModifyTime = a.F_LastModifyTime,
                    F_LastModifyUserId = a.F_LastModifyUserId,
                    F_MaterialCode = b.F_MaterialCode,
                    F_MaterialId = a.F_MaterialId,
                    F_MaterialName = b.F_MaterialName,
                    F_MaterialType = b.F_MaterialType,
                    F_MaterialUnit = b.F_MaterialUnit,
                    F_PlanNum = a.F_PlanNum,
                    F_WorkPlanType = a.F_WorkPlanType,
                    F_ProcessId = a.F_ProcessId
                }) ;
            return query;
        }

		#region 提交数据
		public async Task SubmitForm(WorkPlanEntity entity)
        {
            List<WorkPlanEntity> list = new List<WorkPlanEntity>();
            if (entity.F_WorkPlanType == 0)
            {
                foreach (var item in entity.listData.Where(a=>a.F_PlanNum>0))
                {
                    var materialProcess = uniwork.IQueryable<BomFormEntity>(t => t.F_MaterialId == item.F_MaterialId && t.F_ProcessId == item.F_ProcessId).Count();
                    if (!(materialProcess > 0)) throw new Exception("所选择的物料和工序未绑定，请先绑定！");

                    var temp = list.Find(a => a.F_MaterialId == item.F_MaterialId&&a.F_ProcessId==item.F_ProcessId);
                    if (temp == null)
                    {
                        temp = new WorkPlanEntity();
                        temp.F_DeleteMark = false;
                        temp.F_EnabledMark = true;
                        temp.F_IsUserEdit = true;
                        temp.Create();
                        temp.F_DoneNum = 0;
                        temp.F_MaterialId = item.F_MaterialId;
                        temp.F_ProcessId = item.F_ProcessId;
                        temp.F_PlanNum = item.F_PlanNum;
                        temp.F_Date = entity.F_Date;
                        temp.F_ClassNum = entity.F_ClassNum;
                        temp.F_EqpId = entity.F_EqpId;
                        temp.F_EqpName = entity.F_EqpName;
                        temp.F_WorkPlanType = 0;
                        list.Add(temp);
                    }
                    else
                    {
                        temp.F_PlanNum += item.F_PlanNum;
                    }
                }
            }
            else
            {
                entity.F_DeleteMark = false;
                entity.F_EnabledMark = true;
                entity.F_IsUserEdit = true;
                entity.F_PlanNum =0;
                entity.Create();
                entity.F_DoneNum = 0;
                list.Add(entity);
            }
            var olds = repository.IQueryable().Where(a => a.F_Date == entity.F_Date && entity.F_ClassNum == a.F_ClassNum && a.F_EqpId == entity.F_EqpId).ToList();
            uniwork.BeginTrans();
            if (olds.Count()>0)
			{
                var oldsids = olds.Select(a => a.F_Id);
                if (uniwork.IQueryable<WorkOrderEntity>(a=> oldsids.Contains(a.F_WorkPlanId)).Count()>0)
                {
                    throw new Exception("计划已经启动，请不要修改计划");
                }

                await uniwork.Delete<WorkPlanEntity>(a => a.F_Date == entity.F_Date && entity.F_ClassNum == a.F_ClassNum && a.F_EqpId == entity.F_EqpId);
			}
            await repository.Insert(list);
            uniwork.Commit();

        }

		public async Task DeleteForm(string eqpId, string currentdate, string classnum)
        {
            DateTime now = DateTime.Parse(currentdate).Date;
            var olds = repository.IQueryable().Where(a => a.F_Date == now && classnum == a.F_ClassNum && a.F_EqpId == eqpId).ToList();
            var oldsids = olds.Select(a => a.F_Id);
            if (uniwork.IQueryable<WorkOrderEntity>(a => oldsids.Contains(a.F_WorkPlanId)).Count() > 0)
            {
                throw new Exception("计划已经启动，请不要删除计划");
            }
            await repository.Delete(a => a.F_Date == now && classnum == a.F_ClassNum && a.F_EqpId == eqpId);
        }

        public async Task DeleteUnCreateOrderForm(string eqpId, string currentdate, string classnum)
        {
            DateTime now = DateTime.Parse(currentdate).Date;

            var unCreatePlans = repository.IQueryable().Where(a => a.F_Date == now && classnum == a.F_ClassNum && a.F_EqpId == eqpId)
                .LeftJoin<WorkOrderEntity>((a, b) => a.F_Id == b.F_WorkPlanId).Where((a, b) => b.F_Id == null && b.F_WorkPlanId == null)
                .Select((a, b) => a.F_Id).ToList();

            if (unCreatePlans != null && unCreatePlans.Count > 0)
            {
                await repository.Delete(a => unCreatePlans.Contains(a.F_Id));
            }
            else
            {
                throw new Exception("没有未创建工单的计划，无需删除！");
            }

        }


        public async Task DeleteAllForm(string currentdate, string classnum)
        {
            DateTime now = DateTime.Parse(currentdate).Date;
            var olds = repository.IQueryable().Where(a => a.F_Date == now && classnum == a.F_ClassNum).ToList();
            var oldsids = olds.Select(a => a.F_Id);
            if (uniwork.IQueryable<WorkOrderEntity>(a => oldsids.Contains(a.F_WorkPlanId)).Count() > 0)
            {
                throw new Exception("计划已经启动，请不要删除计划");
            }
            await repository.Delete(a => a.F_Date == now && classnum == a.F_ClassNum);
        }
        public async Task CreateWorkOrder(string eqpId, string currentdate, string classnum)
        {
            DateTime now = DateTime.Parse(currentdate).Date;
            var datas = IQueryable().Where(a => a.F_Date == now && classnum == a.F_ClassNum &&a.F_EqpId==eqpId).ToList();
            var entitys = new List<WorkOrderEntity>();
            var details = new List<WorkOrderDetailEntity>();
            var bandings = new List<WorkOrderDetailEqpBandingEntity>();
            var workdetailprocesses = new List<WorkOrderDetailProcessBandingEntity>();

            int i = 0;
            var ids = datas.Select(a => a.F_Id).ToList();
            //把已创建工单的计划排除
            var otherids = uniwork.IQueryable<WorkOrderEntity>(a => ids.Contains(a.F_WorkPlanId)).Select(a => a.F_WorkPlanId).ToList();
            if (otherids.Count() > 0)
            {
                throw new Exception("计划已经创建工单，请不要再次创建");
            }
            foreach (var item in datas.Where(a => a.F_WorkPlanType == 0))
            {
                var entity = new WorkOrderEntity();
                entity.Create();
                entity.F_DeleteMark = false;
                entity.F_EnabledMark = true;
                entity.F_WorkOrderState = 0;//添写初始化参数;
                entity.F_PlanNum = item.F_PlanNum;
                entity.F_DoneNum = 0;
                entity.F_BadNum = 0;
                entity.F_MaterialId = item.F_MaterialId;
                entity.F_WorkOrderCode = "WC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + i;
                entity.F_SplitType = 0;
				var classNums = await itemsApp.GetItemList("Mes_ClassNumber");
				var tempStartTime = TimeSpan.Parse(classNums[0].F_Description.Split("-")[0]).TotalMinutes;
				var tempEndTime = TimeSpan.Parse(classNums[0].F_Description.Split("-")[1]).TotalMinutes;
                if (classNums.Count() == 1)
                {
                    entity.F_PlanStartTime = now.AddMinutes(tempStartTime);
                    entity.F_PlanEndTime = now.AddMinutes(tempEndTime);
                }
                else
                {
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
                        if (classNums[j].F_ItemCode == classnum)
                        {
                            entity.F_PlanStartTime = now.AddMinutes(tempStartTime);
                            entity.F_PlanEndTime = now.AddMinutes(tempEndTime);
                            break;
                        }
                        tempStartTime = tempEndTime;
                    }
                }
				entity.F_WorkPlanId = item.F_Id;
                entitys.Add(entity);
                var detail = new WorkOrderDetailEntity();
                detail.Create();
                detail.F_BadNum = 0;
                detail.F_DoneNum = 0;
                detail.F_PlanNum = item.F_PlanNum;
                detail.F_DeleteMark = false;
                detail.F_EnabledMark = true;
                detail.F_MaterialId = item.F_MaterialId;
                detail.F_PlanStartTime = entity.F_PlanStartTime;
                detail.F_PlanEndTime = entity.F_PlanEndTime;
                detail.F_WorkOrderId = entity.F_Id;
                detail.F_WorkOrderState = 0;
                detail.F_RunSort = 1;
                details.Add(detail);
                var banding = new WorkOrderDetailEqpBandingEntity();
                banding.F_Id = Utils.GuId();
                banding.F_WorkOrderDetailId = detail.F_Id;
                banding.F_EqpId = item.F_EqpId;
                banding.F_IsMaster = true;
                banding.F_MouldNum = 0;
                var eqp = await uniwork.FindEntity<EquipmentEntity>(item.F_EqpId);
                banding.F_EqpUse = eqp.F_EqpUse;
                banding.F_EqpType = eqp.F_EqpType;
                bandings.Add(banding);

                var workdetailprocess = new WorkOrderDetailProcessBandingEntity();
                workdetailprocess.F_Id = Utils.GuId();
                workdetailprocess.F_WorkOrderDetailId = detail.F_Id;
                workdetailprocess.F_ProcessId = item.F_ProcessId;
                workdetailprocesses.Add(workdetailprocess);

                i++;
            }
            uniwork.BeginTrans();
            await uniwork.Insert(entitys);
            await uniwork.Insert(details);
            await uniwork.Insert(bandings);
            await uniwork.Insert(workdetailprocesses);
            uniwork.Commit();
        }
        public async Task BatchCreateWorkOrder(string currentdate, string classnum)
        {
            DateTime now = DateTime.Parse(currentdate).Date;
            var datas = IQueryable().Where(a => a.F_Date == now && classnum == a.F_ClassNum).ToList();
            var entitys = new List<WorkOrderEntity>();
            var details = new List<WorkOrderDetailEntity>();
            var bandings = new List<WorkOrderDetailEqpBandingEntity>();

            var workdetailprocesses = new List<WorkOrderDetailProcessBandingEntity>();
            int i = 0;
            var ids = datas.Select(a => a.F_Id).ToList();
            //把已创建工单的计划排除
            var otherids = uniwork.IQueryable<WorkOrderEntity>(a => ids.Contains(a.F_WorkPlanId)).Select(a => a.F_WorkPlanId).ToList();
            if (otherids.Count()>0)
			{
                datas = datas.Where(a => !otherids.Contains(a.F_Id)).ToList();
            }
            foreach (var item in datas.Where(a=>a.F_WorkPlanType == 0))
			{
                var entity = new WorkOrderEntity();
                entity.Create();
                entity.F_DeleteMark = false;
                entity.F_EnabledMark = true;
                entity.F_WorkOrderState = 0;//添写初始化参数;
                entity.F_PlanNum = item.F_PlanNum;
                entity.F_DoneNum = 0;
                entity.F_BadNum = 0;
                entity.F_MaterialId = item.F_MaterialId;
                entity.F_WorkOrderCode = "WC_"+DateTime.Now.ToString("yyyyMMddHHmmss")+i;
                entity.F_SplitType = 0;
				var classNums = await itemsApp.GetItemList("Mes_ClassNumber");
				var tempStartTime = TimeSpan.Parse(classNums[0].F_Description.Split("-")[0]).TotalMinutes;
				var tempEndTime = TimeSpan.Parse(classNums[0].F_Description.Split("-")[1]).TotalMinutes;
                if (classNums.Count() == 1)
                {
					entity.F_PlanStartTime = now.AddMinutes(tempStartTime);
					entity.F_PlanEndTime = now.AddMinutes(tempEndTime);
                }
                else
                {
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
                        if (classNums[j].F_ItemCode == classnum)
                        {
                            entity.F_PlanStartTime = now.AddMinutes(tempStartTime);
                            entity.F_PlanEndTime = now.AddMinutes(tempEndTime);
                            break;
                        }
                        tempStartTime = tempEndTime;
                    }
                }
				entity.F_WorkPlanId = item.F_Id;
                entitys.Add(entity);
                var detail = new WorkOrderDetailEntity();
                detail.Create();
                detail.F_BadNum = 0;
                detail.F_DoneNum = 0;
                detail.F_PlanNum = item.F_PlanNum;
                detail.F_DeleteMark = false;
                detail.F_EnabledMark = true;
                detail.F_MaterialId = item.F_MaterialId;
                detail.F_PlanStartTime = entity.F_PlanStartTime;
                detail.F_PlanEndTime = entity.F_PlanEndTime;
                detail.F_WorkOrderId = entity.F_Id;
                detail.F_WorkOrderState = 0;
                detail.F_RunSort = 1;
                details.Add(detail);
                var banding = new WorkOrderDetailEqpBandingEntity();
                banding.F_Id=Utils.GuId();
                banding.F_WorkOrderDetailId = detail.F_Id;
                banding.F_EqpId = item.F_EqpId;
                banding.F_IsMaster = true;
                banding.F_MouldNum = 0;
                var eqp = await uniwork.FindEntity<EquipmentEntity>(item.F_EqpId);
                banding.F_EqpUse = eqp.F_EqpUse;
                banding.F_EqpType = eqp.F_EqpType;
                bandings.Add(banding);

                var workdetailprocess = new WorkOrderDetailProcessBandingEntity();
                workdetailprocess.F_Id = Utils.GuId();
                workdetailprocess.F_WorkOrderDetailId = detail.F_Id;
                workdetailprocess.F_ProcessId = item.F_ProcessId;
                workdetailprocesses.Add(workdetailprocess);

                i++;
            }
            uniwork.BeginTrans();
            await uniwork.Insert(entitys);
            await uniwork.Insert(details);
            await uniwork.Insert(bandings);
            await uniwork.Insert(workdetailprocesses);
            uniwork.Commit();


        }
        public async Task SubmitAddForm(WorkPlanEntity entity)
        {
            List<WorkPlanEntity> list = new List<WorkPlanEntity>();
            if (entity.F_WorkPlanType == 0)
            {
                foreach (var item in entity.listData.Where(a => a.F_PlanNum > 0))
                {
                    var materialProcess = uniwork.IQueryable<BomFormEntity>(t => t.F_MaterialId == item.F_MaterialId && t.F_ProcessId == item.F_ProcessId).Count();
                    if (!(materialProcess > 0)) throw new Exception("所选择的物料和工序未绑定，请先绑定！");

                    var temp = list.Find(a => a.F_MaterialId == item.F_MaterialId&&a.F_ProcessId==item.F_ProcessId);
                    if (temp == null)
                    {
                        temp = new WorkPlanEntity();
                        temp.F_DeleteMark = false;
                        temp.F_EnabledMark = true;
                        temp.Create();
                        temp.F_DoneNum = 0;
                        temp.F_MaterialId = item.F_MaterialId;
                        temp.F_ProcessId = item.F_ProcessId;
                        temp.F_PlanNum = item.F_PlanNum;
                        temp.F_Date = entity.F_Date;
                        temp.F_ClassNum = entity.F_ClassNum;
                        temp.F_EqpId = entity.F_EqpId;
                        temp.F_EqpName = entity.F_EqpName;
                        temp.F_WorkPlanType = 0;
                        list.Add(temp);
                    }
                    else
                    {
                        temp.F_PlanNum += item.F_PlanNum;
                    }
                }
            }
            DateTime now = entity.F_Date.Date;
            var classnum = entity.F_ClassNum;
            var eqpId = entity.F_EqpId;
            var entitys = new List<WorkOrderEntity>();
            var details = new List<WorkOrderDetailEntity>();
            var bandings = new List<WorkOrderDetailEqpBandingEntity>();
            var workdetailprocesses = new List<WorkOrderDetailProcessBandingEntity>();

            int i = 0;
            var ids = list.Select(a => a.F_Id).ToList();
            //把已创建工单的计划排除
            foreach (var item in list)
            {
                var order = new WorkOrderEntity();
                order.Create();
                order.F_DeleteMark = false;
                order.F_EnabledMark = true;
                order.F_WorkOrderState = 0;//添写初始化参数;
                order.F_PlanNum = item.F_PlanNum;
                order.F_DoneNum = 0;
                order.F_BadNum = 0;
                order.F_MaterialId = item.F_MaterialId;
                order.F_WorkOrderCode = "WC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + i;
                order.F_SplitType = 0;
				var classNums = await itemsApp.GetItemList("Mes_ClassNumber");
				var tempStartTime = TimeSpan.Parse(classNums[0].F_Description.Split("-")[0]).TotalMinutes;
				var tempEndTime = TimeSpan.Parse(classNums[0].F_Description.Split("-")[1]).TotalMinutes;
                if (classNums.Count() == 1)
                {
					order.F_PlanStartTime = now.AddMinutes(tempStartTime);
					order.F_PlanEndTime = now.AddMinutes(tempEndTime);
				}
                else
                {
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
						if (classNums[j].F_ItemCode == classnum)
						{
							order.F_PlanStartTime = now.AddMinutes(tempStartTime);
							order.F_PlanEndTime = now.AddMinutes(tempEndTime);
							break;
						}
						tempStartTime = tempEndTime;
					}
				}
                order.F_WorkPlanId = item.F_Id;
                entitys.Add(order);
                var detail = new WorkOrderDetailEntity();
                detail.Create();
                detail.F_BadNum = 0;
                detail.F_DoneNum = 0;
                detail.F_PlanNum = item.F_PlanNum;
                detail.F_DeleteMark = false;
                detail.F_EnabledMark = true;
                detail.F_MaterialId = item.F_MaterialId;
                detail.F_PlanStartTime = order.F_PlanStartTime;
                detail.F_PlanEndTime = order.F_PlanEndTime;
                detail.F_WorkOrderId = order.F_Id;
                detail.F_WorkOrderState = 0;
                detail.F_RunSort = 1;
                details.Add(detail);
                var banding = new WorkOrderDetailEqpBandingEntity();
                banding.F_Id = Utils.GuId();
                banding.F_WorkOrderDetailId = detail.F_Id;
                banding.F_EqpId = item.F_EqpId;
                banding.F_IsMaster = true;
                banding.F_MouldNum = 0;
                var eqp = await uniwork.FindEntity<EquipmentEntity>(item.F_EqpId);
                banding.F_EqpUse = eqp.F_EqpUse;
                banding.F_EqpType = eqp.F_EqpType;
                bandings.Add(banding);

                var workdetailprocess = new WorkOrderDetailProcessBandingEntity();
                workdetailprocess.F_Id= Utils.GuId();
                workdetailprocess.F_WorkOrderDetailId = detail.F_Id;
                workdetailprocess.F_ProcessId = item.F_ProcessId;
                workdetailprocesses.Add(workdetailprocess);

                i++;
            }
            uniwork.BeginTrans();
            await repository.Insert(list);
            await uniwork.Insert(entitys);
            await uniwork.Insert(details);
            await uniwork.Insert(bandings);
            await uniwork.Insert(workdetailprocesses);
            uniwork.Commit();

        }
        #endregion

    }
}
