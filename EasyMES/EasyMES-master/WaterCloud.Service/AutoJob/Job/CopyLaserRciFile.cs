using Chloe;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.Service.SystemSecurity;

namespace WaterCloud.Service.AutoJob
{
    public class CopyLaserRciFile : IJobTask
    {
        private IWebHostEnvironment _hostingEnvironment;
        public CopyLaserRciFile(IDbContext context)
        {
            _hostingEnvironment = GlobalContext.HostingEnvironment;

        }
        public async Task<AlwaysResult> Start()
        {
            AlwaysResult obj = new AlwaysResult();
            obj.state = ResultType.success.ToString();
            obj.message = "无需复制RCI文件！";

            string fromname = "s1_to_m1.dat";
            string toname = "m1_to_s1.dat";
            string path = "E:\\rci";
            try
            {
                if (!FileHelper.IsExistFile(path + "\\通快测试数据\\" + fromname))
                {
                    return obj;
                }

                for (int i = 1; i < 8; i++)
                {
                    if (FileHelper.IsExistFile(path + "\\" + i.ToString() + "\\" + fromname))
                    {
                        continue;
                    }
                    else
                    {
                        string curToFile = path + "\\" + i.ToString() + "\\" + toname;
                        string curFromFile = path + "\\通快测试数据\\" + fromname;

                        if (FileHelper.IsExistFile(curToFile))
                        {
                            TimeSpan span = DateTime.Now - File.GetLastWriteTime(curToFile);
                            if (span.TotalMinutes > 5)
                            {
                                File.Delete(curToFile);
                                File.Copy(curFromFile, path + "\\" + i.ToString() + "\\" + fromname);
                            }
                        }
                        else
                        {
                            File.Copy(curFromFile, path + "\\" + i.ToString() + "\\" + fromname);
                        }
                    }
                }

                obj.state = ResultType.success.ToString();
                obj.message = "复制Rci文件成功！";


            }
            catch (Exception ex)
            {
                obj.state = ResultType.error.ToString();
                obj.message = "服务器状态更新失败！" + ex.Message;
            }
            return obj;
        }
    }
}
