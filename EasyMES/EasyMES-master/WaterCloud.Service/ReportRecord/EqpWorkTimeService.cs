using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Domain.EquipmentManage;

namespace WaterCloud.Service.ReportRecord
{
	/// <summary>
	/// 创 建：超级管理员
	/// 日 期：2020-12-07 12:14
	/// 描 述：工单管理服务类
	/// </summary>
	public class EqpWorkTimeService : DataFilterService<EqpWorkTimeEntity>, IDenpendency
    {
        public EqpWorkTimeService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<EqpWorkTimeEntity>> GetList(SoulPage<EqpWorkTimeEntity> pagination, DateTime? startTime, DateTime? endTime, string keyword)
        {
            var result = new List<EqpWorkTimeEntity>();
            var list = uniwork.IQueryable<EqpWorkTimeEntity>();
            if (startTime != null)
            {
                list = list.Where(t => t.F_StartTime >= startTime);
            }
            else
            {
                startTime = DateTime.Now.Date.AddDays(-7);
                list = list.Where(t => t.F_StartTime >= startTime);
            }
            if (endTime != null)
            {
                list = list.Where(t => t.F_StartTime <= endTime);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(u => u.F_UserName.Contains(keyword)||u.F_Description.Contains(keyword));
            }
            list = GetDataPrivilege("u", "EqpWorkTime", list);
            result = await repository.OrderList(list, pagination);
            return result;
        }

		public async Task<List<EquipmentEntity>> GetEqpListJson(string keyValue)
		{
            var query = repository.IQueryable(a => a.F_Id.ToString() == keyValue).InnerJoin<WorkDetailTimeBandingEntity>((a, b) => a.F_Id == b.F_EqpWorkTimeId)
                .InnerJoin<WorkOrderDetailEqpBandingEntity>((a, b, c) => b.F_WorkOrderDetailId == c.F_WorkOrderDetailId && c.F_IsMaster == true)
                .InnerJoin<EquipmentEntity>((a, b, c, d) => c.F_EqpId == d.F_Id).Select((a, b, c, d) => d);
            return query.Distinct().OrderBy(a => a.F_EqpName).ToList();
		}

		#endregion


		#region 提交数据
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
            var expression = ExtLinq.True<EqpWorkTimeEntity>();
            expression = expression.And(t => t.F_StartTime <= operateTime);
            await uniwork.Delete<EqpWorkTimeEntity>(expression);
        }
        #endregion

    }
}
