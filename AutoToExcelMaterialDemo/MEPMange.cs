using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace AutoToExcelMaterialDemo
{
    //MEP 系统管理 351  部分完成
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class MEPMange : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //获取所有风管系统
            //类型名称  材质  线型  线宽  线颜色  我要新建一个类
            FilteredElementCollector ductSystemCollector = new FilteredElementCollector(doc);
            ductSystemCollector.OfCategory(BuiltInCategory.OST_DuctSystem).WhereElementIsElementType();

            //获取所有类型名称
            string s = null;
            foreach (Element ele in ductSystemCollector) {
                s = s + ele.Name+"\n";
            }
            TaskDialog.Show("s",s);

            //获取所有材质
            FilteredElementCollector fillPatternFilter = new FilteredElementCollector(doc);

            fillPatternFilter.OfClass(typeof(FillPatternElement));
            s = null;
            List<string> sList = new List<string>();
            foreach (Element ele in fillPatternFilter) {
                s = s + ele.Name + "\n";
                sList.Add(s);
            }
            s = null;
            sList.Sort();
            foreach (string s1 in sList) {
                s = s1 + "\n";
            }
            TaskDialog.Show("s", s);

            //获取线型成功
            FilteredElementCollector lineFilter = new FilteredElementCollector(doc);
            lineFilter.OfClass(typeof(LinePatternElement));
            s = null;
            foreach (Element ele in lineFilter)
            {
                s = s + ele.Name + "\n";
                sList.Add(s);
            }
            TaskDialog.Show("s", s);

            //获取线宽成功
            
            s = null;
            //线宽获取
            List<string> lineWidth = new List<string>();
            lineWidth.Add("无替换");
            lineWidth.Add("1");
            lineWidth.Add("2");
            lineWidth.Add("3");
            lineWidth.Add("4");
            lineWidth.Add("5");
            lineWidth.Add("6");
            lineWidth.Add("7");
            lineWidth.Add("8");
            lineWidth.Add("9");
            lineWidth.Add("10");
            lineWidth.Add("11");
            lineWidth.Add("12");
            lineWidth.Add("13");
            lineWidth.Add("14");
            lineWidth.Add("15");
            lineWidth.Add("16");
            TaskDialog.Show("s", s);


            //获取所有管道系统
            //类型名称 缩写 材质 线宽 线颜色 （我要尝试尝试使用DataTable存储数据）
            FilteredElementCollector pipCollector = new FilteredElementCollector(doc);
            pipCollector.OfCategory(BuiltInCategory.OST_PipingSystem).WhereElementIsElementType();

            //获取所有类型名称
            s = null;
            foreach (Element ele in pipCollector)
            {
                s = s + ele.Name + "\n";
            }
            TaskDialog.Show("s", s);
            //然后再加上一个填充即可

            return Result.Succeeded;
        }


        public void ChangeDuctAndPipe(UIDocument uidoc,Document doc) {
            FilteredElementCollector fillPatternFilter = new FilteredElementCollector(doc);

            fillPatternFilter.OfClass(typeof(FillPatternElement));

            //获取实体填充

            FillPatternElement fp = fillPatternFilter.First(m => (m as FillPatternElement).GetFillPattern().IsSolidFill) as FillPatternElement;

            Transaction trans = new Transaction(doc, "trans");

            trans.Start();

            View v = doc.ActiveView;

            ElementId cateId = new ElementId((int)BuiltInCategory.OST_Walls);

            //

            OverrideGraphicSettings ogs = v.GetCategoryOverrides(cateId);

            //设置 投影/表面 ->填充图案->填充图案

            ogs.SetProjectionFillPatternId(fp.Id);

            //设置 投影/表面 ->填充图案->颜色

            ogs.SetProjectionFillColor(new Color(255, 125, 0));

            //ogs.SetCutLineWeight();
            //应用到视图

            v.SetCategoryOverrides(cateId, ogs);

            trans.Commit();

            //控制柱子

            Transaction trans1 = new Transaction(doc, "trans");

            trans1.Start();

            View v1 = doc.ActiveView;

            //ElementId cateId1 = new ElementId((int)BuiltInCategory.OST_Columns);
            ElementId cateId1 = new ElementId((int)336266);
            //ElementId cateId3 = new ElementId(336344);

            TaskDialog.Show("1", cateId1.ToString());
            //

            OverrideGraphicSettings ogs1 = v.GetCategoryOverrides(cateId1);

            //设置 投影/表面 ->填充图案->填充图案

            ogs1.SetProjectionFillPatternId(fp.Id);

            //设置 投影/表面 ->填充图案->颜色

            ogs1.SetProjectionFillColor(new Color(0, 120, 0));



            v1.SetElementOverrides(cateId1, ogs1);
            //应用到视图

            //v1.SetCategoryOverrides(cateId1, ogs1);

            trans1.Commit();

        }

        ///// <summary>
        ///// 获取线样式类型
        ///// </summary>
        ///// <param name="doc"></param>
        ///// <returns></returns>
        //public static List<GraphicsStyle> GetLineStyles(Document doc)
        //{
        //    List<GraphicsStyle> result = new List<GraphicsStyle>();
        //    var styles = doc.GetElement<GraphicsStyle>();

        //    Category lineCategory = null;
        //    foreach (var g in styles)
        //    {
        //        if (g.GraphicsStyleCategory != null)
        //        {
        //            var category = g.GraphicsStyleCategory.Parent;
        //            if (category != null)
        //            {
        //                BuiltInCategory bic = (BuiltInCategory)category.Id.IntegerValue;
        //                if (bic == BuiltInCategory.OST_Lines)
        //                {
        //                    lineCategory = category;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    if (lineCategory != null)
        //    {
        //        foreach (Category obj in lineCategory.SubCategories)
        //        {
        //            BuiltInCategory integerValue = (BuiltInCategory)obj.Id.IntegerValue;
        //            if (integerValue == BuiltInCategory.OST_GenericLines ||
        //            integerValue == BuiltInCategory.OST_CenterLines ||
        //            integerValue == BuiltInCategory.OST_DemolishedLines ||
        //            integerValue == BuiltInCategory.OST_OverheadLines ||
        //            integerValue == BuiltInCategory.OST_LinesBeyond ||
        //            integerValue == BuiltInCategory.OST_HiddenLines ||
        //            integerValue == BuiltInCategory.OST_CurvesMediumLines ||
        //            integerValue == BuiltInCategory.OST_CurvesWideLines ||
        //            integerValue == BuiltInCategory.OST_CurvesThinLines ||
        //            integerValue == BuiltInCategory.OST_LinesHiddenLines)
        //            {
        //                GraphicsStyle gs = obj.GetGraphicsStyle(GraphicsStyleType.Projection);
        //                result.Add(gs);
        //            }

        //            if (obj.Id.IntegerValue > 0)
        //            {
        //                GraphicsStyle gs = obj.GetGraphicsStyle(GraphicsStyleType.Projection);
        //                result.Add(gs);
        //            }
        //        }
        //    }

        //    return result.OrderBy(f => f.Name).ToList();
        //}
    }
}
