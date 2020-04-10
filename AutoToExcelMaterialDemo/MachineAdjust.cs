using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace AutoToExcelMaterialDemo
{
    //机械设备标注 397 完成，研究一下引线即可
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class MachineAdjust : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //当前模型+链接模型
            //单个标注+多个标注
            //标注内容
            //族名称+族类型
            //自定义注释
            //ESC弹出
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //TextFontClass textFontClass = new TextFontClass();
            Reference refer = uidoc.Selection.PickObject(ObjectType.Element,new MachineSelectionList()
                , "请选择标记对象");
            FamilyInstance fsi = doc.GetElement(refer) as FamilyInstance;
            //Wall wall = doc.GetElement(refer) as Wall;
            //TaskDialog.Show("fsi",fsi.Name.ToString());


            //Transaction ts = new Transaction(doc, "1");
            //ts.Start();
            ////34
            ////IndependentTag independentTag = CreateIndependentTag(doc, fsi);

            ////EditFamilyToSetParam(doc, independentTag.Family);
            //ts.Commit();

            LocationPoint locationPoint = fsi.Location as LocationPoint;

            XYZ origin = locationPoint.Point;

            string familyName = null;
            familyName = fsi.Name;
            string familySymbolName = null;
            familySymbolName = fsi.Symbol.Name;
            using (Transaction tran = new Transaction(doc, "Creating a Text note"))
            {
                //XYZ origin = new XYZ(10, 10, 0);
                ElementId defaultTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                tran.Start();
                TextNote note = TextNote.Create(doc, doc.ActiveView.Id, origin,familyName+"\n"+familySymbolName, defaultTypeId);
                note.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L);
                
                tran.Commit();
            }


            return Result.Succeeded;
        }
        private IndependentTag CreateIndependentTag(Autodesk.Revit.DB.Document document
            , FamilyInstance familyInstance)

        {

            // make sure active view is not a 3D view

            Autodesk.Revit.DB.View view = document.ActiveView;



            // define tag mode and tag orientation for new tag

            TagMode tagMode =
                TagMode.TM_ADDBY_CATEGORY;

            TagOrientation tagorn =
            //TagOrientation.Vertical;
            TagOrientation.Horizontal;



            // Add the tag to the middle of the wall

            //55-28

            //是这一块的问题嘛
            //LocationCurve wallLoc = familyInstance.Location as LocationCurve;

            LocationPoint familyInstanceLoc = familyInstance.Location as LocationPoint;

            //XYZ wallStart = wallLoc.Curve.GetEndPoint(0);

            //XYZ wallEnd = wallLoc.Curve.GetEndPoint(1);

            //XYZ wallMid = wallLoc.Curve.Evaluate(0.5, true);

            XYZ location = familyInstanceLoc.Point;

            Reference wallRef = new Reference(familyInstance);



            IndependentTag newTag =
                IndependentTag.Create(document
                , view.Id, wallRef, true, tagMode, tagorn, location);


            //List<Subelement> sbList=
            //newTag.GetSubelements();
            //newTag.Name = string;


            if (null == newTag)

            {

                throw new Exception("Create IndependentTag Failed.");

            }



            // newTag.TagText is read-only, so we change the Type Mark type parameter to

            // set the tag text.  The label parameter for the tag family determines

            // what type parameter is used for the tag text.



            //WallType type = wall.WallType;
            FamilySymbol familySymbol = familyInstance.Symbol;



            //Parameter foundParameter = type.LookupParameter("Type Mark");

            //bool result = foundParameter.Set("Hello");



            // set leader mode free

            // otherwise leader end point move with elbow point

            //newTag.TagText

            string s = familyInstance.Name + "\n" + familySymbol.Name;

            //Transaction ts = new Transaction(document, "1");
            //ts.Start();
            //SubTransaction subTransaction = new SubTransaction(document);
            //subTransaction.Start();
            
            //ts.Commit();
            //newTag.Name = s;

            //Parameter foundParameter = familySymbol.LookupParameter("Type Mark");

            //不存在是什么鬼？？？
            //bool result = foundParameter.Set(s);

            newTag.LeaderEndCondition = LeaderEndCondition.Free;

            //XYZ elbowPnt = wallMid + new XYZ(5.0, 5.0, 0.0);
            XYZ elbowPnt = location + new XYZ(5.0, 5.0, 0.0);

            newTag.LeaderElbow = elbowPnt;

            //XYZ headerPnt = wallMid + new XYZ(10.0, 10.0, 0.0);
            XYZ headerPnt = location + new XYZ(10.0, 10.0, 0.0);

            newTag.TagHeadPosition = headerPnt;



            return newTag;

        }
        /// <summary>
        /// 修改族类型
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="family"></param>
        /// <returns></returns>
        private Family EditFamilyToSetParam(Document doc, Family family)
        {
            try
            {
                Document familyDoc = doc.EditFamily(family);
                FamilyParameter familyParamWidth = null;
                FamilyManager familyMgr = familyDoc.FamilyManager;
                FamilyParameterSet parameters = familyMgr.Parameters;
                FamilyType familyType = familyMgr.CurrentType;

                foreach (FamilyParameter param in parameters)
                {
                    if (param.Definition.Name == "标签")
                    {
                        familyParamWidth = param;
                    }
                }
                //修改族类型参数
                using (Transaction trans = new Transaction(familyDoc))
                {
                    trans.Start("Edit Family Parameter");
                    //double width = diameter + double.Parse(PublicValue.paramDic["leftDist"]) * 2;
                    familyMgr.Set(familyParamWidth, "我是英大俊");
                    //familyMgr.Set(familyParamWidth, width / 304.8);
                    trans.Commit();
                }
                Family familyEnd = familyDoc.LoadFamily(familyDoc);
                //Family familyEnd1 = familyDoc.LoadFamily(familyDoc);
                //Family familyEnd = familyDoc.LoadFamily(doc,new IFamilyLoadOptions(), family);
                return familyEnd;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }




        }

        void EditAndLoadFamilyToDocument(Document projectDoc, Document RevitDoc, Element element)
        {
            // 这里是自定义族实例，比如门，窗，桌子… 
            FamilyInstance famInst = element as FamilyInstance;
            // 编辑族，拿到族文档 
            Document familyDoc = projectDoc.EditFamily(famInst.Symbol.Family);
            // 在族文档中添加一个新的参数 
            using (Transaction tran = new Transaction(projectDoc, "Edit family Document."))
            {
                tran.Start();
                string paramName = "MyParam ";
                familyDoc.FamilyManager.AddParameter(paramName, BuiltInParameterGroup.PG_TEXT, ParameterType.Text, false);
                tran.Commit();
            }
            // 将这些修改重新载入到工程文档中 
            Family loadedFamily = familyDoc.LoadFamily(RevitDoc, new projectFamLoadOption());
        }

        class projectFamLoadOption : IFamilyLoadOptions
        {
            bool IFamilyLoadOptions.OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                return true;
            }
            bool IFamilyLoadOptions.OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                source = FamilySource.Project;
                overwriteParameterValues = true;
                return true;
            }
        }
        class MachineSelectionList : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                //throw new NotImplementedException();
                if (elem.Category.Name== "机械设备") {
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
        ;
    }
}
