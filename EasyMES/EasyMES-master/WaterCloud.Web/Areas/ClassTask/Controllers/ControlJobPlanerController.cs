using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.ClassTask;
using WaterCloud.Service;
using Microsoft.AspNetCore.Authorization;
using WaterCloud.Service.ClassTask;
using WaterCloud.Service.EquipmentManage;
using WaterCloud.Domain.SystemOrganize;

namespace WaterCloud.Web.Areas.ClassTask.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-01-15 14:18
    /// 描 述：待执行任务控制器类
    /// </summary>
    [Area("ClassTask")]
    public class ControlJobPlanerController :  BaseController
    {
        public ControlJobPlanerService _service {get;set;}
        #region 获取数据
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<UserExtend> pagination, string keyword)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_DepartmentId";
                pagination.order = "asc";
            }
            var data = await _service.GetLookList(pagination, keyword);
            return Content(pagination.setData(data).ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetUserJson(string keyword)
        {
            var data = await _service.GetList(keyword);
            return Success(data.Count, data);
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(string keyValue)
        {
            try
            {
                await _service.SubmitForm(keyValue);
                return await Success("操作成功。","","");
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteForm(string keyValue)
        {
            try
            {
                await _service.DeleteForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Delete);
            }
        }
        #endregion
    }
}
