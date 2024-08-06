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
using WaterCloud.Domain.ReportRecord;

namespace WaterCloud.Web.Areas.ReportRecord.Controllers
{
    [Area("ReportRecord")]
    public class CheckDateStorageController : BaseController
    {
        public CheckDateStorageService _service { get; set; }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetList(SoulPage<CheckDateStorageEntity> pagination, string keyword, string datetime="",int checktype=0)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CheckDate desc,F_ClassNum desc,F_MaterialType asc,F_MaterialCode asc";
                pagination.order = "asc";
            }
            var data = await _service.GetList(pagination, datetime, checktype, keyword);
            return Content(pagination.setData(data).ToJson());
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
