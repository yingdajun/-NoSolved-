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
    // 生成放坡 378 我不会做 
    //楼层打断立管  388 //这个应该很容易
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class FangPo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //doc.Create.NewSlab();
            //获取坡道
            IList<Reference> referList = uidoc.Selection.PickObjects(ObjectType.Element, "请选择需要生成的坡道");
            //
            //doc.Create.NewSlab();

            return Result.Succeeded;
        }
    }
}
