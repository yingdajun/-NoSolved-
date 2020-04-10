using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace AutoToExcelMaterialDemo
{
    //上下立管  425（？）  成功完成
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class ShangianLiGuanl : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;


            FilteredElementCollector levelCollector = new FilteredElementCollector(doc);
            levelCollector.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType();
            Dictionary<string, double> levelElevaltionDict = new Dictionary<string, double>();
            Level level = null;
            foreach (Element eleLev in levelCollector) {
                level = eleLev as Level;
                levelElevaltionDict.Add(level.Name, level.Elevation);
            }

            CreateShangXiaLiGuan(uidoc, doc);

            return Result.Succeeded;
        }

        /// <summary>
        /// 生成上下立管，我给忘了！！！！！
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="doc"></param>
        public void CreateShangXiaLiGuan(UIDocument uidoc, Document doc) {

            Reference refer = uidoc.Selection.PickObject(ObjectType.Element, new PipeSelection(), "请选择管道");
            Element ele = doc.GetElement(refer) as Element;

            XYZ pickPoint = refer.GlobalPoint;
            //
            Pipe pipe = ele as Pipe;

            LocationCurve lc = pipe.Location as LocationCurve;

            //xyz 
            XYZ startPoint = lc.Curve.GetEndPoint(0);
            XYZ endPoint = lc.Curve.GetEndPoint(1);

            double toStart = pickPoint.DistanceTo(startPoint);
            double toEnd = pickPoint.DistanceTo(endPoint);

            XYZ drawStartPoint = new XYZ();
            XYZ drawEndPoint = new XYZ();
            //获取系统类型
            ElementId pipeSystemId = pipe.get_Parameter(BuiltInParameter
                .RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();
            //pipe.MEPSystem.Id;
            //获取类型ID 

            //获取类型ID
            ElementId pipeTypeId = pipe.PipeType.Id;

            ElementId levelId = pipe.get_Parameter(BuiltInParameter
                .RBS_START_LEVEL_PARAM).AsElementId();

            Level lev1 = doc.GetElement(new ElementId(729642)) as Level;
            Level lev2 = doc.GetElement(new ElementId(729656)) as Level;
            if (toStart < toEnd)
            {
                drawStartPoint = new XYZ(startPoint.X, startPoint.Y, lev1.Elevation);
                drawEndPoint = new XYZ(startPoint.X, startPoint.Y, lev2.Elevation);
                Transaction ts = new Transaction(doc, "上下立管");
                ts.Start();
                Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                drawStartPoint, drawEndPoint
                                );
                ts.Commit();
            }
            else
            {
                drawStartPoint = new XYZ(endPoint.X, endPoint.Y, lev1.Elevation);
                drawEndPoint = new XYZ(endPoint.X, endPoint.Y, lev2.Elevation);
                Transaction ts = new Transaction(doc, "上下立管");
                ts.Start();
                Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                drawStartPoint, drawEndPoint
                                );
                ts.Commit();
            }
        }
    }
}
