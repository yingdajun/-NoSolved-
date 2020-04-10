using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
namespace AutoToExcelMaterialDemo
{
    //柱子对齐墙边  成功完成 409
    [Transaction(TransactionMode.Manual)]
    class ColumnAlToWall:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Reference refer=
            uidoc.Selection.PickObject(ObjectType.Element,"请选择墙体");

            ////哈哈哈哈，老子有了这个就可以解决这个问题了
            //Reference refer2 =
            //uidoc.Selection.PickObject(ObjectType.Edge, "请选择墙体的线");
            Wall wall = doc.GetElement(refer) as Wall;
            LocationCurve locationCure = wall.Location as LocationCurve;
            Line li = locationCure.Curve as Line;

            //将维度降到和point一致

            //失败。。。。阿西吧
            Reference refer1 =
            uidoc.Selection.PickObject(ObjectType.Element, "请选择柱子");
            FamilyInstance fsi = doc.GetElement(refer1) as FamilyInstance;
            
            
            LocationPoint point = fsi.Location as LocationPoint;


            //墙线
            //Wall wall = null;
            XYZ translate = null;
            Line locationLine = (wall.Location as LocationCurve).Curve as Line;
            //柱子的点
            //XYZ columnPoint = null;
            //还是不熊
            if (locationLine != null)
            {
                //投影
                Line projectLine = Line.CreateUnbound(locationLine.GetEndPoint(0), locationLine.Direction);
                //投影点
                XYZ projectPoint = projectLine.Project(point.Point).XYZPoint;
                translate = projectPoint-point.Point;
            }
            Transaction ts = new Transaction(doc, "1");
            ts.Start();
            ElementTransformUtils.MoveElement(
                    doc, fsi.Id, translate);
            //doc.Create.NewAlignment(doc.ActiveView,refer,refer1);
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

        //public List<PlanarFace> GetWallFaces(Wall wall)
        //{
        //    Options opt = new Options();
        //    opt.ComputeReferences = true;
        //    opt.DetailLevel = DetailLevels.Medium;
        //    GeometryElement ge = wall.get_Geometry(opt);
        //    List<PlanarFace> lstpf = new List<PlanarFace>();
        //    foreach (GeometryObject obj in ge.Objects)
        //    {
        //        Solid solid = obj as Solid;
        //        if (solid != null)
        //        {
        //            foreach (Face face in solid.Faces)
        //            {
        //                PlanarFace pf = face as PlanarFace;
        //                if (pf != null)
        //                {
        //                    if (pf.Normal.CrossProduct(wall.Orientation).IsZeroLength())
        //                    {
        //                        lstpf.Add(pf);
        //                    }
        //                }
        //            }
        //            return lstpf;
        //        }
        //    }
        //    return null;
        //}


        //////////////////////////////////////////
        ///
        public PlanarFace GetWallSideFace(Wall wall, ShellLayerType slt)
        {
            Reference reference = HostObjectUtils.GetSideFaces(wall, slt)[0];
            PlanarFace face =
        wall.GetGeometryObjectFromReference(reference) as PlanarFace;
            return face;
        }


        /// <summary>
        /// 仅供选择要柱子
        /// </summary>
        public class ColumnSlectionFileter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                if (elem.Category.Name == "柱")
                {
                    return true;
                }
                return false;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }
    }
   

}
