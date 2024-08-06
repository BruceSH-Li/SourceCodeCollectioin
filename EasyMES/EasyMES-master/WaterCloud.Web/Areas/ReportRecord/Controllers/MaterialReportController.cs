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

namespace WaterCloud.Web.Areas.ReportRecord.Controllers
{
    [Area("ReportRecord")]
    public class MaterialReportController : BaseController
    {
        public ReportRecordService _service { get; set; }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetGridJson(string materialCode, string materialBatch)
        {
            var data = await _service.GetMaterialFlow(materialCode, materialBatch);
            return Content(data.ToJson());
        }
    }
}
