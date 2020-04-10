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
using System.Threading;

namespace AutoToExcelMaterialDemo
{
    //轴线编号
    [Transaction(TransactionMode.Manual)]
     class NameByGrid : IExternalCommand
    {
        private const double _offset = 0.1;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //获取柱子
            //明天换成其他柱子玩


            //Reference refer = uidoc.Selection.PickObject(ObjectType.Element, "请选择柱");
            Element ele = null;
                //doc.GetElement(refer);

            //PickedBox pickedBox=
            ColumnSlectionFileter columnSlectionFileter = new ColumnSlectionFileter();
            IList<Reference> columReferList=
            uidoc.Selection.PickObjects(ObjectType.Element,columnSlectionFileter,"选择当前所有的柱子");

            List<FamilyInstance> familyInstancesList = new List<FamilyInstance>();
            FamilyInstance fsi = null;
            Thread thread = new Thread(() => {
                for (var i = 0; i < columReferList.Count; i++)
                {
                    fsi = doc.GetElement(columReferList[i]) as FamilyInstance;
                    familyInstancesList.Add(fsi);
                    //Line line = Line.CreateBound(new XYZ(0, 10 * i, 0), new XYZ(10, 10 * i, 0));
                    //curveArray.Append(line);
                }
            });
            thread.Start();
            thread.Join();

            //Thread thread2 = new Thread(() => {
            //    for (var i = 0; i < familyInstancesList .Count; i++)
            //    {
            //string tag = 0;
            foreach (FamilyInstance tmp in familyInstancesList) {
                ChangeColumnComment(uidoc, doc, tmp);
                //tag = tag + 1;
            }
                    
            //    }
            //});
            //thread2.Start();
            //thread2.Join();




            //获取墙体
            //Reference refer1 = uidoc.Selection.PickObject(ObjectType.Element, "请选择墙");
            //Wall wall = doc.GetElement(refer) as Wall;

            /////生成墙体
            Selection sel = uidoc.Selection;
            Reference ref1 = sel.PickObject(ObjectType.Element, "选择一个墙体");
            Element elem =doc.GetElement(ref1);
            Wall wall = elem as Wall;

            //获取类型
            string wallname = wall.Name;
            //获取标高
            //这个是存在的阿西吧

            //也就是说可以判断是否是纵向相交喽
            TaskDialog.Show("WALL",wall.Name.ToString());
            Level level = doc.GetElement(wall.LevelId) as Level;
            //获取标高位置
            string wallLevelName = level.Name;
            //ALL_MODEL_INSTANCE_COMMENTS

            //老子竟然可以做到，哈哈哈哈
            Parameter wallPara = fsi.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
            Transaction ts1 = new Transaction(doc, "1");
            ts1.Start();
            wallPara.Set(wallLevelName);
            ts1.Commit();
            LocationCurve locationCurve = wall.Location as LocationCurve;
            Line li = locationCurve.Curve as Line;
            Curve c1 = locationCurve.Curve;

            Reference refer2 = uidoc.Selection.PickObject(ObjectType.Element, "轴网");
            Grid grid = doc.GetElement(refer2) as Grid;
            Line Gl = grid.Curve as Line;
            Curve c2 = grid.Curve;
            string curveName = null;

            //循环判断是否有节点
            //那我就反向循环，还和这个是一个位置的
            curveName = DetectLines(c1, c2);
            TaskDialog.Show("demo",curveName);



            return Result.Succeeded;

        }


        public void ChangeColumnComment(UIDocument uidoc, Document doc,FamilyInstance fsi) {
            //这里还有其他的

            //FamilyInstance fsi = ele1 as FamilyInstance;
            //获取类型名称

            string fsiName = fsi.Name;
            Parameter para = fsi.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
            //TaskDialog.Show("title", para.AsString());
            //底部标高 FAMILY_BASE_LEVEL_PARAM
            Parameter levelpara1 = fsi.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM);
            string levelName = levelpara1.AsValueString();
            //TaskDialog.Show(levelName, levelName);
            //获取柱子的column
            LocationPoint locationPoint = fsi.Location as LocationPoint;
            XYZ point = new XYZ(locationPoint.Point.X, locationPoint.Point.Y, locationPoint.Point.Z);
            //轴号
            //楼层名称+轴号
            //轴号+类型名称
            //楼层+轴号+类型名称
            //明天就是整理这些方法的时候了
            //1.使用PickObject方法获取所有柱子的点
            //2.将这些封装在方法里面
            //3.遍历循环使用多线程
            //4.写界面的时候试一试sourceGrid吧
            string tag = GetDistance(uidoc, doc, point);
            Transaction ts = new Transaction(doc, "1");
            ts.Start();
            para.Set(levelName + tag + fsiName);
            ts.Commit();
        }

        /// <summary>
        /// 仅供选择要柱子
        /// </summary>
        public class ColumnSlectionFileter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                if (elem.Category.Name == "柱" )
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
 

        /// <summary>
        /// 存储备份垃圾的点
        /// </summary>
        public void Copy() {

            ///////板子
            //Reference refer3 = uidoc.Selection.PickObject(ObjectType.Element, "获取楼板");
            //Element ele = doc.GetElement(refer3);
            //Reference refer4 = uidoc.Selection.PickObject(ObjectType.Element, "其他轴网");
            //Element grid2 = doc.GetElement(refer2);
            //Floor floor = ele as Floor;
            //Options opt = new Options();
            //opt.ComputeReferences = true;
            //opt.DetailLevel = ViewDetailLevel.Fine;
            //List<List<XYZ>> polygons = new List<List<XYZ>>();
            //GeometryElement geo = floor.get_Geometry(opt);
            //foreach (GeometryObject obj in geo)
            //{
            //    Solid solid = obj as Solid;
            //    if (solid != null)
            //    {
            //        GetBoundary(polygons, solid);
            //    }
            //}

            /////////////////////////////////////////////////////////
            ///
            //Transaction ts2 = new Transaction(doc, "CreateLine");
            //ts2.Start();
            //foreach (List<XYZ> loop in polygons)
            //{
            //    for (int i = 0; i < loop.Count; i++)
            //    {
            //        XYZ p = loop[i];
            //        if (i != loop.Count - 1)
            //        {
            //            Line line1 = doc.NewModelLine(p, loop[i + 1]);
            //            arry.Append(line1);
            //        }
            //        else
            //        {
            //            Line line2 = doc.NewModelLine(p, loop[0]);
            //            arry.Append(line2);
            //        }
            //    }
            //}
            //ts2.Commit();

            ////////////////////////////////////// //////获取楼板边界

            //FamilyInstance fsi2 = ele as FamilyInstance;
            //LocationCurve lc = fsi2.Location as LocationCurve;
            //Curve beamLi = lc.Curve;

            //Reference refer4 = uidoc.Selection.PickObject(ObjectType.Element, "其他轴网");
            //Grid grid2 = doc.GetElement(refer2) as Grid;
            //Curve grid2Curve = grid2.Curve;
            //string curveName2 = null;

            ////循环判断是否有节点
            //curveName2 = DetectLines(beamLi, grid2Curve);
            //TaskDialog.Show("demo2", curveName2);

            ////获取梁  战书放弃

            ///我是不是应该做一个投影


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

        /// <summary>
        /// 获取墙体的标高
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public string GetDistance(UIDocument uidoc, Document doc,XYZ sel_point)
        {
            string tag = null;
            //选择点
            //XYZ sel_point = uidoc.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None);

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
            string inputStr = name1 + "#" + name2;
            //显示
            //TaskDialog.Show("goodwish", inputStr);
            tag = inputStr;
            //复制到剪贴板
            //System.Windows.Forms.Clipboard.SetText(inputStr);
            return tag;
        }

        /// <summary>
        /// 获取相交元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="category"></param>
        /// <param name="contain"></param>
        /// <returns></returns>
        public static  List<Element> GetIntersectElements(Element element, BuiltInCategory category, bool contain = false)
        {
            List<Element> listElement = new List<Element>();
            Document doc = element.Document;
            BoundingBoxXYZ boundingBoxXyz = element.get_BoundingBox(doc.ActiveView);
            Outline outline = new Outline(boundingBoxXyz.Min, boundingBoxXyz.Max);
            BoundingBoxIntersectsFilter boundFilter = new BoundingBoxIntersectsFilter(outline);
            ElementCategoryFilter filter2 = new ElementCategoryFilter(category);
            LogicalAndFilter filter3 = new LogicalAndFilter(boundFilter, filter2);
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            listElement = collector.WherePasses(filter3).WhereElementIsNotElementType().ToList();

            if (contain == false)
            {
                listElement.RemoveAll(m => m.Id == element.Id);
            }

            return listElement;
        }



        //获取所有的Point
        private bool CheckPoint(List<XYZ> points, XYZ point)
        {
            bool flag = false;
            foreach (XYZ p in points)
            {
                if (p.IsAlmostEqualTo(point))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        //获取柱子高度
        public Double GetColumHeight(Element elem,Document doc) //elem为柱构件
        {
            Double mHeight = 0;

            Double b_value = 0;
            Double t_value = 0;
            if (elem != null)
            {
                //获取顶标高
                Parameter topLevel = elem.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);
                ElementId ip = topLevel.AsElementId();
                Level top = doc.GetElement(ip) as Level;
                t_value = top.ProjectElevation;
                //获取底标高 
                Parameter BotLevel = elem.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
                ElementId bip = BotLevel.AsElementId();
                Level bot = doc.GetElement(bip) as Level;
                b_value = bot.ProjectElevation;

                mHeight = (t_value - b_value);//英尺转毫米
            }
            return mHeight;
        }


        public static string DetectLines(Curve c1, Curve c2)
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
                //有可能就是共面不平行
                message = "两条线无交点";
            }
            else
            {
                message = "您并无列出此种情况";
            }

            return message;
        }

        /// <summary>
        /// 判断
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static string DetectLines(ModelLine line1, ModelLine line2)
        {
            string message = "";
            Curve c1 = (line1.Location as LocationCurve).Curve;
            Curve c2 = (line2.Location as LocationCurve).Curve;

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


    }


}
