using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
namespace AutoToExcelMaterialDemo
{
   public  class JClassDemo
    {
        //交点1
        public  XYZ fristPoint;
        //交点2
        public  XYZ secondPoint;
        //一条边的交点
        public  XYZ intialPoint0;
        //另一条边的交点
        public  XYZ intialPoint1;

        public JClassDemo(XYZ x1,XYZ x2,XYZ x3,XYZ x4) {
            this.intialPoint0 = x1;
            this.intialPoint1 = x2;
            this.fristPoint = x3;
            this.secondPoint = x4;
        }

        public void SetFirst(XYZ xyz) {
            this.fristPoint = xyz;
        }

        public void SetSecond(XYZ xyz)
        {
            this.secondPoint = xyz;
        }

        public void SetIntial0(XYZ xyz)
        {
            this.intialPoint0 = xyz;
        }

        public void SetIntial1(XYZ xyz)
        {
            this.intialPoint1 = xyz;
        }

        public void toReverse() {
            XYZ tmp = null;
            tmp = fristPoint;
            intialPoint0 = tmp;
            tmp = null;
            tmp = secondPoint;
            intialPoint1 = tmp;
        }
       
    }
}
