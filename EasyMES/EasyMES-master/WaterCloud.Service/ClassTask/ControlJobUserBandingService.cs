using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.ClassTask;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Service.SystemOrganize;
using WaterCloud.Domain.SystemOrganize;
using WaterCloud.Domain.EquipmentManage;
using System.Threading;

namespace WaterCloud.Service.ClassTask
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-03 13:55
    /// 描 述：信息设置服务类
    /// </summary>
    public class ControlJobUserBandingService : DataFilterService<ControlJobUserBandingEntity>, IDenpendency
    {
        private UserService userApp;
        public ControlJobUserBandingService(IDbContext context) : base(context)
        {
            userApp = new UserService(context);
        }
        #region 获取数据
        public async Task<List<ControlJobUserBandingEntity>> GetList(string keyword = "")
        {
            var data = repository.IQueryable();
            return data.OrderBy(t => t.F_Id).ToList();
        }

        public async Task<List<ControlJobUserBandingEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable();
             //权限过滤
             query = GetDataPrivilege("u", "", query);
            return query.OrderBy(t => t.F_Id).ToList();
        }

        public async Task<List<ControlJobUserBandingEntity>> GetLookList(string eqpId,string itemId = "", string keyword = "")
        {
            var query = repository.IQueryable();
            if(!string.IsNullOrEmpty(itemId))
            {
                var temp = await uniwork.FindEntity<ItemsDetailEntity>(itemId);
				if (temp!=null)
				{
                    var count = int.Parse(temp.F_ItemCode);
                    query = query.Where(u => u.F_JobType == count);
                }
				else
				{
                    query = query.Where(u => u.F_JobType == -99);
                }
            }
            if (!string.IsNullOrEmpty(eqpId))
            {
                query = query.Where(u => u.F_EqpId == eqpId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_EqpName .Contains(keyword)||u.F_UserName.Contains(keyword));
            }
            //权限过滤
            query = GetDataPrivilege("u","",query);
            return query.ToList();
        }

		public async Task<ControlJobMustTimeEntity> GetTimeFormJson(string keyValue)
		{
            var temp = await uniwork.FindEntity<ItemsDetailEntity>(keyValue);
            var jobType = int.Parse(temp.F_ItemCode);
            var entity = await uniwork.FindEntity<ControlJobMustTimeEntity>(a => a.F_JobType == jobType);
            if (entity == null)
			{
                entity = new ControlJobMustTimeEntity();
                entity.F_Id = Utils.GuId();
                entity.F_JobType = jobType;
                entity.F_NeedTime = 0;
            }
            return entity;
		}

		public async Task<List<UserExtend>> GetList(string eqpId, string itemId, string keyword)
        {
            var list = repository.IQueryable();
			if (!string.IsNullOrEmpty(eqpId))
			{
                list = list.Where(a => a.F_EqpId == eqpId);

            }
            if (!string.IsNullOrEmpty(itemId))
            {
                var temp = await uniwork.FindEntity<ItemsDetailEntity>(itemId);
                var count = int.Parse(temp.F_ItemCode);
                list = list.Where(a => a.F_JobType == count);
            }
            var ids = list.Select(a => a.F_UserId).ToList();
            var query = userApp.GetQuery().Where(u => u.F_IsAdmin == false &&!ids.Contains(u.F_Id));
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_Account.Contains(keyword) || u.F_RealName.Contains(keyword) || u.F_MobilePhone.Contains(keyword));
            }
            var roles = uniwork.IQueryable<RoleEntity>().ToList();
            var orgs = uniwork.IQueryable<OrganizeEntity>().ToList();
            var data = query.ToList();
            foreach (var item in data)
            {
                string[] roleIds = item.F_RoleId.Split(',');
                string[] departmentIds = item.F_DepartmentId.Split(',');
                item.F_DepartmentName = string.Join(',', orgs.Where(a => departmentIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
                item.F_RoleName = string.Join(',', roles.Where(a => roleIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
            }
            return data;
        }

		public async Task<ControlJobUserBandingEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<ControlJobUserBandingEntity> GetLookForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(data);
        }

        #region 提交数据
        public async Task SubmitForm(string eqpId, string itemId, string keyValue)
        {
            var temp = await uniwork.FindEntity<ItemsDetailEntity>(itemId);
            var count = int.Parse(temp.F_ItemCode);
            var ids = keyValue.Split(',');
            var list = new List<ControlJobUserBandingEntity>();
			foreach (var item in ids)
			{
                ControlJobUserBandingEntity entity = new ControlJobUserBandingEntity();
                entity.F_Id = Utils.GuId();
                entity.F_UserId = item;
                entity.F_JobType = count;
                entity.F_EqpId = eqpId;
                var user = await uniwork.FindEntity<UserEntity>(item);
                entity.F_UserName = user.F_RealName;
				if (!string.IsNullOrEmpty(eqpId))
				{
                    var eqp = await uniwork.FindEntity<EquipmentEntity>(eqpId);
                    if (eqp != null)
                    {
                        entity.F_EqpName = eqp.F_EqpName;
                    }
                }

                list.Add(entity);
            }
            await repository.Insert(list);
        }

        public async Task SubmitTimeForm(ControlJobMustTimeEntity entity)
        {
            var old = await uniwork.FindEntity<ControlJobMustTimeEntity>(a => a.F_JobType == entity.F_JobType);
			if (old == null)
			{
                entity.F_Id = Utils.GuId();
                await uniwork.Insert(entity);
            }
			else
			{
                await uniwork.Update<ControlJobMustTimeEntity>(a => a.F_Id == old.F_Id, a => new ControlJobMustTimeEntity
                {
                    F_NeedTime = entity.F_NeedTime
                });
			}
        }
        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
        }

        public async Task SubmitCloneForm(int jobType, string ids, string eqpId,bool useEqp)
        {
            string[] ArrayId = ids.Split(',');
            var data = await this.GetList();
            List<ControlJobUserBandingEntity> entitys = new List<ControlJobUserBandingEntity>();
            var eqp = await uniwork.FindEntity<EquipmentEntity>(a => a.F_Id == eqpId);
            foreach (ControlJobUserBandingEntity entity in data.Where(a=> ArrayId.Contains(a.F_Id)).OrderBy(a=>a.F_Id))
            {
                entity.F_Id = Utils.GuId();
                Console.WriteLine(entity.F_Id);
                entity.F_JobType = jobType;
				if (useEqp)
				{
                    if (eqp != null)
                    {
                        entity.F_EqpId = eqpId;
                        entity.F_EqpName = eqp.F_EqpName;
                    }
					else
					{
                        entity.F_EqpId = null;
                        entity.F_EqpName = null;
                    }
                }
                entitys.Add(entity);
            }
            await repository.Insert(entitys);
        }
        #endregion

    }
}
