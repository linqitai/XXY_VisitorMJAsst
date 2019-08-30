using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XXY_VisitorMJAsst._3SystemMaintenance
{
    public partial class D_OperatorLogFrm_LogSearch : Form
    {
        public D_OperatorLogFrm_LogSearch()
        {
            InitializeComponent();
        }
        private string strSQL = "";
        private DataTable myTable = new DataTable();
        private SQLHelper  SQLHelper = new SQLHelper ();
        private string strCSRQ = "1900/01/01";//初始日期
        public string strRZXX_CXTJ = "";

        private void D_OperatorLogFrm_LogSearch_Load(object sender, EventArgs e)
        {
            clsIme.SetIme(this);//控制输入法的状态
            strSQL = " select distinct FormName FormName  from  T_OperateLogInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)//取操作日志中的最大操作时间,防止操作员通过修改系统时间实现对日志的删除
            {
                this.cmbox_FormName.Items.Add("");
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    this.cmbox_FormName.Items.Add(myTable.Rows[i]["FormName"].ToString().Trim());
                }
            }
            strSQL = " select distinct OperatorName OperatorName  from  T_OperateLogInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)//取操作日志中的最大操作时间,防止操作员通过修改系统时间实现对日志的删除
            {
                this.cmbox_FormName.Items.Add("");
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    this.cmbox_OperatorName.Items.Add(myTable.Rows[i]["OperatorName"].ToString().Trim());
                }
            }
            this.dtp_OperateDTStart.Value = Convert.ToDateTime(D_OperatorLogFrm.strDt_Start);
            this.dtp_OperateDTEnd.Value = Convert.ToDateTime(D_OperatorLogFrm.strDt_End);
            this.rbtn_Blur.Checked = true;
        }

        private void rbtn_Blur_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbtn_Blur.Checked == true)
            {
                this.rbtn_Blur.BackColor = Color.FromArgb(225, 128, 225);
                this.rbtn_Strict.Checked = false;
            }
            else
            {
                this.rbtn_Strict.Checked = true;
                this.rbtn_Blur.BackColor = Color.FromArgb(225, 225, 225);
            }
        }

        private void rbtn_Strict_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbtn_Strict.Checked == true)
            {
                this.rbtn_Strict.BackColor = Color.FromArgb(225, 128, 225);
                this.rbtn_Blur.Checked = false;
            }
            else
            {
                this.rbtn_Blur.Checked = true;
                this.rbtn_Strict.BackColor = Color.FromArgb(225, 225, 225);
            }
        }

        private void TSB_Search_Click(object sender, EventArgs e)
        {
            string strRowFilter = "";
            string strResult = "";
            if (this.cmbox_Result.Text.Trim() != "")
            {
                this.ckbox_Result.Checked = true;
                if (this.cmbox_Result.Text == "成功")
                {
                    strResult = "1";
                }
                else if (this.cmbox_Result.Text == "失败")
                {
                    strResult = "0";
                }
            }
            else
            {
                this.ckbox_Result.Checked = false;
            }
            if (this.dtp_OperateDTStart.Value != Convert.ToDateTime(strCSRQ))
            {
                this.ckbox_OperateDTStart.Checked = true;
            }
            else
            {
                this.ckbox_OperateDTStart.Checked = false;
            }
            if (this.dtp_OperateDTEnd.Value != Convert.ToDateTime(strCSRQ))
            {
                this.ckbox_OperateDTEnd.Checked = true;
            }
            else
            {
                this.ckbox_OperateDTEnd.Checked = false;
            }
            if (this.rbtn_Blur.Checked == true)
            {
                #region//模糊查询
                if (this.ckbox_OperatorName.Checked == true)
                {
                    strRowFilter += "T_OperateLogInf.OperatorName LIKE  '%" + this.cmbox_OperatorName.Text.ToString().Trim() + "%' " + " and ";
                }
                if (this.ckbox_Result.Checked == true)
                {
                    strRowFilter += "T_OperateLogInf.Result LIKE  '%" + strResult + "%' " + " and ";
                }
                if (this.ckbox_FormName.Checked == true)
                {
                    strRowFilter += "T_OperateLogInf.FormName LIKE  '%" + this.cmbox_FormName.Text.ToString().Trim() + "%' " + " and ";
                }
                if (this.ckbox_OperateDTStart.Checked == true || this.ckbox_OperateDTEnd.Checked == true)
                {
                    strRowFilter += "T_OperateLogInf.OperateDT between  '" + this.dtp_OperateDTStart.Value.ToString() + "'  and  '" + this.dtp_OperateDTEnd.Value.ToString() + "' and ";

                }
                #endregion
            }
            else
            {
                #region//严格查询
                if (this.ckbox_OperatorName.Checked == true)
                {
                    strRowFilter += "T_OperateLogInf.OperatorName =  '" + this.cmbox_OperatorName.Text.ToString().Trim() + "' " + " and ";
                }
                if (this.ckbox_Result.Checked == true)
                {
                    strRowFilter += "T_OperateLogInf.Result =  '" + strResult + "' " + " and ";
                }
                if (this.ckbox_FormName.Checked == true)
                {
                    strRowFilter += "T_OperateLogInf.FormName =  '" + this.cmbox_FormName.Text.ToString().Trim() + "' " + " and ";
                }
                if (this.ckbox_OperateDTStart.Checked == true || this.ckbox_OperateDTEnd.Checked == true)
                {
                    strRowFilter += "T_OperateLogInf.OperateDT between  '" + this.dtp_OperateDTStart.Value.ToString() + "'  and  '" + this.dtp_OperateDTEnd.Value.ToString() + "' and ";

                }
                #endregion
            }
            if (strRowFilter != "")//传回条件就行了，查询字段直接调用父窗体预先保留的
            {
                D_OperatorLogFrm.strDt_Start = this.dtp_OperateDTStart.Value.ToString();
                D_OperatorLogFrm.strDt_End = Convert.ToString(Convert.ToDateTime(this.dtp_OperateDTEnd.Value));//求出每月最后一天
                strRowFilter = strRowFilter.Substring(0, strRowFilter.ToString().Length - 5);
                strRZXX_CXTJ = strRowFilter + "   ";//加上空格，防止在组成查找条件时出错.
                this.Close();
            }
            else
            {
                MessageBox.Show("请输入查询条件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void TSB_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region//按键响应
        private void cmbox_OperatorName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbox_OperatorName.Text.Trim() != "")
            {
                this.ckbox_OperatorName.Checked = true;
            }
            else
            {
                this.ckbox_OperatorName.Checked = false;
            }
        }
 

        private void cmbox_Result_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbox_Result.Text.Trim() != "")
            {
                this.ckbox_Result.Checked = true;
            }
            else
            {
                this.ckbox_Result.Checked = false;
            }
        }

        private void cmb_FormName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbox_FormName.Text.Trim() != "")
            {
                this.ckbox_FormName.Checked = true;
            }
            else
            {
                this.ckbox_FormName.Checked = false;
            }
        }

        private void dtp_OperateDTStart_ValueChanged(object sender, EventArgs e)
        {
            if (dtp_OperateDTStart.Value != Convert.ToDateTime(strCSRQ))
            {
                this.ckbox_OperateDTStart.Checked = true;
            }
            else
            {
                this.ckbox_OperateDTStart.Checked = false;
            }
        }

        private void dtp_OperateDTStart_Leave(object sender, EventArgs e)
        {
            if (dtp_OperateDTStart.Value != Convert.ToDateTime(strCSRQ))
            {
                this.ckbox_OperateDTStart.Checked = true;
            }
            else
            {
                this.ckbox_OperateDTStart.Checked = false;
            }
        }

        private void dtp_OperateDTEnd_ValueChanged(object sender, EventArgs e)
        {
            if (dtp_OperateDTEnd.Value != Convert.ToDateTime(strCSRQ))
            {
                this.ckbox_OperateDTEnd.Checked = true;
            }
            else
            {
                this.ckbox_OperateDTEnd.Checked = false;
            }
        }

        private void dtp_OperateDTEnd_Leave(object sender, EventArgs e)
        {
            if (dtp_OperateDTEnd.Value != Convert.ToDateTime(strCSRQ))
            {
                this.ckbox_OperateDTEnd.Checked = true;
            }
            else
            {
                this.ckbox_OperateDTEnd.Checked = false;
            }
        }
        #endregion

     



    }
}
