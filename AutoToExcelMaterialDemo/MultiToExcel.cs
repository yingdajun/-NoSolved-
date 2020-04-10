using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoToExcelMaterialDemo
{
    
    public partial class MultiToExcel : Form
    {
        public static List<string> inputsList = new List<string>();
        public static List<string> outputList = new List<string>();
        public MultiToExcel(List<string> inputList)
        {
            InitializeComponent();
            inputsList = inputList;
        }

        //导出
        private void button1_Click(object sender, EventArgs e)
        {
            
            outputList = new List<string>();
            //遍历当前的ListBox
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //获取当前的需要选择的位置
        }
    }
}
