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
using System.Drawing.Text;
using System.Drawing;
using System.Collections;

namespace AutoToExcelMaterialDemo
{
    //修改字体 399 功能完成
    [Transaction(TransactionMode.Manual)]
    class AdjustZiTi : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //获取所有的文字样式 有
            //字体   有
            //宽度系数 有
            //文字高度 应该是大小吧。。。
            //标签尺寸 上下斜粗  功能完成
            TextNote note = null;

            TextFontClass textFontClass = new TextFontClass();
            Reference refer = uidoc.Selection.PickObject(ObjectType.Element,textFontClass
                , "请选择需要替换格式的文字");
            Element ele = doc.GetElement(refer);
            //TextNode textNode = ele as TextNode;
            //TextNote textNote = ele as TextNote;
            note = ele as TextNote;

            //获取所有字体类型
            FilteredElementCollector textNodeStyle = new FilteredElementCollector(doc);
            textNodeStyle.OfClass(typeof(TextNoteType));
            ////文本样式的功能
            ////文本样式
            List<string> textStyleList = new List<string>();
            Dictionary<string, ElementId> textDict = new Dictionary<string, ElementId>();
            //List<ElementId> textElementIdList = new List<ElementId>();
            string s = null;
            int num = 0;
            foreach (Element ele1 in textNodeStyle)
            {
                
                s = s + ele1.Name.ToString() + "\n";
                textStyleList.Add(ele1.Name);
                //名称相同是什么
                textDict.Add(ele1.Name, ele1.Id);

                //TaskDialog.Show("title"+num,ele.Name);
                num = num + 1;
            }
            TaskDialog.Show("S", s);

            //获取所有字体
            s = null;
            InstalledFontCollection MyFont = new InstalledFontCollection();
            FontFamily[] MyFontFamilies = MyFont.Families;
            ArrayList list = new ArrayList();
            int Count = MyFontFamilies.Length;
            for (int i = 0; i < Count; i++)
            {
                string FontName = MyFontFamilies[i].Name;
                s = s + FontName+"\n";
                list.Add(FontName);

            }
            TaskDialog.Show("S1", s);

           
            ////设置样式、大小、宽度系数
            Transaction tr = new Transaction(doc, "text");
            tr.Start();
            FormatText(note);
            IList<Element> listNote = new FilteredElementCollector(doc)
                .OfClass(typeof(TextNoteType)).ToElements();
            //控制字体类型
            //这难道是一改全改嘛
            TextNoteType tny = listNote[2] as TextNoteType;

            tny.get_Parameter(BuiltInParameter.TEXT_FONT).Set("黑体");
            //控制字体大小
            //tny.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(5 / 304.8);//注意要转换为英尺
            IList<Parameter> pList = tny.GetParameters("宽度系数");
            //0.1-10.0的距离，阿西吧
            foreach (Parameter pl in pList)
            {
                pl.Set(1.0);
            }

            //XYZ xyz = uidoc.Selection.PickPoint();
            //TextNote.Create(doc, doc.ActiveView.Id, xyz, "test", tny.Id);

            tr.Commit();

            //LocationPoint locationPoint = ele.Location as LocationPoint;
            //TaskDialog.Show("xyz",locationPoint.Point.X.ToString()+"\n"
            //    +locationPoint.Point.Y.ToString()+"\n"+
            //    locationPoint.Point.Z.ToString());

            //note = doc.GetElement(refer) as TextNode;
            //note.Coord;
            //using (Transaction tran = new Transaction(doc, "Creating a Text note"))
            //{

            //    XYZ origin = new XYZ(10, 10, 0);
            //    ElementId defaultTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

            //    tran.Start();
            //    note = TextNote.Create(doc, doc.ActiveView.Id, origin,
            //        "Text Note", defaultTypeId);
            //    note.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L);
            //    //FormatText(note);
            //    tran.Commit();
            //}






            //////获取相应的字体



            //////TextNode titleNode = null;

            ////////设置字体的宽度系数等等
            //////System.Windows.Forms.Control control = new System.Windows.Forms.Control();
            //////System.Drawing.Graphics g = control.CreateGraphics();
            //////System.Drawing.SizeF sizeFOrigin = g.MeasureString("宋", new System.Drawing.Font("宋体", 5));
            ////////System.Drawing.SizeF sizeF = g.MeasureString(titleNode.Font
            //////    , new System.Drawing.Font(titleTextSet.Font, 5));
            //////g.Dispose();
            //////double r1 = 1.48 * (sizeF.Width / sizeFOrigin.Width); //宽度系数
            //////double r2 = 1.8 * (sizeF.Height / sizeFOrigin.Height); //高度系数




            //List就是你要的东西

            //修改字体
            //TextNode textNode =
            //textNoteType.get_Parameter(BuiltInParameter.TEXT_FONT).Set("宋体");


            //TaskDialog.Show("S",s);


            return Result.Succeeded;
        }

        /// <summary>
        /// 设置字体的形式,改变为宽度和倾斜
        /// </summary>
        /// <param name="textNote"></param>
        public void FormatText(TextNote textNote)
        {
            // TextNote created with "New sample text"
            //修改了前三个
            int len = textNote.Text.Length;
            FormattedText formatText = textNote.GetFormattedText();

            // italicize "New"
            TextRange range = new TextRange(0, len);
            //formatText.SetItalicStatus(range, true);

            // make "sample" bold
            range = formatText.Find("sample", 0, false, true);
            if (range.Length > 0)
                formatText.SetBoldStatus(range, true);

            // make "text" underlined
            //range = formatText.Find("text", 0, false, true);
            //if (range.Length > 0)
            //    formatText.SetUnderlineStatus(range, true);

            // make all text uppercase
            formatText.SetAllCapsStatus(true);

            textNote.SetFormattedText(formatText);
        }

        /// <summary>
        /// 设置文字字体
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="doc"></param>
        public void SetTextNode(UIDocument uidoc, Document doc) {

            //UIApplication uiApp = new UIApplication(commandData.Application.Application);
            //UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            //Document doc = commandData.Application.ActiveUIDocument.Document;
            using (Transaction trans = new Transaction(doc, "Text Node creation"))
            {
                trans.Start();
                XYZ textLoc = uidoc.Selection.PickPoint("Pick a point for sample text.");
                ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
                TextNoteOptions opts = new TextNoteOptions(defaultTextTypeId);
                opts.HorizontalAlignment = HorizontalTextAlignment.Left;//设置文字的位置
                opts.Rotation = Math.PI / 4;//设置文字的倾斜角度
                TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, textLoc, DateTime.Now.ToString(), opts);
                trans.Commit();
            }

        }
    }
    public class TextFontClass : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            //throw new NotImplementedException();
            if (elem.Category.Name == "文字注释") {
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
