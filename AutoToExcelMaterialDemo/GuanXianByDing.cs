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
    //剖面管线定位标注  387  完成
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class GuanXianByDing : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //生成标注
            //TagSelectFilter tagSelectFilter = new TagSelectFilter();
            //CollectorSelection collectorSelection = new CollectorSelection();
            //IList<Reference> reDisList = uidoc.Selection.PickObjects(ObjectType.Element
            //    , collectorSelection
            //    , "请选择需要进行自动标注的构件");

            string s = null;
            FilteredElementCollector dimenCollector = new FilteredElementCollector(doc);
            //dimenCollector.OfCategory(BuiltInCategory.OST_Dimensions)
            //    .WhereElementIsElementType();
            //dimenCollector.OfCategory(BuiltInCategory.OST_Walls)
            //    .WhereElementIsElementType();
            dimenCollector
                .OfClass(typeof(DimensionType)).WhereElementIsElementType();

            //????难道就获取一个实体嘛
            //dimenCollector.OfCategory(BuiltInCategory.OST_Walls)
            //    .WhereElementIsElementType();
            //.OfClass(typeof(FamilySymbol));
            //.WhereElementIsElementType();

            //这个是用于显示DimensionType的
            DimensionType dtype = null;
            List<DimensionType> dimenTypeList = new List<DimensionType>();
            foreach (Element ele in dimenCollector) {
                //TaskDialog.Show("1",ele.Name);
                dtype = ele as DimensionType;
                if (dtype.FamilyName== "线性尺寸标注样式"&&dtype.Name!= "线性尺寸标注样式") {
                    s = s + dtype.Name + "\n";
                    dimenTypeList.Add(dtype);
                }    
            }
            TaskDialog.Show("cout",dimenTypeList.Count.ToString());
            TaskDialog.Show("类型",s);

            ////管道 风管 桥架
            //定位设置 
            //垂直+水平 
            //垂直 标高 楼板 梁 天花板
            //水平 轴线 墙体 柱

            //进行自动标注，功能还行
            Selection selection = uidoc.Selection;

            Transaction ts = new Transaction(doc, "创建标注");
            ts.Start();

            AutoCreatDimension(doc, selection);

            ts.Commit();

            ////管道 
            ////风管
            ////桥架
            ////水平+垂直
            ////保温层
            //List<XYZ> xyzList = new List<XYZ>();
            //List<XYZ> newXYZList = new List<XYZ>();
            //newXYZList.Add(new XYZ());
            //Wall wall = null;
            //LocationCurve lc = null;
            //XYZ mid = null;
            //foreach (Reference refer in reDisList ) {
            //    wall = doc.GetElement(refer) as Wall;
            //    lc = wall.Location as LocationCurve;
            //    mid = lc.Curve.Evaluate(0.5, true);
            //}

            ////生成管道
            //LocationCurve location = pipe.Location as LocationCurve;
            //XYZ mid = location.Curve.Evaluate(0.5, true);//取得管道的中心点
            

           

            return Result.Succeeded;
        }

        /// <summary>
        /// 自动创建所选中的一组图元的线性距离尺寸
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="selection"></param>
        public void AutoCreatDimension(Document doc, Selection selection)
        {

            //选择需要标注尺寸的图元
            //IList<Reference> referenceList = selection.PickObjects(ObjectType.Element
            //    , new ElementSelectionFilter(doc), "请选择一组图元");
            CollectorSelection collectorSelection = new CollectorSelection();
            IList<Reference> referenceList = selection.PickObjects(ObjectType.Element
                , collectorSelection
                , "请选择一组图元");

            //水平也可以，垂直也可以
            if (referenceList.Count == 0)
            {
                TaskDialog.Show("警告", "您没有选择任何元素，请重新选择");
                return;
            }
            //取得其中一个图元 获取其位置
            // Pipe pipe = doc.GetElement(referenceList.ElementAt(0)) as Pipe;
            Element element = doc.GetElement(referenceList.ElementAt(0));
            Line line = (element.Location as LocationCurve).Curve as Line;

            View view = doc.ActiveView;

            XYZ selectionPoint = selection.PickPoint();

            XYZ projectPoint = line.Project(selectionPoint).XYZPoint;

            Line newLine = Line.CreateBound(selectionPoint, projectPoint);


            ReferenceArray references = new ReferenceArray();

            foreach (Reference reference in referenceList)
            {
                references.Append(reference);
            }
            //调用创建尺寸的方法创建
            Dimension autoDimension = doc.Create.NewDimension(view, newLine, references);
        }
    }


   

    public class CollectorSelection : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            //throw new NotImplementedException();
            if (elem.Category.Name == "墙" || elem.Category.Name == "风管" || elem.Category.Name == "管道" || elem.Category.Name== "电缆桥架") {
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
