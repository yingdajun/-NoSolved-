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
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI;
using NPOI.SS.UserModel;
//using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Threading;

namespace AutoToExcelMaterialDemo
{
    [Transaction(TransactionMode.Manual)]
    //明细表用EXCEL表打开

    class MingXiToExcel : IExternalCommand
    {
        //将DataTable相关的信息导入到CSV各式中
        string AddInPath = typeof(MingXiToExcel).Assembly.Location;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();

            //1.启动表格
            //2.使用数据库
            //3.把数据库导入到明细表
            //4.打开明细表
            // 
            //DataTable dt = null;

            Document doc = commandData.Application.ActiveUIDocument.Document;

            ViewSchedule v = doc.ActiveView as ViewSchedule;

            //TaskDialog.Show("Vname",v.Name.ToString());


            TableData td = v.GetTableData();

            TableSectionData tdb = td.GetSectionData(SectionType.Header);

            //这是获取控件的头嘛
            string head = v.GetCellText(SectionType.Header, 0, 0);

            //TaskDialog.Show("HEAD",head);

            
            
            TableSectionData tdd = td.GetSectionData(SectionType.Body);

            //中间有狠行的那一列要跳过去
            //或者说要保留好
            string body = v.GetCellText(SectionType.Body, 2, 2);
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
            System.Diagnostics.Process.Start(excelPath);

            //这里应该加入一个读取EXCEL为DataTable，然后去遍历dataTable
            //或许也可以进行判断两个表的差异性


            return Result.Succeeded;
        }
        /// <summary>  
        /// 将excel导入到datatable  
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="isColumnName">第一行是否是列名</param>  
        /// <returns>返回datatable</returns>  
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case NPOI.SS.UserModel.CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case NPOI.SS.UserModel.CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case NPOI.SS.UserModel.CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }



    }
}
