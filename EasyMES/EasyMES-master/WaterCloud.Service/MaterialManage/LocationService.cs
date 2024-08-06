using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.ProcessManage;
using System.IO;

namespace WaterCloud.Service.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-17 08:31
    /// 描 述：库位管理服务类
    /// </summary>
    public class LocationService : DataFilterService<LocationEntity>, IDenpendency
    {
        public LocationService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<LocationEntity>> GetList(string keyword = "")
        {
            var cachedata = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                cachedata = cachedata.Where(t => t.F_AreaCode.Contains(keyword) || t.F_LocationCode.Contains(keyword));
            }
            return cachedata.Where(t => t.F_DeleteMark == false).OrderBy(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<LocationEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_AreaCode.Contains(keyword) || u.F_LocationCode.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<LocationEntity>> GetLookList(string itemId,SoulPage<LocationEntity> pagination,string keyword = "",string id="")
        {
            //获取数据权限
            var query = repository.IQueryable().Where(u => u.F_AreaId == itemId && u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_AreaCode.Contains(keyword) || u.F_LocationCode.Contains(keyword));
            }
            if(!string.IsNullOrEmpty(id))
            {
                query= query.Where(u=>u.F_Id==id);
            }
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<LocationEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.FindEntity(keyValue);
            return cachedata;
        }

        public async Task<LocationEntity> GetLookForm(object keyValue)
        {
            var cachedata = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(cachedata);
        }
        /// <summary>
        /// 判断库位是否可用
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        public bool IsUseLocation(string locationCode,string materialCode)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(locationCode))
            {
                var location = uniwork.IQueryable<LocationEntity>(a => a.F_LocationCode == locationCode && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_LocationState == false).FirstOrDefault();
                if (location != null)
                {
                    var area = uniwork.IQueryable<AreaEntity>(a => a.F_Id == location.F_AreaId).FirstOrDefault();
                    //先用绑定库位，然后使用非绑定库位
                    if (area.F_StackType != true)
                    {
                        var last= uniwork.IQueryable<LocationMaterialBandingEntity>(a=>a.F_MaterialCode==materialCode).InnerJoin<LocationEntity>((a,b) => b.F_AreaId == area.F_Id && b.F_EnabledMark == true && b.F_DeleteMark == false && b.F_LocationState == false && a.F_LocationId==b.F_Id).Select((a,b)=>b).OrderBy(a=>a.F_SortCode).FirstOrDefault();
                        var list = new List<string>();
                        if (last != null)
                        {
                            list = uniwork.IQueryable<LocationMaterialBandingEntity>(a => a.F_MaterialCode == materialCode).InnerJoin<LocationEntity>((a, b) => b.F_AreaId == area.F_Id && b.F_EnabledMark == true && b.F_DeleteMark == false && b.F_LocationState == false && a.F_LocationId == b.F_Id && b.F_SortCode == last.F_SortCode).Select((a, b) => b).Select(a => a.F_LocationCode).ToList();
                        }
						else
						{
                            last = uniwork.IQueryable<LocationEntity>(a => a.F_AreaId == area.F_Id && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_LocationState == false && (a.F_MaterialName == null || a.F_MaterialName == "")).OrderBy(a => a.F_SortCode).FirstOrDefault();
                            list = uniwork.IQueryable<LocationEntity>(a =>a.F_AreaId == area.F_Id && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_LocationState == false && (a.F_MaterialName == null || a.F_MaterialName == "") && a.F_SortCode==last.F_SortCode).OrderBy(a => a.F_SortCode).Select(a => a.F_LocationCode).ToList();
                        }
                        if (list.Contains(locationCode))
						{
                            result = true;
                        }
                    }
                    else
                    {
                        var list = uniwork.IQueryable<LocationEntity>(a => a.F_AreaId == area.F_Id && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_LocationState == false).InnerJoin<LocationMaterialBandingEntity>((a, b) => a.F_Id == b.F_LocationId && b.F_MaterialCode == materialCode).Select((a, b) => a.F_LocationCode).ToList();
                        if (list != null && list.Count() > 0)
                        {
							if (list.Contains(locationCode))
							{
                                result = true;
                            }
                        }
						else
						{
                            list = uniwork.IQueryable<LocationEntity>(a => a.F_AreaId == area.F_Id && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_LocationState == false && (a.F_MaterialName == null || a.F_MaterialName == "")).Select(a => a.F_LocationCode).ToList();
                            if (list.Contains(locationCode))
                            {
                                result = true;
                            }
                        }
                    }
                    var material = uniwork.IQueryable<MaterialEntity>(a => a.F_MaterialCode == materialCode).FirstOrDefault();
					//检测物料跟库位是否有关联,禁止跨库乱放
					if (material.F_MaterialType == 0)
					{
						if (area.F_AreaType != 0)
						{
                            result = false;
                        }
					}
                    else if (uniwork.IQueryable<EqpAreaBandingEntity>(a => a.F_AreaId == area.F_Id)
                        .InnerJoin<ProcessEqpBandingEntity>((a, b) => a.F_EqpId == b.F_EqpId)
                        .InnerJoin<BomFormEntity>((a, b, c) => b.F_ProcessId == c.F_ProcessId&& c.F_MaterialId==material.F_Id)
                        .Select((a,b,c)=>c).Count() == 0)
					{
                        result = false;
                    }        
                }
            }
            return result;
        }

		public async Task<List<LocationEntity>> GetLocationListByLocation(string locationCode, string materialCode)
		{
            if (string.IsNullOrEmpty(locationCode))
            {
                return new List<LocationEntity>();
            }
            var location = uniwork.IQueryable<LocationEntity>(a => a.F_LocationCode == locationCode).FirstOrDefault();
            if (location==null)
            {
                return new List<LocationEntity>();
            }
            var query = uniwork.IQueryable<LocationEntity>().Where(a => a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_AreaId == location.F_AreaId);
            var area = await uniwork.FindEntity<AreaEntity>(a => a.F_Id == location.F_AreaId);
            if (!string.IsNullOrEmpty(materialCode))
            {
                if (query.InnerJoin<LocationMaterialBandingEntity>((a, b) => a.F_Id == b.F_LocationId && b.F_MaterialCode == materialCode).Select((a, b) => a).Count() > 0)
                {
                    query = query.InnerJoin<LocationMaterialBandingEntity>((a, b) => a.F_Id == b.F_LocationId && b.F_MaterialCode == materialCode).Select((a, b) => a);
                }
                else
                {
                    query = query.Where(a => a.F_MaterialName == null || a.F_MaterialName == "");
                }
            }
            return query.OrderBy("F_SortCode,F_LocationCode").ToList();
        }

		public async Task<LocationEntity> GetLocationReturn(string transferCode)
        {
            var temp = uniwork.IQueryable<EqpMaterialUseEntity>().Where(a => a.F_TransferBoxCode == transferCode&&a.F_DoneNum!=a.F_Num).FirstOrDefault();
            if (temp == null)
            {
                return null;
            }
            var location= uniwork.IQueryable<LocationEntity>().Where(a => a.F_LocationCode==temp.F_LocationCode).FirstOrDefault();
            AreaEntity area;
            if (location == null)
            {
                area = uniwork.IQueryable<AreaEntity>(a => a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_AreaType == 0).FirstOrDefault();
            }
			else
			{
                area = uniwork.IQueryable<AreaEntity>(a => a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_Id ==location.F_AreaId).FirstOrDefault();
            }
            //规则重新定义,有绑定先退绑定，然后退非绑定
            if (uniwork.IQueryable<LocationEntity>(a => a.F_AreaId == area.F_Id && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_LocationState == false).InnerJoin<LocationMaterialBandingEntity>((a, b) => a.F_Id == b.F_LocationId && b.F_MaterialCode == temp.F_MaterialCode).Select((a, b) => a).Count() > 0)
            {
                return uniwork.IQueryable<LocationEntity>(a => a.F_AreaId == area.F_Id && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_LocationState == false).InnerJoin<LocationMaterialBandingEntity>((a, b) => a.F_Id == b.F_LocationId && b.F_MaterialCode == temp.F_MaterialCode).Select((a, b) => a).OrderBy("F_SortCode,F_LocationCode").FirstOrDefault();
            }
            return uniwork.IQueryable<LocationEntity>(a => a.F_AreaId == area.F_Id && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_LocationState == false && (a.F_MaterialName == null || a.F_MaterialName == "")).OrderBy("F_SortCode,F_LocationCode").FirstOrDefault();
        }
        public async Task<List<LocationEntity>> GetLocationList(string eqpName, string areaCode, string materialCode = "")
        {
            if (!string.IsNullOrEmpty(eqpName))
            {
                var eqp = await uniwork.FindEntity<EquipmentEntity>(a => a.F_EqpName == eqpName);
                var temp = uniwork.IQueryable<EqpAreaBandingEntity>().Where(a => a.F_EqpId == eqp.F_Id).FirstOrDefault();
                if (temp != null)
                {
                    var areaTemp = await uniwork.FindEntity<AreaEntity>(temp.F_AreaId);
                    areaCode = areaTemp.F_AreaCode;
                }
            }
            if (string.IsNullOrEmpty(areaCode))
            {
                return new List<LocationEntity>();
            }
            var area = await uniwork.FindEntity<AreaEntity>(a=>a.F_AreaCode == areaCode);
            var query = uniwork.IQueryable<LocationEntity>().Where(a => a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_AreaCode == areaCode);
            if (!string.IsNullOrEmpty(materialCode))
			{
                if (query.InnerJoin<LocationMaterialBandingEntity>((a, b) => a.F_Id == b.F_LocationId && b.F_MaterialCode == materialCode).Select((a, b) => a).Count()>0)
                {
                    query = query.InnerJoin<LocationMaterialBandingEntity>((a, b) => a.F_Id == b.F_LocationId && b.F_MaterialCode == materialCode).Select((a, b) => a);
                }
                else
                {
                    query = query.Where(a => a.F_MaterialName == null|| a.F_MaterialName == "");
                }
            }
            return query.OrderBy("F_SortCode,F_LocationCode").ToList();
        }
        public async Task<List<LocationEntity>> GetLocationList()
        {
            var query = uniwork.IQueryable<LocationEntity>().Where(a => a.F_EnabledMark == true && a.F_DeleteMark == false&&a.F_LocationState==false).OrderBy("F_AreaId,F_SortCode");
            return query.ToList();
        }
        #endregion

        #region 提交数据
        public async Task SubmitForm(LocationEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.F_LocationState = false;
                entity.Create();
                var area = await uniwork.FindEntity<AreaEntity>(entity.F_AreaId);
                entity.F_AreaCode = area.F_AreaCode;
                await repository.Insert(entity);
            }
            else
            {
                var old = await repository.FindEntity(keyValue);
                //库区编号不允许修改
                entity.F_LocationCode = null;
                if (repository.IQueryable().Where(a =>a.F_Id==keyValue&& a.F_LocationState==true).Count() > 0)
                {
                    throw new Exception("库位使用中无法修改库位");
                }
                entity.Modify(keyValue);
                var area = await uniwork.FindEntity<AreaEntity>(entity.F_AreaId);
                entity.F_AreaCode = area.F_AreaCode;
                await repository.Update(entity);
            }
        }

		public async Task BandingForm(string id, string keyValue)
		{
            var ids = keyValue.Split(',');
            var materialIds = id.Split(',');
            var materials = uniwork.IQueryable<MaterialEntity>(a => materialIds.Contains(a.F_Id)).ToList();
            var materialName =string.Join(",", materials.Select(a => a.F_MaterialName).ToList());
            uniwork.BeginTrans();
            await repository.Update(t => ids.Contains(t.F_Id), t => new LocationEntity
            {
                F_MaterialName= materialName
            });
            await uniwork.Delete<LocationMaterialBandingEntity>(a => ids.Contains(a.F_LocationId));
            List<LocationMaterialBandingEntity> list = new List<LocationMaterialBandingEntity>();
			foreach (var item in materials)
			{
				foreach (var locationId in ids)
				{
                    LocationMaterialBandingEntity temp = new LocationMaterialBandingEntity();
                    temp.F_Id = Utils.GuId();
                    temp.F_LocationId = locationId;
                    temp.F_MaterialId = item.F_Id;
                    temp.F_MaterialCode = item.F_MaterialCode;
                    temp.F_MaterialName = item.F_MaterialName;
                    list.Add(temp);
                }
            }
            await uniwork.Insert(list);
            uniwork.Commit();
        }

		public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            if (repository.IQueryable().Where(a => ids.Contains(a.F_Id) && a.F_LocationState == true).Count() > 0)
            {
                throw new Exception("库位使用中无法删除");
            }
            await repository.Update(t => ids.Contains(t.F_Id), t => new LocationEntity
            {
                F_DeleteMark = true,
                F_DeleteUserId = currentuser.UserId,
                F_DeleteTime = DateTime.Now
            });
        }
        public async Task<List<LocationEntity>> CheckFile(string fileFullName)
        {
            if (!FileHelper.IsExcel(fileFullName))
            {
                throw new Exception("文件不是有效的Excel文件!");
            }
            //文件解析
            var list = new ExcelHelper<LocationEntity>().ImportFromExcel(fileFullName);
            //删除文件
            File.Delete(fileFullName);
            foreach (var item in list)
            {
                item.F_Id = Utils.GuId();
                item.F_EnabledMark = true;
                item.F_LocationType = item.F_LocationType == null ? 1 : item.F_LocationType;
                item.F_DeleteMark = false;
                item.F_LocationState = false;
                List<string> str = new List<string>();
                var area = await uniwork.FindEntity<AreaEntity>(a => a.F_AreaCode == item.F_AreaCode);
                if (string.IsNullOrEmpty(item.F_AreaCode)|| area==null)
                {
                    item.F_EnabledMark = false;
                    item.ErrorMsg = "编号不存在";
                    continue;
                }
                else
                {
                    item.F_AreaId = area.F_Id;
                }
                if (repository.IQueryable(a => a.F_LocationCode == item.F_LocationCode).Count() > 0 || list.Where(a => a.F_LocationCode == item.F_LocationCode).Count() > 1)
                {
                    str.Add("编号重复");
                    item.F_EnabledMark = false;
                }
                if (item.F_SortCode == null)
                {
                    str.Add("顺序不存在");
                    item.F_EnabledMark = false;
                }
                if (item.F_EnabledMark == false)
                {
                    item.ErrorMsg = string.Join(',', str.ToArray());
                }
            }
            return list;
        }

        public async Task ImportForm(List<LocationEntity> filterList)
        {
            foreach (var item in filterList)
            {
                item.Create();
            }
            await uniwork.Insert(filterList);
        }
        #endregion

    }
}
