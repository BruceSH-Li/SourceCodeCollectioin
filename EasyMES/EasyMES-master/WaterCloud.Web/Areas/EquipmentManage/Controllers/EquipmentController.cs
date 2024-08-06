using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Service;
using WaterCloud.Service.EquipmentManage;
using WaterCloud.Service.SystemManage;

namespace WaterCloud.Web.Areas.EquipmentManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-11-30 14:03
    /// 描 述：设备清单控制器类
    /// </summary>
    [Area("EquipmentManage")]
    public class EquipmentController :  BaseController
    {
        public EquipmentService _service {get;set;}
        public ItemsDataService _itemsService {get;set; }

        [HttpGet]
        public virtual ActionResult MaterialForm()
        {
            return View();
        }
        [HttpGet]
        public virtual ActionResult AddForm()
        {
            return View();
        }
        #region 获取数据
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTreeGridJson(string keyword)
        {
            var data = await _service.GetLookList(keyword);
            if (!string.IsNullOrEmpty(keyword))
            {
                 data = data.TreeWhere(t => t.F_EqpName.Contains(keyword));
            }
            return Success(data.Count, data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetGridJson(SoulPage<EquipmentEntity> pagination, string keyword)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "desc";
            }
            var data = await _service.GetLookList(pagination, keyword);
            return Content(pagination.setData(data).ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTreeSelectJson()
        {
            var data = await _service.GetList();
            var treeList = new List<TreeSelectModel>();
            foreach (var item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.F_Id;
                treeModel.text = item.F_EqpName;
                treeModel.parentId = item.F_ParentId;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        [HandlerBoard]
        public async Task<ActionResult> GetListJson()
        {
            var data = await _service.GetList();
            data = data.Where(a => a.F_ParentId == "0" && a.F_EqpType == 0).OrderBy(a=>a.F_EqpName).ToList();
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetAddListJson(string keyword, string ids, int type = -1, int use = -1)
        {
            var data = await _service.GetList(keyword);
            if (use!=-1)
            {
                if (type!=-1)
                {
                    data = data.Where(a => a.F_ParentId == "0" && a.F_EqpType == type && a.F_EqpUse == use && a.F_EnabledMark == true).ToList();
                }
                else
                {
                    data = data.Where(a => a.F_ParentId == "0" && a.F_EqpUse == use && a.F_EnabledMark == true).ToList();
                }
            }
            else
            {
                if (type != -1)
                {
                    data = data.Where(a => a.F_ParentId == "0" && a.F_EqpType == type && a.F_EnabledMark == true).ToList();
                }
                else
                {
                    data = data.Where(a => a.F_ParentId == "0" && a.F_EnabledMark == true).ToList();
                }
            }
            if (!string.IsNullOrEmpty(ids))
            {
                foreach (var item in ids.Split(','))
                {
                    var temp = data.Find(a => a.F_Id == item);
                    if (temp != null)
                    {
                        temp.LAY_CHECKED = true;
                    }
                }
            }
            return Success(data.Count, data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetMaterialListJson(string keyValue)
        {
            var data = await _service.GetMaterialListJson(keyValue);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetMaterialList(string keyValue)
        {
            var data = await _service.GetMaterialJson(keyValue);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetEqpUseListJson(int type=0)
        {
            var data = await _itemsService.GetItemList("Mes_EqpUse");
            switch (type)
            {
                case 1:
					data = data.Where(a => a.F_Description == "流动").ToList();
					break;
                case 2:
                    data = data.Where(a => a.F_Description == "其他").ToList();
                    break;
                default:
                    data = data.Where(a => a.F_Description == "固定").ToList();
                    break;
            }
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
        public async Task<ActionResult> SubmitForm(EquipmentEntity entity, string keyValue)
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
        public async Task<ActionResult> SubmitMaterialForm(List<EqpMaterialBandingEntity> listData, string F_EqpId)
        {
            try
            {
                await _service.SubmitMaterialForm(listData, F_EqpId);
                return await Success("操作成功。", "", F_EqpId);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", F_EqpId);
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteForm(string keyValue)
        {
            try
            {
                //todo 加删除限制
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
