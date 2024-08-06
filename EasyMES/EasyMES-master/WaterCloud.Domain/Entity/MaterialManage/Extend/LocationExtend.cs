using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-17 08:31
    /// 描 述：库位管理实体类
    /// </summary>
    public class LocationExtend
    {
        public string Prefix { get; set; }
        public List<LocationEntity> list { get; set; }
    }

    public class allLocationEnity
    { 
        public List<LocationGroup1> locationGroup1list { get; set; }
        public List<commonlocation> comlocation { get; set; }
        public List<commonlocation> templocation { get; set; }
    }

    public class commonlocation
    { 
        public string F_MaterialCode { get; set; }
        public string F_MaterialName { get; set;}
        public float? totalsum { get; set; }
        public string firstout { get; set; }
    }




    public class LocationGroup1
    {
        public string groupname { get; set; }
        public List<LocationGroup2> grouplist{get;set;}
    }

    public class LocationGroup2
    { 
        public string groupname { get; set; }
        public List<LocationGroupEntity> list { get; set; }
    }

    public class LocationGroupEntity
    {
        public string F_LocationCode { get; set; }
        public float? F_Num { get; set; }
        public string F_MaterialName { get; set; }
        public string F_MaterialCode { get; set; }
        public string F_MaterialBatch { get; set; }
        public string F_MaterialNamePrefix { get; set; }
        public float? maxnum { get; set; }

    }

}
