using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
namespace AutoToExcelMaterialDemo
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    
    public partial class mainWindow : System.Windows.Forms.Form
    {
        UIDocument _uidoc;
        BoundingBoxXYZ _bb;
         public  mainWindow(Middle_Data data, UIDocument uidoc, BoundingBoxXYZ bbx)
        {
            InitializeComponent();
            textBox1.Text = data.view_name;
            cmb_First.DataSource = data.shang_level;
            cmb_second.DataSource = data.xia_level;
            //最小点
            txb_min_X.Text = data.min.X.ToString();
            txb_min_Y.Text = data.min.Y.ToString();
            txb_min_Z.Text = data.min.Z.ToString();
            //最大点
            txb_max_X.Text = data.max.X.ToString();
            txb_max_Y.Text = data.max.Y.ToString();
            txb_max_Z.Text = data.max.Z.ToString();
            //字段赋值
            this._uidoc = uidoc;
            this._bb = bbx;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
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

        private void btn_OK_Click(object sender, EventArgs e)
        {
            View3D view = GenerateView(_uidoc, _bb);
            _uidoc.ActiveView = view;

        }

        private void mainWindow_Load(object sender, EventArgs e)
        {

        }
    }


}
