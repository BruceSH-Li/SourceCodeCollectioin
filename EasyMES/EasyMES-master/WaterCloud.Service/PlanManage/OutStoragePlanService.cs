using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Domain.PlanManage;
using WaterCloud.Service.SystemManage;
using WaterCloud.Domain.ClassTask;
using System.Net.Http;
using WaterCloud.Service.ClassTask;

namespace WaterCloud.Service.PlanManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-23 14:04
    /// 描 述：交付计划服务类
    /// </summary>
    public class OutStoragePlanService : DataFilterService<OutStorageEntity>, IDenpendency
    {
        private ItemsDataService itemsApp;
        private ControlJobService jobApp;
        public OutStoragePlanService(IDbContext context, IHttpClientFactory httpClientFactory) : base(context)
        {
            itemsApp = new ItemsDataService(context);
            jobApp = new ControlJobService(context, httpClientFactory);
        }
        #region 获取数据

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
            DateTime endTime = startTime.AddDays(CountDay);
            var datas = repository.IQueryable().Where(a => a.F_PlanTime >= startTime && a.F_PlanTime < endTime).ToList();
            var datat = uniwork.IQueryable<MaterialEntity>(t => t.F_MaterialType > 0 && t.F_EnabledMark == true && t.F_DeleteMark == false ).OrderBy(t => t.F_MaterialCode).ToList();
            var data = new List<Object>();
            foreach (var item in datat)
            {
                Dictionary<string, string> temp = new Dictionary<string, string>();
                temp["F_MaterialId"] = item.F_Id;
                temp["F_MaterialName"] = item.F_MaterialName;
                for (int i = 0; i < CountDay; i++)
                {
                    temp[startTime.AddDays(i).ToString("yyyy/MM/dd")] = "";
                    if (datas.Where(a => a.F_MaterialId == item.F_Id && startTime.AddDays(i) == a.F_PlanTime ).Count() > 0)
                    {
                        var entitys = datas.Where(a => a.F_MaterialId == item.F_Id && startTime.AddDays(i) == a.F_PlanTime).ToList();
                        var entity = entitys.FirstOrDefault();
                        entity.F_Num = entitys.Sum(a => a.F_Num);
                        entity.F_DoneNum = entitys.Sum(a => a.F_DoneNum);
                        temp[startTime.AddDays(i).ToString("yyyy/MM/dd")] = entity.F_MaterialName + ",计划数量" + entity.F_Num + ",实际数量" + entity.F_DoneNum;

                    }
                }

                data.Add(temp);
            }

            var cols = new List<string>();
            for (int i = 0; i < CountDay; i++)
            {
                cols.Add(startTime.AddDays(i).ToString("yyyy/MM/dd"));
            }
            return new
            {
                data,
                cols
            };
        }

        public async Task<OutStorageEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<OutStorageEntity> GetLookForm(string keyValue, string materialId)
        {
            var startTime = DateTime.Parse(keyValue);
            var data = repository.IQueryable(a=>a.F_PlanTime==startTime&&a.F_MaterialId==materialId).ToList();
            var entity = data.FirstOrDefault();
            if (entity!=null)
            {
                entity.F_Num = data.Sum(a => a.F_Num);
                entity.F_DoneNum = data.Sum(a => a.F_DoneNum);
                var order = await uniwork.FindEntity<OrderEntity>(a => a.F_Id == entity.F_OrderId);
                if (order != null)
                {
                    entity.F_OrderCode = order.F_OrderCode;
                }
            }
            return GetFieldsFilterData(entity);
        }
        public async Task<List<OutStorageEntity>> GetListJson(SoulPage<OutStorageEntity> pagination, string keyValue, string materialId)
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            var setList = await itemsApp.GetItemList("Mes_WorkOrderState");
            Dictionary<string, string> stateTemp = new Dictionary<string, string>();
            foreach (var item in setList)
            {
                stateTemp.Add(item.F_ItemName, item.F_ItemCode);
            }
            dic.Add("F_OutStorageState", stateTemp);
            pagination = ChangeSoulData(dic, pagination);
            //获取数据权限
            var query = IQueryable();
            if (!string.IsNullOrEmpty(keyValue))
            {
                //此处需修改
                var time = DateTime.Parse(keyValue);
                query = query.Where(t => t.F_PlanTime == time);
            }
            if (!string.IsNullOrEmpty(materialId))
            {
                query = query.Where(u => u.F_MaterialId == materialId);
            }
            query = GetDataPrivilege("u", "", query);
            return await repository.OrderList(query, pagination);
        }

        private IQuery<OutStorageEntity> IQueryable()
        {
            var query = repository.IQueryable()
                .LeftJoin<OrderEntity>((a, b) => a.F_OrderId == b.F_Id)
                .Select((a, b) => new OutStorageEntity
                {
                    F_CarNo = a.F_CarNo,
                    F_CreatorTime = a.F_CreatorTime,
                    F_CreatorUserId = a.F_CreatorUserId,
                    F_Description = a.F_Description,
                    F_DoneNum = a.F_DoneNum,
                    F_EnabledMark = a.F_EnabledMark,
                    F_Id = a.F_Id,
                    F_OrderCode = b.F_OrderCode,
                    F_LastModifyTime = a.F_LastModifyTime,
                    F_LastModifyUserId = a.F_LastModifyUserId,
                    F_MaterialCode = a.F_MaterialCode,
                    F_MaterialId = a.F_MaterialId,
                    F_MaterialName = a.F_MaterialName,
                    F_MaterialType = a.F_MaterialType,
                    F_MaterialUnit = a.F_MaterialUnit,
                    F_Num = a.F_Num,
                    F_OrderId = a.F_OrderId,
                    F_OutStorageCode = a.F_OutStorageCode,
                    F_OutStorageState = a.F_OutStorageState,
                    F_PlanTime = a.F_PlanTime,
                    F_IsUserEdit=a.F_IsUserEdit,

                });
            return query;
        }
        #region 提交数据
        public async Task SubmitForm(OutStorageEntity entity)
        {
            if (string.IsNullOrEmpty(entity.F_Id))
            {
                if (entity.F_PlanTime == null)
                {
                    throw new Exception("交付计划没有时间，无法制定交付计划");
                }
                if (entity.F_PlanTime < DateTime.Now.Date)
                {
                    throw new Exception("交付计划时间必须的今天以后");
                }
                var material = await uniwork.FindEntity<MaterialEntity>(entity.F_MaterialId);
                entity.F_OutStorageState = 0;
                entity.F_OutStorageCode = "ON-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                entity.Create();
                if (DateTime.Now.Date != entity.F_PlanTime)
                {
                    entity.F_EnabledMark = false;
                }
                else
                {
                    entity.F_EnabledMark = true;
                }
                entity.F_DoneNum = 0;
                entity.F_MaterialCode = material.F_MaterialCode;
                entity.F_MaterialName = material.F_MaterialName;
                entity.F_IsUserEdit = true;
                entity.F_MaterialType = material.F_MaterialType;
                entity.F_MaterialUnit = material.F_MaterialUnit;
                entity.F_IsUserEdit = true;
                if (!string.IsNullOrEmpty(entity.F_OrderId))
                {
                    var order = await uniwork.FindEntity<OrderDetailEntity>(a => a.F_OrderId == entity.F_OrderId && a.F_MaterialId == entity.F_MaterialId);
                    if (order == null)
                    {
                        throw new Exception("订单不需要此物料，请检查");
                    }
                }
                await repository.Insert(entity);
            }
            else
            {
                entity.Modify(entity.F_Id);
                entity.F_IsUserEdit = true;
                var old = await repository.FindEntity(entity.F_Id);
                if (old.F_DoneNum > 0)
                {
                    throw new Exception("出库单已开始作业无法修改");
                }
                await repository.Update(entity);
            }     
        }

        public async Task DeleteForm(string currentdate, string materialId)
        {
            var startTime = DateTime.Parse(currentdate);
            if (repository.IQueryable(a => a.F_PlanTime == startTime && a.F_MaterialId == materialId && a.F_OutStorageState > 0).Count() > 0)
            {
                throw new Exception("已有出库单开始作业，无法删除交付计划");
            }
            await repository.Delete(a => a.F_PlanTime == startTime && a.F_MaterialId == materialId && a.F_OutStorageState == 0);
        }

        public async Task DeleteAllForm(string currentdate)
        {
            var startTime = DateTime.Parse(currentdate);
            if (repository.IQueryable(a => a.F_PlanTime == startTime && a.F_OutStorageState > 0).Count() > 0)
            {
                throw new Exception("已有出库单开始作业，无法删除交付计划");
            }
            await repository.Delete(a => a.F_PlanTime == startTime && a.F_OutStorageState == 0);
        }
        public async Task DeleteAddForm(string keyValue)
        {
            if (repository.IQueryable(a => a.F_Id == keyValue && a.F_OutStorageState > 0).Count() > 0)
            {
                throw new Exception("出库单已开始作业，无法删除入库单");
            }
            await repository.Delete(a => a.F_Id == keyValue && a.F_OutStorageState == 0);
        }
        public async Task EnabledForm(string keyValue)
        {
            var outNote = await repository.FindEntity(keyValue);
            if (uniwork.IQueryable<StorageEntity>(a => a.F_MaterialId == outNote.F_MaterialId && a.F_IsCheckout == true).Sum(a => a.F_Num) < outNote.F_Num)
            {
                throw new Exception("库存不足无法启动");
            }
            uniwork.BeginTrans();
            await repository.Update(a => a.F_Id == keyValue && a.F_EnabledMark == false, a => new OutStorageEntity
            {
                F_EnabledMark = true
            });
            if (outNote == null || outNote.F_OutStorageState > 0)
            {
                throw new Exception("出库单已开始作业无需启动");
            }
            else
            {
                await repository.Update(a => a.F_Id == keyValue && a.F_OutStorageState == 0 && a.F_DoneNum == 0, a => new OutStorageEntity
                {
                    F_OutStorageState = 1
                });
            }
            if (outNote.F_PlanTime != null && ((DateTime)outNote.F_PlanTime).Date != DateTime.Now.Date)
            {
                throw new Exception("出库单计划时间不是今天的，请检查");
            }
            //创建任务
            ControlJobEntity jobEntity = new ControlJobEntity();
            jobEntity.F_JobType = 2;
            jobEntity.F_NeedId = keyValue;
            jobEntity.F_Priority = 1;
            jobEntity.F_NeedNum = outNote.F_Num;
            jobEntity.F_JobCode = "CJ_" + DateTime.Now.ToString("yyyyMMddHHmmss") + 0;
            jobEntity.F_JobInfo = "产品出库,出库单号为" + outNote.F_OutStorageCode + ",物料编号为" + outNote.F_MaterialCode + ",名称为" + outNote.F_MaterialCode + "的物料需要出库。";
            jobEntity.F_JobInfo += "数量为" + outNote.F_Num;
            await jobApp.SubmitForm(jobEntity);
            uniwork.Commit();
        }

        public async Task EnabledAllForm(string currentdate)
        {
            DateTime now = DateTime.Parse(currentdate).Date;
            await repository.Update(a => a.F_PlanTime == now && a.F_EnabledMark == false, a => new OutStorageEntity
            {
                F_EnabledMark = true
            });
        }
        #endregion

    }
}
