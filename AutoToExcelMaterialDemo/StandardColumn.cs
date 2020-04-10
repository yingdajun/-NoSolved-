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
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    //标准柱子 完成
    public class StandardColumn : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector gridFilter = new FilteredElementCollector(doc);

            // 获取所有的轴网
            List<Grid> allGrids = gridFilter.OfClass(typeof(Grid)).Cast<Grid>().ToList();

            //获取轴网的所有交点
            List<XYZ> Points = new List<XYZ>();
            foreach (Grid grid in allGrids)
            {
                Grid currentGrid = grid;
                foreach (Grid grd in allGrids)
                {
                    IntersectionResultArray ira = null;
                    SetComparisonResult scr = currentGrid.Curve.Intersect(grd.Curve, out ira);
                    if (ira != null)
                    {
                        IntersectionResult ir = ira.get_Item(0);


                        // 判断点是否重复
                        if (!CheckPoint(Points, ir.XYZPoint))
                        {
                            Points.Add(ir.XYZPoint);
                        }
                    }
                }
            }

            //柱子完成，哈哈哈哈哈哈

            //// 设置ViewModel
            //MyDataContext myDataContext = new MyDataContext(doc);
            //MyWin myWin = new MyWin(myDataContext);
            //if (myWin.ShowDialog() ?? false)
            //{
            //    // 返回用户选定的建筑柱FamilySymbol
            //    FamilySymbol symbol = myDataContext.Symbol as FamilySymbol;

            //    // 返回柱子的顶部标高
            //    Level topLevel = myDataContext.TopLevel as Level;

            //    // 返回柱子的底部标高
            //    Level btmLevel = myDataContext.BtmLevel as Level;

            //    // 返回顶部偏移
            //    double topOffset = myDataContext.TopOffset / 304.8;

            //    // 返回底部偏移
            //    double btmOffset = myDataContext.BtmOffset / 304.8;

            //    //启动 事务

            FamilySymbol symbol = doc.GetElement(new ElementId(52557)) as FamilySymbol;
            Level btmLevel = doc.GetElement(new ElementId(311)) as Level;
            Transaction trans = new Transaction(doc, "Create");
            trans.Start();
            foreach (XYZ p in Points)
            {
                FamilyInstance column = doc.Create.NewFamilyInstance(p, symbol, btmLevel, StructuralType.NonStructural);
                ////设置底部偏移
                //column.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM).Set(btmOffset);
                ////设置顶部标高
                //column.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).Set(topLevel.Id);
                ////设置顶部偏移
                //column.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).Set(topOffset);
            }
            // 提交事务
            trans.Commit();
            //}


            return Result.Succeeded;
        }

        private bool CheckPoint(List<XYZ> points, XYZ point)
        {
            bool flag = false;
            foreach (XYZ p in points)
            {
                if (p.IsAlmostEqualTo(point))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
    }

}
