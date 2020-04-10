using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;

namespace AutoToExcelMaterialDemo
{
    //根据管线长度去打断管线 437  完成
    [Transaction(TransactionMode.Manual)]
    class GuanXianByLength:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();

            //初始化
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            CreateSpiltPipe(uidoc, doc, Convert.ToDouble(200));

            ///水管和风管段线
            ///
            //Reference r = uidoc.Selection.PickObject(ObjectType.PointOnElement);

            //Element elem = doc.GetElement(r);

            //// pipe 分段

            //Pipe p = elem as Pipe;

            //if (p != null)

            //根据这一点去切分

            

            ////Duct分段

            //Duct duct = elem as Duct;

            //if (duct != null)

            //    MechanicalUtils.BreakCurve(doc, duct.Id, r.GlobalPoint);
            ////


            return Result.Succeeded;
        }

        public void CreateSpiltPipe(UIDocument uidoc,Document doc,double length) {
            Selection selection = uidoc.Selection;
            length = length / 304.8;
            //选择打断
            //BreakPipe(doc, selection);

            //假定是这个

            //管道长度
            Reference reference = selection.PickObject(ObjectType.Element, new PipeSelection(), "请选择管道");
            Pipe pipe = doc.GetElement(reference) as Pipe;
            double pipeLength = pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
            //向下取值
            if (length > pipeLength)
                TaskDialog.Show("错误", "输入长度大于管道长度");
            double i = Math.Floor(pipeLength / length);


            LocationCurve locationCurve = pipe.Location as LocationCurve;
            Line li = locationCurve.Curve as Line;

            Line line =
                Line.CreateBound(li.GetEndPoint(0), li.GetEndPoint(0) + (li.Direction.Normalize() * length));

            List<XYZ> newXYZList = new List<XYZ>();
            List<XYZ> newFinalXYZList = new List<XYZ>();
            XYZ spiltPoint = new XYZ();
            newXYZList.Add(li.GetEndPoint(0));

            newFinalXYZList.Add(li.GetEndPoint(0));

            int j = 1;
            while (true)
            {

                spiltPoint = li.GetEndPoint(0) + (li.Direction.Normalize() * j * length);
                newXYZList.Add(spiltPoint);
                newFinalXYZList.Add(spiltPoint);
                j++;
                if (j == (i + 1))
                    break;

                //break;
            }
            //把最后一点加上去
            //第一个去掉一点
            newXYZList.Add(li.GetEndPoint(1));
            newFinalXYZList.Add(li.GetEndPoint(1));
            newFinalXYZList.Add(li.GetEndPoint(1));


            Transaction ts = new Transaction(doc, "按长度断管");
            ts.Start();
            int tag = 0;

            Curve newCurve2 = null;
            foreach (XYZ xyz in newXYZList)
            {
                //难道第0个
                //当是26的时候就太短了
                //这是什么妖怪呢
                //到了最后就跳出去
                if (tag == newXYZList.Count - 1)
                    break;
                //TaskDialog.Show("TAG", tag.ToString());
                newCurve2 = Line.CreateBound(newXYZList[tag]
                    , newFinalXYZList[tag + 1]);
                Pipe copyPipe = doc.GetElement(ElementTransformUtils.CopyElement(doc, pipe.Id
                    , new XYZ(0, 0, 0)).ElementAt(0)) as Pipe;
                LocationCurve copyPipeLocationCurve = copyPipe.Location as LocationCurve;
                //复制以后的点是这个
                copyPipeLocationCurve.Curve = newCurve2;
                tag = tag + 1;
                //这个不是原始的嘛
                //doc.Delete(pipe.Id);
            }
            doc.Delete(pipe.Id);

            ts.Commit();
        }


        public static void NewLine( Document doc, XYZ pStart, XYZ pEnd)
        {
            if (pStart.IsAlmostEqualTo(pEnd))
            {
                return;
            }
            using (Transaction tr = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                try
                {
                    tr.Start();
                    Line line = Line.CreateBound(pStart, pEnd);
                    double angle = line.Direction.AngleTo(XYZ.BasisX);
                    XYZ norm = line.Direction.CrossProduct(XYZ.BasisX).Normalize();
                    if (angle - 0.0 < 1e-6)
                    {
                        angle = line.Direction.AngleTo(XYZ.BasisY);
                        norm = line.Direction.CrossProduct(XYZ.BasisY).Normalize();
                    }
                    if (angle - 0.0 < 1e-6)
                    {
                        angle = line.Direction.AngleTo(XYZ.BasisZ);
                        norm = line.Direction.CrossProduct(XYZ.BasisZ).Normalize();
                    }
                    Plane plane = Plane.CreateByNormalAndOrigin
                        (norm, line.Origin);
                    SketchPlane skplane = SketchPlane.Create(doc, plane);
                    ModelCurve newLine = doc.Create.NewModelCurve(line, skplane);
                    tr.Commit();
                }
                catch (Exception ex)
                {
                    tr.RollBack();
                }
            }
        }

        /// <summary>
        /// 可以对齐很不错呢。。。
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="selection"></param>
        private void BreakPipe(Document doc, Selection selection)
        {
            Reference reference = selection.PickObject(ObjectType.Element,new PipeSelection(),"请选择管道");
            Pipe pipe = doc.GetElement(reference) as Pipe;
            LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;

            XYZ pickPoint = reference.GlobalPoint;
            XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;

            //原来是更倾向于右边是
            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Break Pipe");

                //取得管道新的定位基线
                Curve newCurve1 = Line.CreateBound(pipeLocationCurve.Curve.GetEndPoint(0), projectPickPoint);
                Curve newCurve2 = Line.CreateBound(projectPickPoint, pipeLocationCurve.Curve.GetEndPoint(1));

                //复制管道
                Pipe copyPipe = doc.GetElement(ElementTransformUtils.CopyElement(doc, pipe.Id, new XYZ(0, 0, 0)).ElementAt(0)) as Pipe;
                LocationCurve copyPipeLocationCurve = copyPipe.Location as LocationCurve;

                //设置管道的新基线
                pipeLocationCurve.Curve = newCurve1;
                copyPipeLocationCurve.Curve = newCurve2;

                //TaskDialog.Show("1", pipe.Id.ToString());
                //TaskDialog.Show("2", copyPipe.Id.ToString());
                trans.Commit();
                
            }
            
        }

        /// <summary>
        /// 用于划分的
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="selection"></param>
        private void BreakPipe2(Document doc, Selection selection,double length)
        {
            Reference reference = selection.PickObject(ObjectType.Element, new PipeSelection(), "请选择管道");
            Pipe pipe = doc.GetElement(reference) as Pipe;
            LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;

            //1.进行切断
            List<XYZ> newPickList = new List<XYZ>();
            List<XYZ> newPickList2 = new List<XYZ>();

            //2.List<XYZ>

            XYZ pickPoint = reference.GlobalPoint;
            XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;

            //原来是更倾向于右边是
            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Break Pipe");

                //取得管道新的定位基线
                Curve newCurve1 = Line.CreateBound(pipeLocationCurve.Curve.GetEndPoint(0), projectPickPoint);
                Curve newCurve2 = Line.CreateBound(projectPickPoint, pipeLocationCurve.Curve.GetEndPoint(1));

                //复制管道
                Pipe copyPipe = doc.GetElement(ElementTransformUtils.CopyElement(doc, pipe.Id, new XYZ(0, 0, 0)).ElementAt(0)) as Pipe;
                LocationCurve copyPipeLocationCurve = copyPipe.Location as LocationCurve;

                //设置管道的新基线
                pipeLocationCurve.Curve = newCurve1;
                copyPipeLocationCurve.Curve = newCurve2;

                TaskDialog.Show("1", pipe.Id.ToString());
                TaskDialog.Show("2", copyPipe.Id.ToString());
                trans.Commit();

            }

        }
    }

    
}
