/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/

using System;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Service;
using WaterCloud.Service.SystemSecurity;
using WaterCloud.Code;
using WaterCloud.Domain.SystemSecurity;
using System.Threading.Tasks;
using WaterCloud.Service.SystemOrganize;
using WaterCloud.Domain.SystemOrganize;
using Chloe;
using WaterCloud.Service.DingTalkManage;
using System.Net;
using WaterCloud.Service.RunMonitor;

namespace WaterCloud.Web.Controllers
{
    public class BoardLoginController : Controller
    {
        public BoardAccountService _boardService { get; set; }
        public LogService _logService { get; set; }
        public SystemSetService _setService { get; set; }
        public DingTalkConfigService _dingService { get; set; }
        [HttpGet]
        public virtual async Task<ActionResult> Index()
        {
            //登录页获取logo和项目名称
            try
            {
                var dingconfig = await _dingService.GetBoardLoginForm();
                if (dingconfig != null)
                {
                    ViewBag.Url = dingconfig.F_RedirectUrl;
                    ViewBag.DingLoginAppId = dingconfig.F_AppId;
                }
                var systemset = await _setService.GetFormByHost("");
                ViewBag.ProjectName = "MES可视化看板";
                ViewBag.LogoIcon = ".." + systemset.F_Logo;
                return View();
            }
            catch (Exception)
            {
                ViewBag.ProjectName = "MES可视化看板";
                ViewBag.LogoIcon = "../icon/favicon.ico";
                return View();
            }

        }
        [HttpGet]
        public async Task<ActionResult> OutLogin()
        {
            await _logService.WriteDbLog(new LogEntity
            {
                F_ModuleName = "系统登录(看板)",
                F_Type = DbLogType.Exit.ToString(),
                F_Account = OperatorProvider.Provider.GetBoardCurrent().F_Account,
                F_NickName = OperatorProvider.Provider.GetBoardCurrent().F_Account,
                F_Result = true,
                F_Description = "安全退出系统",
            });
            await OperatorProvider.Provider.EmptyBoardCurrent();
            return Redirect("/BoardLogin/Index");
        }
        /// <summary>
        /// 验证登录状态请求接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> CheckLoginState()
        {
            try
            {
                var current = OperatorProvider.Provider.GetBoardCurrent();
                if (current == null|| current.F_Id == null)
                {
                    await OperatorProvider.Provider.EmptyBoardCurrent();
                    return Content(new AlwaysResult { state = ResultType.error.ToString() }.ToJson());
                }
                else
                {
                    return Content(new AlwaysResult { state = ResultType.success.ToString() }.ToJson());
                }
            }
            catch (Exception)
            {
                return Content(new AlwaysResult { state = ResultType.error.ToString() }.ToJson());
            }

        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> CheckLogin(string username, string password)
        {
            //根据域名判断租户
            LogEntity logEntity = new LogEntity();
            logEntity.F_ModuleName ="系统登录(看板)";
            logEntity.F_Type = DbLogType.Login.ToString();
            try
            {
                OperatorBoard operatorBoard = new OperatorBoard();
                try
                {
                    operatorBoard = await _boardService.CheckLogin(username, password);
                    await OperatorProvider.Provider.AddLoginBoard(operatorBoard);
                    logEntity.F_Account = operatorBoard.F_Account;
                    logEntity.F_NickName = operatorBoard.F_Account;
                    logEntity.F_Result = true;
                    logEntity.F_Description = "登录成功";
                    await _logService.WriteDbLog(logEntity);
                    return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "登录成功。" }.ToJson());
                }
                catch (Exception ex)
                {
                    if (ex.Message== "账户不存在，请重新输入")
                    {
                        UserEntity userEntity = await _boardService.CheckDingLogin(username, password);
                        operatorBoard = await _boardService.CheckLoginSkip();
                        WebHelper.WriteCookie("boardlogin_uerId", userEntity.F_Id);
                        WebHelper.WriteCookie("boardlogin_uerName", userEntity.F_RealName);
                        await OperatorProvider.Provider.AddLoginBoard(operatorBoard);
                        logEntity.F_Account = operatorBoard.F_Account;
                        logEntity.F_NickName = operatorBoard.F_Account;
                        logEntity.F_Result = true;
                        logEntity.F_Description = "登录成功";
                        await _logService.WriteDbLog(logEntity);
                        return Content(new AlwaysResult { state = ResultType.success.ToString(), message = "登录成功。" }.ToJson());
                    }
                    else
                    {
                        throw new Exception(ex.Message);
                    }
                }
                //缓存保存用户信息

            }
            catch (Exception ex)
            {
                logEntity.F_Account = username;
                logEntity.F_NickName = username;
                logEntity.F_Result = false;
                logEntity.F_Description = "登录失败，" + ex.Message;
                await _logService.WriteDbLog(logEntity);
                return Content(new AlwaysResult { state = ResultType.error.ToString(), message = ex.Message }.ToJson());
            }
        }
        /// <summary>
        /// 钉钉扫码登录
        /// </summary>
        /// <param name="code">临时授权</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> DingLogin(string code)
        {
            LogEntity logEntity = new LogEntity();
            logEntity.F_ModuleName = "系统登录";
            logEntity.F_Type = DbLogType.Login.ToString();
            try
            {
                UserEntity userEntity = await _boardService.CheckDingLogin(code);
                if (userEntity.F_Id == null)
                {
                    return new RedirectResult(HttpContext.Request.PathBase + "/Login/DingBanding?OpenId=" + userEntity.F_DingTalkOpenId + "&Nick=" + WebUtility.UrlEncode(userEntity.F_DingTalkNick) + "&UnionId=" + userEntity.F_DingTalkUnionId+"&type=1");
                }
                OperatorBoard operatorBoard = await _boardService.CheckLoginSkip();
                await OperatorProvider.Provider.AddLoginBoard(operatorBoard);
                logEntity.F_Account = operatorBoard.F_Account;
                logEntity.F_NickName = operatorBoard.F_Account;
                logEntity.F_Description = "登录成功";
                await _logService.WriteDbLog(logEntity);
                WebHelper.WriteCookie("boardlogin_uerId", userEntity.F_Id);
                WebHelper.WriteCookie("boardlogin_uerName", userEntity.F_RealName);
                return new RedirectResult(HttpContext.Request.PathBase + "/BoardHome/Index?userId="+userEntity.F_Id);

            }
            catch (Exception ex)
            {
                logEntity.F_Account = "钉钉登录";
                logEntity.F_NickName = "钉钉登录";
                logEntity.F_Result = false;
                logEntity.F_Description = "登录失败，" + ex.Message;
                await _logService.WriteDbLog(logEntity);
                return new RedirectResult(HttpContext.Request.PathBase + "/Home/Error?msg=405");
            }
        }
    }
}
