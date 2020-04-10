using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;

namespace AutoToExcelMaterialDemo
{
    //两根相连 431  完成
    [Transaction(TransactionMode.Manual)]
    class BetweenJoin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {



            ////throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference refer = uidoc.Selection.PickObject(ObjectType.Element
                , new PipeSelection()
                , "请选择管道");
            Element ele = doc.GetElement(refer) as Element;
            Pipe pipe = ele as Pipe;
            LocationCurve lc = pipe.Location as LocationCurve;
            Line line1 = lc.Curve as Line;

            Reference refer1 = uidoc.Selection.PickObject(ObjectType.Element,
                new PipeSelection()
                , "请选择管道1");
            Element ele1 = doc.GetElement(refer1) as Element;
            Pipe pipe1 = ele1 as Pipe;
            LocationCurve lc1 = pipe1.Location as LocationCurve;
            Line line2 = lc1.Curve as Line;


            //获取系统类型
            ElementId pipeSystemId = pipe.get_Parameter(BuiltInParameter
                .RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();
            //pipe.MEPSystem.Id;
            //获取类型ID 

            //获取类型ID
            ElementId pipeTypeId = pipe.PipeType.Id;

            ElementId levelId = pipe.get_Parameter(BuiltInParameter
                .RBS_START_LEVEL_PARAM).AsElementId();

            ////点击完前后是一个！！！！！

            XYZ startPoint = lc.Curve.GetEndPoint(0);
            XYZ endPoint = lc.Curve.GetEndPoint(1);

            XYZ startPoint1 = lc1.Curve.GetEndPoint(0);
            XYZ endPoint1 = lc1.Curve.GetEndPoint(1);

            Dictionary<double, ChangeLine0Cureve> distanceDict = 
                new Dictionary<double, ChangeLine0Cureve>();
            ////那我就把他们强制转换一下

            List<double> minDistanceList = new List<double>();
            //计算List 
            double distance = 0.0;
            
            //0
            distance = startPoint.DistanceTo(startPoint1);
            minDistanceList.Add(startPoint.DistanceTo(startPoint1));
            ChangeLine0Cureve cl = new ChangeLine0Cureve(startPoint,startPoint1);
            distanceDict.Add(distance, cl);

            //1
            distance = 0.0;
            distance = startPoint.DistanceTo(endPoint1);
            minDistanceList.Add(startPoint.DistanceTo(endPoint1));
            ChangeLine0Cureve c2 = new ChangeLine0Cureve(startPoint,endPoint1);
            distanceDict.Add(distance, c2);

            //2
            distance = 0.0;
            distance = endPoint.DistanceTo(startPoint1);
            minDistanceList.Add(endPoint.DistanceTo(startPoint1));
            ChangeLine0Cureve c3 = new ChangeLine0Cureve(endPoint,startPoint1);
            distanceDict.Add(distance, c3);

            //3
            distance = 0.0;
            distance = endPoint.DistanceTo(endPoint1);
            minDistanceList.Add(endPoint.DistanceTo(endPoint1));
            ChangeLine0Cureve c4 = new ChangeLine0Cureve(endPoint,endPoint1);
            distanceDict.Add(distance, c4);

            double min = minDistanceList.Min();
            int flag =
            minDistanceList.IndexOf(min);
            //这是第3个
            //TaskDialog.Show("flag",flag.ToString());

            //string s = null;
            //s = s + startPoint.X + "," + startPoint.Y + "," + startPoint.Z + "\n";
            //s = s + endPoint.X + "," + endPoint.Y + "," + endPoint.Z + "\n";
            //s = s + startPoint1.X + "," + startPoint1.Y + "," + startPoint1.Z + "\n";
            //s = s + endPoint1.X + "," + endPoint1.Y + "," + endPoint1.Z + "\n";
            //TaskDialog.Show("s", s);

            //成功
            if (flag == 0) {
                //flag 0
                Transaction ts = new Transaction(doc, "两管连接");
                ts.Start();
                Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                startPoint, new XYZ(startPoint1.X, startPoint1.Y, startPoint.Z)
                                );
                Pipe pipedemo1 = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                 new XYZ(startPoint1.X, startPoint1.Y, startPoint.Z), startPoint1
                                );
                ts.Commit();
            }
            //成功
            if (flag == 1) {
                //flag 1
                Transaction ts = new Transaction(doc, "两管连接");
                ts.Start();
                Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                startPoint, new XYZ(endPoint1.X, endPoint1.Y, startPoint.Z)
                                );
                Pipe pipedemo1 = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                 new XYZ(endPoint1.X, endPoint1.Y, startPoint.Z), endPoint1
                                );
                ts.Commit();
            }
            //成功
            if (flag == 2) {
                //flag 2
                Transaction ts = new Transaction(doc, "两管连接");
                ts.Start();
                Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                endPoint, new XYZ(startPoint1.X, startPoint1.Y, endPoint.Z)
                                );
                Pipe pipedemo1 = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                 new XYZ(startPoint1.X, startPoint1.Y, endPoint.Z), startPoint1
                                );
                ts.Commit();
            }
            if (flag == 3) {
                //flag 3
                Transaction ts = new Transaction(doc, "两管连接");
                ts.Start();
                Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                endPoint, new XYZ(endPoint1.X, endPoint1.Y, endPoint.Z)
                                );
                Pipe pipedemo1 = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                 new XYZ(endPoint1.X, endPoint1.Y, endPoint.Z), endPoint1
                                );
                ts.Commit();
            }

            


            return Result.Succeeded;
        }

        public class ChangeLine0Cureve {
            public XYZ startPoints;
            public XYZ endPoints;
            public ChangeLine0Cureve(XYZ startPoint,XYZ endPoint) {
                this.startPoints = startPoint;
                this.endPoints = endPoint;
            }
            public Line ChangeCure(XYZ startPoint, XYZ endPoint, int flag)
            {
                Line li = null;

                    Line.CreateBound(startPoint, endPoint);
                return li;
            }
        }

        


        /// <summary>
        /// 获取一个向量的任意垂直向量，得到发现
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public XYZ getNormal1(XYZ dir)
        {
            return new XYZ(dir.Y + dir.Z, -dir.X + dir.Z, -dir.X - dir.Y);
        }

    }
}
