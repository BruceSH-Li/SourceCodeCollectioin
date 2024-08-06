using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.DingTalkManage;

namespace WaterCloud.Service.DingTalkManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-11-21 09:52
    /// 描 述：应用配置服务类
    /// </summary>
    public class DingTalkConfigService : DataFilterService<DingTalkConfigEntity>, IDenpendency
    {
        private string cacheKey = "watercloud_dingtalkconfigdata_";

        public DingTalkConfigService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<DingTalkConfigEntity>> GetList(string keyword = "")
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_AppName.Contains(keyword) || t.F_Description.Contains(keyword)).ToList();
            }
            return cachedata.Where(t => t.F_DeleteMark == false).OrderByDescending(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<DingTalkConfigEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_AppName.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return query.OrderBy(t => t.F_CreatorTime).ToList();
        }
        public async Task<List<DingTalkConfigEntity>> GetLookList(SoulPage<DingTalkConfigEntity> pagination,string keyword = "")
        {
            //获取数据权限
            var query = repository.IQueryable().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_AppName.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<DingTalkConfigEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        public async Task<DingTalkConfigEntity> GetLoginForm()
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            cachedata = cachedata.Where(t => t.F_EnabledMark == true && t.F_DeleteMark == false && t.F_IsWebLogin == true).ToList();
            return cachedata.FirstOrDefault();
        }

        public async Task<DingTalkConfigEntity> GetBoardLoginForm()
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            cachedata = cachedata.Where(t => t.F_EnabledMark == true && t.F_DeleteMark == false && t.F_IsBoardLogin == true).ToList();
            return cachedata.FirstOrDefault();
        }
        public async Task<DingTalkConfigEntity> GetLoginDingApp()
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            cachedata = cachedata.Where(t => t.F_EnabledMark == true && t.F_DeleteMark == false && t.F_DingType == 0).ToList();
            return cachedata.FirstOrDefault();
        }

        public async Task<DingTalkConfigEntity> GetAppletForm(bool isWork = false)
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            cachedata = cachedata.Where(t => t.F_EnabledMark == true && t.F_DeleteMark == false && t.F_IsDingInfo == true).ToList();
            if (isWork)
            {
                return cachedata.Where(a => a.F_DingType == 0).FirstOrDefault();
            }
            else
            {
                return cachedata.Where(a => a.F_DingType == 2).FirstOrDefault();
            }
        }
        public async Task<DingTalkConfigEntity> GetLookForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return GetFieldsFilterData(cachedata);
        }
        #endregion


        #region 提交数据
        public async Task SubmitForm(DingTalkConfigEntity entity, string keyValue)
        {
            //三方web可以设置扫码登录
            if (entity.F_DingType != 1)
            {
                entity.F_IsWebLogin = false;
                entity.F_IsBoardLogin = false;
            }
            else
            {
                //h5移动端或小程序设置调用钉钉
                entity.F_IsDingInfo = false;
            }
            uniwork.BeginTrans();
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.Create();
                //扫码登录和钉钉调用都只存在一个
                if (entity.F_DingType == 1 && entity.F_IsWebLogin == true)
                {
                    var temp = repository.IQueryable().Where(a => a.F_IsWebLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).FirstOrDefault();
                    if (temp != null)
                    {
                        await repository.Update(a => a.F_Id == temp.F_Id, a => new DingTalkConfigEntity
                        {
                            F_IsWebLogin = false,
                        });
                    } 
                }
                else if (entity.F_DingType!=1 && entity.F_IsDingInfo == true)
                {
                    var temp = repository.IQueryable().Where(a => a.F_IsDingInfo == true && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_DingType == entity.F_DingType).FirstOrDefault();
                    if (temp != null)
                    {
                        await repository.Update(a => a.F_Id == temp.F_Id && a.F_DingType == entity.F_DingType, a => new DingTalkConfigEntity
                        {
                            F_IsDingInfo = false,
                        });
                    }
                }
                if (entity.F_DingType == 1 && entity.F_IsBoardLogin == true)
                {
                    var temp = repository.IQueryable().Where(a => a.F_IsBoardLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).FirstOrDefault();
                    if (temp != null)
                    {
                        await repository.Update(a => a.F_Id == temp.F_Id, a => new DingTalkConfigEntity
                        {
                            F_IsBoardLogin = false,
                        });
                    }
                }
                await repository.Insert(entity);
                if (repository.IQueryable().Where(a => a.F_IsWebLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).Count()==0)
                {
                    throw new Exception("请保证有一个扫码登录的钉钉应用");
                }
                if (repository.IQueryable().Where(a => a.F_IsBoardLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).Count() == 0)
                {
                    throw new Exception("请保证有一个看板扫码登录的钉钉应用");
                }
                uniwork.Commit();
            }
            else
            {
                entity.Modify(keyValue);
                if (entity.F_DingType == 1 && entity.F_IsWebLogin == true)
                {
                    var temp = repository.IQueryable().Where(a => a.F_IsWebLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).FirstOrDefault();
                    if (temp != null && temp.F_Id != entity.F_Id)
                    {
                        await repository.Update(a => a.F_Id == temp.F_Id, a => new DingTalkConfigEntity
                        {
                            F_IsWebLogin = false,
                        });
                    }
                }
                else if (entity.F_DingType !=1 && entity.F_IsDingInfo == true)
                {
                    var temp = repository.IQueryable().Where(a => a.F_IsDingInfo == true && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_DingType == entity.F_DingType).FirstOrDefault();
                    if (temp != null && temp.F_Id != entity.F_Id)
                    {
                        await repository.Update(a => a.F_Id == temp.F_Id && a.F_DingType == entity.F_DingType, a => new DingTalkConfigEntity
                        {
                            F_IsDingInfo = false,
                        });
                    }
                }
                if (entity.F_DingType == 1 && entity.F_IsBoardLogin == true)
                {
                    var temp = repository.IQueryable().Where(a => a.F_IsBoardLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).FirstOrDefault();
                    if (temp != null)
                    {
                        await repository.Update(a => a.F_Id == temp.F_Id, a => new DingTalkConfigEntity
                        {
                            F_IsBoardLogin = false,
                        });
                    }
                }
                await repository.Update(entity);
                if (repository.IQueryable().Where(a => a.F_IsWebLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).Count() == 0)
                {
                    throw new Exception("请保证有一个扫码登录的钉钉应用");
                }
                if (repository.IQueryable().Where(a => a.F_IsDingInfo == true && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_DingType == 2).Count() == 0)
                {
                    throw new Exception("请保证有一个钉钉调用的钉钉应用");
                }
                if (repository.IQueryable().Where(a => a.F_IsBoardLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).Count() == 0)
                {
                    throw new Exception("请保证有一个看板扫码登录的钉钉应用");
                }
                uniwork.Commit();
                await CacheHelper.Remove(cacheKey + keyValue);
            }
            await CacheHelper.Remove(cacheKey + "list");
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            uniwork.BeginTrans();
            await repository.Delete(t => ids.Contains(t.F_Id));
            if (repository.IQueryable().Where(a => a.F_IsWebLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).Count() == 0)
            {
                throw new Exception("请保证有一个扫码登录的钉钉应用");
            }
            if (repository.IQueryable().Where(a => a.F_IsDingInfo == true && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_DingType == 2).Count() == 0)
            {
                throw new Exception("请保证有一个钉钉调用的钉钉应用");
            }
            if (repository.IQueryable().Where(a => a.F_IsBoardLogin == true && a.F_EnabledMark == true && a.F_DeleteMark == false).Count() == 0)
            {
                throw new Exception("请保证有一个看板扫码登录的钉钉应用");
            }
            uniwork.Commit();
            foreach (var item in ids)
            {
            await CacheHelper.Remove(cacheKey + item);
            }
            await CacheHelper.Remove(cacheKey + "list");
        }
        #endregion

    }
}
