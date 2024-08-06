using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.QualityManage;
using WaterCloud.Service.SystemManage;
using WaterCloud.Domain.MaterialManage;
using System.Net.Http;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Domain.EquipmentManage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WaterCloud.Service.QualityManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-05-21 13:57
    /// 描 述：不良物料管理服务类
    /// </summary>
    public class ScapMaterialService : DataFilterService<ScapMaterialEntity>, IDenpendency
    {
        private ItemsDataService itemsApp;
        private IHttpClientFactory _httpClientFactory;
        public ScapMaterialService(IDbContext context, IHttpClientFactory httpClientFactory) : base(context)
        {
            itemsApp = new ItemsDataService(context);
            _httpClientFactory = httpClientFactory;
        }
        #region 获取数据
        public async Task<List<ScapMaterialEntity>> GetList(string keyword = "")
        {
            var data = repository.IQueryable().Where(a => a.F_IsTemp == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(t => t.F_BatchCode.Contains(keyword)
                || t.F_MaterialCode.Contains(keyword)
                || t.F_MaterialName.Contains(keyword));
            }
            return data.OrderBy(t => t.F_BatchCode).ToList();
        }
        public async Task<List<ScapMaterialEntity>> GetUnDoList(string keyword = "")
        {
            var data = repository.IQueryable().Where(a => a.F_IsTemp == false && a.F_IsCheck == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(t => t.F_BatchCode.Contains(keyword)
                || t.F_MaterialCode.Contains(keyword)
                || t.F_MaterialName.Contains(keyword));
            }
            return data.OrderBy(t => t.F_BatchCode).ToList();
        }
        public async Task<List<ScapMaterialEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable().Where(a=>a.F_IsTemp == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_BatchCode.Contains(keyword)
                || t.F_MaterialCode.Contains(keyword)
                || t.F_MaterialName.Contains(keyword));
            }
             //权限过滤
             query = GetDataPrivilege("u", "", query);
            return query.OrderBy(t => t.F_BatchCode).ToList();
        }

		public async Task<ScapMaterialEntity> GetScapMaterialInfoByCode(string code,bool? isTemp)
		{
            var data = await repository.FindEntity(a=>a.F_BatchCode==code);
			if (isTemp!=null)
			{
                data = await repository.FindEntity(a => a.F_BatchCode == code && a.F_IsTemp== isTemp);
            }
			if (data!=null)
			{
                data.list = uniwork.IQueryable<ScapImageEntity>(a => a.F_ScapId == data.F_Id).ToList();
                data.F_OtherNum = data.F_Num - data.F_ReturnNum - data.F_ScapNum;
            }
            return data;
        }

		public async Task<List<ScapMaterialEntity>> GetLookList(SoulPage<ScapMaterialEntity> pagination, int timetype, string keyword = "",long id=0)
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            var setList = await itemsApp.GetItemList("Mes_MaterialType");
            Dictionary<string, string> stateTemp = new Dictionary<string, string>();
            foreach (var item in setList)
            {
                stateTemp.Add(item.F_ItemName, item.F_ItemCode);
            }
            dic.Add("F_MaterialType", stateTemp);
            Dictionary<string, string> typeTemp = new Dictionary<string, string>();
            var typeList = await itemsApp.GetItemList("Mes_CheckType");
            foreach (var item in typeList)
            {
                typeTemp.Add(item.F_ItemName, item.F_ItemCode);
            }
            dic.Add("F_CheckType", typeTemp);
            pagination = ChangeSoulData(dic, pagination);
            DateTime startTime = DateTime.Now.ToString("yyyy-MM-dd").ToDate();
            DateTime endTime = DateTime.Now.ToString("yyyy-MM-dd").ToDate().AddDays(1);
            switch (timetype)
            {
                case 1:
                    break;
                case 2:
                    startTime = startTime.AddDays(-7);
                    break;
                case 3:
                    startTime = startTime.AddMonths(-1);
                    break;
                case 4:
                    startTime = startTime.AddMonths(-2);
                    break;
                default:
                    break;
            }
            var query = repository.IQueryable().Select(a=>new ScapMaterialEntity { 
                F_BatchCode=a.F_BatchCode,
                F_CheckId=a.F_CheckId,
                F_CheckType=a.F_CheckType,
                F_CreatorTime=a.F_CreatorTime,   
                F_CreatorUserId=a.F_CreatorUserId,
                F_CreatorUserName=a.F_CreatorUserName,
                F_Id=a.F_Id,
                F_IsCheck=a.F_IsCheck,
                F_IsTemp=a.F_IsTemp,
                F_MaterialBatch=a.F_MaterialBatch,
                F_MaterialCode=a.F_MaterialCode,
                F_MaterialId=a.F_MaterialId,
                F_MaterialName=a.F_MaterialName,
                F_MaterialType=a.F_MaterialType,  
                F_MaterialUnit=a.F_MaterialUnit,
                F_Num=a.F_Num,
                F_OtherNum=a.F_Num-a.F_ReturnNum-a.F_ScapNum,
                F_ReturnNum=a.F_ReturnNum,
                F_ScapNum=a.F_ScapNum
            }).Where(t => t.F_CreatorTime >= startTime && t.F_CreatorTime <= endTime).Where(a => a.F_IsTemp == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.F_BatchCode.Contains(keyword)
                || t.F_MaterialCode.Contains(keyword)
                || t.F_MaterialName.Contains(keyword));
            }
            if(id != 0)
            {
                query= query.Where(u=>u.F_Id==id);
            }
            //权限过滤
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<List<ScapMaterialReturnEntity>> GetLookReturnList(SoulPage<ScapMaterialReturnEntity> pagination, int timetype, string keyword = "", long id = 0)
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            var setList = await itemsApp.GetItemList("Mes_MaterialType");
            Dictionary<string, string> stateTemp = new Dictionary<string, string>();
            foreach (var item in setList)
            {
                stateTemp.Add(item.F_ItemName, item.F_ItemCode);
            }
            dic.Add("F_MaterialType", stateTemp);
            pagination = ChangeSoulData(dic, pagination);
            DateTime startTime = DateTime.Now.ToString("yyyy-MM-dd").ToDate();
            DateTime endTime = DateTime.Now.ToString("yyyy-MM-dd").ToDate().AddDays(1);
            switch (timetype)
            {
                case 1:
                    break;
                case 2:
                    startTime = startTime.AddDays(-7);
                    break;
                case 3:
                    startTime = startTime.AddMonths(-1);
                    break;
                case 4:
                    startTime = startTime.AddMonths(-2);
                    break;
                default:
                    break;
            }
            var query = uniwork.IQueryable<ScapMaterialReturnEntity>().Where(t => t.F_CreatorTime >= startTime && t.F_CreatorTime <= endTime);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.F_BatchCode.Contains(keyword)
                || t.F_MaterialCode.Contains(keyword)
                || t.F_MaterialName.Contains(keyword));
            }
            if (id != 0)
            {
                query = query.Where(u => u.F_Id == id);
            }
            //权限过滤
            query = GetDataPrivilege("u", "ScapMaterialReturn", query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<ScapMaterialEntity> GetForm(long keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            if (data != null)
            {
                data.F_OtherNum = data.F_Num - data.F_ReturnNum - data.F_ScapNum;
            }
            return data;
        }
        #endregion

        public async Task<ScapMaterialEntity> GetLookForm(long keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            if (data != null)
            {
                data.list = uniwork.IQueryable<ScapImageEntity>(a => a.F_ScapId == data.F_Id).ToList();
                data.F_OtherNum = data.F_Num - data.F_ReturnNum - data.F_ScapNum;
            }
            return GetFieldsFilterData(data);
        }

		public async Task<List<ScapMaterialEntity>> GetReturnInfoList()
		{
            var storage = uniwork.IQueryable<ScapMaterialReturnEntity>().GroupBy(a => a.F_ScapMaterialId).Select(a => new { a.F_ScapMaterialId, num = Sql.Sum(a.F_Num) });
            var data = repository.IQueryable(a=>a.F_ReturnNum>0).LeftJoin(storage,(a,b)=>a.F_Id==b.F_ScapMaterialId)
                .Select((a,b)=>new ScapMaterialEntity { 
                    F_BatchCode=a.F_BatchCode,
                    F_CheckId=a.F_CheckId,
                    F_CheckType=a.F_CheckType,
                    F_CreatorTime=a.F_CreatorTime,
                    F_CreatorUserId=a.F_CreatorUserId,
                    F_CreatorUserName=a.F_CreatorUserName,
                    F_Id=a.F_Id,
                    F_IsCheck=a.F_IsCheck,
                    F_IsTemp=a.F_IsTemp,
                    F_MaterialBatch=a.F_MaterialBatch,
                    F_MaterialCode=a.F_MaterialCode,
                    F_MaterialId=a.F_MaterialId,
                    F_MaterialName=a.F_MaterialName,
                    F_MaterialType=a.F_MaterialType,
                    F_MaterialUnit=a.F_MaterialUnit,
                    F_Num=  a.F_ReturnNum,
                    F_OtherNum= b.F_ScapMaterialId == null ? a.F_ReturnNum :(a.F_ReturnNum - b.num),
                    F_ReturnNum = b.F_ScapMaterialId == null ? 0 : b.num,
                    F_ScapNum =a.F_ScapNum,              
                }).Where(a=>a.F_OtherNum>0)
                .OrderBy(a => a.F_BatchCode).ToList();
            return data;
		}

        public async Task<List<ScapMaterialReturnEntity>> GetReturnInfoByTransfer(string code)
        {
            var data = uniwork.IQueryable<ScapMaterialReturnEntity>(a => a.F_IsProduce == true && a.F_TransferBoxCode == code).ToList();
            return data;
        }
        public async Task<ScapMaterialEntity> GetReturnScapMaterialInfoByCode(string code)
        {
            var data = await repository.FindEntity(a => a.F_BatchCode == code);
            if (data != null)
            {
                data.list = uniwork.IQueryable<ScapImageEntity>(a => a.F_ScapId == data.F_Id).ToList();
                var returnnum = uniwork.IQueryable<ScapMaterialReturnEntity>(a => a.F_ScapMaterialId == data.F_Id).Sum(a=>a.F_Num)??0;
                data.F_OtherNum = data.F_ReturnNum-returnnum;
                data.F_Num = data.F_ReturnNum;
                data.F_ReturnNum = returnnum;
            }
            return data;
        }
        #region 提交数据
        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
        }
        //提交可疑产品
        public async Task SubmitForm(ScapMaterialEntity entity)
        {
            if (entity.F_Num <= 0)
            {
                throw new Exception("数量不能小于等于0");
            }
            var check = await uniwork.FindEntity<NeedCheckEntity>(entity.F_CheckId);
            entity.F_CheckType = check.F_CheckType;
            entity.F_Id = 0;
            entity.F_CreatorTime = DateTime.Now;
            entity.F_CreatorUserId = currentuser.UserId;
            entity.F_CreatorUserName = currentuser.UserName;
            entity.F_IsCheck = false;
            entity.F_IsTemp = true;
            entity.F_MaterialBatch = check.F_MaterialBatch;
            entity.F_MaterialCode = check.F_MaterialCode;
            entity.F_MaterialId = check.F_MaterialId;
            entity.F_MaterialName = check.F_MaterialName;
            entity.F_MaterialType = check.F_MaterialType;
            entity.F_MaterialUnit = check.F_MaterialUnit;
            entity.F_ScapNum = 0;
            entity.F_ReturnNum = 0;
            if (check.F_Num - entity.F_Num < repository.IQueryable(a => a.F_CheckId == check.F_Id).Sum(a=>a.F_Num))
            {
                throw new Exception("可疑数量已超出质检数量");
            }
            uniwork.BeginTrans();
            entity=await repository.Insert(entity);
			foreach (var item in entity.list)
			{
                item.F_ScapId = entity.F_Id;
                var count = int.Parse(entity.F_Num.ToString());
                await uniwork.Update<CheckoutDescriptionEntity>(a => a.F_Id == item.F_DescriptionId, a => new CheckoutDescriptionEntity
                {
                    F_ClickNum = a.F_ClickNum + count,
                });
            }
            await uniwork.Insert(entity.list);
            uniwork.Commit();
        }

        public async Task CancleForm(string batchCode)
        {
            var scap = await uniwork.FindEntity<ScapMaterialEntity>(a=>a.F_BatchCode==batchCode);
            var check = await uniwork.FindEntity<NeedCheckEntity>(scap.F_CheckId);
			if (check.F_IsCheck==true)
			{
                throw new Exception("检验已完成，无法取消可疑");
			}
            uniwork.BeginTrans();
            await uniwork.Delete<ScapMaterialEntity>(a => a.F_Id == scap.F_Id);
            await uniwork.Delete<ScapImageEntity>(a => a.F_ScapId == scap.F_Id);
            uniwork.Commit();
        }
        public async Task ReturnForm(string batchCode,string transferBoxCode, float? num = 0,bool isProduce=false)
        {
            var scap = await uniwork.FindEntity<ScapMaterialEntity>(a => a.F_BatchCode == batchCode);
            var donenum = uniwork.IQueryable<ScapMaterialReturnEntity>(a => a.F_ScapMaterialId == scap.F_Id).Sum(a => a.F_Num)??0;
			if (scap.F_ReturnNum< num + donenum)
			{
                throw new Exception("退回数量不足，请检查");
            }
            var transfer = await uniwork.FindEntity<TransferBoxEntity>(a => a.F_TransferCode == transferBoxCode);
			if (transfer == null||transfer.F_TransferState>2)
			{
                throw new Exception($"流转箱{transferBoxCode}不可用");
            }
            ScapMaterialReturnEntity entity = new ScapMaterialReturnEntity();
            entity.F_Id = 0;
            entity.F_CreatorTime = DateTime.Now;
            entity.F_TransferBoxCode = transferBoxCode;
            entity.F_BatchCode = batchCode;
            entity.F_CreatorUserId = currentuser.UserId;
            entity.F_CreatorUserName = currentuser.UserName;
            entity.F_MaterialBatch = scap.F_MaterialBatch;
            entity.F_ScapMaterialId = scap.F_Id;
            entity.F_MaterialCode = scap.F_MaterialCode;
            entity.F_MaterialId = scap.F_MaterialId;
            entity.F_MaterialName = scap.F_MaterialName;
            entity.F_MaterialType = scap.F_MaterialType;
            entity.F_MaterialUnit = scap.F_MaterialUnit;
            entity.F_Num = num;
            entity.F_BackMaterialBatch = entity.F_MaterialBatch;
            entity.F_IsProduce = isProduce;
            uniwork.BeginTrans();
            StorageEntity storage = new StorageEntity();
            if (transfer.F_TransferState == 2)
			{
                storage = await uniwork.FindEntity<StorageEntity>(a => a.F_TransferBoxCode == transferBoxCode && a.F_MaterialId == scap.F_MaterialId);
                if (storage == null)
                {
                    throw new Exception($"流转箱{transferBoxCode}不可用");
                }
				if (transfer.F_TransferMaxNum<storage.F_Num+ num)
				{
                    throw new Exception($"流转箱{transferBoxCode}不可用");
                }
                await uniwork.Update<StorageEntity>(a => a.F_Id == storage.F_Id, a => new StorageEntity { 
                    F_Num=a.F_Num+num
                });
                entity.F_BackMaterialBatch = storage.F_MaterialBatch;
            }
			else
			{
                if (entity.F_IsProduce == true)
                {
                    await uniwork.Update<TransferBoxEntity>(a => a.F_Id == transfer.F_Id, a => new TransferBoxEntity
                    {
                        F_TransferState = 1
                    });
                }
                else
                {
                    storage.Create();
                    storage.F_MaterialName = scap.F_MaterialName;
                    storage.F_IsCheckout = true;
                    storage.F_MaterialCode = scap.F_MaterialCode;
                    storage.F_MaterialBatch = scap.F_MaterialBatch;
                    storage.F_EnabledMark = true;
                    storage.F_MaterialType = scap.F_MaterialType;
                    storage.F_MaterialUnit = scap.F_MaterialUnit;
                    storage.F_MaterialId = scap.F_MaterialId;
                    storage.F_TransferBoxCode = transferBoxCode;
                    storage.F_Num = num;
                    //todo 分配库位
                    AreaEntity area;
                    var old = uniwork.IQueryable<InStorageInfoEntity>(a => a.F_MaterialId == storage.F_MaterialId).Select(a => a.F_LocationCode).FirstOrDefault();
                    if (string.IsNullOrEmpty(old))
                    {
                        old = uniwork.IQueryable<OutPutInfoEntity>(a => a.F_MaterialId == storage.F_MaterialId && a.F_IsTemp == false).Select(a => a.F_LocationCode).FirstOrDefault();
                        if (string.IsNullOrEmpty(old))
                        {
                            area = uniwork.IQueryable<AreaEntity>(a => a.F_AreaType == 0).FirstOrDefault();
                        }
                        else
                        {
                            area = uniwork.IQueryable<LocationEntity>(a => a.F_LocationCode == old).InnerJoin<AreaEntity>((a, b) => a.F_AreaId == b.F_Id).Select((a, b) => b).FirstOrDefault();
                        }
                    }
                    else
                    {
                        var locationold = await uniwork.FindEntity<LocationEntity>(a => a.F_LocationCode == old);
                        if (locationold == null)
                        {
                            if (entity.F_MaterialType==0)
                            {
                                area = await uniwork.FindEntity<AreaEntity>(a => a.F_AreaType == 0);
                            }
                            else
                            {
                                area = uniwork.IQueryable<BomFormEntity>(a => a.F_MaterialId == entity.F_MaterialId)
                                   .InnerJoin<ProcessEqpBandingEntity>((a, b) => a.F_ProcessId == b.F_ProcessId)
                                   .InnerJoin<EqpAreaBandingEntity>((a, b, c) => b.F_EqpId == c.F_EqpId)
                                   .InnerJoin<AreaEntity>((a, b, c, d) => c.F_AreaId == d.F_Id).Select((a, b, c, d) => d).FirstOrDefault();
                            }
                        }
                        else
                        {
                            area = uniwork.IQueryable<LocationEntity>(a => a.F_LocationCode == old).InnerJoin<AreaEntity>((a, b) => a.F_AreaId == b.F_Id).Select((a, b) => b).FirstOrDefault();
                        }
                    }

                    var location = uniwork.IQueryable<LocationEntity>(a => a.F_AreaId == area.F_Id && a.F_LocationState == false).OrderBy("F_SortCode asc,F_LocationCode asc").FirstOrDefault();
                    storage.F_LocationCode = location.F_LocationCode;
                    //更新库位容器状态
                    await uniwork.Update<LocationEntity>(a => a.F_Id == location.F_Id, a => new LocationEntity
                    {
                        F_LocationState = true
                    });
                    await uniwork.Update<TransferBoxEntity>(a => a.F_Id == transfer.F_Id, a => new TransferBoxEntity
                    {
                        F_TransferState = 2,
                        F_LocationCode = storage.F_LocationCode
                    });
                    await uniwork.Insert(storage);
                }     
            }
            await uniwork.Insert(entity);
            uniwork.Commit();
        }
        #endregion

    }
}
