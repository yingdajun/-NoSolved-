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
using Autodesk.Revit.DB.Mechanical;

namespace AutoToExcelMaterialDemo
{
    //电缆桥架人防开洞  380
    [Transaction(TransactionMode.Manual)]
    class ReFangKaiDong : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            //UIDocument uidoc = commandData.Application.ActiveUIDocument;
            //Document doc = uidoc.Document;
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            Selection sel = uiApp.ActiveUIDocument.Selection;

            Transaction ts = new Transaction(doc, "revit.5d6d.com");
            ts.Start();
            /*
            //选择一面墙
            WallSelectionFilter fWall = new WallSelectionFilter();
            Reference ref1 = uiApp.ActiveUIDocument.Selection.PickObject(ObjectType.Element, fWall, "选择一面墙：");
            Element elem1 = doc.GetElement(ref1);
            Wall wall = elem1 as Wall;
            //选择一个风管
            DuctSelectionFilter fDuct = new DuctSelectionFilter();
            Reference ref2 = uiApp.ActiveUIDocument.Selection.PickObject(ObjectType.Element, fDuct, "选择一个风管：");
            Element elem2 = doc.GetElement(ref2);
            Duct duct = elem2 as Duct;
             */
            //开同心洞
            //CenterOpen(doc, wall, duct, 640, 640);
            List<Duct> listDuct = FindAllDuct(doc);
            foreach (Duct duct in listDuct)
            {
                List<Wall> listWall = FindDuctWall(doc, duct);
                foreach (Wall wall in listWall)
                {
                    CenterOpen(doc, wall, duct, 640, 640);
                }
            }

            ts.Commit();

            return Result.Succeeded;
        }

        //找到所有风管
        List<Duct> FindAllDuct(Document doc)
        {
            List<Duct> listDuct = new List<Duct>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Duct)).OfCategory(BuiltInCategory.OST_DuctCurves);

            foreach (Element el in collector)
            {
                Duct duct = el as Duct;
                if (duct != null)
                    listDuct.Add(duct);
            }
            return listDuct;
        }
        //找到与风管相交的墙
        List<Wall> FindDuctWall(Document doc, Duct duct)
        {
            List<Wall> listWall = new List<Wall>();
            //找到outLine
            BoundingBoxXYZ bb = duct.get_BoundingBox(doc.ActiveView);
            Outline outline = new Outline(bb.Min, bb.Max);
            //
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            BoundingBoxIntersectsFilter invertFilter = new BoundingBoxIntersectsFilter(outline, false);
            IList<Element> noIntersectWalls = collector.OfClass(typeof(Wall)).WherePasses(invertFilter).ToElements();
            foreach (Element el in noIntersectWalls)
            {
                Wall wall = el as Wall;
                if (wall != null)
                    listWall.Add(wall);
            }
            return listWall;
        }
        //开同心洞
        Result CenterOpen(Document doc, Wall wall, Duct duct, double dWidth, double dHeigh)
        {
            SubTransaction subTs = new SubTransaction(doc);
            subTs.Start();
            try
            {
                //求面和线的交点
                Face face = FindWallFace(wall);
                Curve curve = FindDuctCurve(duct);
                XYZ xyz = FindFaceCurve(face, curve);
                //墙线的向量
                XYZ wallVector = FindWallVector(wall);
                //交点向上向墙线正方向移动160(风管宽高320)
                XYZ pt1 = xyz + new XYZ(0, 0, 1) * dHeigh / 2 / 304.8;
                pt1 = pt1 + wallVector.Normalize() * dWidth / 2 / 304.8;
                //交点向下向墙线反方向移动160(风管宽高320)
                XYZ pt2 = xyz + new XYZ(0, 0, -1) * dHeigh / 2 / 304.8;
                pt2 = pt2 - wallVector.Normalize() * dWidth / 2 / 304.8;
                //开洞
                doc.Create.NewOpening(wall, pt1, pt2);

                subTs.Commit();
                return Result.Succeeded;
            }
            catch
            {
                subTs.RollBack();
                return Result.Failed;
            }
        }
        //找到墙线的向量
        XYZ FindWallVector(Wall wall)
        {
            LocationCurve lCurve = wall.Location as LocationCurve;
            XYZ xyz = lCurve.Curve.GetEndPoint(1) - lCurve.Curve.GetEndPoint(0);
            return xyz;
        }
        //求线和面的交点
        XYZ FindFaceCurve(Face face, Curve curve)
        {
            //求交点
            IntersectionResultArray intersectionR = new IntersectionResultArray();//交点集合
            SetComparisonResult comparisonR;//Comparison比较
            comparisonR = face.Intersect(curve, out intersectionR);
            XYZ intersectionResult = null;//交点坐标
            if (SetComparisonResult.Disjoint != comparisonR)//Disjoint不交
            {
                if (!intersectionR.IsEmpty)
                {
                    intersectionResult = intersectionR.get_Item(0).XYZPoint;
                }
            }
            return intersectionResult;
        }
        //找到风管对应的曲线
        Curve FindDuctCurve(Duct duct)
        {
            //得到风管曲线
            IList<XYZ> list = new List<XYZ>();
            ConnectorSetIterator csi = duct.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                list.Add(conn.Origin);
            }
            Curve curve = Line.CreateBound(list.ElementAt(0), list.ElementAt(1)) as Curve;
            return curve;
        }
        //找到墙的正面
        Face FindWallFace(Wall wall)
        {
            Face normalFace = null;
            //
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel =ViewDetailLevel.Medium;
            //
            GeometryElement e = wall.get_Geometry(opt);
            //e.
            foreach (GeometryObject obj in e)
            {
                Solid solid = obj as Solid;
                if (solid != null && solid.Faces.Size > 0)
                {
                    foreach (Face face in solid.Faces)
                    {
                        PlanarFace pf = face as PlanarFace;
                        if (pf != null)
                        {
                            if (pf.FaceNormal.AngleTo(wall.Orientation) < 0.01)//数值在0到PI之间
                            {
                                normalFace = face;
                            }
                        }
                    }
                }
            }
            return normalFace;
        }
    }
}


