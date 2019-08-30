namespace XXY_VisitorMJAsst
{
    partial class LoginFrm_Help
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginFrm_Help));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_CopyAllToDeskTop = new System.Windows.Forms.Button();
            this.btn_DetailedMaster = new System.Windows.Forms.Button();
            this.btn_ConsiceTraining = new System.Windows.Forms.Button();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Exit);
            this.groupBox1.Controls.Add(this.btn_CopyAllToDeskTop);
            this.groupBox1.Controls.Add(this.btn_DetailedMaster);
            this.groupBox1.Controls.Add(this.btn_ConsiceTraining);
            this.groupBox1.Location = new System.Drawing.Point(5, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(579, 95);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btn_CopyAllToDeskTop
            // 
            this.btn_CopyAllToDeskTop.Image = ((System.Drawing.Image)(resources.GetObject("btn_CopyAllToDeskTop.Image")));
            this.btn_CopyAllToDeskTop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_CopyAllToDeskTop.Location = new System.Drawing.Point(311, 32);
            this.btn_CopyAllToDeskTop.Name = "btn_CopyAllToDeskTop";
            this.btn_CopyAllToDeskTop.Size = new System.Drawing.Size(120, 40);
            this.btn_CopyAllToDeskTop.TabIndex = 0;
            this.btn_CopyAllToDeskTop.Text = "全部复制到桌面";
            this.btn_CopyAllToDeskTop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_CopyAllToDeskTop.UseVisualStyleBackColor = true;
            this.btn_CopyAllToDeskTop.Click += new System.EventHandler(this.btn_CopyAllToDeskTop_Click);
            // 
            // btn_DetailedMaster
            // 
            this.btn_DetailedMaster.Image = ((System.Drawing.Image)(resources.GetObject("btn_DetailedMaster.Image")));
            this.btn_DetailedMaster.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_DetailedMaster.Location = new System.Drawing.Point(166, 32);
            this.btn_DetailedMaster.Name = "btn_DetailedMaster";
            this.btn_DetailedMaster.Size = new System.Drawing.Size(92, 40);
            this.btn_DetailedMaster.TabIndex = 0;
            this.btn_DetailedMaster.Text = "详细精通版";
            this.btn_DetailedMaster.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_DetailedMaster.UseVisualStyleBackColor = true;
            this.btn_DetailedMaster.Click += new System.EventHandler(this.btn_DetailedMaster_Click);
            // 
            // btn_ConsiceTraining
            // 
            this.btn_ConsiceTraining.Image = ((System.Drawing.Image)(resources.GetObject("btn_ConsiceTraining.Image")));
            this.btn_ConsiceTraining.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_ConsiceTraining.Location = new System.Drawing.Point(21, 32);
            this.btn_ConsiceTraining.Name = "btn_ConsiceTraining";
            this.btn_ConsiceTraining.Size = new System.Drawing.Size(92, 40);
            this.btn_ConsiceTraining.TabIndex = 0;
            this.btn_ConsiceTraining.Text = "简洁培训版";
            this.btn_ConsiceTraining.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_ConsiceTraining.UseVisualStyleBackColor = true;
            this.btn_ConsiceTraining.Click += new System.EventHandler(this.btn_ConsiceTraining_Click);
            // 
            // btn_Exit
            // 
            this.btn_Exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Exit.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Exit.ForeColor = System.Drawing.Color.Black;
            this.btn_Exit.Image = ((System.Drawing.Image)(resources.GetObject("btn_Exit.Image")));
            this.btn_Exit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Exit.Location = new System.Drawing.Point(480, 32);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(93, 40);
            this.btn_Exit.TabIndex = 19;
            this.btn_Exit.Text = "  退 出";
            this.btn_Exit.UseVisualStyleBackColor = true;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // LoginFrm_Help
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(596, 102);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginFrm_Help";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "请选择要打开的帮助文件";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_DetailedMaster;
        private System.Windows.Forms.Button btn_ConsiceTraining;
        private System.Windows.Forms.Button btn_CopyAllToDeskTop;
        private System.Windows.Forms.Button btn_Exit;
    }
}