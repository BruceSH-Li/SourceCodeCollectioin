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
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-10 15:47
    /// 描 述：设备停机控制器类
    /// </summary>
    [Area("EquipmentManage")]
    public class EqpStopRecordController :  BaseController
    {
        public EqpStopRecordService _service {get;set;}

        [HttpGet]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public  ActionResult AddForm()
        {
            return View();
        }

        #region 获取数据
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<EqpStopRecordEntity> pagination, string F_EqpId,string StartTime,string EndTime, int StopType=2)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "desc";
            }
            var data = await _service.GetLookList(pagination,F_EqpId,StartTime,EndTime, StopType);

            
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
        public async Task<ActionResult> SubmitForm(EqpStopRecordEntity entity, string keyValue)
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
        #endregion
    }
}
