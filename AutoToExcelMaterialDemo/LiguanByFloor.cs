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
    //楼层打断立管  388 完成
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class LiguanByFloor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            LiGuanSpiltCreatePipe(uidoc, doc);

            return Result.Succeeded;
        }

        public void LiGuanSpiltCreatePipe(UIDocument uidoc, Document doc) {

            Reference refer = uidoc.Selection.PickObject(ObjectType.Element, new PipeSelection(), "请选择管道");
            Element ele = doc.GetElement(refer) as Element;
            Pipe pipe = ele as Pipe;
            LocationCurve lc = pipe.Location as LocationCurve;
            //xyz 
            XYZ startPoint = lc.Curve.GetEndPoint(0);
            XYZ endPoint = lc.Curve.GetEndPoint(1);
            XYZ tmp = null;
            if (startPoint.Z > endPoint.Z)
            {
                tmp = startPoint;
                startPoint = endPoint;
                endPoint = tmp;
            }

            double x = startPoint.X;
            double y = startPoint.Y;


            List<double> newXYZList = new List<double>();
            List<double> newFinalXYZList = new List<double>();

            List<string> sList = new List<string>();

            FilteredElementCollector levelCollector = new FilteredElementCollector(doc);
            levelCollector.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType();
            Dictionary<double, ElementId> levelElevaltionDict = new Dictionary<double, ElementId>();
            Level level = null;

            //这个是真的几把适合等长等宽的啊


            List<ElementId> levelIdList = new List<ElementId>();
            string s1 = null;
            s1 = "初高度" + startPoint.Z.ToString() + "\n";
            int tag = 0; //0个点
            foreach (Element eleLev in levelCollector)
            {
                level = eleLev as Level;
                tag = tag + 1;
                s1 = s1 + tag + level.Elevation.ToString() + "\n";
                //当在这个范围的时候
                //原来是这里的问题，MMP的
                if (level.Elevation <= endPoint.Z && level.Elevation >= startPoint.Z)
                {
                    newXYZList.Add(level.Elevation);
                    newFinalXYZList.Add(level.Elevation);
                    //TaskDialog.Show("tmp", "tmp");
                    //将标高标记起来。
                    levelIdList.Add(level.Id);
                    sList.Add(tag.ToString());
                    tag = tag + 1;
                }
                levelElevaltionDict.Add(level.Elevation, level.Id);
            }
            s1 = s1 + "末尾高度" + endPoint.Z.ToString();

            //升序
            newXYZList.Sort();
            newFinalXYZList.Sort();


            //获取系统类型
            ElementId pipeSystemId = pipe.get_Parameter(BuiltInParameter
                .RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();
            //pipe.MEPSystem.Id;
            //获取类型ID 

            //获取类型ID
            ElementId pipeTypeId = pipe.PipeType.Id;

            //这是对应的啥子
            ElementId levelId = null;


            tag = 0;

            Transaction ts = new Transaction(doc, "生成立管");
            ts.Start();
            foreach (string s in sList)
            {
                levelId = levelElevaltionDict[newXYZList[tag]];
                Pipe pipedemo = Pipe.Create(doc, pipeSystemId, pipeTypeId, levelId,
                                new XYZ(x, y, newXYZList[tag]),
                                new XYZ(x, y, newFinalXYZList[tag + 1])
                                );
                levelId = null;
                tag = tag + 1;
            }
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
    }
}
