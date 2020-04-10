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
using Autodesk.Revit.DB.Plumbing;
namespace AutoToExcelMaterialDemo
{
    //调整管道间距 381
    [Transaction(TransactionMode.Manual)]
    class AdjustPipeDistance:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            //UIApplication uiapp = commandData.Application;
            //Application app = uiapp.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //PipeSelection pipeSelection = new PipeSelection();
            //IList<Reference> pipeReferList = uidoc.Selection.PickObjects(ObjectType.Element
            //    ,pipeSelection, "请选择相应的管道");
            //List<Pipe> pipeList = new List<Pipe>();

            //Pipe pipe = null;
            //foreach (Reference refer in pipeReferList) {
            //    pipe = doc.GetElement(refer) as Pipe;
            //    pipeList.Add(pipe);
            //}
            //Line li = null;
            //List<Line> mepList = new List<Line>();
            //LocationCurve lc = null;
            //foreach (Pipe pi in pipeList) {
            //    lc= pi.Location as LocationCurve;
            //    li = lc.Curve as Line;
            //    mepList.Add(li);
            //}
            //var stablemep1 = mepList.First();

            Reference refer = uidoc.Selection.PickObject(ObjectType.Element,new PipeSelection()
                , "选择管道A");
            Reference refer1 = uidoc.Selection.PickObject(ObjectType.Element,new PipeSelection()
                , "选择管道B");

            Pipe p1 = doc.GetElement(refer) as Pipe;
            Pipe p2 = doc.GetElement(refer1) as Pipe;
            XYZ point1 = null;
            LocationCurve locationCurve = null;
            locationCurve = p1.Location as LocationCurve;
            Line li = locationCurve.Curve as Line;
            point1 = li.Origin;
            XYZ point2 = null;
            LocationCurve locationCurve2 = null;
            locationCurve2 = p2.Location as LocationCurve;
            Line li2 = locationCurve2.Curve as Line;
            point2 = li2.Origin;

            //获取系统类型
            ElementId pipeSystemId = p1.get_Parameter(BuiltInParameter
                .RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();
            //pipe.MEPSystem.Id;
            //获取类型ID 

            //获取类型ID
            ElementId pipeTypeId = p1.PipeType.Id;

            //这是对应的啥子
            ElementId levelId = p1.get_Parameter(BuiltInParameter
                .RBS_START_LEVEL_PARAM).AsElementId();
            //p1.LevelId;


            XYZ start = locationCurve.Curve.GetEndPoint(0);
            XYZ end = locationCurve.Curve.GetEndPoint(1);
            XYZ offsetBase =getNormal1(li.Direction).Normalize()*(-309/304.8
              );
            Transaction ts = new Transaction(doc, "进行管道间距排序");
            ts.Start();
            doc.Delete(p1.Id);
            Pipe newP2 = Pipe.Create(doc,pipeSystemId,pipeTypeId,levelId,
               start+offsetBase,end+offsetBase);
            
            //ElementTransformUtils.MoveElement(doc, p1.Id,  offsetBase);

            ts.Commit();
            

            return Result.Succeeded;
        }
        /// <summary>
        /// 获取一个向量的任意垂直向量
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public XYZ getNormal1(XYZ dir)
        {
            return new XYZ(dir.Y + dir.Z, -dir.X + dir.Z, -dir.X - dir.Y);
        }


    }
}
