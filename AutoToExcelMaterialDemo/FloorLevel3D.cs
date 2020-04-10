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
    //楼层3D视图
    //好像有原始代码搞起
    //[Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class FloorLevel3D:IExternalCommand
    {
        /// <summary>
        /// 判断元素是否是标高
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public bool IsLevel(Element el)
        {
            Level le = el as Level;
            if (el == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 把元素转换为标高
        /// </summary>
        /// <param name="elems"></param>
        /// <returns></returns>
        public List<Level> getLevel(List<Element> elems)
        {
            List<Level> lists = new List<Level>();
            foreach (Element el in elems)
            {
                if (IsLevel(el))
                {
                    Level temp_level = el as Level;
                    lists.Add(temp_level);
                }

            }
            return lists;
        }
        /// <summary>
        /// 给标高排序，按标高从下到上的循序排
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public List<Level> sortedLevel(List<Level> levels)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                for (int j = 1; j < levels.Count - i; j++)
                {
                    if (levels[i].Elevation > levels[j].Elevation)
                    {
                        Level temp = levels[i];
                        levels[i] = levels[j];
                        levels[j] = temp;
                    }
                }
            }
            //levels.RemoveAt(0);
            return levels;
        }
        /// <summary>
        ///获取所有的标高
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public List<Level> getAllLevels(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<Element> element = collector.OfCategory(BuiltInCategory.OST_Levels).
                WhereElementIsNotElementType().ToList();
            List<Level> lists = getLevel(element);
            return lists;
        }

        /// <summary>
        /// 获取生成局部三维视图所需的包围框
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        public BoundingBoxXYZ getBoundingBox(Selection selection)
        {
            PickedBox pb = selection.PickBox(PickBoxStyle.Directional);
            XYZ min = pb.Min;
            XYZ max = pb.Max;

            double[] xnum = new double[] { min.X, max.X };
            double[] ynum = new double[] { min.Y, max.Y };

            //定义新的最大最小点，即右上角的点和左下角的点
            XYZ new_min = new XYZ(xnum.Min(), ynum.Min(), 0);
            XYZ new_max = new XYZ(xnum.Max(), ynum.Max(), 4000 / 304.8);

            Transform tsf = Transform.Identity;//世界坐标系
            tsf.Origin = XYZ.Zero;
            tsf.BasisX = XYZ.BasisX;
            tsf.BasisY = XYZ.BasisY;
            tsf.BasisZ = XYZ.BasisZ;

            //新的包围框
            BoundingBoxXYZ bounding = new BoundingBoxXYZ();
            bounding.Transform = tsf;
            bounding.Min = new_min;
            bounding.Max = new_max;
            return bounding;
        }
        /// <summary>
        /// 设置局部三位视图
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="bb"></param>
        /// <returns></returns>
        public View3D GenerateView(UIDocument uidoc, BoundingBoxXYZ bb)
        {
            using (Transaction ts = new Transaction(uidoc.Document))
            {
                ts.Start("局部三位");
                View3D view = View3D.CreateIsometric(uidoc.Document, new FilteredElementCollector(uidoc.Document).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().
                    Where(x => x.ViewFamily == ViewFamily.ThreeDimensional).First().Id);
                view.SetSectionBox(bb);
                ts.Commit();
                return view;
            }
        }

        public Middle_Data passData(Document doc, Selection selection)
        {
            Middle_Data data = new Middle_Data();
            data.view_name = "三位视图1";
            List<Level> lists = sortedLevel(getAllLevels(doc));
            data.shang_level = lists;
            data.xia_level = lists;
            data.min = getBoundingBox(selection).Min;
            data.max = getBoundingBox(selection).Max;
            return data;
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            Middle_Data data = passData(doc, selection);
            BoundingBoxXYZ bbox = getBoundingBox(selection);
            mainWindow window = new mainWindow(data, uidoc, bbox);
            window.Show();
            return Result.Succeeded;
        }
    }
}
