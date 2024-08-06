using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.ClassTask;
using WaterCloud.Service;
using Microsoft.AspNetCore.Authorization;
using WaterCloud.Service.ClassTask;
using WaterCloud.Service.EquipmentManage;
using WaterCloud.Service.MaterialManage;

namespace WaterCloud.Web.Areas.ClassTask.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-01-15 14:18
    /// 描 述：待执行任务控制器类
    /// </summary>
    [Area("ClassTask")]
    public class ControlJobController :  BaseController
    {
        public ControlJobService _service {get;set;}
        public StorageService _storageService {get;set; }
        public EquipmentService _eqpService {get;set; }

        [HttpGet]
        public virtual ActionResult Doing()
        {
            return View();
        }
        [HttpGet]
        public virtual ActionResult Done()
        {
            return View();
        }
        [HttpGet]
        public virtual ActionResult Location()
        {
            return View();
        }
        #region 获取数据
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<ControlJobEntity> pagination, string keyword,int type=0)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {

                switch (type)
                {
                    case 2:
                        pagination.field = "F_NeedTime";
                        pagination.order = "desc";
                        break;
                    default:
                        pagination.field = "F_NeedTime";
                        pagination.order = "asc";
                        break;
                }
            }
            var data = await _service.GetLookList(pagination,keyword, type);
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
        public async Task<ActionResult> GetLocationJson(int type, string locationCode,string areaCode,int areaType)
        {
            var data = await _service.GetLocationJson(type, locationCode, areaCode, areaType);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetStorageByOutStorage(string keyValue)
        {
            var data = await _storageService.GetStorageByOutStorage(keyValue);
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
        public async Task<ActionResult> SubmitForm(ControlJobEntity entity)
        {
            try
            {
                entity.F_JobType = 4;
                await _service.SubmitForm(entity,true);
                return await Success("操作成功。","","");
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteForm(string keyValue)
        {
            try
            {
                await _service.DeleteForm(keyValue , true);
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
        public async Task<ActionResult> RunForm(string keyValue)
        {
            try
            {
                await _service.RunForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Submit);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Submit);
            }
        }
        #endregion
    }
}
