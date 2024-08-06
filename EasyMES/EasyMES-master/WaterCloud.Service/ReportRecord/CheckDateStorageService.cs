using Chloe;
using WaterCloud.Service.SystemManage;
using WaterCloud.Domain.ReportRecord;
using WaterCloud.Code;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace WaterCloud.Service.ReportRecord
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-16 13:37
    /// 描 述：盘点服务类
    /// </summary>
    public class CheckDateStorageService : DataFilterService<CheckDateStorageEntity>, IDenpendency
    {
        private ItemsDataService itemsApp;
        public CheckDateStorageService(IDbContext context) : base(context)
        {
            itemsApp = new ItemsDataService(context);
        }
        public async Task<List<CheckDateStorageEntity>> GetList(SoulPage<CheckDateStorageEntity> pagination,string datetime, int checktype, string keyword)
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            var setList = await itemsApp.GetItemList("Mes_MaterialType");
            Dictionary<string, string> stateTemp = new Dictionary<string, string>();
            foreach (var item in setList)
            {
                stateTemp.Add(item.F_ItemName, item.F_ItemCode);
            }
            dic.Add("F_MaterialType", stateTemp);
            Dictionary<string, string> isTemp = new Dictionary<string, string>();
            isTemp.Add("库存盘点", "0");
            isTemp.Add("质检盘点", "1");
            dic.Add("F_CheckStorageType", isTemp);
            pagination = ChangeSoulData(dic, pagination);
            var result = new List<CheckDateStorageEntity>();
            DateTime startTime = DateTime.Now.ToString("yyyy-MM-dd").ToDate();
            DateTime endTime = DateTime.Now.ToString("yyyy-MM-dd").ToDate().AddDays(1);
            if (!string.IsNullOrEmpty(datetime))
            {
                startTime = DateTime.Parse(datetime).Date;
                endTime = DateTime.Parse(datetime).Date.AddDays(1);
            }
            var list = uniwork.IQueryable<CheckDateStorageEntity>().Where(t => t.F_CheckDate >= startTime && t.F_CheckDate <= endTime && t.F_CheckStorageType == checktype);
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(u => u.F_MaterialCode.Contains(keyword) || u.F_MaterialName.Contains(keyword));
            }
            list = GetDataPrivilege("u", "", list);
            result = await repository.OrderList(list, pagination);
            return result;
        }

        public async Task RemoveRecord(string keepTime)
        {
            DateTime operateTime = DateTime.Now;
            if (keepTime == "7")            //保留近一周
            {
                operateTime = DateTime.Now.AddDays(-7);
            }
            else if (keepTime == "1")       //保留近一个月
            {
                operateTime = DateTime.Now.AddMonths(-1);
            }
            else if (keepTime == "3")       //保留近三个月
            {
                operateTime = DateTime.Now.AddMonths(-2);
            }
            var expression = ExtLinq.True<CheckDateStorageEntity>();
            expression = expression.And(t => t.F_CheckDate <= operateTime);
            await uniwork.Delete<CheckDateStorageEntity>(expression);
        }
    }
}
