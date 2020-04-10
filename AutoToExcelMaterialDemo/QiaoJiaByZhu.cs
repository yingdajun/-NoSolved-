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
using Autodesk.Revit.DB.Electrical;
namespace AutoToExcelMaterialDemo
{
    //桥架标注  395 功能完成
    [Transaction(TransactionMode.Manual)]
    class QiaoJiaByZhu : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            //UIApplication uiapp = commandData.Application;
            //Application app = uiapp.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            TextNote note = null;
            //手动标注+自动标注判断与桥架相交的桥架
            //系统类型+尺寸+标高（顶部/底部/中心高度）
            //是否添加引线+擅长之前的标注
            //标注到管线的距离
            //标记长度大于管线
            //标记管道半径（宽度)大于管线
            Reference refer = uidoc.Selection.PickObject(ObjectType.Element, "请选择需要标记的桥架");
            CableTray cableTray = doc.GetElement(refer) as CableTray;
            string name = cableTray.Name;
            Parameter paraWidth = cableTray.get_Parameter(BuiltInParameter.
                RBS_CABLETRAY_WIDTH_PARAM);
            string width = paraWidth.AsValueString();
            Parameter paraHeight = cableTray.get_Parameter(BuiltInParameter
                .RBS_CABLETRAY_HEIGHT_PARAM);
            string height = paraHeight.AsValueString();

            //顶部标高
            Parameter paraTopHeight = cableTray.get_Parameter(BuiltInParameter
                .RBS_CTC_TOP_ELEVATION);
            string topHeight = paraTopHeight.AsValueString();
            
           
            //底部高度
            Parameter paraBottomHeight = cableTray.get_Parameter(BuiltInParameter
                .RBS_CTC_BOTTOM_ELEVATION);
            string bottomHeight = paraBottomHeight.AsValueString();
            //底部标高
            //中心高度 我不知道
            double mid = (Convert.ToDouble(topHeight)
                + Convert.ToDouble(bottomHeight)) / 2;
            string midHeight = mid.ToString();

            LocationCurve lc = cableTray.Location as LocationCurve;
            XYZ origin = lc.Curve.Evaluate(0.5,true);

            //double length = lc.Curve.GetEndPoint(0).DistanceTo(lc.Curve.GetEndPoint(1));
            ////TaskDialog.Show("LENGTH",length.ToString());
            //double limitLen = 3000/304.8;

            //指定一个数值，大于该长度才进行标注
            //if (length>limitLen) {
                //using (Transaction tran = new Transaction(doc, "Creating a Text note"))
                //{

                //    //XYZ origin = new XYZ(10, 10, 0);
                //    ElementId defaultTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                //    tran.Start();
                //    note = TextNote.Create(doc, doc.ActiveView.Id, origin,
                //        name + " " + width + "x" + height + " " + bottomHeight, defaultTypeId);
                //    note.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L);
                //    //FormatText(note);
                //    tran.Commit();
                //}
            //}

            double limitWidth = 600 / 304.8;
            //格式不对


            //double widthD = double.Parse(width);
            
            //paraWidth.Double
            if (paraWidth.AsDouble() > limitWidth) {
                using (Transaction tran = new Transaction(doc, "Creating a Text note"))
                {

                    //XYZ origin = new XYZ(10, 10, 0);
                    ElementId defaultTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                    tran.Start();
                    note = TextNote.Create(doc, doc.ActiveView.Id, origin,
                        name + " " + width + "x" + height + " " + bottomHeight, defaultTypeId);
                    note.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L);
                    //FormatText(note);
                    tran.Commit();
                }
            }
            

            return Result.Succeeded;
        }
    }
}
