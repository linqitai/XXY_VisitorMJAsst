using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XXY_VisitorMJAsst
{
    public partial class D_RemoterControlFrm_SysSetting : Form
    {
        public D_RemoterControlFrm_SysSetting()
        {
            InitializeComponent();
        }
        private string strSQL = "";
        private DataTable myTable;
        private SQLHelper SQLHelper = new SQLHelper();
        private string[] ASetings = new string[500];

        #region//操作日志
        private void OperatorLog(string para_strOperateDescribe, string para_strResult)
        {
            try
            {
                string strOperatorLog = "insert into " + LoginFrm.strT_OperateLogInf + " (OperatorNo,OperatorActualNo,OperatorName,";
                strOperatorLog += " FormName,OperateDescribe,Result,OperateDT,Flag,CAccoutNo)values('" + LoginFrm.strOperatorNo + "',";
                strOperatorLog += "'" + LoginFrm.strOperatorActualNo + "','" + LoginFrm.strOperatorName + "','" + "访客门禁通信助手系统设置" + "',";
                strOperatorLog += "'" + para_strOperateDescribe + "','" + para_strResult + "','" + System.DateTime.Now.ToString() + "','" + "访客系统" + "',";
                strOperatorLog += "'" + LoginFrm.strCAccout.Substring(1, 4).Trim() + "')";
                SQLHelper.ExecuteSql(strOperatorLog);
            }
            catch (Exception exp)
            {
                if (exp.ToString().Trim().Contains("远程主机") == true)
                {
                    MessageBox.Show("本机与远程数据库服务器失去连接，系统将自动退出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region//复选框样式
        private void ckbox_AllowedAllLeaveNormal_CheckedChanged(object sender, EventArgs e)
        {

            if (this.ckbox_AllowedAllLeaveNormal.Checked == true)
            {

                this.nud_AllowedAllLeaveNormal_Hour.Enabled = true;
                this.nud_AllowedAllLeaveNormal_Minute.Enabled = true;

                this.lbl_AllowedAllLeaveNormal_Hour.BackColor = Color.FromArgb(225, 128, 225);
                this.lbl_AllowedAllLeaveNormal_Minute.BackColor = Color.FromArgb(225, 128, 225);
                this.ckbox_AllowedAllLeaveNormal.BackColor = Color.FromArgb(225, 128, 225);

                this.ckbox_AllowedAllLeaveAbNormal.Checked = false;
            }
            else
            {
                this.nud_AllowedAllLeaveNormal_Hour.Enabled = false;
                this.nud_AllowedAllLeaveNormal_Minute.Enabled = false;

                this.lbl_AllowedAllLeaveNormal_Hour.BackColor = Color.FromArgb(225, 225, 225);
                this.lbl_AllowedAllLeaveNormal_Minute.BackColor = Color.FromArgb(225, 225, 225);
                this.ckbox_AllowedAllLeaveNormal.BackColor = Color.FromArgb(225, 225, 225);
            }
        }

        private void ckbox_AllowedAllLeaveAbNormal_CheckedChanged(object sender, EventArgs e)
        {

            if (this.ckbox_AllowedAllLeaveAbNormal.Checked == true)
            {
                this.nud_AllowedAllLeaveAbNormal_Hour.Enabled = true;
                this.nud_AllowedAllLeaveAbNormal_Minute.Enabled = true;

                this.lbl_AllowedAllLeaveAbNormal_Hour.BackColor = Color.FromArgb(225, 128, 225);
                this.lbl_AllowedAllLeaveAbNormal_Minute.BackColor = Color.FromArgb(225, 128, 225);
                this.ckbox_AllowedAllLeaveAbNormal.BackColor = Color.FromArgb(225, 128, 225);

                this.ckbox_AllowedAllLeaveNormal.Checked = false;
            }
            else
            {
                this.nud_AllowedAllLeaveAbNormal_Hour.Enabled = false;
                this.nud_AllowedAllLeaveAbNormal_Minute.Enabled = false;

                this.lbl_AllowedAllLeaveAbNormal_Hour.BackColor = Color.FromArgb(225, 225, 225);
                this.lbl_AllowedAllLeaveAbNormal_Minute.BackColor = Color.FromArgb(225, 225, 225);
                this.ckbox_AllowedAllLeaveAbNormal.BackColor = Color.FromArgb(225, 225, 225);
            }
        }

        private void ckbox_AllowedLogoutRedCardNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbox_AllowedLogoutRedCardNormal.Checked == true)
            {
                this.nud_AllowedLogoutRedCardNormal_Hour.Enabled = true;
                this.nud_AllowedLogoutRedCardNormal_Minute.Enabled = true;

                this.lbl_AllowedLogoutRedCardNormal_Hour.BackColor = Color.FromArgb(225, 128, 225);
                this.lbl_AllowedLogoutRedCardNormal_Minute.BackColor = Color.FromArgb(225, 128, 225);
                this.ckbox_AllowedLogoutRedCardNormal.BackColor = Color.FromArgb(225, 128, 225);

                this.ckbox_AllowedLogoutRedCardAbNormal.Checked = false;
            }
            else
            {
                this.nud_AllowedLogoutRedCardNormal_Hour.Enabled = false;
                this.nud_AllowedLogoutRedCardNormal_Minute.Enabled = false;

                this.lbl_AllowedLogoutRedCardNormal_Hour.BackColor = Color.FromArgb(225, 225, 225);
                this.lbl_AllowedLogoutRedCardNormal_Minute.BackColor = Color.FromArgb(225, 225, 225);
                this.ckbox_AllowedLogoutRedCardNormal.BackColor = Color.FromArgb(225, 225, 225);
            }
        }


        private void ckbox_AllowedLogoutRedCardAbNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbox_AllowedLogoutRedCardAbNormal.Checked == true)
            {
                this.nud_AllowedLogoutRedCardAbNormal_Hour.Enabled = true;
                this.nud_AllowedLogoutRedCardAbNormal_Minute.Enabled = true;

                this.lbl_AllowedLogoutRedCardAbNormal_Hour.BackColor = Color.FromArgb(225, 128, 225);
                this.lbl_AllowedLogoutRedCardAbNormal_Minute.BackColor = Color.FromArgb(225, 128, 225);
                this.ckbox_AllowedLogoutRedCardAbNormal.BackColor = Color.FromArgb(225, 128, 225);

                this.ckbox_AllowedLogoutRedCardNormal.Checked = false;
            }
            else
            {
                this.nud_AllowedLogoutRedCardAbNormal_Hour.Enabled = false;
                this.nud_AllowedLogoutRedCardAbNormal_Minute.Enabled = false;

                this.lbl_AllowedLogoutRedCardAbNormal_Hour.BackColor = Color.FromArgb(225, 225, 225);
                this.lbl_AllowedLogoutRedCardAbNormal_Minute.BackColor = Color.FromArgb(225, 225, 225);
                this.ckbox_AllowedLogoutRedCardAbNormal.BackColor = Color.FromArgb(225, 225, 225);
            }
        }
 

        private void ckbox_AllowedLogoutBlueCard_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbox_AllowedLogoutBlueCard.Checked == true)
            {
                this.nud_AllowedLogoutBlueCard_Hour.Enabled = true;
                this.nud_AllowedLogoutBlueCard_Minute.Enabled = true;

                this.lbl_AllowedLogoutBlueCard_Hour.BackColor = Color.FromArgb(225, 128, 225);
                this.lbl_AllowedLogoutBlueCard_Minute.BackColor = Color.FromArgb(225, 128, 225);
                this.ckbox_AllowedLogoutBlueCard.BackColor = Color.FromArgb(225, 128, 225);
            }
            else
            {
                this.nud_AllowedLogoutBlueCard_Hour.Enabled = false;
                this.nud_AllowedLogoutBlueCard_Minute.Enabled = false;

                this.lbl_AllowedLogoutBlueCard_Hour.BackColor = Color.FromArgb(225, 225, 225);
                this.lbl_AllowedLogoutBlueCard_Minute.BackColor = Color.FromArgb(225, 225, 225);
                this.ckbox_AllowedLogoutBlueCard.BackColor = Color.FromArgb(225, 225, 225);
            }
        }

        private void ckbox_AllowedOverdueOrder_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbox_AllowedOverdueOrder.Checked == true)
            {
                this.nud_AllowedOverdueOrder_Hour.Enabled = true;
                this.nud_AllowedOverdueOrder_Minute.Enabled = true;

                this.lbl_AllowedOverdueOrder_Hour.BackColor = Color.FromArgb(225, 128, 225);
                this.lbl_AllowedOverdueOrder_Minute.BackColor = Color.FromArgb(225, 128, 225);
                this.ckbox_AllowedOverdueOrder.BackColor = Color.FromArgb(225, 128, 225);
            }
            else
            {
                this.nud_AllowedOverdueOrder_Hour.Enabled = false;
                this.nud_AllowedOverdueOrder_Minute.Enabled = false;

                this.lbl_AllowedOverdueOrder_Hour.BackColor = Color.FromArgb(225, 225, 225);
                this.lbl_AllowedOverdueOrder_Minute.BackColor = Color.FromArgb(225, 225, 225);
                this.ckbox_AllowedOverdueOrder.BackColor = Color.FromArgb(225, 225, 225);
            }
        }
        private void ckbox_OpenDoorDelay_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbox_AllowedOpenDoorDelay.Checked == true)
            {
                this.nud_AllowedOpenDorrDelay_Seconds.Enabled = true;

                this.lbl_OpenDoorDelay_Seconds.BackColor = Color.FromArgb(225, 128, 225);

                this.ckbox_AllowedOpenDoorDelay.BackColor = Color.FromArgb(225, 128, 225);
            }
            else
            {
                MessageBox.Show("此功能需强制性开启!","提示",MessageBoxButtons.OK ,MessageBoxIcon.Exclamation);
                this.ckbox_AllowedOpenDoorDelay.Checked = true;
                this.nud_AllowedOpenDorrDelay_Seconds.Enabled = true;
                this.lbl_OpenDoorDelay_Seconds.BackColor = Color.FromArgb(225, 128, 225);
                this.ckbox_AllowedOpenDoorDelay.BackColor = Color.FromArgb(225, 128, 225);
                //this.nud_AllowedOpenDorrDelay_Seconds.Enabled = false;

                //this.lbl_OpenDorrDelay_Seconds.BackColor = Color.FromArgb(225, 225, 225);
                //this.ckbox_AllowedOpenDoorDelay.BackColor = Color.FromArgb(225, 225, 225);
            }
        }

        private void ckbox_AllowedEnterAndLeaveCount_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbox_AllowedEnterAndLeaveCount.Checked == true)
            {
                this.nud_AllowedEnterAndLeaveCount_Enter.Enabled = true;
                this.nud_AllowedEnterAndLeaveCount_Leave.Enabled = true;

                this.lbl_AllowedEnterAndLeaveCount_Enter.BackColor = Color.FromArgb(225, 128, 225);
                this.lbl_AllowedEnterAndLeaveCount_Leave.BackColor = Color.FromArgb(225, 128, 225);
                this.ckbox_AllowedEnterAndLeaveCount.BackColor = Color.FromArgb(225, 128, 225);
            }
            else
            {
                MessageBox.Show("此功能需强制性开启!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ckbox_AllowedEnterAndLeaveCount.Checked = true;
                this.nud_AllowedEnterAndLeaveCount_Enter.Enabled = true;
                this.nud_AllowedEnterAndLeaveCount_Leave.Enabled = true;

                this.lbl_AllowedEnterAndLeaveCount_Enter.BackColor = Color.FromArgb(225, 128, 225);
                this.lbl_AllowedEnterAndLeaveCount_Leave.BackColor = Color.FromArgb(225, 128, 225);
                this.ckbox_AllowedEnterAndLeaveCount.BackColor = Color.FromArgb(225, 128, 225);
                //this.nud_AllowedEnterAndLeaveCount_Enter.Enabled = false;
                //this.nud_AllowedEnterAndLeaveCount_Leave.Enabled = false;

                //this.lbl_AllowedEnterAndLeaveCount_Enter.BackColor = Color.FromArgb(225, 225, 225);
                //this.lbl_AllowedEnterAndLeaveCount_Leave.BackColor = Color.FromArgb(225, 225, 225);
                //this.ckbox_AllowedEnterAndLeaveCount.BackColor = Color.FromArgb(225, 225, 225);
            }
        }

        private void ckbox_AllowedOpenMSCat_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbox_AllowedOpenMSCat.Checked == true)
            {
                this.ckbox_AllowedOpenMSCat.BackColor = Color.FromArgb(225, 128, 225);
            }
            else
            {
                this.ckbox_AllowedOpenMSCat.BackColor = Color.FromArgb(225, 225, 225);
            }
        }



        #endregion

        private void D_RemoterControlFrm_SysSetting_Load(object sender, EventArgs e)
        {
            clsIme.SetIme(this);//控制输入法的状态
            for (int i = 0; i < ASetings.Length; i++)
            {
                ASetings[i] = "0";
            }
            strSQL = "select * from XXCLOUD.DBO.T_CloudVisitorRegSettingInf    ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                #region
                // string[] ASetings = myTable.Rows[0]["ASetings"].ToString().Trim().Split(',');//以","分离字符串
                int iCount = 0;
                strSQL = "";
                for (int i = 0; i < myTable.Rows[0]["MJASetings"].ToString().Trim().Length; i++)
                {
                    if (myTable.Rows[0]["MJASetings"].ToString().Trim().Substring(i, 1).ToString() != ",")
                    {
                        strSQL += myTable.Rows[0]["MJASetings"].ToString().Trim().Substring(i, 1).ToString().Trim();
                    }
                    else
                    {
                        ASetings[iCount++] = strSQL;
                        strSQL = "";
                    }
                }
                if (ASetings[0].ToString().Trim() == "1")//每天自动定时签离全部未签离来访记录并记为正常签离,时间为
                {
                    this.ckbox_AllowedAllLeaveNormal.Checked = true;
                    this.nud_AllowedAllLeaveNormal_Hour.Enabled = true;
                    this.nud_AllowedAllLeaveNormal_Minute.Enabled = true;
                    try
                    {
                        this.nud_AllowedAllLeaveNormal_Hour.Value = Convert.ToInt32(ASetings[1].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedAllLeaveNormal_Hour.Value = 0;
                    }
                    try
                    {
                        this.nud_AllowedAllLeaveNormal_Minute.Value = Convert.ToInt32(ASetings[2].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedAllLeaveNormal_Minute.Value = 0;
                    }
                }
                else
                {
                    this.ckbox_AllowedAllLeaveNormal.Checked = false;
                    this.nud_AllowedAllLeaveNormal_Hour.Enabled = false;
                    this.nud_AllowedAllLeaveNormal_Minute.Enabled = false;
                }
                if (ASetings[3].ToString().Trim() == "1")//每天自动定时签离全部未签离来访记录并记为1次非正常签离,时间为
                {
                    this.ckbox_AllowedAllLeaveAbNormal.Checked = true;
                    this.nud_AllowedAllLeaveAbNormal_Hour.Enabled = true;
                    this.nud_AllowedAllLeaveAbNormal_Minute.Enabled = true;
                    try
                    {
                        this.nud_AllowedAllLeaveAbNormal_Hour.Value = Convert.ToInt32(ASetings[4].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedAllLeaveAbNormal_Hour.Value = 0;
                    }
                    try
                    {
                        this.nud_AllowedAllLeaveAbNormal_Minute.Value = Convert.ToInt32(ASetings[5].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedAllLeaveAbNormal_Minute.Value = 0;
                    }
                }
                else
                {
                    this.ckbox_AllowedAllLeaveAbNormal.Checked = false;
                    this.nud_AllowedAllLeaveAbNormal_Hour.Enabled = false;
                    this.nud_AllowedAllLeaveAbNormal_Minute.Enabled = false;
                }

                if (ASetings[6].ToString().Trim() == "1")//每天自动定时从门禁控制器上注销所有未注销的红卡权限并记为正常注销,时间为 
                {
                    this.ckbox_AllowedLogoutRedCardNormal.Checked = true;
                    this.nud_AllowedLogoutRedCardNormal_Hour.Enabled = true;
                    this.nud_AllowedLogoutRedCardNormal_Minute.Enabled = true;
                    try
                    {
                        this.nud_AllowedLogoutRedCardNormal_Hour.Value = Convert.ToInt32(ASetings[7].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedLogoutRedCardNormal_Hour.Value = 0;
                    }
                    try
                    {
                        this.nud_AllowedLogoutRedCardNormal_Minute.Value = Convert.ToInt32(ASetings[8].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedLogoutRedCardNormal_Minute.Value = 0;
                    }
                }
                else
                {
                    this.ckbox_AllowedLogoutRedCardNormal.Checked = false;
                    this.nud_AllowedLogoutRedCardNormal_Hour.Enabled = false;
                    this.nud_AllowedLogoutRedCardNormal_Minute.Enabled = false;
                }

                if (ASetings[9].ToString().Trim() == "1")//每天自动定时从门禁控制器上注销所有未注销的红卡权限并记为1次非正常注销,时间为
                {
                    this.ckbox_AllowedLogoutRedCardAbNormal.Checked = true;
                    this.nud_AllowedLogoutRedCardAbNormal_Hour.Enabled = true;
                    this.nud_AllowedLogoutRedCardAbNormal_Minute.Enabled = true;
                    try
                    {
                        this.nud_AllowedLogoutRedCardAbNormal_Hour.Value = Convert.ToInt32(ASetings[10].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedLogoutRedCardAbNormal_Hour.Value = 0;
                    }
                    try
                    {
                        this.nud_AllowedLogoutRedCardAbNormal_Minute.Value = Convert.ToInt32(ASetings[11].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedLogoutRedCardAbNormal_Minute.Value = 0;
                    }
                }
                else
                {
                    this.ckbox_AllowedLogoutRedCardAbNormal.Checked = false;
                    this.nud_AllowedLogoutRedCardAbNormal_Hour.Enabled = false;
                    this.nud_AllowedLogoutRedCardAbNormal_Minute.Enabled = false;
                }

                if (ASetings[12].ToString().Trim() == "1")//每天自动定时根据蓝卡有效期限从门禁控制器上注销所有未注销的蓝卡权限,时间为
                {
                    this.ckbox_AllowedLogoutBlueCard.Checked = true;
                    this.nud_AllowedLogoutBlueCard_Hour.Enabled = true;
                    this.nud_AllowedLogoutBlueCard_Minute.Enabled = true;
                    try
                    {
                        this.nud_AllowedLogoutBlueCard_Hour.Value = Convert.ToInt32(ASetings[13].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedLogoutBlueCard_Hour.Value = 0;
                    }
                    try
                    {
                        this.nud_AllowedLogoutBlueCard_Minute.Value = Convert.ToInt32(ASetings[14].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedLogoutBlueCard_Minute.Value = 0;
                    }
                }
                else
                {
                    this.ckbox_AllowedLogoutBlueCard.Checked = false;
                    this.nud_AllowedLogoutBlueCard_Hour.Enabled = false;
                    this.nud_AllowedLogoutBlueCard_Minute.Enabled = false;
                }
                if (ASetings[15].ToString().Trim() == "1")//每天自动定时根据预约有效期限把过期预约设为异常预约,时间为
                {
                    this.ckbox_AllowedOverdueOrder.Checked = true;
                    this.nud_AllowedOverdueOrder_Hour.Enabled = true;
                    this.nud_AllowedOverdueOrder_Minute.Enabled = true;
                    try
                    {
                        this.nud_AllowedOverdueOrder_Hour.Value = Convert.ToInt32(ASetings[16].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedOverdueOrder_Hour.Value = 0;
                    }
                    try
                    {
                        this.nud_AllowedOverdueOrder_Minute.Value = Convert.ToInt32(ASetings[17].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedOverdueOrder_Minute.Value = 0;
                    }
                }
                else
                {
                    this.ckbox_AllowedLogoutBlueCard.Checked = false;
                    this.nud_AllowedLogoutBlueCard_Hour.Enabled = false;
                    this.nud_AllowedLogoutBlueCard_Minute.Enabled = false;
                }
                if (ASetings[18].ToString().Trim() == "1")//合法门禁卡刷卡后,闸机在X秒内检测到未有人通过时,则自动关闭
                {
                    this.ckbox_AllowedOpenDoorDelay.Checked = true;
                    this.nud_AllowedOpenDorrDelay_Seconds.Enabled = true;
               
                    try
                    {
                        this.nud_AllowedOpenDorrDelay_Seconds.Value = Convert.ToInt32(ASetings[19].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedOpenDorrDelay_Seconds.Value = 3;
                    }
                
                }
                else
                {
                    this.ckbox_AllowedOpenDoorDelay.Checked = false ;
                    this.nud_AllowedOpenDorrDelay_Seconds.Enabled = false ;
                }
                if (ASetings[20].ToString().Trim() == "1")//合法红卡在有效期内只允许在所有的闸机上X进X出
                {
                    this.ckbox_AllowedEnterAndLeaveCount.Checked = true;
                    this.nud_AllowedEnterAndLeaveCount_Enter.Enabled = true;
                    this.nud_AllowedEnterAndLeaveCount_Leave.Enabled = true;
                    try
                    {
                        this.nud_AllowedEnterAndLeaveCount_Enter.Value = Convert.ToInt32(ASetings[21].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedEnterAndLeaveCount_Enter.Value = 1;
                    }
                    try
                    {
                        this.nud_AllowedEnterAndLeaveCount_Leave.Value = Convert.ToInt32(ASetings[22].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedEnterAndLeaveCount_Leave.Value = 1;
                    }
                }
                else
                {
                    this.ckbox_AllowedEnterAndLeaveCount.Checked = false;
                    this.nud_AllowedEnterAndLeaveCount_Enter.Enabled = false;
                    this.nud_AllowedEnterAndLeaveCount_Leave.Enabled = false;
                }

                if (ASetings[23].ToString().Trim() == "1")//自助登记时，允许开启手机短信验证功能
                {
                    this.ckbox_AllowedOpenMSCat.Checked = true;
                }
                else
                {
                    this.ckbox_AllowedOpenMSCat.Checked = false ;
                }
                if (ASetings[24].ToString().Trim() == "1")//贵宾(VIP)门禁卡刷卡后,开门时长为*秒
                {
                    this.ckbox_AllowedVIPOpenDoorDelay.Checked = true;
                    this.nud_AllowedVIPOpenDorrDelay_Seconds.Enabled = true;

                    try
                    {
                        this.nud_AllowedVIPOpenDorrDelay_Seconds.Value = Convert.ToInt32(ASetings[25].ToString().Trim());
                    }
                    catch
                    {
                        this.nud_AllowedVIPOpenDorrDelay_Seconds.Value = 60;
                    }

                }
                else
                {
                    this.ckbox_AllowedVIPOpenDoorDelay.Checked = false;
                    this.nud_AllowedVIPOpenDorrDelay_Seconds.Enabled = false;
                }
                #endregion
            }
            else
            {
                #region//默认
                this.ckbox_AllowedAllLeaveNormal.Checked = false ;
                this.nud_AllowedAllLeaveNormal_Hour.Value = 0;
                this.nud_AllowedAllLeaveNormal_Minute.Value = 0;

                this.ckbox_AllowedAllLeaveAbNormal.Checked = true;
                this.nud_AllowedAllLeaveAbNormal_Hour.Value = 0;
                this.nud_AllowedAllLeaveAbNormal_Minute.Value = 0;

                this.ckbox_AllowedLogoutRedCardNormal.Checked = false ;
                this.nud_AllowedLogoutRedCardNormal_Hour.Value = 0;
                this.nud_AllowedLogoutRedCardNormal_Minute.Value = 0;

                this.ckbox_AllowedLogoutRedCardAbNormal.Checked = true ;
                this.nud_AllowedLogoutRedCardAbNormal_Hour.Value = 0;
                this.nud_AllowedLogoutRedCardAbNormal_Minute.Value = 0;

                this.ckbox_AllowedLogoutBlueCard.Checked = true;
                this.nud_AllowedLogoutBlueCard_Hour.Value = 0;
                this.nud_AllowedLogoutBlueCard_Minute.Value = 0;

                this.ckbox_AllowedOverdueOrder.Checked = true;
                this.nud_AllowedOverdueOrder_Hour.Value = 0;
                this.nud_AllowedOverdueOrder_Minute.Value = 0;

                this.ckbox_AllowedOpenDoorDelay.Checked = true;
                this.nud_AllowedOpenDorrDelay_Seconds.Value = 3;


                this.ckbox_AllowedEnterAndLeaveCount.Checked = true;
                this.nud_AllowedEnterAndLeaveCount_Enter.Value = 1;
                this.nud_AllowedEnterAndLeaveCount_Leave.Value = 1;

                this.ckbox_AllowedOpenMSCat.Checked = true;
                #endregion
            }
        }

        private void TSB_Save_Click(object sender, EventArgs e)
        {
            this.ckbox_AllowedAllLeaveAbNormal.Checked = false;
            this.ckbox_AllowedLogoutBlueCard.Checked = false;
            this.ckbox_AllowedLogoutRedCardAbNormal.Checked = false;
            this.ckbox_AllowedLogoutBlueCard.Checked = false;
            string strFlag = "";
            if (this.ckbox_AllowedAllLeaveNormal.Checked == true)
            {
                strFlag = "1," + this.nud_AllowedAllLeaveNormal_Hour.Value.ToString() + "," + this.nud_AllowedAllLeaveNormal_Minute.Value.ToString() + ",";
            }
            else
            {
                strFlag = "0,0,0,";
            }

            if (this.ckbox_AllowedAllLeaveAbNormal.Checked == true)
            {
                strFlag += "1," + this.nud_AllowedAllLeaveAbNormal_Hour.Value.ToString() + "," + this.nud_AllowedAllLeaveAbNormal_Minute.Value.ToString() + ",";
            }
            else
            {
                strFlag += "0,0,0,";
            }

            if (this.ckbox_AllowedLogoutRedCardNormal.Checked == true)
            {
                strFlag += "1," + this.nud_AllowedLogoutRedCardNormal_Hour.Value.ToString() + "," + this.nud_AllowedLogoutRedCardNormal_Minute.Value.ToString() + ",";
            }
            else
            {
                strFlag += "0,0,0,";
            }
            if (this.ckbox_AllowedLogoutRedCardAbNormal.Checked == true)
            {
                strFlag += "1," + this.nud_AllowedLogoutRedCardAbNormal_Hour.Value.ToString() + "," + this.nud_AllowedLogoutRedCardAbNormal_Minute.Value.ToString() + ",";
            }
            else
            {
                strFlag += "0,0,0,";
            }
            if (this.ckbox_AllowedLogoutBlueCard.Checked == true)
            {
                strFlag += "1," + this.nud_AllowedLogoutBlueCard_Hour.Value.ToString() + "," + this.nud_AllowedLogoutBlueCard_Minute.Value.ToString() + ",";
            }
            else
            {
                strFlag += "0,0,0,";
            }
            if (this.ckbox_AllowedOverdueOrder.Checked == true)
            {
                strFlag += "1," + this.nud_AllowedOverdueOrder_Hour.Value.ToString() + "," + this.nud_AllowedOverdueOrder_Minute.Value.ToString() + ",";
            }
            else
            {
                strFlag += "0,0,0,";
            }
            if (this.ckbox_AllowedOpenDoorDelay.Checked == true)
            {
                strFlag += "1," + this.nud_AllowedOpenDorrDelay_Seconds.Value.ToString() + ",";
            }
            else
            {
                strFlag += "0,3,";
            }
            if (this.ckbox_AllowedEnterAndLeaveCount.Checked == true)
            {
                strFlag += "1," + this.nud_AllowedEnterAndLeaveCount_Enter.Value.ToString() + "," + this.nud_AllowedEnterAndLeaveCount_Leave.Value.ToString() + ",";
            }
            else
            {
                strFlag += "0,1,1,";
            }
            if (this.ckbox_AllowedOpenMSCat.Checked == true)
            {
                strFlag += "1,";
            }
            else
            {
                strFlag += "0,";
            }
            if (this.ckbox_AllowedVIPOpenDoorDelay.Checked == true)
            {
                strFlag += "1," + this.nud_AllowedVIPOpenDorrDelay_Seconds.Value.ToString() + ",";
            }
            else
            {
                strFlag += "0,60,";
            }
            strFlag += "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,";//额外加上这些功能，便于后期功能扩展
            strSQL = "update XXCLOUD.DBO.T_CloudVisitorRegSettingInf  set MJASetings ='" + strFlag + "' ";
            if (SQLHelper.ExecuteSql(strSQL) != 0)
            {
                OperatorLog("系统设置", "1");
                MessageBox.Show("系统设置保存成功，重启软件后才能生效!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                OperatorLog("系统设置", "0");
                MessageBox.Show("系统设置保存出错,请重新设置!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void TSB_Default_Click(object sender, EventArgs e)
        {
            strSQL = "使用默认设置将删除当前已设置好的系统设置,确认使用默认设置吗?";
            if (MessageBox.Show(strSQL, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            #region//默认
            this.ckbox_AllowedAllLeaveNormal.Checked = false;
            this.nud_AllowedAllLeaveNormal_Hour.Value = 0;
            this.nud_AllowedAllLeaveNormal_Minute.Value = 0;

            this.ckbox_AllowedAllLeaveAbNormal.Checked = true;
            this.nud_AllowedAllLeaveAbNormal_Hour.Value = 0;
            this.nud_AllowedAllLeaveAbNormal_Minute.Value = 0;

            this.ckbox_AllowedLogoutRedCardNormal.Checked = false;
            this.nud_AllowedLogoutRedCardNormal_Hour.Value = 0;
            this.nud_AllowedLogoutRedCardNormal_Minute.Value = 0;

            this.ckbox_AllowedLogoutRedCardAbNormal.Checked = false ;
            this.nud_AllowedLogoutRedCardAbNormal_Hour.Value = 0;
            this.nud_AllowedLogoutRedCardAbNormal_Minute.Value = 0;

            this.ckbox_AllowedLogoutBlueCard.Checked = false;
            this.nud_AllowedLogoutBlueCard_Hour.Value = 0;
            this.nud_AllowedLogoutBlueCard_Minute.Value = 0;

            this.ckbox_AllowedOverdueOrder.Checked = true;
            this.nud_AllowedOverdueOrder_Hour.Value = 0;
            this.nud_AllowedOverdueOrder_Minute.Value = 0;

            this.ckbox_AllowedOpenDoorDelay.Checked = true;
            this.nud_AllowedOpenDorrDelay_Seconds.Value = 3;


            this.ckbox_AllowedEnterAndLeaveCount.Checked = true;
            this.nud_AllowedEnterAndLeaveCount_Enter.Value = 1;
            this.nud_AllowedEnterAndLeaveCount_Leave.Value = 1;

            this.ckbox_AllowedOpenMSCat.Checked = true;
            #endregion
        }

        private void TSB_Exit_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void ckbox_AllowedVIPOpenDoorDelay_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbox_AllowedVIPOpenDoorDelay.Checked == true)
            {
                this.nud_AllowedVIPOpenDorrDelay_Seconds.Enabled = true;

                this.lbl_VIPOpenDoorDelay_Seconds.BackColor = Color.FromArgb(225, 128, 225);

                this.ckbox_AllowedVIPOpenDoorDelay.BackColor = Color.FromArgb(225, 128, 225);
            }
            else
            {
                 this.ckbox_AllowedVIPOpenDoorDelay.Checked = false ;
                this.nud_AllowedVIPOpenDorrDelay_Seconds.Enabled = false ;
                this.lbl_VIPOpenDoorDelay_Seconds.BackColor = Color.FromArgb(225, 225, 225);
                this.ckbox_AllowedVIPOpenDoorDelay.BackColor = Color.FromArgb(225, 225, 225);
  
            }
        }

 

    }
}
