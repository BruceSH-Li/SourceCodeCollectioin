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
using WaterCloud.Service.SystemManage;
using WaterCloud.Domain.SystemManage;

namespace WaterCloud.Web.Areas.ClassTask.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-03 13:55
    /// 描 述：信息设置控制器类
    /// </summary>
    [Area("ClassTask")]
    public class ControlJobUserBandingController :  BaseController
    {
        public ControlJobUserBandingService _service {get;set;}
        public ItemsDataService _typeService { get; set; }
        [HttpGet]
        public ActionResult CloneForm()
        {
            return View();
        }
        [HttpGet]
        public ActionResult TimeForm()
        {
            return View();
        }
        #region 获取数据
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetGridJson(string itemId = "" , string keyword="", string eqpId = "")
        {
            var data = await _service.GetLookList(eqpId,itemId, keyword);
            return Success(data.Count, data);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetListJson(string itemId, string keyword="", string eqpId="")
        {
            var data = await _service.GetList(eqpId, itemId, keyword);
            return Success(data.Count, data);
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
        public async Task<ActionResult> GetTimeFormJson(string keyValue)
        {
            var data = await _service.GetTimeFormJson(keyValue);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTreeGridJson(string keyword)
        {
            var data = await _typeService.GetItemList("Mes_ControlJobType");
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.F_ItemCode.Contains(keyword));
            }
            var treeList = new List<TreeGridModel>();
            foreach (ItemsDetailEntity item in data)
            {
                TreeGridModel treeModel = new TreeGridModel();
                treeModel.id = item.F_Id;
                treeModel.title = item.F_ItemName;
                treeModel.parentId = "0";
                //treeModel.self = item;
                treeList.Add(treeModel);
            }
            return DTreeResult(treeList.TreeList());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(string itemId, string keyValue, string eqpId)
        {
            try
            {
                await _service.SubmitForm(eqpId, itemId, keyValue);
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitTimeForm(ControlJobMustTimeEntity entity)
        {
            try
            {
                await _service.SubmitTimeForm(entity);
                return await Success("操作成功。", "", null);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", null);
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
        public async Task<ActionResult> SubmitCloneForm(int jobType, string ids, bool useEqp, string eqpId="")
        {
            try
            {
                await _service.SubmitCloneForm(jobType, ids, eqpId, useEqp);
                return await Success("克隆成功。", "", ids, DbLogType.Create);
            }
            catch (Exception ex)
            {
                return await Error("克隆失败，" + ex.Message, "", ids, DbLogType.Create);
            }
        }
        #endregion
    }
}
