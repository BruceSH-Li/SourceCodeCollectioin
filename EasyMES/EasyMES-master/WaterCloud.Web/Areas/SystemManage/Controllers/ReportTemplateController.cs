using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Service;
using Microsoft.AspNetCore.Authorization;
using WaterCloud.Service.SystemManage;
using System.IO;

namespace WaterCloud.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-08-25 16:06
    /// 描 述：报表模板控制器类
    /// </summary>
    [Area("SystemManage")]
    public class ReportTemplateController :  BaseController
    {
        public ReportTemplateService _service {get;set;}

        #region 获取数据
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<ReportTemplateEntity> pagination, string keyword)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "desc";
            }
            var data = await _service.GetLookList(pagination,keyword);
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
        public async Task<ActionResult> SubmitForm(ReportTemplateEntity entity, string keyValue)
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
        [HttpGet]
        [ServiceFilter(typeof(HandlerLoginAttribute))]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> Download(string filePath)
        {
            var data = await _service.GetFileForm(filePath);
            string filepath = GlobalContext.HostingEnvironment.WebRootPath + $@"/" + data.F_FilePath;
            if (!FileHelper.IsExistFile(filepath))
            {
                return Error("文件不存在");
            }
            ///定义并实例化一个内存流，以存放图片的字节数组。
            MemoryStream ms = new MemoryStream();
            ///图片读入FileStream
            FileStream f = new FileStream(filepath, FileMode.Open);
            ///把FileStream写入MemoryStream
            ms.SetLength(f.Length);
            f.Read(ms.GetBuffer(), 0, (int)f.Length);
            ms.Flush();
            f.Close();
            string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + data.F_FileExtension;
            var contentType = MimeMapping.GetMimeMapping(filename);
            return File(ms, contentType, filename);
        }
        #endregion
    }
}
