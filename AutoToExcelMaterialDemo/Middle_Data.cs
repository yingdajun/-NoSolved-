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
   public class Middle_Data
    {
        //字段
        private string Name;
        private List<Level> Shang_Level;
        private List<Level> Xia_Level;

        //private List<string> Shang_Level;
        //private List<string> Xia_Level;
        private XYZ Max;
        private XYZ Min;
        //属性
        public string view_name { get; set; }//视图名称
        public List<Level> shang_level { get; set; }//上标高
        public List<Level> xia_level { get; set; }//下标高
        public XYZ max { get; set; }//最大点的坐标
        public XYZ min { get; set; }//最小点的坐标
        //constructor
        //public Middle_Data(string n,Level[] s,Level[] x,XYZ mi,)
    }
    
}
