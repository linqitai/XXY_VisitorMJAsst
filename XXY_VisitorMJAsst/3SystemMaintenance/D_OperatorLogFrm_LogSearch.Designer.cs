namespace XXY_VisitorMJAsst._3SystemMaintenance
{
    partial class D_OperatorLogFrm_LogSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(D_OperatorLogFrm_LogSearch));
            this.dtp_OperateDTEnd = new System.Windows.Forms.DateTimePicker();
            this.ckbox_OperateDTEnd = new System.Windows.Forms.CheckBox();
            this.dtp_OperateDTStart = new System.Windows.Forms.DateTimePicker();
            this.ckbox_OperateDTStart = new System.Windows.Forms.CheckBox();
            this.cmbox_FormName = new System.Windows.Forms.ComboBox();
            this.ckbox_FormName = new System.Windows.Forms.CheckBox();
            this.cmbox_Result = new System.Windows.Forms.ComboBox();
            this.ckbox_Result = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbox_OperatorName = new System.Windows.Forms.ComboBox();
            this.ckbox_OperatorName = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.TSB_Search = new System.Windows.Forms.ToolStripButton();
            this.TSB_Exit = new System.Windows.Forms.ToolStripButton();
            this.rbtn_Strict = new System.Windows.Forms.RadioButton();
            this.rbtn_Blur = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtp_OperateDTEnd
            // 
            this.dtp_OperateDTEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp_OperateDTEnd.Location = new System.Drawing.Point(238, 17);
            this.dtp_OperateDTEnd.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtp_OperateDTEnd.Name = "dtp_OperateDTEnd";
            this.dtp_OperateDTEnd.Size = new System.Drawing.Size(102, 21);
            this.dtp_OperateDTEnd.TabIndex = 147;
            this.dtp_OperateDTEnd.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtp_OperateDTEnd.ValueChanged += new System.EventHandler(this.dtp_OperateDTEnd_ValueChanged);
            this.dtp_OperateDTEnd.Leave += new System.EventHandler(this.dtp_OperateDTEnd_Leave);
            // 
            // ckbox_OperateDTEnd
            // 
            this.ckbox_OperateDTEnd.AutoSize = true;
            this.ckbox_OperateDTEnd.Enabled = false;
            this.ckbox_OperateDTEnd.ForeColor = System.Drawing.Color.Black;
            this.ckbox_OperateDTEnd.Location = new System.Drawing.Point(196, 20);
            this.ckbox_OperateDTEnd.Name = "ckbox_OperateDTEnd";
            this.ckbox_OperateDTEnd.Size = new System.Drawing.Size(36, 16);
            this.ckbox_OperateDTEnd.TabIndex = 148;
            this.ckbox_OperateDTEnd.Text = "至";
            this.ckbox_OperateDTEnd.UseVisualStyleBackColor = true;
            // 
            // dtp_OperateDTStart
            // 
            this.dtp_OperateDTStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp_OperateDTStart.Location = new System.Drawing.Point(90, 17);
            this.dtp_OperateDTStart.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtp_OperateDTStart.Name = "dtp_OperateDTStart";
            this.dtp_OperateDTStart.Size = new System.Drawing.Size(100, 21);
            this.dtp_OperateDTStart.TabIndex = 87;
            this.dtp_OperateDTStart.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtp_OperateDTStart.ValueChanged += new System.EventHandler(this.dtp_OperateDTStart_ValueChanged);
            this.dtp_OperateDTStart.Leave += new System.EventHandler(this.dtp_OperateDTStart_Leave);
            // 
            // ckbox_OperateDTStart
            // 
            this.ckbox_OperateDTStart.AutoSize = true;
            this.ckbox_OperateDTStart.Enabled = false;
            this.ckbox_OperateDTStart.Location = new System.Drawing.Point(6, 20);
            this.ckbox_OperateDTStart.Name = "ckbox_OperateDTStart";
            this.ckbox_OperateDTStart.Size = new System.Drawing.Size(72, 16);
            this.ckbox_OperateDTStart.TabIndex = 86;
            this.ckbox_OperateDTStart.Text = "操作时间";
            this.ckbox_OperateDTStart.UseVisualStyleBackColor = true;
            // 
            // cmbox_FormName
            // 
            this.cmbox_FormName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbox_FormName.FormattingEnabled = true;
            this.cmbox_FormName.Location = new System.Drawing.Point(90, 71);
            this.cmbox_FormName.Name = "cmbox_FormName";
            this.cmbox_FormName.Size = new System.Drawing.Size(250, 20);
            this.cmbox_FormName.TabIndex = 85;
            this.cmbox_FormName.SelectedIndexChanged += new System.EventHandler(this.cmb_FormName_SelectedIndexChanged);
            // 
            // ckbox_FormName
            // 
            this.ckbox_FormName.AutoSize = true;
            this.ckbox_FormName.Enabled = false;
            this.ckbox_FormName.Location = new System.Drawing.Point(6, 74);
            this.ckbox_FormName.Name = "ckbox_FormName";
            this.ckbox_FormName.Size = new System.Drawing.Size(72, 16);
            this.ckbox_FormName.TabIndex = 84;
            this.ckbox_FormName.Text = "所在窗体";
            this.ckbox_FormName.UseVisualStyleBackColor = true;
            // 
            // cmbox_Result
            // 
            this.cmbox_Result.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbox_Result.FormattingEnabled = true;
            this.cmbox_Result.Items.AddRange(new object[] {
            "",
            "成功",
            "失败"});
            this.cmbox_Result.Location = new System.Drawing.Point(278, 45);
            this.cmbox_Result.Name = "cmbox_Result";
            this.cmbox_Result.Size = new System.Drawing.Size(62, 20);
            this.cmbox_Result.TabIndex = 25;
            this.cmbox_Result.SelectedIndexChanged += new System.EventHandler(this.cmbox_Result_SelectedIndexChanged);
            // 
            // ckbox_Result
            // 
            this.ckbox_Result.AutoSize = true;
            this.ckbox_Result.Enabled = false;
            this.ckbox_Result.ForeColor = System.Drawing.Color.Black;
            this.ckbox_Result.Location = new System.Drawing.Point(208, 49);
            this.ckbox_Result.Name = "ckbox_Result";
            this.ckbox_Result.Size = new System.Drawing.Size(72, 16);
            this.ckbox_Result.TabIndex = 83;
            this.ckbox_Result.Text = "操作结果";
            this.ckbox_Result.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbox_OperatorName);
            this.groupBox1.Controls.Add(this.dtp_OperateDTEnd);
            this.groupBox1.Controls.Add(this.cmbox_FormName);
            this.groupBox1.Controls.Add(this.ckbox_FormName);
            this.groupBox1.Controls.Add(this.ckbox_OperateDTEnd);
            this.groupBox1.Controls.Add(this.cmbox_Result);
            this.groupBox1.Controls.Add(this.ckbox_Result);
            this.groupBox1.Controls.Add(this.dtp_OperateDTStart);
            this.groupBox1.Controls.Add(this.ckbox_OperatorName);
            this.groupBox1.Controls.Add(this.ckbox_OperateDTStart);
            this.groupBox1.Location = new System.Drawing.Point(3, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(345, 98);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            // 
            // cmbox_OperatorName
            // 
            this.cmbox_OperatorName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbox_OperatorName.FormattingEnabled = true;
            this.cmbox_OperatorName.Location = new System.Drawing.Point(90, 45);
            this.cmbox_OperatorName.Name = "cmbox_OperatorName";
            this.cmbox_OperatorName.Size = new System.Drawing.Size(115, 20);
            this.cmbox_OperatorName.TabIndex = 149;
            this.cmbox_OperatorName.SelectedIndexChanged += new System.EventHandler(this.cmbox_OperatorName_SelectedIndexChanged);
            // 
            // ckbox_OperatorName
            // 
            this.ckbox_OperatorName.AutoSize = true;
            this.ckbox_OperatorName.Enabled = false;
            this.ckbox_OperatorName.ForeColor = System.Drawing.Color.Black;
            this.ckbox_OperatorName.Location = new System.Drawing.Point(6, 49);
            this.ckbox_OperatorName.Name = "ckbox_OperatorName";
            this.ckbox_OperatorName.Size = new System.Drawing.Size(84, 16);
            this.ckbox_OperatorName.TabIndex = 80;
            this.ckbox_OperatorName.Text = "操作员姓名";
            this.ckbox_OperatorName.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSB_Search,
            this.TSB_Exit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(351, 25);
            this.toolStrip1.TabIndex = 25;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // TSB_Search
            // 
            this.TSB_Search.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Search.Image")));
            this.TSB_Search.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Search.Name = "TSB_Search";
            this.TSB_Search.Size = new System.Drawing.Size(49, 22);
            this.TSB_Search.Text = "查询";
            this.TSB_Search.Click += new System.EventHandler(this.TSB_Search_Click);
            // 
            // TSB_Exit
            // 
            this.TSB_Exit.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Exit.Image")));
            this.TSB_Exit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Exit.Name = "TSB_Exit";
            this.TSB_Exit.Size = new System.Drawing.Size(49, 22);
            this.TSB_Exit.Text = "退出";
            this.TSB_Exit.Click += new System.EventHandler(this.TSB_Exit_Click);
            // 
            // rbtn_Strict
            // 
            this.rbtn_Strict.AutoSize = true;
            this.rbtn_Strict.Location = new System.Drawing.Point(80, 128);
            this.rbtn_Strict.Name = "rbtn_Strict";
            this.rbtn_Strict.Size = new System.Drawing.Size(71, 16);
            this.rbtn_Strict.TabIndex = 28;
            this.rbtn_Strict.TabStop = true;
            this.rbtn_Strict.Text = "严格查询";
            this.rbtn_Strict.UseVisualStyleBackColor = true;
            this.rbtn_Strict.CheckedChanged += new System.EventHandler(this.rbtn_Strict_CheckedChanged);
            // 
            // rbtn_Blur
            // 
            this.rbtn_Blur.AutoSize = true;
            this.rbtn_Blur.Location = new System.Drawing.Point(3, 128);
            this.rbtn_Blur.Name = "rbtn_Blur";
            this.rbtn_Blur.Size = new System.Drawing.Size(71, 16);
            this.rbtn_Blur.TabIndex = 27;
            this.rbtn_Blur.TabStop = true;
            this.rbtn_Blur.Text = "模糊查询";
            this.rbtn_Blur.UseVisualStyleBackColor = true;
            this.rbtn_Blur.CheckedChanged += new System.EventHandler(this.rbtn_Blur_CheckedChanged);
            // 
            // D_OperatorLogFrm_LogSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(351, 147);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.rbtn_Strict);
            this.Controls.Add(this.rbtn_Blur);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "D_OperatorLogFrm_LogSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统操作日志查询";
            this.Load += new System.EventHandler(this.D_OperatorLogFrm_LogSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtp_OperateDTEnd;
        private System.Windows.Forms.CheckBox ckbox_OperateDTEnd;
        private System.Windows.Forms.DateTimePicker dtp_OperateDTStart;
        private System.Windows.Forms.CheckBox ckbox_OperateDTStart;
        private System.Windows.Forms.ComboBox cmbox_FormName;
        private System.Windows.Forms.CheckBox ckbox_FormName;
        private System.Windows.Forms.ComboBox cmbox_Result;
        private System.Windows.Forms.CheckBox ckbox_Result;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckbox_OperatorName;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton TSB_Search;
        private System.Windows.Forms.ToolStripButton TSB_Exit;
        private System.Windows.Forms.RadioButton rbtn_Strict;
        private System.Windows.Forms.RadioButton rbtn_Blur;
        private System.Windows.Forms.ComboBox cmbox_OperatorName;
    }
}