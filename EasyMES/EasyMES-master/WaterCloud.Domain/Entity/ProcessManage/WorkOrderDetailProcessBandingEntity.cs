using Chloe.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCloud.Domain.ProcessManage
{
    [TableAttribute("mes_WorkDetailProcessBanding")]
    public class WorkOrderDetailProcessBandingEntity
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 工单明细Id
        /// </summary>
        /// <returns></returns>
        public string F_WorkOrderDetailId { get; set; }
        /// <summary>
        /// 工序ID
        /// </summary>
        public string F_ProcessId { get; set; }
    }
}
