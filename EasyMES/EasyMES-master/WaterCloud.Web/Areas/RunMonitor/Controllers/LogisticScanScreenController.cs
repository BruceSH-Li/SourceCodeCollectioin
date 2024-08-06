using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Service;
using Microsoft.AspNetCore.Authorization;
using WaterCloud.Service.ProcessManage;
using WaterCloud.Service.SystemOrganize;
using WaterCloud.Service.SystemManage;
using WaterCloud.Service.MaterialManage;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Service.ClassTask;
using iTextSharp.text;

namespace WaterCloud.Web.Areas.ProcessManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-03 09:48
    /// 描 述：总生产情况控制器类
    /// </summary>
    [Area("RunMonitor")]
    [HandlerBoard]
    public class LogisticScanScreenController :  BaseController
    {
        public SystemSetService _setService { get; set; }
        public ModuleService _moduleService { get; set; }
        public StorageService _storageService { get; set; }
        public ControlJobService _jobService { get; set; }
        public MaterialService _materialService { get; set; }
        public WorkRunService _workrunService { get; set; }
        public LocationService _locationService { get; set; }
        public AreaService _areaService { get; set; }
		public override ActionResult Index()
        {
            //主页信息获取
            var systemset = _setService.GetFormByHost("").GetAwaiter().GetResult();
            ViewBag.ProjectName = systemset.F_ProjectName;
            ViewBag.LogoIcon = "../.." + systemset.F_Logo;
            var moduleCode = ReflectionHelper.GetClassName(1);
            moduleCode = moduleCode.Substring(0, moduleCode.Length - 10);
            ViewBag.BoardName = _moduleService.GetList().GetAwaiter().GetResult().Where(a=>a.F_EnCode== moduleCode).Select(a=>a.F_FullName).FirstOrDefault();
            return View();
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetListJson(string keyValue)
        {
            //获取当前任务前3条
            var data = await _jobService.GetListByUserId(keyValue);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetUseMaterial(string eqpName,string keyword)
        {
            var isAll = false;
            List<MaterialEntity> materialdata = await _materialService.GetUseMaterialList(eqpName, keyword);
            var data=await _storageService.GetStorageByMaterial(materialdata[0].F_MaterialCode, eqpName, false);
            List<StorageExtend> list = new List<StorageExtend>();
			var areas = await _areaService.GetList();
			foreach (var item in data)
            {
                var temp = list.Where(a => item.F_LocationCode.Contains(a.Prefix)).FirstOrDefault();
				var area = areas.FirstOrDefault(a => a.F_AreaCode == item.F_AreaCode);
				if (temp != null)
                {
					if (isAll || temp.list[0].F_LocationCode.Replace("-", "").Substring(0, area.F_Prefix) == item.F_LocationCode.Replace("-", "").Substring(0, area.F_Prefix))
					{
						temp.list.Add(item);
					}
				}
                else
                {
                    StorageExtend ex = new StorageExtend();
					ex.Prefix = item.F_LocationCode.Substring(0, area.F_Prefix);
					ex.list = new List<StorageEntity>();
                    ex.list.Add(item);
                    list.Add(ex);
                }
            }

            object result = new
            {
                materialdata,
                readyEntity=list
            } as object;            


            return Content(result.ToJson());
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetReturnLocation(string materialCode, string locationCode,bool isAll=false)
        {
            //var data = await _workrunService.GetConsumeMaterialByCode(EqpName, transfercode.ToUpper());
            var data = await _locationService.GetLocationListByLocation(locationCode, materialCode);
            List<LocationExtend> list = new List<LocationExtend>();
            if (isAll)
            {
                data = data.Where(a => a.F_LocationState == false).OrderBy(a => a.F_LocationCode.Length).ToList();
            }
            else
            {
                var first = data.Where(a => a.F_LocationState == false).FirstOrDefault();
                data = data.Where(a => a.F_LocationState == false && a.F_SortCode == first.F_SortCode).ToList();
            }
			var areas = await _areaService.GetList();
			foreach (var item in data)
            {
                var temp = list.Where(a => item.F_LocationCode.Contains(a.Prefix)).FirstOrDefault();
				var area = areas.FirstOrDefault(a => a.F_AreaCode == item.F_AreaCode);
				if (temp != null)
                {
					if (isAll || temp.list[0].F_LocationCode.Replace("-", "").Substring(0, area.F_Prefix) == item.F_LocationCode.Replace("-", "").Substring(0, area.F_Prefix))
					{
						temp.list.Add(item);
					}
				}
                else
                {
                    LocationExtend ex = new LocationExtend();
					ex.Prefix = item.F_LocationCode.Substring(0, area.F_Prefix);
					ex.list = new List<LocationEntity>();
                    ex.list.Add(item);
                    list.Add(ex);
                }
            }

            return Content(list.ToJson());
        }

		/// <summary>
		/// 获取区域列表
		/// </summary>
		/// <param name="keyword">关键词</param>
		/// <param name="type">类型</param>
		/// <returns></returns>
		[HttpGet]
		[HandlerAjaxOnly]
		public async Task<ActionResult> GetAreaList(string keyword = "", int? type = null)
		{
			var data = await _areaService.GetList(keyword);
			data = data.Where(a => a.F_EnabledMark == true).OrderBy(a => a.F_AreaType).ToList();
			if (type != null)
				data = data.Where(a => a.F_AreaType == type).ToList();
			return Content(data.ToJson());
		}
		[HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetUseLocation(string materialCode, string eqpName,string areaCode, bool isAll = false)
        {
            var data = await _locationService.GetLocationList(eqpName, areaCode, materialCode);
            List<LocationExtend> list = new List<LocationExtend>();
            if (isAll)
            {
                data = data.Where(a => a.F_LocationState == false).OrderBy(a => a.F_LocationCode).ToList();
            }
            else
            {
                var first = data.Where(a => a.F_LocationState == false).FirstOrDefault();
                data = data.Where(a => a.F_LocationState == false && a.F_SortCode == first.F_SortCode).ToList();
            }
			var areas = await _areaService.GetList();
			foreach (var item in data)
            {
                var temp = list.Where(a => item.F_LocationCode.Contains(a.Prefix)).FirstOrDefault();
				var area = areas.FirstOrDefault(a => a.F_AreaCode == item.F_AreaCode);
				if (temp != null)
                {
					if (isAll || temp.list[0].F_LocationCode.Replace("-", "").Substring(0, area.F_Prefix) == item.F_LocationCode.Replace("-", "").Substring(0, area.F_Prefix))
					{
						temp.list.Add(item);
					}
				}
                else
                {
                    LocationExtend ex = new LocationExtend();
					ex.Prefix = item.F_LocationCode.Substring(0, area.F_Prefix);
					ex.list = new List<LocationEntity>();
                    ex.list.Add(item);
                    list.Add(ex);
                }
            }
            return Content(list.ToJson());
        }


        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> DoTaskJob(string jobCode)
        {
            try
            {
                await _jobService.DoTaskJob(jobCode);
                return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "操作成功" }.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new AlwaysResult { state = ResultType.error.ToString(), message = LogHelper.ExMsgFormat(ex.Message) }.ToJson());
            }
        }

    }
}
