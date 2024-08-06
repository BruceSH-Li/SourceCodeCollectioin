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
    public class ScapMaterialReturnController :  BaseController
    {
        public ScapMaterialService _service {get;set;}

        #region 获取数据
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> GetGridJson(SoulPage<ScapMaterialReturnEntity> pagination, string keyword, int timetype = 3)
        {
            if (string.IsNullOrEmpty(pagination.field))
            {
                pagination.field = "F_CreatorTime";
                pagination.order = "desc";
            }
            var data = await _service.GetLookReturnList(pagination, timetype, keyword);
            return Content(pagination.setData(data).ToJson());
        }
        #endregion
    }
}
