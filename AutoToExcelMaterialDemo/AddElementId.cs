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
    //添加构件ID  350
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class AddElementId : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            //获取共享ID 
            //然后在这个参数这里加上 前缀和后缀 和对应的构件ID
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //先确定需要过滤的参数和共享参数，然后把共享参数添加进去
            //Category -> Name
            //
            //生成立管的方法
            //和普通管道一样获取当前位置 XYZ 为 初始点 ,XYZ 为延伸点（只是比第一个高很多而已）
            return Result.Succeeded;
        }
    }
}
