using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WaterCloud.Code;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Service;
using WaterCloud.Service.MaterialManage;

namespace WaterCloud.Web.Areas.MaterialManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-14 12:44
    /// 描 述：流转箱管理控制器类
    /// </summary>
    [Area("MaterialManage")]
    public class TransferBoxController :  BaseController
    {
        public TransferBoxService _service {get;set;}

        [HttpGet]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public virtual ActionResult Import()
        {
            return View();
        }
        #region 获取数据
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<TransferBoxEntity> pagination, string keyword,bool IsTemp=true)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_TransferState,F_TransferCode";
                pagination.order = "asc";
            }
            var data = await _service.GetLookList(pagination,keyword,IsTemp);
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
        public async Task<ActionResult> GetUseListJson(string keyword)
        {
            var data = await _service.GetList(keyword);
            data = data.Where(a => a.F_TransferState == 0).ToList();
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
        public async Task<ActionResult> SubmitForm(TransferBoxEntity entity, string keyValue)
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
        public async Task<ActionResult> ReturnForm(string keyValue)
        {
            try
            {
                await _service.ReturnForm(keyValue);
                return await Success("操作成功。", "", keyValue,DbLogType.Submit);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Submit);
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> CheckFile()
        {
            try
            {
                //获取文件参数，创建临时文件，使用完成就删除
                var files = HttpContext.Request.Form.Files;
                long size = files.Sum(f => f.Length);
                if (size > 104857600)
                {
                    throw new Exception("文件大小必须小于100M");
                }
                var file = files.FirstOrDefault();
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new Exception("文件不存在");
                }
                if (!FileHelper.IsExcel(fileName))
                {
                    throw new Exception("请上传Excel");
                }
                string filePath = GlobalContext.HostingEnvironment.WebRootPath + $@"/" + "file" + $@"/";
                fileName = Utils.CreateNo() + fileName.Substring(fileName.LastIndexOf("."));
                string fileFullName = filePath + fileName;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                using (FileStream fs = System.IO.File.Create(fileFullName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                var data = await _service.CheckFile(fileFullName);
                return Content(new { code = 0, msg = "操作成功", data = data }.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new { code = 400, msg = "操作失败," + ex.Message }.ToJson());
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> ImportForm(string listData)
        {
            var filterList = JsonConvert.DeserializeObject<List<TransferBoxEntity>>(listData);
            if (filterList == null || filterList.Count == 0)
            {
                return Error("导入数据不存在!");
            }
            if (filterList.Where(a => a.F_EnabledMark == false).Count() > 0)
            {
                return Error("导入数据存在错误!");
            }
            try
            {
                await _service.ImportForm(filterList);
                return await Success("导入成功。", "", "");
            }
            catch (Exception ex)
            {
                return await Error("导入失败，" + ex.Message, "", "");
            }
        }
        [HttpGet]
        public async Task<FileResult> Download()
        {
            string fileName = "流转箱导入模板.xlsx";
            string fileValue = "model";
            string filePath = GlobalContext.HostingEnvironment.WebRootPath + $@"/" + fileValue + $@"/" + fileName;
            if (!FileHelper.IsExistFile(filePath))
            {
                throw new Exception("文件不存在");
            }
            ///定义并实例化一个内存流，以存放图片的字节数组。
            MemoryStream ms = new MemoryStream();
            ///图片读入FileStream
            FileStream f = new FileStream(filePath, FileMode.Open);
            ///把FileStream写入MemoryStream
            ms.SetLength(f.Length);
            f.Read(ms.GetBuffer(), 0, (int)f.Length);
            ms.Flush();
            f.Close();
            var contentType = MimeMapping.GetMimeMapping(fileName);
            return File(ms, contentType, fileName);
        }
        #endregion
    }
}
