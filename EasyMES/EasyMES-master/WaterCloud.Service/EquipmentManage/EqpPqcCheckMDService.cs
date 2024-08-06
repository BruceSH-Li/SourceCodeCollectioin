using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.EquipmentManage;

namespace WaterCloud.Service.EquipmentManage
{
    public class EqpPqcCheckMDService : DataFilterService<EqpPqcMasterEntity>, IDenpendency
    {
        public EqpPqcCheckMDService(IDbContext context) : base(context)
        {
        }

        public async Task<object> GetLookList(string EqpId, string Shiftday, string Month)
        {
            dynamic data = null;
            var code = 0;
            string curMonth = DateTime.Now.ToString("yyyyMM");
            if (string.IsNullOrEmpty(EqpId)) return null;
            if (string.IsNullOrEmpty(Month)) Month = curMonth;

            var item = uniwork.IQueryable<EqpPqcCheckItemEntity>(t => t.F_EqpId == EqpId).FirstOrDefault();
            if (item == null) code = 10;

            if (Month == curMonth)
            {
                var datas = repository.IQueryable(t => t.F_EqpId == EqpId && t.F_Shift == Shiftday && t.F_CheckMonth == Month)
                                    .LeftJoin<EqpPqcDetailEntity>((a, b) => a.F_Id == b.F_PId).Select((a, b) => new { a, b });
                var datat = uniwork.IQueryable<EqpPqcCheckItemEntity>(t => t.F_EqpId == EqpId);

                data = datat.LeftJoin(datas, (m, n) => m.F_Id == n.b.F_CheckItemId).Select((m, n) => new
                {
                    F_CheckItemId=m.F_Id,
                    m.F_CheckItem,
                    m.F_CheckStand,
                    m.F_EqpId,
                    M_F_Id = n.a.F_Id,
                    F_CheckMonth = Month,
                    F_Shift = Shiftday,
                    n.a.F_Remark,
                    c = n.b
                }).ToList();

            }
            else
            {
                data = repository.IQueryable(t => t.F_EqpId == EqpId && t.F_Shift == Shiftday && t.F_CheckMonth == Month)
                   .LeftJoin<EqpPqcDetailEntity>((a, b) => a.F_Id == b.F_PId).Select((a, b) => new
                   {
                       b.F_CheckItemId,
                       b.F_CheckItem,
                       b.F_CheckStand,
                       a.F_EqpId,
                       M_F_Id = a.F_Id,
                       a.F_CheckMonth,
                       a.F_Shift,
                       a.F_Remark,
                       c = b
                   }).ToList();
            }


            var cols = new List<string>();
            for (int i = DateTime.Now.Day; i >= 1; i--)
            {
                cols.Add("F_Day" + i);
            }


            return new
            {
                data,
                cols,
                code
            };

        }

        public async Task SaveCheckResult(List<EqpPqcTempEntity> tableData)
        {
            if (tableData.Count < 1) throw new Exception("无数据！");
            var M_F_Id = "";
            if (string.IsNullOrEmpty(tableData[0].M_F_Id))
            {
                M_F_Id = Utils.GuId();
                EqpPqcMasterEntity masterEntity = new EqpPqcMasterEntity();
                masterEntity.F_Id = M_F_Id;
                masterEntity.F_EqpId = tableData[0].F_EqpId;
                masterEntity.F_CheckMonth = tableData[0].F_CheckMonth;
                masterEntity.F_Shift = tableData[0].F_Shift;
                masterEntity.F_Remark = tableData[0].F_Remark;
                await repository.Insert(masterEntity);

            }

            else
            {
                M_F_Id = tableData[0].M_F_Id;
                string F_Remark = tableData[0].F_Remark;
                await repository.Update(a => a.F_Id == M_F_Id, a => new EqpPqcMasterEntity()
                {
                    F_Remark = F_Remark
                });
            }

            for (int i = 0; i < tableData.Count; i++)
            {
                EqpPqcDetailEntity detailEntity = new EqpPqcDetailEntity();
                detailEntity.F_PId = M_F_Id;
                detailEntity.F_CheckItemId = tableData[i].F_CheckItemId;
                detailEntity.F_CheckItem = tableData[i].F_CheckItem;
                detailEntity.F_CheckStand = tableData[i].F_CheckStand;
                detailEntity.F_Day1 = tableData[i].c.F_Day1;
                detailEntity.F_Day2 = tableData[i].c.F_Day2;
                detailEntity.F_Day3 = tableData[i].c.F_Day3;
                detailEntity.F_Day4 = tableData[i].c.F_Day4;
                detailEntity.F_Day5 = tableData[i].c.F_Day5;
                detailEntity.F_Day6 = tableData[i].c.F_Day6;
                detailEntity.F_Day7 = tableData[i].c.F_Day7;
                detailEntity.F_Day8 = tableData[i].c.F_Day8;
                detailEntity.F_Day9 = tableData[i].c.F_Day9;
                detailEntity.F_Day10 = tableData[i].c.F_Day10;
                detailEntity.F_Day11 = tableData[i].c.F_Day11;
                detailEntity.F_Day12 = tableData[i].c.F_Day12;
                detailEntity.F_Day13 = tableData[i].c.F_Day13;
                detailEntity.F_Day14 = tableData[i].c.F_Day14;
                detailEntity.F_Day15 = tableData[i].c.F_Day15;
                detailEntity.F_Day16 = tableData[i].c.F_Day16;
                detailEntity.F_Day17 = tableData[i].c.F_Day17;
                detailEntity.F_Day18 = tableData[i].c.F_Day18;
                detailEntity.F_Day19 = tableData[i].c.F_Day19;
                detailEntity.F_Day20 = tableData[i].c.F_Day20;
                detailEntity.F_Day21 = tableData[i].c.F_Day21;
                detailEntity.F_Day22 = tableData[i].c.F_Day22;
                detailEntity.F_Day23 = tableData[i].c.F_Day23;
                detailEntity.F_Day24 = tableData[i].c.F_Day24;
                detailEntity.F_Day25 = tableData[i].c.F_Day25;
                detailEntity.F_Day26 = tableData[i].c.F_Day26;
                detailEntity.F_Day27 = tableData[i].c.F_Day27;
                detailEntity.F_Day28 = tableData[i].c.F_Day28;
                detailEntity.F_Day29 = tableData[i].c.F_Day29;
                detailEntity.F_Day30 = tableData[i].c.F_Day30;
                detailEntity.F_Day31 = tableData[i].c.F_Day31;

                if (string.IsNullOrEmpty(tableData[i].c.F_Id))
                {
                    detailEntity.F_Id = Utils.GuId();
                    await uniwork.Insert<EqpPqcDetailEntity>(detailEntity);
                }
                else
                {
                    detailEntity.F_Id = tableData[i].c.F_Id;
                    await uniwork.Update<EqpPqcDetailEntity>(detailEntity);
                }
            }
        }

        public async Task<List<remarkentity>> GetDailyRemark(string eqpId, string month)
        {
            var query = repository.IQueryable(t => t.F_EqpId == eqpId && t.F_CheckMonth == month)
                .Select(t =>new { t.F_Remark, t.F_Shift }).ToList();
            List<remarkentity> list = new List<remarkentity>();
            foreach (var item in query)
            {
                var remarklist= item.F_Remark.ToObject<List<remarkentity>>();
                if (item.F_Shift == "白班")
                {
                    remarklist.ForEach(t => t.shift = "白班");
                }
                else if (item.F_Shift == "夜班")
                {
                    remarklist.ForEach(t => t.shift = "夜班");
                }
                    list.AddRange(remarklist);
            }
            
            //list = list.OrderByDescending(t => new { t.day, t.shift }).ToList();
            list = list.OrderByDescending(t => t.day).ThenByDescending(t => t.shift).ToList();
            return list;
        }

    }

    public class remarkentity
    {  
        public string shift { get; set; }
        public string day { get; set; }
        public string dayRemark { get; set; }
    }
}
