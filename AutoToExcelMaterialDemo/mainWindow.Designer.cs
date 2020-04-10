namespace AutoToExcelMaterialDemo
{
    partial class mainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cmb_First = new System.Windows.Forms.ComboBox();
            this.cmb_second = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txb_min_X = new System.Windows.Forms.TextBox();
            this.txb_min_Y = new System.Windows.Forms.TextBox();
            this.txb_min_Z = new System.Windows.Forms.TextBox();
            this.txb_max_Z = new System.Windows.Forms.TextBox();
            this.txb_max_Y = new System.Windows.Forms.TextBox();
            this.txb_max_X = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "视图名称";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(86, 56);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(121, 21);
            this.textBox1.TabIndex = 1;
            // 
            // cmb_First
            // 
            this.cmb_First.FormattingEnabled = true;
            this.cmb_First.Location = new System.Drawing.Point(86, 119);
            this.cmb_First.Name = "cmb_First";
            this.cmb_First.Size = new System.Drawing.Size(121, 20);
            this.cmb_First.TabIndex = 2;
            // 
            // cmb_second
            // 
            this.cmb_second.FormattingEnabled = true;
            this.cmb_second.Location = new System.Drawing.Point(281, 119);
            this.cmb_second.Name = "cmb_second";
            this.cmb_second.Size = new System.Drawing.Size(121, 20);
            this.cmb_second.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "上标高：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(251, 182);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "坐标信息";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 237);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "最大点：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(116, 210);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(239, 210);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "Y";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(357, 210);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "Z";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // txb_min_X
            // 
            this.txb_min_X.Location = new System.Drawing.Point(86, 234);
            this.txb_min_X.Name = "txb_min_X";
            this.txb_min_X.Size = new System.Drawing.Size(100, 21);
            this.txb_min_X.TabIndex = 10;
            // 
            // txb_min_Y
            // 
            this.txb_min_Y.Location = new System.Drawing.Point(215, 234);
            this.txb_min_Y.Name = "txb_min_Y";
            this.txb_min_Y.Size = new System.Drawing.Size(100, 21);
            this.txb_min_Y.TabIndex = 11;
            // 
            // txb_min_Z
            // 
            this.txb_min_Z.Location = new System.Drawing.Point(336, 234);
            this.txb_min_Z.Name = "txb_min_Z";
            this.txb_min_Z.Size = new System.Drawing.Size(100, 21);
            this.txb_min_Z.TabIndex = 12;
            // 
            // txb_max_Z
            // 
            this.txb_max_Z.Location = new System.Drawing.Point(336, 303);
            this.txb_max_Z.Name = "txb_max_Z";
            this.txb_max_Z.Size = new System.Drawing.Size(100, 21);
            this.txb_max_Z.TabIndex = 19;
            // 
            // txb_max_Y
            // 
            this.txb_max_Y.Location = new System.Drawing.Point(214, 303);
            this.txb_max_Y.Name = "txb_max_Y";
            this.txb_max_Y.Size = new System.Drawing.Size(100, 21);
            this.txb_max_Y.TabIndex = 18;
            // 
            // txb_max_X
            // 
            this.txb_max_X.Location = new System.Drawing.Point(86, 303);
            this.txb_max_X.Name = "txb_max_X";
            this.txb_max_X.Size = new System.Drawing.Size(100, 21);
            this.txb_max_X.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(357, 279);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "Z";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(239, 279);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(11, 12);
            this.label9.TabIndex = 15;
            this.label9.Text = "Y";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(116, 279);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(11, 12);
            this.label10.TabIndex = 14;
            this.label10.Text = "X";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(27, 306);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 13;
            this.label11.Text = "最小点：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(222, 122);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 20;
            this.label12.Text = "下标高：";
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(200, 374);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 21;
            this.btn_OK.Text = "button1";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // mainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txb_max_Z);
            this.Controls.Add(this.txb_max_Y);
            this.Controls.Add(this.txb_max_X);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txb_min_Z);
            this.Controls.Add(this.txb_min_Y);
            this.Controls.Add(this.txb_min_X);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmb_second);
            this.Controls.Add(this.cmb_First);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "mainWindow";
            this.Text = "局部三维";
            this.Load += new System.EventHandler(this.mainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox cmb_First;
        private System.Windows.Forms.ComboBox cmb_second;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txb_min_X;
        private System.Windows.Forms.TextBox txb_min_Y;
        private System.Windows.Forms.TextBox txb_min_Z;
        private System.Windows.Forms.TextBox txb_max_Z;
        private System.Windows.Forms.TextBox txb_max_Y;
        private System.Windows.Forms.TextBox txb_max_X;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_OK;
    }
}