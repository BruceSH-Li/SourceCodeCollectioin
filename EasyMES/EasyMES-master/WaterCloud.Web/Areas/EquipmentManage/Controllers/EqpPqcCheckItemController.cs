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
    /// 日 期：2021-06-08 12:32
    /// 描 述：设备点检项目控制器类
    /// </summary>
    [Area("EquipmentManage")]
    public class EqpPqcCheckItemController : BaseController
    {
        public EqpPqcCheckItemService _service { get; set; }
        public EquipmentService _eqpservice { get; set; }

        [HttpGet]
        public ActionResult CloneForm()
        {
            return View();
        }

        #region 获取数据

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTreeGridJson(string keyword)
        {
            var data = await _eqpservice.GetLookList();
            data = data.Where(t => t.F_EqpType == 0).ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.F_EqpName.Contains(keyword));
            }
            var treeList = new List<TreeGridModel>();
            foreach (EquipmentEntity item in data)
            {
                TreeGridModel treeModel = new TreeGridModel();
                treeModel.id = item.F_Id;
                treeModel.title = item.F_EqpName;
                treeModel.parentId = "0";
                //treeModel.self = item;
                treeList.Add(treeModel);
            }
            return DTreeResult(treeList.TreeList());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetEqpListJson(string keyword)
        {
            var data = await _eqpservice.GetList(keyword);
            data = data.Where(t => t.F_EqpType == 0).OrderBy(t=>t.F_EqpName).ToList();
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetEqpListJsonxmSelect(string keyword)
        {
            var datanew = await _eqpservice.GetList(keyword);
            var data = datanew.Where(t => t.F_EqpType == 0).OrderBy(t => t.F_EqpName)
                .Select(t=>new { 
                    name=t.F_EqpName,
                    value=t.F_Id
                })
                .ToList();
            return Content(data.ToJson());
        }


        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(string EqpId,SoulPage<EqpPqcCheckItemEntity> pagination, string keyword)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_Sequence";
                pagination.order = "asc";
            }
            var data = await _service.GetLookList(EqpId,pagination, keyword);
            return Content(pagination.setData(data).ToJson());
        }

        //[HttpGet]
        //[HandlerAjaxOnly]
        //public async Task<ActionResult> GetListJson(string keyword)
        //{
        //    var data = await _service.GetList(keyword);
        //    return Content(data.ToJson());
        //}

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
        public async Task<ActionResult> SubmitForm(EqpPqcCheckItemEntity entity, string keyValue)
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
        public async Task<ActionResult> SubmitCloneForm(string ids,string eqpId)
        {
            try
            {
                await _service.SubmitCloneForm(ids, eqpId);
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
