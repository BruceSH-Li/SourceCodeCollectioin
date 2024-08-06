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

namespace WaterCloud.Web.Areas.ProcessManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-03 09:48
    /// 描 述：总生产情况控制器类
    /// </summary>
    [Area("RunMonitor")]
    [HandlerBoard]
    public class SmartScreenController :  BaseController
    {
        public SystemSetService _setService { get; set; }
        public ModuleService _moduleService { get; set; }
        public WorkRunService _service { get; set; }
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
        /// <summary>
        /// 初始数据加载请求方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetBoardDataJson()
        {
            var data = await _service.GetBoardDataJson();
            return Content(data.ToJson());
        }
    }
}
