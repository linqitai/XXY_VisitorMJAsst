namespace XXY_VisitorMJAsst
{
    partial class LoginFrm_Configuration
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginFrm_Configuration));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.TSB_Ok = new System.Windows.Forms.ToolStripButton();
            this.TSB_Exit = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TP数据库配置 = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtn_Windows = new System.Windows.Forms.RadioButton();
            this.rbtn_Sql = new System.Windows.Forms.RadioButton();
            this.rbtn_Net = new System.Windows.Forms.RadioButton();
            this.rbtn_Single = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_DLoginPwd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_DLoginName = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txt_ServerName = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.TP加入年份 = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_JoinAccoutNo = new System.Windows.Forms.TextBox();
            this.TP新建年份 = new System.Windows.Forms.TabPage();
            this.txt_NewAccoutNo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_NewOperatorName = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txt_NewEndUserName = new System.Windows.Forms.TextBox();
            this.txt_NewOperatorNo = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_NewAccoutName = new System.Windows.Forms.TextBox();
            this.sP1 = new System.IO.Ports.SerialPort(this.components);
            this.Opendlg = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TP数据库配置.SuspendLayout();
            this.panel1.SuspendLayout();
            this.TP加入年份.SuspendLayout();
            this.TP新建年份.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSB_Ok,
            this.TSB_Exit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(328, 25);
            this.toolStrip1.TabIndex = 33;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // TSB_Ok
            // 
            this.TSB_Ok.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Ok.Image")));
            this.TSB_Ok.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Ok.Name = "TSB_Ok";
            this.TSB_Ok.Size = new System.Drawing.Size(52, 22);
            this.TSB_Ok.Text = "保存";
            this.TSB_Ok.Click += new System.EventHandler(this.TSB_Ok_Click);
            // 
            // TSB_Exit
            // 
            this.TSB_Exit.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Exit.Image")));
            this.TSB_Exit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Exit.Name = "TSB_Exit";
            this.TSB_Exit.Size = new System.Drawing.Size(52, 22);
            this.TSB_Exit.Text = "退出";
            this.TSB_Exit.Click += new System.EventHandler(this.TSB_Exit_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TP数据库配置);
            this.tabControl1.Controls.Add(this.TP加入年份);
            this.tabControl1.Controls.Add(this.TP新建年份);
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(329, 169);
            this.tabControl1.TabIndex = 34;
            // 
            // TP数据库配置
            // 
            this.TP数据库配置.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.TP数据库配置.Controls.Add(this.label17);
            this.TP数据库配置.Controls.Add(this.panel1);
            this.TP数据库配置.Controls.Add(this.rbtn_Net);
            this.TP数据库配置.Controls.Add(this.rbtn_Single);
            this.TP数据库配置.Controls.Add(this.label12);
            this.TP数据库配置.Controls.Add(this.label5);
            this.TP数据库配置.Controls.Add(this.label16);
            this.TP数据库配置.Controls.Add(this.label2);
            this.TP数据库配置.Controls.Add(this.txt_DLoginPwd);
            this.TP数据库配置.Controls.Add(this.label1);
            this.TP数据库配置.Controls.Add(this.txt_DLoginName);
            this.TP数据库配置.Controls.Add(this.Label3);
            this.TP数据库配置.Controls.Add(this.txt_ServerName);
            this.TP数据库配置.Controls.Add(this.Label4);
            this.TP数据库配置.Location = new System.Drawing.Point(4, 22);
            this.TP数据库配置.Name = "TP数据库配置";
            this.TP数据库配置.Size = new System.Drawing.Size(321, 143);
            this.TP数据库配置.TabIndex = 3;
            this.TP数据库配置.Text = "服务器配置";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.Color.Blue;
            this.label17.Location = new System.Drawing.Point(201, 117);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(107, 12);
            this.label17.TabIndex = 30;
            this.label17.Text = "*请不要随意修改！";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtn_Windows);
            this.panel1.Controls.Add(this.rbtn_Sql);
            this.panel1.Location = new System.Drawing.Point(79, 34);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(144, 46);
            this.panel1.TabIndex = 29;
            // 
            // rbtn_Windows
            // 
            this.rbtn_Windows.AutoSize = true;
            this.rbtn_Windows.Location = new System.Drawing.Point(3, 3);
            this.rbtn_Windows.Name = "rbtn_Windows";
            this.rbtn_Windows.Size = new System.Drawing.Size(113, 16);
            this.rbtn_Windows.TabIndex = 2;
            this.rbtn_Windows.Text = "Windows身份验证";
            this.rbtn_Windows.CheckedChanged += new System.EventHandler(this.rbtn_Windows_CheckedChanged);
            // 
            // rbtn_Sql
            // 
            this.rbtn_Sql.AutoSize = true;
            this.rbtn_Sql.Location = new System.Drawing.Point(3, 26);
            this.rbtn_Sql.Name = "rbtn_Sql";
            this.rbtn_Sql.Size = new System.Drawing.Size(131, 16);
            this.rbtn_Sql.TabIndex = 3;
            this.rbtn_Sql.Text = "SQL Server身份验证";
            // 
            // rbtn_Net
            // 
            this.rbtn_Net.AutoSize = true;
            this.rbtn_Net.Location = new System.Drawing.Point(261, 12);
            this.rbtn_Net.Name = "rbtn_Net";
            this.rbtn_Net.Size = new System.Drawing.Size(59, 16);
            this.rbtn_Net.TabIndex = 28;
            this.rbtn_Net.Text = "网络版";
            this.rbtn_Net.UseVisualStyleBackColor = true;
            this.rbtn_Net.Click += new System.EventHandler(this.rbtn_Net_Click);
            // 
            // rbtn_Single
            // 
            this.rbtn_Single.AutoSize = true;
            this.rbtn_Single.Checked = true;
            this.rbtn_Single.Location = new System.Drawing.Point(197, 12);
            this.rbtn_Single.Name = "rbtn_Single";
            this.rbtn_Single.Size = new System.Drawing.Size(59, 16);
            this.rbtn_Single.TabIndex = 27;
            this.rbtn_Single.TabStop = true;
            this.rbtn_Single.Text = "单机版";
            this.rbtn_Single.UseVisualStyleBackColor = true;
            this.rbtn_Single.Click += new System.EventHandler(this.rbtn_Single_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.Blue;
            this.label12.Location = new System.Drawing.Point(233, 114);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 12);
            this.label12.TabIndex = 26;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point(201, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 12);
            this.label5.TabIndex = 25;
            this.label5.Text = "*此窗体一旦配置好,";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(-125, 321);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 12);
            this.label16.TabIndex = 22;
            this.label16.Text = "密码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "服务器名称";
            // 
            // txt_DLoginPwd
            // 
            this.txt_DLoginPwd.Location = new System.Drawing.Point(79, 114);
            this.txt_DLoginPwd.MaxLength = 50;
            this.txt_DLoginPwd.Name = "txt_DLoginPwd";
            this.txt_DLoginPwd.PasswordChar = '*';
            this.txt_DLoginPwd.Size = new System.Drawing.Size(116, 21);
            this.txt_DLoginPwd.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "身份验证";
            // 
            // txt_DLoginName
            // 
            this.txt_DLoginName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_DLoginName.Location = new System.Drawing.Point(79, 86);
            this.txt_DLoginName.MaxLength = 50;
            this.txt_DLoginName.Name = "txt_DLoginName";
            this.txt_DLoginName.Size = new System.Drawing.Size(116, 21);
            this.txt_DLoginName.TabIndex = 4;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(8, 120);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(41, 12);
            this.Label3.TabIndex = 20;
            this.Label3.Text = "密  码";
            // 
            // txt_ServerName
            // 
            this.txt_ServerName.Location = new System.Drawing.Point(79, 9);
            this.txt_ServerName.MaxLength = 50;
            this.txt_ServerName.Name = "txt_ServerName";
            this.txt_ServerName.Size = new System.Drawing.Size(116, 21);
            this.txt_ServerName.TabIndex = 1;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(8, 92);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(41, 12);
            this.Label4.TabIndex = 19;
            this.Label4.Text = "登录名";
            // 
            // TP加入年份
            // 
            this.TP加入年份.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.TP加入年份.Controls.Add(this.label15);
            this.TP加入年份.Controls.Add(this.label6);
            this.TP加入年份.Controls.Add(this.txt_JoinAccoutNo);
            this.TP加入年份.Location = new System.Drawing.Point(4, 22);
            this.TP加入年份.Name = "TP加入年份";
            this.TP加入年份.Size = new System.Drawing.Size(321, 143);
            this.TP加入年份.TabIndex = 6;
            this.TP加入年份.Text = "加入年份";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.Blue;
            this.label15.Location = new System.Drawing.Point(169, 16);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(143, 12);
            this.label15.TabIndex = 24;
            this.label15.Text = "*请输入4位纯数字,如2017";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "年份编号";
            // 
            // txt_JoinAccoutNo
            // 
            this.txt_JoinAccoutNo.Location = new System.Drawing.Point(79, 11);
            this.txt_JoinAccoutNo.MaxLength = 4;
            this.txt_JoinAccoutNo.Name = "txt_JoinAccoutNo";
            this.txt_JoinAccoutNo.Size = new System.Drawing.Size(84, 21);
            this.txt_JoinAccoutNo.TabIndex = 17;
            // 
            // TP新建年份
            // 
            this.TP新建年份.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.TP新建年份.Controls.Add(this.txt_NewAccoutNo);
            this.TP新建年份.Controls.Add(this.label7);
            this.TP新建年份.Controls.Add(this.label11);
            this.TP新建年份.Controls.Add(this.label10);
            this.TP新建年份.Controls.Add(this.label8);
            this.TP新建年份.Controls.Add(this.txt_NewOperatorName);
            this.TP新建年份.Controls.Add(this.label13);
            this.TP新建年份.Controls.Add(this.txt_NewEndUserName);
            this.TP新建年份.Controls.Add(this.txt_NewOperatorNo);
            this.TP新建年份.Controls.Add(this.label9);
            this.TP新建年份.Controls.Add(this.txt_NewAccoutName);
            this.TP新建年份.Location = new System.Drawing.Point(4, 22);
            this.TP新建年份.Name = "TP新建年份";
            this.TP新建年份.Size = new System.Drawing.Size(321, 143);
            this.TP新建年份.TabIndex = 7;
            this.TP新建年份.Text = "新建年份";
            // 
            // txt_NewAccoutNo
            // 
            this.txt_NewAccoutNo.Location = new System.Drawing.Point(78, 6);
            this.txt_NewAccoutNo.MaxLength = 4;
            this.txt_NewAccoutNo.Name = "txt_NewAccoutNo";
            this.txt_NewAccoutNo.Size = new System.Drawing.Size(78, 21);
            this.txt_NewAccoutNo.TabIndex = 24;
            this.txt_NewAccoutNo.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 92);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 33;
            this.label7.Text = "使用单位";
            this.label7.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 25;
            this.label11.Text = "年份编号";
            this.label11.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 38);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 28;
            this.label10.Text = "年份名称";
            this.label10.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 65);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 32;
            this.label8.Text = "操作员姓名";
            this.label8.Visible = false;
            // 
            // txt_NewOperatorName
            // 
            this.txt_NewOperatorName.Location = new System.Drawing.Point(78, 60);
            this.txt_NewOperatorName.Name = "txt_NewOperatorName";
            this.txt_NewOperatorName.Size = new System.Drawing.Size(236, 21);
            this.txt_NewOperatorName.TabIndex = 29;
            this.txt_NewOperatorName.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.Blue;
            this.label13.Location = new System.Drawing.Point(167, 10);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(143, 12);
            this.label13.TabIndex = 34;
            this.label13.Text = "*请输入4位纯数字,如2017";
            this.label13.Visible = false;
            // 
            // txt_NewEndUserName
            // 
            this.txt_NewEndUserName.Location = new System.Drawing.Point(78, 87);
            this.txt_NewEndUserName.Name = "txt_NewEndUserName";
            this.txt_NewEndUserName.Size = new System.Drawing.Size(236, 21);
            this.txt_NewEndUserName.TabIndex = 30;
            this.txt_NewEndUserName.Visible = false;
            // 
            // txt_NewOperatorNo
            // 
            this.txt_NewOperatorNo.Location = new System.Drawing.Point(78, 116);
            this.txt_NewOperatorNo.Name = "txt_NewOperatorNo";
            this.txt_NewOperatorNo.Size = new System.Drawing.Size(78, 21);
            this.txt_NewOperatorNo.TabIndex = 27;
            this.txt_NewOperatorNo.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 65);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 31;
            this.label9.Text = "操作员";
            this.label9.Visible = false;
            // 
            // txt_NewAccoutName
            // 
            this.txt_NewAccoutName.Location = new System.Drawing.Point(78, 33);
            this.txt_NewAccoutName.Name = "txt_NewAccoutName";
            this.txt_NewAccoutName.Size = new System.Drawing.Size(236, 21);
            this.txt_NewAccoutName.TabIndex = 26;
            this.txt_NewAccoutName.Visible = false;
            // 
            // Opendlg
            // 
            this.Opendlg.FileName = "openFileDialog1";
            // 
            // LoginFrm_Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 193);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginFrm_Configuration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "配置";
            this.Load += new System.EventHandler(this.LoginFrm_Configuration_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.TP数据库配置.ResumeLayout(false);
            this.TP数据库配置.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.TP加入年份.ResumeLayout(false);
            this.TP加入年份.PerformLayout();
            this.TP新建年份.ResumeLayout(false);
            this.TP新建年份.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton TSB_Ok;
        private System.Windows.Forms.ToolStripButton TSB_Exit;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TP数据库配置;
        internal System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.RadioButton rbtn_Windows;
        private System.Windows.Forms.TextBox txt_DLoginPwd;
        internal System.Windows.Forms.RadioButton rbtn_Sql;
        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_DLoginName;
        internal System.Windows.Forms.Label Label3;
        private System.Windows.Forms.TextBox txt_ServerName;
        internal System.Windows.Forms.Label Label4;
        private System.Windows.Forms.TabPage TP加入年份;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_JoinAccoutNo;
        private System.IO.Ports.SerialPort sP1;
        private System.Windows.Forms.OpenFileDialog Opendlg;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbtn_Net;
        private System.Windows.Forms.RadioButton rbtn_Single;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TabPage TP新建年份;
        private System.Windows.Forms.TextBox txt_NewAccoutNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_NewOperatorName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txt_NewEndUserName;
        private System.Windows.Forms.TextBox txt_NewOperatorNo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txt_NewAccoutName;
    }
}