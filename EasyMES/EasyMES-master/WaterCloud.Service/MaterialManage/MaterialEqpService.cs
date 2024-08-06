using System;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.MaterialManage;
using System.Collections.Generic;
using WaterCloud.Domain.EquipmentManage;

namespace WaterCloud.Service.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-01 12:23
    /// 描 述：模具物料绑定服务类
    /// </summary>
    public class MaterialEqpService : DataFilterService<MaterialEqpBandingEntity>, IDenpendency
    {

        public MaterialEqpService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<EquipmentEntity>> GetLookList(string itemId, string keyword = "")
        {
            var query = repository.IQueryable(a => a.F_MaterialId == itemId).LeftJoin<EquipmentEntity>((a, b) => a.F_EqpId == b.F_Id).Select((a, b) => new EquipmentEntity { 
             F_Id=a.F_Id,
             F_EqpState=b.F_EqpState,
             F_CompanyName=b.F_CompanyName,
             F_DeleteMark=b.F_DeleteMark,
             F_DeleteTime=b.F_DeleteTime,
             F_DeleteUserId=b.F_DeleteUserId,
             F_Description=b.F_Description,
             F_UseDate=b.F_UseDate,
             F_CreatorTime=b.F_CreatorTime,
             F_CreatorUserId=b.F_CreatorUserId,
             F_EnabledMark=b.F_EnabledMark,
             F_EqpName=b.F_EqpName,
             F_EqpOwner=b.F_EqpOwner,
             F_EqpType=b.F_EqpType,
             F_EqpUse=b.F_EqpUse,
             F_IsMaintain=b.F_IsMaintain,
             F_IsOtherEqp=b.F_IsOtherEqp,
             F_MaintainNum=b.F_MaintainNum,
             F_MaintainType=b.F_MaintainType,
             F_MoldingNum=b.F_MoldingNum,
             F_MouldType=b.F_MouldType,
             F_ParentId=b.F_ParentId,
             F_PlanUserNum=b.F_PlanUserNum,
             F_UsedNum=b.F_UsedNum,
            });
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_EqpName.Contains(keyword));
            }
            return query.ToList();

        }
        public async Task<MaterialEqpBandingEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.FindEntity(keyValue);
            return cachedata;
        }
        #endregion

        public async Task<MaterialEqpBandingEntity> GetLookForm(object keyValue)
        {
            var cachedata = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(cachedata);
        }

        #region 提交数据
        public async Task SubmitForm(MaterialEqpBandingEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_Id=Utils.GuId();
                await repository.Insert(entity);
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
