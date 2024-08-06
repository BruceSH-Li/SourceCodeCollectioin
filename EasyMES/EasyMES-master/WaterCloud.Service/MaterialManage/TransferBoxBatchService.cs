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
    /// 日 期：2021-06-17 14:19
    /// 描 述：流转箱打印服务类
    /// </summary>
    public class TransferBoxBatchService : DataFilterService<TransferBoxBatchEntity>, IDenpendency
    {
        public TransferBoxBatchService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<TransferBoxBatchEntity>> GetList(string keyword = "")
        {
            var data = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(t => t.F_BatchName.Contains(keyword));
            }
            return data.OrderBy(t => t.F_Id).ToList();
        }

        public async Task<List<TransferBoxBatchEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_BatchName.Contains(keyword));
            }
             //权限过滤
             query = GetDataPrivilege("u", "", query);
            return query.OrderBy(t => t.F_Id).ToList();
        }

        public async Task<List<TransferBoxBatchEntity>> GetLookList(SoulPage<TransferBoxBatchEntity> pagination,string keyword = "",string id="")
        {
            var query = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.F_BatchName.Contains(keyword));
            }
            if(!string.IsNullOrEmpty(id))
            {
                query= query.Where(u=>u.F_Id==id);
            }
            //权限过滤
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<TransferBoxBatchEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<TransferBoxBatchEntity> GetLookForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(data);
        }

        #region 提交数据
        public async Task SubmitForm(TransferBoxBatchEntity entity, string keyValue)
        {
            if(string.IsNullOrEmpty(keyValue))
            {
                //初始值添加
                entity.F_Id = Utils.GuId() ;
                entity.F_BatchNum = 1;
                await repository.Insert(entity);
            }
            else
            {
                //此处需修改
                entity.F_Id = keyValue; 
                await repository.Update(entity);
            }
        }

		public async Task<List<string>> PrinterForm(string keyValue, long num)
		{
            var data = await repository.FindEntity(keyValue);
            List<TransferBoxEntity> list = new List<TransferBoxEntity>();
            List<string> strs = new List<string>();
            for (int i = 0; i < num; i++)
			{
                TransferBoxEntity entity = new TransferBoxEntity();
                entity.Create();
                entity.F_IsTemp = true;
                entity.F_TransferMaxNum = data.F_MaxNum;
                entity.F_TransferState = 0;
                entity.F_TransferType = "0";
                entity.F_DeleteMark = false;
                entity.F_EnabledMark = true;
                entity.F_TransferCode = data.F_BatchCode;
                var batchNum =data.F_BatchNum+ i;
                if (batchNum > 99999)
				{
                    entity.F_TransferCode = data.F_BatchCode + batchNum;
                }
				else if(batchNum > 9999)
				{
                    entity.F_TransferCode = data.F_BatchCode +"0"+ batchNum;
                }
                else if (batchNum > 999)
                {
                    entity.F_TransferCode = data.F_BatchCode + "00" + batchNum;
                }
                else if (batchNum > 99)
                {
                    entity.F_TransferCode = data.F_BatchCode + "000" + batchNum;
                }
                else if (batchNum > 9)
                {
                    entity.F_TransferCode = data.F_BatchCode + "0000" + batchNum;
                }
                else
                {
                    entity.F_TransferCode = data.F_BatchCode + "00000" + batchNum;
                }
                list.Add(entity);
                strs.Add(entity.F_TransferCode);
            }
            uniwork.BeginTrans();
            if (data.F_IsCheck != true)
            {
                await uniwork.Insert(list);
            }
            await repository.Update(a => a.F_Id == keyValue, a => new TransferBoxBatchEntity
            {
                F_BatchNum = a.F_BatchNum + num

            });
            uniwork.Commit();
            return strs;
        }

        public async Task<TransferBoxBatchEntity> GetCheckForm()
        {
            var data = repository.IQueryable(a => a.F_IsCheck == true).OrderByDesc(a => a.F_Id).FirstOrDefault();
            return data;
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
        }
        #endregion

    }
}
