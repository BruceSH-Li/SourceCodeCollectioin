using System;

namespace WaterCloud.Domain.ClassTask
{
    public class ControlJobMsg
    {
        /// <summary>
        /// 时间
        /// </summary>
        /// <returns></returns>
        public DateTime? time { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        /// <returns></returns>
        public string msg { get; set; }
       
    }
}
