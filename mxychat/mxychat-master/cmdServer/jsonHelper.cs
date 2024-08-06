using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace cmdServer
{
    /// <summary>
    /// 在VS中需要添加引用System.Web.Script.Serialization的时候，请先引用System.Web.Extensions
    /// </summary>
    public class JsonHelp
    {
        /// <summary>
        /// json序列化（非二进制方式）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string JsonSerializer<T>(T t)
        {
            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            return jsonSerialize.Serialize(t);
        }

        /// <summary>
        /// json反序列化（非二进制方式）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string jsonString)
        {
            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            return (T)jsonSerialize.Deserialize<T>(jsonString);
        }

    }
}
