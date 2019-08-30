using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace XXY_VisitorMJAsst
{
    public partial class LoginFrm_Help : Form
    {
        public LoginFrm_Help()
        {
            InitializeComponent();
        }

        private void btn_ConsiceTraining_Click(object sender, EventArgs e)
        {
            try
            {
                string strHelpFN = Application.StartupPath + "//访客机操作说明.doc";
                System.Diagnostics.Process.Start(strHelpFN);
                this.Close();
            }
            catch
            {
                MessageBox.Show("对不起，找不到相应的帮助文件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btn_DetailedMaster_Click(object sender, EventArgs e)
        {
            try
            {
                string strHelpFN = Application.StartupPath + "//Visitor.CHM";
                System.Diagnostics.Process.Start(strHelpFN);
                this.Close();
            }
            catch
            {
                MessageBox.Show("对不起，找不到相应的帮助文件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btn_CopyAllToDeskTop_Click(object sender, EventArgs e)
        {
            try
            {
                string strDeskTopPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                int iCount = 0, i1 = 0, i2 = 0;
                if (File.Exists(Application.StartupPath + "//访客机操作说明.doc") == true)
                {
                    iCount++;
                    i1 = 1;
                    File.Copy(Application.StartupPath + "//访客机操作说明.doc", strDeskTopPath + "//访客机操作说明.doc");
                }
                if (File.Exists(Application.StartupPath + "//Visitor.CHM") == true)
                {
                    iCount++;
                    i2 = 1;
                    File.Copy(Application.StartupPath + "//Visitor.CHM", strDeskTopPath + "//Visitor.CHM");
                }
                if (iCount == 0)
                {
                    MessageBox.Show("复制失败，请确认软件根目录下是否存在相关帮助文件或直接向供应商获取!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    string strSQL = "成功复制 " + iCount.ToString() + " 个文件，分别为:\n\n";
                    if (i1 == 1)
                    {
                        strSQL += "1.访客机操作说明.doc";
                        if (i2 == 1)
                        {
                            strSQL += "\n\n2.Visitor.CHM";
                        }
                    }
                    else
                    {
                        if (i2 == 1)
                        {
                            strSQL += "\n\n1.Visitor.CHM";
                        }
                    }
                    MessageBox.Show(strSQL, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

 
    }
}
