using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.QualityManage;
using WaterCloud.Domain.MaterialManage;

namespace WaterCloud.Service.QualityManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-04-23 15:23
    /// 描 述：质检参数服务类
    /// </summary>
    public class CheckoutDescriptionService : DataFilterService<CheckoutDescriptionEntity>, IDenpendency
    {
        public CheckoutDescriptionService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<CheckoutDescriptionEntity>> GetList(string keyword = "")
        {
            var data = repository.IQueryable(a=>a.F_Layers<=3);
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(t => t.F_Description.Contains(keyword));
            }
            return data.OrderBy(t => t.F_CreatorTime).ToList();
        }
        public async Task<List<CheckoutDescriptionEntity>> GetAllList(string keyword = "")
        {
            var data = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(t => t.F_Description.Contains(keyword));
            }
            return data.OrderByDesc(t => t.F_ClickNum).ToList();
        }
        public async Task<List<CheckoutDescriptionEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_Description.Contains(keyword));
            }
             //权限过滤
             query = GetDataPrivilege("u", "", query);
            return query.OrderBy(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<CheckoutDescriptionEntity>> GetLookList(SoulPage<CheckoutDescriptionEntity> pagination,string keyword = "",string id="")
        {
            var query = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.F_Description.Contains(keyword));
            }
            if(!string.IsNullOrEmpty(id))
            {
                query= query.Where(u=>u.F_Id==id);
            }
            //权限过滤
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<CheckoutDescriptionEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<CheckoutDescriptionEntity> GetLookForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(data);
        }

        #region 提交数据
        public async Task SubmitForm(CheckoutDescriptionEntity entity, string keyValue)
        {
            if(string.IsNullOrEmpty(keyValue))
            {
                if (entity.F_ParentId=="0")
				{
                    entity.F_Layers = 1;
				}
				else
				{
                    var parent = await repository.FindEntity(entity.F_ParentId);
                    entity.F_Layers = parent.F_Layers+1;
                }
                entity.F_ClickNum = 0;
                entity.Create();
                await repository.Insert(entity);
            }
            else
            {
                if (entity.F_ParentId == "0")
                {
                    entity.F_Layers = 1;
                }
                else
                {
                    var parent = await repository.FindEntity(entity.F_ParentId);
                    entity.F_Layers = parent.F_Layers+1;
                }
                entity.F_Id=keyValue; 
                await repository.Update(entity);
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
			if (repository.IQueryable(a=> ids.Contains(a.F_ParentId)).Count()>0)
			{
                throw new Exception("质量问题被使用，无法删除");
			}
            await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
        }
        #endregion

    }
}
