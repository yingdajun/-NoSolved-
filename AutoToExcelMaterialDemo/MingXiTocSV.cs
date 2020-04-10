using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace AutoToExcelMaterialDemo
{
    //明细表批量导出
    [Transaction(TransactionMode.Manual)]
    class MingXiTocSV:IExternalCommand
    {
        string AddInPath = typeof(MingXiTocSV).Assembly.Location;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();

            //1.获取所有明细表
            //2.
            //3.把数据库导入到明细表
            //4.打开明细表
            // 


            Document doc = commandData.Application.ActiveUIDocument.Document;


            FilteredElementCollector viewSchedule = new FilteredElementCollector(doc);
            //应该这个就是明细表
            viewSchedule.OfCategory(BuiltInCategory.OST_Schedules).WhereElementIsNotElementType();
            TaskDialog.Show("viewSchedule",viewSchedule.Count().ToString());

            List<string> viewScheList = new List<string>();

            Dictionary<string, ViewSchedule> viewScheduleDict = new Dictionary<string, ViewSchedule>();

            ViewSchedule vs = null;
            foreach (Element ele in viewSchedule) {
                vs = ele as ViewSchedule;
                viewScheduleDict.Add(vs.Name, vs);
                viewScheList.Add(vs.Name);
                vs = null;
            }

            //然后再把最后的Name输入到Vode的list中去。
            //选择好的List
            List<string> viewSelectList = new List<string>();

            //成功导出吼吼吼完美
            foreach (string s1 in viewScheList) {
                ExcelToDataTable(viewScheduleDict[s1]);
            }

            //ViewSchedule v = doc.ActiveView as ViewSchedule;



            //TableData td = v.GetTableData();

            //TableSectionData tdb = td.GetSectionData(SectionType.Header);

            //string head = v.GetCellText(SectionType.Header, 0, 0);



            //TableSectionData tdd = td.GetSectionData(SectionType.Body);



            //int c = tdd.NumberOfColumns;

            //int r = tdd.NumberOfRows;


            ///////将这个表格导入至数据库中就行了，哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈


            ////将这些东西写进去

            ////HSSFWorkbook work = new HSSFWorkbook();

            ////ISheet sheet = work.CreateSheet("mysheet");

            ////for (int i = 0; i < r; i++)

            ////{

            ////    IRow row = sheet.CreateRow(i);

            ////    for (int j = 0; j < c; j++)

            ////    {

            ////        Autodesk.Revit.DB.CellType ctype = tdd.GetCellType(i, j);

            ////        ICell cell = row.CreateCell(j);

            ////        string str = v.GetCellText(SectionType.Body, i, j);

            ////        cell.SetCellValue(str);

            ////    }

            ////}

            //using (FileStream fs = File.Create("d:\\excel.xls"))

            //{

            //    //work.Write(fs);

            //    fs.Close();

            //}
            return Result.Succeeded;
        }

        public void ExcelToDataTable(ViewSchedule v) {

            List<string> excelList = new List<string>();
            TableData td = v.GetTableData();

            TableSectionData tdb = td.GetSectionData(SectionType.Header);

            //这是获取控件的头嘛
            string head = v.GetCellText(SectionType.Header, 0, 0);

            //TaskDialog.Show("HEAD",head);



            TableSectionData tdd = td.GetSectionData(SectionType.Body);

            //中间有狠行的那一列要跳过去
            //或者说要保留好
            //string body = v.GetCellText(SectionType.Body, 2, 2);
            //TaskDialog.Show("BODY", body);

            int c = tdd.NumberOfColumns;

            int r = tdd.NumberOfRows;

            //我懂一定是列的问题
            //dt = ScheDataToCsv.CreateScheExcelTitle(v, c, r);

            ////c是纵向
            //TaskDialog.Show("C", c.ToString());
            ////r是横向
            //TaskDialog.Show("r", r.ToString());



            /////将这个表格导入至数据库中就行了，哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈


            //将这些东西写进去

            HSSFWorkbook work = new HSSFWorkbook();

            ISheet sheet = work.CreateSheet(head);

            for (int i = 0; i < r; i++)
            {
                IRow row = sheet.CreateRow(i);



                for (int j = 0; j < c; j++)
                {

                    Autodesk.Revit.DB.CellType ctype = tdd.GetCellType(i, j);

                    ICell cell = row.CreateCell(j);

                    string str = v.GetCellText(SectionType.Body, i, j);

                    cell.SetCellValue(str);

                }

            }

            string excelPath = null;
            //excelPath = head;

            excelPath = AddInPath.Replace("AutoToExcelMaterialDemo.dll", head + ".xlsx");
            //ScheDataToCsv.
            //dataTableToCsv(dt, excelPath);
            //除了速度太慢没有啥子缺点
            using (FileStream fs = File.Create(excelPath))
            {
                work.Write(fs);
            }

            //打开
            //System.Diagnostics.Process.Start(excelPath);

            //这里应该加入一个读取EXCEL为DataTable，然后去遍历dataTable
            //或许也可以进行判断两个表的差异性

            
        }
    }
}
