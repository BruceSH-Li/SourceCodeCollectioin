using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCloud.Code
{
    public class AlwaysResultEx : AlwaysResult
    {
        public object error { get; set; }
        public long insertId { get; set; }

        public object callbackdata { get; set; }
    }

    public class ResultEx: AlwaysResult
    {
        public int code { get; set; }

    }

    public enum errorCodeType
    {

        /// <summary>
        /// 成功结果类型
        /// </summary>
        success=0,
        /// <summary>
        /// 异常结果类型
        /// </summary>
        error=1
    }
}
