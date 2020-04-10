using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.IO;
using Autodesk.Revit.Attributes;
namespace AutoToExcelMaterialDemo
{
    //显示浮动标号  橄榄山建模神器 433
    [Transaction(TransactionMode.Manual)]
    class HideAndVisibleGrid : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();

            //这是一个操作
            //1.判断轴网是否在动弹
            //2.获取所有轴网
            //3.获取轴网的轴号
            //4.控制显示或者隐藏

            //打开主界面
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //获取所有的轴网标头
            FilteredElementCollector gridCollector = new FilteredElementCollector(doc);
            gridCollector.OfCategory(BuiltInCategory.OST_GridHeads).WhereElementIsNotElementType();
            //控制所有轴网标头的显示位置

            return Result.Succeeded;
        }
    }
}
