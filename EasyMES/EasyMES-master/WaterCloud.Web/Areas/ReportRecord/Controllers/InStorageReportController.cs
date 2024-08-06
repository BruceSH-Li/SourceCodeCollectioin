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
using WaterCloud.Domain.MaterialManage;

namespace WaterCloud.Web.Areas.ReportRecord.Controllers
{
    [Area("ReportRecord")]
    public class InStorageReportController : BaseController
    {
        public ReportRecordService _service { get; set; }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetGridJson(SoulPage<InStorageInfoEntity> pagination, string keyword, int timetype = 2)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_Id";
                pagination.order = "desc";
            }
            var data = await _service.GetInStorageInfoList(pagination, timetype, keyword);
            return Content(pagination.setData(data).ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitRemoveLog(string keepTime)
        {
            await _service.RemoveInStorageRecord(keepTime);
            return Success("清空成功。");
        }
    }
}
