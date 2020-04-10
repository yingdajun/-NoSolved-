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
//using Autodesk.Revit.ApplicationServices;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AutoToExcelMaterialDemo
{
    //点分割板  成功完成，哈哈哈哈  428


    [Transaction(TransactionMode.Manual)]
    class BanByPoint : IExternalCommand
    {
        //private Floor floor = null;
        private static double precision = 0.000001;
        ICollection<ElementId> ids_add = new List<ElementId>();
        private Application App = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ////throw new NotImplementedException();
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //点分割板
            BanByPointVer(uidoc, doc, uiapp, app);

            return Result.Succeeded;
        }

        //这里可以迁移到施工段和其他地方
        public void BanByPointVer(UIDocument uidoc,Document doc,UIApplication uiapp,Application app) {

            Reference floorRefer = uidoc.Selection.PickObject(ObjectType.Element, "请选择楼板");
            Element ele = doc.GetElement(floorRefer) as Element;

            Floor floor = ele as Floor;
            ////获取对应的最小轮廓

            //1.获取对应的法向向量
            XYZ point = uidoc.Selection.PickPoint("请选择需要破的点");

            //取得最小的封闭环数列
            //这是啥子
            var faceReferences = HostObjectUtils.GetTopFaces(floor);
            GeometryObject topFaceGeo0 = floor.GetGeometryObjectFromReference(faceReferences[0]);
            Face topFace = topFaceGeo0 as Face;
            var loopList1 = topFace.GetEdgesAsCurveLoops();//最小封闭环

            //Line li = null;
            CurveArray curveArray = new CurveArray();

            CurveArray curveArray1 = new CurveArray();

            List<XYZ> getPoint = new List<XYZ>();
            //每轮转一次 还有 

            //测试
            //XYZ interSectPoint = null;

            //真实直线
            XYZ truePoint = null;

            Curve cu = null;
            Line ci = null;
            //不能光做这个啊

            //获取曲线
            List<Curve> cuList = new List<Curve>();
            foreach (CurveLoop curveloop in loopList1)
            {
                var curveListTmp = curveloop.ToArray();//曲线链分离成曲线数组
                                                       //最重要是获取这个CurveArray

                //阴阳轮转第一步
                foreach (var i in curveListTmp)//数组变组合
                {
                    //curveArray.Append(i);
                    //求得这一点的相交
                    cuList.Add(i);
                    if (i.Distance(point) < 0.01)
                    {
                        ci = i as Line;
                        cu = i;
                        break;

                    }
                }


            }

            //去掉这一点
            cuList.Remove(cu);

            //相当于我把那条边给去掉了 这是上面的

            List<XYZ> allFacePointsList = new List<XYZ>();

            //求相应的交点
            //IntersectionResultArray results;//交点数组

            //新的Curve
            Line cuNew = null;
            IntersectionResultArray intersectionR = new IntersectionResultArray();
            SetComparisonResult comparisonR;
            Line specialLine = null;
            foreach (Curve cu1 in cuList)
            {
                cuNew = cu1 as Line;
                //newLine = Line.CreateUnbound(line.GetEndPoint(0),line.Direction);
                comparisonR = cu1.Intersect(cu, out intersectionR);
                if (SetComparisonResult.Disjoint == comparisonR)//Disjoint不交
                {
                    //求平行线 为啥球了平行线还是不想交
                    specialLine = cuNew;
                    break;
                }


            }

            truePoint = specialLine.Project(point).XYZPoint;

            //也就是同一个嘛？？？
            //这是真的嘛？？？我日，为啥一直是交点
            //（21,12,0） （21,27,0）


            //太短是什么回事
            Line line = Line.CreateBound(point, truePoint);


            //var uiapp = commandData.Application;
            //var app = uiapp.Application;
            
            App = app;
            //var uidoc = uiapp.ActiveUIDocument;

            //var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var acview = doc.ActiveView;

            var linedir = line.Direction;
            var startpo = line.GetEndPoint(0);
            var endpo = line.GetEndPoint(1);
            var updir = XYZ.BasisZ;

            var leftNorm = updir.CrossProduct(linedir).Normalize();
            var rightNorm = updir.CrossProduct(-linedir).Normalize();

            var leftspacePlane = Plane.CreateByNormalAndOrigin(leftNorm, startpo);
            var rightspacePlane = Plane.CreateByNormalAndOrigin(rightNorm, startpo);

            //获取实体
            var geoele = floor.get_Geometry(new Options()
            { ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine });
            Solid solid = null;
            foreach (GeometryObject o in geoele)
            {
                //Point point = o as Point;
                Solid solid1 = o as Solid;
                if (solid1 != null && solid1.Faces.Size > 0)
                {
                    //获取实体
                    solid = solid1;
                    //FindBottomFace(solid);   //得到最底下的边的面积和原点坐标
                    //FindEdge(solid);        //得到12个边的长度

                }
            }
            var newsolid1 = BooleanOperationsUtils.CutWithHalfSpace(solid, leftspacePlane);
            var newsolid2 = BooleanOperationsUtils.CutWithHalfSpace(solid, rightspacePlane);

            IList<Face> upface1Array = new List<Face>();
            upface1Array = GetFacesOfGeometryObject(newsolid1);

            IList<Face> upface2Array = new List<Face>();
            upface2Array = GetFacesOfGeometryObject(newsolid2);


            Face upface1 = null;

            Face upface2 = null;
            //也就是说这里有问题了
            foreach (Face face in upface1Array)
            {

                if (IsSameDirection(face.ComputeNormal(new UV())
                   , XYZ.BasisZ))
                {
                    //XYZ.BasisZ;
                    upface1 = face;
                    break;
                };
            }

            foreach (Face face in upface2Array)
            {
                if (IsSameDirection(face.ComputeNormal(new UV())
                   , XYZ.BasisZ))
                {
                    //XYZ.BasisZ;
                    upface2 = face;
                    break;
                };
            }

            if (upface1 == null)
            {
                TaskDialog.Show("upFace1", "不存在");
            }


            //请问这个怎么又空了呢
            var curveloop1 = upface1.
                GetEdgesAsCurveLoops().FirstOrDefault();

            var curveList = curveloop1.ToArray();

            //var curvearray1 =
            //     curveloop1.ToArray();
            CurveArray curvearray1 = new CurveArray();
            foreach (var i in curveList)//数组变组合
            {
                curvearray1.Append(i);
            }
            if (upface2 == null)
            {
                TaskDialog.Show("upFace2", "不存在");
            }
            var curveloop2 = upface2.GetEdgesAsCurveLoops().FirstOrDefault();
            var curveList2 = curveloop2.ToArray();
            //var curvearray2 =
            //     curveloop2.ToArray();
            CurveArray curvearray2 = new CurveArray();
            foreach (var i in curveList2)//数组变组合
            {
                curvearray2.Append(i);
            }


            Level lev = doc.GetElement(floor.LevelId) as Level;
            Transaction ts = new Transaction(doc, "1");
            ts.Start();
            var newfloor1 = doc.Create.NewFloor(curvearray1, floor.FloorType, lev, false);
            var newfloor2 = doc.Create.NewFloor(curvearray2, floor.FloorType,
                lev, false);
            doc.Delete(floor.Id);
            ts.Commit();
        }

        public static IList<Face> GetFacesOfGeometryObject(GeometryObject geoobj)
        {
            IList<Face> result = new List<Face>();
            List<Face> temresult = new List<Face>();
            if (geoobj is GeometryElement)
            {
                GeometryElement geoele = geoobj as GeometryElement;
                foreach (GeometryObject geoitem in geoele)
                {
                    temresult.AddRange(GetFacesOfGeometryObject(geoitem));
                }
            }
            else if (geoobj is GeometryInstance)
            {
                GeometryElement geoele = (geoobj as GeometryInstance).SymbolGeometry;
                foreach (GeometryObject obj in geoele)
                {
                    if (obj is Solid)
                    {
                        //result.Add(obj as Face);
                        temresult.AddRange(GetFacesOfGeometryObject(obj));
                    }
                }
            }
            else if (geoobj is Solid)
            {
                Solid solid = geoobj as Solid;
                foreach (Face face in solid.Faces)
                {
                    temresult.Add(face);
                }
            }
            else if (geoobj is Face)
            {
                temresult.Add(geoobj as Face);
            }
            result = temresult;
            return result;
        }

        /// <summary>
        /// 判断同向
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        public static bool IsSameDirection(XYZ dir1, XYZ dir2)
        {
            bool result = false;

            double dotproduct = dir1.Normalize().DotProduct(dir2.Normalize());

            if (Math.Abs(dotproduct - 1) < precision)
            {
                result = true;
            }

            return result;
        }
    }
}
