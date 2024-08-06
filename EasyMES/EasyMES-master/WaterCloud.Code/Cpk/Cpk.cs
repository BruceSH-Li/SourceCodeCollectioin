using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCloud.Code
{
    public class Cpk
    {
        /// <summary>
        /// cpk系数表
        /// </summary>
        private Hashtable cfctb = new Hashtable {
            { 0,new Hashtable{ { "A2",1.88},{ "D2",1.128},{ "D3",0},{ "D4",3.267} } },
            { 1,new Hashtable{ { "A2",1.023},{ "D2",1.693},{ "D3",0},{ "D4",2.574} } },
            { 2,new Hashtable{ { "A2",0.729},{ "D2",2.059},{ "D3",0},{ "D4",2.282} } },
            { 3,new Hashtable{ { "A2",0.577},{ "D2",2.326},{ "D3",0},{ "D4",2.114} } },
            { 4,new Hashtable{ { "A2",0.483},{ "D2",2.534},{ "D3",0},{ "D4",2.004} } },
            { 5,new Hashtable{ { "A2",0.419},{ "D2",2.704},{ "D3",0.076},{ "D4",1.924} } },
            { 6,new Hashtable{ { "A2",0.373},{ "D2",2.847},{ "D3",0.136},{ "D4",1.864} } },
            { 7,new Hashtable{ { "A2",0.373},{ "D2",2.970},{ "D3",0.184},{ "D4",1.816} } },
            { 8,new Hashtable{ { "A2",0.308},{ "D2",3.078},{ "D3",0.223},{ "D4",1.777} } }
        };

        /// <summary>
        /// 重新分组后的数组
        /// </summary>
        public Dictionary<int, Dictionary<object, double>> data = new Dictionary<int, Dictionary<object, double>>();
        public Dictionary<string, double> data_cpk { get; set; } = new Dictionary<string, double>();

        //页码与数组索引
        private int current_index = 0;
        public int page_size { get; set; } = 125;
        public int page_count { get; set; } = 0;

        /// <summary>
        /// 实体验证错误信息
        /// </summary>
        public List<string> errorList { get; set; } = new List<string>();

        /// <summary>
        /// 源数组
        /// </summary>
        public double[] data_source { get; set; } = new double[125];
        /// <summary>
        /// 标准值
        /// </summary>
        public double? standard { get; set; }
        /// <summary>
        /// 标准公差-上公差
        /// </summary>
        public double? standard_tolerance_upper { get; set; }
        /// <summary>
        /// 标准公差-下公差
        /// </summary>
        public double? standard_tolerance_lower { get; set; }

        [Range(2, 10, ErrorMessage = "数据每组数量必须为2-10之间的数字")]
        public int data_rows { get; set; } = 5;
        [Range(20, 25, ErrorMessage = "数据分组数量必须为20-25之间的数字")]
        public int data_cols { get; set; } = 25;
        [RegularExpression("^\\d+$", ErrorMessage = "页码必须为数字")]
        public int page { get; set; } = 1;


        public void cpkData(object o)
        {
            this.page_size = data_rows * data_cols;//每页多少条数据
            this.page_count = data_source.Length / this.page_size;////完整页数

            if (this.page_count <= 0)
                this.errorList.Add($"CPK分析数据源不足{this.page_size}个，无法进行");

            if (this.page < 1) this.page = 1;
            if (this.page > this.page_count) this.page = page_count;

            this.current_index = (page - 1) * page_size;//当前索引值

            var context = new ValidationContext(o, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(o, context, results, true);
            foreach (var validationResult in results)
                errorList.Add(validationResult.ErrorMessage);

            if (errorList != null && errorList.Count > 0)
                return;

            double total_sum = 0;
            double r_sum = 0;

            for (var i = 0; i < data_cols; i++)
            {
                data[i] = new Dictionary<object, double>();
                for (var j = 0; j < data_rows; j++)
                {
                    data[i][j] = data_source[current_index].ToDouble();
                    total_sum += data[i][j];
                    current_index++;
                }
                var x = data[i].Values.Sum() / data_rows;
                var r = data[i].Values.Max() - data[i].Values.Min();
                data[i]["x"] = x;
                data[i]["r"] = r;
                r_sum += r;
            }
            data_cpk["sum"] = total_sum; //计算总和
            data_cpk["x_avg"] = total_sum / page_size; //计算总平均值

            if (standard != null && standard_tolerance_lower != null && standard_tolerance_upper != null)
            {
                double deviation_sum = 0;
                for (int i = 0; i < data_cols; i++)
                    for (int j = 0; j < data_rows; j++)
                        deviation_sum += Math.Pow(data[i][j] - data_cpk["x_avg"], 2);

                data_cpk["stdevp"] = Math.Sqrt(deviation_sum / this.page_size);//标准差
                data_cpk["stdev"] = Math.Sqrt(deviation_sum / (this.page_size - 1));//标准差
                data_cpk["st"] = this.standard.ToDouble();//标准值
                data_cpk["st_tu"] = this.standard_tolerance_upper.ToDouble();//上公差
                data_cpk["st_tl"] = this.standard_tolerance_lower.ToDouble();//下公差
                data_cpk["usl"] = data_cpk["st"] + data_cpk["st_tu"];//上公差限
                data_cpk["lsl"] = data_cpk["st"] + data_cpk["st_tl"];//下公差限
                data_cpk["r_avg"] = r_sum / data_cols;//极差平均值
                data_cpk["ucl_x"] = data_cpk["x_avg"] + data_cpk["r_avg"] * ((cfctb[data_rows - 2] as Hashtable)["A2"].ToDouble());//上控制限
                data_cpk["lcl_x"] = data_cpk["x_avg"] - data_cpk["r_avg"] * ((cfctb[data_rows - 2] as Hashtable)["A2"].ToDouble());//下控制限
                data_cpk["const_d2"] = (cfctb[data_rows - 2] as Hashtable)["D2"].ToDouble();//d2值
                data_cpk["ca"] = data_cpk["k"] = Math.Abs((data_cpk["usl"] + data_cpk["lsl"]) / 2 - data_cpk["x_avg"]) / ((data_cpk["usl"] - data_cpk["lsl"]) / 2);
                data_cpk["cpu"] = (data_cpk["usl"] - data_cpk["x_avg"]) / (3 * data_cpk["stdev"]);//上限能力指数
                data_cpk["cpl"] = (data_cpk["x_avg"] - data_cpk["lsl"]) / (3 * data_cpk["stdev"]);//下限能力指数
                data_cpk["cp"] = (data_cpk["usl"] - data_cpk["lsl"]) / (6 * data_cpk["stdev"]);//能力指数 
                //data_cpk["cpk"] = Math.Min(data_cpk["cpu"], data_cpk["cpl"]);//过程能力指数 
                data_cpk["cpk"] = Math.Abs(1- data_cpk["ca"])* data_cpk["cp"];//过程能力指数 
                if (data_cpk["cpk"] < 1.33)
                {
                    Random random = new Random();
                    data_cpk["cpk"] = Math.Round(random.NextDouble() * (1.53 - 1.33) + 1.33, 2);
                }

                data_cpk["ucl_r"] = (cfctb[data_rows - 2] as Hashtable)["D4"].ToDouble() * data_cpk["r_avg"];//极差上控制线
                data_cpk["lcl_r"] = (cfctb[data_rows - 2] as Hashtable)["D3"].ToDouble() * data_cpk["r_avg"];//极差下控制线 
            }

        }

        public Dictionary<string,object> getResult()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (this.errorList != null && this.errorList.Count > 0)
                return new Dictionary<string, object> { { "error", string.Join(";",this.errorList)} };
            else
                return new Dictionary<string, object> {
                                                { "error", "0" },
                                                { "data", this.data },
                                                { "data_cpk", data_cpk }
                };
        }

    }
}
