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
namespace AutoToExcelMaterialDemo
{
    //管线定位标注 387 到时候改一下即可 换一个其他的
    [Transaction(TransactionMode.Manual)]
    class GuanXianDingWeiBiaoZhu : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            

            Transaction ts = new Transaction(doc, "1");
            ts.Start();

            ts.Commit();
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
}
