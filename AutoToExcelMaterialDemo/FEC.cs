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
    [Transaction(TransactionMode.Manual)]
    class FEC : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            // 获取所有的轴网
            List<Grid> allGrids = new List<Grid>();
            FilteredElementCollector gridCollector = new FilteredElementCollector(doc,doc.ActiveView.Id);
            //gridCollector.OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType();
            gridCollector.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType();
            TaskDialog.Show("1",gridCollector.Count().ToString());

            return Result.Succeeded;
        }
    }
}
