using Chloe;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.Domain.ClassTask;
using WaterCloud.Domain.DingTalkManage;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.InfoManage;
using WaterCloud.Domain.MaterialManage;
using WaterCloud.Domain.PlanManage;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Domain.QualityManage;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Domain.SystemOrganize;

namespace WaterCloud.Service.AutoJob
{
    public class ReturnProduceJob : IJobTask
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IDbContext _context;
        public ReturnProduceJob(IDbContext context)
        {
            _hostingEnvironment = GlobalContext.HostingEnvironment;
            _context = context;
         }
        public async Task<AlwaysResult> Start()
        {
            AlwaysResult obj = new AlwaysResult();
            try
            {
                //产出时间大于24小时的全部取消
                var datetime = DateTime.Now.AddHours(-24);
                //获取未上架的产出记录,只取消成品的
                var outputlog = _context.Query<OutPutInfoEntity>(a => a.F_IsTemp == true && a.F_MaterialType == 2 && a.F_BandingTime <= datetime).ToList();
                if (outputlog.Count()>0)
                {
                    foreach (var item in outputlog)
                    {
                        OutputMaterialCancle(item.F_TransferBoxCode);
                    }
                }
                obj.state = ResultType.success.ToString();
                obj.message = "定时生产退回成功！";
            }
            catch (Exception ex)
            {
                obj.state = ResultType.error.ToString();
                obj.message = "定时生产退回失败！" + ex.Message;
            }
            return obj;
        }
        public void OutputMaterialCancle(string code)
        {
            var datetime = DateTime.Now.AddHours(-24);
            var check = _context.Query<NeedCheckEntity>(a => a.F_TransferBoxCode == code && a.F_IsCheck == false).FirstOrDefault();
            var temps = _context.Query<OutPutInfoEntity>(a => a.F_TransferBoxCode == code && a.F_IsTemp == true).ToList();
            //质检物料跳过检验
            if (check == null && (temps[0].F_CreatorTime == null || temps[0].F_CreatorTime <= datetime))
            {
                if (temps == null || temps.Count() == 0)
                    throw new Exception($"流转箱编号{code}不存在");
                var material = _context.Query<MaterialEntity>(a => a.F_Id == temps[0].F_MaterialId).FirstOrDefault();
                if (material == null)
                    throw new Exception($"物料{temps[0].F_MaterialCode}不存在，请重试");
                int resultId = 0;
                float tempNum = temps.Sum(a => a.F_Num) ?? 0;
                List<long> tempids = temps.Select(a => a.F_Id).ToList();
                var scanpreturns = _context.Query<ScapMaterialReturnEntity>(a => a.F_TransferBoxCode == code && a.F_IsProduce == true && a.F_MaterialId == material.F_Id).Sum(a => a.F_Num) ?? 0;
                //回退工单作业数量
                var order = _context.Query<WorkOrderDetailEntity>(a => a.F_Id == temps[0].F_WorkOrderDetailId).FirstOrDefault();
                resultId = _context.Update<WorkOrderDetailEntity>(a => a.F_Id == temps[0].F_WorkOrderDetailId && a.F_DoneNum - tempNum >= 0, a => new WorkOrderDetailEntity
                {
                    F_DoneNum = a.F_DoneNum - tempNum,
                });
                if (resultId <= 0)
                {
                    throw new Exception("数量异常，请重新提交");
                }
                var workinote = _context.QueryByKey<WorkOrderEntity>(order.F_WorkOrderId);
                //更新工单数量
                if (workinote.F_MaterialId == order.F_MaterialId)
                {
                    _context.Update<WorkOrderEntity>(a => a.F_Id == workinote.F_Id, a => new WorkOrderEntity
                    {
                        F_DoneNum = a.F_DoneNum - tempNum
                    });
                    if (!string.IsNullOrEmpty(workinote.F_WorkPlanId))
                    {
                        _context.Update<WorkPlanEntity>(a => a.F_Id == workinote.F_WorkPlanId, a => new WorkPlanEntity
                        {
                            F_DoneNum = a.F_DoneNum - tempNum
                        });
                    }
                }
                //更新使用记录
                var consumes = _context.Query<ConsumeInfoEntity>(a => tempids.Contains(a.F_OutPutId ?? 0)).ToList();
                foreach (var item in consumes)
                {
                    var consumenum = item.F_Num;
                    var uses = _context.Query<EqpMaterialUseEntity>(a => a.F_EqpId == item.F_EqpId && a.F_MaterialId == item.F_MaterialId && a.F_MaterialBatch == item.F_MaterialBatch && a.F_TransferBoxCode == item.F_TransferBoxCode).OrderByDesc(a => a.F_CreatorTime).ToList();
					foreach (var eqpmaterialuse in uses)
					{
						if (eqpmaterialuse.F_DoneNum >= consumenum)
						{
                            resultId = _context.Update<EqpMaterialUseEntity>(a => a.F_Id == eqpmaterialuse.F_Id && a.F_DoneNum >= consumenum, a => new EqpMaterialUseEntity
                            {
                                F_DoneNum = a.F_DoneNum - consumenum
                            });
                            if (resultId == 0)
                            {
                                throw new Exception("使用数量已变更，请重试");
                            }
                            consumenum = 0;
                            break;
                        }
						else
						{
                            resultId = _context.Update<EqpMaterialUseEntity>(a => a.F_Id == eqpmaterialuse.F_Id && a.F_DoneNum == eqpmaterialuse.F_DoneNum, a => new EqpMaterialUseEntity
                            {
                                F_DoneNum = 0
                            });
                            if (resultId == 0)
                            {
                                throw new Exception("使用数量已变更，请重试");
                            }
                            consumenum = consumenum- eqpmaterialuse.F_DoneNum;
                        }
                    }
					if (consumenum !=0 )
					{
                        throw new Exception("领用数量不足消耗，请检查");
                    }
                    _context.Update<TransferBoxEntity>(a => a.F_TransferCode == item.F_TransferBoxCode, a => new TransferBoxEntity
                    {
                        F_TransferState = 3,
                    });
                }
                //删除消耗记录
                resultId = _context.Delete<ConsumeInfoEntity>(a => tempids.Contains(a.F_OutPutId ?? 0));
                if (resultId != consumes.Count())
                {
                    throw new Exception("数量异常，请重试");
                }
                //删除产出记录
                resultId = _context.Delete<OutPutInfoEntity>(a => tempids.Contains(a.F_Id) && a.F_IsTemp == true);
                if (resultId != tempids.Count)
                {
                    throw new Exception("数量异常，请重试");
                }
                //删除检验单
                var checks = _context.Query<NeedCheckEntity>(a => a.F_TransferBoxCode == code && a.F_CheckType == 2 && a.F_MaterialBatch == temps[0].F_MaterialBatch && a.F_EqpId == temps[0].F_EqpId).FirstOrDefault();
                if (checks != null)
                {
                    var badnum = _context.Query<ScapMaterialEntity>(a => a.F_CheckId == checks.F_Id && a.F_IsCheck == true).Sum(a => a.F_ScapNum) ?? 0;
                    _context.Delete<ScapMaterialEntity>(a => a.F_CheckId == checks.F_Id);
                    _context.Delete<NeedCheckEntity>(a => a.F_Id == checks.F_Id);
                    // 回退工单作业数量
                    if (badnum > 0)
                    {
                        resultId = _context.Update<WorkOrderDetailEntity>(a => a.F_Id == temps[0].F_WorkOrderDetailId && a.F_BadNum >= badnum, a => new WorkOrderDetailEntity
                        {
                            F_BadNum = a.F_BadNum - badnum,
                        });
                        if (resultId <= 0)
                        {
                            throw new Exception("数量异常，请重新提交");
                        }
                        //更新工单数量
                        if (workinote.F_MaterialId == order.F_MaterialId)
                        {
                            _context.Update<WorkOrderEntity>(a => a.F_Id == workinote.F_Id, a => new WorkOrderEntity
                            {
                                F_BadNum = a.F_BadNum - badnum
                            });
                        }
                    }
                }
                //关闭调度任务
                var job = _context.Query<ControlJobEntity>(a => a.F_NeedId == code && a.F_NeedEqpId == temps[0].F_EqpId && (a.F_JobType == 3 || a.F_JobType == 8) && a.F_JobState < 2).FirstOrDefault();
                if (job != null)
                {
                    CloseJob(job.F_Id);
                }
                //更新流转箱状态
                if (scanpreturns == 0)
                {
                    resultId = _context.Update<TransferBoxEntity>(a => a.F_TransferCode == code && a.F_TransferState == 1, a => new TransferBoxEntity
                    {
                        F_TransferState = 0,
                    });
                    if (resultId <= 0)
                    {
                        throw new Exception("流转箱异常，请重新提交");
                    }
                }
            }
        }
        public void CloseJob(string keyValue)
        {
            _context.Update<ControlJobEntity>(a => a.F_Id == keyValue && a.F_FinishTime == null && a.F_JobState < 2, a => new ControlJobEntity
            {
                F_JobState = 3,
                F_FinishTime = DateTime.Now,
            });
            var entity = _context.QueryByKey<ControlJobEntity>(keyValue);
            var msg = _context.Query<MessageEntity>(a => a.F_KeyValue == keyValue).FirstOrDefault();
            if (msg != null)
            {
                ReadMsgForm(msg.F_Id);
                //创建完成信息
                MessageEntity remsg = new MessageEntity();
                remsg.F_MessageType = 0;
                remsg.F_MessageInfo = entity.F_JobInfo + "--任务已关闭";
                var module = _context.Query<ModuleEntity>(a => a.F_EnCode == "DoneControlJob").FirstOrDefault();
                remsg.F_Href = module.F_UrlAddress;
                remsg.F_HrefTarget = module.F_Target;
                remsg.F_ClickRead = true;
                remsg.F_ToUserId = msg.F_ToUserId;
                remsg.F_ToUserName = msg.F_ToUserName;
                SubmitForm(remsg);
            }
        }
        public void ReadMsgForm(long keyValue)
        {
            MessageHistoryEntity msghis = new MessageHistoryEntity();
            msghis.F_Id = 0;
            msghis.F_CreatorUserId = GlobalContext.SystemConfig.SysemUserId;
            msghis.F_CreatorTime = DateTime.Now;
            msghis.F_CreatorUserName = GlobalContext.SystemConfig.SysemUserCode;
            msghis.F_MessageId = keyValue;
            _context.Insert(msghis);
        }
        public void SubmitForm(MessageEntity entity)
        {
            entity.F_Id = 0;
            entity.F_EnabledMark = true;
            entity.F_CreatorUserId = GlobalContext.SystemConfig.SysemUserId;
            entity.F_CreatorTime = DateTime.Now;
            entity.F_CreatorUserName = GlobalContext.SystemConfig.SysemUserCode;
            MessageEntity messageEntity = new MessageEntity();
            if (string.IsNullOrEmpty(entity.F_ToUserId))
            {
                entity.F_ToUserName = "所有人";
                entity.F_ToUserId = "";
                messageEntity = _context.Insert(entity);
            }
            else
            {
                var users = entity.F_ToUserId.Split(",").ToList();
                entity.F_ToUserName = string.Join(",", _context.Query<UserEntity>(a => users.Contains(a.F_Id)).Select(a => a.F_RealName).ToList());
                messageEntity = _context.Insert(entity);
            }
        }
    }
}
