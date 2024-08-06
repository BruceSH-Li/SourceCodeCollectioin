/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/

using Microsoft.AspNetCore.Mvc;
using WaterCloud.Service.SystemSecurity;
using WaterCloud.Code;
using System.Threading.Tasks;
using WaterCloud.Service.SystemOrganize;
using WaterCloud.Service.RunMonitor;
using System.Text;
using WaterCloud.Domain;
using System.Collections.Generic;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Service.MaterialManage;

namespace WaterCloud.Web.Controllers
{
    [HandlerBoard]
    public class BoardHomeController : Controller
    {
        public BoardAccountService _boardService { get; set; }
        public RoleAuthorizeService _roleAuthorizeService { get; set; }
        public LogService _logService { get; set; }
        public SystemSetService _setService { get; set; }
        public AreaService _areaService { get; set; }
        [HttpGet]
        [ServiceFilter(typeof(HandlerLoginAttribute))]
        public async Task<ActionResult> Index()
        {
            var systemset = await _setService.GetFormByHost("");
            ViewBag.ProjectName = "MES可视化看板";
            ViewBag.LogoIcon = ".." + systemset.F_Logo;
            return View();
        }
        [HttpGet]
        [ServiceFilter(typeof(HandlerLoginAttribute))]
        public async Task<ActionResult> BoardSetting()
        {
            return View();
        }
        /// <summary>
        /// 获取当前看板信息请求方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerLoginAttribute))]
        public async Task<ActionResult> GetBoardCode()
        {
            var currentuser = OperatorProvider.Provider.GetBoardCurrent();
            if (currentuser==null||currentuser.F_Id == null)
            {
                return Content("");
            }
            return Content(currentuser.ToJson());
        }
        /// <summary>
        /// 获取库区信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerLoginAttribute))]
        public async Task<ActionResult> GetAreaList(string keyword)
        {
            var data = await _areaService.GetList(keyword);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 初始菜单列表请求方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerLoginAttribute))]
        public async Task<ActionResult> GetInitDataJson()
        {
            return Content(await this.GetMenuListNew());
        }
        /// <summary>
        /// 菜单按钮信息
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetMenuListNew()
        {
            StringBuilder sbJson = new StringBuilder();
            InitEntity init = new InitEntity();
            init.homeInfo = new HomeInfoEntity();
            init.homeInfo.href = "";
            init.logoInfo = new LogoInfoEntity();
            var systemset = await _setService.GetFormByHost("");
            //修改主页及logo参数
            init.logoInfo.title = systemset.F_LogoCode;
            init.logoInfo.image = ".." + systemset.F_Logo;
            init.menuInfo = new List<MenuInfoEntity>();
            init.menuInfo = ToMenuJsonNew(await _roleAuthorizeService.GetMenuBoardList());
            sbJson.Append(init.ToJson());
            return sbJson.ToString();
        }
        /// <summary>
        /// 菜单信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<MenuInfoEntity> ToMenuJsonNew(List<ModuleEntity> data)
        {
            List<MenuInfoEntity> list = new List<MenuInfoEntity>();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    MenuInfoEntity munu = new MenuInfoEntity();
                    munu.title = item.F_FullName;
                    munu.icon = item.F_Icon;
                    munu.href = item.F_UrlAddress;
                    switch (item.F_Target)
                    {
                        case "iframe":
                            munu.target = "_self";
                            break;
                        case "open":
                            munu.target = "_open";
                            break;
                        case "blank":
                            munu.target = "_blank";
                            break;
                        default:
                            munu.target = "_self";
                            break;
                    }
                    if (item.F_IsMenu == true)
                    {
                        list.Add(munu);
                    }
                };
            }
            return list;
        }
    }
}
