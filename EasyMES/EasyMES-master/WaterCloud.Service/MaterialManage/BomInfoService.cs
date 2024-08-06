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
    /// 日 期：2020-12-03 13:55
    /// 描 述：Bom结构服务类
    /// </summary>
    public class BomInfoService : DataFilterService<BomFormEntity>, IDenpendency
    {
        public BomInfoService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<MaterialEntity>> GetLookList(string itemId, string keyword = "")
        {
            var query = repository.IQueryable(a => a.F_MaterialId == itemId && a.F_BomType == 1).LeftJoin<MaterialEntity>((a, b) => a.F_SonMaterialId == b.F_Id).Select((a, b) => new MaterialEntity
            {
                F_Id = a.F_Id,
                F_Num = a.F_Num,
                F_CreatorTime = b.F_CreatorTime,
                F_CreatorUserId = b.F_CreatorUserId,
                F_Customer = b.F_Customer,
                F_CustomerProNO = b.F_CustomerProNO,
                F_DeleteMark = b.F_DeleteMark,
                F_DeleteTime = b.F_DeleteTime,
                F_DeleteUserId = b.F_DeleteUserId,
                F_Description = a.F_Description,
                F_EnabledMark = b.F_EnabledMark,
                F_LastModifyTime = b.F_LastModifyTime,
                F_LastModifyUserId = b.F_LastModifyUserId,
                F_MaterialBy = b.F_MaterialBy,
                F_MaterialCode = b.F_MaterialCode,
                F_MaterialDescription = b.F_MaterialDescription,
                F_MaterialName = b.F_MaterialName,
                F_MaterialPic = b.F_MaterialPic,
                F_MaterialSize = b.F_MaterialSize,
                F_MaterialType = b.F_MaterialType,
                F_MaterialUnit = b.F_MaterialUnit,
                F_ProcessId = a.F_ProcessId,
                F_SonMaterialId = b.F_Id,
            });
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_MaterialCode.Contains(keyword) || a.F_MaterialName.Contains(keyword));
            }
            return query.ToList();
        }

		#endregion

		public async Task<MaterialEntity> GetLookForm(string itemId)
        {
            var cachedata = await uniwork.FindEntity<MaterialEntity>(itemId);
            cachedata.listData = (await GetLookList(itemId)).ToJson();
            return cachedata;
        }

        #region 提交数据
        public async Task SubmitForm(BomFormEntity entity, string listData)
        {
            uniwork.BeginTrans();
            if (await uniwork.IQueryable<MaterialEntity>(a => a.F_Id == entity.F_MaterialId && a.F_MaterialType == 0).AnyAsync())
                throw new Exception("原料不能有bom构成");
            if (!string.IsNullOrEmpty(listData))
            {
                var list = listData.ToList<BomFormEntity>();
                if (list.Any(a=>a.F_SonMaterialId == a.F_MaterialId))
                    throw new Exception("bom构物料异常");
                foreach (var item in list)
                {
                    item.Create();
                    item.F_MaterialId = entity.F_MaterialId;
                    item.F_ProcessId = entity.F_ProcessId;
                    item.F_DeleteMark = false;
                    item.F_BomType = 1;
                }
                await repository.Delete(a => a.F_BomType == 1 && a.F_MaterialId == entity.F_MaterialId && a.F_ProcessId == entity.F_ProcessId);
                await repository.Insert(list);
            }
            uniwork.Commit();
        }
        public async Task DeleteForm(string itemId, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
                await repository.Delete(a => a.F_BomType == 1 && a.F_MaterialId == itemId);
            await repository.Delete(a => a.F_BomType == 1 && a.F_MaterialId == itemId && a.F_ProcessId == keyValue);
        }
        #endregion

    }
}
