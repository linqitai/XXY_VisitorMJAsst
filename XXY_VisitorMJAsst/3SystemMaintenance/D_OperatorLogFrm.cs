using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XXY_VisitorMJAsst._3SystemMaintenance
{
    public partial class D_OperatorLogFrm : Form
    {
        public D_OperatorLogFrm()
        {
            InitializeComponent();
        }
        private string strSQL = "";
        private DataTable myTable = new DataTable();
        private SQLHelper SQLHelper = new SQLHelper ();
        private XXCLOUDDLL XXCLOUDDLL =new XXCLOUDDLL();
        private string strFormName = "访客登记_系统操作日志_日志排序方式";
        private string strDisplayOrder = "";
        public static string strDt_Start = "";
        public static string strDt_End = "";
        private string strOperateDT = System.DateTime.Now.ToShortDateString();
        private string strSQL_CX = "";
        private bool blFlag = false;
        private DataTable dt_Search = new DataTable();//操作日志记录字段
        public static DataTable dt_GOId = new DataTable();
        private bool blLoad = false;//false:窗体首次加载  true:已加载完成
        private DataTable dt_TempSearch = new DataTable();
        private string strSearchContent = "";//查询内容

        #region//加载查询下拉列表框
        private void LoadSearchList()
        {

            this.dt_Search.Rows.Clear();
            DataRow dr = dt_Search.NewRow();
            dr["FieldName"] = "";
            dr["FieldSimplify"] = "";
            dt_Search.Rows.Add(dr);

            dr = dt_Search.NewRow();
            dr["FieldName"] = "操作员";
            dr["FieldSimplify"] = "OperatorName";
            dt_Search.Rows.Add(dr);

            dr = dt_Search.NewRow();
            dr["FieldName"] = "所在操作窗体";
            dr["FieldSimplify"] = "FormName";
            dt_Search.Rows.Add(dr);

            dr = dt_Search.NewRow();
            dr["FieldName"] = "结果";
            dr["FieldSimplify"] = "Result";
            dt_Search.Rows.Add(dr);

             

            this.cmb_List.DataSource = dt_Search;
            this.cmb_List.DisplayMember = "FieldName";
            this.cmb_List.ValueMember = "FieldSimplify";
        }
        #endregion

        #region//加载系统操作日志
        private void LoadOperateLog(string para_strOperateDescribe)
        {
            try
            {
                this.grid1.AutoRedraw = false;
                this.grid1.Rows = 1;
                strSQL = "select * from  T_OperateLogInf where Flag='" + "访客系统" + "' and (CAccoutNo ='" + LoginFrm.strCAccout.Substring(1, 4).ToString().Trim() + "'";
                strSQL += " or CAccoutNo='" + "" + "') and OperateDT between '" + Convert.ToString(this.dtp_Start.Value.AddSeconds(1).AddSeconds(-1)) + "' and  '" + Convert.ToString(this.dtp_End.Value.AddDays(1).AddSeconds(-1)) + "'  " + para_strOperateDescribe + "   " + strDisplayOrder;
                myTable = SQLHelper.DTQuery(strSQL);
                strSQL_CX = strSQL;
                if (myTable.Rows.Count > 0)
                {
                    System.Drawing.Font font = new System.Drawing.Font("微软雅黑", (float)10, FontStyle.Regular);
                    PointF pointF = new PointF(this.progressBar1.Width / 2 - 250, this.progressBar1.Height / 2 - 10);
                    this.progressBar1.Minimum = 0;
                    this.progressBar1.Maximum = myTable.Rows.Count;
                    this.progressBar1.Visible = true;
                    for (int i = 0; i < myTable.Rows.Count; i++)
                    {
                        this.progressBar1.Value = i;
                        System.Windows.Forms.Application.DoEvents();
                        this.progressBar1.CreateGraphics().DrawString("正在加载当前时间段内的系统操作日志记录：" + this.progressBar1.Value.ToString() + "/" + myTable.Rows.Count.ToString() + "   请耐心等候....", font, Brushes.Red, pointF);

                        this.grid1.Rows++;

                        if (myTable.Rows[i]["Result"].ToString().Trim() == "1")
                        {
                            this.grid1.Cell(this.grid1.Rows - 1, 6).Text = "成功";
                            this.grid1.Cell(this.grid1.Rows - 1, 1).Text = myTable.Rows[i]["Id"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 2).Text = myTable.Rows[i]["OperatorNo"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 3).Text = myTable.Rows[i]["OperatorName"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 4).Text = myTable.Rows[i]["FormName"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 5).Text = myTable.Rows[i]["OperateDescribe"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 7).Text = myTable.Rows[i]["OperateDT"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 1).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 2).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 3).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 4).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 5).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 6).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 7).ForeColor = Color.Blue;
                        }
                        else if (myTable.Rows[i]["Result"].ToString().Trim() == "0")
                        {
                            this.grid1.Cell(this.grid1.Rows - 1, 6).Text = "失败";
                            this.grid1.Cell(this.grid1.Rows - 1, 1).Text = myTable.Rows[i]["Id"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 2).Text = myTable.Rows[i]["OperatorNo"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 3).Text = myTable.Rows[i]["OperatorName"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 4).Text = myTable.Rows[i]["FormName"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 5).Text = myTable.Rows[i]["OperateDescribe"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 7).Text = myTable.Rows[i]["OperateDT"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 1).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 2).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 3).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 4).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 5).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 6).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 7).ForeColor = Color.Red;
                        }
                        this.grid1.Row(this.grid1.Rows - 1).Locked = true;
              
                    }
                    System.Threading.Thread.Sleep(500);
                    this.progressBar1.Visible = false;
                }
                if (this.grid1.Rows > 1)
                {
                    this.grid1.Range(this.grid1.Rows - 1, 1, this.grid1.Rows - 1, this.grid1.Cols - 1).SelectCells();
                }
                this.grid1.AutoRedraw = true;
                this.grid1.Refresh();
            }
            catch (Exception exp)
            {
                this.progressBar1.Visible = false;
                MessageBox.Show(exp.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//操作日志
        private void OperatorLog(string para_strOperateDescribe, string para_strResult)
        {
            try
            {
                string strOperatorLog = "insert into " + LoginFrm.strT_OperateLogInf + " (OperatorNo,OperatorActualNo,OperatorName,";
                strOperatorLog += " FormName,OperateDescribe,Result,OperateDT,Flag,CAccoutNo)values('" + LoginFrm.strOperatorNo + "',";
                strOperatorLog += "'" + LoginFrm.strOperatorActualNo + "','" + LoginFrm.strOperatorName + "','" + "系统操作日志" + "',";
                strOperatorLog += "'" + para_strOperateDescribe + "','" + para_strResult + "','" + System.DateTime.Now.ToString() + "','" + "访客系统" + "',";
                strOperatorLog += "'" + LoginFrm.strCAccout.Substring(1, 4).Trim() + "')";
                SQLHelper.ExecuteSql(strOperatorLog);
            }
            catch (Exception exp)
            {
                if (exp.ToString().Trim().Contains("远程主机") == true)
                {
                    //XXY_VisitorMJAsst._4ProsceniumRegister.PRegisterFrm.blExit = true;
                    //MainBFrm.blExit = true;
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

        private void D_OperatorLogFrm_Load(object sender, EventArgs e)
        {
            this.cmb_code.SelectedIndex = 0;
            clsIme.SetIme(this);//控制输入法的状态
            this.TSB_ChangingLog.Checked = false;
            #region//设置grid1样式
            this.grid1.AutoRedraw = false;
            this.grid1.Rows = 1;
            this.grid1.Cols = 1;
            this.grid1.DisplayRowNumber = true;
            this.grid1.DisplayRowArrow = true;
            this.grid1.MultiSelect = true;
            this.grid1.StartRowNumber = 1;
            this.grid1.EnableTabKey = true;
            this.grid1.EnableVisualStyles = true;
            this.grid1.SelectionMode = FlexCell.SelectionModeEnum.ByRow;//选中整行
            this.grid1.DisplayFocusRect = true;//返回或设置控件在当前活动单元格是否显示一个虚框
            this.grid1.ExtendLastCol = true;//扩展最后一列
            this.grid1.FixedRowColStyle = FlexCell.FixedRowColStyleEnum.Light3D;//返回或设置固定行/列的样式
            this.grid1.DefaultFont = new Font("宋体", 9);
            this.grid1.BorderStyle = FlexCell.BorderStyleEnum.Light3D;
            this.grid1.BackColorFixed = Color.FromArgb(225, 225, 225);//固定行／列的颜色
            this.grid1.BackColorFixedSel = Color.FromArgb(225, 225, 225);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
            this.grid1.BackColorSel = Color.FromArgb(210, 255, 166);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
            this.grid1.BackColorBkg = Color.FromArgb(255, 255, 255);//返回和设置空白区域的背景色(这里为白色)
            this.grid1.BackColor1 = Color.FromArgb(231, 235, 247);//返回或设置表格中奇数行的背景色
            this.grid1.BackColor2 = Color.FromArgb(239, 243, 255);//返回或设置表格中偶数行的背景色
            this.grid1.CellBorderColorFixed = Color.Red;//返回或设置固定行和固定列上的单元格边框的颜色
            this.grid1.GridColor = Color.FromArgb(148, 190, 231);//返回或设置网格线的颜色
            this.grid1.AllowUserReorderColumn = true;//允许操作员拖动列标题来移动整列
            this.grid1.AllowUserResizing = FlexCell.ResizeEnum.Both;
            this.grid1.AllowUserSort = true;

            this.grid1.EnableVisualStyles = true;//显示XP效果
            this.grid1.DefaultRowHeight = 27;//默认行高
            this.grid1.EnableTabKey = true;//按Tab键时移动活动单元格
            this.grid1.EnterKeyMoveTo = FlexCell.MoveToEnum.NextCol;//按回车键时自动跳到下一列
            this.grid1.SelectionBorderColor = Color.Red;//设置selection边框的颜色
            this.grid1.ShowResizeTip = true;//返回或设置一个值，该值决定了操作员用鼠标调整FlexCell表格的行高或列宽时，是否显示行高或列宽的提示窗口。

            grid1.Cols = 8;
            grid1.Cell(0, 1).Text = "序号";
            grid1.Cell(0, 2).Text = "操作员编号";
            grid1.Cell(0, 3).Text = "姓名";
            grid1.Cell(0, 4).Text = "所在操作窗体";
            grid1.Cell(0, 5).Text = "操作描述";
            grid1.Cell(0, 6).Text = "结果";
            grid1.Cell(0, 7).Text = "操作时间";
            grid1.Column(1).Width = 50;
            grid1.Column(2).Width = 70;
            grid1.Column(3).Width = 80;
            grid1.Column(4).Width = 200;
            grid1.Column(5).Width = 650;
            grid1.Column(6).Width = 45;
            grid1.Column(7).Width = 140;
            this.grid1.AutoRedraw = true;
            this.grid1.Refresh();
            #endregion

            #region//日志排序方式
            strSQL = "select * from T_FormDisplayItemInf where FormName = '" + strFormName + "%'  ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                if (myTable.Rows[0]["Flag"].ToString() == "0")
                {
                    strDisplayOrder = "order by  OperateDT asc";
                    this.TSB_AscOrder.Checked = true;
                }
                else if (myTable.Rows[0]["Flag"].ToString() == "1")
                {
                    strDisplayOrder = " order by OperateDT desc ";
                    this.TSB_DecOrder.Checked = true;
                }
                else if (myTable.Rows[0]["Flag"].ToString() == "2")
                {
                    strDisplayOrder = " order by OperatorNo";
                    this.TSB_OperatorNoOrder.Checked = true;
                }
            }
            else
            {
                this.TSB_AscOrder.Checked = true;
                XXCLOUDDLL.DisplayInf(strFormName, "", "0", "");
            }

            #endregion

            strSQL = " select max(OperateDT) OperateDT from  T_OperateLogInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)//取操作日志中的最大操作时间,防止操作员通过修改系统时间实现对日志的删除
            {
                if (myTable.Rows[0]["OperateDT"].ToString().Trim() != "" && myTable.Rows[0]["OperateDT"].ToString().Trim() != null)
                {
                    strOperateDT = Convert.ToDateTime(myTable.Rows[0]["OperateDT"].ToString().Trim()).ToShortDateString();
                }
            }
            strDt_Start = Convert.ToDateTime(System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-01").ToString();
            string strLastDay = DateTime.DaysInMonth(System.DateTime.Now.Year, System.DateTime.Now.Month).ToString();//可求出每月的最后一天
            strDt_End = Convert.ToDateTime(System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + strLastDay).ToString();

            this.dtp_Start.Value = Convert.ToDateTime(System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-01");
            //string strLastDay = DateTime.DaysInMonth(System.DateTime.Now.Year, System.DateTime.Now.Month).ToString();//可求出每月的最后一天
            this.dtp_End.Value = Convert.ToDateTime(System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + strLastDay);

            #region//加载查询下拉框
            try
            {
                if (blFlag == false)
                {
                    blFlag = true;
                    dt_GOId.Columns.Add("Id");
                }
            }
            catch
            {

            }
            dt_Search.Columns.Add("FieldName");
            dt_Search.Columns.Add("FieldSimplify");
            LoadSearchList();

            #endregion
            LoadOperateLog("");
            blLoad = true;
        }

        private void TSB_Delete_Click(object sender, EventArgs e)
        {
            if (this.grid1.Cell(this.grid1.Selection.FirstRow, 1).Text == "")
            {
                MessageBox.Show("请选择要删除的操作日志信息!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            DataTable dt_temp = new DataTable();
            StringBuilder sqlList = new StringBuilder();
            try
            {
                if (this.grid1.Selection.FirstRow == this.grid1.Selection.LastRow)
                {
                    if (MessageBox.Show("是否真的删除当前选定的操作日志信息?[系统将默认保留三天内的日志记录]", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DateTime dt_DQRQ = Convert.ToDateTime(strOperateDT);
                        DateTime d2;
                        if (this.grid1.Cell(this.grid1.Selection.FirstRow, 7).Text.Trim() != "" && this.grid1.Cell(this.grid1.Selection.FirstRow, 7).Text.Trim() != null)
                        {
                            d2 = Convert.ToDateTime(this.grid1.Cell(this.grid1.Selection.FirstRow, 7).Text.Trim());
                            TimeSpan s = dt_DQRQ - d2;
                            if (s.TotalDays > 2)
                            {

                            }
                            else
                            {
                                MessageBox.Show("系统默认保留三天内的日志记录,不能删除!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                        //清空string strSQL = "delete from QD_LOG where  Datediff(d,SJ,  '" + System.DateTime.Now.ToShortDateString() + "') > 3 ";
                        strSQL = "delete from T_OperateLogInf where  Datediff(d,OperateDT,  '" + strOperateDT + "') > 2 and ID in (" + this.grid1.Cell(this.grid1.Selection.FirstRow, 1).Text.Trim() + ")";
                        if (SQLHelper.ExecuteSql(strSQL) != 0)
                        {
                            this.grid1.Selection.DeleteByRow();
                            if (this.grid1.Rows > 1)
                            {
                                this.grid1.Range(this.grid1.Rows - 1, 1, this.grid1.Rows - 1, this.grid1.Cols - 1).SelectCells();
                            }
                        }
                        else
                        {
                            MessageBox.Show("系统默认保留三天内的日志记录,不能删除!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("是否真的删除当前选定的所有操作日志信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //要进行判断,是否在三天之内,要提示,参考职工删除
                        string strFilter = "";
                        DateTime dt_DQRQ = Convert.ToDateTime(strOperateDT);
                        DateTime d2;
                        bool bl = false;
                        bool bl_1 = false;
                        int iCount_Record = 0;
                        for (int i = this.grid1.Selection.FirstRow; i <= this.grid1.Selection.LastRow; i++)
                        {
                            if (this.grid1.Cell(i, 7).Text.Trim() != "" && this.grid1.Cell(i, 7).Text.Trim() != null)
                            {
                                d2 = Convert.ToDateTime(this.grid1.Cell(i, 7).Text.Trim());
                                TimeSpan s = dt_DQRQ - d2;
                                if (s.TotalDays > 2)
                                {
                                    strFilter += " ID  = " + "'" + this.grid1.Cell(i, 1).Text.Trim() + "'" + " or ";
                                    iCount_Record++;
                                    bl_1 = true;
                                }
                                else
                                {
                                    bl = true;
                                }
                            }
                        }
                        if (bl == true && bl_1 == true)
                        {
                            if (MessageBox.Show("在所选择中的操作日志中有部份或全部日志由于是在最近三天内发生的,故不能删除,是否删除最近三天外的操作日志信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                //空操作
                            }
                            else
                            {
                                return;
                            }

                        }
                        if (bl == true && bl_1 == false)
                        {
                            MessageBox.Show("在所选择中的操作日志中有部份或全部日志由于是在最近三天内发生的,故不能删除!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        if (strFilter.Trim() != "")
                        {
                            strFilter = strFilter.Substring(0, strFilter.Length - 4);
                        }
                        strSQL = "delete from T_OperateLogInf where " + strFilter;
                        if (SQLHelper.ExecuteSql(strSQL) != 0)
                        {
                            OperatorLog("删除多选操作日志信息,共" + iCount_Record.ToString() + "条", "1");
                            this.grid1.Selection.DeleteByRow();
                            if (this.grid1.Rows > 1)
                            {
                                this.grid1.Range(this.grid1.Rows - 1, 1, this.grid1.Rows - 1, this.grid1.Cols - 1).SelectCells();
                            }
                        }

                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OperatorLog("删除操作日志信息出现错误", "0");
            }
        }

        private void TSB_Refresh_Click(object sender, EventArgs e)
        {
            strDt_Start = Convert.ToDateTime(System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-01").ToString();
            strDt_End = Convert.ToString(Convert.ToDateTime(System.DateTime.Now.ToShortDateString()).AddDays(1).AddSeconds(-1));//求出每月最后一天
            if (this.TSB_ChangingLog.Checked == false)
            {
                LoadOperateLog("");
            }
            else
            {
                LoadOperateLog(" and FormName ='" + "登录窗体" + "' and OperateDescribe Like '%" + "]登录" + "%'");
            }
        }

        private void TSB_AscOrder_Click(object sender, EventArgs e)
        {
            if (this.TSB_AscOrder.Checked == true)
            {
                this.TSB_AscOrder.Checked = false;
            }
            else
            {
                this.TSB_AscOrder.Checked = true;
                this.TSB_OperatorNoOrder.Checked = false;
                this.TSB_DecOrder.Checked = false;
                strDisplayOrder = " order by OperateDT asc";
            }
            if (this.TSB_AscOrder.Checked == false && this.TSB_OperatorNoOrder.Checked == false && this.TSB_DecOrder.Checked == false)
            {
                this.TSB_AscOrder.Checked = true;
                strDisplayOrder = " order by OperateDT asc";
            }
            XXCLOUDDLL.DisplayInf(strFormName, "", "0", "");
            if (this.TSB_ChangingLog.Checked == false)
            {
                LoadOperateLog("");
            }
            else
            {
                LoadOperateLog(" and FormName ='" + "登录窗体" + "' and OperateDescribe Like '%" + "]登录" + "%'");
            }
        }

        private void TSB_DecOrder_Click(object sender, EventArgs e)
        {
            if (this.TSB_DecOrder.Checked == true)
            {
                this.TSB_DecOrder.Checked = false;
            }
            else
            {
                this.TSB_DecOrder.Checked = true;
                this.TSB_AscOrder.Checked = false;
                this.TSB_OperatorNoOrder.Checked = false;
                strDisplayOrder = "  order by OperateDT desc  ";
            }

            if (this.TSB_AscOrder.Checked == false && this.TSB_OperatorNoOrder.Checked == false && this.TSB_DecOrder.Checked == false)
            {
                this.TSB_DecOrder.Checked = true;
                strDisplayOrder = " order by OperateDT desc ";
            }
            XXCLOUDDLL.DisplayInf(strFormName, "", "1", "");
            if (this.TSB_ChangingLog.Checked == false)
            {
                LoadOperateLog("");
            }
            else
            {
                LoadOperateLog(" and FormName ='" + "登录窗体" + "' and OperateDescribe Like '%" + "]登录" + "%'");
            }
        }

        private void TSB_OperatorNoOrder_Click(object sender, EventArgs e)
        {
            if (this.TSB_OperatorNoOrder.Checked == true)
            {
                this.TSB_OperatorNoOrder.Checked = false;
            }
            else
            {
                this.TSB_OperatorNoOrder.Checked = true;
                this.TSB_AscOrder.Checked = false;
                this.TSB_DecOrder.Checked = false;
                strDisplayOrder = " order by OperatorNo  ";
            }
            if (this.TSB_AscOrder.Checked == false && this.TSB_OperatorNoOrder.Checked == false && this.TSB_DecOrder.Checked == false)
            {
                this.TSB_DecOrder.Checked = true;
                strDisplayOrder = " order by OperatorNo ";
            }
            XXCLOUDDLL.DisplayInf(strFormName, "", "2", "");
            if (this.TSB_ChangingLog.Checked == false)
            {
                LoadOperateLog("");
            }
            else
            {
                LoadOperateLog(" and FormName ='" + "登录窗体" + "' and OperateDescribe Like '%" + "]登录" + "%'");
            }
        }

        private void TSB_ChangingLog_Click(object sender, EventArgs e)
        {
            if (this.TSB_ChangingLog.Checked == true)
            {
                this.TSB_ChangingLog.Checked = false;
                this.TSI_ChangingLog.Checked = false;
                LoadOperateLog("");
            }
            else
            {
                this.TSB_ChangingLog.Checked = true;
                this.TSI_ChangingLog.Checked = true;
                LoadOperateLog(" and ((FormName ='" + "登录窗体" + "' and OperateDescribe Like '%" + "]登录" + "%') or (FormName='" + "前台交接班" + "' and OperateDescribe Like '%" + "前台交接" + "%'))");
            }
        }

        private void TSB_Search_Click(object sender, EventArgs e)
        {
            try
            {
                //问题:自定义时,日期型数据的处理
                D_OperatorLogFrm_LogSearch newFrm = new  D_OperatorLogFrm_LogSearch ();
                newFrm.ShowDialog();
                if (newFrm.strRZXX_CXTJ.Trim() == "")
                {
                    return;
                }

                this.grid1.AutoRedraw = false;
                this.grid1.Rows = 1;
                strSQL = strSQL_CX;
                if (strSQL.Contains("OperateDT") == true)
                {
                    strSQL = strSQL.Substring(0, strSQL.LastIndexOf("OperateDT"));
                }
                strSQL += "  " + newFrm.strRZXX_CXTJ + strDisplayOrder;
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    for (int i = 0; i < myTable.Rows.Count; i++)
                    {
                        this.grid1.Rows++;

                        if (myTable.Rows[i]["Result"].ToString().Trim() == "1")
                        {
                            this.grid1.Cell(this.grid1.Rows - 1, 6).Text = "成功";
                            this.grid1.Cell(this.grid1.Rows - 1, 1).Text = myTable.Rows[i]["Id"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 2).Text = myTable.Rows[i]["OperatorNo"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 3).Text = myTable.Rows[i]["OperatorName"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 4).Text = myTable.Rows[i]["FormName"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 5).Text = myTable.Rows[i]["OperateDescribe"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 7).Text = myTable.Rows[i]["OperateDT"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 1).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 2).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 3).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 4).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 5).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 6).ForeColor = Color.Blue;
                            this.grid1.Cell(this.grid1.Rows - 1, 7).ForeColor = Color.Blue;
                        }
                        else if (myTable.Rows[i]["Result"].ToString().Trim() == "0")
                        {
                            this.grid1.Cell(this.grid1.Rows - 1, 6).Text = "失败";
                            this.grid1.Cell(this.grid1.Rows - 1, 1).Text = myTable.Rows[i]["Id"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 2).Text = myTable.Rows[i]["OperatorNo"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 3).Text = myTable.Rows[i]["OperatorName"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 4).Text = myTable.Rows[i]["FormName"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 5).Text = myTable.Rows[i]["OperateDescribe"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 7).Text = myTable.Rows[i]["OperateDT"].ToString().Trim();
                            this.grid1.Cell(this.grid1.Rows - 1, 1).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 2).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 3).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 4).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 5).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 6).ForeColor = Color.Red;
                            this.grid1.Cell(this.grid1.Rows - 1, 7).ForeColor = Color.Red;
                        }
                    }
                }
                if (this.grid1.Rows > 1)
                {
                    this.grid1.Range(this.grid1.Rows - 1, 1, this.grid1.Rows - 1, this.grid1.Cols - 1).SelectCells();
                }
                newFrm.Dispose();
                newFrm = null;
                grid1.AutoRedraw = true;
                grid1.Refresh();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TSB_Print_Click(object sender, EventArgs e)
        {
            if (this.grid1.Rows <= 1)
            {
                MessageBox.Show("无数据,不能打印!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            try
            {
                bool bl_1 = false; ;
                int i = 0;
                System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
                System.Drawing.Printing.PaperSize paperSize = new System.Drawing.Printing.PaperSize();
                //使用同样的打印机
                if (this.grid1.PageSetup.PrinterName.Trim().Length > 0)
                {
                    pd.PrinterSettings.PrinterName = this.grid1.PageSetup.PrinterName;
                }
                //取得匹配的纸张大小
                if (pd.PrinterSettings.IsValid)
                {
                    for (i = 0; i < pd.PrinterSettings.PaperSizes.Count - 1; i++)
                    {
                        //以下程序把纸张大小设置为A4
                        if (pd.PrinterSettings.PaperSizes[i].Kind == System.Drawing.Printing.PaperKind.A4)
                        {
                            this.grid1.PageSetup.PaperSize = pd.PrinterSettings.PaperSizes[i];
                            pd.Dispose();
                            bl_1 = true;
                            break;
                        }
                    }
                }
                pd.Dispose();
                if (bl_1 == false)
                {
                    MessageBox.Show("系统检测不到A4类型纸张,无法打印!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    this.grid1.ReportTitles.Clear();
                    string strTitle1 = "(" + Convert.ToDateTime(strDt_Start).ToShortDateString() + "至" + Convert.ToDateTime(strDt_End).ToShortDateString() + ")";
                    this.grid1.ReportTitles.Add(new FlexCell.ReportTitle(strTitle1, new Font(FontFamily.GenericSansSerif, 10), HorizontalAlignment.Center, Color.Black));
                    this.grid1.PageSetup.CenterHeader = LoginFrm.strEnduserName.Trim() + "----系统操作日志";
                    this.grid1.PageSetup.CenterHeaderFont = new Font(FontFamily.GenericSansSerif, 18, FontStyle.Bold);
                    this.grid1.PageSetup.PrintFixedColumn = true;//打印固定列
                    this.grid1.PageSetup.PrintFixedRow = true; //打印固定行
                    this.grid1.PageSetup.PrintGridLines = true;//打印网络线
                    this.grid1.PageSetup.CenterHorizontally = true;//水平居中
                    this.grid1.PageSetup.Landscape = true;//横向打印
                    this.grid1.PageSetup.BlackAndWhite = true;
                    this.grid1.PageSetup.LeftMargin = 1;
                    this.grid1.PageSetup.FirstPageNumber = 1;
                    this.grid1.PageSetup.CenterFooter = "&P/&N    &D";
                    this.grid1.Print();
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TSB_ExportToExcel_Click(object sender, EventArgs e)
        {
            if (this.grid1.Rows <= 1)
            {
                MessageBox.Show("无数据,不能导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            string strTitle1 = "(" + Convert.ToDateTime(strDt_Start).ToString("yyyyMMddHHmmss").Substring(0,8) + "至" + Convert.ToDateTime(strDt_End).ToString("yyyyMMddHHmmss").Substring(0,8) + ")";

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "文件另存为";
                sfd.OverwritePrompt = true;
                sfd.CheckPathExists = true;
                sfd.Filter = "*.xls" + "|" + "*.xls";
                sfd.ShowHelp = true;
                sfd.FileName = "系统操作日志" + strTitle1 + ".xls";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string strFileName = sfd.FileName;
                    this.grid1.ExportToExcel(@"" + strFileName, true, true);
                    strFileName = @"" + strFileName;
                    MessageBox.Show("导出成功,导出后的Excel工作薄存放路径及名称为:\n\n" + strFileName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (this.grid1.Rows > 1)
                {
                    this.grid1.Range(this.grid1.Rows - 1, 1, this.grid1.Rows - 1, this.grid1.Cols - 1).SelectCells();
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void TSB_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void D_OperatorLogFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //LoginFrm.FormStatus[24] = 0;
        }

        private void TSI_Refresh_Click(object sender, EventArgs e)
        {
            TSB_Refresh_Click(sender, e);
        }

        private void TSI_Delete_Click(object sender, EventArgs e)
        {
            TSB_Delete_Click(sender, e);
        }

        private void TSI_ChangingLog_Click(object sender, EventArgs e)
        {
            TSB_ChangingLog_Click(sender, e);
        }

        private void TSI_Search_Click(object sender, EventArgs e)
        {
            TSB_Search_Click(sender, e);
        }

        private void cmb_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (blLoad == false)
            {
                return;
            }
            this.cmbox_Class.Items.Clear();
            this.cmbox_Class.Text = "";
            dt_TempSearch.Rows.Clear();
            if (this.cmb_List.Text.Trim() == "操作员")
            {
                strSQL = "select distinct  OperatorName   from  XXCLOUD.dbo.T_OperateLogInf ";
                strSQL += " where   OperateDT between '" + Convert.ToString(this.dtp_Start.Value.AddSeconds(1).AddSeconds(-1)) + " ' and '" + Convert.ToString(this.dtp_End.Value.AddDays(1).AddSeconds(-1)) + "'";
                strSQL += "    order by OperatorName ";
                dt_TempSearch = SQLHelper.DTQuery(strSQL);
                if (dt_TempSearch.Rows.Count > 0)
                {
                    this.cmbox_Class.Items.Add("");
                    for (int i = 0; i < dt_TempSearch.Rows.Count; i++)
                    {
                        if (dt_TempSearch.Rows[i]["OperatorName"].ToString().Trim() != "")
                        {
                            this.cmbox_Class.Items.Add(dt_TempSearch.Rows[i]["OperatorName"].ToString().Trim());
                        }
                    }
                }
            }
            else if (this.cmb_List.Text.Trim() == "所在操作窗体")
            {
                strSQL = "select distinct  FormName   from  XXCLOUD.dbo.T_OperateLogInf ";
                strSQL += " where   OperateDT between '" + Convert.ToString(this.dtp_Start.Value.AddSeconds(1).AddSeconds(-1)) + " ' and '" + Convert.ToString(this.dtp_End.Value.AddDays(1).AddSeconds(-1)) + "'";
                strSQL += "    order by FormName ";
                dt_TempSearch = SQLHelper.DTQuery(strSQL);
                if (dt_TempSearch.Rows.Count > 0)
                {
                    this.cmbox_Class.Items.Add("");
                    for (int i = 0; i < dt_TempSearch.Rows.Count; i++)
                    {
                        if (dt_TempSearch.Rows[i]["FormName"].ToString().Trim() != "")
                        {
                            this.cmbox_Class.Items.Add(dt_TempSearch.Rows[i]["FormName"].ToString().Trim());
                        }
                    }
                }
            }
            else if (this.cmb_List.Text.Trim() == "结果")
            {
                strSQL = "select distinct  Result   from  XXCLOUD.dbo.T_OperateLogInf ";
                strSQL += " where   OperateDT between '" + Convert.ToString(this.dtp_Start.Value.AddSeconds(1).AddSeconds(-1)) + " ' and '" + Convert.ToString(this.dtp_End.Value.AddDays(1).AddSeconds(-1)) + "'";
                strSQL += "    order by Result ";
                dt_TempSearch = SQLHelper.DTQuery(strSQL);
                if (dt_TempSearch.Rows.Count > 0)
                {
                    this.cmbox_Class.Items.Add("");
                    for (int i = 0; i < dt_TempSearch.Rows.Count; i++)
                    {
                        if (dt_TempSearch.Rows[i]["Result"].ToString().Trim() != "")
                        {
                            if (dt_TempSearch.Rows[i]["Result"].ToString().Trim() == "1")
                            {
                                this.cmbox_Class.Items.Add("成功");
                            }
                            else
                            {
                                this.cmbox_Class.Items.Add("失败");
                            }
                        }
                    }
                }
            }
        }

        private void btn_Search_Click(object sender, EventArgs e)
        {
            strSearchContent = "";
            if (this.cmb_List.Text.Trim() == "" || this.cmbox_Class.Text.Trim() == "")
            {
                LoadOperateLog("");
                return;
            }
            if (this.rbtn_Blur.Checked == true)
            {
                #region//模糊查询
                //if (this.cmb_List.Text.Trim() == "门禁人数")
                //{
                //    try
                //    {
                //        double i = Convert.ToDouble(this.cmbox_Class.Text);
                //        i = Math.Round(i, 0);
                //        this.cmbox_Class.Text = i.ToString();
                //    }
                //    catch
                //    {
                //        MessageBox.Show("输入的门禁人数格式不正确，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        this.cmbox_Class.Text = "";
                //        this.cmbox_Class.Focus();
                //        return;
                //    }
                //}
                //else
                //{
                if (SQLHelper.CheckFromInput(this.cmbox_Class.Text.Trim().ToLower()) == false)
                {
                    MessageBox.Show("输入的 " + this.cmb_List.Text + " 中包含非法字符，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.cmbox_Class.Text = "";
                    this.cmbox_Class.Focus();
                    return;
                }
                //}
                string strNR = this.cmbox_Class.Text.Trim().ToUpper();
                if (this.cmb_List.Text.Trim() == "操作员")
                {
                    if (this.cmb_code.Text.Trim() == "LIKE")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + "  like '%" + strNR + "%'";
                    }
                    else if (this.cmb_code.Text.Trim() == "=")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " ='" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == ">")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " >'" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == "<")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " <'" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == ">=")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " >='" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == "<=")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " <='" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == "!=")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " <>'" + strNR + "'";
                    }
                }
                else if (this.cmb_List.Text.Trim() == "所在操作窗体")
                {
                    string strMCode = SQLHelper.HzToPy_SZM(strNR).ToUpper();
                    if (this.cmb_code.Text.Trim() == "LIKE")
                    {
                        byte[] bytes = System.Text.Encoding.Default.GetBytes(this.cmbox_Class.Text.Trim());
                        if (bytes.Length > this.cmbox_Class.Text.Trim().Length)//包含汉字
                        {
                            strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + "  like '%" + strNR + "%' )";
                        }
                        else//全数字或字母
                        {
                            strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + "  like '%" + strNR + "%' ')";
                        }
                    }
                    else if (this.cmb_code.Text.Trim() == "=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " ='" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == ">")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " >'" + strNR + "'  )";
                    }
                    else if (this.cmb_code.Text.Trim() == "<")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <'" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == ">=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " >='" + strNR + "'  )";
                    }
                    else if (this.cmb_code.Text.Trim() == "<=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <='" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == "!=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <>'" + strNR + "'  )";
                    }
                }
                else if (this.cmb_List.Text.Trim() == "结果")
                {

                    if (strNR.Trim() == "成功")
                    {
                        strNR = "1";
                    }
                    else
                    {
                        strNR = "0";
                    }
                    string strMCode = XXCLOUDDLL.MCodeAll(strNR).ToUpper();
                    if (this.cmb_code.Text.Trim() == "LIKE")
                    {
                        byte[] bytes = System.Text.Encoding.Default.GetBytes(this.cmbox_Class.Text.Trim());
                        if (bytes.Length > this.cmbox_Class.Text.Trim().Length)//包含汉字
                        {
                            strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + "  like '%" + strNR + "%' )";
                        }
                        else
                        {
                            strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + "  like '%" + strNR + "%'  )";
                        }
                    }
                    else if (this.cmb_code.Text.Trim() == "=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " ='" + strNR + "'  )";
                    }
                    else if (this.cmb_code.Text.Trim() == ">")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " >'" + strNR + "'  )";
                    }
                    else if (this.cmb_code.Text.Trim() == "<")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <'" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == ">=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " >='" + strNR + "'  )";
                    }
                    else if (this.cmb_code.Text.Trim() == "<=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <='" + strNR + "'  )";
                    }
                    else if (this.cmb_code.Text.Trim() == "!=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <>'" + strNR + "' )";
                    }
                }
                #endregion
            }
            else
            {
                #region//严格查询
                //if (this.cmb_List.Text.Trim() == "门禁人数")
                //{
                //    try
                //    {
                //        double i = Convert.ToDouble(this.cmbox_Class.Text);
                //        i = Math.Round(i, 0);
                //        this.cmbox_Class.Text = i.ToString();
                //    }
                //    catch
                //    {
                //        MessageBox.Show("输入的门禁人数格式不正确，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        this.cmbox_Class.Text = "";
                //        this.cmbox_Class.Focus();
                //        return;
                //    }
                //}
                //else
                //{
                if (SQLHelper.CheckFromInput(this.cmbox_Class.Text.Trim().ToLower()) == false)
                {
                    MessageBox.Show("输入的 " + this.cmb_List.Text + " 中包含非法字符，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.cmbox_Class.Text = "";
                    this.cmbox_Class.Focus();
                    return;
                }
                //}
                string strNR = this.cmbox_Class.Text.Trim().ToUpper();
                if (this.cmb_List.Text.Trim() == "操作员")
                {
                    if (this.cmb_code.Text.Trim() == "LIKE")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + "  like '" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == "=")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " ='" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == ">")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " >'" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == "<")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " <'" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == ">=")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " >='" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == "<=")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " <='" + strNR + "'";
                    }
                    else if (this.cmb_code.Text.Trim() == "!=")
                    {
                        strSearchContent = this.cmb_List.SelectedValue.ToString() + " <>'" + strNR + "'";
                    }
                }
                else if (this.cmb_List.Text.Trim() == "所在操作窗体")
                {
                    string strMCode = SQLHelper.HzToPy_SZM(strNR).ToUpper();
                    if (this.cmb_code.Text.Trim() == "LIKE")
                    {
                        byte[] bytes = System.Text.Encoding.Default.GetBytes(this.cmbox_Class.Text.Trim());
                        if (bytes.Length > this.cmbox_Class.Text.Trim().Length)//包含汉字
                        {
                            strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + "  like '" + strNR + "' )";
                        }
                        else//全数字或字母
                        {
                            strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + "  like '" + strNR + "' )";
                        }
                    }
                    else if (this.cmb_code.Text.Trim() == "=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " ='" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == ">")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " >'" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == "<")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <'" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == ">=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " >='" + strNR + "'  )";
                    }
                    else if (this.cmb_code.Text.Trim() == "<=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <='" + strNR + "'  )";
                    }
                    else if (this.cmb_code.Text.Trim() == "!=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <>'" + strNR + "'  )";
                    }
                }
                else if (this.cmb_List.Text.Trim() == "结果")
                {
                    if (strNR.Trim() == "成功")
                    {
                        strNR = "1";
                    }
                    else
                    {
                        strNR = "0";
                    }
                    string strMCode = XXCLOUDDLL.MCodeAll(strNR).ToUpper();
                    if (this.cmb_code.Text.Trim() == "LIKE")
                    {
                        byte[] bytes = System.Text.Encoding.Default.GetBytes(this.cmbox_Class.Text.Trim());
                        if (bytes.Length > this.cmbox_Class.Text.Trim().Length)//包含汉字
                        {
                            strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + "  like '" + strNR + "' )";
                        }
                        else
                        {
                            strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + "  like '" + strNR + "' )";
                        }
                    }
                    else if (this.cmb_code.Text.Trim() == "=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " ='" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == ">")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " >'" + strNR + "'  )";
                    }
                    else if (this.cmb_code.Text.Trim() == "<")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <'" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == ">=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " >='" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == "<=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <='" + strNR + "' )";
                    }
                    else if (this.cmb_code.Text.Trim() == "!=")
                    {
                        strSearchContent = "(" + this.cmb_List.SelectedValue.ToString() + " <>'" + strNR + "'  )";
                    }
                }
                #endregion
            }
            if (strSearchContent.Trim() == "")
            {
                return;
            }

            LoadOperateLog(" and " + strSearchContent);

        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            this.cmb_List.Text = "";
            this.cmb_code.SelectedIndex = 0;
            this.cmbox_Class.Text = "";
            btn_Search_Click(sender, e);
            this.cmbox_Class.Focus();
        }

        private void cmbox_Class_Enter(object sender, EventArgs e)
        {
            this.cmbox_Class.DropDownStyle = ComboBoxStyle.DropDown;
            if (this.cmb_List.Text.Trim() == "评价结果")
            {
                this.cmb_code.Text = "=";
            }
        }

        private void cmbox_Class_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == 13)
            //{
            //    if (this.cmb_List.Text.Trim() != "" && this.cmbox_Class.Text.Trim() != "")
            //    {
            //        btn_Search_Click(sender, e);
            //    }
            //}
        }

        private void cmbox_Class_Leave(object sender, EventArgs e)
        {
            this.cmbox_Class.DropDownStyle = ComboBoxStyle.Simple;
            this.cmbox_Class.BackColor = Color.White;
            this.cmbox_Class.BackColor = Color.White;
            if (this.cmbox_Class.Text.Trim() == "")
            {
                return;
            }
            if (this.cmb_List.Text.Trim() == "门禁人数")
            {
                try
                {
                    double i = Convert.ToDouble(this.cmbox_Class.Text);
                    i = Math.Round(i, 0);
                    this.cmbox_Class.Text = i.ToString();
                }
                catch
                {
                    MessageBox.Show("输入的门禁人数格式不正确，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.cmbox_Class.Text = "";
                    this.cmbox_Class.Focus();
                }
            }
            else
            {
                if (SQLHelper.CheckFromInput(this.cmbox_Class.Text.Trim().ToLower()) == false)
                {
                    MessageBox.Show("输入的 " + this.cmb_List.Text + " 中包含非法字符，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.cmbox_Class.Text = "";
                    this.cmbox_Class.Focus();
                    return;
                }
            }
        }

        private void cmbox_Class_MouseEnter(object sender, EventArgs e)
        {
            this.cmbox_Class.DropDownStyle = ComboBoxStyle.DropDown;
            if (this.cmb_List.Text.Trim() == "评价结果")
            {
                this.cmb_code.Text = "=";
            }
        }

        private void cmbox_Class_MouseLeave(object sender, EventArgs e)
        {
            this.cmbox_Class.DropDownStyle = ComboBoxStyle.Simple;
        }

        private void cmbox_Class_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_Search_Click(sender, e);
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


    }
}
