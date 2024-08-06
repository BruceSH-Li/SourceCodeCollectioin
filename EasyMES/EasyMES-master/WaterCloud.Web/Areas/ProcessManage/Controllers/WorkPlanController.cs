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

namespace WaterCloud.Web.Areas.ProcessManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-15 10:07
    /// 描 述：生产计划控制器类
    /// </summary>
    [Area("ProcessManage")]
    public class WorkPlanController :  BaseController
    {
        public WorkPlanService _service {get;set;}

        [HttpGet]
        public virtual ActionResult AddForm()
        {
            return View();
        }
        #region 获取数据

        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(string startDay, int CountDay = 7)
        {
            var data = await _service.GetLookList(startDay, CountDay);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetListJson(string keyword)
        {
            var data = await _service.GetList(keyword);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetAddListJson(string eqpId)
        {
            var data = await _service.GetAddListJson(eqpId);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetOpListJson(string eqpId)
        {
            var data = await _service.GetOpListJson(eqpId);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetFormJson(string eqpId, string currentdate, string classnum)
        {
            var data = await _service.GetLookForm(eqpId, currentdate, classnum);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(WorkPlanEntity entity)
        {
            try
            {
                await _service.SubmitForm(entity);
                return await Success("操作成功。", "","");
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "");
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitAddForm(WorkPlanEntity entity)
        {
            try
            {
                await _service.SubmitAddForm(entity);
                return await Success("操作成功。", "", "");
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "");
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteForm(string eqpId, string currentdate, string classnum)
        {
            try
            {
                await _service.DeleteForm(eqpId, currentdate, classnum);
                return await Success("操作成功。", "", eqpId + "," + currentdate + classnum, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", eqpId+","+ currentdate+ classnum,DbLogType.Delete);
            }
        }

        /// <summary>
        /// 删除未创建工单的计划
        /// </summary>
        /// <param name="eqpId"></param>
        /// <param name="currentdate"></param>
        /// <param name="classnum"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteUnCreateOrderForm(string eqpId, string currentdate, string classnum)
        {
            try
            {
                await _service.DeleteUnCreateOrderForm(eqpId, currentdate, classnum);
                return await Success("操作成功。", "", eqpId + "," + currentdate + classnum, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", eqpId + "," + currentdate + classnum, DbLogType.Delete);
            }
        }


        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteAllForm(string currentdate, string classnum)
        {
            try
            {
                await _service.DeleteAllForm(currentdate, classnum);
                return await Success("操作成功。", "",  currentdate + classnum, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", currentdate + classnum, DbLogType.Delete);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> CreateWorkOrder(string eqpId, string currentdate, string classnum)
        {
            try
            {
                await _service.CreateWorkOrder(eqpId, currentdate, classnum);
                return await Success("操作成功。", "", eqpId + "," + currentdate + classnum, DbLogType.Submit);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", eqpId + "," + currentdate + classnum, DbLogType.Submit);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> BatchCreateWorkOrder(string currentdate, string classnum)
        {
            try
            {
                await _service.BatchCreateWorkOrder(currentdate, classnum);
                return await Success("操作成功。", "", currentdate + classnum, DbLogType.Submit);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", currentdate + classnum, DbLogType.Submit);
            }
        }
        #endregion
    }
}
