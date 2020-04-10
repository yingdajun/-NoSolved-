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
    //线管翻模 我不想做了，MMP
    [Transaction(TransactionMode.Manual)]
    class XianGuanFanMo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();

            return Result.Succeeded;
        }
    }
}
