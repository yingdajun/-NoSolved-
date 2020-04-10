using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
namespace AutoToExcelMaterialDemo
{
    [Transaction(TransactionMode.Manual)]
    class tmp:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            //过滤楼板
            FilteredElementCollector floorCollector = new FilteredElementCollector(doc).OfClass(typeof(Floor));
            var floorList = floorCollector.ToElements().ToList();
            Options opt = new Options();

            //获取楼板边界坐标点集合
            List<List<XYZ>> polygons = GetFloorBoundaryPolygons(floorList, opt);

            List<Line> arry = new List<Line>();
            Transaction ts = new Transaction(doc, "CreateLine");
            ts.Start();
            foreach (List<XYZ> loop in polygons)
            {
                for (int i = 0; i < loop.Count; i++)
                {
                    XYZ p = loop[i];
                    if (i != loop.Count - 1)
                    {
                        Line line1 =Line.CreateBound(p, loop[i + 1]);
                        arry.Add(line1);
                    }
                    else
                    {
                        Line line2 = Line.CreateBound(p, loop[0]);
                        arry.Add(line2);
                    }
                }
            }
            ts.Commit();
            TaskDialog.Show("cout",arry.Count.ToString());
            Reference refer2 = uidoc.Selection.PickObject(ObjectType.Element, "轴网");
            Grid grid = doc.GetElement(refer2) as Grid;
            Line Gl = grid.Curve as Line;
            Curve c2 = grid.Curve;
            string tag = null;
            foreach (Line tmp in arry) {
                tag = DetectLines(tmp, Gl);
                TaskDialog.Show(tag, tag);
            }
            return Result.Succeeded;
        }

        public static string DetectLines(Line c1, Line c2)
        {
            string message = "";
            //Curve c1 = (line1.Location as LocationCurve).Curve;
            //Curve c2 = (line2.Location as LocationCurve).Curve;

            SetComparisonResult res = c1.Intersect(c2);
            if (res.Equals(SetComparisonResult.Overlap))
            {
                message = "只有一个交点";
            }
            else if (res.Equals(SetComparisonResult.Subset))
            {
                message = "两条线收尾相连";
            }
            else if (res.Equals(SetComparisonResult.Equal))
            {
                message = "两条线部分重合";
            }
            else if (res.Equals(SetComparisonResult.Disjoint))
            {
                message = "两条线无交点";
            }
            else
            {
                message = "您并无列出此种情况";
            }

            return message;
        }


        private const double _offset = 0.1;
        /// <summary>
        /// 获取楼板的边界坐标点集合
        /// 向下少量偏移
        /// </summary>
        public List<List<XYZ>> GetFloorBoundaryPolygons(List<Element> floors, Options opt)
        {
            List<List<XYZ>> polygons = new List<List<XYZ>>();
            foreach (Floor floor in floors)
            {
                //获取楼板的几何信息
                GeometryElement geo = floor.get_Geometry(opt);
                foreach (GeometryObject obj in geo)
                {
                    Solid solid = obj as Solid;
                    if (solid != null)
                    {
                        GetBoundary(polygons, solid);
                    }
                }
            }
            return polygons;
        }

        /// <summary>
        /// 计算最低水平面边界点坐标
        /// </summary>
        /// <param name="polygons">返回坐标点集合，包含边界与开孔</param>
        /// <param name="solid"></param>
        /// <returns>是否找到最低面</returns>
        private bool GetBoundary(List<List<XYZ>> polygons, Solid solid)
        {
            //最低面
            PlanarFace lowest = null;
            FaceArray faces = solid.Faces;
            foreach (Face f in faces)
            {
                PlanarFace pf = f as PlanarFace;
                if (null != pf && IsHorizontal(pf))
                {
                    if ((null == lowest) || (pf.Origin.Z < lowest.Origin.Z))
                    {
                        lowest = pf;
                    }
                }
            }
            if (null != lowest)
            {
                XYZ p, q = XYZ.Zero;
                bool first;
                int i, n;
                EdgeArrayArray loops = lowest.EdgeLoops;
                foreach (EdgeArray loop in loops)
                {
                    List<XYZ> vertices = new List<XYZ>();
                    first = true;
                    foreach (Edge e in loop)
                    {
                        IList<XYZ> points = e.Tessellate();
                        p = points[0];
                        n = points.Count;
                        q = points[n - 1];
                        for (i = 0; i < n - 1; ++i)
                        {
                            XYZ v = points[i];
                            v -= _offset * XYZ.BasisZ;
                            vertices.Add(v);
                        }
                    }
                    q -= _offset * XYZ.BasisZ;
                    polygons.Add(vertices);
                }
            }
            return null != lowest;
        }
        //是否是水平面
        public bool IsHorizontal(PlanarFace f)
        {
            double eps = 1.0e-9;
            XYZ v = f.FaceNormal;
            return eps > Math.Abs(v.X) && eps > Math.Abs(v.Y);
        }

        public string GetDistance(UIDocument uidoc, Document doc) {
            string tag = null;
            //选择点
            XYZ sel_point = uidoc.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None);

            //获取所有轴网
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            filteredElementCollector.OfClass(typeof(Grid));

            //处理多段轴网及弧形轴网
            //to do...

            //过滤出所有直线轴网
            List<Grid> lineGrid = new List<Grid>();
            foreach (Grid g in filteredElementCollector)
            {
                if ((g.Curve as Line) != null) lineGrid.Add(g);
            }
            //变量;
            Grid grid_n1 = null;
            Grid grid_n2 = null;
            double dis1 = double.MaxValue;
            double dis2 = double.MaxValue;
            //遍历轴网，计算出离选择点最近的一条轴网
            foreach (Grid g in lineGrid)
            {
                if (g.Curve.Distance(sel_point) < dis1)
                {
                    grid_n1 = g;
                    dis1 = g.Curve.Distance(sel_point);
                }
            }
            //遍历轴网，计算出离选择点最近的第二条轴网
            foreach (Grid g in lineGrid)
            {
                if (!(g.Curve as Line).Direction.IsAlmostEqualTo((grid_n1.Curve as Line).Direction) && g.Curve.Distance(sel_point) < dis2)
                {
                    grid_n2 = g;
                    dis2 = g.Curve.Distance(sel_point);
                }
            }

            //显示时将数字结尾的轴网号排在前面
            string name1 = grid_n1.Name;
            string name2 = grid_n2.Name;
            if (!char.IsNumber(name1.Last()))
            {
                string name = name1;
                name1 = name2;
                name2 = name;
            }
            string inputStr = name1 + "轴 交 " + name2 + "轴";
            //显示
            //TaskDialog.Show("goodwish", inputStr);
            tag = inputStr;
            //复制到剪贴板
            //System.Windows.Forms.Clipboard.SetText(inputStr);
            return tag;
        }

    }
}
