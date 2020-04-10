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
    // 批量构件对齐  377
    //好好看一波
    [Transaction(TransactionMode.Manual)]
    class ElementByAL : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;


            //快速撤回不知道咋做
            //基准图元不就和立管对齐一样嘛
            //对齐方式表示要好多好多其他函数
            //老子要做7-8个！！！！
            return Result.Succeeded;
        }
    }
}
