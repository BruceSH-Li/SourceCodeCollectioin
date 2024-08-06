using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.EquipmentManage;

namespace WaterCloud.Service.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-08 12:32
    /// 描 述：wdd服务类
    /// </summary>
    public class EqpPqcCheckItemService : DataFilterService<EqpPqcCheckItemEntity>, IDenpendency
    {
        public EqpPqcCheckItemService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        //public async Task<List<EqpPqcCheckItemEntity>> GetList(string keyword = "")
        //{
        //    var data = repository.IQueryable();
        //    if (!string.IsNullOrEmpty(keyword))
        //    {
        //        data = data.Where(t => t.F_EnCode.Contains(keyword)
        //        || t.F_FullName.Contains(keyword));
        //    }
        //    return data.Where(t => t.F_DeleteMark == false).OrderByDesc(t => t.F_CreatorTime).ToList();
        //}

        //public async Task<List<EqpPqcCheckItemEntity>> GetLookList(string keyword = "")
        //{
        //    var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
        //    if (!string.IsNullOrEmpty(keyword))
        //    {
        //        //此处需修改
        //        query = query.Where(t => t.F_EnCode.Contains(keyword)
        //        || t.F_FullName.Contains(keyword));
        //    }
        //    //权限过滤
        //    query = GetDataPrivilege("u", "", query);
        //    return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        //}

        public async Task<List<EqpPqcCheckItemEntity>> GetLookList(string EqpId,SoulPage<EqpPqcCheckItemEntity> pagination, string keyword = "", string id = "")
        {
            var query = repository.IQueryable().Where(t =>t.F_EqpId==EqpId&&t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.F_CheckItem.Contains(keyword)
                || t.F_CheckStand.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(u => u.F_Id == id);
            }
            //权限过滤
            query = GetDataPrivilege("u", "", query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<EqpPqcCheckItemEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<EqpPqcCheckItemEntity> GetLookForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(data);
        }

        #region 提交数据
        public async Task SubmitForm(EqpPqcCheckItemEntity entity, string keyValue)
        {
            string month = DateTime.Now.Month.ToString();

            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.Create();               
                await repository.Insert(entity);


                //var query = uniwork.IQueryable<EqpPqcMasterEntity>(t => t.F_EqpId == entity.F_EqpId && t.F_CheckMonth == month).ToList();
                //if (query != null && query.Count > 0)
                //{
                //    foreach (var queryitem in query)
                //    {
                //        EqpPqcDetailEntity detailEntity = new EqpPqcDetailEntity();
                //        detailEntity.F_Id = Utils.GuId();
                //        detailEntity.F_PId = queryitem.F_Id;
                //        detailEntity.F_CheckItemId = entity.F_Id;
                //        await uniwork.Insert<EqpPqcDetailEntity>(detailEntity);
                //    }
                //}
                //else
                //{
                //    EqpPqcMasterEntity masterEntity1 = new EqpPqcMasterEntity();
                //    masterEntity1.F_Id= Utils.GuId();
                //}


            }
            else
            {
                entity.Modify(keyValue);
                await repository.Update(entity);
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
        }

        public async Task SubmitCloneForm(string ids, string eqpId)
        {
            var eqp = uniwork.IQueryable<EquipmentEntity>(t => t.F_Id == eqpId);
            if (eqp == null) throw new Exception("不存在此设备");

            string[] ArrayId = ids.Split(',');
            //List<EqpPqcCheckItemEntity> list = new List<EqpPqcCheckItemEntity>();
            var sourceData = repository.IQueryable(t => ArrayId.Contains(t.F_Id)).OrderBy(t => t.F_Sequence).ToList();
            
            foreach (var item in sourceData)
            {
                item.F_Id = Utils.GuId();
                item.F_EqpId = eqpId;
                //list.Add(item);
            }
            await repository.Insert(sourceData);

        }
        #endregion

    }
}
