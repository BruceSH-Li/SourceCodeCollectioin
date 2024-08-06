using Chloe;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.DingTalk;
using WaterCloud.Domain.DingTalkManage;
using WaterCloud.Domain.SystemSecurity;
using WaterCloud.Service.SystemSecurity;

namespace WaterCloud.Service.AutoJob
{
    public class ReadDingUserWorkJob : IJobTask
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IDbContext _context;
        public ReadDingUserWorkJob(IDbContext context)
        {
            _hostingEnvironment = GlobalContext.HostingEnvironment;
            _context = context;
         }
        public async Task<AlwaysResult> Start()
        {
            AlwaysResult obj = new AlwaysResult();
            try
            {
                var dingconfig = _context.Query<DingTalkConfigEntity>(). Where(t => t.F_EnabledMark == true && t.F_DeleteMark == false && t.F_IsDingInfo == true).Where(a => a.F_DingType == 0).FirstOrDefault();
                var dingUsers = _context.Query<DingUserEntity>().ToList();
                var dingdingtoken = await CacheHelper.Get<string>(dingconfig.F_AgentId);
                if (string.IsNullOrEmpty(dingdingtoken))
                {

                    string appKey = dingconfig.F_AppKey;
                    string appSecret = dingconfig.F_AppSecret;
                    dingdingtoken = DingTalkHelper.GetAccessToken2(appKey, appSecret);
                    await CacheHelper.Set(dingconfig.F_AgentId, dingdingtoken, 2, false);
                }
                var list = new List<string>();
                int count = 1;
                var temps= new List<string>();
                foreach (var item in dingUsers.Select(a=>a.F_UserId).Distinct())
				{
                    temps.Add(item);
                    if (count%50 == 0)
					{
                        var listtemp = DingTalkCheckWorkApi.CheckWorkOn(dingdingtoken, item, string.Join(",", temps));
                        if (listtemp != null && listtemp.Count() > 0)
                        {
                            list.AddRange(listtemp);
                        }
                        temps = new List<string>();
                    }
                    count++;
				}
				if (temps.Count>0)
				{
                    var listtemp = DingTalkCheckWorkApi.CheckWorkOn(dingdingtoken, temps.First(), string.Join(",", temps));
                    if (listtemp != null && listtemp.Count() > 0)
                    {
                        list.AddRange(listtemp);
                    }
                }
                _context.Update<DingUserEntity>(a=>a.F_UserId!=null,a => new DingUserEntity
                {
                    F_IsWork=false,
                });
                _context.Update<DingUserEntity>(a => list.Contains(a.F_UserId), a => new DingUserEntity
                {
                    F_IsWork = true
                });
                obj.state = ResultType.success.ToString();
                obj.message = "更新员工在班情况成功！";
            }
            catch (Exception ex)
            {
                obj.state = ResultType.error.ToString();
                obj.message = "更新员工在班情况失败！" + ex.Message;
            }
            return obj;
        }
    }
}
