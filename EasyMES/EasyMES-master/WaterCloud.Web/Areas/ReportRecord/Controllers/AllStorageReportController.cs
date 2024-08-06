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
using System;

namespace WaterCloud.Web.Areas.ReportRecord.Controllers
{
    [Area("ReportRecord")]
    public class AllStorageReportController : BaseController
    {
        public ReportRecordService _service { get; set; }
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<MaterialEntity> pagination, string keyword)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_MaterialType";
                pagination.order = "asc";
            }
            var data = await _service.GetAllStorageInfoList(pagination, keyword);
            return Content(pagination.setData(data).ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(int? num, string keyValue)
        {
            try
            {
                await _service.SubmitAllStorageForm(num, keyValue);
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
            }
        }
    }
}
