using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Service;
using Microsoft.AspNetCore.Authorization;
using WaterCloud.Service.MaterialManage;
using WaterCloud.Service.PlanManage;
using WaterCloud.Domain.PlanManage;

namespace WaterCloud.Web.Areas.MaterialManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-16 13:38
    /// 描 述：出库管理控制器类
    /// </summary>
    [Area("MaterialManage")]
    public class OutStorageController :  BaseController
    {
        public OutStorageService _service {get;set;}
        public OrderService _orderService {get;set; }


        [HttpGet]
        public virtual ActionResult AddForm()
        {
            return View();
        }

        [HttpGet]
        public virtual ActionResult AddOrderForm()
        {
            return View();
        }
        #region 获取数据
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetMaterialGridJson(SoulPage<MaterialEntity> pagination, string keyword, string ids)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "asc";
            }
            var data = await _service.GetMaterialList(pagination, keyword, ids);
            return Content(pagination.setData(data).ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetOrderGridJson(SoulPage<OrderEntity> pagination, string keyword, string ids)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_PlanStartTime";
                pagination.order = "desc";
            }
            var data = await _orderService.GetLookList(pagination, keyword, ids,false);
            return Content(pagination.setData(data).ToJson());
        }
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<OutStorageEntity> pagination, string keyword)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_PlanTime desc,F_OutStorageCode asc";
                pagination.order = "asc";
            }
            var data = await _service.GetLookList(pagination,keyword);
            return Content(pagination.setData(data).ToJson());
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
        public async Task<ActionResult> GetFormJson(string keyValue)
        {
            var data = await _service.GetLookForm(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(OutStorageEntity entity, string keyValue)
        {
            try
            {
                await _service.SubmitForm(entity, keyValue);
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
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
        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
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
        public async Task<ActionResult> DisabledForm(string keyValue)
        {
            try
            {
                await _service.DisabledForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Submit);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Submit);
            }
        }
        #endregion
    }
}
