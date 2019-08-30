using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.OleDb;
using System.Data.Sql;
using System.Net;
using System.Threading;
using System.IO.Ports;


namespace XXY_VisitorMJAsst
{
    public partial class LoginFrm_Configuration : Form
    {
        public LoginFrm_Configuration()
        {
            InitializeComponent();
        }
        private string strSQL = "";
        private DataTable myTable = new DataTable();
        private SQLHelper SQLHelper = new SQLHelper();
        private XXCLOUDDLL XXCLOUDDLL =new XXCLOUDDLL();
        public string strZT = "0";
        public string strServerName = Dns.GetHostName();//数据库服务器名称
        public string strDDIdConfirm = "0";
        public string strDLoginName = "";
        public string strDLoginPwd = "";
        private string strOActualNo = "";//新建年份时的操作员的实际编号
        private DataSet ds = new DataSet();
       
        #region//操作日志
        private void OperatorLog(string para_strOperateDescribe, string para_strResult)
        {
            try
            {
                if (LoginFrm.strCAccout.Trim().Length > 4)
                {
                    string strOperatorLog = "insert into " + LoginFrm.strT_OperateLogInf + " (OperatorNo,OperatorActualNo,OperatorName,";
                    strOperatorLog += " FormName,OperateDescribe,Result,OperateDT,Flag,CAccoutNo)values('" + LoginFrm.strOperatorNo + "',";
                    strOperatorLog += "'" + LoginFrm.strOperatorActualNo + "','" + LoginFrm.strOperatorName + "','" + "登录窗体_配置" + "',";
                    strOperatorLog += "'" + para_strOperateDescribe + "','" + para_strResult + "','" + System.DateTime.Now.ToString() + "','" + "访客系统" + "',";
                    strOperatorLog += "'" + LoginFrm.strCAccout.Substring(1, 4).Trim() + "')";
                    SQLHelper.ExecuteSql(strOperatorLog);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "日志写入错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//服务器配置
        private void FWQSZ()
        {
            try
            {
                if (this.txt_ServerName.Text.Trim() == "")
                {
                    MessageBox.Show("请输入SQL数据库服务器名称!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.txt_ServerName.Focus();
                    return;
                }
                if (this.rbtn_Sql.Checked == true)
                {
                    strDDIdConfirm = "1";
                    if (this.txt_DLoginName.Text.Trim() == "")
                    {
                        MessageBox.Show("请输入登录名!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.txt_DLoginName.Focus();
                        return;
                    }
                    if (SQLHelper.CheckFromInput(this.txt_DLoginName.Text.Trim()) == false)
                    {
                        MessageBox.Show("输入的登录名中包含非法字符，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.txt_DLoginName.Text = "";
                        this.txt_DLoginName.Focus();
                        return;
                    }
                    //if (this.txt_DLoginPwd.Text.Trim() != "")
                    //{
                    //    if (SQLHelper.CheckFromInput(this.txt_DLoginPwd.Text.Trim()) == false)
                    //    {
                    //        MessageBox.Show("输入的密码中包含非法字符，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //        this.txt_DLoginPwd.Text = "";
                    //        this.txt_DLoginPwd.Focus();
                    //        return;
                    //    }
                    //}
                    strDLoginName = this.txt_DLoginName.Text.Trim();
                    strDLoginPwd = SQLHelper.EncryptString(this.txt_DLoginPwd.Text.Trim() + "A4");//对输入的密码进行加密
                }
                string strCardCom_IC = "1";

                string strCardCom_ID = "Com2";
                if (strCardCom_ID.Trim() == "")
                {
                    strCardCom_ID = "Com2";
                }
                if (this.rbtn_Net.Checked == true)
                {
                    LoginFrm.iFlag_LockEncryption = 1;
                }
                else
                {
                    LoginFrm.iFlag_LockEncryption = 0;
                }
                XXCLOUDDLL.FWQSZ(Application.StartupPath, LoginFrm.strGuardRoomId, LoginFrm.strGuardRoomName, LoginFrm.strAccessPwd,
    this.txt_ServerName.Text.Trim(), strDDIdConfirm, strDLoginName, strDLoginPwd, strCardCom_IC, strCardCom_ID, "COM3", LoginFrm.iFlag_LockEncryption, LoginFrm.strRubbishClear, LoginFrm.strSqlServer_Remote, LoginFrm.strDDIdConfirm_Remote, LoginFrm.strDLoginName_Remote, LoginFrm.strDLoginPwd_Remote);

                //XXCLOUDDLL.FWQSZ(Application.StartupPath, LoginFrm.strGuardRoomId, LoginFrm.strGuardRoomName, LoginFrm.strAccessPwd,
                //    this.txt_ServerName.Text.Trim(),strDDIdConfirm, strDLoginName,strDLoginPwd, strCardCom_IC, strCardCom_ID, LoginFrm.iFlag_LockEncryption, LoginFrm.strRubbishClear, LoginFrm.strSqlServer_Remote, LoginFrm.strDDIdConfirm_Remote, LoginFrm.strDLoginName_Remote, LoginFrm.strDLoginPwd_Remote);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OperatorLog("服务器配置出现错误", "1");
            }
        }
        #endregion
 

        private void LoginFrm_Configuration_Load(object sender, EventArgs e)
        {
         
            ds.Tables.Clear();
            ds = XXCLOUDDLL.LoginFrm_LoadLocalConfigurationInf(Application.StartupPath.Trim(), LoginFrm.strAccessPwd);
            if (ds.Tables[0].Rows.Count > 0)
            {
                strSQL = SQLHelper.DecryptString(ds.Tables[0].Rows[0]["A1"].ToString().Trim());
                this.txt_ServerName.Text = strSQL.Substring(0, strSQL.Length - 2);
                strSQL = SQLHelper.DecryptString(ds.Tables[0].Rows[0]["A2"].ToString().Trim());
                strSQL = strSQL.Substring(0, strSQL.Length - 2);
                if (strSQL == "0")
                {
                    this.rbtn_Windows.Checked = true;
                    this.rbtn_Windows.BackColor = Color.FromArgb(225, 128, 225);
                    this.rbtn_Sql.BackColor = Color.FromArgb(225, 225, 225);
                }
                else
                {
                    this.rbtn_Sql.Checked = true;
                    this.rbtn_Windows.BackColor = Color.FromArgb(225, 225, 225);
                    this.rbtn_Sql.BackColor = Color.FromArgb(225, 128, 225);
                    strSQL = SQLHelper.DecryptString(ds.Tables[0].Rows[0]["A3"].ToString().Trim());
                    this.txt_DLoginName.Text = strSQL.Substring(0, strSQL.Length - 2);
                    strSQL = SQLHelper.DecryptString(ds.Tables[0].Rows[0]["A4"].ToString().Trim());
                    this.txt_DLoginPwd.Text = strSQL.Substring(0, strSQL.Length - 2);
                }
  
                if (ds.Tables[0].Rows[0]["A10"].ToString().Trim() == "0")
                {
                    this.rbtn_Single.Checked = true;
                    this.rbtn_Single.BackColor = Color.FromArgb(225, 128, 225);
                    this.rbtn_Net.BackColor = Color.FromArgb(225, 225, 225);
                }
                else
                {
                    this.rbtn_Net.Checked = true;
                    this.rbtn_Single.BackColor = Color.FromArgb(225, 225, 225);
                    this.rbtn_Net.BackColor = Color.FromArgb(225, 128, 225);
                }
            }
            else
            {
                this.rbtn_Single.BackColor = Color.FromArgb(225, 128, 225);
                this.txt_ServerName.Text = Dns.GetHostName();
                this.rbtn_Sql.Checked = true;
                this.rbtn_Sql.BackColor = Color.FromArgb(225, 128, 225);
                this.txt_DLoginName.Text = "sa";
                this.txt_DLoginPwd.Text = "123456";
            }
        }

        private void TSB_Ok_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            strServerName = this.txt_ServerName.Text.Trim();
            if (this.tabControl1.SelectedIndex == 0  )
            {
                #region
                FWQSZ();
                if (this.rbtn_Windows.Checked == true)//Windows身份验证
                {
                    strDDIdConfirm = "0";
                }
                else //SQL Server身份验证
                {
                    if (this.txt_DLoginName.Text.Trim() == "")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请输入登录名!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.tabControl1.SelectedIndex = 0;
                        this.txt_DLoginName.Focus();
                        return;
                    }
                    strDDIdConfirm = "1";
                }
                try
                {
                    if (SQLHelper.DBLink(strDDIdConfirm, this.txt_ServerName.Text.Trim(), "1", this.txt_DLoginName.Text.Trim(), this.txt_DLoginPwd.Text.Trim(), LoginFrm.strFlagDBLink, LoginFrm.strByInternet) == false)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("无法连接到数据库服务器,请确认配置正确！\n\n***注意：如果是网络版，请检查以下原因：\n\n 1、请确保本机与数据库服务器之间的网络畅通 \n 2、数据库服务器已开机使用，且关闭防火墙，开启网络通信助手***", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                catch (Exception exp)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                OperatorLog("服务器配置", "1");
                strZT = "1";
                this.Close();
                #endregion
            }
            else if (this.tabControl1.SelectedIndex == 1)//加入年份
            {
                #region
                if (this.txt_ServerName.Text.Trim() == "")
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("请输入SQL数据库服务器名称!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.tabControl1.SelectedIndex = 0;
                    this.txt_ServerName.Focus();
                    return;
                }
                try
                {
                    if (this.txt_JoinAccoutNo.Text.Trim() == "")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请输入4位纯数字的年份编号!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.txt_JoinAccoutNo.Text = "";
                        this.txt_JoinAccoutNo.Focus();
                        return;
                    }
                    if (this.txt_JoinAccoutNo.Text.Trim().Length != 4)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请输入4位纯数字的年份编号!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.txt_JoinAccoutNo.Text = "";
                        this.txt_JoinAccoutNo.Focus();
                        return;
                    }
                    if (this.txt_JoinAccoutNo.Text.Trim() != "")
                    {
                        int d = Convert.ToInt32(this.txt_JoinAccoutNo.Text.Trim());
                    }
                }
                catch
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("请输入4位纯数字的年份编号!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txt_JoinAccoutNo.Text = "";
                    this.txt_JoinAccoutNo.Focus();
                }
                if (this.rbtn_Windows.Checked == true)//Windows身份验证
                {
                    strDDIdConfirm = "0";
                }
                else //SQL Server身份验证
                {
                    strDDIdConfirm = "1";
                    if (this.txt_DLoginName.Text.Trim() == "")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请输入登录名!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.txt_DLoginName.Focus();
                        return;
                    }
                    if (SQLHelper.CheckFromInput(this.txt_DLoginName.Text.Trim()) == false)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("输入的登录名中包含非法字符，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.txt_DLoginName.Text = "";
                        this.txt_DLoginName.Focus();
                        return;
                    }
                    if (this.txt_DLoginPwd.Text.Trim() != "")
                    {
                        if (SQLHelper.CheckFromInput(this.txt_DLoginPwd.Text.Trim()) == false)
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("输入的密码中包含非法字符，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this.txt_DLoginPwd.Text = "";
                            this.txt_DLoginPwd.Focus();
                            return;
                        }
                    }
                    strDLoginName = this.txt_DLoginName.Text.Trim();
                    strDLoginPwd = SQLHelper.EncryptString(this.txt_DLoginPwd.Text.Trim() + "A4");//对输入的密码进行加密
                }
                try
                {
                    if (SQLHelper.DBLink(strDDIdConfirm, this.txt_ServerName.Text.Trim(), "1", this.txt_DLoginName.Text.Trim(), this.txt_DLoginPwd.Text.Trim(), LoginFrm.strFlagDBLink, LoginFrm.strByInternet) == false)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("无法连接到数据库服务器,请确认配置正确！\n\n***注意：如果是网络版，请检查以下原因：\n\n 1、请确保本机与数据库服务器之间的网络畅通 \n 2、数据库服务器已开机使用，且关闭防火墙，开启网络通信助手***", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string strCardCom_IC = "";// this.cmb_CK.Text.Trim();
                if (strCardCom_IC.Trim() == "")
                {
                    strCardCom_IC = "Com1";
                }
                string strCardCom_ID = "Com2";
                if (strCardCom_ID.Trim() == "")
                {
                    strCardCom_ID = "Com2";
                }
                if (this.rbtn_Net.Checked == true)
                {
                    LoginFrm.iFlag_LockEncryption = 1;
                }
                else
                {
                    LoginFrm.iFlag_LockEncryption = 0;
                }
                string strResult = "", strJoinAccoutName = "";
                strResult = XXCLOUDDLL.AddAccoutInfToLoc(this.txt_JoinAccoutNo.Text.Trim(), ref strJoinAccoutName, Application.StartupPath, LoginFrm.strAccessPwd, LoginFrm.strGuardRoomId, LoginFrm.strGuardRoomName,
    this.txt_ServerName.Text.Trim(), strDDIdConfirm, strDLoginName, strDLoginPwd, strCardCom_IC, strCardCom_ID, "COM3", LoginFrm.iFlag_LockEncryption, LoginFrm.strRubbishClear, LoginFrm.strSqlServer_Remote, LoginFrm.strDDIdConfirm_Remote, LoginFrm.strDLoginName_Remote, LoginFrm.strDLoginPwd_Remote);
                //strResult = XXCLOUDDLL.AddAccoutInfToLoc(this.txt_JoinAccoutNo.Text.Trim(), ref strJoinAccoutName, Application.StartupPath, LoginFrm.strAccessPwd, LoginFrm.strGuardRoomId, LoginFrm.strGuardRoomName,
                //    this.txt_ServerName.Text.Trim(), strDDIdConfirm, strDLoginName, strDLoginPwd, strCardCom_IC, strCardCom_ID, LoginFrm.iFlag_LockEncryption, LoginFrm.strRubbishClear, LoginFrm.strSqlServer_Remote, LoginFrm.strDDIdConfirm_Remote, LoginFrm.strDLoginName_Remote, LoginFrm.strDLoginPwd_Remote);



                if (strResult.Length > 3)
                {
                    strZT = "3";
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("名称为 [" + this.txt_JoinAccoutNo.Text.Trim() + "]" + strJoinAccoutName + " 的年份加入成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OperatorLog("新年份加入成功!" + " 名称为[" + this.txt_JoinAccoutNo.Text.Trim() + "]" + strJoinAccoutName, "1");
                    this.Close();
                }
                else
                {
                    if (strResult == "0")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("名称为 [" + this.txt_JoinAccoutNo.Text.Trim() + "]" + strJoinAccoutName + " 的年份已经存在,加入失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        OperatorLog("加入已存在年份!" + " 名称为[" + this.txt_JoinAccoutNo.Text.Trim() + "]" + strJoinAccoutName, "0");
                    }
                    else if (strResult == "1")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("名称为 [YEAR" + this.txt_JoinAccoutNo.Text.Trim() + "]" + " 的年份不存在,加入失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        OperatorLog("加入不存在年份!" + " 名称为 [YEAR" + this.txt_JoinAccoutNo.Text.Trim() + "]", "0");
                    }
                    else if (strResult == "2")
                    {
                        this.Cursor = Cursors.Default;
                        OperatorLog("加入新年份失败!" + " 名称为[" + this.txt_JoinAccoutNo.Text.Trim() + "]" + strJoinAccoutName, "1");
                        MessageBox.Show("名称为 [YEAR" + this.txt_JoinAccoutNo.Text.Trim() + "]" + " 的年份不存在,加入失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (strResult == "3")
                    {
                        this.Cursor = Cursors.Default;
                        OperatorLog("加入新年份失败!" + " 名称为[" + this.txt_JoinAccoutNo.Text.Trim() + "]" + strJoinAccoutName, "1");
                    }
                }
                #endregion
            }
            else if (this.tabControl1.SelectedIndex == 2)//新建年份
            {
                #region
                try
                {
                    if (this.txt_ServerName.Text.Trim() == "")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请输入SQL数据库服务器名称!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.tabControl1.SelectedIndex = 0;
                        this.txt_ServerName.Focus();
                        return;
                    }

                    try
                    {
                        if (this.txt_NewAccoutNo.Text.Trim() == "")
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("请输入4位纯数字的年份编号!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.txt_NewAccoutNo.Text = "";
                            this.txt_NewAccoutNo.Focus();
                            return;
                        }
                        if (this.txt_NewAccoutNo.Text.Trim().Length != 4)
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("请输入4位纯数字的年份编号!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this.txt_NewAccoutNo.Text = "";
                            this.txt_NewAccoutNo.Focus();
                            return;
                        }
                        if (this.txt_NewAccoutNo.Text.Trim() != "")
                        {
                            int d = Convert.ToInt32(this.txt_NewAccoutNo.Text.Trim());
                        }
                    }
                    catch
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请输入4位纯数字的年份编号!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.txt_NewAccoutNo.Text = "";
                        this.txt_NewAccoutNo.Focus();
                    }
                    if (this.txt_NewAccoutName.Text.Trim() == "")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请输入年份名称!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.txt_NewAccoutName.Focus();
                        return;
                    }
                    if (this.txt_NewOperatorName.Text.Trim() == "")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请输入操作员姓名!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.txt_NewOperatorName.Focus();
                        return;
                    }
                    //if (this.txt_NewEndUserName.Text.Trim() == "")
                    //{
                    //    this.Cursor = Cursors.Default;
                    //    MessageBox.Show("请输入本软件使用单位名称!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //    this.txt_NewEndUserName.Focus();
                    //    return;
                    //}
                    if (this.txt_ServerName.Text.Trim() == "")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请输入SQL数据库服务器名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.txt_ServerName.Focus();
                        return;
                    }
                    if (this.rbtn_Windows.Checked == true)//Windows身份验证
                    {
                        strDDIdConfirm = "0";
                    }
                    else //SQL Server身份验证
                    {
                        strDDIdConfirm = "1";
                        if (this.txt_DLoginName.Text.Trim() == "")
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("请输入登录名!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this.tabControl1.SelectedIndex = 0;
                            this.txt_DLoginName.Focus();
                            return;
                        }
                        strDLoginName = this.txt_DLoginName.Text.Trim();
                        strDLoginPwd = SQLHelper.EncryptString(this.txt_DLoginPwd.Text.Trim() + "A4");//对输入的密码进行加密
                    }
                    try
                    {
                        if (SQLHelper.DBLink(strDDIdConfirm, this.txt_ServerName.Text.Trim(), "1", this.txt_DLoginName.Text.Trim(), this.txt_DLoginPwd.Text.Trim(), LoginFrm.strFlagDBLink, LoginFrm.strByInternet) == false)
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("无法连接到数据库服务器,请确认配置正确！\n\n***注意：如果是网络版，请检查以下原因：\n\n 1、请确保本机与数据库服务器之间的网络畅通 \n 2、数据库服务器已开机使用，且关闭防火墙，开启网络通信助手***", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string strCardCom_IC = "COM1";// this.cmb_CK.Text.Trim();
                    if (strCardCom_IC.Trim() == "")
                    {
                        strCardCom_IC = "1";
                    }
                    else
                    {
                        strCardCom_IC = strCardCom_IC.ToString().Trim().Substring(3);
                    }
                    string strCardCom_ID = "COM1";
                    if (strCardCom_ID.Trim() == "")
                    {
                        strCardCom_ID = "2";
                    }
                    else
                    {
                        strCardCom_ID = strCardCom_ID.ToString().Trim().Substring(3);
                    }
                    string strSMSCatCom = "Com1";
                    if (strSMSCatCom.Trim() == "")
                    {
                        strSMSCatCom = "Com3";
                    }
                    //string iFlag_LockEncryption = "0";
                    // if (this.rbtn_Net.Checked == true)
                    // {
                    //     iFlag_LockEncryption = "1";
                    // }
                    string strBJ = XXCLOUDDLL.CreateDBTable("YEAR" + this.txt_NewAccoutNo.Text.Trim(), SQLHelper.connectionString_1, Application.StartupPath.ToString(),
                          this.txt_NewOperatorNo.Text, this.txt_NewOperatorName.Text.Trim(), strOActualNo, this.txt_NewAccoutNo.Text.Trim(),
                           this.txt_NewAccoutName.Text.Trim(), this.txt_NewEndUserName.Text.Trim(), LoginFrm.strAccessPwd, LoginFrm.strGuardRoomId,
                          LoginFrm.strGuardRoomName, this.txt_ServerName.Text.Trim(), strDDIdConfirm, strDLoginName, strDLoginPwd, strCardCom_IC, strCardCom_ID, strSMSCatCom, LoginFrm.iFlag_LockEncryption, LoginFrm.strRubbishClear, LoginFrm.strSqlServer_Remote, LoginFrm.strDDIdConfirm_Remote, LoginFrm.strDLoginName_Remote, LoginFrm.strDLoginPwd_Remote);
                    if (strBJ == "1")
                    {
                        MessageBox.Show("名称为 [" + this.txt_NewAccoutNo.Text.Trim() + "]" + this.txt_NewAccoutName.Text.Trim() + " 的年份新建成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        strZT = "2";
                        OperatorLog("新建年份" + "名称为 [" + this.txt_NewAccoutNo.Text.Trim() + "]" + this.txt_NewAccoutName.Text.Trim(), "1");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(strBJ, "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception exp)
                {
                    this.Cursor = Cursors.Default;
                    OperatorLog("新建年份出现错误" + "名称为 [" + this.txt_NewAccoutNo.Text.Trim() + "]" + this.txt_NewAccoutName.Text.Trim(), "0");
                    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion
            }
        }

        private void TSB_Exit_Click(object sender, EventArgs e)
        {

            strZT = "0";
            this.Close();
        }

        private void rbtn_Windows_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbtn_Sql.Checked == true)
            {
                this.txt_DLoginName.ReadOnly = false;
                this.txt_DLoginPwd.ReadOnly = false;
                this.rbtn_Sql.BackColor = Color.FromArgb(225, 128, 225);
                this.rbtn_Windows.BackColor = Color.FromArgb(225, 225, 225);
                this.txt_DLoginName.Focus();
            }
            else
            {
                this.txt_DLoginName.ReadOnly = true;
                this.txt_DLoginPwd.ReadOnly = true;
                this.rbtn_Windows.BackColor = Color.FromArgb(225, 128, 225);
                this.rbtn_Sql.BackColor = Color.FromArgb(225, 225, 225);
            }
        }
 
     
 


        private void rbtn_Single_Click(object sender, EventArgs e)
        {
            this.rbtn_Single.BackColor = Color.FromArgb(225, 128, 225);
            this.rbtn_Net.BackColor = Color.FromArgb(225, 225, 225);
            this.txt_ServerName.Text = Dns.GetHostName();
            this.txt_ServerName.Focus();
        }

        private void rbtn_Net_Click(object sender, EventArgs e)
        {
            string LocalName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(LocalName);
            foreach (IPAddress ip in host.AddressList)
            {
                this.txt_ServerName.Text = ip.ToString();
                if (this.txt_ServerName.Text.Trim().Contains(":") == false)
                {
                    break;
                }
            }
            this.rbtn_Net.BackColor = Color.FromArgb(225, 128, 225);
            this.rbtn_Single.BackColor = Color.FromArgb(225, 225, 225);
            this.txt_ServerName.Focus();
    
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
            //if (this.comboBox1.Text.Trim() == "")
            //{
            //    return;
            //}
            //else if (this.comboBox1.Text.Trim() == "钻石绿")
            //{
            //    this.skinEngine1.SkinFile = "DiamondGreen.ssk";
            //}
            //else if (this.comboBox1.Text.Trim() == "深蓝绿")
            //{
            //    this.skinEngine1.SkinFile = "DeepCyan.ssk";
            //}
            //else if (this.comboBox1.Text.Trim() == "浅蓝")
            //{
            //    this.skinEngine1.SkinFile = "MP10.ssk";
            //}
            //else if (this.comboBox1.Text.Trim() == "MSN")
            //{
            //    this.skinEngine1.SkinFile = "MSN.ssk";
            //}
            //else if (this.comboBox1.Text.Trim() == "Vista2_color7")
            //{
            //    this.skinEngine1.SkinFile = "Vista2_color7.ssk";
            //}
            //else if (this.comboBox1.Text.Trim() == "MacOS")
            //{
            //    this.skinEngine1.SkinFile = "MacOS.ssk";
            //}
            //else if (this.comboBox1.Text.Trim() == "Vista2_color7")
            //{
            //    this.skinEngine1.SkinFile = "Vista2_color7.ssk";
            //}
            //else if (this.comboBox1.Text.Trim() == "Vista2_color7")
            //{
            //    this.skinEngine1.SkinFile = "Vista2_color7.ssk";
            //}
            //else if (this.comboBox1.Text.Trim() == "Vista2_color7")
            //{
            //    this.skinEngine1.SkinFile = "Vista2_color7.ssk";
            //}
            //else if (this.comboBox1.Text.Trim() == "Vista2_color7")
            //{
            //    this.skinEngine1.SkinFile = "Vista2_color7.ssk";
            //}
        }

        
  
    }
}
