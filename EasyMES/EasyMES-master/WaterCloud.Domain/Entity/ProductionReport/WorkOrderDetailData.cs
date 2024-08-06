using Chloe.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCloud.Domain.ProductionReport
{
    [TableAttribute("mes_WorkOrderDetailData")]
    public class WorkOrderDetailDataEntity
    {
        public string F_WorkOrderDetailId { get; set; }

        public string F_EqpId { get; set; }
        public string F_DataTable { get; set; }
        public long F_DataTableId { get; set; }
        public int? F_MouldNum { get; set; }
    }
}
