/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Code;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterCloud.Service.ReportRecord;
using System;
using WaterCloud.Domain.ProcessManage;

namespace WaterCloud.Web.Areas.ReportRecord.Controllers
{
	[Area("ReportRecord")]
    public class EqpWorkTimeController : BaseController
    {
        public EqpWorkTimeService _service { get; set; }
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<EqpWorkTimeEntity> pagination, string keyword, DateTime? startTime, DateTime? endTime)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_Id";
                pagination.order = "asc";
            }
            var data = await _service.GetList(pagination, startTime, endTime, keyword);
            return Content(pagination.setData(data).ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetEqpListJson(string keyValue)
        {
            var data = await _service.GetEqpListJson(keyValue);
            return Success(data.Count, data);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitRemoveLog(string keepTime)
        {
            await _service.RemoveRecord(keepTime);
            return Success("清空成功。");
        }
    }
}
