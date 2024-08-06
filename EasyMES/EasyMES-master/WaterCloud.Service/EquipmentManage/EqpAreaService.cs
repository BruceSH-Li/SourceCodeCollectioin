using System;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using Chloe;
using System.Collections.Generic;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.MaterialManage;

namespace WaterCloud.Service.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-01 12:23
    /// 描 述：模具物料绑定服务类
    /// </summary>
    public class EqpAreaService : DataFilterService<EqpAreaBandingEntity>, IDenpendency
    {

        public EqpAreaService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<AreaEntity>> GetLookList(string itemId, string keyword = "")
        {
            var query = repository.IQueryable(a => a.F_EqpId == itemId).LeftJoin<AreaEntity>((a, b) => a.F_AreaId == b.F_Id).Select((a, b) => new AreaEntity
            {
                F_Id = a.F_Id,
                F_AreaCode = b.F_AreaCode,
                F_AreaType = b.F_AreaType,
                F_DeleteMark = b.F_DeleteMark,
                F_DeleteTime = b.F_DeleteTime,
                F_DeleteUserId = b.F_DeleteUserId,
                F_Description = b.F_Description,
                F_CreatorTime = b.F_CreatorTime,
                F_CreatorUserId = b.F_CreatorUserId,
                F_EnabledMark = b.F_EnabledMark,
            });
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_AreaCode.Contains(keyword));
            }
            return query.ToList();
        }

        public async Task<List<AreaEntity>> GetAddList(string itemId, string keyword)
        {
            var unlist = repository.IQueryable(a => a.F_EqpId == itemId).LeftJoin<AreaEntity>((a, b) => a.F_AreaId == b.F_Id).Select((a, b) => b.F_Id).ToList();
            var query = uniwork.IQueryable<AreaEntity>(a => !unlist.Contains(a.F_Id));
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.F_AreaCode.Contains(keyword));
            }
            return query.ToList();
        }

        public async Task<EquipmentEntity> GetLookForm(object keyValue)
        {
            var data = await uniwork.FindEntity<EquipmentEntity>(keyValue);
            return data;
        }
        #endregion


        #region 提交数据
        public async Task SubmitForm(EqpAreaBandingEntity entity, string keyValue)
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
