using Chloe;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.Domain.InfoManage;
using WaterCloud.Domain.SystemSecurity;
using WaterCloud.Service.SystemSecurity;

namespace WaterCloud.Service.AutoJob
{
    public class DeleteMessageJob: IJobTask
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IDbContext _context;
        public DeleteMessageJob(IDbContext context)
        {
            _hostingEnvironment = GlobalContext.HostingEnvironment;
            _context = context;
         }
        public async Task<AlwaysResult> Start()
        {
            AlwaysResult obj = new AlwaysResult();
            try
            {
                var time = DateTime.Now.Date.AddDays(-1);
                _context.Delete<MessageEntity>(a => a.F_CreatorTime <= time);
                _context.Delete<MessageHistoryEntity>(a => a.F_CreatorTime <= time);
                obj.state = ResultType.success.ToString();
                obj.message = "删除通知信息成功！";
            }
            catch (Exception ex)
            {
                obj.state = ResultType.error.ToString();
                obj.message = "删除通知信息成功！" + ex.Message;
            }
            return obj;
        }
    }
}
