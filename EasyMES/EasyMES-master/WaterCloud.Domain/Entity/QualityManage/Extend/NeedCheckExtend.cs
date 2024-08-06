using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.QualityManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-17 08:31
    /// 描 述：库位管理实体类
    /// </summary>
    public class NeedCheckExtend
    {
        public string CheckType { get; set; }
        public List<NeedCheckEntity> list { get; set; }
    }
}
