using Chloe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.DataBase;
using WaterCloud.Domain.EquipmentManage;
using WaterCloud.Domain.ProcessManage;
using WaterCloud.Domain.ProductionReport;
using WaterCloud.Domain.SystemOrganize;

namespace WaterCloud.Service.ProductionReport
{
    public class HelpClassService : IDenpendency
    {
        // 用户信息
        public OperatorModel currentuser;
        // 用于其他表操作
        protected IRepositoryBase uniwork;

        public HelpClassService(IDbContext context, IHttpClientFactory httpClientFactory)
        {
            currentuser = OperatorProvider.Provider.GetCurrent();
            uniwork = new RepositoryBase(context);
            if (currentuser == null)
            {
                currentuser = new OperatorModel();
            }

        }

        public string getuser()
        {
            var a = uniwork.IQueryable<EquipmentEntity>().FirstOrDefault().F_Id;
            return a;
        }

        public async Task InsertWorkDetailEqpBanding(string EqpId, string TableName, long insertId)
        {
            if (insertId == 0)
                return;
            var entity = uniwork.IQueryable<WorkOrderDetailEqpBandingEntity>(t => t.F_EqpId == EqpId).Select(t => new WorkOrderDetailDataEntity
            {
                F_WorkOrderDetailId = t.F_WorkOrderDetailId,
                F_MouldNum = t.F_MouldNum,
                F_EqpId = t.F_EqpId,
                F_DataTable = TableName,
                F_DataTableId = insertId
            }).ToList();
            if (entity != null && entity.Count > 0)
                await uniwork.Insert(entity);
        }

        public async Task<string> getsysUserIdByUnionId(string UnionId)
        {
            var userId = uniwork.IQueryable<UserEntity>(t => t.F_DingTalkUnionId == UnionId).Select(t => t.F_Id).FirstOrDefault();
            return userId;
        }

        public async Task<bool> IsAdminRole(string UnionId)
        {
            var roleId = uniwork.IQueryable<UserEntity>(t => t.F_DingTalkUnionId == UnionId).Select(t => t.F_RoleId).FirstOrDefault();
            if (!string.IsNullOrEmpty(roleId) && roleId.Contains("c987d51f-2d82-4286-bfb5-bdf679975803"))
            {
                return true;
            }
            return false;
        }





    }
}
