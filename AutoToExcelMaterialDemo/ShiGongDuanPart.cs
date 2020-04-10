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
using System.IO;
using Autodesk.Revit.DB.Structure;
using System.Windows.Forms;

namespace AutoToExcelMaterialDemo
{
    //施工段划分 基本完成部分 417
    [Transaction(TransactionMode.Manual)]
    class ShiGongDuanPart : IExternalCommand
    {
        public StructuralType Struct { get; private set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;


            //单独选择楼板模式。
            //掌握了这个程序以后，以后关于那个横向纵向切轴网我也清楚了
            //两个标记我也清楚了，啊啊啊
            CreateFloorFinal(uidoc, doc);


            //获取所有的墙


            //获取所有的楼板

            //获取所有的梁

            //获取所有的标高
            return Result.Succeeded;
        }


        /// <summary>
        /// 获取族文件位置
        /// </summary>
        /// <returns></returns>
        private FileSystemInfo PickFolderInfo()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择族文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return new DirectoryInfo(dialog.SelectedPath);
            }
            return null;
        }

        //原始生成Floor方法1
        public void CreateFloor(UIDocument uidoc, Document doc) {
            //FilteredElementCollector collector = new FilteredElementCollector(doc);

            //生成墙体
            //CreateWall(uidoc, doc);

            //生成梁
            //CreateBeam(uidoc, doc);  //这里的问题

            Reference refer = uidoc.Selection.PickObject(ObjectType.Element, "请选择楼板");
            Floor floor = doc.GetElement(refer) as Floor;

            //取得最小的封闭环数列
            var faceReferences1 = HostObjectUtils.GetTopFaces(floor);
            GeometryObject topFaceGeo1 = floor.GetGeometryObjectFromReference(faceReferences1[0]);
            Face topFace1 = topFaceGeo1 as Face;
            var loopList = topFace1.GetEdgesAsCurveLoops();//最小封闭环

            Level level = doc.GetElement(floor.LevelId) as Level;
            FloorType floorType = floor.FloorType;

            //根据封闭的曲线数组进行新的楼板创建
            //第一条是轴线，可能会比较难，我要一条一条的测试

            List<CurveArray> cuArray = new List<CurveArray>();
            //这是轴网链接
            GetGridSelect getGridSelect = new GetGridSelect();
            IList<Reference> getGrid = uidoc.Selection.PickObjects(ObjectType.Element, getGridSelect
                , "用于切割的轴网");

            List<Curve> gridList = new List<Curve>();
            //Curve ci = null;
            Grid gi = null;
            foreach (Reference re in getGrid)
            {
                gi = doc.GetElement(re) as Grid;
                gridList.Add(gi.Curve);
            }

            int cout = 0;
            //根据封闭的曲线数组进行新的楼板创建
            using (Transaction tran1 = new Transaction(doc))
            {
                tran1.Start("split");
                doc.Delete(floor.Id);//删除旧楼板

                XYZ point1 = null;
                XYZ point2 = null;
                XYZ fi = null;
                XYZ fn = null;

                //存储最终节点的n1
                XYZ n1 = null;
                //存储最终节点的n2
                XYZ n2 = null;
                //加一个连接点
                //加一个点存储最初的代码
                XYZ tmp1 = null; //存储上面的交点1
                XYZ tmp2 = null; //存储上面的交点2

                //这是最初的first0 
                XYZ fistn1 = null;
                //这是最初的first1
                XYZ fistn2 = null;

                List<string> tagList = new List<string>();

                //这是第一个节点和第二个节点的位置
                XYZ x1, x2, x3, x4 = null;

                //XYZ tmpX1,tmpX2 = null;
                XYZ d1 = null;
                XYZ d2 = null;
                XYZ d3 = null;
                XYZ d4 = null;
                //把上一条的插件点准备到最后一条去
                foreach (Curve cu in gridList)
                {

                    cout = cout + 1;

                    //只要是两个以上的就开始报错，MMP的

                    //那就按照最原始的方式搞起
                    if (cout == 1)
                    {
                        foreach (CurveLoop curveloop in loopList)
                        {
                            var curveList = curveloop.ToArray();//曲线链分离成曲线数组
                                                                //最重要是获取这个CurveArray
                            CurveArray curveArray = new CurveArray();
                            //每轮转一次 还有 

                            int j = 0;
                            //阴阳轮转第一步
                            foreach (var i in curveList)//数组变组合
                            {
                                //curveArray.Append(i);
                                //cout = cout + 1;

                                //如果是相交的
                                if (GetXYZ(cu, i) != null)
                                {

                                    j = j + 1;

                                    if (j == 1)
                                    {
                                        //这是交点
                                        //这是唯一变化的
                                        point1 = GetXYZ(cu, i);
                                        //这是最初点
                                        tmp1 = point1; //把相应的值赋予进去
                                        fi = i.GetEndPoint(0);
                                        //
                                        fistn1 = fi;
                                        //这是末尾的最初点
                                        n1 = i.GetEndPoint(1);
                                        d3 = n1;
                                        //获取相应的节点
                                        //这是第一步
                                        //x1_list.Add(fi);
                                        //x2_list.Add(point1);
                                        //添加List
                                        //相应节点2 


                                    }
                                    else
                                    {
                                        //这是交点
                                        //这是唯一变化的
                                        point2 = GetXYZ(cu, i);
                                        tmp2 = point2;  //把相应的值给赋予进去
                                                        //这是最初点
                                        fn = i.GetEndPoint(1);
                                        fistn2 = fn;
                                        n2 = i.GetEndPoint(0);
                                        d4 = n2;
                                        //获取相应的节点
                                        //这是第一步
                                        //jClassDemo = new JClassDemo(new XYZ(), new XYZ(), new XYZ(), new XYZ());
                                        //添加List
                                        //相应节点2 


                                        //当前的最后一点
                                        //x4_list.Add(fn);
                                        //x3_list.Add(point2);
                                        //jiClassDemoList.Add(jClassDemo);
                                        //jiClassDemoListTmp.Add(jClassDemo);
                                    }


                                }

                            }


                            //this.intialPoint0 = n1;
                            //this.intialPoint1 = n2;
                            //this.fristPoint = n3;
                            //this.secondPoint = n4;

                            x1 = point1;
                            x2 = fi;
                            x3 = fn;
                            x4 = point2;
                            ////可能是输入不准确
                            //curveArray.Append(Line.CreateBound(point1, fi));
                            ////fn是最初的点
                            //curveArray.Append(Line.CreateBound(fi, fn));
                            //curveArray.Append(Line.CreateBound(fn, point2));
                            //curveArray.Append(Line.CreateBound(point2, point1));

                            //可能是输入不准确
                            curveArray.Append(Line.CreateBound(x1, x2));
                            //fn是最初的点
                            curveArray.Append(Line.CreateBound(x2, x3));
                            curveArray.Append(Line.CreateBound(x3, x4));
                            curveArray.Append(Line.CreateBound(x4, x1));

                            Floor newfloor = doc.Create.NewFloor(curveArray, floorType, level, false);
                            j = 0;
                            //把第一次的结果给弄下来
                            d1 = x1;
                            d2 = x4;
                        }
                        //相当于废物点
                        ;

                        //这个是结尾

                    }
                    else
                    {

                        //如果是最后的边，那么x4就是之前的x3
                        if (cout == gridList.Count)
                        {
                            foreach (CurveLoop curveloop in loopList)
                            {
                                var curveList = curveloop.ToArray();//曲线链分离成曲线数组
                                                                    //最重要是获取这个CurveArray
                                CurveArray curveArray = new CurveArray();
                                //每轮转一次 还有 

                                int j = 0;
                                //阴阳轮转第一步
                                foreach (var i in curveList)//数组变组合
                                {
                                    //curveArray.Append(i);
                                    //cout = cout + 1;

                                    //如果是相交的
                                    if (GetXYZ(cu, i) != null)
                                    {

                                        j = j + 1;

                                        if (j == 1)
                                        {
                                            //这是交点
                                            //这是唯一变化的
                                            point1 = GetXYZ(cu, i);
                                            //这是最初点
                                            tmp1 = point1; //把相应的值赋予进去
                                            fi = i.GetEndPoint(0);
                                            //
                                            fistn1 = fi;
                                            //这是末尾的最初点
                                            n1 = i.GetEndPoint(1);
                                            //获取相应的节点
                                            //这是第一步
                                            //x1_list.Add(fi);
                                            //x2_list.Add(point1);
                                            //添加List
                                            //相应节点2 


                                        }
                                        else
                                        {
                                            //这是交点
                                            //这是唯一变化的
                                            point2 = GetXYZ(cu, i);
                                            tmp2 = point2;  //把相应的值给赋予进去
                                                            //这是最初点
                                            fn = i.GetEndPoint(1);
                                            fistn2 = fn;
                                            n2 = i.GetEndPoint(0);
                                            //获取相应的节点
                                            //这是第一步
                                            //jClassDemo = new JClassDemo(new XYZ(), new XYZ(), new XYZ(), new XYZ());
                                            //添加List
                                            //相应节点2 


                                            //当前的最后一点
                                            //x4_list.Add(fn);
                                            //x3_list.Add(point2);
                                            //jiClassDemoList.Add(jClassDemo);
                                            //jiClassDemoListTmp.Add(jClassDemo);
                                        }


                                    }

                                }


                                //this.intialPoint0 = n1;
                                //this.intialPoint1 = n2;
                                //this.fristPoint = n3;
                                //this.secondPoint = n4;

                                x1 = point1;
                                x2 = d3;
                                x3 = d4;
                                x4 = point2;
                                ////可能是输入不准确
                                //curveArray.Append(Line.CreateBound(point1, fi));
                                ////fn是最初的点
                                //curveArray.Append(Line.CreateBound(fi, fn));
                                //curveArray.Append(Line.CreateBound(fn, point2));
                                //curveArray.Append(Line.CreateBound(point2, point1));

                                //可能是输入不准确
                                curveArray.Append(Line.CreateBound(x1, x2));
                                //fn是最初的点
                                curveArray.Append(Line.CreateBound(x2, x3));
                                curveArray.Append(Line.CreateBound(x3, x4));
                                curveArray.Append(Line.CreateBound(x4, x1));

                                Floor newfloor = doc.Create.NewFloor(curveArray, floorType, level, false);
                            }
                        }


                        ////tagList.Add(cout.ToString());
                        //foreach (CurveLoop curveloop in loopList)
                        //{
                        //    var curveList = curveloop.ToArray();//曲线链分离成曲线数组
                        //                                        //最重要是获取这个CurveArray
                        //    CurveArray curveArray = new CurveArray();
                        //    //每轮转一次 还有 

                        //    int j = 0;
                        //    //阴阳轮转第一步
                        //    foreach (var i in curveList)//数组变组合
                        //    {
                        //        //curveArray.Append(i);
                        //        //cout = cout + 1;

                        //        //如果是相交的
                        //        if (GetXYZ(cu, i) != null)
                        //        {

                        //            j = j + 1;

                        //            if (j == 1)
                        //            {
                        //                //这是交点
                        //                //这是唯一变化的
                        //                point1 = GetXYZ(cu, i);
                        //                //这是最初点
                        //                tmp1 = point1; //把相应的值赋予进去
                        //                fi = i.GetEndPoint(0);
                        //                //
                        //                fistn1 = fi;
                        //                //这是末尾的最初点
                        //                n1 = i.GetEndPoint(1);
                        //                //获取相应的节点
                        //                //这是第一步
                        //                //x1_list.Add(fi);
                        //                //x2_list.Add(point1);
                        //                //添加List
                        //                //相应节点2 


                        //            }
                        //            else
                        //            {
                        //                //这是交点
                        //                //这是唯一变化的
                        //                point2 = GetXYZ(cu, i);
                        //                tmp2 = point2;  //把相应的值给赋予进去
                        //                                //这是最初点
                        //                fn = i.GetEndPoint(1);
                        //                fistn2 = fn;
                        //                n2 = i.GetEndPoint(0);
                        //                //获取相应的节点
                        //                //这是第一步
                        //                //jClassDemo = new JClassDemo(new XYZ(), new XYZ(), new XYZ(), new XYZ());
                        //                //添加List
                        //                //相应节点2 


                        //                //当前的最后一点
                        //                //x4_list.Add(fn);
                        //                //x3_list.Add(point2);
                        //                //jiClassDemoList.Add(jClassDemo);
                        //                //jiClassDemoListTmp.Add(jClassDemo);
                        //            }


                        //        }

                        //    }


                        //    //this.intialPoint0 = n1;
                        //    //this.intialPoint1 = n2;
                        //    //this.fristPoint = n3;
                        //    //this.secondPoint = n4;

                        //    x1 = point1;
                        //    x2 = d1;
                        //    x3 = d2;
                        //    x4 = point2;
                        //    ////可能是输入不准确
                        //    //curveArray.Append(Line.CreateBound(point1, fi));
                        //    ////fn是最初的点
                        //    //curveArray.Append(Line.CreateBound(fi, fn));
                        //    //curveArray.Append(Line.CreateBound(fn, point2));
                        //    //curveArray.Append(Line.CreateBound(point2, point1));

                        //    //可能是输入不准确
                        //    curveArray.Append(Line.CreateBound(x1, x2));
                        //    //fn是最初的点
                        //    curveArray.Append(Line.CreateBound(x2, x3));
                        //    curveArray.Append(Line.CreateBound(x3, x4));
                        //    curveArray.Append(Line.CreateBound(x4, x1));

                        //    Floor newfloor = doc.Create.NewFloor(curveArray, floorType, level, false);
                        //    d1 = x1;
                        //    d2 = x4;
                        //}
                    }




                }
                //最后补一刀
                cout = cout + 1;
                tagList.Add(cout.ToString());

                tran1.Commit();
            }
            TaskDialog.Show("cout", cout.ToString());

        }

        /// <summary>
        /// 这是什么鬼
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="doc"></param>
        public void CreateFloorVer1(UIDocument uidoc, Document doc)
        {
            
            //步骤 第1 步获取楼板

            //步骤 第2 步 获取对应的楼板

            Reference refer = uidoc.Selection.PickObject(ObjectType.Element, "请选择楼板");
            Floor floor = doc.GetElement(refer) as Floor;

            //根据封闭的曲线数组进行新的楼板创建
            //第一条是轴线，可能会比较难，我要一条一条的测试

            List<CurveArray> cuArray = new List<CurveArray>();
            //这是轴网链接
            GetGridSelect getGridSelect = new GetGridSelect();
            IList<Reference> getGrid = uidoc.Selection.PickObjects(ObjectType.Element, getGridSelect
                , "用于切割的轴网");



            List<Curve> gridList = new List<Curve>();
            //Curve ci = null;
            Grid gi = null;
            foreach (Reference re in getGrid)
            {
                gi = doc.GetElement(re) as Grid;
                gridList.Add(gi.Curve);
            }




            //取得最小的封闭环数列
            var faceReferences1 = HostObjectUtils.GetTopFaces(floor);
            GeometryObject topFaceGeo1 = floor.GetGeometryObjectFromReference(faceReferences1[0]);
            Face topFace1 = topFaceGeo1 as Face;
            var loopList = topFace1.GetEdgesAsCurveLoops();//最小封闭环

            Level level = doc.GetElement(floor.LevelId) as Level;
            FloorType floorType = floor.FloorType;

            

            //int cout = 0;
            //根据封闭的曲线数组进行新的楼板创建
            using (Transaction tran1 = new Transaction(doc))
            {
                tran1.Start("split");
                doc.Delete(floor.Id);//删除旧楼板

                XYZ point1 = null;
                XYZ point2 = null;
                XYZ fi = null;
                XYZ fn = null;

                //存储最终节点的n1
                XYZ n1 = null;
                //存储最终节点的n2
                XYZ n2 = null;
                //加一个连接点
                //加一个点存储最初的代码
                XYZ tmp1 = null; //存储上面的交点1
                XYZ tmp2 = null; //存储上面的交点2

                //这是最初的first0 
                XYZ fistn1 = null;
                //这是最初的first1
                XYZ fistn2 = null;

                List<string> tagList = new List<string>();

                //这是第一个节点和第二个节点的位置
                XYZ x1, x2, x3, x4 = null;

                //XYZ tmpX1,tmpX2 = null;
                XYZ d1 = null;
                XYZ d2 = null;
                XYZ d3 = null;
                XYZ d4 = null;
                //把上一条的插件点准备到最后一条去
                foreach (Curve cu in gridList)
                {

                    //cout = cout + 1;

                    //只要是两个以上的就开始报错，MMP的

                    //那就按照最原始的方式搞起

                    ////
                    
                    //
                    foreach (CurveLoop curveloop in loopList)
                    {
                        var curveList = curveloop.ToArray();//曲线链分离成曲线数组
                                                            //最重要是获取这个CurveArray
                        CurveArray curveArray = new CurveArray();
                        //每轮转一次 还有 

                        int j = 0;
                        //阴阳轮转第一步
                        foreach (var i in curveList)//数组变组合
                        {
                            //curveArray.Append(i);
                            //cout = cout + 1;

                            //如果是相交的
                            if (GetXYZ(cu, i) != null)
                            {

                                j = j + 1;

                                if (j == 1)
                                {
                                    //这是交点
                                    //这是唯一变化的
                                    point1 = GetXYZ(cu, i);
                                    //这是最初点
                                    tmp1 = point1; //把相应的值赋予进去
                                    fi = i.GetEndPoint(0);
                                    //
                                    fistn1 = fi;
                                    //这是末尾的最初点
                                    n1 = i.GetEndPoint(1);
                                    d3 = n1;
                                    //获取相应的节点
                                    //这是第一步
                                    //x1_list.Add(fi);
                                    //x2_list.Add(point1);
                                    //添加List
                                    //相应节点2 


                                }
                                else
                                {
                                    //这是交点
                                    //这是唯一变化的
                                    point2 = GetXYZ(cu, i);
                                    tmp2 = point2;  //把相应的值给赋予进去
                                                    //这是最初点
                                    fn = i.GetEndPoint(1);
                                    fistn2 = fn;
                                    n2 = i.GetEndPoint(0);
                                    d4 = n2;
                                    //获取相应的节点
                                    //这是第一步
                                    //jClassDemo = new JClassDemo(new XYZ(), new XYZ(), new XYZ(), new XYZ());
                                    //添加List
                                    //相应节点2 


                                    //当前的最后一点
                                    //x4_list.Add(fn);
                                    //x3_list.Add(point2);
                                    //jiClassDemoList.Add(jClassDemo);
                                    //jiClassDemoListTmp.Add(jClassDemo);
                                }


                            }

                        }


                        //this.intialPoint0 = n1;
                        //this.intialPoint1 = n2;
                        //this.fristPoint = n3;
                        //this.secondPoint = n4;

                        x1 = point1;
                        x2 = fi;
                        x3 = fn;
                        x4 = point2;

                        curveArray = new CurveArray();
                        ////可能是输入不准确
                        //curveArray.Append(Line.CreateBound(point1, fi));
                        ////fn是最初的点
                        //curveArray.Append(Line.CreateBound(fi, fn));
                        //curveArray.Append(Line.CreateBound(fn, point2));
                        //curveArray.Append(Line.CreateBound(point2, point1));

                        //是不是循环的模式有问题
                        curveArray.Append(Line.CreateBound(x2, x3));
                        //fn是最初的点
                        curveArray.Append(Line.CreateBound(x3, x4));
                        curveArray.Append(Line.CreateBound(x4, x1));
                        curveArray.Append(Line.CreateBound(x1, x2));

                        Floor newfloor = doc.Create.NewFloor(curveArray, floorType, level, false);

                        CurveArray
                        curveArray1 = new CurveArray();
                        //可能是输入不准确
                        x2 = n1;
                        x3 = n2;
                        curveArray1.Append(Line.CreateBound(x1, x2));
                        //fn是最初的点
                        curveArray1.Append(Line.CreateBound(x2, x3));
                        curveArray1.Append(Line.CreateBound(x3, x4));
                        curveArray1.Append(Line.CreateBound(x4, x1));

                        //原来不能用同一个curveArray
                        Floor newfloor1 = doc.Create.NewFloor(curveArray1, floorType, level, false);

                        j = 0;
                        //把第一次的结果给弄下来
                        d1 = x1;
                        d2 = x4;
                    }
                    //相当于废物点


                    //这个是结尾

                    //这就是
                    //SpitFloor(doc,floor,cu);




                }


                tran1.Commit();
            }
            

        }

        public Floor SpitFloor(Document doc,Floor floor,Curve cu) {

            Floor floorTmp=null;
            if (cu == null) return null;
            //取得最小的封闭环数列
            var faceReferences1 = HostObjectUtils.GetTopFaces(floor);
            GeometryObject topFaceGeo1 = floor.GetGeometryObjectFromReference(faceReferences1[0]);
            Face topFace1 = topFaceGeo1 as Face;
            var loopList = topFace1.GetEdgesAsCurveLoops();//最小封闭环

            Level level = doc.GetElement(floor.LevelId) as Level;
            FloorType floorType = floor.FloorType;

            XYZ point1 = null;
            XYZ point2 = null;
            XYZ fi = null;
            XYZ fn = null;

            //存储最终节点的n1
            XYZ n1 = null;
            //存储最终节点的n2
            XYZ n2 = null;
            //加一个连接点
            //加一个点存储最初的代码
            XYZ tmp1 = null; //存储上面的交点1
            XYZ tmp2 = null; //存储上面的交点2

            //这是最初的first0 
            XYZ fistn1 = null;
            //这是最初的first1
            XYZ fistn2 = null;

            List<string> tagList = new List<string>();

            //这是第一个节点和第二个节点的位置
            XYZ x1, x2, x3, x4 = null;

            //XYZ tmpX1,tmpX2 = null;
            XYZ d1 = null;
            XYZ d2 = null;
            XYZ d3 = null;
            XYZ d4 = null;
            foreach (CurveLoop curveloop in loopList)
            {
                var curveList = curveloop.ToArray();//曲线链分离成曲线数组
                                                    //最重要是获取这个CurveArray
                CurveArray curveArray = new CurveArray();
                //每轮转一次 还有 

                int j = 0;
                //阴阳轮转第一步
                foreach (var i in curveList)//数组变组合
                {
                    //curveArray.Append(i);
                    //cout = cout + 1;

                    //如果是相交的
                    if (GetXYZ(cu, i) != null)
                    {

                        j = j + 1;

                        if (j == 1)
                        {
                            //这是交点
                            //这是唯一变化的
                            point1 = GetXYZ(cu, i);
                            //这是最初点
                            tmp1 = point1; //把相应的值赋予进去
                            fi = i.GetEndPoint(0);
                            //
                            fistn1 = fi;
                            //这是末尾的最初点
                            n1 = i.GetEndPoint(1);
                            d3 = n1;
                            //获取相应的节点
                            //这是第一步
                            //x1_list.Add(fi);
                            //x2_list.Add(point1);
                            //添加List
                            //相应节点2 


                        }
                        else
                        {
                            //这是交点
                            //这是唯一变化的
                            point2 = GetXYZ(cu, i);
                            tmp2 = point2;  //把相应的值给赋予进去
                                            //这是最初点
                            fn = i.GetEndPoint(1);
                            fistn2 = fn;
                            n2 = i.GetEndPoint(0);
                            d4 = n2;
                            //获取相应的节点
                            //这是第一步
                            //jClassDemo = new JClassDemo(new XYZ(), new XYZ(), new XYZ(), new XYZ());
                            //添加List
                            //相应节点2 


                            //当前的最后一点
                            //x4_list.Add(fn);
                            //x3_list.Add(point2);
                            //jiClassDemoList.Add(jClassDemo);
                            //jiClassDemoListTmp.Add(jClassDemo);
                        }


                    }

                }


                //this.intialPoint0 = n1;
                //this.intialPoint1 = n2;
                //this.fristPoint = n3;
                //this.secondPoint = n4;

                x1 = point1;
                x2 = fi;
                x3 = fn;
                x4 = point2;

                curveArray = new CurveArray();
                ////可能是输入不准确
                curveArray.Append(Line.CreateBound(point1, fi));
                //fn是最初的点
                curveArray.Append(Line.CreateBound(fi, fn));
                curveArray.Append(Line.CreateBound(fn, point2));
                curveArray.Append(Line.CreateBound(point2, point1));

                Floor newfloor = doc.Create.NewFloor(curveArray, floorType, level, false);

                CurveArray
                curveArray1 = new CurveArray();
                //可能是输入不准确
                x2 = n1;
                x3 = n2;
                curveArray1.Append(Line.CreateBound(x1, x2));
                //fn是最初的点
                curveArray1.Append(Line.CreateBound(x2, x3));
                curveArray1.Append(Line.CreateBound(x3, x4));
                curveArray1.Append(Line.CreateBound(x4, x1));

                //原来不能用同一个curveArray
                Floor newfloor1 = doc.Create.NewFloor(curveArray1, floorType, level, false);

                j = 0;
                //把第一次的结果给弄下来
                d1 = x1;
                d2 = x4;
                //把对应的floor给赋予起来

                //floorTmp = newfloor1;

                floorTmp = SpitFloor(doc, newfloor1, cu);
            }
            //相当于废物点
            return floorTmp;
        }

        public void CreateWall(UIDocument uidoc,Document doc) {
            //对构件进行切分分割
            GetWallSelect getWallSelect = new GetWallSelect();
            Reference refer = uidoc.Selection.PickObject(ObjectType.Element,
                getWallSelect, "请选择墙体");
            //获取当前的墙体
            Wall wall = doc.GetElement(refer) as Wall;
            LocationCurve lc = wall.Location as LocationCurve;

            Curve c1 = lc.Curve;
            //开始点
            XYZ first =
            c1.GetEndPoint(0);
            //最终点
            XYZ final =
            c1.GetEndPoint(1);
            GetGridSelect getGridSelect = new GetGridSelect();
            IList<Reference> getGrid = uidoc.Selection.PickObjects(ObjectType.Element, getGridSelect
                , "用于切割的轴网");
            //List<Curve> lineList = new List<Curve>();
            Grid grid = null;
            //Line lili = null;
            Curve cici = null;
            List<XYZ> pointList = new List<XYZ>();
            pointList.Add(first);

            foreach (Reference referTmp in getGrid)
            {
                grid = doc.GetElement(referTmp) as Grid;
                cici = grid.Curve;
                //lineList.Add(cici);
                pointList.Add(GetXYZ(c1, cici));
            }
            pointList.Add(final);

            Level level = doc.GetElement(wall.LevelId) as Level;

            Transaction ts = new Transaction(doc, "生成墙体");
            ts.Start();

            int time = 0;
            int cout = pointList.Count;
            doc.Delete(wall.Id);
            foreach (XYZ point in pointList)
            {
                time = time + 1;
                if (time == cout)
                    break;
                //Wall.Create(doc,line ,false);
                Wall.Create(doc, Line.CreateBound(point, pointList[time]), level.Id, false);

            }

            ts.Commit();
        }


        public void CreateBeam(UIDocument uidoc, Document doc)
        {
            //对构件进行切分分割
            //GetWallSelect getWallSelect = new GetWallSelect();
            Reference refer = uidoc.Selection.PickObject(ObjectType.Element,
                 "请选择结构梁");
            //获取当前的墙体
            FamilyInstance wall = doc.GetElement(refer) as FamilyInstance;
            LocationCurve lc = wall.Location as LocationCurve;

            Curve c1 = lc.Curve;
            //开始点
            XYZ first =
            c1.GetEndPoint(0);
            //最终点
            XYZ final =
            c1.GetEndPoint(1);
            GetGridSelect getGridSelect = new GetGridSelect();
            IList<Reference> getGrid = uidoc.Selection.PickObjects(ObjectType.Element, getGridSelect
                , "用于切割的轴网");
            //List<Curve> lineList = new List<Curve>();
            Grid grid = null;
            //Line lili = null;
            Curve cici = null;
            List<XYZ> pointList = new List<XYZ>();
            pointList.Add(first);

            //string s = null;
            //s = s +"X"+ first.X.ToString()+"Y"+ first.Y.ToString()+"Z" +first.Z.ToString()+"\\";
            //s = s + "X" + final.X.ToString() + "Y" + final.Y.ToString() + "Z" + final.Z.ToString();
            //TaskDialog.Show("s",s);

            Line li = null;
                
            foreach (Reference referTmp in getGrid)
            {
                grid = doc.GetElement(referTmp) as Grid;
                cici = grid.Curve;
                //139有问题嘛
                li = Line.CreateBound(new XYZ(cici.GetEndPoint(0).X,
                    cici.GetEndPoint(0).Y,first.Z
                    ), new XYZ(
                        cici.GetEndPoint(1).X,
                    cici.GetEndPoint(1).Y, first.Z
                        ));
                //TaskDialog.Show("F 1",(cici.GetEndPoint(0).X+
                //    cici.GetEndPoint(0).Y+ li.GetEndPoint(0).Z).ToString());
                //TaskDialog.Show("FN 1", (cici.GetEndPoint(1).X +
                //    cici.GetEndPoint(1).Y + li.GetEndPoint(1).Z).ToString());
                //lineList.Add(cici);
                pointList.Add(GetXYZ(c1, li));
            }
            pointList.Add(final);

            //Level level = doc.GetElement(wall.LevelId) as Level;
            FamilySymbol fsi = wall.Symbol;
            Level lev = doc.GetElement(wall.LevelId) as Level;
            Transaction ts = new Transaction(doc, "生成梁");
            ts.Start();

            int time = 0;
            int cout = pointList.Count;
            doc.Delete(wall.Id);
            

            //可能是没有点
            foreach (XYZ point in pointList)
            {
                time = time + 1;
                if (time == cout)
                    break;
                //Wall.Create(doc,line ,false);
                //Wall.Create(doc, Line.CreateBound(point, pointList[time]), level.Id, false);
                //这是啥子
                TaskDialog.Show("TIME",time.ToString());
                doc.Create.NewFamilyInstance(Line.CreateBound(point, pointList[time])
                    ,fsi,lev, StructuralType.Beam);
            }

            ts.Commit();
        }


        public void CreateFloorFinal(UIDocument uidoc, Document doc)
        {
            FloorSlectionFileter floorSlectionFileter = new FloorSlectionFileter();
            var references = uidoc.Selection.PickObjects(ObjectType.Element, floorSlectionFileter);
            List<Element> beamList = new List<Element>();
            List<Element> gridDemoList = new List<Element>();
            List<Element> floorList = new List<Element>();
            foreach (var refer in references)
            {
                Element elem = doc.GetElement(refer) as Element;
                if (elem.Category.Name == "轴网")
                {
                    gridDemoList.Add(elem);
                }
                else if (elem.Category.Name == "楼板")
                {
                    floorList.Add(elem);
                }
            }

            List<Curve> gridList = new List<Curve>();
            //Curve ci = null;
            Grid gi = null;
            foreach (Element ele1 in gridDemoList)
            {
                gi = ele1 as Grid;
                gridList.Add(gi.Curve);
            }

            //同时获取对应的beamList;
            //将梁、板分离
            var floor = floorList[0] as Floor;

            //265299

            //到时候在这里还需要导入一波那个FamiySymbol
            FamilySymbol fsi = doc.GetElement(new ElementId(337159)) as FamilySymbol;
            //FamilySymbol fsi = doc.GetElement(new ElementId(265299)) as FamilySymbol;

            Level lev = doc.GetElement(floor.LevelId) as Level;
            //取得最小的封闭环数列

            var faceReferences = HostObjectUtils.GetTopFaces(floor);
            GeometryObject topFaceGeo0 = floor.GetGeometryObjectFromReference(faceReferences[0]);
            Face topFace = topFaceGeo0 as Face;
            var loopList1 = topFace.GetEdgesAsCurveLoops();//最小封闭环

            List<XYZ> pointList = new List<XYZ>();

            List<ElementId> eleList = new List<ElementId>();
            Element ele = null;
            foreach (Curve cu in gridList)
            {

                //cout = cout + 1;

                //只要是两个以上的就开始报错，MMP的

                //那就按照最原始的方式搞起

                ////

                //
                pointList = new List<XYZ>();
                foreach (CurveLoop curveloop in loopList1)
                {
                    XYZ point1, point2 = null;
                    var curveList = curveloop.ToArray();//曲线链分离成曲线数组
                                                        //最重要是获取这个CurveArray
                    CurveArray curveArray = new CurveArray();
                    //每轮转一次 还有 

                    int j = 0;
                    //阴阳轮转第一步
                    foreach (var i in curveList)//数组变组合
                    {
                        //curveArray.Append(i);
                        //cout = cout + 1;

                        //如果是相交的
                        if (GetXYZ(cu, i) != null)
                        {
                            //XYZ tmp = null;
                            j = j + 1;
                            if (j == 1)
                            {
                                point1 = GetXYZ(cu, i);
                                //tmp = point1;
                                pointList.Add(point1);
                            }
                            else
                            {
                                point2 = GetXYZ(cu, i);
                                pointList.Add(point2);
                            }


                        }

                    }



                }
                Transaction ts = new Transaction(doc, "1");
                ts.Start();
                ele =
                doc.Create.NewFamilyInstance(Line.CreateBound(pointList[0], pointList[1])
                   , fsi, lev, StructuralType.Beam);
                Parameter para = ele.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION);
                para.Set(1 / 304.8);
                //beamList.Add(ele);
                Parameter para1 = ele.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION);
                para1.Set(1 / 304.8);
                beamList.Add(ele);
                eleList.Add(ele.Id);
                ts.Commit();
                pointList = new List<XYZ>();
            }

            //将梁与楼板进行连接，目的为了取得最小的封闭环。
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("Join");
                foreach (var beam in beamList)
                {
                    if (!JoinGeometryUtils.AreElementsJoined(doc, floor, beam))//判断是否连接
                    {
                        JoinGeometryUtils.JoinGeometry(doc, floor, beam);//连接操作
                        JoinGeometryUtils.SwitchJoinOrder(doc, floor, beam);//取反操作
                    }
                }
                tran.Commit();
            }

            //取得最小的封闭环数列
            var faceReferences1 = HostObjectUtils.GetTopFaces(floor);
            GeometryObject topFaceGeo1 = floor.GetGeometryObjectFromReference(faceReferences1[0]);
            Face topFace1 = topFaceGeo1 as Face;
            var loopList = topFace1.GetEdgesAsCurveLoops();//最小封闭环



            Level level = doc.GetElement(floor.LevelId) as Level;
            FloorType floorType = floor.FloorType;

            //根据封闭的曲线数组进行新的楼板创建
            using (Transaction tran1 = new Transaction(doc))
            {
                tran1.Start("split");
                doc.Delete(floor.Id);//删除旧楼板
                foreach (CurveLoop curveloop in loopList)
                {
                    var curveList = curveloop.ToArray();//曲线链分离成曲线数组
                    CurveArray curveArray = new CurveArray();
                    foreach (var i in curveList)//数组变组合
                    {
                        curveArray.Append(i);
                    }
                    Floor newfloor = doc.Create.NewFloor(curveArray, floorType, level, false);
                }
                //把相应的elId给删除干净
                //但是有很大一截空白啊
                doc.Delete(eleList);
                tran1.Commit();
            }

        }


        public void CreateFloorFinal2(UIDocument uidoc, Document doc)
        {
            FloorSlectionFileter floorSlectionFileter = new FloorSlectionFileter();
            var references = uidoc.Selection.PickObjects(ObjectType.Element, floorSlectionFileter);
            List<Element> beamList = new List<Element>();
            List<Element> gridDemoList = new List<Element>();
            List<Element> floorList = new List<Element>();
            foreach (var refer in references)
            {
                Element elem = doc.GetElement(refer) as Element;
                if (elem.Category.Name == "轴网")
                {
                    gridDemoList.Add(elem);
                }
                else if (elem.Category.Name == "楼板")
                {
                    floorList.Add(elem);
                }
            }

            List<Curve> gridList = new List<Curve>();
            //Curve ci = null;
            Grid gi = null;
            foreach (Element ele1 in gridDemoList)
            {
                gi = ele1 as Grid;
                gridList.Add(gi.Curve);
            }

            //同时获取对应的beamList;
            //将梁、板分离
            var floor = floorList[0] as Floor;

            //265299

            //到时候在这里还需要导入一波那个FamiySymbol
            FamilySymbol fsi = doc.GetElement(new ElementId(337159)) as FamilySymbol;
            //FamilySymbol fsi = doc.GetElement(new ElementId(265299)) as FamilySymbol;

            Level lev = doc.GetElement(floor.LevelId) as Level;
            //取得最小的封闭环数列

            var faceReferences = HostObjectUtils.GetTopFaces(floor);
            GeometryObject topFaceGeo0 = floor.GetGeometryObjectFromReference(faceReferences[0]);
            Face topFace = topFaceGeo0 as Face;
            var loopList1 = topFace.GetEdgesAsCurveLoops();//最小封闭环

            List<XYZ> pointList = new List<XYZ>();

            List<ElementId> eleList = new List<ElementId>();
            Element ele = null;
            foreach (Curve cu in gridList)
            {

                //cout = cout + 1;

                //只要是两个以上的就开始报错，MMP的

                //那就按照最原始的方式搞起

                ////

                //
                pointList = new List<XYZ>();
                foreach (CurveLoop curveloop in loopList1)
                {
                    XYZ point1, point2 = null;
                    var curveList = curveloop.ToArray();//曲线链分离成曲线数组
                                                        //最重要是获取这个CurveArray
                    CurveArray curveArray = new CurveArray();
                    //每轮转一次 还有 

                    int j = 0;
                    //阴阳轮转第一步
                    foreach (var i in curveList)//数组变组合
                    {
                        //curveArray.Append(i);
                        //cout = cout + 1;

                        //如果是相交的
                        if (GetXYZ(cu, i) != null)
                        {
                            //XYZ tmp = null;
                            j = j + 1;
                            if (j == 1)
                            {
                                point1 = GetXYZ(cu, i);
                                //tmp = point1;
                                pointList.Add(point1);
                            }
                            else
                            {
                                point2 = GetXYZ(cu, i);
                                pointList.Add(point2);
                            }


                        }

                    }



                }
                Transaction ts = new Transaction(doc, "1");
                ts.Start();
                ele =
                doc.Create.NewFamilyInstance(Line.CreateBound(pointList[0], pointList[1])
                   , fsi, lev, StructuralType.Beam);
                Parameter para = ele.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION);
                para.Set(1 / 304.8);
                //beamList.Add(ele);
                Parameter para1 = ele.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION);
                para1.Set(1 / 304.8);
                beamList.Add(ele);
                eleList.Add(ele.Id);
                ts.Commit();
                pointList = new List<XYZ>();
            }

            //将梁与楼板进行连接，目的为了取得最小的封闭环。
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("Join");
                foreach (var beam in beamList)
                {
                    if (!JoinGeometryUtils.AreElementsJoined(doc, floor, beam))//判断是否连接
                    {
                        JoinGeometryUtils.JoinGeometry(doc, floor, beam);//连接操作
                        JoinGeometryUtils.SwitchJoinOrder(doc, floor, beam);//取反操作
                    }
                }
                tran.Commit();
            }

            //取得最小的封闭环数列
            var faceReferences1 = HostObjectUtils.GetTopFaces(floor);
            GeometryObject topFaceGeo1 = floor.GetGeometryObjectFromReference(faceReferences1[0]);
            Face topFace1 = topFaceGeo1 as Face;
            var loopList = topFace1.GetEdgesAsCurveLoops();//最小封闭环



            Level level = doc.GetElement(floor.LevelId) as Level;
            FloorType floorType = floor.FloorType;

            //根据封闭的曲线数组进行新的楼板创建
            using (Transaction tran1 = new Transaction(doc))
            {
                tran1.Start("split");
                doc.Delete(floor.Id);//删除旧楼板
                foreach (CurveLoop curveloop in loopList)
                {
                    var curveList = curveloop.ToArray();//曲线链分离成曲线数组
                    CurveArray curveArray = new CurveArray();
                    foreach (var i in curveList)//数组变组合
                    {
                        curveArray.Append(i);
                    }
                    Floor newfloor = doc.Create.NewFloor(curveArray, floorType, level, false);
                }
                //把相应的elId给删除干净
                //但是有很大一截空白啊
                doc.Delete(eleList);
                tran1.Commit();
            }

        }
        /// <summary>
        /// 仅供选择要分段的梁和与之相连接的柱和需要连接的柱子
        /// </summary>
        public class FloorSlectionFileter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                if (elem.Category.Name == "楼板" || elem.Category.Name == "轴网")
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

        public void CreateFloor1(UIDocument uidoc, Document doc)
        {
            //对构件进行切分分割
            //GetWallSelect getWallSelect = new GetWallSelect();
            Reference refer = uidoc.Selection.PickObject(ObjectType.Element,
                 "请选择楼板");
            //获取当前的墙体
            FamilyInstance wall = doc.GetElement(refer) as FamilyInstance;
            LocationCurve lc = wall.Location as LocationCurve;

            Curve c1 = lc.Curve;
            //开始点
            XYZ first =
            c1.GetEndPoint(0);
            //最终点
            XYZ final =
            c1.GetEndPoint(1);
            GetGridSelect getGridSelect = new GetGridSelect();
            IList<Reference> getGrid = uidoc.Selection.PickObjects(ObjectType.Element, getGridSelect
                , "用于切割的轴网");
            //List<Curve> lineList = new List<Curve>();
            Grid grid = null;
            //Line lili = null;
            Curve cici = null;
            List<XYZ> pointList = new List<XYZ>();
            pointList.Add(first);

            //string s = null;
            //s = s +"X"+ first.X.ToString()+"Y"+ first.Y.ToString()+"Z" +first.Z.ToString()+"\\";
            //s = s + "X" + final.X.ToString() + "Y" + final.Y.ToString() + "Z" + final.Z.ToString();
            //TaskDialog.Show("s",s);

            Line li = null;

            foreach (Reference referTmp in getGrid)
            {
                grid = doc.GetElement(referTmp) as Grid;
                cici = grid.Curve;
                //139有问题嘛
                li = Line.CreateBound(new XYZ(cici.GetEndPoint(0).X,
                    cici.GetEndPoint(0).Y, first.Z
                    ), new XYZ(
                        cici.GetEndPoint(1).X,
                    cici.GetEndPoint(1).Y, first.Z
                        ));
                //TaskDialog.Show("F 1",(cici.GetEndPoint(0).X+
                //    cici.GetEndPoint(0).Y+ li.GetEndPoint(0).Z).ToString());
                //TaskDialog.Show("FN 1", (cici.GetEndPoint(1).X +
                //    cici.GetEndPoint(1).Y + li.GetEndPoint(1).Z).ToString());
                //lineList.Add(cici);
                pointList.Add(GetXYZ(c1, li));
            }
            pointList.Add(final);

            //Level level = doc.GetElement(wall.LevelId) as Level;
            FamilySymbol fsi = wall.Symbol;
            Level lev = doc.GetElement(wall.LevelId) as Level;
            Transaction ts = new Transaction(doc, "生成梁");
            ts.Start();

            int time = 0;
            int cout = pointList.Count;
            doc.Delete(wall.Id);


            //可能是没有点
            foreach (XYZ point in pointList)
            {
                time = time + 1;
                if (time == cout)
                    break;
                //Wall.Create(doc,line ,false);
                //Wall.Create(doc, Line.CreateBound(point, pointList[time]), level.Id, false);
                //这是啥子
                TaskDialog.Show("TIME", time.ToString());
                doc.Create.NewFamilyInstance(Line.CreateBound(point, pointList[time])
                    , fsi, lev, StructuralType.Beam);
            }

            ts.Commit();
        }

        public XYZ GetXYZ(Curve c1,Curve c2) {

            XYZ point = null;
            IntersectionResultArray resultArray = null;
            SetComparisonResult result = c1.Intersect(c2, out resultArray);

            //获取相交点
            if (result.Equals(SetComparisonResult.Overlap))
            {
                foreach (IntersectionResult item in resultArray)
                {
                    point = item.XYZPoint;
                }
            }
            return point;
        }

        public class GetWallSelect : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                //throw new NotImplementedException();
                if (elem.Category.Name == "墙")
                {

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

        public class GetGridSelect : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                //throw new NotImplementedException();
                if (elem.Category.Name == "轴网")
                {

                    return true;

                }
                return false;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                //throw new NotImplementedException();
               return  false;
            }
        }
    }
}
