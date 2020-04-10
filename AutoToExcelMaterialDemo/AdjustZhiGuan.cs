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
    //快速调整支管高度 391 支管变高
    [Transaction(TransactionMode.Manual)]
    class AdjustZhiGuan : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            //UIApplication uiapp = commandData.Application;
            //Application app = uiapp.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //调整高度

            //立管角度
            //立管直径同支管+立管直径同主管 立管直径自定义

            Reference refer = uidoc.Selection.PickObject(ObjectType.Element,"请选择需要变换的立管");
            Pipe pipe = doc.GetElement(refer) as Pipe;
            LocationCurve lc = pipe.Location as LocationCurve;
            //往上偏移
            Line li = lc.Curve as Line;
            //往下偏移
            XYZ start = li.GetEndPoint(0);
            XYZ end = li.GetEndPoint(1);
            double offset = -500/304.8;
            Line newLine = Line.CreateBound(new XYZ(start.X,start.Y,(start.Z+offset)),
                new XYZ(end.X,end.Y,end.Z+offset));

            //pipe.ConnectorManager.Connectors;
            ////获取管道连接
            //FilteredElementCollector pipeCollector = new FilteredElementCollector(doc);
            //pipeCollector.OfCategory(BuiltInCategory.OST_PipeCurves)
            //    .WhereElementIsNotElementType();
            //string s = null;
            //foreach (Element ele in pipeCollector) {
            //    if (JoinGeometryUtils.AreElementsJoined(doc,pipe,ele)) {
            //        s = s + ele.Id.ToString()+"\n";
            //    }
            //}

            //string message1 = "";

            
            //TaskDialog.Show("相交元素",s);
            Transaction ts = new Transaction(doc, "changePipe");
            ts.Start();
            //LocationCurve PipeLocationCurve = copyPipe.Location as LocationCurve;
            lc.Curve = newLine;
            ts.Commit();
            
            return Result.Succeeded;
        }
    }
}
