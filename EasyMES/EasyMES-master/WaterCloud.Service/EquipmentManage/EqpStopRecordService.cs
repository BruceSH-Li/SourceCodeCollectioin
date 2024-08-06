using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.EquipmentManage;

namespace WaterCloud.Service.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-10 15:47
    /// 描 述：设备停机服务类
    /// </summary>
    public class EqpStopRecordService : DataFilterService<EqpStopRecordEntity>, IDenpendency
    {
        public EqpStopRecordService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<EqpStopRecordEntity>> GetList(string keyword = "")
        {
            var data = repository.IQueryable();
            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    data = data.Where(t => t.F_EqpId.Contains(keyword)
            //    || t.F_FullName.Contains(keyword));
            //}
            return data.Where(t => t.F_DeleteMark == false).OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<EqpStopRecordEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                //query = query.Where(t => t.F_EnCode.Contains(keyword)
                //|| t.F_FullName.Contains(keyword));
            }
             //权限过滤
             query = GetDataPrivilege("u", "", query);
            return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<EqpStopRecordEntity>> GetLookList(SoulPage<EqpStopRecordEntity> pagination,string F_EqpId,string StartTime,string EndTime,int StopType)
        {
            var result = repository.IQueryable().Where(t => t.F_DeleteMark == false && t.F_StopType == StopType).Where(t=>string.IsNullOrEmpty(t.F_RepeatWeekday));

            if (!string.IsNullOrEmpty(F_EqpId))
                result = result.Where(u => u.F_EqpId == F_EqpId);

            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
                result = result.Where(u => (Sql.DiffSeconds(StartTime.ToDate(), u.F_PlanStopStartTime) >= 0 && Sql.DiffSeconds(u.F_PlanStopStartTime, EndTime.ToDate()) >= 0)
                || (Sql.DiffSeconds(StartTime.ToDate(), u.F_RealStopStartTime) >= 0 && Sql.DiffSeconds(u.F_RealStopStartTime, EndTime.ToDate()) >= 0));
            else if (!string.IsNullOrEmpty(StartTime) && string.IsNullOrEmpty(EndTime))
                result = result.Where(u => (Sql.DiffSeconds(StartTime.ToDate(), u.F_PlanStopStartTime) >= 0)
                || (Sql.DiffSeconds(StartTime.ToDate(), u.F_RealStopStartTime) >= 0));
            else if (string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
                result = result.Where(u => (Sql.DiffSeconds(u.F_PlanStopStartTime, EndTime.ToDate()) >= 0)
                ||(Sql.DiffSeconds(u.F_RealStopStartTime, EndTime.ToDate()) >= 0));

           
            //权限过滤
            result = GetDataPrivilege("u","", result);
            var repeatquery= repository.IQueryable().Where(t => t.F_DeleteMark == false).Where(t => !string.IsNullOrEmpty(t.F_RepeatWeekday));
            if (!string.IsNullOrEmpty(F_EqpId))
                repeatquery = repeatquery.Where(u => u.F_EqpId == F_EqpId);

            var query = await repository.OrderList(result, pagination);
            //var query = result.ToList();
            if (StopType==2)
            {
                query.AddRange(repeatquery.ToList());
            }
            return query.Distinct().ToList();
        }

        public async Task<EqpStopRecordEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<EqpStopRecordEntity> GetLookForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(data);
        }

        #region 提交数据
        public async Task SubmitForm(EqpStopRecordEntity entity, string keyValue)
        {
            if(string.IsNullOrEmpty(keyValue))
            {
                List<EqpStopRecordEntity> list = new List<EqpStopRecordEntity>();
                Dictionary<DateTime, DateTime> datetime = new Dictionary<DateTime, DateTime>();
                var starttime = (DateTime)entity.F_PlanStopStartTime;
                while (starttime < entity.F_PlanStopEndTime)
                {
                    var endtime = starttime.Date.AddHours(32);
                    if (starttime.Hour < 8)
                    {
                        endtime= starttime.Date.AddHours(8);
                    }
                    if (endtime >= entity.F_PlanStopEndTime)
                    {
                        endtime = (DateTime)entity.F_PlanStopEndTime;
                    }
                    datetime.Add(starttime, endtime);
                    starttime = endtime;
                }
                if (entity.F_EqpId.IndexOf(",") > 0)
                {
                    var EqpList = entity.F_EqpId.Split(",");
                    var EqpNameList = entity.F_EqpName.Split(",");
                    for (var i = 0; i < EqpList.Length; i++)
                    {
                        foreach (var dic in datetime)
                        {
                            var temp = entity.ToJson().ToObject<EqpStopRecordEntity>();
                            temp.F_EqpId = EqpList[i];
                            temp.F_EqpName = EqpNameList[i];
                            temp.F_DeleteMark = false;
                            temp.F_EnabledMark = true;
                            temp.F_PlanStopStartTime = dic.Key;
                            temp.F_PlanStopEndTime = dic.Value;
                            if (repository.IQueryable(a => a.F_EqpId == temp.F_EqpId && a.F_StopType == temp.F_StopType && ((temp.F_PlanStopStartTime <= a.F_PlanStopStartTime && temp.F_PlanStopEndTime > a.F_PlanStopStartTime) || (temp.F_PlanStopStartTime > a.F_PlanStopEndTime && temp.F_PlanStopEndTime <= a.F_PlanStopEndTime))).Count() > 0)
                            {
                                throw new Exception("停机的时间段已存在，请检查");
							}
                            temp.Create();
                            list.Add(temp);
                        }
                    }
                }
                else
                {
                    foreach (var dic in datetime)
                    {
                        var temp = entity.ToJson().ToObject<EqpStopRecordEntity>();
                        temp.F_DeleteMark = false;
                        temp.F_EnabledMark = true;
                        temp.F_PlanStopStartTime = dic.Key;
                        temp.F_PlanStopEndTime = dic.Value;
                        if (repository.IQueryable(a => a.F_EqpId == temp.F_EqpId && a.F_StopType == temp.F_StopType && ((temp.F_PlanStopStartTime <= a.F_PlanStopStartTime && temp.F_PlanStopEndTime > a.F_PlanStopStartTime) || (temp.F_PlanStopStartTime > a.F_PlanStopEndTime && temp.F_PlanStopEndTime <= a.F_PlanStopEndTime))).Count() > 0)
                        {
                            throw new Exception("停机的时间段已存在，请检查");
                        }
                        temp.Create();
                        list.Add(temp);
                    }
                }
                await repository.Insert(list);
            }
            else
            {
                entity.Modify(keyValue);
                await repository.Delete(entity);
                List<EqpStopRecordEntity> list = new List<EqpStopRecordEntity>();
                Dictionary<DateTime, DateTime> datetime = new Dictionary<DateTime, DateTime>();
                var starttime = (DateTime)entity.F_PlanStopStartTime;
                while (starttime < entity.F_PlanStopEndTime)
                {
                    var endtime = starttime.Date.AddHours(32);
                    if (starttime.Hour < 8)
                    {
                        endtime = starttime.Date.AddHours(8);
                    }
                    if (endtime >= entity.F_PlanStopEndTime)
                    {
                        endtime = (DateTime)entity.F_PlanStopEndTime;
                    }
                    datetime.Add(starttime, endtime);
                    starttime = endtime;
                }
                if (entity.F_EqpId.IndexOf(",") > 0)
                {
                    var EqpList = entity.F_EqpId.Split(",");
                    var EqpNameList = entity.F_EqpName.Split(",");
                    for (var i = 0; i < EqpList.Length; i++)
                    {
                        foreach (var dic in datetime)
                        {
                            var temp = entity.ToJson().ToObject<EqpStopRecordEntity>();
                            temp.F_EqpId = EqpList[i];
                            temp.F_EqpName = EqpNameList[i];
                            temp.F_DeleteMark = false;
                            temp.F_EnabledMark = true;
                            temp.F_PlanStopStartTime = dic.Key;
                            temp.F_PlanStopEndTime = dic.Value;
                            if (repository.IQueryable(a => a.F_EqpId == temp.F_EqpId && a.F_StopType == temp.F_StopType && ((temp.F_PlanStopStartTime <= a.F_PlanStopStartTime && temp.F_PlanStopEndTime > a.F_PlanStopStartTime) || (temp.F_PlanStopStartTime > a.F_PlanStopEndTime && temp.F_PlanStopEndTime <= a.F_PlanStopEndTime))).Count() > 0)
                            {
                                throw new Exception("停机的时间段已存在，请检查");
                            }
                            temp.Create();
                            list.Add(temp);
                        }
                    }
                }
                else
                {
                    foreach (var dic in datetime)
                    {
                        var temp = entity.ToJson().ToObject<EqpStopRecordEntity>();
                        temp.F_DeleteMark = false;
                        temp.F_EnabledMark = true;
                        temp.F_PlanStopStartTime = dic.Key;
                        temp.F_PlanStopEndTime = dic.Value;
                        if (repository.IQueryable(a => a.F_EqpId == temp.F_EqpId && a.F_StopType == temp.F_StopType && ((temp.F_PlanStopStartTime <= a.F_PlanStopStartTime && temp.F_PlanStopEndTime > a.F_PlanStopStartTime) || (temp.F_PlanStopStartTime > a.F_PlanStopEndTime && temp.F_PlanStopEndTime <= a.F_PlanStopEndTime))).Count() > 0)
                        {
                            throw new Exception("停机的时间段已存在，请检查");
                        }
                        temp.Create();
                        list.Add(temp);
                    }
                }
                await repository.Insert(list);
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
        }
        #endregion

    }
}
