using System;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.MaterialManage;
using System.Collections.Generic;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.ProcessManage;

namespace WaterCloud.Service.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-01 12:23
    /// 描 述：模具物料绑定服务类
    /// </summary>
    public class MaterialBomService : DataFilterService<BomFormEntity>, IDenpendency
    {

        public MaterialBomService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<MaterialEntity>> GetLookList(string keyValue, string itemId)
        {
            var query = repository.IQueryable(a => a.F_MaterialId == keyValue && a.F_BomType == 1 && a.F_ProcessId==itemId).LeftJoin<MaterialEntity>((a, b) => a.F_SonMaterialId== b.F_Id).Select((a, b) => new MaterialEntity { 
                F_Id=b.F_Id,
                F_MaterialBy=b.F_MaterialBy,
                F_Customer=b.F_Customer,
                F_CustomerProNO=b.F_CustomerProNO,
                F_MaterialCode=b.F_MaterialCode,
                F_CpkStData=b.F_CpkStData,
                F_EnabledMark=b.F_EnabledMark,
                F_MaterialName=b.F_MaterialName,
                F_MaterialDescription=b.F_MaterialDescription,
                F_MaterialPic=b.F_MaterialPic,
                F_MaterialSize=b.F_MaterialSize,
                F_MaterialType=b.F_MaterialType,
                F_MaterialUnit=b.F_MaterialUnit,
                F_Description=b.F_Description,
                F_DeleteMark=b.F_DeleteMark,
                F_Num=a.F_Num,
                F_ContainerNum=b.F_ContainerNum,
                F_ModelRefer=b.F_ModelRefer,
                F_ReceiveNum=b.F_ReceiveNum,
                F_NeedCheckout=b.F_NeedCheckout, 
                F_ProcessId=a.F_ProcessId
            });
            return query.ToList();

        }
        #endregion

        public async Task<MaterialEntity> GetLookForm(string keyValue,string itemId)
        {
            var cachedata = await uniwork.FindEntity<MaterialEntity>(keyValue);
            cachedata.listData = (await GetLookList(keyValue, itemId)).ToJson();
            return cachedata;
        }

		public async Task<List<MaterialEntity>> GetBomList(string keyValue, string itemId)
		{
            var query = repository.IQueryable(a => a.F_MaterialId == keyValue && a.F_BomType == 1 && a.F_ProcessId == itemId).LeftJoin<MaterialEntity>((a, b) => a.F_SonMaterialId == b.F_Id).Select((a, b) => b);
            return await query.ToListAsync();
        }

        #region 提交数据
        public async Task SubmitForm(BomFormEntity entity, string listData)
        {
            uniwork.BeginTrans();
            await repository.Delete(a => a.F_BomType == 1 && a.F_MaterialId == entity.F_MaterialId && a.F_ProcessId==entity.F_ProcessId);
            if (!string.IsNullOrEmpty(listData))
            {
                var list = listData.ToList<BomFormEntity>();
                foreach (var item in list)
                {
                    item.Create();
                    item.F_MaterialId = entity.F_MaterialId;
                    item.F_ProcessId = entity.F_ProcessId;
                    item.F_DeleteMark = false;
                    item.F_BomType = 1;
                }
                await repository.Insert(list);
            }
            uniwork.Commit();
        }
        #endregion

    }
}
