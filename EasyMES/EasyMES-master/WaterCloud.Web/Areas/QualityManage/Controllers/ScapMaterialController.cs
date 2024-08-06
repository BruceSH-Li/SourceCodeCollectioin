using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.QualityManage;
using WaterCloud.Service;
using Microsoft.AspNetCore.Authorization;
using WaterCloud.Service.QualityManage;
using System.IO;

namespace WaterCloud.Web.Areas.QualityManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-05-21 13:57
    /// 描 述：不良物料管理控制器类
    /// </summary>
    [Area("QualityManage")]
    public class ScapMaterialController :  BaseController
    {
        public ScapMaterialService _service {get;set;}

        #region 获取数据
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<ScapMaterialEntity> pagination, string keyword, int timetype = 3)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "desc";
            }
            var data = await _service.GetLookList(pagination, timetype, keyword);
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
        public async Task<ActionResult> GetFormJson(long keyValue)
        {
            var data = await _service.GetLookForm(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据

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
        /// <summary>
        /// 图片转存至服务器
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<dynamic> PhotoSaveIndex()
        {
            try
            {
                var cd = DateTime.Now;
                var BasePath = GlobalContext.HostingEnvironment.ContentRootPath;
                var SavePath = Path.Combine("upload", "Quality");
                string fullPath = Path.Combine(BasePath, SavePath);
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
                var formfiles = Request.Form.Files;
                List<string> files = new List<string>();
                foreach (var formfile in formfiles)
                {
                    var fullFileName = formfile.FileName;
                    var fileExtension = Path.GetExtension(fullFileName);
                    var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + new Random().Next(100000, 999999) + fileExtension;
                    using (var fs = System.IO.File.Create(Path.Combine(fullPath, filename)))
                    {
                        await formfile.CopyToAsync(fs);
                        fs.Flush();
                    }
                    files.Add($@"/upload/Quality/" +filename);
                    try
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(Path.Combine(fullPath, filename));
                    }
                    catch (Exception ex)
                    {
                        FileHelper.DeleteFile(Path.Combine(fullPath, filename));
                        throw new Exception("文件非法，请检查！");
                    }
                }
                dynamic result = new { error = 0, files = files.ToArray() };

                return result;
            }
            catch (Exception ex)
            {
                return new AlwaysResultEx { error = (int)errorCodeType.error, message = LogHelper.ExMsgFormat(ex.Message) };
            }
        }
        #endregion
    }
}
