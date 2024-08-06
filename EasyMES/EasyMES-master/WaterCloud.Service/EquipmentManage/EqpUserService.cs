using System;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using Chloe;
using System.Collections.Generic;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Domain.SystemOrganize;

namespace WaterCloud.Service.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-01 12:23
    /// 描 述：模具用户绑定服务类
    /// </summary>
    public class EqpUserService : DataFilterService<EqpUserBandingEntity>, IDenpendency
    {

        public EqpUserService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<UserEntity>> GetLookList(string itemId, string keyword = "")
        {
            var query = repository.IQueryable(a => a.F_EqpId == itemId).InnerJoin<UserEntity>((a, b) => a.F_UserId == b.F_Id&&b.F_DeleteMark==false&&b.F_EnabledMark==true).Select((a, b) => new UserEntity
            {
                F_Id = a.F_Id,
                F_Account = b.F_Account,
                F_RealName = b.F_RealName,
                F_DeleteMark = b.F_DeleteMark,
                F_DeleteTime = b.F_DeleteTime,
                F_DeleteUserId = b.F_DeleteUserId,
                F_Description = b.F_Description,
                F_CreatorTime = b.F_CreatorTime,
                F_CreatorUserId = b.F_CreatorUserId,
                F_DepartmentId=b.F_DepartmentId,
                F_EnabledMark = b.F_EnabledMark,
            });
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_Account.Contains(keyword) || a.F_RealName.Contains(keyword));
            }
            var data= query.ToList();
            var orgs = uniwork.IQueryable<OrganizeEntity>().ToList();
            foreach (var item in data)
            {
                string[] departmentIds = item.F_DepartmentId.Split(',');
                item.F_DepartmentName = string.Join(',', orgs.Where(a => departmentIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
            }
            return data;
        }

        public async Task<List<UserEntity>> GetAddList(string itemId, string keyword)
        {
            var unlist = repository.IQueryable(a => a.F_EqpId == itemId).InnerJoin<UserEntity>((a, b) => a.F_UserId == b.F_Id).Select((a, b) => b.F_Id).ToList();
            var query = uniwork.IQueryable<UserEntity>(a => !unlist.Contains(a.F_Id)&&a.F_IsAdmin==false&&a.F_DeleteMark==false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_Account.Contains(keyword) || a.F_RealName.Contains(keyword));
            }
            var data = query.ToList();
            var orgs = uniwork.IQueryable<OrganizeEntity>().ToList();
            foreach (var item in data)
            {
                string[] departmentIds = item.F_DepartmentId.Split(',');
                item.F_DepartmentName = string.Join(',', orgs.Where(a => departmentIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
            }
            return data;
        }

        public async Task<EquipmentEntity> GetLookForm(object keyValue)
        {
            var data = await uniwork.FindEntity<EquipmentEntity>(keyValue);
            return data;
        }
        #endregion


        #region 提交数据
        public async Task SubmitForm(EqpUserBandingEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
				if (!string.IsNullOrEmpty(entity.F_UserId))
				{
                    var temps = entity.F_UserId.Split(',');
                    var list = new List<EqpUserBandingEntity>();
					foreach (var item in temps)
					{
                        EqpUserBandingEntity temp = new EqpUserBandingEntity();
                        temp.F_Id = Utils.GuId();
                        temp.F_EqpId = entity.F_EqpId;
                        temp.F_UserId = item;
                        list.Add(temp);
                    }
                    await repository.Insert(list);
                }
            }
            else
            {
                entity.F_Id = keyValue; 
                await repository.Update(entity);
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                var ids = keyValue.Split(',');
                await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
            }
        }
        #endregion

    }
}
