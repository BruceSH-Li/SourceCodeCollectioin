using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Service;
using WaterCloud.Service.MaterialManage;
using WaterCloud.Service.SystemManage;

namespace WaterCloud.Web.Areas.MaterialManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-14 12:44
    /// 描 述：流转箱管理控制器类
    /// </summary>
    [Area("MaterialManage")]
    public class TransferTypeController : BaseController
    {
        public TransferTypeService _service { get; set; }
        public ItemsDataService _itemService { get; set; }

        #region 获取数据
        [HttpGet]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public override ActionResult Index()
        {
            var temp= _itemService.GetItem("Mes_TransferType").GetAwaiter().GetResult();
            ViewBag.KeyValue = temp.F_Id;
            return View();
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetMaterialGridJson(string ItemId,string keyword)
        {
            var data = await _service.GetList(ItemId, keyword);
            return Success(data.Count, data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetGridJson(string keyword)
        {
            var data = await _itemService.GetItemList("Mes_TransferType");
            if (!string.IsNullOrEmpty(keyword))
            {
                data= data.Where(a=>a.F_ItemCode.Contains(keyword)||a.F_ItemName.Contains(keyword)).ToList();
            }
            return Success(data.Count, data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetMaterialJson(Pagination pagination, string ItemId, string keyword)
        {
            pagination.sort = "F_CreatorTime";
            pagination.order = "desc";
            var data = await _service.GetMaterialList(pagination,ItemId, keyword);
            return Success(data.Count, data);
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(string ItemId, string keyValue)
        {
            try
            {
                await _service.SubmitForm(ItemId, keyValue);
                return await Success("操作成功。","","");
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", "");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteForm(string ItemId, string keyValue)
        {
            try
            {
                await _service.DeleteForm(ItemId, keyValue);
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
