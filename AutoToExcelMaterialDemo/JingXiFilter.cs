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
//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using Autodesk.Revit.UI.Selection;
//using Autodesk.Revit.DB.Architecture;
namespace AutoToExcelMaterialDemo
{
    //精细过滤
    [Transaction(TransactionMode.Manual)]
    class JingXiFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //类别类型

            //Category 
            //FilteredElementCollector collector = new FilteredElementCollector(doc);
            Categories categories = doc.Settings.Categories;

            ////楼层
            //FilteredElementCollector levelCollectorList = new FilteredElementCollector(doc);
            //List<string> levelNameList = new List<string>();
            //foreach (Element ele in levelCollectorList) {
            //    levelNameList.Add(ele.Name);
            //}

            //获取所有对应的类型
            
            //我要创建一个类啊
            //看来还真有必要
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType();
            //墙体
            string s = null;
            foreach (Element ele in collector) {
                s = s + ele.Name+"\n";
                //这些类型里面还有其他的小实体类
                //可能还需要相关实例
            }
            TaskDialog.Show("S",s);

            ////获取墙的某个类型
            //UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            //Document doc = uiDoc.Document;

            //ElementId id = doc.GetDefaultElementTypeId(ElementTypeGroup.WallType);
            //WallType type = doc.GetElement(id) as WallType;
            ////获取所有类别
            //if (type == null)
            //    return Result.Failed;

            //using (Transaction tr = new Transaction(doc))
            //{
            //    tr.Start("重命名类型");
            //    type.Name = "自定义墙类型";
            //    tr.Commit();
            //}


            //已经满足了

            //Reference refer = uidoc.Selection.PickObject(ObjectType.Element, "选择墙体");

            //Wall wall = doc.GetElement(refer) as Wall;
            ////FamilyInstance fsi = doc.GetElement(refer) as FamilyInstance;
            ////Element ele = null;
            //FilteredElementCollector collector1 = new FilteredElementCollector(doc);
            //collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType();
            ////墙体
            //s = null;
            //foreach (Element ele in collector)
            //{
            //    s = s + ele.Name + "\n";
            //    //这些类型里面还有其他的小实体类
            //    //可能还需要相关实例
            //}
            //TaskDialog.Show("S", s);




            return Result.Succeeded;
        }

        private void AssignDefaultTypeToColumn(Document document, FamilyInstance column)
        {
            ElementId defaultTypeId = document.GetDefaultFamilyTypeId(new ElementId(BuiltInCategory.OST_StructuralColumns));

            if (defaultTypeId != ElementId.InvalidElementId)
            {
                FamilySymbol defaultType = document.GetElement(defaultTypeId) as FamilySymbol;
                if (defaultType != null)
                {
                    column.Symbol = defaultType;
                }
            }
        }
    }
}
