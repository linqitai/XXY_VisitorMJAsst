namespace XXY_VisitorMJAsst._3SystemMaintenance
{
    partial class D_OperatorLogFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(D_OperatorLogFrm));
            this.TS1 = new System.Windows.Forms.ToolStrip();
            this.TSB_Refresh = new System.Windows.Forms.ToolStripButton();
            this.TSB_Delete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.TS基础设置 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.TSB_AscOrder = new System.Windows.Forms.ToolStripMenuItem();
            this.TSB_DecOrder = new System.Windows.Forms.ToolStripMenuItem();
            this.TSB_OperatorNoOrder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TSB_ChangingLog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TSB_Print = new System.Windows.Forms.ToolStripMenuItem();
            this.TSB_ExportToExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.TSB_Search = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.TSB_Exit = new System.Windows.Forms.ToolStripButton();
            this.grid1 = new FlexCell.Grid();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TSI_Refresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.TSI_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.TSI_ChangingLog = new System.Windows.Forms.ToolStripMenuItem();
            this.TSI_Search = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cmbox_Class = new System.Windows.Forms.ComboBox();
            this.cmb_List = new System.Windows.Forms.ComboBox();
            this.cmb_code = new System.Windows.Forms.ComboBox();
            this.btn_Refresh = new System.Windows.Forms.Button();
            this.btn_Search = new System.Windows.Forms.Button();
            this.dtp_End = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtp_Start = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtn_Strict = new System.Windows.Forms.RadioButton();
            this.rbtn_Blur = new System.Windows.Forms.RadioButton();
            this.TS1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // TS1
            // 
            this.TS1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSB_Refresh,
            this.TSB_Delete,
            this.toolStripSeparator3,
            this.toolStripSeparator4,
            this.TS基础设置,
            this.TSB_Search,
            this.toolStripSeparator6,
            this.toolStripSeparator5,
            this.TSB_Exit});
            this.TS1.Location = new System.Drawing.Point(0, 0);
            this.TS1.Name = "TS1";
            this.TS1.Size = new System.Drawing.Size(1008, 25);
            this.TS1.TabIndex = 11;
            this.TS1.Text = "toolStrip1";
            // 
            // TSB_Refresh
            // 
            this.TSB_Refresh.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Refresh.Image")));
            this.TSB_Refresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Refresh.Name = "TSB_Refresh";
            this.TSB_Refresh.Size = new System.Drawing.Size(52, 22);
            this.TSB_Refresh.Text = "刷新";
            this.TSB_Refresh.Click += new System.EventHandler(this.TSB_Refresh_Click);
            // 
            // TSB_Delete
            // 
            this.TSB_Delete.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Delete.Image")));
            this.TSB_Delete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Delete.Name = "TSB_Delete";
            this.TSB_Delete.Size = new System.Drawing.Size(52, 22);
            this.TSB_Delete.Text = "删除";
            this.TSB_Delete.Visible = false;
            this.TSB_Delete.Click += new System.EventHandler(this.TSB_Delete_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // TS基础设置
            // 
            this.TS基础设置.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.TSB_ChangingLog,
            this.toolStripSeparator2,
            this.TSB_Print,
            this.TSB_ExportToExcel});
            this.TS基础设置.Image = ((System.Drawing.Image)(resources.GetObject("TS基础设置.Image")));
            this.TS基础设置.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TS基础设置.Name = "TS基础设置";
            this.TS基础设置.Size = new System.Drawing.Size(85, 22);
            this.TS基础设置.Text = "更多操作";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSB_AscOrder,
            this.TSB_DecOrder,
            this.TSB_OperatorNoOrder});
            this.toolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem1.Image")));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItem1.Text = "排序方式";
            // 
            // TSB_AscOrder
            // 
            this.TSB_AscOrder.Name = "TSB_AscOrder";
            this.TSB_AscOrder.Size = new System.Drawing.Size(208, 22);
            this.TSB_AscOrder.Text = "按日志生成顺序升序排序";
            this.TSB_AscOrder.Click += new System.EventHandler(this.TSB_AscOrder_Click);
            // 
            // TSB_DecOrder
            // 
            this.TSB_DecOrder.Name = "TSB_DecOrder";
            this.TSB_DecOrder.Size = new System.Drawing.Size(208, 22);
            this.TSB_DecOrder.Text = "按日志生成顺序降序排序";
            this.TSB_DecOrder.Click += new System.EventHandler(this.TSB_DecOrder_Click);
            // 
            // TSB_OperatorNoOrder
            // 
            this.TSB_OperatorNoOrder.Name = "TSB_OperatorNoOrder";
            this.TSB_OperatorNoOrder.Size = new System.Drawing.Size(208, 22);
            this.TSB_OperatorNoOrder.Text = "按操作员编号顺序排序";
            this.TSB_OperatorNoOrder.Click += new System.EventHandler(this.TSB_OperatorNoOrder_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(138, 6);
            // 
            // TSB_ChangingLog
            // 
            this.TSB_ChangingLog.Name = "TSB_ChangingLog";
            this.TSB_ChangingLog.Size = new System.Drawing.Size(141, 22);
            this.TSB_ChangingLog.Text = "交接班日志";
            this.TSB_ChangingLog.Click += new System.EventHandler(this.TSB_ChangingLog_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(138, 6);
            // 
            // TSB_Print
            // 
            this.TSB_Print.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Print.Image")));
            this.TSB_Print.Name = "TSB_Print";
            this.TSB_Print.Size = new System.Drawing.Size(141, 22);
            this.TSB_Print.Text = "打印";
            this.TSB_Print.Click += new System.EventHandler(this.TSB_Print_Click);
            // 
            // TSB_ExportToExcel
            // 
            this.TSB_ExportToExcel.Image = ((System.Drawing.Image)(resources.GetObject("TSB_ExportToExcel.Image")));
            this.TSB_ExportToExcel.Name = "TSB_ExportToExcel";
            this.TSB_ExportToExcel.Size = new System.Drawing.Size(141, 22);
            this.TSB_ExportToExcel.Text = "导出到Excel";
            this.TSB_ExportToExcel.Click += new System.EventHandler(this.TSB_ExportToExcel_Click);
            // 
            // TSB_Search
            // 
            this.TSB_Search.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Search.Image")));
            this.TSB_Search.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Search.Name = "TSB_Search";
            this.TSB_Search.Size = new System.Drawing.Size(52, 22);
            this.TSB_Search.Text = "查询";
            this.TSB_Search.Click += new System.EventHandler(this.TSB_Search_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
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
            // grid1
            // 
            this.grid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid1.CheckedImage = ((System.Drawing.Bitmap)(resources.GetObject("grid1.CheckedImage")));
            this.grid1.ContextMenuStrip = this.contextMenuStrip1;
            this.grid1.DefaultFont = new System.Drawing.Font("宋体", 9F);
            this.grid1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grid1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.grid1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.grid1.Location = new System.Drawing.Point(0, 61);
            this.grid1.Name = "grid1";
            this.grid1.Size = new System.Drawing.Size(1009, 638);
            this.grid1.TabIndex = 12;
            this.grid1.UncheckedImage = ((System.Drawing.Bitmap)(resources.GetObject("grid1.UncheckedImage")));
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSI_Refresh,
            this.toolStripSeparator8,
            this.toolStripSeparator7,
            this.TSI_Delete,
            this.TSI_ChangingLog,
            this.TSI_Search});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 104);
            // 
            // TSI_Refresh
            // 
            this.TSI_Refresh.Image = ((System.Drawing.Image)(resources.GetObject("TSI_Refresh.Image")));
            this.TSI_Refresh.Name = "TSI_Refresh";
            this.TSI_Refresh.Size = new System.Drawing.Size(136, 22);
            this.TSI_Refresh.Text = "刷新";
            this.TSI_Refresh.Click += new System.EventHandler(this.TSI_Refresh_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(133, 6);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(133, 6);
            // 
            // TSI_Delete
            // 
            this.TSI_Delete.Image = ((System.Drawing.Image)(resources.GetObject("TSI_Delete.Image")));
            this.TSI_Delete.Name = "TSI_Delete";
            this.TSI_Delete.Size = new System.Drawing.Size(136, 22);
            this.TSI_Delete.Text = "删除";
            this.TSI_Delete.Click += new System.EventHandler(this.TSI_Delete_Click);
            // 
            // TSI_ChangingLog
            // 
            this.TSI_ChangingLog.Name = "TSI_ChangingLog";
            this.TSI_ChangingLog.Size = new System.Drawing.Size(136, 22);
            this.TSI_ChangingLog.Text = "交接班日志";
            this.TSI_ChangingLog.Click += new System.EventHandler(this.TSI_ChangingLog_Click);
            // 
            // TSI_Search
            // 
            this.TSI_Search.Image = ((System.Drawing.Image)(resources.GetObject("TSI_Search.Image")));
            this.TSI_Search.Name = "TSI_Search";
            this.TSI_Search.Size = new System.Drawing.Size(136, 22);
            this.TSI_Search.Text = "查询";
            this.TSI_Search.Click += new System.EventHandler(this.TSI_Search_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(0, 705);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1008, 23);
            this.progressBar1.TabIndex = 36;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.dtp_End);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dtp_Start);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1008, 31);
            this.panel1.TabIndex = 37;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.cmbox_Class);
            this.panel4.Controls.Add(this.cmb_List);
            this.panel4.Controls.Add(this.cmb_code);
            this.panel4.Controls.Add(this.btn_Refresh);
            this.panel4.Controls.Add(this.btn_Search);
            this.panel4.Location = new System.Drawing.Point(263, -2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(464, 35);
            this.panel4.TabIndex = 24;
            // 
            // cmbox_Class
            // 
            this.cmbox_Class.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbox_Class.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbox_Class.Font = new System.Drawing.Font("宋体", 11F);
            this.cmbox_Class.ForeColor = System.Drawing.Color.Blue;
            this.cmbox_Class.FormattingEnabled = true;
            this.cmbox_Class.Location = new System.Drawing.Point(199, 5);
            this.cmbox_Class.MaxDropDownItems = 50;
            this.cmbox_Class.Name = "cmbox_Class";
            this.cmbox_Class.Size = new System.Drawing.Size(95, 23);
            this.cmbox_Class.TabIndex = 10;
            this.cmbox_Class.SelectedIndexChanged += new System.EventHandler(this.cmbox_Class_SelectedIndexChanged);
            this.cmbox_Class.Enter += new System.EventHandler(this.cmbox_Class_Enter);
            this.cmbox_Class.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbox_Class_KeyPress);
            this.cmbox_Class.Leave += new System.EventHandler(this.cmbox_Class_Leave);
            this.cmbox_Class.MouseEnter += new System.EventHandler(this.cmbox_Class_MouseEnter);
            this.cmbox_Class.MouseLeave += new System.EventHandler(this.cmbox_Class_MouseLeave);
            // 
            // cmb_List
            // 
            this.cmb_List.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_List.Font = new System.Drawing.Font("宋体", 10.5F);
            this.cmb_List.ForeColor = System.Drawing.Color.Blue;
            this.cmb_List.FormattingEnabled = true;
            this.cmb_List.ItemHeight = 14;
            this.cmb_List.Location = new System.Drawing.Point(3, 6);
            this.cmb_List.MaxDropDownItems = 30;
            this.cmb_List.Name = "cmb_List";
            this.cmb_List.Size = new System.Drawing.Size(121, 22);
            this.cmb_List.TabIndex = 8;
            this.cmb_List.SelectedIndexChanged += new System.EventHandler(this.cmb_List_SelectedIndexChanged);
            // 
            // cmb_code
            // 
            this.cmb_code.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_code.Font = new System.Drawing.Font("宋体", 10.5F);
            this.cmb_code.ForeColor = System.Drawing.Color.Blue;
            this.cmb_code.FormattingEnabled = true;
            this.cmb_code.Items.AddRange(new object[] {
            "LIKE",
            "=",
            ">",
            "<",
            ">=",
            "<=",
            "!="});
            this.cmb_code.Location = new System.Drawing.Point(132, 6);
            this.cmb_code.Name = "cmb_code";
            this.cmb_code.Size = new System.Drawing.Size(62, 22);
            this.cmb_code.TabIndex = 9;
            // 
            // btn_Refresh
            // 
            this.btn_Refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Refresh.Image = ((System.Drawing.Image)(resources.GetObject("btn_Refresh.Image")));
            this.btn_Refresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Refresh.Location = new System.Drawing.Point(386, 3);
            this.btn_Refresh.Name = "btn_Refresh";
            this.btn_Refresh.Size = new System.Drawing.Size(64, 26);
            this.btn_Refresh.TabIndex = 12;
            this.btn_Refresh.Text = "刷新";
            this.btn_Refresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_Refresh.UseVisualStyleBackColor = true;
            this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
            // 
            // btn_Search
            // 
            this.btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Search.Image = ((System.Drawing.Image)(resources.GetObject("btn_Search.Image")));
            this.btn_Search.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Search.Location = new System.Drawing.Point(300, 3);
            this.btn_Search.Name = "btn_Search";
            this.btn_Search.Size = new System.Drawing.Size(64, 26);
            this.btn_Search.TabIndex = 11;
            this.btn_Search.Text = "查询";
            this.btn_Search.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_Search.UseVisualStyleBackColor = true;
            this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
            // 
            // dtp_End
            // 
            this.dtp_End.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp_End.Location = new System.Drawing.Point(168, 4);
            this.dtp_End.Name = "dtp_End";
            this.dtp_End.Size = new System.Drawing.Size(91, 21);
            this.dtp_End.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "至";
            // 
            // dtp_Start
            // 
            this.dtp_Start.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp_Start.Location = new System.Drawing.Point(58, 4);
            this.dtp_Start.Name = "dtp_Start";
            this.dtp_Start.Size = new System.Drawing.Size(91, 21);
            this.dtp_Start.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "操作时间";
            // 
            // rbtn_Strict
            // 
            this.rbtn_Strict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtn_Strict.AutoSize = true;
            this.rbtn_Strict.Location = new System.Drawing.Point(930, 707);
            this.rbtn_Strict.Name = "rbtn_Strict";
            this.rbtn_Strict.Size = new System.Drawing.Size(71, 16);
            this.rbtn_Strict.TabIndex = 39;
            this.rbtn_Strict.Text = "严格查询";
            this.rbtn_Strict.UseVisualStyleBackColor = true;
            this.rbtn_Strict.CheckedChanged += new System.EventHandler(this.rbtn_Strict_CheckedChanged);
            // 
            // rbtn_Blur
            // 
            this.rbtn_Blur.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtn_Blur.AutoSize = true;
            this.rbtn_Blur.Checked = true;
            this.rbtn_Blur.Location = new System.Drawing.Point(853, 707);
            this.rbtn_Blur.Name = "rbtn_Blur";
            this.rbtn_Blur.Size = new System.Drawing.Size(71, 16);
            this.rbtn_Blur.TabIndex = 38;
            this.rbtn_Blur.TabStop = true;
            this.rbtn_Blur.Text = "模糊查询";
            this.rbtn_Blur.UseVisualStyleBackColor = true;
            this.rbtn_Blur.CheckedChanged += new System.EventHandler(this.rbtn_Blur_CheckedChanged);
            // 
            // D_OperatorLogFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.rbtn_Strict);
            this.Controls.Add(this.rbtn_Blur);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grid1);
            this.Controls.Add(this.TS1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "D_OperatorLogFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统操作日志";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.D_OperatorLogFrm_FormClosing);
            this.Load += new System.EventHandler(this.D_OperatorLogFrm_Load);
            this.TS1.ResumeLayout(false);
            this.TS1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip TS1;
        private System.Windows.Forms.ToolStripButton TSB_Delete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripDropDownButton TS基础设置;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem TSB_AscOrder;
        private System.Windows.Forms.ToolStripMenuItem TSB_OperatorNoOrder;
        private System.Windows.Forms.ToolStripMenuItem TSB_DecOrder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem TSB_Print;
        private System.Windows.Forms.ToolStripMenuItem TSB_ExportToExcel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton TSB_Search;
        private FlexCell.Grid grid1;
        private System.Windows.Forms.ToolStripButton TSB_Refresh;
        private System.Windows.Forms.ToolStripButton TSB_Exit;
        private System.Windows.Forms.ToolStripMenuItem TSB_ChangingLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem TSI_Refresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem TSI_Delete;
        private System.Windows.Forms.ToolStripMenuItem TSI_ChangingLog;
        private System.Windows.Forms.ToolStripMenuItem TSI_Search;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ComboBox cmbox_Class;
        private System.Windows.Forms.ComboBox cmb_List;
        private System.Windows.Forms.ComboBox cmb_code;
        private System.Windows.Forms.Button btn_Refresh;
        private System.Windows.Forms.Button btn_Search;
        private System.Windows.Forms.DateTimePicker dtp_End;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtp_Start;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbtn_Strict;
        private System.Windows.Forms.RadioButton rbtn_Blur;
    }
}