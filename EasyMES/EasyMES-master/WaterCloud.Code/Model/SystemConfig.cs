using System;
using System.Collections.Generic;
using System.Text;

namespace WaterCloud.Code.Model
{
    public class SystemConfig
    {
        /// <summary>
        /// 是否是Demo模式
        /// </summary>
        public bool Demo { get; set; }
        /// <summary>
        /// 是否是调试模式
        /// </summary>
        public bool Debug { get; set; }
        /// <summary>
        /// 允许一个用户在多个电脑同时登录
        /// </summary>
        public bool LoginMultiple { get; set; }
        /// <summary>
        /// 允许跨域的站点
        /// </summary>
        public string AllowCorsSite { get; set; }

        public string DBProvider { get; set; }
        public string DBConnectionString { get; set; }
        public string DBCommandTimeout { get; set; }

        public string CacheProvider { get; set; }
        public string HandleLogProvider { get; set; }
        public string RedisConnectionString { get; set; }
        public string SysemUserId { get; set; }
        public string SysemUserCode { get; set; }
        public string SysemUserPwd { get; set; }
        public string SysemMasterProject { get; set; }
        public string TokenName { get; set; }
        //缓存过期时间
        public int LoginExpire { get; set; }
        public string HomePage { get; set; }
        public bool? LocalLAN { get; set; }
        /// <summary>
        /// 是否重置密码
        /// </summary>
		public bool? ReviseSysem { get; set; }
        /// <summary>
        /// 出库质检
        /// </summary>
		public bool? OutStorageCheck { get; set; }
        //看板密码
		public string BoardPassword { get; set; }

    }
}
