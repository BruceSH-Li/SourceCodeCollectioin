using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.MaterialManage;

namespace WaterCloud.Service.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-17 08:30
    /// 描 述：库区管理服务类
    /// </summary>
    public class AreaService : DataFilterService<AreaEntity>, IDenpendency
    {
        private string cacheKey = "watercloud_areadata_";
        public AreaService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<AreaEntity>> GetList(string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_AreaCode.Contains(keyword) || t.F_Description.Contains(keyword)).ToList();
            }
            return cachedata.Where(t => t.F_DeleteMark == false).OrderByDescending(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<AreaEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_AreaCode.Contains(keyword) || t.F_Description.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<AreaEntity>> GetLookList(SoulPage<AreaEntity> pagination,string keyword = "",string id="")
        {
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> enabledTemp = new Dictionary<string, string>();
            enabledTemp.Add("有效", "1");
            enabledTemp.Add("无效", "0");
            dic.Add("F_EnabledMark", enabledTemp);
            Dictionary<string, string> checkoutTemp = new Dictionary<string, string>();
            checkoutTemp.Add("是", "1");
            checkoutTemp.Add("否", "0");
            dic.Add("F_NeedRule", checkoutTemp);
            pagination = ChangeSoulData(dic, pagination);
            //获取数据权限
            var query=repository.IQueryable().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_AreaCode.Contains(keyword) || t.F_Description.Contains(keyword));
            }
            if(!string.IsNullOrEmpty(id))
            {
                query = query.Where(u=>u.F_Id==id);
            }
            query = GetDataPrivilege("u","", query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<AreaEntity> GetForm(string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
               return await uniwork.FindEntity<AreaEntity>(a => a.F_AreaType == 0);
            }
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        #endregion

        public async Task<AreaEntity> GetLookForm(object keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }

        #region 提交数据
        public async Task SubmitForm(AreaEntity entity, string keyValue)
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
                entity.Modify(keyValue);
                uniwork.BeginTrans();
                await repository.Update(entity);
                if (!string.IsNullOrEmpty(entity.F_AreaCode))
                {
                    await uniwork.Update<LocationEntity>(a => a.F_AreaId == keyValue, a => new LocationEntity
                    {
                        F_AreaCode = entity.F_AreaCode
                    });

                }
                uniwork.Commit();
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            if (uniwork.IQueryable<LocationEntity>().Where(a=>ids.Contains(a.F_AreaId)).Count()>0)
            {
                throw new Exception("库区使用中无法删除");
            }
            await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
            foreach (var item in ids)
            {
            await CacheHelper.Remove(cacheKey + item);
            }
            await CacheHelper.Remove(cacheKey + "list");
        }
        #endregion

    }
}
