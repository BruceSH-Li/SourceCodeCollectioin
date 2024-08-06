using Chloe;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Domain.QualityManage;
using WaterCloud.Domain.ReportRecord;
using WaterCloud.Service.ReportRecord;
using WaterCloud.Service.SystemManage;

namespace WaterCloud.Service.AutoJob
{
    public class CheckDateStorageJob : IJobTask
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IDbContext _context;
		private ItemsDataService itemsApp;
		public CheckDateStorageJob(IDbContext context)
        {
            _hostingEnvironment = GlobalContext.HostingEnvironment;
            _context = context;
			itemsApp = new ItemsDataService(context);
		}
        public async Task<AlwaysResult> Start()
        {
            AlwaysResult obj = new AlwaysResult();
            try
            {
                DateTime checkdate = DateTime.Now.Date;
                DateTime starttime = DateTime.Now.Date;
                DateTime endtime = DateTime.Now.Date;
                string classNum = "";
				var classNums = await itemsApp.GetItemList("Mes_ClassNumber");
				var classStartTime = TimeSpan.Parse(classNums.FirstOrDefault().F_Description.Split("-")[0]);
				var tempStartTime = classStartTime.TotalMinutes;
				var classEndTime = TimeSpan.Parse(classNums.FirstOrDefault().F_Description.Split("-")[1]);
				var tempEndTime = classEndTime.TotalMinutes;
                var selectClass = 0;
                if (classNums.Count() == 1)
                {
					checkdate = checkdate.AddDays(-1);
					starttime = checkdate.AddMinutes(tempStartTime);
					endtime = checkdate.AddMinutes(tempEndTime);
				}
                else
                {
					tempEndTime = tempStartTime;
					for (int i = 0; i < classNums.Count(); i++)
					{
						var startTime = TimeSpan.Parse(classNums[i].F_Description.Split("-")[0]).TotalMinutes;
						var endTime = TimeSpan.Parse(classNums[i].F_Description.Split("-")[1]).TotalMinutes;
						if (endTime > startTime)
						{
							tempEndTime += endTime - startTime;
						}
						else
						{
							tempEndTime += endTime + 24 * 60 - startTime;
						}
						if (checkdate.AddMinutes(tempStartTime) < DateTime.Now && checkdate.AddMinutes(tempEndTime) >= DateTime.Now)
						{
							selectClass = i - 1;
						}
						tempStartTime = tempEndTime;
					}
					if (selectClass < 0)
					{
						selectClass = classNums.Count() - 1;
					}
					for (int i = 0; i < classNums.Count(); i++)
					{
						var startTime = TimeSpan.Parse(classNums[i].F_Description.Split("-")[0]).TotalMinutes;
						var endTime = TimeSpan.Parse(classNums[i].F_Description.Split("-")[1]).TotalMinutes;
						if (endTime > startTime)
						{
							tempEndTime += endTime - startTime;
						}
						else
						{
							tempEndTime += endTime + 24 * 60 - startTime;
						}
						if (i == selectClass)
						{
							starttime = checkdate.AddMinutes(tempStartTime);
							endtime = checkdate.AddMinutes(tempEndTime);
							if (starttime > DateTime.Now)
							{
								checkdate = DateTime.Now.Date.AddDays(-1);
								starttime = starttime.AddDays(-1);
								endtime = endtime.AddDays(-1);
							}
							break;
						}
						tempStartTime = tempEndTime;
					}
				}
                List<CheckDateStorageEntity> list = new List<CheckDateStorageEntity>();
                var materials = _context.Query<MaterialEntity>(a => a.F_EnabledMark == true && a.F_DeleteMark == false).ToList();
                foreach (var item in materials)
                {
                    var storage = new CheckDateStorageEntity();
                    var check = new CheckDateStorageEntity();
                    storage.F_CurrentNum = _context.Query<StorageEntity>(a => a.F_MaterialId == item.F_Id && a.F_IsCheckout != false).Sum(a=>a.F_Num) ?? 0;
                    storage.F_CurrentPallet = _context.Query<StorageEntity>(a => a.F_MaterialId == item.F_Id && a.F_IsCheckout != false).GroupBy(a=>a.F_TransferBoxCode).Select(a=>a.F_TransferBoxCode).Count();
                    //入库数量
                    //入库
                    var instorage = _context.Query<InStorageInfoEntity>(a => a.F_MaterialId == item.F_Id && a.F_CreatorTime >= starttime && a.F_CreatorTime < endtime && a.F_IsTemp!=true).Sum(a=>a.F_Num) ?? 0;
                    //产出
                    var output = _context.Query<OutPutInfoEntity>(a => a.F_MaterialId == item.F_Id && a.F_CreatorTime >= starttime && a.F_CreatorTime < endtime && a.F_IsTemp != true).Sum(a => a.F_Num) ?? 0;
                    //质检退回(只有库存会退回)
                    var checkreturns = _context.Query<NeedCheckEntity>(a => a.F_MaterialId == item.F_Id && a.F_FinishTime >= starttime && a.F_FinishTime < endtime && a.F_CheckType == 3 ).ToList();
                    foreach (var checkreturn in checkreturns)
                    {
                        var scapnum = _context.Query<ScapMaterialEntity>(a => a.F_CheckId == checkreturn.F_Id).Sum(a => a.F_Num) ?? 0;
                        checkreturn.F_Num -= scapnum;
                    }
                    float checkreturnnum = checkreturns.Sum(a=>a.F_Num)??0;
                    //可疑退回
                    var twocheckreturn = _context.Query<ScapMaterialReturnEntity>(a => a.F_MaterialId == item.F_Id && a.F_CreatorTime >= starttime && a.F_CreatorTime < endtime).Sum(a => a.F_Num) ?? 0;
                    //领料退回
                    var usereturn = _context.Query<ReturnMaterialEntity>(a => a.F_MaterialId == item.F_Id && a.F_CreatorTime >= starttime && a.F_CreatorTime < endtime).Sum(a => a.F_Num)??0;
                    //数量变动
                    var changeup = _context.Query<StorageChangeInfoEntity>(a => a.F_NewTransferBoxCode == null && a.F_NewLocationCode == null && a.F_Num < 0 && a.F_CreatorTime >= starttime && a.F_CreatorTime < endtime).Sum(a=>a.F_Num) ?? 0;
                    storage.F_InNum = instorage + output + checkreturnnum + twocheckreturn + usereturn - changeup;

                    //出库数量
                    //出库
                    var outstorage = _context.Query<OutStorageInfoEntity>(a => a.F_MaterialId == item.F_Id && a.F_BandingTime >= starttime && a.F_IsTemp != true && a.F_BandingTime < endtime).Sum(a => a.F_Num) ?? 0;
                    //领用
                    var materialuse = _context.Query<EqpMaterialUseEntity>(a => a.F_MaterialId == item.F_Id && a.F_CreatorTime >= starttime && a.F_CreatorTime < endtime).Sum(a => a.F_Num) ?? 0;
                    //可疑(排除领用可疑料)
                    var twochecknum = _context.Query<ScapMaterialEntity>(a => a.F_MaterialId == item.F_Id && a.F_CheckType!=1 && a.F_CreatorTime >= starttime && a.F_IsTemp != true && a.F_CreatorTime < endtime).Sum(a => a.F_Num) ?? 0;
                    //质检
                    var checknum = _context.Query<StorageEntity>(a => a.F_MaterialId == item.F_Id && a.F_IsCheckout == false)
                        .InnerJoin<NeedCheckEntity>((a,b)=> a.F_MaterialId == b.F_MaterialId && a.F_TransferBoxCode == b.F_TransferBoxCode && b.F_IsCheck == false && b.F_CreatorTime >= starttime && a.F_CreatorTime < endtime)
                        .Select((a,b)=>a).Sum(a=>a.F_Num)??0;

                    //数量变动
                    var changedown=_context.Query<StorageChangeInfoEntity>(a => a.F_NewTransferBoxCode == null && a.F_NewLocationCode == null && a.F_Num > 0&&a.F_MaterialId == item.F_Id && a.F_CreatorTime >= starttime && a.F_CreatorTime < endtime).Sum(a => a.F_Num) ?? 0;
                    //领用未使用
                    var useundone = _context.Query<EqpMaterialUseEntity>(a => a.F_MaterialId == item.F_Id && a.F_DoneNum!=a.F_Num).Sum(a => a.F_Num-a.F_DoneNum) ?? 0;
                    //未上架数量
                    var outundone = _context.Query<OutPutInfoEntity>(a => a.F_MaterialId == item.F_Id && a.F_IsTemp == true).Sum(a => a.F_Num) ?? 0;
                    //未上架托数
                    var outundonepallet = _context.Query<OutPutInfoEntity>(a => a.F_MaterialId == item.F_Id && a.F_IsTemp == true).Count();
                    storage.F_CheckDate = checkdate;
                    storage.F_ClassNum = classNum;
                    storage.F_OutNum = outstorage + materialuse + checknum + twochecknum + changedown;
                    storage.F_BeginNum = storage.F_CurrentNum + storage.F_OutNum - storage.F_InNum;
                    storage.F_CheckStorageType = 0;
                    storage.F_Id = Utils.GuId();
                    storage.F_MaterialId = item.F_Id;
                    storage.F_MaterialCode = item.F_MaterialCode;
                    storage.F_MaterialName = item.F_MaterialName;
                    storage.F_MaterialType = item.F_MaterialType;
                    storage.F_MaterialUnit = item.F_MaterialUnit;
                    storage.F_Description = $"入-入库数量:{instorage},产出数量:{output}，质检退回:{checkreturnnum}，可疑退回:{twocheckreturn}，领用退回:{usereturn}，数量变动:{0 - changeup};出-出库数量:{outstorage}，领用数量:{materialuse}，质检数量:{checknum}，可疑数量:{twochecknum}，数量变动:{changedown};领用未使用数量:{useundone};产出未上架数量:{outundone}/{outundonepallet}。";
                    list.Add(storage);
                    check.F_CheckDate = checkdate;
                    check.F_ClassNum = classNum;
                    check.F_CheckStorageType = 1;
                    check.F_Id = Utils.GuId();
                    check.F_MaterialId = item.F_Id;
                    check.F_MaterialCode = item.F_MaterialCode;
                    check.F_MaterialName = item.F_MaterialName;
                    check.F_MaterialType = item.F_MaterialType;
                    check.F_MaterialUnit = item.F_MaterialUnit;
                    check.F_CurrentPallet = 0;
                    check.F_CurrentNum = _context.Query<ScapMaterialEntity>(a => a.F_MaterialId == item.F_Id && a.F_IsTemp != true).InnerJoin<NeedCheckEntity>((a,b)=>a.F_CheckId==b.F_Id && b.F_FinishTime < endtime)
                        .Select((a,b)=>a).Sum(a=>a.F_Num)??0;
                    check.F_InNum = _context.Query<ScapMaterialEntity>(a => a.F_MaterialId == item.F_Id && a.F_IsTemp != true && a.F_CreatorTime >= starttime && a.F_CreatorTime < endtime)
                        .InnerJoin<NeedCheckEntity>((a, b) => a.F_CheckId == b.F_Id && b.F_FinishTime < endtime)
                        .Select((a, b) => a).Sum(a => a.F_Num) ?? 0;
                    check.F_OutNum = twocheckreturn;
                    check.F_BeginNum = check.F_CurrentNum - check.F_InNum + check.F_OutNum;
                    var uncheckNum = _context.Query<NeedCheckEntity>(a => a.F_IsCheck == false && a.F_MaterialId == item.F_Id).Sum(a=>a.F_Num)??0;
                    var uncheck = _context.Query<NeedCheckEntity>(a => a.F_IsCheck == false && a.F_MaterialId == item.F_Id).Count();
                    check.F_Description = $"入-可疑数量:{check.F_InNum};出-退回数量:{twocheckreturn};待检验数量:{uncheckNum}/{uncheck}";
                    list.Add(check);
                }
                _context.InsertRange<CheckDateStorageEntity>(list);
                obj.state = ResultType.success.ToString();
                obj.message = "盘点数据更新成功！";
            }
            catch (Exception ex)
            {
                obj.state = ResultType.error.ToString();
                obj.message = "盘点数据更新失败！" + ex.Message;
            }
            return obj;
        }
    }
}
