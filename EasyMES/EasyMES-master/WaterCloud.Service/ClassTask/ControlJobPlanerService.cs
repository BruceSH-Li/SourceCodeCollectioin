using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.SystemOrganize;
using WaterCloud.Service.SystemOrganize;

namespace WaterCloud.Service.ClassTask
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-01-15 14:18
    /// 描 述：待执行任务服务类
    /// </summary>
    public class ControlJobPlanerService : DataFilterService<UserEntity>, IDenpendency
    {
        private string cacheKey = "watercloud_userdata_";
        private UserService userApp;
        public ControlJobPlanerService(IDbContext context) : base(context)
        {
            userApp = new UserService(context);
        }
        #region 获取数据
        public async Task<List<UserExtend>> GetLookList(SoulPage<UserExtend> pagination,string keyword = "")
        {
            //获取数据权限
            var query = userApp.GetQuery().Where(u => u.F_IsAdmin == false && u.F_IsPlanMan==true);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_Account.Contains(keyword) || u.F_RealName.Contains(keyword) || u.F_MobilePhone.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            var data = await repository.OrderList(query, pagination);
            var roles = uniwork.IQueryable<RoleEntity>().ToList();
            var orgs = uniwork.IQueryable<OrganizeEntity>().ToList();
            foreach (var item in data)
            {
                string[] roleIds = item.F_RoleId.Split(',');
                string[] departmentIds = item.F_DepartmentId.Split(',');
                item.F_DepartmentName = string.Join(',', orgs.Where(a => departmentIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
                item.F_RoleName = string.Join(',', roles.Where(a => roleIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
            }
            return data;
        }

        public async Task<List<UserExtend>> GetList(string keyword)
        {
            var query = userApp.GetQuery().Where(u => u.F_IsAdmin == false && u.F_IsPlanMan == false);
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
        #endregion

        #region 提交数据
        public async Task SubmitForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Update(t => ids.Contains(t.F_Id), t => new UserEntity
            {
                F_IsPlanMan = true
            });
			foreach (var item in ids)
			{
                await CacheHelper.Remove(cacheKey + item);
            }
            await CacheHelper.Remove(cacheKey + "list");

        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Update(t => ids.Contains(t.F_Id),t=>new UserEntity { 
                F_IsPlanMan=false          
            });
            foreach (var item in ids)
            {
                await CacheHelper.Remove(cacheKey + item);
            }
            await CacheHelper.Remove(cacheKey + "list");
        }
        #endregion

    }
}
