using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Domain.FileManage;

namespace WaterCloud.Service.SystemManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-01-14 13:23
    /// 描 述：打印模板服务类
    /// </summary>
    public class PrintTemplateService : DataFilterService<PrintTemplateEntity>, IDenpendency
    {
        public PrintTemplateService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<PrintTemplateEntity>> GetList(string keyword = "")
        {
            var data = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                data = data.Where(t => t.F_PrintTemplateName.Contains(keyword) || t.F_PrintTemplateDescription.Contains(keyword));
            }
            return data.Where(t => t.F_DeleteMark == false).OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<PrintTemplateEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_PrintTemplateName.Contains(keyword) || t.F_PrintTemplateDescription.Contains(keyword));
            }
             //权限过滤
             query = GetDataPrivilege("u", "", query);
            return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<PrintTemplateEntity>> GetLookList(SoulPage<PrintTemplateEntity> pagination,string keyword = "",string id="")
        {
            var query = repository.IQueryable().Where(u => u.F_DeleteMark==false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_PrintTemplateName.Contains(keyword) || t.F_PrintTemplateDescription.Contains(keyword));
            }
            if(!string.IsNullOrEmpty(id))
            {
                query= query.Where(u=>u.F_Id==id);
            }
            //权限过滤
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<PrintTemplateEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<PrintTemplateEntity> GetLookForm(object keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            data.list = uniwork.IQueryable<PrintTemplateHisEntity>(a => a.F_ParentId == data.F_Id).OrderByDesc(a=>a.F_CreatorTime).ToList();
            return GetFieldsFilterData(data);
        }

        #region 提交数据
        public async Task SubmitForm(PrintTemplateEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.Create();
                await repository.Insert(entity);
            }
            else
            {
                var old = await repository.FindEntity(keyValue);
                entity.Modify(keyValue);
                uniwork.BeginTrans();
                await repository.Update(entity);
                if (old != null && old.F_PrintTemplatePath != entity.F_PrintTemplatePath)
                {
                    PrintTemplateHisEntity temp = new PrintTemplateHisEntity();
                    temp.Create();
                    temp.F_ParentId = entity.F_Id;
                    temp.F_PrintTemplatePath = old.F_PrintTemplatePath;
                    await uniwork.Insert(temp);
                }
                uniwork.Commit();
            }
        }

        public async Task<UploadfileEntity> GetFileForm(string filePath)
        {
            var data = uniwork.IQueryable<UploadfileEntity>(a => a.F_FilePath == filePath).FirstOrDefault();
            return data;
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Update(t => ids.Contains(t.F_Id.ToString()), a => new PrintTemplateEntity
            {
                F_DeleteMark = true,
                F_DeleteTime = DateTime.Now,
                F_DeleteUserId = currentuser.UserId
            });
        }
        #endregion

    }
}
