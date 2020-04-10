using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Threading;

namespace AutoToExcelMaterialDemo
{
    public static class ScheDataToCsv
    {
        //将DataTable相关的信息导入到CSV各式中
        public static void dataTableToCsv(System.Data.DataTable table, string file)
        {
            string title = "";
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                title += table.Columns[i].ColumnName + "\t";
            }
            title = title.Substring(0, title.Length - 1) + "\n";
            sw.Write(title);
            foreach (DataRow row in table.Rows)
            {
                string line = "";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    line += row[i].ToString().Trim() + "\t";
                }
                line = line.Substring(0, line.Length - 1) + "\n";
                sw.Write(line);
            }
            sw.Close();
            fs.Close();
            //TaskDialog.Show("使用提示", "更新数据前请把当前的EXCEL关闭");
        }


        public static System.Data.DataTable CreateScheExcelTitle(ViewSchedule view, int c,int r)
        {

            //生成明细表
            System.Data.DataTable dt = new System.Data.DataTable(view.Name);
            //自动生成表格所对应的ID
            //DataColumn dc = dt.Columns.Add("id", Type.GetType("System.Int32"));

            //dc.AutoIncrement = true;
            //dc.AutoIncrementSeed = 1;
            //dc.AutoIncrementStep = 1;
            //dc.AllowDBNull = false;

            string str = null;
            //将Title赋予一波,这是第0-c列的值
            //这是tile的标题
            List<string> titleList = new List<string>();
            DataColumn dataColumn = null;
            
            for (int i = 0; i < c; i++)
            {
                //这是title 
                //这是第0列的1到c列的值
                dataColumn= new DataColumn(str, Type.GetType("System.String"));
                str = view.GetCellText(SectionType.Body, 0, i);
                dt.Columns.Add(dataColumn);
                dataColumn = null;
                titleList.Add(str);
            }

            Dictionary<int, string> dict = new Dictionary<int, string>();

            string s = null;
            Thread thread = new Thread(() => {
                for (var i = 0; i < c; i++)
                {
                    //str = view.GetCellText(SectionType.Body, 0, i);
                    str = Convert.ToInt32(i).ToString();
                    dt.Columns.Add(new DataColumn(str, Type.GetType("System.String")));
                    titleList.Add(str);
                    //把对应的dict导入dictionary
                    dict[i] = str;
                }
            });
            thread.Start();
            thread.Join();


            TaskDialog.Show("1",titleList.Count.ToString());
            //这是相关的body值
            string str1 = null;
            //第i行
            for (int i = 1; i < c; i++)
            {
                //第i行第c列
                for (int j = 0; j < r; j++)
                {
                    DataRow dr = dt.NewRow();
                    //将相应的值赋予进title里面
                    str1 = view.GetCellText(SectionType.Body, i, j);
                    dr[dict[j]] = str1;
                    //将对应的值赋予进去
                    dt.Rows.Add(dr);
                    dr = null;
                }
            }


            //dt.Columns.Add(new DataColumn("参照标高", Type.GetType("System.String")));
            //dt.Columns.Add(new DataColumn("结构材质", Type.GetType("System.String")));
            //dt.Columns.Add(new DataColumn("长度", Type.GetType("System.Decimal")));
            //dt.Columns.Add(new DataColumn("起点标高偏移", Type.GetType("System.Decimal")));
            //dt.Columns.Add(new DataColumn("终点标高偏移", Type.GetType("System.Decimal")));
            //dt.Columns.Add(new DataColumn("底部高程", Type.GetType("System.Decimal")));
            //dt.Columns.Add(new DataColumn("顶部高程", Type.GetType("System.Decimal")));
            //dt.Columns.Add(new DataColumn("Y轴对正", Type.GetType("System.String")));
            //dt.Columns.Add(new DataColumn("Y轴偏移量", Type.GetType("System.Decimal")));
            //dt.Columns.Add(new DataColumn("YZ轴对正", Type.GetType("System.String")));
            //dt.Columns.Add(new DataColumn("Z轴对正", Type.GetType("System.String")));
            //dt.Columns.Add(new DataColumn("Z轴偏移量", Type.GetType("System.Decimal")));
            //dt.Columns.Add(new DataColumn("结构用途", Type.GetType("System.String")));

            return dt;
        }
    }
}
