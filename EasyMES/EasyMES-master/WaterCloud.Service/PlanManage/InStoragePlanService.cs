using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Service.SystemManage;
using WaterCloud.Domain.ClassTask;
using WaterCloud.Service.ClassTask;
using System.Net.Http;

namespace WaterCloud.Service.PlanManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-23 14:04
    /// 描 述：采购计划服务类
    /// </summary>
    public class InStoragePlanService : DataFilterService<InStorageEntity>, IDenpendency
    {
        private ItemsDataService itemsApp;
        private ControlJobService jobApp;
        public InStoragePlanService(IDbContext context, IHttpClientFactory httpClientFactory) : base(context)
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
            var datat = uniwork.IQueryable<MaterialEntity>(t => t.F_MaterialType < 2 && t.F_EnabledMark == true && t.F_DeleteMark == false ).OrderBy(t => t.F_MaterialCode).ToList();
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
                        var entitys = datas.Where(a => a.F_MaterialId == item.F_Id && startTime.AddDays(i) == a.F_PlanTime ).ToList();
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

        public async Task<List<InStorageEntity>> GetListJson(SoulPage<InStorageEntity> pagination, string keyValue, string materialId)
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            var setList = await itemsApp.GetItemList("Mes_WorkOrderState");
            Dictionary<string, string> stateTemp = new Dictionary<string, string>();
            foreach (var item in setList)
            {
                stateTemp.Add(item.F_ItemName, item.F_ItemCode);
            }
            dic.Add("F_InStorageState", stateTemp);
            pagination = ChangeSoulData(dic, pagination);
            //获取数据权限
            var query = repository.IQueryable();
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

        public async Task<InStorageEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<InStorageEntity> GetLookForm(string keyValue, string materialId)
        {
            var startTime = DateTime.Parse(keyValue);
            var data = repository.IQueryable(a=>a.F_PlanTime==startTime&&a.F_MaterialId==materialId).ToList();
            var entity = data.FirstOrDefault();
            if (entity!=null)
            {
                entity.F_Num = data.Sum(a => a.F_Num);
                entity.F_DoneNum = data.Sum(a => a.F_DoneNum);
            }
            return GetFieldsFilterData(entity);
        }

        #region 提交数据
        public async Task SubmitForm(InStorageEntity entity)
        {
            if (string.IsNullOrEmpty(entity.F_Id))
            {
                if (entity.F_PlanTime == null)
                {
                    throw new Exception("采购计划没有时间，无法制定采购计划");
                }
                var material = await uniwork.FindEntity<MaterialEntity>(entity.F_MaterialId);
                entity.F_InStorageState = 0;
                entity.F_InStorageCode = "IN-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                entity.Create();
                entity.F_DoneNum = 0;
                if (DateTime.Now.Date != entity.F_PlanTime)
                {
                    entity.F_EnabledMark = false;
                }
                else
                {
                    entity.F_EnabledMark = true;
                }
                entity.F_BadNum = 0;
                entity.F_MaterialCode = material.F_MaterialCode;
                entity.F_IsUserEdit = true;
                entity.F_MaterialName = material.F_MaterialName;
                entity.F_MaterialType = material.F_MaterialType;
                entity.F_MaterialUnit = material.F_MaterialUnit;
                entity.F_IsUserEdit = true;
                await repository.Insert(entity);
            }
            else
            {
                entity.F_IsUserEdit = true;
                var old = await repository.FindEntity(entity.F_Id);
                if (old.F_InStorageState > 0 && old.F_DoneNum > 0)
                {
                    throw new Exception("入库单已开始作业无法修改");
                }
                var material = await uniwork.FindEntity<MaterialEntity>(entity.F_MaterialId);
                entity.F_MaterialCode = material.F_MaterialCode;
                entity.F_MaterialName = material.F_MaterialName;
                entity.F_MaterialType = material.F_MaterialType;
                entity.F_MaterialUnit = material.F_MaterialUnit;
                if (entity.list != null && entity.list.Count > 0)
                {
                    foreach (var item in entity.list)
                    {
                        item.F_MaterialName = entity.F_MaterialName;
                    }
                    entity.F_BatchJson = entity.list.ToJson();
                }
                await repository.Update(a => a.F_Id == entity.F_Id, a => new InStorageEntity
                {
                    F_MaterialId = entity.F_MaterialId,
                    F_Description = entity.F_Description,
                    F_MaterialCode = entity.F_MaterialCode,
                    F_MaterialName = entity.F_MaterialName,
                    F_MaterialType = entity.F_MaterialType,
                    F_Num = entity.F_Num,
                    F_IsUserEdit = true
                });
            }
            
        }

        public async Task DeleteForm(string currentdate, string materialId)
        {
            var startTime = DateTime.Parse(currentdate);
            if (repository.IQueryable(a => a.F_PlanTime == startTime && a.F_MaterialId == materialId && a.F_InStorageState > 0).Count() > 0)
            {
                throw new Exception("已有入库单开始作业，无法删除采购计划");
            }
            await repository.Delete(a => a.F_PlanTime == startTime && a.F_MaterialId == materialId && a.F_InStorageState == 0);
        }

        public async Task DeleteAllForm(string currentdate)
        {
            var startTime = DateTime.Parse(currentdate);
            if (repository.IQueryable(a => a.F_PlanTime == startTime && a.F_InStorageState > 0).Count() > 0)
            {
                throw new Exception("已有入库单开始作业，无法删除采购计划");
            }
            await repository.Delete(a => a.F_PlanTime == startTime && a.F_InStorageState == 0);
        }
        public async Task DeleteAddForm(string keyValue)
        {
            if (repository.IQueryable(a => a.F_Id == keyValue && a.F_InStorageState > 0).Count() > 0)
            {
                throw new Exception("入库单已开始作业，无法删除入库单");
            }
            await repository.Delete(a => a.F_Id == keyValue && a.F_InStorageState == 0);
        }

        public async Task EnabledForm(string keyValue)
        {
            var inNote = await repository.FindEntity(keyValue);
            uniwork.BeginTrans();
            await repository.Update(a => a.F_Id == keyValue && a.F_EnabledMark == false, a => new InStorageEntity
            {
                F_EnabledMark = true
            });
            if (inNote == null || inNote.F_InStorageState > 0)
            {
                throw new Exception("入库单已开始作业无需启动");
            }
            if (inNote.F_PlanTime != null && ((DateTime)inNote.F_PlanTime).Date != DateTime.Now.Date)
            {
                throw new Exception("入库单计划时间不是今天的，请检查");
            }
            if (!string.IsNullOrEmpty(inNote.F_BatchJson))
            {
                await repository.Update(a => a.F_Id == keyValue && a.F_InStorageState == 0 && a.F_DoneNum == 0, a => new InStorageEntity
                {
                    F_InStorageState = 2,
                    F_DoneNum = a.F_Num
                });
                var list = inNote.F_BatchJson.ToObject<List<InStorageInfoEntity>>();
                List<TransferBoxEntity> transfers = new List<TransferBoxEntity>();
                List<InStorageInfoEntity> temps = new List<InStorageInfoEntity>();
                //批量增加临时容器
                //批量绑定
                DateTime dt = DateTime.Now;
                foreach (var item in list)
                {
                    TransferBoxEntity transfer = new TransferBoxEntity();
                    InStorageInfoEntity temp = new InStorageInfoEntity();
                    transfer.Create();
                    transfer.F_CreatorTime = dt;
                    transfer.F_TransferCode = item.F_MaterialBatch;
                    transfer.F_EnabledMark = true;
                    transfer.F_IsTemp = true;
                    transfer.F_TransferState = 1;
                    transfer.F_TransferMaxNum = 125;
                    transfer.F_TransferType = "0";
                    transfer.F_DeleteMark = false;
                    transfers.Add(transfer);
                    temp.F_Id = 0;
                    //获取物料信息
                    var material = await uniwork.FindEntity<MaterialEntity>(a => a.F_Id == inNote.F_MaterialId);
                    if (material == null)
                    {
                        throw new Exception($"物料编号:{material.F_MaterialCode}不存在");
                    }
                    temp.F_IsTemp = true;
                    temp.F_MaterialId = material.F_Id;
                    temp.F_MaterialCode = material.F_MaterialCode;
                    temp.F_MaterialName = material.F_MaterialName;
                    temp.F_MaterialType = material.F_MaterialType;
                    temp.F_MaterialUnit = material.F_MaterialUnit;
                    temp.F_EnabledMark = true;
                    temp.F_MaterialBatch = item.F_MaterialBatch;
                    temp.F_InStorageId = inNote.F_Id;
                    temp.F_InStorageCode = inNote.F_InStorageCode;
                    temp.F_Num = item.F_Num;
                    temp.F_TransferBoxCode = transfer.F_TransferCode;
                    temp.F_CreatorUserId = currentuser.UserId;
                    temp.F_BandingUserId = temp.F_CreatorUserId;
                    temp.F_BandingUserName = currentuser.UserName;
                    temp.F_BandingTime = dt;
                    temps.Add(temp);
                }
                await uniwork.Insert(transfers);
                await uniwork.Insert(temps);
            }
            else
            {
                await repository.Update(a => a.F_Id == keyValue && a.F_InStorageState == 0 && a.F_DoneNum == 0, a => new InStorageEntity
                {
                    F_InStorageState = 1
                });
            }
            //创建任务
            ControlJobEntity jobEntity = new ControlJobEntity();
            jobEntity.F_JobType = 1;
            jobEntity.F_NeedId = keyValue;
            jobEntity.F_NeedNum = inNote.F_Num;
            jobEntity.F_Priority = 1;
            jobEntity.F_JobCode = "CJ_" + DateTime.Now.ToString("yyyyMMddHHmmss") + 0; ;
            jobEntity.F_JobInfo = "来料入库,入库号为" + inNote.F_InStorageCode + ",物料编号为" + inNote.F_MaterialCode + ",名称为" + inNote.F_MaterialName + "的物料需要入库。";
            jobEntity.F_JobInfo += "数量为" + inNote.F_Num;
            await jobApp.SubmitForm(jobEntity);
            uniwork.Commit();
        }

        public async Task EnabledAllForm(string currentdate)
        {
            DateTime now = DateTime.Parse(currentdate).Date;
            await repository.Update(a => a.F_PlanTime==now && a.F_EnabledMark == false, a => new InStorageEntity
            {
                F_EnabledMark = true
            });
        }
        #endregion

    }
}
