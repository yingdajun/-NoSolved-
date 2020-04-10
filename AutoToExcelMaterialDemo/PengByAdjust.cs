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
    // 喷管批量连接管道  390
    [Transaction(TransactionMode.Manual)]
    class PengByAdjust : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            //UIApplication uiapp = commandData.Application;
            //Application app = uiapp.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            

            return Result.Succeeded;
        }
    }
}
