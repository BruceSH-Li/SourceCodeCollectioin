using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Service;
using Microsoft.AspNetCore.Authorization;
using WaterCloud.Service.EquipmentManage;

namespace WaterCloud.Web.Areas.EquipmentManage.Controllers
{
    [Area("EquipmentManage")]
    public class EqpPqcCheckMDController : BaseController
    {
        public EquipmentService _eqpservice { get; set; }
        public EqpPqcCheckMDService _service { get; set; }

        [HttpGet]
        public ActionResult RemarkDetail()
        {
            return View();
        }

        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(string EqpId, string Shiftday, string Month)
        {
            var data = await _service.GetLookList(EqpId, Shiftday, Month);
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SaveCheckResult([FromBody] List<EqpPqcTempEntity> tableData)
        {
            try
            {
                await _service.SaveCheckResult(tableData);
                return await Success("操作成功。", "","");
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", "");
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetRemarkDetailJson(string eqpid, string month)
        {
            var data = await _service.GetDailyRemark(eqpid, month);
            return Content(data.ToJson());
        }


    }
}
