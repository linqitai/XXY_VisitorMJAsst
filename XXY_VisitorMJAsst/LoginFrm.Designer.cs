namespace XXY_VisitorMJAsst
{
    partial class LoginFrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginFrm));
            this.pic_Exit = new System.Windows.Forms.PictureBox();
            this.txt_Password = new System.Windows.Forms.TextBox();
            this.sP1 = new System.IO.Ports.SerialPort(this.components);
            this.timer_SIdRead = new System.Windows.Forms.Timer(this.components);
            this.timer_CardRead = new System.Windows.Forms.Timer(this.components);
            this.pbox_Logo = new System.Windows.Forms.PictureBox();
            this.txt_Operator = new System.Windows.Forms.TextBox();
            this.picbox_Operator = new System.Windows.Forms.PictureBox();
            this.txt_AccoutList = new System.Windows.Forms.TextBox();
            this.picbox_AccoutList = new System.Windows.Forms.PictureBox();
            this.grid_AccoutList = new FlexCell.Grid();
            this.grid_Operator = new FlexCell.Grid();
            this.picbox_ClearRubbish = new System.Windows.Forms.PictureBox();
            this.pic_Help = new System.Windows.Forms.PictureBox();
            this.pic_Configuration = new System.Windows.Forms.PictureBox();
            this.serviceController1 = new System.ServiceProcess.ServiceController();
            this.pic_Enter = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Exit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbox_Logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_Operator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_AccoutList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_ClearRubbish)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Help)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Configuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Enter)).BeginInit();
            this.SuspendLayout();
            // 
            // pic_Exit
            // 
            this.pic_Exit.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pic_Exit.ErrorImage")));
            this.pic_Exit.Image = ((System.Drawing.Image)(resources.GetObject("pic_Exit.Image")));
            this.pic_Exit.InitialImage = ((System.Drawing.Image)(resources.GetObject("pic_Exit.InitialImage")));
            this.pic_Exit.Location = new System.Drawing.Point(476, 204);
            this.pic_Exit.Name = "pic_Exit";
            this.pic_Exit.Size = new System.Drawing.Size(65, 32);
            this.pic_Exit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pic_Exit.TabIndex = 21;
            this.pic_Exit.TabStop = false;
            this.pic_Exit.Click += new System.EventHandler(this.pic_Exit_Click);
            // 
            // txt_Password
            // 
            this.txt_Password.Font = new System.Drawing.Font("宋体", 12F);
            this.txt_Password.ForeColor = System.Drawing.Color.Blue;
            this.txt_Password.Location = new System.Drawing.Point(276, 161);
            this.txt_Password.MaxLength = 50;
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.PasswordChar = '*';
            this.txt_Password.Size = new System.Drawing.Size(304, 26);
            this.txt_Password.TabIndex = 2;
            this.txt_Password.Enter += new System.EventHandler(this.txt_Password_Enter);
            this.txt_Password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Password_KeyPress);
            this.txt_Password.Leave += new System.EventHandler(this.txt_Password_Leave);
            // 
            // timer_SIdRead
            // 
            this.timer_SIdRead.Interval = 1000;
            this.timer_SIdRead.Tick += new System.EventHandler(this.timer_SIdRead_Tick);
            // 
            // timer_CardRead
            // 
            this.timer_CardRead.Tick += new System.EventHandler(this.timer_CardRead_Tick);
            // 
            // pbox_Logo
            // 
            this.pbox_Logo.Location = new System.Drawing.Point(3, 1);
            this.pbox_Logo.Name = "pbox_Logo";
            this.pbox_Logo.Size = new System.Drawing.Size(10, 10);
            this.pbox_Logo.TabIndex = 28;
            this.pbox_Logo.TabStop = false;
            this.pbox_Logo.Visible = false;
            // 
            // txt_Operator
            // 
            this.txt_Operator.Font = new System.Drawing.Font("宋体", 12F);
            this.txt_Operator.ForeColor = System.Drawing.Color.Blue;
            this.txt_Operator.Location = new System.Drawing.Point(276, 127);
            this.txt_Operator.MaxLength = 50;
            this.txt_Operator.Name = "txt_Operator";
            this.txt_Operator.Size = new System.Drawing.Size(304, 26);
            this.txt_Operator.TabIndex = 1;
            this.txt_Operator.Click += new System.EventHandler(this.txt_Operator_Click);
            this.txt_Operator.Enter += new System.EventHandler(this.txt_Operator_Enter);
            this.txt_Operator.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Operator_KeyPress);
            this.txt_Operator.Leave += new System.EventHandler(this.txt_Operator_Leave);
            // 
            // picbox_Operator
            // 
            this.picbox_Operator.BackColor = System.Drawing.Color.White;
            this.picbox_Operator.Image = global::XXY_VisitorMJAsst.Properties.Resources._1;
            this.picbox_Operator.Location = new System.Drawing.Point(559, 130);
            this.picbox_Operator.Name = "picbox_Operator";
            this.picbox_Operator.Size = new System.Drawing.Size(20, 20);
            this.picbox_Operator.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbox_Operator.TabIndex = 30;
            this.picbox_Operator.TabStop = false;
            this.picbox_Operator.Click += new System.EventHandler(this.picbox_Operator_Click);
            this.picbox_Operator.MouseEnter += new System.EventHandler(this.picbox_Operator_MouseEnter);
            this.picbox_Operator.MouseLeave += new System.EventHandler(this.picbox_Operator_MouseLeave);
            // 
            // txt_AccoutList
            // 
            this.txt_AccoutList.BackColor = System.Drawing.Color.White;
            this.txt_AccoutList.Font = new System.Drawing.Font("宋体", 12F);
            this.txt_AccoutList.ForeColor = System.Drawing.Color.Blue;
            this.txt_AccoutList.Location = new System.Drawing.Point(276, 92);
            this.txt_AccoutList.MaxLength = 50;
            this.txt_AccoutList.Name = "txt_AccoutList";
            this.txt_AccoutList.ReadOnly = true;
            this.txt_AccoutList.Size = new System.Drawing.Size(304, 26);
            this.txt_AccoutList.TabIndex = 0;
            this.txt_AccoutList.Click += new System.EventHandler(this.txt_AccoutList_Click);
            this.txt_AccoutList.Enter += new System.EventHandler(this.txt_AccoutList_Enter);
            this.txt_AccoutList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_AccoutList_KeyPress);
            this.txt_AccoutList.Leave += new System.EventHandler(this.txt_AccoutList_Leave);
            // 
            // picbox_AccoutList
            // 
            this.picbox_AccoutList.BackColor = System.Drawing.Color.White;
            this.picbox_AccoutList.Image = global::XXY_VisitorMJAsst.Properties.Resources._1;
            this.picbox_AccoutList.Location = new System.Drawing.Point(559, 95);
            this.picbox_AccoutList.Name = "picbox_AccoutList";
            this.picbox_AccoutList.Size = new System.Drawing.Size(20, 20);
            this.picbox_AccoutList.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbox_AccoutList.TabIndex = 30;
            this.picbox_AccoutList.TabStop = false;
            this.picbox_AccoutList.Click += new System.EventHandler(this.picbox_AccoutList_Click);
            this.picbox_AccoutList.MouseEnter += new System.EventHandler(this.picbox_AccoutList_MouseEnter);
            this.picbox_AccoutList.MouseLeave += new System.EventHandler(this.picbox_AccoutList_MouseLeave);
            // 
            // grid_AccoutList
            // 
            this.grid_AccoutList.CheckedImage = null;
            this.grid_AccoutList.DefaultFont = new System.Drawing.Font("宋体", 9F);
            this.grid_AccoutList.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grid_AccoutList.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.grid_AccoutList.HideGridLines = true;
            this.grid_AccoutList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.grid_AccoutList.Location = new System.Drawing.Point(276, 118);
            this.grid_AccoutList.Name = "grid_AccoutList";
            this.grid_AccoutList.Size = new System.Drawing.Size(304, 16);
            this.grid_AccoutList.TabIndex = 31;
            this.grid_AccoutList.UncheckedImage = ((System.Drawing.Bitmap)(resources.GetObject("grid_AccoutList.UncheckedImage")));
            this.grid_AccoutList.Click += new FlexCell.Grid.ClickEventHandler(this.grid_AccoutList_Click);
            // 
            // grid_Operator
            // 
            this.grid_Operator.CheckedImage = ((System.Drawing.Bitmap)(resources.GetObject("grid_Operator.CheckedImage")));
            this.grid_Operator.DefaultFont = new System.Drawing.Font("宋体", 9F);
            this.grid_Operator.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grid_Operator.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.grid_Operator.HideGridLines = true;
            this.grid_Operator.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.grid_Operator.Location = new System.Drawing.Point(276, 153);
            this.grid_Operator.Name = "grid_Operator";
            this.grid_Operator.Size = new System.Drawing.Size(304, 16);
            this.grid_Operator.TabIndex = 31;
            this.grid_Operator.UncheckedImage = ((System.Drawing.Bitmap)(resources.GetObject("grid_Operator.UncheckedImage")));
            this.grid_Operator.Click += new FlexCell.Grid.ClickEventHandler(this.grid_Operator_Click);
            // 
            // picbox_ClearRubbish
            // 
            this.picbox_ClearRubbish.ErrorImage = null;
            this.picbox_ClearRubbish.Image = ((System.Drawing.Image)(resources.GetObject("picbox_ClearRubbish.Image")));
            this.picbox_ClearRubbish.InitialImage = null;
            this.picbox_ClearRubbish.Location = new System.Drawing.Point(589, 164);
            this.picbox_ClearRubbish.Name = "picbox_ClearRubbish";
            this.picbox_ClearRubbish.Size = new System.Drawing.Size(58, 22);
            this.picbox_ClearRubbish.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbox_ClearRubbish.TabIndex = 37;
            this.picbox_ClearRubbish.TabStop = false;
            this.picbox_ClearRubbish.Click += new System.EventHandler(this.picbox_ClearRubbish_Click);
            // 
            // pic_Help
            // 
            this.pic_Help.ErrorImage = null;
            this.pic_Help.Image = ((System.Drawing.Image)(resources.GetObject("pic_Help.Image")));
            this.pic_Help.InitialImage = null;
            this.pic_Help.Location = new System.Drawing.Point(589, 128);
            this.pic_Help.Name = "pic_Help";
            this.pic_Help.Size = new System.Drawing.Size(58, 22);
            this.pic_Help.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_Help.TabIndex = 36;
            this.pic_Help.TabStop = false;
            this.pic_Help.Click += new System.EventHandler(this.pic_Help_Click);
            // 
            // pic_Configuration
            // 
            this.pic_Configuration.ErrorImage = null;
            this.pic_Configuration.Image = ((System.Drawing.Image)(resources.GetObject("pic_Configuration.Image")));
            this.pic_Configuration.InitialImage = null;
            this.pic_Configuration.Location = new System.Drawing.Point(589, 92);
            this.pic_Configuration.Name = "pic_Configuration";
            this.pic_Configuration.Size = new System.Drawing.Size(58, 22);
            this.pic_Configuration.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_Configuration.TabIndex = 35;
            this.pic_Configuration.TabStop = false;
            this.pic_Configuration.Click += new System.EventHandler(this.pic_Configuration_Click);
            // 
            // pic_Enter
            // 
            this.pic_Enter.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pic_Enter.ErrorImage")));
            this.pic_Enter.Image = ((System.Drawing.Image)(resources.GetObject("pic_Enter.Image")));
            this.pic_Enter.InitialImage = ((System.Drawing.Image)(resources.GetObject("pic_Enter.InitialImage")));
            this.pic_Enter.Location = new System.Drawing.Point(306, 204);
            this.pic_Enter.Name = "pic_Enter";
            this.pic_Enter.Size = new System.Drawing.Size(65, 32);
            this.pic_Enter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pic_Enter.TabIndex = 20;
            this.pic_Enter.TabStop = false;
            this.pic_Enter.Click += new System.EventHandler(this.pic_Enter_Click);
            // 
            // LoginFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(658, 257);
            this.Controls.Add(this.picbox_ClearRubbish);
            this.Controls.Add(this.pic_Help);
            this.Controls.Add(this.pic_Configuration);
            this.Controls.Add(this.grid_Operator);
            this.Controls.Add(this.grid_AccoutList);
            this.Controls.Add(this.picbox_AccoutList);
            this.Controls.Add(this.picbox_Operator);
            this.Controls.Add(this.txt_AccoutList);
            this.Controls.Add(this.txt_Operator);
            this.Controls.Add(this.pbox_Logo);
            this.Controls.Add(this.pic_Exit);
            this.Controls.Add(this.pic_Enter);
            this.Controls.Add(this.txt_Password);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "欢迎使用云访客门禁网络通信助手V2019";
            this.Load += new System.EventHandler(this.LoginFrm_Load);
            this.Click += new System.EventHandler(this.LoginFrm_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Exit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbox_Logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_Operator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_AccoutList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_ClearRubbish)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Help)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Configuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Enter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pic_Exit;
        private System.Windows.Forms.TextBox txt_Password;
        private System.IO.Ports.SerialPort sP1;
        private System.Windows.Forms.Timer timer_SIdRead;
        private System.Windows.Forms.Timer timer_CardRead;
        private System.Windows.Forms.PictureBox pbox_Logo;
        private System.Windows.Forms.TextBox txt_Operator;
        private System.Windows.Forms.PictureBox picbox_Operator;
        private System.Windows.Forms.TextBox txt_AccoutList;
        private System.Windows.Forms.PictureBox picbox_AccoutList;
        private FlexCell.Grid grid_AccoutList;
        private FlexCell.Grid grid_Operator;
        private System.Windows.Forms.PictureBox picbox_ClearRubbish;
        private System.Windows.Forms.PictureBox pic_Help;
        private System.Windows.Forms.PictureBox pic_Configuration;
        private System.ServiceProcess.ServiceController serviceController1;
        private System.Windows.Forms.PictureBox pic_Enter;

    }
}

