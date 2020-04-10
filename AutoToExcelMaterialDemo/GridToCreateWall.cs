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
    //轴线生成墙体  部分完成 
    [Transaction(TransactionMode.Manual)]
    class GridToCreateWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FilteredElementCollector gridFilter = new FilteredElementCollector(doc);
            // 获取所有的轴网
            List<Grid> allGrids = new List<Grid>();
            gridFilter.OfClass(typeof(Grid)).Cast<Grid>().ToList();

            //FilteredElementCollector gridCollector = new FilteredElementCollector(doc);
            //gridCollector.OfCategory(BuiltInCategory.OST_Grids);



            //////////////////////////////////////////////////////////////////
            Grid gridTmp = null;
            foreach (Element ele in gridFilter)
            {
                gridTmp = ele as Grid;
                allGrids.Add(gridTmp);
            }
            TaskDialog.Show("1", "1");
            //////速度有点慢，其他都还好


            CreateWall(allGrids, uidoc, doc);
            CreateWall(allGrids, uidoc, doc);
            CreateWall(allGrids, uidoc, doc);
            CreateWall(allGrids, uidoc, doc);


            //基线类型  1,2,3 不知道是啥

            //确认是否按照多楼层切分墙（看看句芒的插件）

            //确认定位线（看看小火车）

            //读取当前的定高和标高搞在一起 

            FilteredElementCollector levelcollector = new FilteredElementCollector(doc);
            levelcollector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();
            string s = null;
            foreach (Element ele in levelcollector) {
                s = s + ele.Name;
            }
            TaskDialog.Show("s",s.ToString());

            //墙的类型

            //WallType wallType = null;

            //FilteredElementCollector wallTypeCollector = new FilteredElementCollector(doc);
            //wallTypeCollector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType();

            ////默认是建筑墙体
            //bool flag = false; //这是建筑墙
            ////Wall wall=Wall.Create(doc,null,)
            
            
            //按照轴线交点拆分墙
            //这样因此将会在其中获取
            return Result.Succeeded;
        }

        public void CreateWall(List<Grid> allGrids,UIDocument uidoc,Document doc) {

            //基线模式
            //获取轴网的所有交点
            List<XYZ> Points = new List<XYZ>();
            double dis = 0.0;
            //选择点,转化模式的时候可以用

            //这儿是啥子鬼
            XYZ xyzSelect =
            uidoc.Selection.PickPoint();
            string sTag = null;
            Dictionary<string, XYZ> pointDict = new Dictionary<string, XYZ>();
            //创建sketchPlane
            //SketchPlane modelSketch1 = null;
            //Line line1 = null;

            Dictionary<string, double> distanceList = new Dictionary<string, double>();
            List<double> distance = new List<double>();
            foreach (Grid grid in allGrids)
            {
                Grid currentGrid = grid;
                foreach (Grid grd in allGrids)
                {
                    IntersectionResultArray ira = null;
                    SetComparisonResult scr = currentGrid.Curve.Intersect(grd.Curve, out ira);
                    if (ira != null)
                    {
                        IntersectionResult ir = ira.get_Item(0);
                        // 判断点是否重复
                        if (!CheckPoint(Points, ir.XYZPoint))
                        {
                            //这个可以用作相关内容的点
                            sTag = sTag + grid.Name + "/" + grd.Name;
                            pointDict.Add(sTag, ir.XYZPoint);
                            dis = Calculate(xyzSelect.X, xyzSelect.Y, xyzSelect.Z,
                    ir.XYZPoint.X, ir.XYZPoint.Y, ir.XYZPoint.Z);
                            //添加数量，然后根据值去判断
                            distance.Add(dis);
                            distanceList.Add(sTag, dis);
                            Points.Add(ir.XYZPoint);
                        }
                    }
                }
            }


            //////////////////////////////////////////////////////

            //////////////////////////////////////

            //判断两个不是
            double minFirst = distance.Min();
            distance.Remove(minFirst);
            double minSecond = distance.Min();

            //一般只有两种情况,一种是完全相等，一种是完全不相等
            List<string> keyList = (from q in distanceList
                                    where q.Value == minFirst
                                    select q.Key).ToList<string>();
            List<string> keyList1 = (from q in distanceList
                                     where q.Value == minSecond
                                     select q.Key).ToList<string>();
            //TaskDialog.Show("key1",keyList.Count.ToString());
            //TaskDialog.Show("key2", keyList1.Count.ToString());
            XYZ tmp1 = null;
            foreach (string s1 in keyList)
            {
                tmp1 = pointDict[s1];
            }
            XYZ tmp2 = null;
            foreach (string s1 in keyList1)
            {
                tmp2 = pointDict[s1];
            }
            //////////////////////////////////

            //get all keys

            //for (int i=0;i<Points.Count;i++) {

            //}


            //生成轴网一下的点
            Transaction ts = new Transaction(doc, "1");
            ts.Start();
            Level level = doc.GetElement(new ElementId(311)) as Level;
            Line li = Line.CreateBound(tmp1, tmp2);
            Wall wall = Wall.Create(doc, li, level.Id, false);
            ts.Commit();
        }

        /// <summary> 
        /// 计算两个点之间的距离
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <returns></returns>
        public static double Calculate(double x0, double y0, double? z0, double x1, double y1, double? z1)
        {
            double dSquareSum = 0;
            bool bHasZ = z0.HasValue && z1.HasValue;
            dSquareSum = Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2);
            dSquareSum += bHasZ ? Math.Pow(z0.Value - z1.Value, 2) : 0;
            return Math.Sqrt(dSquareSum);
        }

        //获取所有轴网
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

    }
}
