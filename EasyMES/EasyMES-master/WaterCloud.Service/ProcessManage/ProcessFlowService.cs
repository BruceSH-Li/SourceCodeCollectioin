using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.MaterialManage;

namespace WaterCloud.Service.ProcessManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-03 09:48
    /// 描 述：工序管理服务类
    /// </summary>
    public class ProcessFlowService : DataFilterService<ProcessFlowEntity>, IDenpendency
    {
        private string cacheKey = "watercloud_processflowdata_";

        public ProcessFlowService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<ProcessFlowEntity>> GetList(string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_ProcessFlowName.Contains(keyword) || t.F_Description.Contains(keyword)).ToList();
            }
            return cachedata.Where(t => t.F_DeleteMark == false).OrderByDescending(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<EquipmentEntity>> GetEqpList(string itemId,string keyword = "")
        {
            var query = uniwork.IQueryable<ProcessEqpBandingEntity>(a => a.F_ProcessId == itemId).LeftJoin<EquipmentEntity>((a, b) => a.F_EqpId == b.F_Id)
                .Select((a, b) => new EquipmentEntity
                {
                    F_Id=a.F_Id,
                    F_CompanyName=b.F_CompanyName,
                    F_CreatorTime=b.F_CreatorTime,
                    F_CreatorUserId=b.F_CreatorUserId,
                    F_Description=b.F_Description,
                    F_UseDate=b.F_UseDate,
                    F_EnabledMark=b.F_EnabledMark,
                    F_EqpName=b.F_EqpName,
                    F_EqpOwner=b.F_EqpOwner,
                    F_EqpState=b.F_EqpState,
                    F_EqpType=b.F_EqpType,
                    F_EqpUse=b.F_EqpUse,
                    F_IsMaintain=b.F_IsMaintain,
                    F_IsOtherEqp=b.F_IsOtherEqp,
                    F_MaintainNum=b.F_MaintainNum,
                    F_MaintainType=b.F_MaintainType,
                    F_MoldingNum=b.F_MoldingNum,
                    F_MouldType=b.F_MouldType,
                    F_ParentId=b.F_ParentId,
                    F_PlanUserNum=b.F_PlanUserNum,
                    F_UsedNum=b.F_UsedNum,
                });
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_EqpName.Contains(keyword) || a.F_Description.Contains(keyword));
            }
            return query.ToList();
        }

		public async Task<List<ProcessFlowEntity>> GetListJsonById(string keyValue)
		{
            var query = uniwork.IQueryable<BomFormEntity>(a => a.F_MaterialId == keyValue).LeftJoin<ProcessFlowEntity>((a, b) => a.F_ProcessId == b.F_Id)
                            .Select((a, b) => b);
            return query.Where(t => t.F_DeleteMark == false).Distinct().OrderByDesc(t => t.F_CreatorTime).ToList();
        }

		public async Task<List<ProcessFlowEntity>> GetLookList(SoulPage<ProcessFlowEntity> pagination,string keyword = "",string id="")
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> enabledTemp = new Dictionary<string, string>();
            enabledTemp.Add("有效", "1");
            enabledTemp.Add("无效", "0");
            dic.Add("F_EnabledMark", enabledTemp);
            pagination = ChangeSoulData(dic, pagination);
            //获取数据权限
            var query = repository.IQueryable().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_ProcessFlowName.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            if(!string.IsNullOrEmpty(id))
            {
                query= query.Where(u=>u.F_Id==id);
            }
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<ProcessFlowEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        #endregion

        public async Task<ProcessFlowEntity> GetLookForm(object keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }

        #region 提交数据
        public async Task SubmitForm(ProcessFlowEntity entity, string keyValue)
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
        public async Task SubmitEqpForm(ProcessEqpBandingEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_Id=Utils.GuId();
                await uniwork.Insert(entity);
            }
            else
            {
                entity.F_Id = keyValue;
                await uniwork.Update(entity);
            }
        }
        public async Task DeleteEqpForm(string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                var list = keyValue.Split(',');
                await uniwork.Delete<ProcessEqpBandingEntity>(t => list.Contains(t.F_Id));
            }

        }
        #endregion

    }
}
