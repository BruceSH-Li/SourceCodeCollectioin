using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace WaterCloud.Code
{
    public class Utils
    {
        #region 自动生成编号
        /// <summary>
        /// 表示全局唯一标识符 (GUID)。
        /// </summary>
        /// <returns></returns>
        public static string GuId()
        {
            return IDGen.NextID().ToString();
        }
        /// <summary>
        /// 自动生成编号  201008251145409865
        /// </summary>
        /// <returns></returns>
        public static string CreateNo()
        {
            Random random = new Random();
            string strRandom = random.Next(1000, 10000).ToString(); //生成编号 
            string code = DateTime.Now.ToString("yyyyMMddHHmmss") + strRandom;//形如
            return code;
        }
        #endregion
        public static string GetGuid()
        {
            return IDGen.NextID().ToString().Replace("-", string.Empty).ToLower();
        }
        /// <summary>
        /// Signature 签名算法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HmacSign(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(hashmessage));
            }
        }
        /// <summary>
        /// 获取时间戳（毫秒）
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        /// <summary>
        /// 将时间戳转换为日期类型，并格式化
        /// </summary>
        /// <param name="longDateTime"></param>
        /// <returns></returns>
        public static DateTime? LongDateTimeToDateTimeString(string longDateTime)
        {
            //用来格式化long类型时间的,声明的变量
            long unixDate;
            DateTime start;
            DateTime date;
            start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //ENd
            if (string.IsNullOrEmpty(longDateTime))
            {
                return null;
            }
            unixDate = long.Parse(longDateTime);
            date = start.AddMilliseconds(unixDate).ToLocalTime();

            return date;

        }
        public static double Round(double d, int i)
        {
            if (d >= 0)
            {
                d += 5 * Math.Pow(10, -(i + 1));
            }
            else
            {
                d += -5 * Math.Pow(10, -(i + 1));
            }
            string str = d.ToString();
            string[] strs = str.Split('.');
            int idot = str.IndexOf('.');
            string prestr = strs[0];
            string poststr = strs.Length>1? strs[1] : "";
            if (poststr.Length > i)
            {
                poststr = str.Substring(idot + 1, i);
            }
            string strd = prestr + "." + poststr;
            d = Double.Parse(strd);
            return d;
        }
    }
}
