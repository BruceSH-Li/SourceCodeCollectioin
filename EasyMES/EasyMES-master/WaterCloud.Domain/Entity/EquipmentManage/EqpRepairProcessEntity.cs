using Chloe.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCloud.Domain.EquipmentManage
{
    [TableAttribute("mes_EqpRepairProcess")]
    public class EqpRepairProcessEntity
    {
        public string F_RepairId { get; set; }

        public DateTime F_AddTime { get; set; }
        public string F_ActionDesc { get; set; }

        [NotMapped]
        public string addtime { get; set; }

    }
}
