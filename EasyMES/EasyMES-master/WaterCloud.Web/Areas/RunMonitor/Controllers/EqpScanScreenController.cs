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

namespace WaterCloud.Web.Areas.ProcessManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-03 09:48
    /// 描 述：总生产情况控制器类
    /// </summary>
    [Area("RunMonitor")]
    [HandlerBoard]
    public class EqpScanScreenController :  BaseController
    {
        public SystemSetService _setService { get; set; }
        public ModuleService _moduleService { get; set; }
        public StorageService _storageService { get; set; }
        public TransferBoxService _boxService { get; set; }
        public MaterialService _materialService { get; set; }
        public WorkRunService _runService { get; set; }
        public UserService _userService { get; set; }
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
        public ActionResult EqpForm()
        {
            return View();
        }
        [HttpGet]
        public ActionResult NeedMaterialJob()
        {
            return View();
        }
        [HttpGet]
        public ActionResult NeedMaterial()
        {
            return View();
        }
        [HttpGet]
        public ActionResult OutputMaterial()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ReadyBox()
        {
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetListJson(string keyValue)
        {
            //获取领料申请前3条
            //设备名称、值班人、产出物料、产量、当前班次
            //当前领用物料名称，数量
            //容器产出计数
            var data = await _storageService.GetEqpScanJson(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> CheckPassword(string password)
        {
            try
            {
                await _userService.CheckSystemPassword(password);
                return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "操作成功" }.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new AlwaysResult { state = ResultType.error.ToString(), message = LogHelper.ExMsgFormat(ex.Message) }.ToJson());
            }
        }
        #region 预绑定流转箱
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetReadyBoxes(string keyValue)
        {
            //获取预绑定流转箱
            var data = await _boxService.GetReadyBoxes(keyValue);
            return Content(data.ToJson());
        }
		[HttpGet]
		[HandlerAjaxOnly]
		public async Task<ActionResult> GetBatch()
		{
			//获取预绑定流转箱
			var data = await _runService.GetBatch();
			return Content(data);
		}
		[HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> AddReadyBox(ReadyTransferBoxEntity entity)
        {
            try
            {
                await _boxService.AddReadyBox(entity);
                return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "操作成功" }.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new AlwaysResult { state = ResultType.error.ToString(), message = LogHelper.ExMsgFormat(ex.Message) }.ToJson());
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> DeleteReadyBox(string transferBoxCode)
        {
            try
            {
                await _boxService.DeleteReadyBox(transferBoxCode);
                return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "操作成功" }.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new AlwaysResult { state = ResultType.error.ToString(), message = LogHelper.ExMsgFormat(ex.Message) }.ToJson());
            }
        }
        #endregion

        #region 领用申请
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetNeedMaterialList(string keyValue)
        {
            //获取预绑定流转箱
            var data = await _materialService.GetNeedMaterialList(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> NeedMaterialJob(string keyValue ,string materialId)
        {
            try
            {
                await _runService.NeedMaterialJob(keyValue, materialId);
                return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "操作成功" }.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new AlwaysResult { state = ResultType.error.ToString(), message = LogHelper.ExMsgFormat(ex.Message) }.ToJson());
            }
        }
        #endregion

        #region 领取物料
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTransferInfo(string transferBoxCode)
        {
            var data = await _storageService.GetTransferInfo(transferBoxCode.ToUpper());
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> NeedMaterial(string keyValue, string transferBoxCode)
        {
            try
            {
                await _runService.ConsumeMaterial(keyValue, transferBoxCode.ToUpper());
                return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "操作成功" }.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new AlwaysResult { state = ResultType.error.ToString(), message = LogHelper.ExMsgFormat(ex.Message) }.ToJson());
            }
        }
        #endregion

        #region 产出物料
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetOutMaterialList(string keyValue)
        {
            var data = await _materialService.GetOutMaterialList(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> OutputMaterial(string keyValue,string materialId, string transferBoxCode,int num,string materialBatch)
        {
            try
            {
                await _runService.OutputMaterial(keyValue, materialId, transferBoxCode.ToUpper(), num, materialBatch);
                return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "操作成功" }.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new AlwaysResult { state = ResultType.error.ToString(), message = LogHelper.ExMsgFormat(ex.Message) }.ToJson());
            }
        }
        #endregion

    }
}
