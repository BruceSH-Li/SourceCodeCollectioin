using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.QualityManage;
using WaterCloud.Service;
using Microsoft.AspNetCore.Authorization;
using WaterCloud.Service.QualityManage;
using WaterCloud.Service.MaterialManage;

namespace WaterCloud.Web.Areas.QualityManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-05-21 13:50
    /// 描 述：质检单管理控制器类
    /// </summary>
    [Area("QualityManage")]
    public class NeedCheckController :  BaseController
    {
        public NeedCheckService _service {get;set;}
        public TransferBoxBatchService _batchService { get; set; }

        #region 获取数据
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<NeedCheckEntity> pagination, string keyword, int timetype = 2)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "desc";
            }
            var data = await _service.GetLookList(pagination, timetype, keyword);
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
        public async Task<ActionResult> GetFormJson(long keyValue)
        {
            var data = await _service.GetLookForm(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(NeedCheckEntity entity, long keyValue)
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
        public async Task<ActionResult> PrinterForm(long num)
        {
            try
            {
                var entity = await _batchService.GetCheckForm();
                var data = await _batchService.PrinterForm(entity.F_Id,num);
                await Success("操作成功。", "", entity.F_Id, DbLogType.Submit);
                return Content(data.ToJson());
            }
            catch (Exception ex)
            {
                await Error(ex.Message, "", "", DbLogType.Submit);
                return Content(null);
            }
        }
        #endregion
    }
}
