using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Service;
using WaterCloud.Service.PlanManage;

namespace WaterCloud.Web.Areas.PlanManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-23 14:04
    /// 描 述：采购计划控制器类
    /// </summary>
    [Area("PlanManage")]
    public class InStoragePlanController :  BaseController
    {
        public InStoragePlanService _service {get;set;}

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
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetListJson(SoulPage<InStorageEntity> pagination, string keyValue, string materialId)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_InStorageState asc,F_PlanTime asc";
                pagination.order = "asc";
            }
            var data = await _service.GetListJson(pagination, keyValue, materialId);
            return Content(pagination.setData(data).ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetFormJson(string keyValue,string materialId)
        {
            var data = await _service.GetLookForm(keyValue, materialId);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(InStorageEntity entity,string id)
        {
            try
            {
                entity.F_Id = id;
                await _service.SubmitForm(entity);
                return await Success("操作成功。", "", "");
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", "");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteForm(string currentdate, string materialId)
        {
            try
            {
                await _service.DeleteForm(currentdate, materialId);
                return await Success("操作成功。", "", currentdate + ","+ materialId, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", currentdate + "," + materialId, DbLogType.Delete);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteAddForm(string keyValue)
        {
            try
            {
                await _service.DeleteAddForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Delete);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteAllForm(string currentdate)
        {
            try
            {
                await _service.DeleteAllForm(currentdate);
                return await Success("操作成功。", "", currentdate, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", currentdate, DbLogType.Delete);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> EnabledForm(string keyValue)
        {
            try
            {
                await _service.EnabledForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Submit);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Submit);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> EnabledAllForm(string currentdate)
        {
            try
            {
                await _service.EnabledAllForm(currentdate);
                return await Success("操作成功。", "", currentdate, DbLogType.Submit);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", currentdate, DbLogType.Submit);
            }
        }
        #endregion
    }
}
