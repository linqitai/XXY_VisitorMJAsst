namespace XXY_VisitorMJAsst
{
    partial class D_RemoterControlFrm_AwhileOpenDoor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(D_RemoterControlFrm_AwhileOpenDoor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.TSB_Save = new System.Windows.Forms.ToolStripButton();
            this.TSB_Exit = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nud_Second = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Second)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSB_Save,
            this.TSB_Exit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(281, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // TSB_Save
            // 
            this.TSB_Save.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Save.Image")));
            this.TSB_Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Save.Name = "TSB_Save";
            this.TSB_Save.Size = new System.Drawing.Size(52, 22);
            this.TSB_Save.Text = "保存";
            this.TSB_Save.Click += new System.EventHandler(this.TSB_Save_Click);
            // 
            // TSB_Exit
            // 
            this.TSB_Exit.Image = global::XXY_VisitorMJAsst.Properties.Resources.AIM;
            this.TSB_Exit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Exit.Name = "TSB_Exit";
            this.TSB_Exit.Size = new System.Drawing.Size(52, 22);
            this.TSB_Exit.Text = "退出";
            this.TSB_Exit.Click += new System.EventHandler(this.TSB_Exit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nud_Second);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(1, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(279, 72);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(244, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "秒";
            // 
            // nud_Second
            // 
            this.nud_Second.Font = new System.Drawing.Font("宋体", 9F);
            this.nud_Second.Location = new System.Drawing.Point(68, 26);
            this.nud_Second.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nud_Second.Name = "nud_Second";
            this.nud_Second.Size = new System.Drawing.Size(170, 21);
            this.nud_Second.TabIndex = 9;
            this.nud_Second.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nud_Second.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "开门时长";
            // 
            // D_RemoterControlFrm_AwhileOpenDoor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 93);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "D_RemoterControlFrm_AwhileOpenDoor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置开门时长";
            this.Load += new System.EventHandler(this.D_RemoterControlFrm_AwhileOpenDoor_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Second)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton TSB_Save;
        private System.Windows.Forms.ToolStripButton TSB_Exit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nud_Second;
        private System.Windows.Forms.Label label1;
    }
}