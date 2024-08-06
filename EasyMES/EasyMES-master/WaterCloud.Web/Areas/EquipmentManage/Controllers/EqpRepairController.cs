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
using WaterCloud.Service.SystemOrganize;

namespace WaterCloud.Web.Areas.EquipmentManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-15 12:37
    /// 描 述：设备维修记录表控制器类
    /// </summary>
    [Area("EquipmentManage")]
    public class EqpRepairController :  BaseController
    {
        public EqpRepairService _service {get;set;}
        public UserService _userService { get; set; }

        [HttpGet]
        public virtual ActionResult ManRecordForm()
        {
            return View();
        }

        [HttpGet]
        public virtual ActionResult ManRecordDetails()
        {
            return View();
        }


        #region 获取数据
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<EqpRepairEntity> pagination, string RepairNo,string EqpId,string StartTime,string EndTime,string Status)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "desc";
            }
            var data = await _service.GetLookList(pagination,RepairNo,EqpId,StartTime,EndTime,Status);
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
        public async Task<ActionResult> GetUserListJson(string keyword)
        {
            var data = await _userService.GetUserList(keyword);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetRepairManListJson(string RepairNo)
        {
            var data = await _service.GetRepairManList(RepairNo);
            return Content(data.ToJson());
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetFormJson(string keyValue)
        {
            var data = await _service.GetLookForm(keyValue);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetManRecordFormJson(string keyValue)
        {
            var data = await _service.GetManRecordLookForm(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> ManRecordSubmitForm(EqpRepairManRecordEntity entity, string keyValue)
        {
            try
            {
                await _service.ManRecordSubmitForm(entity, keyValue);
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(EqpRepairEntity entity, string keyValue)
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
        public async Task<ActionResult> ManRecordDeleteForm(string keyValue)
        {
            try
            {
                await _service.ManRecordDeleteForm(keyValue);
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
