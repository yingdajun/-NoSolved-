using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace AutoToExcelMaterialDemo
{
    //根据EXCEL生成楼板的
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;


            //C#读取EXCEL表格内容+反向内容


            //确定房间的边界生成新的楼板

            //赋予楼板新的材质
            //这里必须有一个获取所有材质的词典，到时候查询一波

            //将原有楼板给拆分掉

            
            return Result.Succeeded;
        }
    }
}
