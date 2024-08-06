using System;
using System.ComponentModel.DataAnnotations;
using Chloe.Annotations;

namespace WaterCloud.Domain.EquipmentManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-06-08 15:55
    /// 描 述：设备日点检明细表实体类
    /// </summary>
    [TableAttribute("mes_EqpPqcDetail")]
    public class EqpPqcDetailEntity : IEntity<EqpPqcDetailEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_PId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_CheckItemId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day7 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day8 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day9 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day10 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day11 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day12 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day13 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day14 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day15 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day16 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day17 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day18 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day19 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day20 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day21 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day22 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day23 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day24 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day25 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day26 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day27 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day28 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day29 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day30 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day31 { get; set; }

        public string F_CheckItem { get; set; }
        public string F_CheckStand { get; set; }

        
    }

    public class EqpPqcTempEntity
    { 
        public string F_CheckItemId { get; set; }
        public string F_CheckItem { get; set; }
        public string F_CheckMonth { get; set; }
        public string F_CheckStand { get; set; }
        public string F_EqpId { get; set; }
        public string F_Remark { get;set; }
        public string F_Shift { get; set; }
        public string M_F_Id { get; set; }

        public EqpPqcTempDeQEntity c { get; set; }

    }

    public class EqpPqcTempDeQEntity
    { 
        public string F_Id { get; set; }
        public string F_PId { get; set; }
        public string F_CheckItemId { get; set; }
        public string F_CheckItem { get; set; }
        public string F_CheckStand { get; set; }
        public string F_Day1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day7 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day8 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day9 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day10 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day11 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day12 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day13 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day14 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day15 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day16 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day17 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day18 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day19 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day20 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day21 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day22 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day23 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day24 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day25 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day26 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day27 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day28 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day29 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day30 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Day31 { get; set; }
    }
    

}
