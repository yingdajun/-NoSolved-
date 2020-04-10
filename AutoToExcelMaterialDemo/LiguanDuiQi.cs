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
    //立管对齐  435 已完成
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class LiguanDuiQi : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Selection selection = uidoc.Selection;

            //CreatePipe(doc, selection,2);
            CreatePipeByLine(doc, selection);

            //原来你是建立约束的
            //Align2PipesViaCenterline(pipe, pipe1);
            //垂直一列

            //使用移动命令

            //没啥卵用

            //对应一列
            return Result.Succeeded;
        }

        public void CreatePipe(Document doc, Selection selection, int flag) {
            //pipe1
            PipeSelection pipeSelection = new PipeSelection();

            Reference reference = selection.PickObject(ObjectType.Element,pipeSelection, "请选择管道");
            Pipe pipe = doc.GetElement(reference) as Pipe;
            LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
            //pipe2
            Reference reference1 = selection.PickObject(ObjectType.Element,pipeSelection, "请选择作为标准的管道");
            Pipe pipe1 = doc.GetElement(reference1) as Pipe;
            LocationCurve pipeLocationCurve1 = pipe1.Location as LocationCurve;

            //获取系统类型
            ElementId pipeSystemId = pipe.get_Parameter(BuiltInParameter
                .RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();
            //pipe.MEPSystem.Id;
            //获取类型ID 

            //获取类型ID
            ElementId pipeTypeId = pipe.PipeType.Id;

            ElementId levelId = pipe.get_Parameter(BuiltInParameter
                .RBS_START_LEVEL_PARAM).AsElementId();
            Transaction ts = new Transaction(doc, "水平对齐");
            ts.Start();
            
            //水平
            if (flag == 0) {
                Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                new XYZ(pipeLocationCurve.Curve.GetEndPoint(0).X,
                                pipeLocationCurve1.Curve.GetEndPoint(0).Y
                                , pipeLocationCurve.Curve.GetEndPoint(0).Z
                                ), new XYZ(
                                    pipeLocationCurve.Curve.GetEndPoint(1).X,
                                pipeLocationCurve1.Curve.GetEndPoint(1).Y
                                , pipeLocationCurve.Curve.GetEndPoint(1).Z
                                    )
                                );
                doc.Delete(pipe.Id);

            }
            //垂直
            if (flag == 1) {
                Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                new XYZ(pipeLocationCurve1.Curve.GetEndPoint(0).X,
                pipeLocationCurve.Curve.GetEndPoint(0).Y
                , pipeLocationCurve.Curve.GetEndPoint(0).Z
                ), new XYZ(
                    pipeLocationCurve1.Curve.GetEndPoint(1).X,
                pipeLocationCurve.Curve.GetEndPoint(1).Y
                , pipeLocationCurve.Curve.GetEndPoint(1).Z
                    )
                );
                doc.Delete(pipe.Id);
            }
           
            ts.Commit();
        }

        public void CreatePipeByLine(Document doc,Selection selection) {
            PipeSelection pipeSelection = new PipeSelection();
            Reference reference = selection.PickObject(ObjectType.Element,pipeSelection
                , "请选择管道");
            Pipe pipe = doc.GetElement(reference) as Pipe;
            LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
            ////pipe2
            //Reference reference1 = selection.PickObject(ObjectType.Element, "请选择作为标准的管道");
            //Pipe pipe1 = doc.GetElement(reference1) as Pipe;
            //LocationCurve pipeLocationCurve1 = pipe1.Location as LocationCurve;

            //获取系统类型
            ElementId pipeSystemId = pipe.get_Parameter(BuiltInParameter
                .RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();
            //pipe.MEPSystem.Id;
            //获取类型ID 

            //获取类型ID
            ElementId pipeTypeId = pipe.PipeType.Id;

            ElementId levelId = pipe.get_Parameter(BuiltInParameter
                .RBS_START_LEVEL_PARAM).AsElementId();


            ////BreakPipe(doc, selection);


            //XYZ translation = new XYZ();
            //pipeLocationCurve1.Curve.GetEndPoint(0)-
            //pipeLocationCurve.Curve.GetEndPoint(0);
            Transaction ts = new Transaction(doc, "水平对齐");
            ts.Start();

            XYZ point1 = selection.PickPoint("第一点");
            XYZ point2 = selection.PickPoint("第二点");
            //这是直线
            Line li = Line.CreateBound(point1, point2);
            Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
            new XYZ(
               li.Project(pipeLocationCurve.Curve.GetEndPoint(0)).XYZPoint
                .X,
             li.Project(pipeLocationCurve.Curve.GetEndPoint(0)).XYZPoint
             .Y
            , pipeLocationCurve.Curve.GetEndPoint(0).Z
            ), new XYZ(
               li.Project(pipeLocationCurve.Curve.GetEndPoint(1)).XYZPoint
                .X,
             li.Project(pipeLocationCurve.Curve.GetEndPoint(1)).XYZPoint
             .Y
            , pipeLocationCurve.Curve.GetEndPoint(1).Z
                )
            );
            doc.Delete(pipe.Id);
            ts.Commit();
        }

        /// <summary>
        /// 可以对齐很不错呢。。。
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="selection"></param>
        private void BreakPipe(Document doc, Selection selection)
        {
            Reference reference = selection.PickObject(ObjectType.Element);
            Pipe pipe = doc.GetElement(reference) as Pipe;
            LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;

            XYZ pickPoint = reference.GlobalPoint;
            XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;

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

                trans.Commit();
            }
        }


        /// <summary>
        /// 取得平行线的单位向量
        /// </summary>
        /// <param name="line0"></param>
        /// <param name="line1"></param>
        /// <returns></returns>
        public XYZ GetXYVector(Line line0, Line line1)
        {
            //基准
            XYZ startPoint0 = line0.GetEndPoint(0);
            XYZ endPoint0 = line0.GetEndPoint(1);
            //参照
            XYZ startPoint1 = line1.GetEndPoint(0);
            XYZ endPoint1 = line1.GetEndPoint(1);
            //
            XYZ resultVector = new XYZ(0, 0, 0);

            XYZ zStart = new XYZ(startPoint1.X, startPoint1.Y, 0);
            XYZ zEnd = new XYZ(endPoint1.X, endPoint1.Y, 0);

            Line line = Line.CreateBound
                (new XYZ(startPoint0.X, startPoint0.Y, 0), new XYZ(endPoint0.X, endPoint0.Y, 0));
            line.MakeUnbound();

            //取得一条直线所在直线相垂直的直线
            XYZ tPoint = zStart - zEnd;
            XYZ zVector = new XYZ(0, 0, 1);
            XYZ nPoint = tPoint.CrossProduct(zVector).Normalize() + zStart;
            Line lineN = Line.CreateBound(zStart, nPoint);
            lineN.MakeUnbound();

            //与另外一条风管的交点
            IntersectionResultArray intersectionR = new IntersectionResultArray();
            SetComparisonResult comparisonR;

            comparisonR = line.Intersect(lineN, out intersectionR);

            if (SetComparisonResult.Disjoint != comparisonR)
            {
                if (!intersectionR.IsEmpty)
                {
                    resultVector = intersectionR.get_Item(0).XYZPoint;
                }
            }

            if (!zStart.IsAlmostEqualTo(resultVector))
            {
                resultVector = (resultVector - zStart).Normalize();
            }

            return resultVector;
        }

        //这里假定两个管道是平行的
        //假定当前视图是平面视图
        private static Dimension
            Align2PipesViaCenterline(Pipe pipeBase, Pipe pipe)
        {
            Dimension dimension = null;
            Document doc = pipeBase.Document;
            View view = doc.ActiveView;
            Line baseLine = GetCenterline(pipeBase);
            if (baseLine == null) return null;
            Line line = GetCenterline(pipe);
            if (line == null) return null;
            var clone = line.Clone();
            clone.MakeUnbound();
            IntersectionResult result = clone.Project(baseLine.Origin);
            if (result != null)
            {
                var point = result.XYZPoint;
                var translate = baseLine.Origin - point;
                using (Transaction transaction = new Transaction(doc))
                {
                    try
                    {
                        transaction.Start("Align pipes");
                        ElementTransformUtils.MoveElement(
                            doc, pipe.Id, translate);
                        dimension = doc.Create.NewAlignment(view,
                            baseLine.Reference, line.Reference);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        //Trace.WriteLine(ex.ToString());
                        transaction.RollBack();
                    }
                }
            }
            return dimension;
        }

        private static Line GetCenterline(Pipe pipe)
        {
            Options options = new Options();
            options.ComputeReferences = true; //!!!
            options.IncludeNonVisibleObjects = true; //!!! 
            if (pipe.Document.ActiveView != null)
                options.View = pipe.Document.ActiveView;
            else
                options.DetailLevel = ViewDetailLevel.Fine;
            var geoElem = pipe.get_Geometry(options);
            foreach (var item in geoElem)
            {
                Line lineObj = item as Line;
                if (lineObj != null)
                {
                    return lineObj;
                }
            }
            return null;
        }
    }

    public class PipeSelection : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            //throw new NotImplementedException();
            if (elem.Category.Name== "管道") {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            //throw new NotImplementedException();

            return false;
        }
    }
}
