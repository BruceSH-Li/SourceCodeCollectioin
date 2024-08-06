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
    /// 日 期：2020-12-14 12:44
    /// 描 述：流转箱类型绑定服务类
    /// </summary>
    public class TransferTypeService : DataFilterService<TransferTypeMaterialBandingEntity>, IDenpendency
    {
        public TransferTypeService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<MaterialEntity>> GetList(string ItemId, string keyword)
        {
            var query = repository.IQueryable().Where(a => a.F_TransferType == ItemId).InnerJoin<MaterialEntity>((a,b)=>a.F_MaterialId==b.F_Id).Select((a,b)=>b);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_MaterialCode.Contains(keyword) || u.F_MaterialName.Contains(keyword));
            }
            return query.ToList();
        }
        #endregion

        #region 提交数据
        public async Task SubmitForm(string ItemId, string keyValue)
        {
            List<TransferTypeMaterialBandingEntity> temps = new List<TransferTypeMaterialBandingEntity>();
            if (string.IsNullOrEmpty(keyValue))
            {
                return;
            }
            else
            {
                var list = keyValue.Split(',');
                foreach (var item in list)
                {
                    TransferTypeMaterialBandingEntity entity = new TransferTypeMaterialBandingEntity();
                    entity.F_Id = Utils.GuId();
                    entity.F_TransferType = ItemId;
                    entity.F_MaterialId = item;
                    temps.Add(entity);
                }
            }
            await repository.Insert(temps);
        }

        public async Task DeleteForm(string ItemId, string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Delete(t =>t.F_TransferType==ItemId&& ids.Contains(t.F_MaterialId.ToString()));
        }

        public async Task<List<MaterialEntity>> GetMaterialList(Pagination pagination, string itemId, string keyword)
        {
            var unList = repository.IQueryable(a => a.F_TransferType == itemId).Select(a => a.F_MaterialId).ToList();
            var query = uniwork.IQueryable<MaterialEntity>().Where(a => !unList.Contains(a.F_Id) && a.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_MaterialCode.Contains(keyword) || u.F_MaterialName.Contains(keyword));
            }
            return await repository.OrderList(query, pagination);
        }

        #endregion

    }
}
