using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Service;
using WaterCloud.Service.MaterialManage;

namespace WaterCloud.Web.Areas.EquipmentManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-01 12:23
    /// 描 述：模具物料绑定控制器类
    /// </summary>
    [Area("MaterialManage")]
    public class MaterialBomController :  BaseController
    {
        public MaterialBomService _service {get;set;}

        [HttpGet]
        public ActionResult BandingForm()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ProcessForm()
        {
            return View();
        }
        #region 获取数据
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetFormJson(string keyValue, string itemId)
        {
            var data = await _service.GetLookForm(keyValue, itemId);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetBomList(string keyValue,string itemId)
        {
            var data = await _service.GetBomList(keyValue, itemId);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(BomFormEntity entity, string listData)
        {
            try
            {
                await _service.SubmitForm(entity, listData);
                return await Success("操作成功。", "", "");
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", "");
            }
        }
        #endregion
    }
}
