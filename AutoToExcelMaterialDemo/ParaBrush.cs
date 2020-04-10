using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace AutoToExcelMaterialDemo
{
    [Transaction(TransactionMode.Manual)]
    class ParaBrush : IExternalCommand
    {
        //橄榄山万能刷功能 完成部分
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            //选择万年刷

            //选择源对象
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference refer = uidoc.Selection.PickObject(ObjectType.Element, "请选择需要刷新的源对象");
            //转化对象类型
            Element element = doc.GetElement(refer) as Element;

            Reference refer1 = uidoc.Selection.PickObject(ObjectType.Element, "请选择需要刷新的复制对象");
            //转化对象类型
            Element element1 = doc.GetElement(refer1) as Element;
            //取出该构件的Category类型
            //将构件的Category获取出来
            Category category = element.Category;

            //封印完成
            //选择属性
            //把这些给去掉
            Parameter paraOrign = element.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
            if (paraOrign.IsReadOnly) {
                TaskDialog.Show("1","该参数不可修改");
            }
            if (paraOrign.UserModifiable) {
                TaskDialog.Show("2","该参数可在交互时修改");
            }
            
            string orignText = paraOrign.AsString();
            Parameter paraCopy= element1.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
            double offset = -10000/304.8;
            
            Transaction ts = new Transaction(doc, "1");
            ts.Start();
            if (paraOrign.StorageType == StorageType.Double)
            {
                TaskDialog.Show("3", "该参数是double类型");
                
                bool flag = paraCopy.Set(Convert.ToDouble(offset));
            }
            if (paraOrign.StorageType == StorageType.Integer)
            {
                TaskDialog.Show("4", "该参数是Int类型");
                bool flag = paraCopy.Set(Convert.ToInt32(offset));
            }
            if (paraOrign.StorageType == StorageType.String)
            {
                TaskDialog.Show("5", "该参数是String类型");
                bool flag = paraCopy.Set(offset.ToString());
            }
            if (paraOrign.StorageType == StorageType.ElementId)
            {
                TaskDialog.Show("6", "该参数是ElementId类型");
                bool flag = paraCopy.Set(new ElementId(Convert.ToInt32(offset)));
            }

            ts.Commit();
            //TaskDialog.Show("flag",flag.ToString());


            ////////////外界源码
            ///

            ///////这里还需要加一个修改材质赋予材质


            #region//过滤出需要的族类型
            FilteredElementCollector FilterElemens = new FilteredElementCollector(doc);
            IList<Element> list1 = FilterElemens.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel).ToElements();
            ElementId familysymbolid = null;
            if (list1 != null)
            {
                foreach (var item in list1)
                {
                    if (item.Name == "测试1")
                    {
                        //修改族类型参数
                        #region
                        ParameterSet parameters = item.Parameters;
                        foreach (Parameter item1 in parameters)
                        {
                            if (item1.Definition.Name == "高度")
                            {
                                //string ss = item1.AsValueString();
                                //TaskDialog.Show("1", ss);
                                Transaction transaction = new Transaction(doc);
                                transaction.Start("李博");
                                double bb = 0.200;
                                item1.Set(bb);
                                transaction.Commit();
                            }
                        }
                        #endregion
                        familysymbolid = item.Id;
                        //修改族实例参数
                        #region

                        FamilyInstanceFilter familyInstanceFilter = new FamilyInstanceFilter(doc, familysymbolid);
                        FilteredElementCollector elementss = new FilteredElementCollector(doc);
                        IList<Element> familuinstancElements = elementss.WherePasses(familyInstanceFilter).ToElements();
                        foreach (var item3 in familuinstancElements)
                        {
                            ParameterSet parameterss = item3.Parameters;
                            foreach (Parameter item4 in parameterss)
                            {
                                if (item4.Definition.Name == "上部分高度")
                                {
                                    Transaction transaction1 = new Transaction(doc);
                                    transaction1.Start("李博 ");
                                    double bb = 0.200;
                                    item4.Set(bb);
                                    transaction1.Commit();
                                }
                            }

                        }
                        #endregion

                    }
                }
            }
            #endregion

            /////////外界源码

            //需要改变的对象

            //获取其中所有关于构件的ID
            List<ElementId> needSelectIdList = new List<ElementId>();
            //打算开一波进程

            //完成
            return Result.Succeeded;
        }
    }
}
