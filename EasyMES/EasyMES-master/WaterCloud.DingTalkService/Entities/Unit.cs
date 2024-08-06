using System;

namespace WaterCloud.DingTalk
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public class DingTalkUtils
    {

        // <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  毫秒
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static long DateTimeToTimeStamp(System.DateTime time)
        {
            //System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            //return (long)(time - startTime).TotalSeconds;
            return (time.Ticks - 621355968000000000) / 10000;
        }

        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime TimeStampToDateTime(string timeStamp)
        {
            //用来格式化long类型时间的,声明的变量
            long unixDate;
            DateTime start;
            DateTime date;
            start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //ENd
            if (string.IsNullOrEmpty(timeStamp))
            {
                return start;
            }
            unixDate = long.Parse(timeStamp);
            date = start.AddMilliseconds(unixDate).ToLocalTime();

            return date;
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            return TimeStampToDateTime(timeStamp + "0000000");
        }
    }
}
