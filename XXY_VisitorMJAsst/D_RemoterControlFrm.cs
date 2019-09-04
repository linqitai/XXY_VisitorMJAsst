using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

//添加的命名空间
using System.Net;
using System.Net.Sockets;


using System.Data.SqlClient;
using System.Collections;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Data.OleDb;
using System.Drawing.Printing;
using NoxPlus.Net.Apis;
using System.Diagnostics;
using Microsoft.Win32;
using System.Web.Script.Serialization;


namespace XXY_VisitorMJAsst
{

    public partial class D_RemoterControlFrm : Form
    {
        //使用本机的IP地址

        IPAddress localAddress;
        //监听端口
        private int port = 51888;
        private TcpListener myListener;
        private Service service;
        //连接的用户
        System.Collections.Generic.List<User> userList = new List<User>();
        private bool blExit = false;
        private string strStartWithWindows = "XXY_VisitorMJAsst";
        public static string strEndUserName = "";//使用本软件的单位名称

        public D_RemoterControlFrm()
        {
            InitializeComponent();

            service = new Service(listBox1);//网络通信助手监听

            //D_RemoterControlFrm.CheckForIllegalCrossThreadCalls = false;
            Form.CheckForIllegalCrossThreadCalls = false;


            this.grid_GORecord = grid1;
            setGridCallBack_GORecord = new SetGridCallBack_GORecord(setGrid_GORecord);

            this.grid_AlarmRecord = grid2;
            setGridCallBack_AlarmRecord = new SetGridCallBack_AlarmRecord(setGrid_AlarmRecord);

            this.grid_NoCardRecord = grid3;
            setGridCallBack_NoCardRecord = new SetGridCallBack_NoCardRecord(setGrid_NoCardRecord);

            this.grid_SpecialRecord = grid4;
            setGridCallBack_SpecialRecord = new SetGridCallBack_SpecialRecord(setGrid_SpecialRecord);


        }

        AccessMan accessMan;
     public static   XXY_VisitorMJAsst.AcsTcpClass.TOnEventHandler eventHandler;//监听回调方法
     public static XXY_VisitorMJAsst.AcsTcpClass.TOnStatusHandler statusHandler;//监听回调方法
     public static XXY_VisitorMJAsst.AcsTcpClass.TOnDisconnect disconnect;//监听回调方法
     public static XXY_VisitorMJAsst.AcsTcpClass.TOnDataDebug dataDebug;//监听回调方法

        

        private FlexCell.Grid grid_GORecord;//1.声明委托中使用到的控件
        private FlexCell.Grid grid_AlarmRecord;//1.声明委托中使用到的控件
        private FlexCell.Grid grid_NoCardRecord;//1.声明委托中使用到的控件
        private FlexCell.Grid grid_SpecialRecord;//1.声明委托中使用到的控件

        private delegate void SetGridCallBack_GORecord(string str);//2.定义委托：使用关键字delegate
        private SetGridCallBack_GORecord setGridCallBack_GORecord;//3.声明委托

        private delegate void SetGridCallBack_AlarmRecord(string str);//2.定义委托：使用关键字delegate
        private SetGridCallBack_AlarmRecord setGridCallBack_AlarmRecord;//3.声明委托

        private delegate void SetGridCallBack_NoCardRecord(string str);//2.定义委托：使用关键字delegate
        private SetGridCallBack_NoCardRecord setGridCallBack_NoCardRecord;//3.声明委托

        private delegate void SetGridCallBack_SpecialRecord(string str);//2.定义委托：使用关键字delegate
        private SetGridCallBack_SpecialRecord setGridCallBack_SpecialRecord;//3.声明委托

        private string strSQL = "";
        private string strSQL_Temp1 = "";
        private DataTable myTable = new DataTable();
        private DataTable dt_TempSearch = new DataTable();
        private SQLHelper SQLHelper = new SQLHelper();
        private SQLHelper_Remote SQLHelper_Remote = new SQLHelper_Remote();
        private XXCLOUDDLL XXCLOUDDLL =new XXCLOUDDLL();
        private string strAId = "";//区域编号
        private int iRow = 0, iRow1 = 0, iRow2 = 0, iRow3 = 0;
        public static string strPath_MJSound = Application.StartupPath + "\\Sound\\";//门禁声音
        public static string strT_VisitorAccessInf = "",strT_MJRecordAccessInf="";

        private DataTable dtDoor = new DataTable();//门信息
        private DataTable dtMachine = new DataTable();//控制器连接信息
        private string strImageName_SucLinked = Guid.NewGuid().ToString();
        private string strImageName_OpendLinked = Guid.NewGuid().ToString() + "1";
        private string strImageName_ErrorLinked = Guid.NewGuid().ToString() + "1234";

        private int iDisplayDoorCountPRow = 9;//每行显示9个门
        private int iRow_Sum = 0, iCol_Sum = 0, iDoor_Sum = 0;//总行数，列数和门数

        private string[] ASetings = new string[10];//用于分离连接设备的字符串
        private LBDoorControlDLL.AccessV2 accessV2;     //创建设备对象
        private int listenerHandle = 0;  //监听器的句柄

        private DataTable DoorTable = new DataTable();//门信息
        private DataTable HeadReadTable = new DataTable();//读头信息

        private bool blRealTimeCollected = false;//false:实时监控 true:实时采集
        private string strRecordType = "0";//记录类型
        private double dRecord_Sum = 0;//总记录数
        private double dGORecord_Count = 0;//进出记录
        private double dAlarmRecord_Count = 0;//报警记录
        private double dNoCardRecord_Count = 0;//无卡记录
        private double dSpecialRecord_Count = 0;//特殊记录
        private double dNoDefine_Count = 0;//未定义记录
        private string strMName = "", strDName = "", strReadHeadNote = "", strMachineId = "";
        private StringBuilder SBder = new StringBuilder();
        private int iCountExecute = 0;
        private DataTable myCardTable_Tem = new DataTable();//访客证
        private DataTable dtCardSerialNo = new DataTable();//门禁卡号+卡面编号
        private DataTable myCardTable_Con = new DataTable();//临时卡
        private DataTable myCardTable_Mobile = new DataTable();//来自手执机端签离的访客证
        private DataTable myCardTable_GO = new DataTable();//来自手机端签离的表
        private DataTable myTable_Order = new DataTable();//来自手机端签离的表
        private int[] KeyHandle = new int[8];//用于单机狗Nox+
        private int[] KeyNumber = new int[1];//用于单机狗Nox+
        private string strFWQIP = "";
        private DataView DV;
        private DataTable dtDelPower = new DataTable();
        public static string strLoginFrmSelectFlag = "";//" and (VPlace='" + "广东电信广场" + "' or VPlace='" + "" + "' or VPlace is null )  ";

        private DateTime dtStart = System.DateTime.Now;
        private string strRunningTime = "";//运行时长
        private DataTable dtMSSend = new DataTable();//发送手机验证码
        private DataTable dtMSSend_OrderCode = new DataTable();//发送预约码
        private string strSQLMSSend = string.Empty;
        private string strSQLMSSend_OrderCode = string.Empty;
        private DataTable dt_VIPCard = new DataTable();
        private string strNote = "";
        private string strSName = "";
        private string strSNo = "";
        private string strSActualNo = "";
        private string strFloor = "";
        private string strSDDetailName = "";
        private string strApiTypeName = "";
        private string strVisitorType = "";
        private string strGONo = "";
        private string strSIdNo = "";
        private string strSSex = "";

        string strBarCode = "";
        int iBarCode = 0;
        string strRNo = "";
        private StringBuilder sqlList = new StringBuilder();

        private string strYFFaceMachineSum="1";//宇泛人脸机数量
        private string strAPMJMachineSum="1";//奥普控制器数量


        #region//单机加密锁
        private void LockEncryption_Single()
        {
            try
            {
                int Rtn = 11;
                int APPID = Convert.ToInt32("FFFFFFFF", 16);//应用程序标识由工具生成，设号工具为16进制
                //查询加密锁
                Rtn = NoxPlusAPI.NoxFind((int)APPID, KeyHandle, KeyNumber);
                //打开加密锁 操作员密码由工具生成
                string uPin = "12c9d3666676c3456971cea84ec21ad9";
                if (NoxPlusAPI.NoxOpen(KeyHandle[0], uPin) != 0)
                {
                    LoginFrm.iFlag_LockEncryption = 2;
                    return;
                }

                //写内存区数据
                string WMData = "XXCLOUD";
                if (NoxPlusAPI.NoxWriteMem(KeyHandle[0], WMData) != 0)
                {
                    LoginFrm.iFlag_LockEncryption = 2;
                    return;
                }
                LoginFrm.iFlag_LockEncryption = 0;
            }
            catch
            {
                LoginFrm.iFlag_LockEncryption = 2;
            }
        }
        #endregion

        #region//网络通信助手
        //接收客户端连接
        private void ListenClientConnect()
        {
            while (true)
            {
                TcpClient newClient = null;
                try
                {
                    //等待用户进入
                    newClient = myListener.AcceptTcpClient();
                }
                catch
                {
                    //当单击"停止监听"或者退出此窗体时，AcceptTcpClient()会产生异常
                    //可以利用此异常退出循环
                    break;
                }

                //每接受一个客户端连接，就创建一个对应的线程循环接收该客户端发来的信息
                ParameterizedThreadStart pts = new ParameterizedThreadStart(ReceiveData);
                Thread threadReceive = new Thread(pts);
                User user = new User(newClient);
                threadReceive.Start(user);
                userList.Add(user);
                service.SetListBox(string.Format("{0}进入", newClient.Client.RemoteEndPoint));
                service.SetListBox(string.Format("当前连接客户端数:{0}", userList.Count));

            }
        }

        //接收、处理客户端信息，每客户1个线程，参数用于区分是哪个客户
        private void ReceiveData(object obj)
        {
            User user = (User)obj;
            TcpClient client = user.client;
            //是否正常退出接收线程
            bool normalExit = false;
            //用于控制是否退出循环
            bool exitWhile = false;
            while (exitWhile == false)
            {
                string receiveString = null;
                try
                {
                    receiveString = user.sr.ReadLine();
                }
                catch
                {
                    //该客户底层套接字不存在时会出现异常
                    service.SetListBox("接收数据失败");
                }
                //TcpClient对象将套接字进行了封装，如果TcpClient对象关闭了
                //底层套接字并不养老，也不产生异常，但是会导致读取的结果为null
                if (receiveString == null)
                {
                    if (normalExit == false)
                    {
                        //如果停止了监听,Connected为false
                        if (client.Connected == true)
                        {
                            service.SetListBox(string.Format("与{0}失去联系，已终止接收该用户信息", client.Client.RemoteEndPoint));

                        }
                    }
                    //退出循环
                    break;
                }
                service.SetListBox(string.Format("来自{0}:{1}", user.userName, receiveString));
                string[] splitString = receiveString.Split(',');
                switch (splitString[0])
                {
                    case "Login":
                        //格式：Login,昵称
                        //将用户昵称保存到用户列表中
                        //由于是引用类型，因此直接给user赋值也就是给userList中对应的user赋值
                        //用户名中包含其IP和端口的目的是为了帮助理解，实际游戏中一般不会显示IP的
                        user.userName = string.Format("[{0}--{1}]", splitString[1], client.Client.RemoteEndPoint);
                        //允许该用户进入智能访客登记管理系统网络版，即将各桌是否有人情况发送给该用户
                        this.TSL2.Text = "  当前连接客户端数: " + userList.Count.ToString();
                        break;
                    case "Register":
                        //格式:Register,昵称
                        //用户进行了访客登记操作
                        service.SetListBox(string.Format("[{0}--{1}]进行了访客登记操作", user.userName, client.Client.RemoteEndPoint));
                        service.SendToAll(userList, "Register," + client.Client.RemoteEndPoint.ToString());
                        break;
                    case "Leave":
                        //格式:Leave,昵称
                        //用户进行了访客签离
                        service.SetListBox(string.Format("[{0}--{1}]进行了访客签离操作", user.userName, client.Client.RemoteEndPoint));
                        service.SendToAll(userList, "Leave," + client.Client.RemoteEndPoint.ToString());
                        break;
                    case "GONo":
                        //格式:JCBH,昵称
                        //用户进行了红外条码访客签离操作
                        service.SetListBox(string.Format("[{0}--{1}]进行了红外条码访客签离操作", user.userName, client.Client.RemoteEndPoint));
                        service.SendToAll(userList, "GONo," + client.Client.RemoteEndPoint.ToString());
                        break;
                    case "VName":
                        //格式:FKXM,昵称
                        //用户进行了红外条码访客签离操作
                        service.SetListBox(string.Format("[{0}--{1}]进行了红外条码访客签离操作", user.userName, client.Client.RemoteEndPoint));
                        service.SendToAll(userList, "VName," + client.Client.RemoteEndPoint.ToString());
                        break;
                    case "Logout":
                        //格式:Logout
                        //用户退出智能访客登记管理系统网络版
                        service.SetListBox(string.Format("[{0}--{1}]退出智能访客登记管理系统网络版", user.userName, client.Client.RemoteEndPoint));
                        normalExit = true;
                        exitWhile = true;
                        break;
                    case "MSSend":
                        //格式:Logout
                        //用户退出智能访客登记管理系统网络版
                        service.SetListBox(string.Format("[{0}--{1}访客登记机请求发送短信", user.userName, client.Client.RemoteEndPoint));
                        service.SendToAll(userList, "MSSend," + client.Client.RemoteEndPoint.ToString());
                        //dtMSSend.Rows.Clear();
                        //strSQLMSSend = "select * from XXCLOUD.dbo.T_MessageSendInf  where (Flag ='" + "0" + "' or  Flag is null )";
                        //dtMSSend = SQLHelper.DTQuery(strSQLMSSend);
                        //if (dtMSSend.Rows.Count > 0)
                        //{
                        //    for (int i = 0; i < dtMSSend.Rows.Count; i++)
                        //    {
                        //        string strMes = "[" + LoginFrm.strEnduserName + "]验证码:" + dtMSSend.Rows[i]["OrderCode"].ToString().Trim() + "(泄露有风险)，您正在进行访客自助登记，请于2分钟内填写，如非本人操作，请忽略本短信。";
                        //        if (XXCLOUDDLL.SendMesByPhoneAndContext(dtMSSend.Rows[i]["VPhone"].ToString().Trim(), strMes) == true)
                        //        {
                        //            Thread.Sleep(500);
                        //            strSQLMSSend = "update  XXCLOUD.dbo.T_MessageSendInf set SendDT ='" + System.DateTime.Now.ToString() + "',Flag='" + "1" + "' where Id='" + dtMSSend.Rows[i]["Id"].ToString().Trim() + "'";
                        //            if (SQLHelper.ExecuteSql(strSQLMSSend) != 0)
                        //            {
                        //                OperatorLog("服务器向访客" + dtMSSend.Rows[i]["VName"].ToString().Trim() + "[" + dtMSSend.Rows[i]["VPhone"].ToString().Trim() + "]发送手机验证短信成功", "1");
                        //            }
                        //        }
                        //        else
                        //        {
                        //            // MessageBox.Show("短信发送失败，无法继续登记，请联系设备管理员！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //            OperatorLog("服务器向访客" + dtMSSend.Rows[i]["VName"].ToString().Trim() + "[" + dtMSSend.Rows[i]["VPhone"].ToString().Trim() + "]发送手机验证短信失败", "0");
                        //        }
                        //    }
                        //}

                        break;
                    default:
                        service.SendToAll(userList, receiveString);
                        break;

                }
            }
            userList.Remove(user);
            client.Close();
            service.SetListBox(string.Format("有一个退出,剩余连接用户数：{0}", userList.Count));
            this.TSL2.Text = "  当前连接客户端数: " + userList.Count.ToString();
        }
        #endregion

        #region//grid_GORecord单击事件
        private void grid_GORecord_Click()
        {
            try
            {
                this.grid_GORecord.AutoRedraw = false;
                if (iRow >= this.grid_GORecord.Rows)
                {
                    iRow = this.grid_GORecord.Rows - 1;
                }
                for (int i = 1; i < this.grid_GORecord.Cols; i++)
                {
                    if (iRow != 0)
                    {
                        this.grid_GORecord.Cell(iRow, i).ForeColor = Color.Blue;
                    }
                    if (this.grid_GORecord.Selection.FirstRow != 0)
                    {
                        this.grid_GORecord.Cell(this.grid_GORecord.Selection.FirstRow, i).ForeColor = Color.Red;
                    }

                }
                iRow = this.grid_GORecord.Selection.FirstRow;
                this.grid_GORecord.AutoRedraw = true;
                this.grid_GORecord.Refresh();
            }
            catch
            {
                ExecuteSql();
                this.grid_GORecord.AutoRedraw = true;
                this.grid_GORecord.Refresh();
                return;
            }
        }
        #endregion

        #region//grid_AlarmRecord单击事件
        private void grid_AlarmRecord_Click()
        {
            try
            {
                this.grid_AlarmRecord.AutoRedraw = false;
                if (iRow1 >= this.grid_AlarmRecord.Rows)
                {
                    iRow1 = this.grid_AlarmRecord.Rows - 1;
                }
                for (int i = 1; i < this.grid_AlarmRecord.Cols; i++)
                {
                    if (iRow1 != 0)
                    {
                        this.grid_AlarmRecord.Cell(iRow1, i).ForeColor = Color.Blue;
                    }
                    if (this.grid_AlarmRecord.Selection.FirstRow != 0)
                    {
                        this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Selection.FirstRow, i).ForeColor = Color.Red;
                    }

                }
                iRow1 = this.grid_AlarmRecord.Selection.FirstRow;
                this.grid_AlarmRecord.AutoRedraw = true;
                this.grid_AlarmRecord.Refresh();
            }
            catch
            {
                ExecuteSql();
                this.grid_AlarmRecord.AutoRedraw = true;
                this.grid_AlarmRecord.Refresh();
                return;
            }
        }
        #endregion

        #region//grid_NoCardRecord单击事件
        private void grid_NoCardRecord_Click()
        {
            try
            {
                this.grid_NoCardRecord.AutoRedraw = false;
                if (iRow2 >= this.grid_NoCardRecord.Rows)
                {
                    iRow2 = this.grid_NoCardRecord.Rows - 1;
                }
                for (int i = 1; i < this.grid_NoCardRecord.Cols; i++)
                {
                    if (iRow2 != 0)
                    {
                        this.grid_NoCardRecord.Cell(iRow2, i).ForeColor = Color.Blue;
                    }
                    if (this.grid_NoCardRecord.Selection.FirstRow != 0)
                    {
                        this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Selection.FirstRow, i).ForeColor = Color.Red;
                    }

                }
                iRow2 = this.grid_NoCardRecord.Selection.FirstRow;
                this.grid_NoCardRecord.AutoRedraw = true;
                this.grid_NoCardRecord.Refresh();
            }
            catch
            {
                ExecuteSql();
                this.grid_NoCardRecord.AutoRedraw = true;
                this.grid_NoCardRecord.Refresh();
                return;
            }
        }
        #endregion

        #region//grid_SpecialRecord单击事件
        private void grid_SpecialRecord_Click()
        {
            try
            {
                this.grid_SpecialRecord.AutoRedraw = false;
                if (iRow3 >= this.grid_SpecialRecord.Rows)
                {
                    iRow3 = this.grid_SpecialRecord.Rows - 1;
                }
                for (int i = 1; i < this.grid_SpecialRecord.Cols; i++)
                {
                    if (iRow3 != 0)
                    {
                        this.grid_SpecialRecord.Cell(iRow3, i).ForeColor = Color.Blue;
                    }
                    if (this.grid_SpecialRecord.Selection.FirstRow != 0)
                    {
                        this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Selection.FirstRow, i).ForeColor = Color.Red;
                    }

                }
                iRow3 = this.grid_SpecialRecord.Selection.FirstRow;
                this.grid_SpecialRecord.AutoRedraw = true;
                this.grid_SpecialRecord.Refresh();
            }
            catch
            {
                ExecuteSql();
                this.grid_SpecialRecord.AutoRedraw = true;
                this.grid_SpecialRecord.Refresh();
                return;
            }
        }
        #endregion

        #region//多线程设置grid  //4.实例化委托   5：用做某个方法的参数  6：最后在此方法的实现代码中使用
        public void setGrid_GORecord(string str)
        {
            if (grid_GORecord.InvokeRequired == true)
            {
                grid_GORecord.Invoke(setGridCallBack_GORecord, str);
            }
            else
            {
                // LoadGOInf("");
            }
        }

        public void setGrid_AlarmRecord(string str)
        {
            if (grid_AlarmRecord.InvokeRequired == true)
            {
                grid_AlarmRecord.Invoke(setGridCallBack_AlarmRecord, str);
            }
            else
            {
                // LoadGOInf("");
            }
        }

        public void setGrid_NoCardRecord(string str)
        {
            if (grid_NoCardRecord.InvokeRequired == true)
            {
                grid_NoCardRecord.Invoke(setGridCallBack_NoCardRecord, str);
            }
            else
            {
                // LoadGOInf("");
            }
        }

        public void setGrid_SpecialRecord(string str)
        {
            if (grid_SpecialRecord.InvokeRequired == true)
            {
                grid_SpecialRecord.Invoke(setGridCallBack_SpecialRecord, str);
            }
            else
            {
                // LoadGOInf("");
            }
        }
        #endregion

        #region//根据区域编号得到门信息和控制器连接信息
        private void LoadDoorInf()
        {
            try
            {
                string strLinkText = "";//连接文本
                iRow_Sum = 0; iCol_Sum = 0; iDoor_Sum = 0;
                this.grid_Door.AutoRedraw = false;
                this.grid_Door.Rows = 1;
                this.grid_Door.Cols = 1;
                dtDoor.Rows.Clear();
                dtMachine.Rows.Clear();
                myTable.Rows.Clear();
                if (this.TV_Area.SelectedNode.Name != "0")
                {
                    strSQL = " select T_MJAPAreaCategroyInf.AId AId,T_MJAPAreaCategroyInf.ADetailName ADetailName, T_MJAPDoorInf.MachineId  MachineId ,T_MJAPDoorInf.DoorId DoorId,";
                    strSQL += " T_MJAPDoorInf.DName DName, T_MJAPMachineInf.MSNo,T_MJAPMachineInf.MIPAddress,T_MJAPMachineInf.MCommType,T_MJAPMachineInf.MCommPort,T_MJAPMachineInf.MCommPwd ,T_MJAPMachineInf.MName MName  ";
                    strSQL += " from T_MJAPAreaCategroyInf,T_MJAPMachineInf,T_MJAPDoorInf ";
                    strSQL += " where T_MJAPAreaCategroyInf.AId=T_MJAPMachineInf.ADId and T_MJAPMachineInf.MachineId =T_MJAPDoorInf.MachineId and  T_MJAPAreaCategroyInf.AId = '" + this.TV_Area.SelectedNode.Name + "' ";
                    strSQL += " order by   T_MJAPDoorInf.MachineId,T_MJAPDoorInf.DName  ";
                }
                else
                {
                    //全部区域
                    strSQL = " select T_MJAPAreaCategroyInf.AId AId,T_MJAPAreaCategroyInf.ADetailName ADetailName, T_MJAPDoorInf.MachineId  MachineId ,T_MJAPDoorInf.DoorId DoorId,";
                    strSQL += " T_MJAPDoorInf.DName DName, T_MJAPMachineInf.MSNo,T_MJAPMachineInf.MIPAddress,T_MJAPMachineInf.MCommType,T_MJAPMachineInf.MCommPort,T_MJAPMachineInf.MCommPwd ,T_MJAPMachineInf.MName MName ";
                    strSQL += " from T_MJAPAreaCategroyInf,T_MJAPMachineInf,T_MJAPDoorInf ";
                    strSQL += " where T_MJAPAreaCategroyInf.AId=T_MJAPMachineInf.ADId and T_MJAPMachineInf.MachineId =T_MJAPDoorInf.MachineId   ";
                    strSQL += " order by   T_MJAPDoorInf.MachineId,T_MJAPDoorInf.DName  ";
                }
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    iDoor_Sum = myTable.Rows.Count;//保存总门数
                    int iCount = 0;
                    int iCol = 1;
                    int iRow = 1;
                    this.grid_Door.Rows = this.grid_Door.Rows + 2;
                    iRow = iRow + 2;
                    this.grid_Door.Row(this.grid_Door.Rows - 2).Height = 80;
                    this.grid_Door.Row(this.grid_Door.Rows - 1).Height = 25;
                    for (int i = 0; i < myTable.Rows.Count; i++)
                    {
                        iCount++;
                        if (iCount > iDisplayDoorCountPRow)//每行显示9个门
                        {
                            this.grid_Door.Rows = this.grid_Door.Rows + 2;
                            this.grid_Door.Row(this.grid_Door.Rows - 2).Height = 80;
                            this.grid_Door.Row(this.grid_Door.Rows - 1).Height = 25;
                            iRow = iRow + 2;
                            iCount = 0;
                            iCol = 1;
                        }
                        if (this.grid_Door.Cols < iDisplayDoorCountPRow * 2)
                        {
                            this.grid_Door.Cols = this.grid_Door.Cols + 2;
                            this.grid_Door.Column(this.grid_Door.Cols - 2).Width = 40;
                            this.grid_Door.Column(this.grid_Door.Cols - 1).Width = 80;
                        }
                        iCol = iCol + 2;


                        strLinkText = myTable.Rows[i]["MIPAddress"].ToString().Trim() + "," + myTable.Rows[i]["MCommPort"].ToString().Trim() + ",";
                        strLinkText += myTable.Rows[i]["MCommPwd"].ToString().Trim() + "," + myTable.Rows[i]["MName"].ToString().Trim() + ",";
                        strLinkText += myTable.Rows[i]["DName"].ToString().Trim() + "," + myTable.Rows[i]["MSNo"].ToString().Trim() + ",";

                        this.grid_Door.Cell(this.grid_Door.Rows - 2, iCol - 2).CellType = FlexCell.CellTypeEnum.CheckBox;//设置第一列为复选框模式
                        this.grid_Door.Cell(this.grid_Door.Rows - 2, iCol - 2).Tag = strLinkText;//保存连接字符串

                        //this.grid_Door.Range(this.grid_Door.Rows - 2, iCol - 2, this.grid_Door.Rows - 2, iCol - 2).set_Borders(FlexCell.EdgeEnum.Bottom, FlexCell.LineStyleEnum.None);
                        this.grid_Door.Cell(this.grid_Door.Rows - 2, iCol - 1).SetImage(strImageName_SucLinked);
                        this.grid_Door.Cell(this.grid_Door.Rows - 2, iCol - 1).Tag = "CzJDooR" + myTable.Rows[i]["DoorId"].ToString().Trim();//标记:用于开关门

                        this.grid_Door.Cell(this.grid_Door.Rows - 2, iCol - 1).Alignment = FlexCell.AlignmentEnum.CenterCenter;//图片居中显示
                        this.grid_Door.Range(this.grid_Door.Rows - 1, iCol - 2, this.grid_Door.Rows - 1, iCol - 1).Merge();
                        this.grid_Door.Cell(this.grid_Door.Rows - 1, iCol - 2).Text = myTable.Rows[i]["DName"].ToString().Trim();
                        this.grid_Door.Cell(this.grid_Door.Rows - 1, iCol - 2).Locked = true;
                        this.grid_Door.Cell(this.grid_Door.Rows - 1, iCol - 2).Alignment = FlexCell.AlignmentEnum.CenterCenter;
                    }
                    iRow_Sum = this.grid_Door.Rows;//保存总行数
                    iCol_Sum = this.grid_Door.Cols;//保存总列数

                    for (int i = 1; i < iRow_Sum; i = i + 2)
                    {
                        for (int j = 1; j < iCol_Sum; j = j + 2)
                        {
                            if (this.grid_Door.Cell(i, j).Tag != "")
                            {
                                this.grid_Door.Cell(i, j).Text = "1";
                            }
                        }
                    }
                }
                else
                {
                    //没有控制器及其门信息
                }
                this.grid_Door.AutoRedraw = true;
                this.grid_Door.Refresh();
            }
            catch (Exception exp)
            {
                ExecuteSql();
                iRow_Sum = 0; iCol_Sum = 0; iDoor_Sum = 0;
                this.grid_Door.Rows = 1;
                this.grid_Door.Cols = 1;
                this.grid_Door.AutoRedraw = true;
                this.grid_Door.Refresh();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        #endregion

        #region//实时采集
        private void ExecuteSql()
        {
            if (SBder.Length != 0)
            {
                if (SQLHelper.ExecuteNonQueryTran(SBder.ToString().TrimEnd(';').Split(';')) == true)
                {
                    iCountExecute = 0;
                    SBder.Length = 0;
                }
            }
        }
        #endregion

        #region//关闭监听器
        /// <summary>
        /// 关闭监听器
        /// </summary>
        private void closeAccesses()
        {
            //if (TSB_RealTimeCollection.Text == "停止监控")
            //{
            //    TSB_RealTimeCollection.Text = "实时监控";
            //    TSB_RealTimeCollection.Image = XXY_VisitorMJAsst.Properties.Resources.StartMonitoring;
            //    TSB_RealTimeCollection.Visible = true;
            //}
            //else
            //{
            //    TSB_RealTimeCollection.Text = "实时采集";
            //    TSB_RealTimeCollection.Image = XXY_VisitorMJAsst.Properties.Resources.StartCollect;
            //    TSB_RealTimeCollection.Visible = true;
            //}
            try
            {
                accessMan.closeAccess();
            }
            catch (Exception e)
            {
                ExecuteSql();
                MessageBox.Show(e.Message);
            }
        }
        #endregion

        #region//开启监听
        private void openAccessListener()
        {
            blRealTimeCollected = false;
            #region//开启实时监控
            if (iRow_Sum <= 1)
            {
                MessageBox.Show("没有门信息，无法监控!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            bool blSelected = false;//是否有选中
            for (int i = 1; i < iRow_Sum; i = i + 2)
            {
                for (int j = 1; j < iCol_Sum; j = j + 2)
                {
                    if (this.grid_Door.Cell(i, j).Text == "1")
                    {
                        blSelected = true;
                        break;
                    }
                }
            }
            if (blSelected == false)
            {
                MessageBox.Show("请先选择要监控的门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            for (int i = 0; i < ASetings.Length; i++)
            {
                ASetings[i] = "0";
            }
            int iCount = 0;
            string strLinkTextTemp = "";//临时的连接字符串

            for (int i = 1; i < iRow_Sum; i = i + 2)
            {
                for (int j = 1; j < iCol_Sum; j = j + 2)
                {
                    for (int n = 0; n < ASetings.Length; n++)
                    {
                        ASetings[n] = "0";
                    }
                    if (this.grid_Door.Cell(i, j).Text == "1")
                    {
                        if (strLinkTextTemp != this.grid_Door.Cell(i, j).Tag)
                        {
                            strLinkTextTemp = this.grid_Door.Cell(i, j).Tag;
                            iCount = 0;
                            strSQL = "";
                            for (int n = 0; n < this.grid_Door.Cell(i, j).Tag.Length; n++)
                            {
                                if (this.grid_Door.Cell(i, j).Tag.Substring(n, 1).ToString() != ",")
                                {
                                    strSQL += this.grid_Door.Cell(i, j).Tag.Substring(n, 1).ToString().Trim();
                                }
                                else
                                {
                                    ASetings[iCount++] = strSQL;
                                    strSQL = "";
                                }
                            }
                        }

                    }
                }
            }
            //if (blRealTimeCollected == false)
            //{
            //    TSB_RealTimeMonitoring.Text = "停止监控";
            //    TSB_RealTimeMonitoring.Image = XXY_VisitorMJAsst.Properties.Resources.StopControl;
            //    TSB_RealTimeCollection.Visible = false;
            //}
            //else
            //{
            //    TSB_RealTimeCollection.Text = "停止监控";
            //    TSB_RealTimeCollection.Image = XXY_VisitorMJAsst.Properties.Resources.StopControl;
            //    TSB_RealTimeCollection.Visible = false;
            //}
            #endregion
        }
        #endregion

        private void InsertIntoGoRegisiterInf(DataTable para_dtConTemCardInf, string para_strEnterOrLeave, string para_strVCardNo)
        {
            if (para_dtConTemCardInf.Rows.Count <= 0)
            {
                return;
            }
            strRNo = XXCLOUDDLL.PRegisterFrm_RigisterNo(strT_VisitorAccessInf);//登记单号
            #region//2.产生条码图案
            strBarCode = "";


            iBarCode++;
            if (iBarCode > 9)
            {
                iBarCode = 0;
            }
            strBarCode = DateTime.Now.ToString("yyyyMMddHHmmss").Substring(8) + iBarCode.ToString();

            #endregion
            string strIsWPOrBP = "";//需要判断是否黑名单
            string strVDetailName = "";//黑名单或白名单所属类别
            string strSQL_Insert = "";
            //VCustomField8：0 未上传， 1已上传
            try
            {
                sqlList.Length = 0;
                if (para_strEnterOrLeave == "进门")
                {
                    #region//根据门禁卡号更新XXCLOUD.dbo.T_CardLostInf表
                    //Flag:0,未及时归还，1：遗失
                    if (para_strVCardNo.Trim() != "")
                    {
                        strSQL_Insert = "delete from XXCLOUD.dbo.T_CardLostInf where VCardNo ='" + para_strVCardNo + "';";
                        sqlList.AppendFormat(strSQL_Insert);
                        strSQL_Insert = "insert into XXCLOUD.dbo.T_CardLostInf(DigitalSignature,VName,VIdNo,VCardNo,VCardSerialNo,RegDT,Flag,VPlace)Values('" + para_dtConTemCardInf.Rows[0]["DigitalSignature"].ToString().Trim() + "',";
                        strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VName"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["VIdNo"].ToString().Trim() + "','" + para_strVCardNo + "','" + para_dtConTemCardInf.Rows[0]["VCardSerialNo"].ToString().Trim() + "',";
                        strSQL_Insert += "'" + System.DateTime.Now.ToString() + "','" + "0" + "','" + para_dtConTemCardInf.Rows[0]["VPlace"].ToString().Trim() + "');";
                        sqlList.AppendFormat(strSQL_Insert);
                        if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == false)
                        {
                        }
                    }
                    #endregion

                    strSQL_Insert = "insert into " + strT_VisitorAccessInf;
                    strSQL_Insert += "(GONo,VNo,VActualNo,VName,VNameMCode,VSex ,VIdNo,";
                    strSQL_Insert += "VVisitReason ,VUnitName ,VPhone, VVisitCount ,VVisitPCount,";
                    strSQL_Insert += " RegDT,RegYear ,RegMonth ,RegDay ,RegDNo,RegDDetailName,";
                    strSQL_Insert += "RegOperatorNo ,RegOperatorActualNo ,RegOperatorName,BarCodeNo,Flag,LeaDDetailName, VCardNo,VCustomField1,VCustomField2,  ";
                    strSQL_Insert += " MachineKey,VCustomField8,  VCardStartValidDT,VCardEndValidDT,";
                    strSQL_Insert += " VPlace,AId, VCardSerialNo,DigitalSignature,BatchVisit ,EnterDoorDT)Values";//分别是本机使用单位代码，本机供应商代码，数字签名及是否已上传到远程服务器(0:未上传 1：已上传)
                    strSQL_Insert += "('" + strRNo + "','" + para_dtConTemCardInf.Rows[0]["VNo"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["VActualNo"].ToString().Trim() + "',";
                    strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VName"].ToString().Trim() + "','" + XXCLOUDDLL.MCodeAll(para_dtConTemCardInf.Rows[0]["VName"].ToString().Trim()) + "',";
                    strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VSex"].ToString().Trim() + "', '" + para_dtConTemCardInf.Rows[0]["VIdNo"].ToString().Trim() + "',";
                    strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VVisitReason"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["VUnitName"].ToString().Trim() + "',";
                    strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VPhone"].ToString().Trim() + "','" + "1" + "','" + para_dtConTemCardInf.Rows[0]["VVisitPCount"].ToString().Trim() + "',";
                    strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["RegDT"].ToString().Trim() + "',";
                    strSQL_Insert += "'" + System.DateTime.Now.Year.ToString() + "','" + System.DateTime.Now.Month.ToString() + "','" + System.DateTime.Now.Day.ToString() + "',";
                    strSQL_Insert += "'" + "0" + "','" + strMName + "闸机" + "','" + LoginFrm.strOperatorNo.Trim() + "',";
                    strSQL_Insert += "'" + LoginFrm.strOperatorActualNo.Trim() + "','" + LoginFrm.strOperatorName.Trim() + "','" + strBarCode + "','" + para_dtConTemCardInf.Rows[0]["Flag"].ToString().Trim() + "','" + "" + "',";
                    strSQL_Insert += " '" + para_dtConTemCardInf.Rows[0]["VCardNo"].ToString().Trim() + "','" + strIsWPOrBP + "','" + strVDetailName + "',";
                    strSQL_Insert += " '" + LoginFrm.strMachineKey + "','" + "0" + "' ,'" + para_dtConTemCardInf.Rows[0]["VCardStartValidDT"].ToString().Trim() + "',";
                    strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VCardEndValidDT"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["VPlace"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["AId"].ToString().Trim() + "',";
                    strSQL_Insert += " '" + para_dtConTemCardInf.Rows[0]["VCardSerialNo"].ToString().Trim() + "', '" + para_dtConTemCardInf.Rows[0]["DigitalSignature"].ToString().Trim() + "',";
                    strSQL_Insert += " '" + "1" + "', '" + System.DateTime.Now.ToString() + "');";
                    SQLHelper.ExecuteSql(strSQL_Insert);
                }
                else
                {
                    //出门:先判断是否已经存在进门的数据，如果有，则更改，如果没有，则插入新数据
                    strSQL_Insert = "select Id from " + strT_VisitorAccessInf + " where VCardNo ='" + para_strVCardNo + "' and LeaDDetailName='" + "" + "'";
                    DataTable dt_Insert = new DataTable();
                    dt_Insert = SQLHelper.DTQuery(strSQL_Insert);
                    if (dt_Insert.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt_Insert.Rows.Count; i++)
                        {
                            if (para_strVCardNo.Trim() != "")
                            {
                                strSQL_Insert = "delete from XXCLOUD.dbo.T_CardLostInf where VCardNo ='" + para_strVCardNo + "';";
                                sqlList.AppendFormat(strSQL_Insert);
                                strSQL_Insert = "insert into XXCLOUD.dbo.T_CardLostInf(DigitalSignature,VName,VIdNo,VCardNo,VCardSerialNo,RegDT,Flag,VPlace)Values('" + para_dtConTemCardInf.Rows[0]["DigitalSignature"].ToString().Trim() + "',";
                                strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VName"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["VIdNo"].ToString().Trim() + "','" + para_strVCardNo + "','" + para_dtConTemCardInf.Rows[0]["VCardSerialNo"].ToString().Trim() + "',";
                                strSQL_Insert += "'" + System.DateTime.Now.ToString() + "','" + "0" + "','" + para_dtConTemCardInf.Rows[0]["VPlace"].ToString().Trim() + "');";
                                sqlList.AppendFormat(strSQL_Insert);
                                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == false)
                                {

                                }
                            }

                            strSQL_Insert = "update " + strT_VisitorAccessInf + " set LeaDT='" + System.DateTime.Now.ToString() + "',LeaDNo='" + "0" + "',LeaDDetailName='" + strMName + "闸机" + "',";
                            strSQL_Insert += "LeaOperatorNo='" + LoginFrm.strOperatorNo.Trim() + "',LeaOperatorActualNo='" + LoginFrm.strOperatorActualNo.Trim() + "',LeaOperatorName='" + LoginFrm.strOperatorName.Trim() + "', ";
                            strSQL_Insert += "LeaveNormal='" + "1" + "' ";
                            strSQL_Insert += " where Id='" + dt_Insert.Rows[i]["Id"].ToString() + "'";
                            SQLHelper.ExecuteSql(strSQL_Insert);
                        }
                    }
                    else
                    {
                        if (para_strVCardNo.Trim() != "")
                        {
                            strSQL_Insert = "delete from XXCLOUD.dbo.T_CardLostInf where VCardNo ='" + para_strVCardNo + "';";
                            sqlList.AppendFormat(strSQL_Insert);
                            strSQL_Insert = "insert into XXCLOUD.dbo.T_CardLostInf(DigitalSignature,VName,VIdNo,VCardNo,VCardSerialNo,RegDT,Flag,VPlace)Values('" + para_dtConTemCardInf.Rows[0]["DigitalSignature"].ToString().Trim() + "',";
                            strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VName"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["VIdNo"].ToString().Trim() + "','" + para_strVCardNo + "','" + para_dtConTemCardInf.Rows[0]["VCardSerialNo"].ToString().Trim() + "',";
                            strSQL_Insert += "'" + System.DateTime.Now.ToString() + "','" + "0" + "','" + para_dtConTemCardInf.Rows[0]["VPlace"].ToString().Trim() + "');";
                            sqlList.AppendFormat(strSQL_Insert);
                            if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == false)
                            {

                            }
                        }

                        strSQL_Insert = "insert into " + strT_VisitorAccessInf;
                        strSQL_Insert += "(GONo,VNo,VActualNo,VName,VNameMCode,VSex ,VIdNo,";
                        strSQL_Insert += "VVisitReason ,VUnitName ,VPhone, VVisitCount ,VVisitPCount,";
                        strSQL_Insert += " LeaDT,RegYear ,RegMonth ,RegDay ,LeaDNo,LeaDDetailName,";
                        strSQL_Insert += "LeaOperatorNo ,LeaOperatorActualNo ,LeaOperatorName,BarCodeNo,Flag,   VCardNo,VCustomField1,VCustomField2,  ";
                        strSQL_Insert += " MachineKey,VCustomField8,  VCardStartValidDT,VCardEndValidDT,";
                        strSQL_Insert += " VPlace,AId, VCardSerialNo,DigitalSignature,BatchVisit,RegDT,LeaveNormal )Values";//分别是本机使用单位代码，本机供应商代码，数字签名及是否已上传到远程服务器(0:未上传 1：已上传)
                        strSQL_Insert += "('" + strRNo + "','" + para_dtConTemCardInf.Rows[0]["VNo"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["VActualNo"].ToString().Trim() + "',";
                        strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VName"].ToString().Trim() + "','" + XXCLOUDDLL.MCodeAll(para_dtConTemCardInf.Rows[0]["VName"].ToString().Trim()) + "',";
                        strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VSex"].ToString().Trim() + "', '" + para_dtConTemCardInf.Rows[0]["VIdNo"].ToString().Trim() + "',";
                        strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VVisitReason"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["VUnitName"].ToString().Trim() + "',";
                        strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VPhone"].ToString().Trim() + "','" + "1" + "','" + para_dtConTemCardInf.Rows[0]["VVisitPCount"].ToString().Trim() + "',";
                        strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["RegDT"].ToString().Trim() + "',";
                        strSQL_Insert += "'" + System.DateTime.Now.Year.ToString() + "','" + System.DateTime.Now.Month.ToString() + "','" + System.DateTime.Now.Day.ToString() + "',";
                        strSQL_Insert += "'" + "0" + "','" + strMName + "闸机" + "','" + LoginFrm.strOperatorNo.Trim() + "',";
                        strSQL_Insert += "'" + LoginFrm.strOperatorActualNo.Trim() + "','" + LoginFrm.strOperatorName.Trim() + "','" + strBarCode + "','" + para_dtConTemCardInf.Rows[0]["Flag"].ToString().Trim() + "' ,";
                        strSQL_Insert += " '" + para_dtConTemCardInf.Rows[0]["VCardNo"].ToString().Trim() + "','" + strIsWPOrBP + "','" + strVDetailName + "',";
                        strSQL_Insert += " '" + LoginFrm.strMachineKey + "','" + "0" + "' ,'" + para_dtConTemCardInf.Rows[0]["VCardStartValidDT"].ToString().Trim() + "',";
                        strSQL_Insert += "'" + para_dtConTemCardInf.Rows[0]["VCardEndValidDT"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["VPlace"].ToString().Trim() + "','" + para_dtConTemCardInf.Rows[0]["AId"].ToString().Trim() + "',";
                        strSQL_Insert += " '" + para_dtConTemCardInf.Rows[0]["VCardSerialNo"].ToString().Trim() + "', '" + para_dtConTemCardInf.Rows[0]["DigitalSignature"].ToString().Trim() + "',";
                        strSQL_Insert += " '" + "1" + "','" + System.DateTime.Now.ToString() + "','" + "1" + "');";
                        SQLHelper.ExecuteSql(strSQL_Insert);
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region//线程调用各个grid
        delegate void SetUIControlsValue(string para_strReader, string para_strCardNo, string para_strDTNow, string para_strWarnCode, string para_strId);
        private string strEnterOrLeave = "进门";

        private void ALLOCationSetUIControlsValue_GORecord(string para_strReader, string para_strCardNo, string para_strDTNow, string para_strWarnCode,string para_strId)
        {
            if (InvokeRequired)
            {
                SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                Invoke(setUIControlsValue, new object[] { para_strReader, para_strCardNo, para_strDTNow, para_strWarnCode, para_strId });
            }
            else
            {
                // PlaySound(strPath_MJSound + "GO.wav");
                //this.tabControl1.SelectedIndex = 0;
                this.grid_GORecord.AutoRedraw = false;
                this.grid_GORecord.Rows++;
                this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 1).Text = strMName;//设备名称
                this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 2).Text = strDName;//门名称
                if (para_strReader == "0")
                {
                    para_strReader = "读头A";
                }
                else if (para_strReader == "1")
                {
                    para_strReader = "读头B";
                }
                this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 3).Text = para_strReader;//读头

                this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 4).Text = para_strCardNo;//卡号
                this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 5).Text = para_strDTNow;//事件时间
                if (para_strReader == "读头A")
                {
                    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 6).Text = "进门";//进出状态
                }
                else
                {
                    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 6).Text = "出门";//进出状态
                }
                //case NET_DATA_TYPE_CARD: return "刷卡";
                //case NET_DATA_TYPE_ORCode: return "二维码";
                //case NET_DATA_TYPE_DATA: return "数据";
                //case NET_DATA_TYPE_PIN: return "密码";
                //case NET_DATA_TYPE_CHINA: return "身份证";
                //case NET_DATA_TYPE_BIGDATA: return "大数据包";
                //case NET_DATA_TYPE_ALARM: return "报警数据";
                //case NET_DATA_TYPE_BASE64: return "BASE64数据";
                ////ublic const byte NET_DATA_TYPE_PC = 4;  // 
                if (para_strId == "Staff")
                {
                    if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "刷卡")
                    {
                        strNote = "员工门禁卡开门";
                        this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "员工门禁卡开门";
                    }
                    else if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "身份证")
                    {
                        this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "员工身份证开门";
                        strNote = "员工身份证开门";
                    }
                }
                else if (para_strId == "Visitor_Card")
                {
                    //if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "刷卡")
                    //{
                    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "访客身份证开门";
                    strNote = "访客身份证开门";
                    //}
                    //else if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "二维码")
                    //{
                    //    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "访客临时二维码开门";
                    //}
                    //else if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "身份证")
                    //{
                    //    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "访客身份证临时开门";
                    //}
                }
                else if (para_strId == "Student")
                {
                    //if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "刷卡")
                    //{
                    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "学生门禁卡开门";
                    strNote = "学生门禁卡开门";
                    //}
                    //else if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "二维码")
                    //{
                    //    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "访客临时二维码开门";
                    //}
                    //else if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "身份证")
                    //{
                    //    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "访客身份证临时开门";
                    //}
                }
                else if (para_strId == "Visitor_BarCode")
                {
                    //if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "刷卡")
                    //{
                    //this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "访客临时门禁卡开门";
                    //}
                    //else if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "二维码")
                    //{
                     this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "访客临时二维码开门";
                     strNote = "访客临时二维码开门";
                    //}
                    //else if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "身份证")
                    //{
                    //    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 7).Text = "访客身份证临时开门";
                    //}
                }
                this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 8).Text = strSName;
                this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 9).Text = strSDDetailName;
                this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, 10).Text = strVisitorType;


                this.grid_GORecord.Row(this.grid_GORecord.Rows - 1).Locked = true;

                for (int j = 1; j < grid_GORecord.Cols; j++)
                {
                    this.grid_GORecord.Cell(this.grid_GORecord.Rows - 1, j).ForeColor = Color.Blue;
                }
                if (this.grid_GORecord.Rows > 1)
                {
                    this.grid_GORecord.Range(this.grid_GORecord.Rows - 1, 1, this.grid_GORecord.Rows - 1, this.grid_GORecord.Cols - 1).SelectCells();
                    grid_GORecord_Click();
                }
                this.grid_GORecord.AutoRedraw = true;
                this.grid_GORecord.Refresh();
                strEnterOrLeave = "进门";
                if (strReadHeadNote == "出门" || para_strReader == "1")
                {
                    strEnterOrLeave = "出门";
                }
                //把T_LongTemCardInf发卡表中的内容插入到T_VisitorAccessInf来访表中
                string strConTemCardInf = "select top 1 * from XXCLOUD.dbo.T_LongTemCardInf where VCardNo ='" + para_strCardNo + "' and BatchImport ='" + "1" + "'   and VCardStatus ='" + "0" + "' and Flag ='" + "1" + "' order by id desc ";
                InsertIntoGoRegisiterInf(SQLHelper.DTQuery(strConTemCardInf), strEnterOrLeave, para_strCardNo);

                if (strReadHeadNote == "出门" || para_strReader == "1")
                {
                    if (para_strId != "-1")
                    {
                        TemCardLogoutByLeave(para_strCardNo, "出门", para_strId);//访客证出门
                    }
                    else
                    {
                        //临时卡出门
                    }
                }

                dRecord_Sum = dGORecord_Count + dAlarmRecord_Count + dNoCardRecord_Count + dSpecialRecord_Count + dNoDefine_Count;
                this.lbl_Msg.Text = "当前共有门禁总记录数：" + dRecord_Sum.ToString() + "条，其中进出记录：" + dGORecord_Count.ToString() + "条，报警记录：" + dAlarmRecord_Count.ToString() + "条，无卡记录：" + dNoCardRecord_Count.ToString() + "条，特殊记录：" + dSpecialRecord_Count.ToString() + "条，未定义记录：" + dNoDefine_Count.ToString() + "条";
                this.TSL1.Text = this.lbl_Msg.Text;
            }
        }

        private void ALLOCationSetUIControlsValue_AlarmRecord(string para_strReader, string para_strCardNo, string para_strDTNow, string para_strWarnCode, string para_strId)
        {
            if (InvokeRequired)
            {
                SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_AlarmRecord);
                Invoke(setUIControlsValue, new object[] { para_strReader, para_strCardNo, para_strDTNow, para_strWarnCode, para_strId });
            }
            else
            {
                // PlaySound(strPath_MJSound + "Alarm.wav");
                // this.tabControl1.SelectedIndex = 1;
                this.grid_AlarmRecord.AutoRedraw = false;
                this.grid_AlarmRecord.Rows++;
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 1).Text = strMName;//设备名称
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 2).Text = strDName;//门名称
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 3).Text = para_strReader;//读头
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 4).Text = para_strCardNo;//卡号
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 5).Text = para_strDTNow;//事件时间
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 6).Text = strReadHeadNote;//进出状态
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 7).Text = LBDoorControlDLL.AccessV2.RecordTypeTochineseContent(para_strWarnCode);//进出状态
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 8).Text = strSName;
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 9).Text = strSDDetailName;
                this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, 10).Text = strVisitorType;
                this.grid_AlarmRecord.Row(this.grid_AlarmRecord.Rows - 1).Locked = true;

                for (int j = 1; j < grid_AlarmRecord.Cols; j++)
                {
                    this.grid_AlarmRecord.Cell(this.grid_AlarmRecord.Rows - 1, j).ForeColor = Color.Blue;
                }
                if (this.grid_AlarmRecord.Rows > 1)
                {
                    this.grid_AlarmRecord.Range(this.grid_AlarmRecord.Rows - 1, 1, this.grid_AlarmRecord.Rows - 1, this.grid_AlarmRecord.Cols - 1).SelectCells();
                    grid_AlarmRecord_Click();
                }
                this.grid_AlarmRecord.AutoRedraw = true;
                this.grid_AlarmRecord.Refresh();
                dRecord_Sum = dGORecord_Count + dAlarmRecord_Count + dNoCardRecord_Count + dSpecialRecord_Count + dNoDefine_Count;
                this.lbl_Msg.Text = "当前共有门禁总记录数：" + dRecord_Sum.ToString() + "条，其中进出记录：" + dGORecord_Count.ToString() + "条，报警记录：" + dAlarmRecord_Count.ToString() + "条，无卡记录：" + dNoCardRecord_Count.ToString() + "条，特殊记录：" + dSpecialRecord_Count.ToString() + "条，未定义记录：" + dNoDefine_Count.ToString() + "条";
                this.TSL1.Text = this.lbl_Msg.Text;
            }
        }

        private void ALLOCationSetUIControlsValue_NoCardRecord(string para_strReader, string para_strCardNo, string para_strDTNow, string para_strWarnCode, string para_strId)
        {
            if (InvokeRequired)
            {
                SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_NoCardRecord);
                Invoke(setUIControlsValue, new object[] { para_strReader, para_strCardNo, para_strDTNow, para_strWarnCode, para_strId });
            }
            else
            {
                // PlaySound(strPath_MJSound + "NoCard.wav");
                //this.tabControl1.SelectedIndex = 2;
                this.grid_NoCardRecord.AutoRedraw = false;
                this.grid_NoCardRecord.Rows++;
                this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 1).Text = strNoCardMName;//设备名称
                this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 2).Text = strNoCardDName;//门名称
                this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 3).Text = para_strReader;//读头
                this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 4).Text = para_strCardNo;//卡号
                this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 5).Text = para_strDTNow;//事件时间
                this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 6).Text = strReadHeadNote;//进出状态
                if (para_strId == "OpenDoor")
                {
                    this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 7).Text = "电脑远程开门";// +D_RemoterControlFrm_AwhileOpenDoor.strSecond + "秒";//进出状态
                }
                else if (para_strId == "CloseDoor")
                {
                    this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 7).Text = "电脑远程关门" ;//进出状态
                }
                this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 8).Text = strSName;
                this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 9).Text = strSDDetailName;
                this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, 10).Text = strVisitorType;
                this.grid_NoCardRecord.Row(this.grid_NoCardRecord.Rows - 1).Locked = true;

                for (int j = 1; j < grid_NoCardRecord.Cols; j++)
                {
                    this.grid_NoCardRecord.Cell(this.grid_NoCardRecord.Rows - 1, j).ForeColor = Color.Blue;
                }
                if (this.grid_NoCardRecord.Rows > 1)
                {
                    this.grid_NoCardRecord.Range(this.grid_NoCardRecord.Rows - 1, 1, this.grid_NoCardRecord.Rows - 1, this.grid_NoCardRecord.Cols - 1).SelectCells();
                    grid_NoCardRecord_Click();
                }
                this.grid_NoCardRecord.AutoRedraw = true;
                this.grid_NoCardRecord.Refresh();
                dRecord_Sum = dGORecord_Count + dAlarmRecord_Count + dNoCardRecord_Count + dSpecialRecord_Count + dNoDefine_Count;
                this.lbl_Msg.Text = "当前共有门禁总记录数：" + dRecord_Sum.ToString() + "条，其中进出记录：" + dGORecord_Count.ToString() + "条，报警记录：" + dAlarmRecord_Count.ToString() + "条，无卡记录：" + dNoCardRecord_Count.ToString() + "条，特殊记录：" + dSpecialRecord_Count.ToString() + "条，未定义记录：" + dNoDefine_Count.ToString() + "条";
                this.TSL1.Text = this.lbl_Msg.Text;
            }
        }

        private DataTable dtS = new DataTable();
        private string strSQLTemp_SpecialRecord = "";
        private string strFlagConTem_SpecialRecord = "";
        private string strFlagMJStaff_SpecialRecord = "";
        private string strFlagConTem_SpecialRecord_OK = "";
        private int iFlag_ApiType2 = 0;
        private void ALLOCationSetUIControlsValue_SpecialRecord(string para_strReader, string para_strCardNo, string para_strDTNow, string para_strWarnCode, string para_strId)
        {
            strSQLTemp_SpecialRecord = "";
            strFlagConTem_SpecialRecord = "";
            strFlagMJStaff_SpecialRecord = "";
            strFlagConTem_SpecialRecord_OK = "";
            iFlag_ApiType2 = 0;
            if (InvokeRequired)
            {
                SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_SpecialRecord);
                Invoke(setUIControlsValue, new object[] { para_strReader, para_strCardNo, para_strDTNow, para_strWarnCode, "0" });
            }
            else
            {
                //PlaySound(strPath_MJSound + "Special.wav");
                //this.tabControl1.SelectedIndex = 3;
                this.grid_SpecialRecord.AutoRedraw = false;
                this.grid_SpecialRecord.Rows++;
                this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 1).Text = strMName;//设备名称
                this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 2).Text = strDName;//门名称
                if (para_strReader == "0")
                {
                    para_strReader = "读头A";
                }
                else if (para_strReader == "1")
                {
                    para_strReader = "读头B";
                }
                this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 3).Text = para_strReader;//读头
                this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 4).Text = para_strCardNo;//卡号
                this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 5).Text = para_strDTNow;//事件时间
                if (para_strReader == "读头A")
                {
                    this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 6).Text = "进门";//进出状态
                }
                else
                {
                    this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 6).Text = "出门";//进出状态
                }
                if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "刷卡" || TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "二维码")
                {
                    this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 7).Text = "非法门禁卡或二维码";
                }
                else if (TcpPackge.EventTypeStr(byte.Parse(para_strWarnCode)) == "身份证")
                {
                    this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 7).Text = "非法身份证";
                }
                this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 8).Text = strSName;
                this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 9).Text = strSDDetailName;
                this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 10).Text = strVisitorType;
                this.grid_SpecialRecord.Row(this.grid_SpecialRecord.Rows - 1).Locked = true;

                for (int j = 1; j < grid_SpecialRecord.Cols; j++)
                {
                    this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, j).ForeColor = Color.Blue;
                }
                if (this.grid_SpecialRecord.Rows > 1)
                {
                    this.grid_SpecialRecord.Range(this.grid_SpecialRecord.Rows - 1, 1, this.grid_SpecialRecord.Rows - 1, this.grid_SpecialRecord.Cols - 1).SelectCells();
                    grid_SpecialRecord_Click();
                }

                 
                strSDDetailName = strFlagConTem_SpecialRecord + "," + strFlagConTem_SpecialRecord_OK+"  " + strFlagMJStaff_SpecialRecord;
                this.grid_SpecialRecord.Cell(this.grid_SpecialRecord.Rows - 1, 9).Text = strSDDetailName;

         
                this.grid_SpecialRecord.AutoRedraw = true;
                this.grid_AlarmRecord.Refresh();
                dRecord_Sum = dGORecord_Count + dAlarmRecord_Count + dNoCardRecord_Count + dSpecialRecord_Count + dNoDefine_Count;
                this.lbl_Msg.Text = "当前共有门禁总记录数：" + dRecord_Sum.ToString() + "条，其中进出记录：" + dGORecord_Count.ToString() + "条，报警记录：" + dAlarmRecord_Count.ToString() + "条，无卡记录：" + dNoCardRecord_Count.ToString() + "条，特殊记录：" + dSpecialRecord_Count.ToString() + "条，未定义记录：" + dNoDefine_Count.ToString() + "条";
                this.TSL1.Text = this.lbl_Msg.Text;
            }
        }

        #endregion

        #region//播放声音
        private void PlaySound(string para_strPath)
        {
            try
            {
                if (File.Exists(para_strPath) == true)
                {
                    //axWindowsMediaPlayer1.URL = para_strPath;
                    //axWindowsMediaPlayer1.Ctlcontrols.play();
                }
            }
            catch
            {
                ExecuteSql();
            }
        }
        #endregion

        private TcpClient client = null;
        private StreamWriter sw;
        private StreamReader sr;
        private Client Client;

        #region//操作日志
        private void OperatorLog(string para_strOperateDescribe, string para_strResult)
        {
            try
            {
                if (LoginFrm.strOperatorNo.Trim().Length > 50)
                {
                    LoginFrm.strOperatorNo = LoginFrm.strOperatorNo.Substring(0, 50);
                }
                if (LoginFrm.strOperatorActualNo.Trim().Length > 10)
                {
                    LoginFrm.strOperatorActualNo = LoginFrm.strOperatorActualNo.Substring(0, 10);
                }
                if (LoginFrm.strOperatorName.Trim().Length > 50)
                {
                    LoginFrm.strOperatorName = LoginFrm.strOperatorName.Substring(0, 50);
                }
                if (para_strOperateDescribe.Trim().Length > 100)
                {
                    para_strOperateDescribe = para_strOperateDescribe.Substring(0, 100);
                }

                string strOperatorLog = "insert into " + LoginFrm.strT_OperateLogInf + " (OperatorNo,OperatorActualNo,OperatorName,";
                strOperatorLog += " FormName,OperateDescribe,Result,OperateDT,Flag,CAccoutNo)values('" + LoginFrm.strOperatorNo + "',";
                strOperatorLog += "'" + LoginFrm.strOperatorActualNo + "','" + LoginFrm.strOperatorName + "','" + "云访客门禁网络通信助手" + "',";
                strOperatorLog += "'" + para_strOperateDescribe + "','" + para_strResult + "','" + System.DateTime.Now.ToString() + "','" + "访客系统" + "',";
                strOperatorLog += "'" + LoginFrm.strCAccout.Substring(1, 4).Trim() + "')";
                SQLHelper.ExecuteSql(strOperatorLog);
            }
            catch (Exception exp)
            {
                ExecuteSql();
                //if (exp.ToString().Trim().Contains("远程主机") == true)
                //{
                //    MessageBox.Show("本机与远程数据库服务器失去连接，系统将自动退出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    Application.Exit();
                //}
                //else
                //{
                //    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
            }
        }
        #endregion

        #region//出门时访客证注销
        private void TemCardLogoutByLeave(string para_strCardNo, string para_strOut, string para_strId)
        {
            //表：XXCLOUD.dbo.T_LongTemCardInf     Flag:1临时卡 2：访客证  VCardStatus：0正常使用 1挂失  2注销   AreaId：区域编号
            //离开时：根据Flag、VCardStatus和AreaId，找出所属区域中所有的设备和下面的门，然后访客证在这里注销卡片权限
            //临时卡卡：由系统自动在夜里00：01:00里进行统一注销。
            //前台发卡时也要注销权限。
            try
            {

                #region

                //bool blDelSuc = false;//false:权限删除失败  true:权限删除成功
                //myCardTable_Tem.Rows.Clear();
                //string  strSQL_Temp = "select  Id, AId from XXCLOUD.dbo.T_LongTemCardInf where Flag='" + "2" + "' and VCardStatus='" + "0" + "' and  VCardNo ='" + para_strCardNo + "' order by Id desc";
                //myCardTable_Tem = SQLHelper.DTQuery(strSQL_Temp);
                //if (myCardTable_Tem.Rows.Count > 0)
                //{
                //    //根据AId来得到设备进行删除
                //    if (myCardTable_Tem.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < myCardTable_Tem.Rows.Count; i++)//一一循环进行删除
                //        {
                //            if (myCardTable_Tem.Rows[i]["AId"].ToString() != "0")//单独区域
                //            {
                //                for (int n = 0; n < DoorTable.Rows.Count; n++)
                //                {
                //                    if (DoorTable.Rows[n]["ADId"].ToString().Trim() == myCardTable_Tem.Rows[i]["AId"].ToString())
                //                    {
                //                        if (para_strCardNo != "")
                //                        {
                //                            blDelSuc = accessMan.RemoveRegister(DoorTable.Rows[n]["MSNo"].ToString().Trim(), byte.Parse(DoorTable.Rows[n]["DoorId"].ToString().Trim()), Convert.ToUInt32(para_strCardNo));
                //                            if (blDelSuc == false)//删除权限失败
                //                            {
                //                                //插入删除权限失败的门禁卡号信息到XXCLOUD.dbo.T_MJCardLogOutInf表中
                //                                InsertCardIntoLogoutDataTable(para_strCardNo, DoorTable.Rows[n]["MSNo"].ToString().Trim(), DoorTable.Rows[n]["MName"].ToString().Trim(), DoorTable.Rows[n]["DoorId"].ToString().Trim(), DoorTable.Rows[n]["DName"].ToString().Trim(), "0", "2");
                //                            }
                //                        }
                //                    }
                //                }
                //                strSQL_Temp = "update T_LongTemCardInf set VCardStatus='" + "2" + "',VCardCNo='" + LoginFrm.strOperatorNo + "',VCardCActualNo='" + LoginFrm.strOperatorActualNo + "',";
                //                strSQL_Temp += "VCardCName='" + LoginFrm.strOperatorName + "',LeaveNormal='" + "1" + "'  where Id='" + myCardTable_Tem.Rows[i]["Id"].ToString() + "' ";
                //                //strSQL_Temp += " and VCardStatus='" + "0" + "'";
                //                SQLHelper.ExecuteSql(strSQL_Temp);
                //                //根据门禁卡号更新XXCLOUD.dbo.T_CardLostInf表
                //                strSQL_Temp = "delete from XXCLOUD.dbo.T_CardLostInf where VCardNo ='" + para_strCardNo + "';";
                //                SQLHelper.ExecuteSql(strSQL_Temp);
                //            }
                //            else//全部区域
                //            {
                //                for (int n = 0; n < DoorTable.Rows.Count; n++)
                //                {
                //                    if (para_strCardNo != "")
                //                    {
                //                        blDelSuc = accessMan.RemoveRegister(DoorTable.Rows[n]["MSNo"].ToString().Trim(), byte.Parse(DoorTable.Rows[n]["DoorId"].ToString().Trim()), Convert.ToUInt32(para_strCardNo));
                //                        if (blDelSuc == false)//删除权限失败
                //                        {
                //                            //插入删除权限失败的门禁卡号信息到XXCLOUD.dbo.T_MJCardLogOutInf表中
                //                            InsertCardIntoLogoutDataTable(para_strCardNo, DoorTable.Rows[n]["MSNo"].ToString().Trim(), DoorTable.Rows[n]["MName"].ToString().Trim(), DoorTable.Rows[n]["DoorId"].ToString().Trim(), DoorTable.Rows[n]["DName"].ToString().Trim(), "0", "2");
                //                        }
                //                    }
                //                }
                //                strSQL_Temp = "update T_LongTemCardInf set VCardStatus='" + "2" + "',VCardCNo='" + LoginFrm.strOperatorNo + "',VCardCActualNo='" + LoginFrm.strOperatorActualNo + "',";
                //                strSQL_Temp += "VCardCName='" + LoginFrm.strOperatorName + "',LeaveNormal='" + "1" + "'  where Id='" + myCardTable_Tem.Rows[i]["Id"].ToString() + "' ";
                //                //strSQL_Temp += " and VCardStatus='" + "0" + "'";
                //                SQLHelper.ExecuteSql(strSQL_Temp);
                //                //根据门禁卡号更新XXCLOUD.dbo.T_CardLostInf表
                //                strSQL_Temp = "delete from XXCLOUD.dbo.T_CardLostInf where VCardNo ='" + para_strCardNo + "';";
                //                SQLHelper.ExecuteSql(strSQL_Temp);
                //            }
                //        }
                //    }
                //}

                //if (para_strOut == "出门")
                //{
                //    strSQL_Temp = "update " + strT_VisitorAccessInf + " set LeaDT='" + System.DateTime.Now.ToString() + "' ,LeaDNo='" + "0" + "',";
                //    strSQL_Temp += "LeaDDetailName='" + "闸机出门自动签离" + "',LeaveNormal='" + "1" + "'  ,LeaOperatorNo='" + LoginFrm.strOperatorNo + "',LeaOperatorActualNo='" + LoginFrm.strOperatorActualNo + "',";
                //    strSQL_Temp += "LeaOperatorName='" + LoginFrm.strOperatorName + "' where VCardNo='" + para_strCardNo + "' and  LeaDDetailName='" + "" + "'";
                //    SQLHelper.ExecuteSql(strSQL_Temp);
                //}
                #endregion
                if (LoginFrm.iFlag_LockEncryption == 1 && LoginFrm.iFlag_OpenMesCommunication == 1)
                {
                    Client.SendToServer("Leave," + Dns.GetHostName());
                }

            }
            catch (Exception exp)
            {
                ExecuteSql();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//用于同步SQL数据库服务器时间
        public class SetSystemDateTime//设置系统日期类
        {
            [DllImportAttribute("Kernel32.dll")]
            public static extern void GetLocalTime(SystemTime st);
            [DllImportAttribute("Kernel32.dll")]
            public static extern void SetLocalTime(SystemTime st);
        }
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public class SystemTime//系统时间类
        {
            public ushort vYear;//年
            public ushort vMonth;//月
            public ushort vDayOfWeek;//星期
            public ushort vDay;//日
            public ushort vHour;//小时
            public ushort vMinute;//分
            public ushort vSecond;//秒
        }
        #endregion

        #region//来自手执机端的访客证注销
        private void TemCardLogoutFromMobile()
        {
            //表：XXCLOUD.dbo.T_LongTemCardInf     Flag:1临时卡 2：访客证  VCardStatus：0正常使用 1挂失  2注销   AreaId：区域编号
            //离开时：根据Flag、VCardStatus和AreaId，找出所属区域中所有的设备和下面的门，然后访客证在这里注销卡片权限
            //临时卡卡：由系统自动在夜里00：01:00里进行统一注销。
            //前台发卡时也要注销权限。
            try
            {
                bool blDelSuc = false;//false:权限删除失败  true:权限删除成功
                string strCardNo = "";
                myCardTable_GO.Rows.Clear();
                string strSQL_Temp = "select Id,VCardNo from " + strT_VisitorAccessInf + " where LeaveFrom='" + "Mobile" + "' and LeaveFromFlag='" + "0" + "'";
                strSQL_Temp += strLoginFrmSelectFlag;
                myCardTable_GO = SQLHelper.DTQuery(strSQL_Temp);
                if (myCardTable_GO.Rows.Count > 0)
                {
                    #region//
                    for (int k = 0; k < myCardTable_GO.Rows.Count; k++)
                    {
                        myCardTable_Mobile.Rows.Clear();
                        strCardNo = myCardTable_GO.Rows[k]["VCardNo"].ToString().Trim();
                        strSQL_Temp = "select top 1 Id, AId,LeaveCount from XXCLOUD.dbo.T_LongTemCardInf where Flag='" + "2" + "' and VCardStatus='" + "0" + "' and  VCardNo ='" + strCardNo + "' ";
                        strSQL_Temp += strLoginFrmSelectFlag;
                        strSQL_Temp += " order by Id desc ";
                        myCardTable_Mobile = SQLHelper.DTQuery(strSQL_Temp);
                        if (myCardTable_Mobile.Rows.Count > 0)
                        {
                            int iLeaveCount = 1;
                            try
                            {
                                iLeaveCount = Convert.ToInt32(myCardTable_Mobile.Rows[0]["LeaveCount"].ToString().Trim());
                            }
                            catch
                            {
                                iLeaveCount = 1;
                            }
                            //根据AId来得到设备进行删除
                            if (myCardTable_Mobile.Rows.Count > 0)
                            {
                                if (myCardTable_Mobile.Rows[0]["AId"].ToString() != "0")
                                {
                                    for (int n = 0; n < DoorTable.Rows.Count; n++)
                                    {
                                        if (DoorTable.Rows[n]["ADId"].ToString().Trim() == myCardTable_Mobile.Rows[0]["AId"].ToString())
                                        {
                                            if (strCardNo != "")
                                            {
                                                blDelSuc = accessMan.RemoveRegister(DoorTable.Rows[n]["MSNo"].ToString().Trim(), byte.Parse(DoorTable.Rows[n]["DoorId"].ToString().Trim()), Convert.ToUInt32(strCardNo));
                                                if (blDelSuc == false)//删除权限失败
                                                {
                                                    //插入删除权限失败的门禁卡号信息到XXCLOUD.dbo.T_MJCardLogOutInf表中
                                                    InsertCardIntoLogoutDataTable(strCardNo, DoorTable.Rows[n]["MSNo"].ToString().Trim(), DoorTable.Rows[n]["MName"].ToString().Trim(), DoorTable.Rows[n]["DoorId"].ToString().Trim(), DoorTable.Rows[n]["DName"].ToString().Trim(), "0", "2");
                                                }
                                            }
                                        }
                                    }
                                    strSQL_Temp = "update T_LongTemCardInf set VCardStatus='" + "2" + "',VCardCNo='" + LoginFrm.strOperatorNo + "',VCardCActualNo='" + LoginFrm.strOperatorActualNo + "',";
                                    strSQL_Temp += "VCardCName='" + LoginFrm.strOperatorName + "',LeaveNormal='" + "1" + "',LeaveCount='" + iLeaveCount.ToString() + "'  where Id='" + myCardTable_Mobile.Rows[0]["Id"].ToString() + "' ";
                                    strSQL_Temp += " and VCardStatus='" + "0" + "'";
                                    strSQL_Temp += strLoginFrmSelectFlag;
                                    SQLHelper.ExecuteSql(strSQL_Temp);
                                }
                                else
                                {
                                    for (int n = 0; n < DoorTable.Rows.Count; n++)
                                    {
                                        if (strCardNo != "")
                                        {
                                            blDelSuc = accessMan.RemoveRegister(DoorTable.Rows[n]["MSNo"].ToString().Trim(), byte.Parse(DoorTable.Rows[n]["DoorId"].ToString().Trim()), Convert.ToUInt32(strCardNo));
                                            if (blDelSuc == false)//删除权限失败
                                            {
                                                //插入删除权限失败的门禁卡号信息到XXCLOUD.dbo.T_MJCardLogOutInf表中
                                                InsertCardIntoLogoutDataTable(strCardNo, DoorTable.Rows[n]["MSNo"].ToString().Trim(), DoorTable.Rows[n]["MName"].ToString().Trim(), DoorTable.Rows[n]["DoorId"].ToString().Trim(), DoorTable.Rows[n]["DName"].ToString().Trim(), "0", "2");
                                            }
                                        }
                                    }
                                    strSQL_Temp = "update T_LongTemCardInf set VCardStatus='" + "2" + "',VCardCNo='" + LoginFrm.strOperatorNo + "',VCardCActualNo='" + LoginFrm.strOperatorActualNo + "',";
                                    strSQL_Temp += "VCardCName='" + LoginFrm.strOperatorName + "',LeaveNormal='" + "1" + "' ,LeaveCount='" + iLeaveCount.ToString() + "' where Id='" + myCardTable_Mobile.Rows[0]["Id"].ToString() + "' ";
                                    strSQL_Temp += " and VCardStatus='" + "0" + "'";
                                    strSQL_Temp += strLoginFrmSelectFlag;
                                    SQLHelper.ExecuteSql(strSQL_Temp);
                                }
                            }
                        }
                        strSQL_Temp = "update " + strT_VisitorAccessInf + " set LeaveFromFlag='" + "1" + "' where Id ='" + myCardTable_GO.Rows[k]["Id"].ToString() + "' ";
                        SQLHelper.ExecuteSql(strSQL_Temp);
                    }
                    #endregion

                }
            }
            catch (Exception exp)
            {
                ExecuteSql();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//1.每天自动定时签离全部未签离来访记录并记为正常签离(不涉及到卡片注销)
        private void AllowedAllLeaveNormal()
        {
            try
            {
                DataTable dt = new DataTable();
                string strSQL_Temp = "select  Id,GONo,RegDT,VName,VSex,VCardNo,VIdNo,VVisitReason,VCarryGoods,VVisitPCount,VPhone,RDDetailName,RPName,Flag,BarCodeNo,VNameMCode,VCardNo from ";
                strSQL_Temp += strT_VisitorAccessInf + " where LeaDDetailName ='" + "" + "' and ( IsEmergencyConference ='" + "0" + "' or IsEmergencyConference is null )";
                strSQL_Temp += strLoginFrmSelectFlag;
                strSQL_Temp += " order by Id desc";
                dt = SQLHelper.DTQuery(strSQL_Temp);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strSQL_Temp = "update " + strT_VisitorAccessInf + " set LeaDT='" + System.DateTime.Now.ToString() + "' ,LeaDNo='" + LoginFrm.strGuardRoomId + "',";
                        strSQL_Temp += "LeaDDetailName='" + LoginFrm.strGuardRoomName + "',LeaOperatorNo='" + LoginFrm.strOperatorNo + "',LeaOperatorActualNo='" + LoginFrm.strOperatorActualNo + "',";
                        strSQL_Temp += "LeaOperatorName='" + LoginFrm.strOperatorName + "',LeaveNormal='" + "1" + "'  where Id='" + dt.Rows[i]["Id"].ToString().Trim() + "' ";
                        strSQL_Temp += strLoginFrmSelectFlag;
                        SQLHelper.ExecuteSql(strSQL_Temp);
                        //根据门禁卡号更新XXCLOUD.dbo.T_CardLostInf表
                        if (dt.Rows[i]["VCardNo"].ToString().Trim() != "" && dt.Rows[i]["VCardNo"].ToString().Trim() != null)
                        {
                            strSQL_Temp = "delete from XXCLOUD.dbo.T_CardLostInf where VCardNo ='" + dt.Rows[i]["VCardNo"].ToString().Trim() + "' ";
                            strSQL_Temp += strLoginFrmSelectFlag;
                            SQLHelper.ExecuteSql(strSQL_Temp);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                ExecuteSql();
                this.timerRunByMinute.Stop();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//2.每天自动定时从门禁控制器上注销所有未注销的访客证权限并记为正常注销,时间为
        private void AllowedLogoutRedCardNormal()
        {
            try
            {
                bool blDelSuc = false;//false:权限删除失败  true:权限删除成功
                DataTable dt = new DataTable();
                string strSQL_Temp = "select  Id,VCardNo,AId,LeaveCount,VName,VCardEndValidDT,ApiType,VCardStatus from ";
                strSQL_Temp += " XXCLOUD.dbo.T_LongTemCardInf  where (Flag ='" + "2" + "' and VCardStatus='" + "0" + "' ) ";
                strSQL_Temp += "   and VCardEndValidDT <='" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                strSQL_Temp += strLoginFrmSelectFlag;
                strSQL_Temp += " order by Id desc";
                dt = SQLHelper.DTQuery(strSQL_Temp);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        //根据AId来得到设备进行删除
                        if (dt.Rows[i]["AId"].ToString() != "0")
                        {
  
                            strSQL_Temp = "update T_LongTemCardInf set VCardStatus='" + "2" + "',VCardCNo='" + LoginFrm.strOperatorNo + "',VCardCActualNo='" + LoginFrm.strOperatorActualNo + "',";
                            strSQL_Temp += "VCardCName='" + LoginFrm.strOperatorName + "',LeaveNormal='" + "1" + "'  where Id='" + dt.Rows[i]["Id"].ToString().Trim() + "' ";
                            strSQL_Temp += " and VCardStatus='" + "0" + "'";
                            strSQL_Temp += strLoginFrmSelectFlag;
                            SQLHelper.ExecuteSql(strSQL_Temp);

                            //根据门禁卡号更新XXCLOUD.dbo.T_CardLostInf表,存在2和3的日期重叠，但卡号一张的情况，所以这下面三句代码得修改
                            strSQL_Temp = "delete from XXCLOUD.dbo.T_CardLostInf where VCardNo ='" + dt.Rows[i]["VCardNo"].ToString().Trim() + "' ";
                            strSQL_Temp += strLoginFrmSelectFlag;
                            SQLHelper.ExecuteSql(strSQL_Temp);
                        }
                        else
                        {


                            strSQL_Temp = "update T_LongTemCardInf set VCardStatus='" + "2" + "',VCardCNo='" + LoginFrm.strOperatorNo + "',VCardCActualNo='" + LoginFrm.strOperatorActualNo + "',";
                            strSQL_Temp += "VCardCName='" + LoginFrm.strOperatorName + "',LeaveNormal='" + "1" + "'   where Id='" + dt.Rows[i]["Id"].ToString().Trim() + "' ";
                            strSQL_Temp += " and VCardStatus='" + "0" + "'";
                            strSQL_Temp += strLoginFrmSelectFlag;
                            SQLHelper.ExecuteSql(strSQL_Temp);
                            //根据门禁卡号更新XXCLOUD.dbo.T_CardLostInf表
                            strSQL_Temp = "delete from XXCLOUD.dbo.T_CardLostInf where VCardNo ='" + dt.Rows[i]["VCardNo"].ToString().Trim() + "' ";
                            strSQL_Temp += strLoginFrmSelectFlag;
                            SQLHelper.ExecuteSql(strSQL_Temp);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                ExecuteSql();
                this.timerRunByMinute.Stop();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//3.每天自动定时根据预约有效期限把过期预约设为过期预约
        //受邀访客因其他原因而未在规定时间内来访，所以需要把受邀访客表里OrderEnabled设为非1字段，1：表示有效 0：已经登记过 2：表示过期预约
        private void OrderEnabledByPastDue()
        {
            try
            {
                myTable_Order.Rows.Clear();
                string strSQL_Temp = "select Id,IsReg from " + LoginFrm.strOrderDBName + "  where  State='" + "1" + "'   and  EndDate < '" + System.DateTime.Now.ToShortDateString() + "' ";
                strSQL_Temp += " and ApiType <>'" + "2" + "'   order by Id";//支撑人员首次来访登记永远不过期2018-10-16
                myTable_Order = SQLHelper_Remote.DTQuery(strSQL_Temp);
                if (myTable_Order.Rows.Count > 0)
                {
                    for (int n = 0; n < myTable_Order.Rows.Count; n++)
                    {
                        if (myTable_Order.Rows[n]["IsReg"].ToString().Trim() == "1")
                        {
                            //如果没登记过，设为过期预约
                            strSQL_Temp = "update " + LoginFrm.strOrderDBName + "  set State='" + "2" + "' where Id='" + myTable_Order.Rows[n]["Id"].ToString().Trim() + "' ";
                        }
                        else
                        {
                            //如果已登记过，把此预约记录设为已登记预约
                            strSQL_Temp = "update " + LoginFrm.strOrderDBName + "  set State='" + "0" + "' where Id='" + myTable_Order.Rows[n]["Id"].ToString().Trim() + "' ";

                        }
                        SQLHelper_Remote.ExecuteSql(strSQL_Temp);
                    }
                }
            }
            catch (Exception exp)
            {
                ExecuteSql();
                this.timerRunByMinute.Stop();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//4.每天自动定时根据预约有效期限把过期的会议预约删除掉

        private void OrderEnabledByConferenceInf()
        {
            try
            {
                myTable_Order.Rows.Clear();
                string strSQL_Temp = "select Id from XXCLOUD.dbo.T_ConferenceInf  where      EndDate < '" + System.DateTime.Now.ToShortDateString() + "' ";
                strSQL_Temp += " order by Id";
                myTable_Order = SQLHelper.DTQuery(strSQL_Temp);
                if (myTable_Order.Rows.Count > 0)
                {
                    for (int n = 0; n < myTable_Order.Rows.Count; n++)
                    {
                        strSQL_Temp = "delete from XXCLOUD.dbo.T_ConferenceInf  where Id='" + myTable_Order.Rows[n]["Id"].ToString().Trim() + "' ";
                        SQLHelper.ExecuteSql(strSQL_Temp);
                    }
                }
            }
            catch (Exception exp)
            {
                ExecuteSql();
                this.timerRunByMinute.Stop();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//5.每天自动定时签离全部紧急会议记录并记为正常签离(不涉及到卡片注销)
        private void AllowedAllEmergencyConference()
        {
            try
            {
                DataTable dt = new DataTable();
                string strSQL_Temp = "select  Id,GONo,RegDT,VName,VSex,VCardNo,VIdNo,VVisitReason,VCarryGoods,VVisitPCount,VPhone,RDDetailName,RPName,Flag,BarCodeNo,VNameMCode,VCardNo from ";
                strSQL_Temp += strT_VisitorAccessInf + " where LeaDDetailName ='" + "" + "' and  IsEmergencyConference ='" + "1" + "' ";
                strSQL_Temp += "   and VCardEndValidDT <='" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                strSQL_Temp += strLoginFrmSelectFlag;
                strSQL_Temp += " order by Id desc";
                dt = SQLHelper.DTQuery(strSQL_Temp);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strSQL_Temp = "update " + strT_VisitorAccessInf + " set LeaDT='" + System.DateTime.Now.ToString() + "' ,LeaDNo='" + LoginFrm.strGuardRoomId + "',";
                        strSQL_Temp += "LeaDDetailName='" + LoginFrm.strGuardRoomName + "',LeaOperatorNo='" + LoginFrm.strOperatorNo + "',LeaOperatorActualNo='" + LoginFrm.strOperatorActualNo + "',";
                        strSQL_Temp += "LeaOperatorName='" + LoginFrm.strOperatorName + "',LeaveNormal='" + "1" + "'  where Id='" + dt.Rows[i]["Id"].ToString().Trim() + "' ";
                        strSQL_Temp += strLoginFrmSelectFlag;
                        SQLHelper.ExecuteSql(strSQL_Temp);
                        //根据门禁卡号更新XXCLOUD.dbo.T_CardLostInf表
                        if (dt.Rows[i]["VCardNo"].ToString().Trim() != "" && dt.Rows[i]["VCardNo"].ToString().Trim() != null)
                        {
                            strSQL_Temp = "delete from XXCLOUD.dbo.T_CardLostInf where VCardNo ='" + dt.Rows[i]["VCardNo"].ToString().Trim() + "' ";
                            strSQL_Temp += strLoginFrmSelectFlag;
                            SQLHelper.ExecuteSql(strSQL_Temp);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                ExecuteSql();
                this.timerRunByMinute.Stop();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//加载门禁卡号信息
        private void LoaddtCardNo()
        {
            dtCardSerialNo.Rows.Clear();
            strSQL = "select BClassName,BValue from XXCLOUD.dbo.T_BaseCategroyInf where Flag='" + "门禁卡号" + "'";
            strSQL += strLoginFrmSelectFlag;
            dtCardSerialNo = SQLHelper.DTQuery(strSQL);
        }
        #endregion

        #region//插入删除权限失败的门禁卡号信息到XXCLOUD.dbo.T_MJCardLogOutInf表中
        private void InsertCardIntoLogoutDataTable(string para_strCardNo, string para_strMSNo, string para_strMName, string para_strDoorId, string para_strDName, string para_strVCardStatus, string para_strFlag)
        {
            string strCardSerialNo = "";
            try
            {
                DV = null;
                DV = new DataView(dtCardSerialNo.Copy());
                DV.RowFilter = "BClassName ='" + para_strCardNo + "'";
                if (DV.Count > 0)
                {
                    foreach (DataRowView DVRow in DV)
                    {
                        strCardSerialNo = DVRow["BValue"].ToString().Trim();
                    }
                }
                else
                {
                    LoaddtCardNo();
                    DV = new DataView(dtCardSerialNo.Copy());
                    DV.RowFilter = "BClassName ='" + para_strCardNo + "'";
                    if (DV.Count > 0)
                    {
                        foreach (DataRowView DVRow in DV)
                        {
                            strCardSerialNo = DVRow["BValue"].ToString().Trim();
                        }
                    }
                }
                string strSQL_Temp = "";
                strSQL_Temp = "insert into XXCLOUD.dbo.T_MJCardLogOutInf(MSNo,MName,DoorId,DName,VCardNo,VCardSerialNo,VCardStatus,OperatDT,";
                strSQL_Temp += " Flag,OperatorActualNo,OperatorNo,OperatorName,VPlace)Values(";
                strSQL_Temp += "'" + para_strMSNo + "','" + para_strMName + "',";
                strSQL_Temp += "'" + para_strDoorId + "','" + para_strDName + "',";
                strSQL_Temp += "'" + para_strCardNo + "','" + strCardSerialNo + "',";
                strSQL_Temp += "'" + "0" + "','" + System.DateTime.Now.ToString() + "',";
                strSQL_Temp += "'" + "2" + "','" + LoginFrm.strOperatorActualNo + "',";
                strSQL_Temp += "'" + LoginFrm.strOperatorNo + "','" + LoginFrm.strOperatorName + "','" + strEndUserName + "')";
                SQLHelper.ExecuteSql(strSQL_Temp);
            }
            catch (Exception exp)
            {
                ExecuteSql();
                OperatorLog("删除门禁卡片权限出现错误，门禁卡号:" + para_strCardNo + " ,卡面编号:" + strCardSerialNo, "0");
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//根据XXCLOUD.dbo.T_MJCardLogOutInf表中的内容删除权限
        private void DelCardPowerFromMachine()
        {
            try
            {
                bool blDelSuc = false;//false:权限删除失败  true:权限删除成功
                dtDelPower.Rows.Clear();
                string strSQL_Temp = "select top 10 Id,MSNo,DoorId,VCardNo from XXCLOUD.dbo.T_MJCardLogOutInf where VCardStatus='" + "0" + "'";
                dtDelPower = SQLHelper.DTQuery(strSQL_Temp);
                if (dtDelPower.Rows.Count > 0)
                {
                    for (int i = 0; i < dtDelPower.Rows.Count; i++)
                    {
                        blDelSuc = accessMan.RemoveRegister(dtDelPower.Rows[i]["MSNo"].ToString().Trim(), byte.Parse(dtDelPower.Rows[i]["DoorId"].ToString().Trim()), Convert.ToUInt32(dtDelPower.Rows[i]["VCardNo"].ToString().Trim()));
                        if (blDelSuc == false)//删除权限失败
                        {
                            //等待下次继续删除
                        }
                        else
                        {
                            strSQL_Temp = "update XXCLOUD.dbo.T_MJCardLogOutInf set VCardStatus='" + "2" + "' ,COperatorActualNo='" + LoginFrm.strOperatorActualNo + "',";
                            strSQL_Temp += " COperatorNo='" + LoginFrm.strOperatorNo + "',COperatorName='" + LoginFrm.strOperatorName + "',COperatDT='" + System.DateTime.Now.ToString() + "' ";
                            strSQL_Temp += " where Id='" + dtDelPower.Rows[i]["Id"].ToString().Trim() + "' ";
                            strSQL_Temp += strLoginFrmSelectFlag;
                            SQLHelper.ExecuteSql(strSQL_Temp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                ExecuteSql();
                if (exp.ToString().Contains("未将对象") == true)
                {
                    //更换了新的控制器，且老的控制器序列号跟新的不一样导致的错误。
                    //根据dtDelPower表中的MSNo，来查询设备表，删除T_MJCardLogOutInf表中MSNo不存在的记录
                    DataTable dtMSNo_Del = new DataTable();
                    string strSQLMSNo_Del = "select distinct MSNo from XXCLOUD.dbo.T_MJAPMachineInf ";
                    dtMSNo_Del = SQLHelper.DTQuery(strSQLMSNo_Del);
                    if (dtMSNo_Del.Rows.Count > 0 && dtDelPower.Rows.Count > 0)
                    {
                        bool blExistDel = false;
                        for (int i = 0; i < dtDelPower.Rows.Count; i++)
                        {
                            blExistDel = false;
                            for (int j = 0; j < dtMSNo_Del.Rows.Count; j++)
                            {
                                if (dtDelPower.Rows[i]["MSNo"].ToString().Trim() == dtMSNo_Del.Rows[j]["MSNo"].ToString().Trim())
                                {
                                    blExistDel = true;
                                    break;
                                }
                            }
                            if (blExistDel == false)
                            {
                                strSQLMSNo_Del = "delete from XXCLOUD.dbo.T_MJCardLogOutInf where MSNo='" + dtDelPower.Rows[i]["MSNo"].ToString().Trim() + "'";
                                SQLHelper.ExecuteSql(strSQLMSNo_Del);
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        #endregion

        #region//计算两个时间的时间差
        /// <summary>
        /// 已重载.计算两个日期的时间间隔,返回的是时间间隔的日期差的绝对值.
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        private string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                dateDiff = ts.Days.ToString() + "天"
                        + ts.Hours.ToString() + "小时"
                        + ts.Minutes.ToString() + "分钟"
                        + ts.Seconds.ToString() + "秒";
            }
            catch
            {
                ExecuteSql();
            }
            return dateDiff;
        }

        #endregion

        #region//更改预约表里的工作证号，去掉No.或No。这几个字符
        private void ModifyIdCardNoFromReservationRecord()
        {
            return;
            //try
            //{
            //    //预约表
            //    string strSQLTemp = "select Id,IdCardNo from  LogisticsDB.dbo.ReservationRecord where (IdCardNo like '%" + "No." + "%' or  ";
            //    strSQLTemp += " IdCardNo like '%" + "NO." + "%' or IdCardNo like '&" + "No。" + "%')";
            //    DataTable dtTemp = new DataTable();
            //    dtTemp = SQLHelper_Remote.DTQuery(strSQLTemp);
            //    if (dtTemp.Rows.Count > 0)
            //    {
            //        for (int i = 0; i < dtTemp.Rows.Count; i++)
            //        {
            //            strSQLTemp = dtTemp.Rows[i]["IdCardNo"].ToString().Trim().ToUpper();
            //            strSQLTemp = strSQLTemp.Replace("NO.", "");
            //            strSQLTemp = strSQLTemp.Replace("NO。", "");
            //            strSQLTemp = "update LogisticsDB.dbo.ReservationRecord set IdCardNo='" + strSQLTemp + "' where Id='" + dtTemp.Rows[i]["Id"].ToString().Trim() + "'";
            //            SQLHelper_Remote.ExecuteSql(strSQLTemp);
            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            //try
            //{
            //    //来访记录表
            //    DataTable dt = new DataTable();
            //    string strSQL_Temp = "select  Id, VCardNo from ";
            //    strSQL_Temp += strT_VisitorAccessInf + " where (VCardNo like '%" + "No." + "%' or  ";
            //    strSQL_Temp += " VCardNo like '%" + "NO." + "%' or VCardNo like '&" + "No。" + "%')";
            //    DataTable dtTemp = new DataTable();
            //    strSQL_Temp += strLoginFrmSelectFlag;
            //    strSQL += " order by Id desc";
            //    dt = SQLHelper.DTQuery(strSQL_Temp);
            //    if (dt.Rows.Count > 0)
            //    {
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            strSQL_Temp = dt.Rows[i]["VCardNo"].ToString().Trim().ToUpper();
            //            strSQL_Temp = strSQL_Temp.Replace("NO.", "");
            //            strSQL_Temp = strSQL_Temp.Replace("NO。", "");

            //            strSQL_Temp = "update " + strT_VisitorAccessInf + " set VCardNo='" + strSQL_Temp + "'   where Id='" + dt.Rows[i]["Id"].ToString().Trim() + "' ";
            //            strSQL_Temp += strLoginFrmSelectFlag;
            //            SQLHelper.ExecuteSql(strSQL_Temp);
              
            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //    ExecuteSql();
            //    this.timerRunByMinute.Stop();
            //    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            //try
            //{
            //    //常客卡和临时卡记录表
            //    DataTable dt = new DataTable();
            //    string strSQL_Temp = "select  Id, VCardNo from  XXCLOUD.dbo.T_LongTemCardInf where (VCardNo like '%" + "No." + "%' or  ";
            //    strSQL_Temp += " VCardNo like '%" + "NO." + "%' or VCardNo like '&" + "No。" + "%')";
            //    DataTable dtTemp = new DataTable();
            //    strSQL_Temp += strLoginFrmSelectFlag;
            //    strSQL += " order by Id desc";
            //    dt = SQLHelper.DTQuery(strSQL_Temp);
            //    if (dt.Rows.Count > 0)
            //    {
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            strSQL_Temp = dt.Rows[i]["VCardNo"].ToString().Trim().ToUpper();
            //            strSQL_Temp = strSQL_Temp.Replace("NO.", "");
            //            strSQL_Temp = strSQL_Temp.Replace("NO。", "");

            //            strSQL_Temp = "update   XXCLOUD.dbo.T_LongTemCardInf set VCardNo='" + strSQL_Temp + "'   where Id='" + dt.Rows[i]["Id"].ToString().Trim() + "' ";
            //            strSQL_Temp += strLoginFrmSelectFlag;
            //            SQLHelper.ExecuteSql(strSQL_Temp);
                
            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //    ExecuteSql();
            //    this.timerRunByMinute.Stop();
            //    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            //try
            //{
            //    //卡片遗失表
            //    DataTable dt = new DataTable();
            //    string strSQL_Temp = "select  Id, VCardNo from  XXCLOUD.dbo.T_CardLostInf where (VCardNo like '%" + "No." + "%' or  ";
            //    strSQL_Temp += " VCardNo like '%" + "NO." + "%' or VCardNo like '&" + "No。" + "%')";
            //    DataTable dtTemp = new DataTable();
            //    strSQL_Temp += strLoginFrmSelectFlag;
            //    strSQL += " order by Id desc";
            //    dt = SQLHelper.DTQuery(strSQL_Temp);
            //    if (dt.Rows.Count > 0)
            //    {
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            strSQL_Temp = dt.Rows[i]["VCardNo"].ToString().Trim().ToUpper();
            //            strSQL_Temp = strSQL_Temp.Replace("NO.", "");
            //            strSQL_Temp = strSQL_Temp.Replace("NO。", "");

            //            strSQL_Temp = "update   XXCLOUD.dbo.T_CardLostInf set VCardNo='" + strSQL_Temp + "'   where Id='" + dt.Rows[i]["Id"].ToString().Trim() + "' ";
            //            strSQL_Temp += strLoginFrmSelectFlag;
            //            SQLHelper.ExecuteSql(strSQL_Temp);

            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //    ExecuteSql();
            //    this.timerRunByMinute.Stop();
            //    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
        #endregion

        private Int64 index = 0;
 
        #region//云控制器
        void showResult(Boolean result, byte lasterror)
        {
            string msg = "成功";
            if (!result)
            {
                switch (lasterror)
                {
                    case 1: msg = "对象不存在"; break;
                    case 2: msg = "数据超出边界"; break;
                    case 3: msg = "操作超时"; break;
                    case 4: msg = "断开"; break;
                    case 5: msg = "返回数据错误"; break;
                    case 6: msg = "未知错误"; break;
                }
            }
            //label1.Text = msg;
        }
        //监听的回调原型

        private void EventHandler(XXY_VisitorMJAsst.RAcsEvent Event, ref UInt16 time, ref string card, ref string voice, ref string name, ref string note, ref string etime, ref byte relay, ref byte OpenDoor, ref Boolean Ack)
        {

            strMachineId = "";

            strMName = "";
            strDName = "";
            strReadHeadNote = "";
            strNote = "";
            strRecordType = "";
            strSNo = "";
            strSActualNo = "";
            strSName = "";
            strFloor = "";
            strSDDetailName = "";
            strApiTypeName = "";
            strVisitorType = "";
            strGONo = "";
            strSIdNo = "";
            strSSex = "";
            #region//事件（如刷卡开门，远程开门）
            if (this.grid_GORecord.Rows > 1000)
            {
                this.grid_GORecord.AutoRedraw = false;
                this.grid_GORecord.Rows = 1;
                this.grid_GORecord.AutoRedraw = true;
                this.grid_GORecord.Refresh();
            }
            if (this.grid_AlarmRecord.Rows > 1000)
            {

                this.grid_AlarmRecord.AutoRedraw = false;
                this.grid_AlarmRecord.Rows = 1;
                this.grid_AlarmRecord.AutoRedraw = true;
                this.grid_AlarmRecord.Refresh();
            }
            if (this.grid_NoCardRecord.Rows > 1000)
            {
                this.grid_NoCardRecord.AutoRedraw = false;
                this.grid_NoCardRecord.Rows = 1;
                this.grid_NoCardRecord.AutoRedraw = true;
                this.grid_NoCardRecord.Refresh();
            }
            if (this.grid_SpecialRecord.Rows > 1000)
            {
                this.grid_SpecialRecord.AutoRedraw = false;
                this.grid_SpecialRecord.Rows = 1;
                this.grid_SpecialRecord.AutoRedraw = true;
                this.grid_SpecialRecord.Refresh();
            }
            strMachineId = "";
            strMName = Event.ID.ToString();

            strMName = strMName.Trim().Substring(0, strMName.IndexOf("\0"));

            strDName = Event.Door.ToString();
            strReadHeadNote = "";
            strNote = "非法门禁卡或二维码";
            if (DoorTable.Rows.Count > 0)
            {
                for (int n = 0; n < DoorTable.Rows.Count; n++)
                {
                    // MessageBox.Show(Event.Door.ToString() + "A  " + DoorTable.Rows[n]["DoorId"].ToString().Trim() + "     B     " + DoorTable.Rows[n]["MSNo"].ToString().Trim() + "   C   " + Event.ID.ToString());
                    if ((Event.Door.ToString() == DoorTable.Rows[n]["DoorId"].ToString().Trim()) && (strMName == DoorTable.Rows[n]["MSNo"].ToString().Trim()))
                    {
                        strMachineId = DoorTable.Rows[n]["MachineId"].ToString().Trim();
                        strMName = DoorTable.Rows[n]["MName"].ToString().Trim();
                        strDName = DoorTable.Rows[n]["DName"].ToString().Trim();
                        break;
                    }
                }
            }
            if (HeadReadTable.Rows.Count > 0)
            {
                for (int n = 0; n < HeadReadTable.Rows.Count; n++)
                {
                    if ((Event.Door.ToString() == HeadReadTable.Rows[n]["DoorId"].ToString().Trim()) && (Event.ID.ToString() == HeadReadTable.Rows[n]["MSNo"].ToString().Trim()) && (Event.Reader.ToString() == HeadReadTable.Rows[n]["ReadHeadId"].ToString().Trim()))
                    {
                        strReadHeadNote = HeadReadTable.Rows[n]["ReadHeadNote"].ToString().Trim();
                        break;
                    }
                }
            }
            #endregion
            string strDTNow = System.DateTime.Now.ToString();
            dtOpenDoor.Rows.Clear();
            string strCardNoTemp = Event.Value.ToString();

            if (dt_VIPCard.Rows.Count > 0)
            {
                int k = 0;
                bool blExist = false;
                for (; k < dt_VIPCard.Rows.Count; k++)
                {
                    if (dt_VIPCard.Rows[k]["MSNO"].ToString().Trim() == Event.SerialNo)
                    {
                        blExist = true;
                        break;
                    }
                }
                if (blExist == true)
                {
                    if (dt_VIPCard.Rows[k]["Flag"].ToString().Trim() == "1")
                    {
                        strSQL_OpenDoor = "select top 1 Id ,SName,SCardNo,EnterCount,LeaveCount,AId ,SNo,SActualNo,SDDetailName  from XXCLOUD.dbo.T_VIPCardStaffInf where SCardNo ='" + strCardNoTemp + "'  ";//and MJEnabled ='" + "1" + "' ";
                        strSQL_OpenDoor += " and MJCardValidStart <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                        strSQL_OpenDoor += " and MJCardValidEnd >= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                        strSQL_OpenDoor += strLoginFrmSelectFlag;
                        strSQL_OpenDoor += " order by Id desc";
                        dtOpenDoor = SQLHelper.DTQuery(strSQL_OpenDoor);
                        if (dtOpenDoor.Rows.Count > 0)
                        {
                            //继续执行下面所有代码
                        }
                        else
                        {
                            //普通卡在闸机开启VIP卡功能后，当作特殊记录处理
                            dSpecialRecord_Count++;
                            SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_SpecialRecord);
                            setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, strDTNow, Event.EventType.ToString(), "0");
                            return;
                        }
                    }
                }
            }


            //strCardNoTemp = "10643";
            //一、内部学生卡开门
            strSQL_OpenDoor = "select top 1 Id ,SName,SCardNo,EnterCount,LeaveCount,AId ,SNo,SActualNo,SDDetailName,SIdNo,SSex    from XXCLOUD.dbo.T_ClassAndStudentInf where SCardNo ='" + strCardNoTemp + "'  ";//and MJEnabled ='" + "1" + "' ";
            //strSQL_OpenDoor += " and MJCardValidStart <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
            //strSQL_OpenDoor += " and MJCardValidEnd >= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
            strSQL_OpenDoor += strLoginFrmSelectFlag;
            strSQL_OpenDoor += " order by Id desc";
            dtOpenDoor = SQLHelper.DTQuery(strSQL_OpenDoor);
            if (dtOpenDoor.Rows.Count > 0)
            {
                strFloor = "";
                strSName = dtOpenDoor.Rows[0]["SName"].ToString().Trim();
                strSNo = dtOpenDoor.Rows[0]["SNo"].ToString().Trim();
                strSActualNo = dtOpenDoor.Rows[0]["SActualNo"].ToString().Trim();
                strSDDetailName = dtOpenDoor.Rows[0]["SDDetailName"].ToString().Trim();
                strSIdNo = dtOpenDoor.Rows[0]["SIdNo"].ToString().Trim();
                strSSex = dtOpenDoor.Rows[0]["SSex"].ToString().Trim();
                strApiTypeName = "";
                strVisitorType = "学生";


                #region//一、内部学生卡开门
                bool blAccessPermissions = false;//1.判断是否具有通行权限  false:没有权限  true:合法权限
                if (dtOpenDoor.Rows[0]["AId"].ToString().Trim() == "0" || dtOpenDoor.Rows[0]["AId"].ToString().Trim() == null)//全部区域
                {
                    blAccessPermissions = true;
                }
                else
                {
                    for (int k = 0; k < DoorTable.Rows.Count; k++)
                    {
                        if (DoorTable.Rows[k]["ADId"].ToString().Trim() == dtOpenDoor.Rows[0]["AId"].ToString().Trim() && DoorTable.Rows[k]["MSNo"].ToString().Trim() == Event.ID.ToString())
                        {
                            blAccessPermissions = true;
                            break;
                        }
                    }
                }
                //2.如果具有通行权限，则判断哪个控制器的进出
                if (blAccessPermissions == true)
                {
                    //可能存在多条卡号是一样的且开门权限都是合法但进出次数不一样的情况。所以访客软件里保存来访记录前得根据卡号更改发卡表里这些合法记录为非法卡。
                    //MessageBox.Show(dtOpenDoor.Rows.Count.ToString ());
                    strFKId = dtOpenDoor.Rows[0]["Id"].ToString().Trim();
                    if (Event.Reader == 0)//进门
                    {
                        strReadHeadNote = "进门";
                        #region//进门
                        int iEnterCount = 0;
                        try
                        {
                            iEnterCount = Convert.ToInt32(dtOpenDoor.Rows[0]["EnterCount"].ToString().Trim());
                        }
                        catch
                        {
                            iEnterCount = 0;
                        }


                        Ack = true;
                        OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                        relay = Event.Reader;
                        time = Convert.ToUInt16(LoginFrm.strAllowedOpenDoorDelaySecond);
                        card = strCardNoTemp;// "23456"; 
                        if (card == "")
                        {
                            card = "0";
                        }
                        voice = dtOpenDoor.Rows[0]["SName"].ToString().Trim() + "进门";  // "测试语音";
                        name = dtOpenDoor.Rows[0]["SName"].ToString().Trim(); // "姓名"; 
                        note = "进门";// "进门出门"; 
                        etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";           
                        ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);
                        iEnterCount++;
                        //记录最后一次入闸时间,进入次数加1

                        strSQL_OpenDoor = "update XXCLOUD.dbo.T_ClassAndStudentInf  set EnterDoorDT='" + System.DateTime.Now.ToString() + "' ,EnterCount='" + iEnterCount.ToString() + "'  ";
                        strSQL_OpenDoor += " where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString().Trim() + "' ";
                        strSQL_OpenDoor += strLoginFrmSelectFlag;
                        SQLHelper.ExecuteSql(strSQL_OpenDoor);

                        strRecordType = "1";//进出记录
                        dGORecord_Count++;
                        SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                        setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, System.DateTime.Now.ToString(), Event.EventType.ToString(), "Student");


                        #endregion
                    }
                    else if (Event.Reader == 1)//出门
                    {
                        strReadHeadNote = "出门";
                        #region//出门
                        int iLeaveCount = 0;
                        try
                        {
                            iLeaveCount = Convert.ToInt32(dtOpenDoor.Rows[0]["LeaveCount"].ToString().Trim());
                        }
                        catch
                        {
                            iLeaveCount = 0;
                        }

                        #region
                        Ack = true;
                        OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                        relay = Event.Reader;
                        time = Convert.ToUInt16(LoginFrm.strAllowedOpenDoorDelaySecond);
                        card = strCardNoTemp;// "23456"; 
                        if (card == "")
                        {
                            card = "0";
                        }
                        voice = dtOpenDoor.Rows[0]["SName"].ToString().Trim() + "出门";  // "测试语音";
                        name = dtOpenDoor.Rows[0]["SName"].ToString().Trim(); // "姓名"; 
                        note = "出门";// "进门出门"; 
                        etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";           
                        ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);
                        iLeaveCount++;

                        strSQL_OpenDoor = "update XXCLOUD.dbo.T_ClassAndStudentInf  set LeaveDoorDT='" + System.DateTime.Now.ToString() + "' ,LeaveCount='" + iLeaveCount.ToString() + "'  ";
                        strSQL_OpenDoor += " where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString().Trim() + "' ";
                        strSQL_OpenDoor += strLoginFrmSelectFlag;
                        SQLHelper.ExecuteSql(strSQL_OpenDoor);

                        strRecordType = "1";//进出记录
                        dGORecord_Count++;
                        SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                        setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, System.DateTime.Now.ToString(), Event.EventType.ToString(), "Student");


                        #endregion

                        #endregion
                    }
                }
                else
                {
                    dSpecialRecord_Count++;
                    SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_SpecialRecord);
                    setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, strDTNow, Event.EventType.ToString(), "0");
                }
                #endregion
            }
            else
            {
                //一、内部员工卡开门
                strSQL_OpenDoor = "select top 1 Id ,SName,SCardNo,EnterCount,LeaveCount,AId ,SNo,SActualNo,SDDetailName,SIdNo,SSex  from XXCLOUD.dbo.T_StaffInf where SCardNo ='" + strCardNoTemp + "'  ";//and MJEnabled ='" + "1" + "' ";
                strSQL_OpenDoor += " and MJCardValidStart <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                strSQL_OpenDoor += " and MJCardValidEnd >= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                strSQL_OpenDoor += strLoginFrmSelectFlag;
                strSQL_OpenDoor += " order by Id desc";
                dtOpenDoor = SQLHelper.DTQuery(strSQL_OpenDoor);
                if (dtOpenDoor.Rows.Count > 0)
                {
                    strFloor = "";
                    strSName = dtOpenDoor.Rows[0]["SName"].ToString().Trim();
                    strSNo = dtOpenDoor.Rows[0]["SNo"].ToString().Trim();
                    strSActualNo = dtOpenDoor.Rows[0]["SActualNo"].ToString().Trim();
                    strSDDetailName = dtOpenDoor.Rows[0]["SDDetailName"].ToString().Trim();
                    strSIdNo = dtOpenDoor.Rows[0]["SIdNo"].ToString().Trim();
                    strSSex = dtOpenDoor.Rows[0]["SSex"].ToString().Trim();

                    strApiTypeName = "";
                    strVisitorType = "内部员工";
                    #region//一、内部内部员工卡开门
                    bool blAccessPermissions = false;//1.判断是否具有通行权限  false:没有权限  true:合法权限
                    if (dtOpenDoor.Rows[0]["AId"].ToString().Trim() == "0" || dtOpenDoor.Rows[0]["AId"].ToString().Trim() == null)//全部区域
                    {
                        blAccessPermissions = true;
                    }
                    else
                    {
                        for (int k = 0; k < DoorTable.Rows.Count; k++)
                        {
                            if (DoorTable.Rows[k]["ADId"].ToString().Trim() == dtOpenDoor.Rows[0]["AId"].ToString().Trim() && DoorTable.Rows[k]["MSNo"].ToString().Trim() == Event.ID.ToString())
                            {
                                blAccessPermissions = true;
                                break;
                            }
                        }
                    }
                    //2.如果具有通行权限，则判断哪个控制器的进出
                    if (blAccessPermissions == true)
                    {
                        //可能存在多条卡号是一样的且开门权限都是合法但进出次数不一样的情况。所以访客软件里保存来访记录前得根据卡号更改发卡表里这些合法记录为非法卡。
                        //MessageBox.Show(dtOpenDoor.Rows.Count.ToString ());
                        strFKId = dtOpenDoor.Rows[0]["Id"].ToString().Trim();
                        if (Event.Reader == 0)//进门
                        {
                            strReadHeadNote = "进门";
                            #region//进门
                            int iEnterCount = 0;
                            try
                            {
                                iEnterCount = Convert.ToInt32(dtOpenDoor.Rows[0]["EnterCount"].ToString().Trim());
                            }
                            catch
                            {
                                iEnterCount = 0;
                            }


                            Ack = true;
                            OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                            relay = Event.Reader;
                            time = Convert.ToUInt16(LoginFrm.strAllowedOpenDoorDelaySecond);
                            card = strCardNoTemp;// "23456"; 
                            if (card == "")
                            {
                                card = "0";
                            }
                            voice = dtOpenDoor.Rows[0]["SName"].ToString().Trim() + "进门";  // "测试语音";
                            name = dtOpenDoor.Rows[0]["SName"].ToString().Trim(); // "姓名"; 
                            note = "进门";// "进门出门"; 
                            etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";           
                            ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);
                            iEnterCount++;
                            //记录最后一次入闸时间,进入次数加1

                            strSQL_OpenDoor = "update XXCLOUD.dbo.T_StaffInf  set EnterDoorDT='" + System.DateTime.Now.ToString() + "' ,EnterCount='" + iEnterCount.ToString() + "'  ";
                            strSQL_OpenDoor += " where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString().Trim() + "' ";
                            strSQL_OpenDoor += strLoginFrmSelectFlag;
                            SQLHelper.ExecuteSql(strSQL_OpenDoor);

                            strRecordType = "1";//进出记录
                            dGORecord_Count++;
                            SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                            setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, System.DateTime.Now.ToString(), Event.EventType.ToString(), "Staff");


                            #endregion
                        }
                        else if (Event.Reader == 1)//出门
                        {
                            strReadHeadNote = "出门";
                            #region//出门
                            int iLeaveCount = 0;
                            try
                            {
                                iLeaveCount = Convert.ToInt32(dtOpenDoor.Rows[0]["LeaveCount"].ToString().Trim());
                            }
                            catch
                            {
                                iLeaveCount = 0;
                            }

                            #region
                            Ack = true;
                            OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                            relay = Event.Reader;
                            time = Convert.ToUInt16(LoginFrm.strAllowedOpenDoorDelaySecond);
                            card = strCardNoTemp;// "23456"; 
                            if (card == "")
                            {
                                card = "0";
                            }
                            voice = dtOpenDoor.Rows[0]["SName"].ToString().Trim() + "出门";  // "测试语音";
                            name = dtOpenDoor.Rows[0]["SName"].ToString().Trim(); // "姓名"; 
                            note = "出门";// "进门出门"; 
                            etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";           
                            ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);
                            iLeaveCount++;

                            strSQL_OpenDoor = "update XXCLOUD.dbo.T_StaffInf  set LeaveDoorDT='" + System.DateTime.Now.ToString() + "' ,LeaveCount='" + iLeaveCount.ToString() + "'  ";
                            strSQL_OpenDoor += " where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString().Trim() + "' ";
                            strSQL_OpenDoor += strLoginFrmSelectFlag;
                            SQLHelper.ExecuteSql(strSQL_OpenDoor);

                            strRecordType = "1";//进出记录
                            dGORecord_Count++;
                            SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                            setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, System.DateTime.Now.ToString(), Event.EventType.ToString(), "Staff");


                            #endregion

                            #endregion
                        }
                    }
                    else
                    {
                        dSpecialRecord_Count++;
                        SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_SpecialRecord);
                        setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, strDTNow, Event.EventType.ToString(), "0");
                    }
                    #endregion
                }
                else
                {

                    #region //二、访客临时卡或二维码开门
                    // this.tabControl1.SelectedIndex = 3;
                    //备注：来访记录表里只有二维码，而支撑人员首次和延期都没二维码，也就是BarCodeNo一直为空。
                    strRecordType = "4";
                    dtOpenDoor.Rows.Clear();
                    strSQL_OpenDoor = "select  top 1 Id ,GONO,VNo,VIdNo,VActualNo,VName,VSex ,VCardNo,EnterCount,LeaveCount,AId,RegWay,ApiType,VUnitName,ApiTypeName,VisitorType,RPRoomNo  from " + strT_VisitorAccessInf + " where ( VIdWLNoToDec ='" + strCardNoTemp + "'  ) and ( LeaDDetailName ='" + "" + "' or  LeaDDetailName is null )";
                    strSQL_OpenDoor += " and VCardStartValidDT <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                    strSQL_OpenDoor += " and VCardEndValidDT >= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";//and (VerificationResult='" + "4" + "' or VerificationResult is null or VerificationResult='" + "" + "' or VerificationResult='" + "与服务器失连" + "'  or VerificationResult='" + "Unable to connect to the remote server" + "') "; ;
                    strSQL_OpenDoor += strLoginFrmSelectFlag;
                    strSQL_OpenDoor += " order by Id desc";
                    dtOpenDoor = SQLHelper.DTQuery(strSQL_OpenDoor);
                    if (dtOpenDoor.Rows.Count > 0)
                    {
                        strFloor = dtOpenDoor.Rows[0]["RPRoomNo"].ToString().Trim();
                        strSName = dtOpenDoor.Rows[0]["VName"].ToString().Trim();
                        strSNo = dtOpenDoor.Rows[0]["VNo"].ToString().Trim();
                        //  strSActualNo = dtOpenDoor.Rows[0]["VActualNo"].ToString().Trim();
                        strSDDetailName = dtOpenDoor.Rows[0]["VUnitName"].ToString().Trim();
                        strApiTypeName = dtOpenDoor.Rows[0]["ApiTypeName"].ToString().Trim();
                        strVisitorType = dtOpenDoor.Rows[0]["VisitorType"].ToString().Trim();
                        strSIdNo = dtOpenDoor.Rows[0]["VIdNo"].ToString().Trim();
                        strGONo = dtOpenDoor.Rows[0]["GONO"].ToString().Trim();
                        strSSex = dtOpenDoor.Rows[0]["VSex"].ToString().Trim();

                        strSActualNo = dtOpenDoor.Rows[0]["VIdNo"].ToString().Trim();//只能这样处理，不然同一人通行会出现多个豆腐块界面。

                        #region//2.1.判断是访客二维码
                        bool blAccessPermissions = false;//通行权限  false:没有权限  true:合法权限
                        if (dtOpenDoor.Rows[0]["AId"].ToString().Trim() == "0")//全部区域
                        {
                            blAccessPermissions = true;
                        }
                        else
                        {
                            for (int k = 0; k < DoorTable.Rows.Count; k++)
                            {
                                if (DoorTable.Rows[k]["ADId"].ToString().Trim() == dtOpenDoor.Rows[0]["AId"].ToString().Trim() && DoorTable.Rows[k]["MSNo"].ToString().Trim() == strMName)
                                {
                                    blAccessPermissions = true;
                                    break;
                                }
                            }
                        }
                        if (blAccessPermissions == true)
                        {
                            //可能存在多条卡号是一样的且开门权限都是合法但进出次数不一样的情况。所以访客软件里保存来访记录前得根据卡号更改发卡表里这些合法记录为非法卡。
                            //MessageBox.Show(dtOpenDoor.Rows.Count.ToString ());
                            strFKId = dtOpenDoor.Rows[0]["Id"].ToString().Trim();
                            if (Event.Reader == 0)//进门
                            {
                                strReadHeadNote = "进门";
                                #region//进门
                                int iEnterCount = 0;
                                try
                                {
                                    iEnterCount = Convert.ToInt32(dtOpenDoor.Rows[0]["EnterCount"].ToString().Trim());
                                }
                                catch
                                {
                                    iEnterCount = 0;
                                }
                                if (iEnterCount < Convert.ToInt32(LoginFrm.strAllowedEnterCount.ToString()))
                                {
                                    Ack = true;
                                    OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                                    relay = Event.Reader;
                                    time = Convert.ToUInt16(LoginFrm.strAllowedOpenDoorDelaySecond);
                                    card = strCardNoTemp;// "23456"; 
                                    if (card == "")
                                    {
                                        card = "0";
                                    }
                                    voice = dtOpenDoor.Rows[0]["VName"].ToString().Trim() + "进门";  // "测试语音";
                                    name = dtOpenDoor.Rows[0]["VName"].ToString().Trim(); // "姓名"; 
                                    note = "进门";// "进门出门"; 
                                    etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";           
                                    ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);
                                    iEnterCount++;


                                    //记录最后一次入闸时间,进入次数加1

                                    strSQL_OpenDoor = "update " + strT_VisitorAccessInf + " set EnterDoorDT='" + System.DateTime.Now.ToString() + "' ,EnterCount='" + iEnterCount.ToString() + "'  ";
                                    strSQL_OpenDoor += " where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString().Trim() + "' ";
                                    strSQL_OpenDoor += strLoginFrmSelectFlag;
                                    SQLHelper.ExecuteSql(strSQL_OpenDoor);


                                    strRecordType = "1";//进出记录
                                    dGORecord_Count++;
                                    SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                                    setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp.ToString(), strDTNow, Event.EventType.ToString(), "Visitor_Card");
                                }
                                else
                                {
                                    dSpecialRecord_Count++;
                                    SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_SpecialRecord);
                                    setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp.ToString(), strDTNow, Event.EventType.ToString(), "0");
                                }
                                #endregion
                            }
                            else if (Event.Reader == 1)//出门
                            {
                                strReadHeadNote = "出门";
                                #region
                                int iLeaveCount = 0;
                                try
                                {
                                    iLeaveCount = Convert.ToInt32(dtOpenDoor.Rows[0]["LeaveCount"].ToString().Trim());
                                }
                                catch
                                {
                                    iLeaveCount = 0;
                                }
                                if (iLeaveCount < Convert.ToInt32(LoginFrm.strAllowedLeaveCount.ToString()))
                                {
                                    #region
                                    Ack = true;
                                    OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                                    relay = Event.Reader;
                                    time = Convert.ToUInt16(LoginFrm.strAllowedOpenDoorDelaySecond);
                                    card = strCardNoTemp;// "23456"; 
                                    if (card == "")
                                    {
                                        card = "0";
                                    }
                                    voice = dtOpenDoor.Rows[0]["VName"].ToString().Trim() + "出门";  // "测试语音";
                                    name = dtOpenDoor.Rows[0]["VName"].ToString().Trim(); // "姓名"; 
                                    note = "出门";// "进门出门"; 
                                    etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";           
                                    ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);
                                    iLeaveCount++;

                                    string strSQL_Temp = "";
                                    if (iLeaveCount >= Convert.ToInt32(LoginFrm.strAllowedLeaveCount.ToString()))
                                    {
                                        //有离开即设置来访记录为离开状态
                                        strSQL_Temp = "update " + strT_VisitorAccessInf + " set LeaveCount ='" + iLeaveCount.ToString() + "' ,LeaveDoorDT ='" + System.DateTime.Now.ToString() + "', LeaDT='" + System.DateTime.Now.ToString() + "' ,LeaDNo='" + "0" + "',";
                                        strSQL_Temp += "LeaDDetailName='" + strMName + "闸机出门自动签离" + "',LeaveNormal='" + "1" + "'  ,LeaOperatorNo='" + LoginFrm.strOperatorNo + "',LeaOperatorActualNo='" + LoginFrm.strOperatorActualNo + "',";
                                        strSQL_Temp += "LeaOperatorName='" + LoginFrm.strOperatorName + "' where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString() + "' ";
                                        strSQL_Temp += strLoginFrmSelectFlag;
                                        SQLHelper.ExecuteSql(strSQL_Temp);
                                    }
                                    else
                                    {
                                        strSQL_Temp = "update " + strT_VisitorAccessInf + " set LeaveDoorDT ='" + System.DateTime.Now.ToString() + "',  LeaveCount ='" + iLeaveCount.ToString() + "'   where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString() + "' ";
                                        strSQL_Temp += strLoginFrmSelectFlag;
                                        SQLHelper.ExecuteSql(strSQL_Temp);
                                    }



                                    strRecordType = "1";//进出记录
                                    dGORecord_Count++;
                                    SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                                    setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp.ToString(), strDTNow, Event.EventType.ToString(), "Visitor_Card");
                                    #endregion
                                }
                                else
                                {
                                    dSpecialRecord_Count++;
                                    SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_SpecialRecord);
                                    setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp.ToString(), strDTNow, Event.EventType.ToString(), "0");
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            dSpecialRecord_Count++;
                            SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_SpecialRecord);
                            setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp.ToString(), strDTNow, Event.EventType.ToString(), "0");
                        }
                        #endregion
                    }
                    else
                    {
                        DataTable dt_ConTem = new DataTable();
                        string strSQL_ConTem = "select  Id , VNo  ,VIdNo,VActualNo ,VName ,VNameMCode ,VSex,VNation ,VBirthdate ,VIdType  ,VIdNo ,VPermanentAddress ,VVisitReason,";
                        strSQL_ConTem += " VCarryGoods ,VUnitName  ,VUnitNameMCode ,VCarNo  ,VPhone ,VVisitPCount ,VSumVisitCount ,RDID  ,RDDetailName  ,RDOPhone,RDEPhone,";
                        strSQL_ConTem += "RPNo ,RPActualNo ,RPName  ,RPMCode ,RPSex ,RPNation ,RPMPhone ,RPRoomNo ,RPDuties ,RegDT ,RegDNo ,RegDDetailName ,RegOperatorNo ,RegOperatorActualNo ,RegOperatorName,";
                        strSQL_ConTem += "  VCardNo ,VCardStatus ,VCardIssuer  ,VRemarks ,VCardCNo ,VCardCActualNo ,VCardCName ,VCardValidDT ,RSecondWay ,DigitalSignature ,LeaderName ,LeaderIdType ,LeaderIdNo,";
                        strSQL_ConTem += "  VPolice ,VValidStartToEnd ,Flag,VCardStartValidDT,VCardEndValidDT ,AId,VPlace,DataSource ,BatchImport ,UpLoadToMachine ,LeaveNormal ,VCardSerialNo ,EnterCount ,LeaveCount ,VIdWLNo,";
                        strSQL_ConTem += "  VIdWLNoToDec,BarCodeNo ,ApiType ,ApiName,VisitorType,RPRoomNo from XXCLOUD.dbo.T_LongTemCardInf where (VCardNo ='" + strCardNoTemp + "'  ) and VCardStatus ='" + "0" + "' ";//Id ,VCardNo,EnterCount,LeaveCount,DigitalSignature 
                        strSQL_ConTem += " and VCardStartValidDT <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                        strSQL_ConTem += " and VCardEndValidDT >= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                        strSQL_ConTem += strLoginFrmSelectFlag;
                        strSQL_ConTem += " order by Id desc";
                        dt_ConTem = SQLHelper.DTQuery(strSQL_ConTem);

                        //可能存在多条卡号是一样的且开门权限都是合法但进出次数不一样的情况。所以访客软件里保存来访记录前得根据卡号更改发卡表里这些合法记录为非法卡。
                        if (dt_ConTem.Rows.Count > 0)//前台普通登记
                        {
                            #region//2.访客临时卡
                            strFloor = dt_ConTem.Rows[0]["RPRoomNo"].ToString().Trim();
                            strSName = dt_ConTem.Rows[0]["VName"].ToString().Trim();
                            strSNo = dt_ConTem.Rows[0]["VNo"].ToString().Trim();
                            strSActualNo = dt_ConTem.Rows[0]["VActualNo"].ToString().Trim();
                            strSDDetailName = dt_ConTem.Rows[0]["VUnitName"].ToString().Trim();
                            strApiTypeName = dt_ConTem.Rows[0]["ApiName"].ToString().Trim();
                            strVisitorType = dt_ConTem.Rows[0]["VisitorType"].ToString().Trim();
                            strSIdNo = dt_ConTem.Rows[0]["VIdNo"].ToString().Trim();
                            strSSex = dt_ConTem.Rows[0]["VSex"].ToString().Trim();

                            if (Event.Reader == 0)//进门
                            {
                                strReadHeadNote = "进门";
                                #region//进门
                                Ack = true;
                                OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                                relay = Event.Reader;
                                time = Convert.ToUInt16(LoginFrm.strAllowedOpenDoorDelaySecond);
                                card = strCardNoTemp;// "23456"; 
                                if (card == "")
                                {
                                    card = "0";
                                }
                                voice = dt_ConTem.Rows[0]["VName"].ToString().Trim() + "进门";  // "测试语音";
                                name = dt_ConTem.Rows[0]["VName"].ToString().Trim(); // "姓名"; 
                                note = "进门";// "进门出门"; 
                                etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";           
                                ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);

                                //记录最后一次入闸时间
                                strSQL_OpenDoor = "update " + strT_VisitorAccessInf + " set EnterDoorDT='" + System.DateTime.Now.ToString() + "',EnterCount='" + "0" + "'  ";
                                strSQL_OpenDoor += " where DigitalSignature ='" + dt_ConTem.Rows[0]["DigitalSignature"].ToString().Trim() + "'";
                                strSQL_OpenDoor += strLoginFrmSelectFlag;
                                SQLHelper.ExecuteSql(strSQL_OpenDoor);

                                strRecordType = "1";//进出记录
                                dGORecord_Count++;
                                SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                                setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp.ToString(), strDTNow, Event.EventType.ToString(), "Visitor_Card");
                                #endregion
                            }
                            else if (Event.Reader == 1)//出门
                            {
                                strReadHeadNote = "出门";
                                #region
                                Ack = true;
                                OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                                relay = Event.Reader;
                                time = Convert.ToUInt16(LoginFrm.strAllowedOpenDoorDelaySecond);
                                card = strCardNoTemp;// "23456"; 
                                if (card == "")
                                {
                                    card = "0";
                                }
                                voice = dt_ConTem.Rows[0]["VName"].ToString().Trim() + "进门";  // "测试语音";
                                name = dt_ConTem.Rows[0]["VName"].ToString().Trim(); // "姓名"; 
                                note = "进门";// "进门出门"; 
                                etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";           
                                ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);



                                //记录最后一次出闸时间
                                strSQL_OpenDoor = "update " + strT_VisitorAccessInf + " set LeaveDoorDT='" + System.DateTime.Now.ToString() + "', LeaveCount='" + "0" + "' ";
                                strSQL_OpenDoor += " where DigitalSignature ='" + dt_ConTem.Rows[0]["DigitalSignature"].ToString().Trim() + "'";
                                strSQL_OpenDoor += strLoginFrmSelectFlag;
                                SQLHelper.ExecuteSql(strSQL_OpenDoor);

                                strRecordType = "1";//进出记录
                                dGORecord_Count++;
                                SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                                setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp.ToString(), strDTNow, Event.EventType.ToString(), "Visitor_Card");

                                #endregion

                            }
                            #endregion
                        }
                        else
                        {
                            #region//3.VIP卡
                            if (LoginFrm.strAllowedVIPOpenDoorDelay == "1")//允许开启VIP卡功能
                            {
                                strSQL_OpenDoor = "select top 1 Id ,SName,SSex,SIdNo,SCardNo,EnterCount,LeaveCount,AId ,SNo,SActualNo,SDDetailName  from XXCLOUD.dbo.T_VIPCardStaffInf where SCardNo ='" + strCardNoTemp + "'  ";//and MJEnabled ='" + "1" + "' ";
                                strSQL_OpenDoor += " and MJCardValidStart <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                                strSQL_OpenDoor += " and MJCardValidEnd >= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                                strSQL_OpenDoor += strLoginFrmSelectFlag;
                                strSQL_OpenDoor += " order by Id desc";
                                dtOpenDoor = SQLHelper.DTQuery(strSQL_OpenDoor);
                                if (dtOpenDoor.Rows.Count > 0)
                                {
                                    strSName = dtOpenDoor.Rows[0]["SName"].ToString().Trim();
                                    strSNo = dtOpenDoor.Rows[0]["SNo"].ToString().Trim();
                                    strSActualNo = dtOpenDoor.Rows[0]["SActualNo"].ToString().Trim();
                                    strSDDetailName = dtOpenDoor.Rows[0]["SDDetailName"].ToString().Trim();
                                    strSIdNo = dt_ConTem.Rows[0]["SIdNo"].ToString().Trim();
                                    strSSex = dt_ConTem.Rows[0]["SSex"].ToString().Trim();

                                    strApiTypeName = "";
                                    strVisitorType = "贵宾卡";
                                    #region//一、VIP卡开门
                                    bool blAccessPermissions = false;//1.判断是否具有通行权限  false:没有权限  true:合法权限
                                    if (dtOpenDoor.Rows[0]["AId"].ToString().Trim() == "0" || dtOpenDoor.Rows[0]["AId"].ToString().Trim() == null)//全部区域
                                    {
                                        blAccessPermissions = true;
                                    }
                                    else
                                    {
                                        for (int k = 0; k < DoorTable.Rows.Count; k++)
                                        {
                                            if (DoorTable.Rows[k]["ADId"].ToString().Trim() == dtOpenDoor.Rows[0]["AId"].ToString().Trim() && DoorTable.Rows[k]["MSNo"].ToString().Trim() == Event.ID.ToString())
                                            {
                                                blAccessPermissions = true;
                                                break;
                                            }
                                        }
                                    }
                                    //2.如果具有通行权限，则判断哪个控制器的进出
                                    if (blAccessPermissions == true)
                                    {
                                        //可能存在多条卡号是一样的且开门权限都是合法但进出次数不一样的情况。所以访客软件里保存来访记录前得根据卡号更改发卡表里这些合法记录为非法卡。


                                        strFKId = dtOpenDoor.Rows[0]["Id"].ToString().Trim();
                                        if (dt_VIPCard.Rows.Count > 0)
                                        {
                                            int k = 0;
                                            bool blExist = false;
                                            for (; k < dt_VIPCard.Rows.Count; k++)
                                            {
                                                if (dt_VIPCard.Rows[k]["MSNO"].ToString().Trim() == Event.SerialNo)
                                                {
                                                    blExist = true;
                                                    break;
                                                }
                                            }
                                            if (blExist == true)
                                            {
                                                if (dt_VIPCard.Rows[k]["Flag"].ToString().Trim() == "0")
                                                {
                                                    dt_VIPCard.Rows[k]["Direction"] = "进门";
                                                    dt_VIPCard.Rows[k]["Flag"] = "1";
                                                    dt_VIPCard.AcceptChanges();
                                                    #region//此门禁控制器没开启VIP卡功能，则马上开启。如果已开启，则不执行以下这段代码
                                                    Ack = true;
                                                    OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                                                    relay = Event.Reader;
                                                    time = Convert.ToUInt16(LoginFrm.strAllowedVIPOpenDoorDelaySecond);
                                                    card = strCardNoTemp;// "23456"; 
                                                    if (card == "")
                                                    {
                                                        card = "0";
                                                    }
                                                    voice = dtOpenDoor.Rows[0]["SName"].ToString().Trim() + "出门";  // "测试语音";
                                                    name = dtOpenDoor.Rows[0]["SName"].ToString().Trim(); // "姓名"; 
                                                    note = "开启VIP卡功能";// "进门出门"; 
                                                    etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";   



                                                    ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);
                                                    if (!accessMan.OpenToEvent(Event.SerialNo, time))//同时开启2个门
                                                    {
                                                        //MessageBox.Show("设备连接异常，无法远程开门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                                        //return;
                                                    }


                                                    //记录最后一次入闸时间,进入次数加1

                                                    strSQL_OpenDoor = "update XXCLOUD.dbo.T_VIPCardStaffInf  set EnterDoorDT='" + System.DateTime.Now.ToString() + "'  ";
                                                    strSQL_OpenDoor += " where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString().Trim() + "' ";
                                                    strSQL_OpenDoor += strLoginFrmSelectFlag;
                                                    SQLHelper.ExecuteSql(strSQL_OpenDoor);

                                                    strRecordType = "1";//进出记录
                                                    dGORecord_Count++;
                                                    SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                                                    setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, System.DateTime.Now.ToString(), Event.EventType.ToString(), "VIPCardOpen");
                                                    #endregion
                                                }
                                                else if (dt_VIPCard.Rows[k]["Flag"].ToString().Trim() == "1")
                                                {
                                                    #region
                                                    dt_VIPCard.Rows[k]["Direction"] = "";
                                                    dt_VIPCard.Rows[k]["Flag"] = "0";
                                                    dt_VIPCard.AcceptChanges();


                                                    if (!accessMan.CloseDoor(Event.SerialNo, byte.Parse("100")))//同时关闭两个门
                                                    {
                                                        //MessageBox.Show("设备连接异常，无法远程关门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                                        //return;
                                                    }




                                                    strSQL_OpenDoor = "update XXCLOUD.dbo.T_VIPCardStaffInf  set EnterDoorDT='" + System.DateTime.Now.ToString() + "'  ";
                                                    strSQL_OpenDoor += " where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString().Trim() + "' ";
                                                    strSQL_OpenDoor += strLoginFrmSelectFlag;
                                                    SQLHelper.ExecuteSql(strSQL_OpenDoor);

                                                    strRecordType = "1";//进出记录
                                                    dGORecord_Count++;
                                                    SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                                                    setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, System.DateTime.Now.ToString(), Event.EventType.ToString(), "VIPCardClose");
                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                #region//当前VIP卡表中不存在，则新增
                                                DataRow dr = dt_VIPCard.NewRow();
                                                dr["MSNO"] = Event.SerialNo;
                                                dr["Flag"] = "1";
                                                dt_VIPCard.Rows.Add(dr);
                                                #region//此门禁控制器没开启VIP卡功能，则马上开启。如果已开启，则不执行以下这段代码


                                                Ack = true;
                                                OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                                                relay = Event.Reader;
                                                time = Convert.ToUInt16(LoginFrm.strAllowedVIPOpenDoorDelaySecond);
                                                card = strCardNoTemp;// "23456"; 
                                                if (card == "")
                                                {
                                                    card = "0";
                                                }
                                                voice = dtOpenDoor.Rows[0]["SName"].ToString().Trim() + "出门";  // "测试语音";
                                                name = dtOpenDoor.Rows[0]["SName"].ToString().Trim(); // "姓名"; 
                                                note = "开启VIP卡功能";// "进门出门"; 
                                                etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";       


                                                ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);


                                                if (!accessMan.OpenToEvent(Event.SerialNo, time))//同时开启2个门
                                                {
                                                    //MessageBox.Show("设备连接异常，无法远程开门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                                    //return;
                                                }


                                                //记录最后一次入闸时间,进入次数加1

                                                strSQL_OpenDoor = "update XXCLOUD.dbo.T_VIPCardStaffInf  set EnterDoorDT='" + System.DateTime.Now.ToString() + "'  ";
                                                strSQL_OpenDoor += " where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString().Trim() + "' ";
                                                strSQL_OpenDoor += strLoginFrmSelectFlag;
                                                SQLHelper.ExecuteSql(strSQL_OpenDoor);

                                                strRecordType = "1";//进出记录
                                                dGORecord_Count++;
                                                SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                                                setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, System.DateTime.Now.ToString(), Event.EventType.ToString(), "VIPCardOpen");
                                                #endregion
                                                #endregion
                                            }
                                        }
                                        else
                                        {
                                            #region//当前VIP卡表中不存在，则新增
                                            DataRow dr = dt_VIPCard.NewRow();
                                            dr["MSNO"] = Event.SerialNo;
                                            dr["Flag"] = "1";
                                            dt_VIPCard.Rows.Add(dr);
                                            #region//此门禁控制器没开启VIP卡功能，则马上开启。如果已开启，则不执行以下这段代码


                                            Ack = true;
                                            OpenDoor = Convert.ToByte(true);  // 0 不开 1开 2报警
                                            relay = Event.Reader;
                                            time = Convert.ToUInt16(LoginFrm.strAllowedVIPOpenDoorDelaySecond);
                                            card = strCardNoTemp;// "23456"; 
                                            if (card == "")
                                            {
                                                card = "0";
                                            }
                                            voice = dtOpenDoor.Rows[0]["SName"].ToString().Trim() + "出门";  // "测试语音";
                                            name = dtOpenDoor.Rows[0]["SName"].ToString().Trim(); // "姓名"; 
                                            note = "开启VIP卡功能";// "进门出门"; 
                                            etime = DateTime.Now.ToLocalTime().ToString(); // "2016-12-26 12:25:34";       


                                            ShowEvent(Event.SerialNo, Event.ID, Event.Datetime, Event.Reader, relay, Event.EventType, Event.Alarm, Event.Value);

                                            if (!accessMan.OpenToEvent(Event.SerialNo, time))//同时开启2个门
                                            {
                                                //MessageBox.Show("设备连接异常，无法远程开门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                                //return;
                                            }

                                            //记录最后一次入闸时间,进入次数加1

                                            strSQL_OpenDoor = "update XXCLOUD.dbo.T_VIPCardStaffInf  set EnterDoorDT='" + System.DateTime.Now.ToString() + "'  ";
                                            strSQL_OpenDoor += " where Id ='" + dtOpenDoor.Rows[0]["Id"].ToString().Trim() + "' ";
                                            strSQL_OpenDoor += strLoginFrmSelectFlag;
                                            SQLHelper.ExecuteSql(strSQL_OpenDoor);

                                            strRecordType = "1";//进出记录
                                            dGORecord_Count++;
                                            SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_GORecord);
                                            setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, System.DateTime.Now.ToString(), Event.EventType.ToString(), "VIPCardOpen");
                                            #endregion
                                            #endregion
                                        }
                                    }


                                    else
                                    {
                                        dSpecialRecord_Count++;
                                        SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_SpecialRecord);
                                        setUIControlsValue.Invoke(Event.Reader.ToString(), strCardNoTemp, strDTNow, Event.EventType.ToString(), "0");
                                    }
                                    #endregion
                                }
                                else
                                {
                                    dSpecialRecord_Count++;
                                    SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_SpecialRecord);
                                    setUIControlsValue.Invoke(Event.Reader.ToString(), Event.Value.ToString(), strDTNow, Event.EventType.ToString(), "0");
                                }

                            }
                            #endregion
                        }

                    }
                    #endregion
                }
            }
            //string strSQL_Temp2 = "insert into " + strT_MJRecordAccessInf + "(MSNo,MName,MachineId ,LogIndex,CardNo ,DoorId ,DName ,ReadHeadId ,ReadHeadNote ,RecordDT,RecordDate,";
            //strSQL_Temp2 += " WarnCode,RecordNote ,RecordType ,DownLoadDT ,ONo ,OActualNo ,OName,VPlace,FKId,SNo,SActualNo,SName,SDDetailName,ApiTypeName,VisitorType,SRoomNo)values('" + Event.ID.ToString().Substring(0, 6) + "','" + strMName + "',";//MSNo,MName
            //strSQL_Temp2 += "'" + strMachineId + "','" + "" + "','" + strCardNoTemp + "','" + Event.Door.ToString() + "',";//MachineId ,LogIndex,CardNo,DoorId 
            //strSQL_Temp2 += "'" + strDName + "','" + Event.Reader.ToString() + "','" + strReadHeadNote + "','" + strDTNow.ToString() + "','" + System.DateTime.Now.ToString("yyyy-MM-dd") + "','" + Event.EventType.ToString() + "',";
            //strSQL_Temp2 += "'" + strNote + "','" + strRecordType + "','" + System.DateTime.Now.ToString() + "',";
            //strSQL_Temp2 += "'" + LoginFrm.strOperatorNo + "','" + LoginFrm.strOperatorActualNo + "','" + LoginFrm.strOperatorName + "','" + strEndUserName + "','" + strFKId + "',";
            //strSQL_Temp2 += "'" + strSNo + "','" + strSActualNo + "','" + strSName + "','" + "test" + "','" + strApiTypeName + "','" + strVisitorType + "','" + strFloor + "');";
            //SQLHelper.ExecuteSql(strSQL_Temp2);
            //strSQL_Temp1 = "insert into "+strT_MJRecordAccessInf+" (MSNo,MName,MachineId ,LogIndex,CardNo ,DoorId ,DName ,ReadHeadId ,ReadHeadNote ,RecordDT,";
            //strSQL_Temp1 += " WarnCode,RecordNote ,RecordType ,DownLoadDT ,ONo ,OActualNo ,OName,VPlace,FKId,SNo,SActualNo,SName,SDDetailName,ApiTypeName,VisitorType,SRoomNo)values('" + Event.ID.ToString().Substring(0, 6) + "','" + strMName + "',";//MSNo,MName
            //strSQL_Temp1 += "'" + strMachineId + "','" + "" + "','" + strCardNoTemp + "','" + Event.Door.ToString() + "',";//MachineId ,LogIndex,CardNo,DoorId 
            //strSQL_Temp1 += "'" + strDName + "','" + Event.Reader.ToString() + "','" + strReadHeadNote + "','" + strDTNow.ToString() + "','" + Event.EventType.ToString() + "',";
            //strSQL_Temp1 += "'" + strNote + "','" + strRecordType + "','" + System.DateTime.Now.ToString() + "',";
            //strSQL_Temp1 += "'" + LoginFrm.strOperatorNo + "','" + LoginFrm.strOperatorActualNo + "','" + LoginFrm.strOperatorName + "','" + strEndUserName + "','" + strFKId + "',";
            //strSQL_Temp1 += "'" + strSNo + "','" + strSActualNo + "','" + strSName + "','" + strSDDetailName + "','" + strApiTypeName + "','" + strVisitorType + "','" + strFloor + "');";
            //SQLHelper.ExecuteSql(strSQL_Temp1);
            //SBder.AppendFormat(strSQL_Temp1, new object[0]);
            //iCountExecute++;
            //if (iCountExecute >= 20)
            //{
            //    if (SQLHelper.ExecuteNonQueryTran(SBder.ToString().TrimEnd(';').Split(';')) == true)
            //    {
            //        iCountExecute = 0;
            //        SBder.Length = 0;
            //    }

            #region 消息推送所有代码
            string appid = "wx31583db6413b8fed";
            string secret = "22e05f670434da88586fef1c7eab275c";
            string className = "";

            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;

            HttpHelper http = new HttpHelper();

            string json = http.GetResponseString(HttpHelper.CreateGetHttpResponse(url));

            JavaScriptSerializer js = new JavaScriptSerializer();   //实例化一个能够序列化数据的类
            AccessClass list = js.Deserialize<AccessClass>(json);    //将json数据转化为对象类型并赋值给list
            //textBox1.Text = list.access_token;
            string access_token = list.access_token;


            #region 获取touser_openid
            string openId = "";
            //string resultLink = SQLHelper4XXYXT.LinkSqlDatabase();
            string sql_getStudentInfo = "select * from XXCLOUD.dbo.T_ClassAndStudentInf a,XXCLOUD.dbo.T_SurrogateInf b where a.SActualNo=b.SActualNo and a.SActualNo=@SActualNo and b.IsRecived=1";
            SqlParameter[] pms_getStudentInfo = new SqlParameter[]{
                new SqlParameter("@SActualNo",SqlDbType.VarChar){Value=strSActualNo}
            };
            //MessageBox.Show("strSActualNo:" + strSActualNo);
            DataTable dt = SQLHelper4XXYXT.ExecuteDataTable(sql_getStudentInfo, System.Data.CommandType.Text, pms_getStudentInfo);
            //MessageBox.Show("dt.Rows.Count:" + dt.Rows.Count);
            
            if (dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    string phone = dt.Rows[i]["SurrogateMPhone"].ToString();
                    className = dt.Rows[i]["SDDetailName"].ToString();

                    string sql_getOpenId = "select * from XXCLOUD.dbo.T_WXUserInfo where Phone=@Phone";
                    SqlParameter[] pms_getOpenId = new SqlParameter[]{
                        new SqlParameter("@Phone",SqlDbType.VarChar){Value=phone}
                    };
                    DataTable dt_getOpenId = SQLHelper4XXYXT.ExecuteDataTable(sql_getOpenId, System.Data.CommandType.Text, pms_getOpenId);
                    if (dt_getOpenId.Rows.Count > 0)
                    {
                        openId = dt_getOpenId.Rows[0]["OpenId"].ToString();

                        #region 获取form_id
                        string _form_id = "";
                        //通过接收消息者的openId获得此人登录小程序后所产生的Form_id
                        string sql_getForm_id = "select top 1 *  from XXCLOUD.dbo.T_WXFormId where OpenId=@OpenId order by Id";
                        SqlParameter[] pms_getForm_id = new SqlParameter[]{
                            new SqlParameter("@OpenId",SqlDbType.VarChar){Value=openId}
                        };
                        DataTable dt_Form_id = SQLHelper4XXYXT.ExecuteDataTable(sql_getForm_id, System.Data.CommandType.Text, pms_getForm_id);
                        if (dt_Form_id.Rows.Count > 0)
                        {
                            _form_id = dt_Form_id.Rows[0]["Form_id"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("not get _form_id");
                        }
                        #endregion

                        #region 微信服务消息发送
                        string touser = openId;//需要从数据库中获取
                        string template_id = "";
                        string form_id = _form_id;//需要从数据库中获取
                        //textBox1.Text = "_form_id:" + form_id;
                        //string page = "pages/user/index?SActualNo=" + strSActualNo;
                        string page = "pages/user/index?studentName=" + strSName;
                        //var data = new TemplateModel("智慧幼儿园", "SName", DateTime.Now.ToString("yyy-MM-dd"));

                        if (strReadHeadNote == "进门")
                        {
                            template_id = "wuDEUpW5amir_awDXpCwgj7ZFPjcOPHE8nABc2drJyk";
                        }
                        else if (strReadHeadNote == "出门")
                        {
                            template_id = "oD7GRQB7Da4gsPLtbjgLM6qWFZbRNm2LghMqRmR2sZs";
                        }

                        var keyword1 = new
                        {
                            value = strSName,
                            color = "#173177"
                        };
                        var keyword2 = new
                        {
                            value = DateTime.Now.ToString("yyy-MM-dd hh:mm:ss"),
                            color = "#173177"
                        };
                        var data = new
                        {
                            keyword1 = keyword1,
                            keyword2 = keyword2
                        };
                        WeChat wechat = new WeChat();
                        string result = wechat.SendTemplete(access_token, template_id, touser, form_id, page, data);
                        //textBox2.Text = result;
                        // form_id使用后记得删除。有可能发送模板消息后，通过page里面的路径可以打开页面并请求接口，微信小程序的BUG？
                        string sql_deleteForm_id = "delete from XXCLOUD.dbo.T_WXFormId where Form_id=@Form_id";
                        SqlParameter[] pms_deleteForm_id = new SqlParameter[]{
                            new SqlParameter("@Form_id",SqlDbType.VarChar){Value=form_id}
                        };
                        object obj_deleteFromId = SQLHelper4XXYXT.ExecuteNonQuery(sql_deleteForm_id, System.Data.CommandType.Text, pms_deleteForm_id);
                        if (Convert.ToInt32(obj_deleteFromId) == 1)
                        {
                            //MessageBox.Show("删除formId成功");
                            //textBox1.Text = "删除formId成功";
                        }
                        else
                        {
                            MessageBox.Show("删除formId失败");
                        }
                        #endregion
                    }
                    else
                    {
                        MessageBox.Show("not get openId");
                    }
                }
            }
            else
            {
                MessageBox.Show("学生表里并无此人");
            }
            #endregion

            #endregion

            string strSQL_Temp2 = "insert into "+strT_MJRecordAccessInf+"(MSNo,MName,MachineId ,LogIndex,CardNo ,DoorId ,DName ,ReadHeadId ,ReadHeadNote ,RecordDT,RecordDate,";
            strSQL_Temp2 += " WarnCode,RecordNote ,RecordType ,DownLoadDT ,ONo ,OActualNo ,OName,VPlace,FKId,SNo,SActualNo,SName,SDDetailName,ApiTypeName,VisitorType,SRoomNo,SIdNo,GONO,SSex)values('" + Event.ID.ToString().Substring(0, 6) + "','" + strMName + "',";//MSNo,MName
            strSQL_Temp2 += "'" + strMachineId + "','" + "" + "','" + strCardNoTemp + "','" + Event.Door.ToString() + "',";//MachineId ,LogIndex,CardNo,DoorId 
            strSQL_Temp2 += "'" + strDName + "','" + Event.Reader.ToString() + "','" + strReadHeadNote + "','" + strDTNow.ToString() + "','" + System.DateTime.Now.ToString("yyyy-MM-dd") + "','" + Event.EventType.ToString() + "',";
            strSQL_Temp2 += "'" + strNote + "','" + strRecordType + "','" + System.DateTime.Now.ToString() + "',";
            strSQL_Temp2 += "'" + LoginFrm.strOperatorNo + "','" + LoginFrm.strOperatorActualNo + "','" + LoginFrm.strOperatorName + "','" + strEndUserName + "','" + strFKId + "',";
            strSQL_Temp2 += "'" + strSNo + "','" + strSActualNo + "','" + strSName + "','" + className + "','" + strApiTypeName + "','" + strVisitorType + "','" + strFloor + "',";
            strSQL_Temp2 += "'" + strSIdNo + "','" + strGONo + "','" + strSSex + "');";
            SQLHelper.ExecuteSql(strSQL_Temp2);
            iCountExecute = 0;
            SBder.Length = 0;
        }
        public struct AccessClass
        {
            public string access_token { get; set; }
        }
        void ShowEvent(string SerialNo, string ID, DateTime Datetime, byte reader, byte relay, byte EventType, byte Alarm, string Value)
        {
            listBox1.Items.Clear();
            ShowStr("");
            ShowStr("============事件记录=====================");
            ShowStr("序列号标识: " + SerialNo + "-" + ID);
            ShowStr("      时间: " + Datetime.ToString());
            ShowStr("      读头: " + reader.ToString());
            ShowStr("    继电器: " + relay.ToString());
            ShowStr("  事件类型: " + EventType.ToString() + " " + TcpPackge.EventTypeStr(EventType));
            if (EventType == TcpPackge.NET_DATA_TYPE_ALARM)
            {
                ShowStr("    记录号: " + Value.ToString());
            }
            ShowStr("        值: " + Value.ToString());

        }
        void StatusHandler(XXY_VisitorMJAsst.RAcsStatus Status, string SerialNo, string Version, string ID, DateTime Datetime, byte reader, byte Door,
      byte DoorStatus, byte Ver, Boolean Online, ref byte relay, ref byte OpenDoor, ref Boolean Ack)
        {
            Ack = true;
            OpenDoor = 0;// 0 不开 1开 2报警
            relay = 0;
            ShowEventStatus(SerialNo, Version, ID, Datetime, DoorStatus, Ver, Online, Status.T1, Status.H1, Status.T2, Status.H2);
        }
        void ShowEventStatus(string SerialNo, string Version, string ID, DateTime Datetime, byte DoorStatus, byte Ver, Boolean Online, float T1, float H1, float T2, float H2)
        {
            return;
            ShowStr("");
            ShowStr("============状态记录=====================");

            ShowStr("      在线: " + Online.ToString());

            if (Online)
            {
                ShowStr("序列号标识: " + SerialNo + "-" + ID);
                ShowStr("      时间: " + Datetime.ToString());
                ShowStr("    门状态: " + DoorStatus.ToString());
                ShowStr("      版本: " + Ver.ToString() + "/" + Version);
                ShowStr("   温湿度1: " + T1.ToString() + "  " + H1.ToString());
                ShowStr("   温湿度2: " + T2.ToString() + "  " + H2.ToString());
            }

        }
        void ShowStr(string str)
        {
            if (this.InvokeRequired)//等待异步 
            {
                ThreadShowMsg fc = new ThreadShowMsg(ShowStr);
                this.BeginInvoke(fc, new object[] { str });//通过代理调用刷新方法 
            }
            else
            {
                index++;
                listBox1.BeginUpdate();
                listBox1.Items.Insert(0, str + "    " + System.DateTime.Now.ToString());
                listBox1.EndUpdate();
            }
        }
        void OnDisconnect()
        {
            ShowMsg("状态:false");
        }
        private delegate void ThreadShowMsg(string st);
        void ShowMsg(string str)
        {
            if (this.InvokeRequired)//等待异步 
            {
                ThreadShowMsg fc = new ThreadShowMsg(ShowMsg);
                this.BeginInvoke(fc, new object[] { str });//通过代理调用刷新方法 
            }
            else
            {
                //listBox1.Items.Clear();
                index++;
                str = string.Concat(index.ToString(), "=>", str);

                listBox1.BeginUpdate();
                listBox1.Items.Insert(0, "");
                listBox1.Items.Insert(0, str);
                listBox1.EndUpdate();
            }
        }
        private delegate void ThreadWork(byte[] buff, int len, string st);
        void ShowHexMsg(byte[] buff, int len, string str)
        {

            if (this.InvokeRequired)//等待异步 
            {
                ThreadWork fc = new ThreadWork(ShowHexMsg);
                this.BeginInvoke(fc, new object[] { buff, len, str });//通过代理调用刷新方法 
            }
            else
            {
                index++;
                str = string.Concat(index.ToString(), "=>", str);
                listBox1.BeginUpdate();
                listBox1.Items.Insert(0, "");
                listBox1.Items.Insert(0, str + "    " + System.DateTime.Now.ToString());
                listBox1.EndUpdate();
            }
        }
        #endregion
        private void D_RemoterControlFrm_Load(object sender, EventArgs e)
        {
            //加载宇泛人脸机数量和奥普控制器数量
            strSQL = "select AItem1 from XXCLOUD.dbo.T_YearInf  where ANo='" + LoginFrm.strCAccout.Substring(1, 4).Trim() + "'";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0) 
            {
                strSQL = myTable.Rows[0]["AItem1"].ToString().Trim();
                if (strSQL.Trim() != "")
                {
                    strSQL = SQLHelper.DecryptString(strSQL);
                    string[] Count = strSQL.Trim().Split(',');//以","分离字符串
                    strYFFaceMachineSum = Count[0].ToString();
                    strAPMJMachineSum = Count[1].ToString();
                }
            }
            else
            {
                strYFFaceMachineSum = "1";
                strAPMJMachineSum = "1";
            }
            if (strAPMJMachineSum == "2" && strYFFaceMachineSum == "4")
            {
                strAPMJMachineSum = "10";
            }
            //VIP卡
            dt_VIPCard.Columns.Add("MSNO");
            dt_VIPCard.Columns.Add("Direction");//进门，出门
            dt_VIPCard.Columns.Add("Flag");//0表示当前门禁控制器未启用VIP卡功能

            dtStart = System.DateTime.Now;
            OperatorLog("进入云访客门禁网络通信助手,开始运行时间：" + dtStart.ToString(), "1");
            LoaddtCardNo();

            #region//同步数据库服务器时间到本机
            //strSQL = " select GETDATE() dt ";
            //DataTable NowDT = new DataTable();
            //NowDT = SQLHelper.DTQuery(strSQL);
            //if (NowDT.Rows.Count > 0)
            //{
            //    DateTime SQLDTNow = Convert.ToDateTime(NowDT.Rows[0]["dt"].ToString().Trim());//得到时间信息
            //    SystemTime MySystemTime = new SystemTime();//创建系统时间类的对象
            //    SetSystemDateTime.GetLocalTime(MySystemTime);//得到系统时间
            //    MySystemTime.vYear = 2017;// (ushort)SQLDTNow.Year;//设置年
            //    MySystemTime.vMonth = 5;// (ushort)SQLDTNow.Month;//设置月
            //    MySystemTime.vDay = 22;// (ushort)SQLDTNow.Day;//设置日
            //    MySystemTime.vHour = 23;// (ushort)SQLDTNow.Hour;//设置小时
            //    MySystemTime.vMinute = 57;// (ushort)SQLDTNow.Minute;//设置分
            //    MySystemTime.vSecond = 1;// (ushort)SQLDTNow.Second;//设置秒
            //    SetSystemDateTime.SetLocalTime(MySystemTime);//设置系统时间
            //}
            #endregion

            #region//网络通信助手
            this.listBox1.Items.Clear();
            listBox1.HorizontalScrollbar = true;
            btn_Start.Enabled = true;
            btn_Stop.Enabled = false;
            this.TSL2.Text = "  当前连接客户端数: " + userList.Count.ToString();
            string LocalName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(LocalName);
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.ToString().Trim().Contains(":") == false)
                {
                    //this.TSL2.Text = "           本机IP地址: " + ip.ToString();
                    localAddress = System.Net.IPAddress.Parse(ip.ToString());
                    break;

                }
            }
            btn_Start_Click(sender, e);
            #endregion
            try
            {

                strSQL = "select  Top "+strAPMJMachineSum+" T_MJAPMachineInf.ADId,MSNo,MName,T_MJAPDoorInf.MachineId,DoorId,DName ,MIPAddress,MCommPwd ,MCommPort";
                strSQL += " from XXCLOUD.dbo.T_MJAPDoorInf,XXCLOUD.dbo.T_MJAPMachineInf ";//读取门信息
                strSQL += " where T_MJAPDoorInf.MachineId=T_MJAPMachineInf.MachineId ";
                DoorTable = SQLHelper.DTQuery(strSQL);

                strSQL = "select  MSNo,T_MJAPMachineInf.MachineId,DoorId,ReadHeadId,ReadHeadNote  ";//读取读头信息
                strSQL += " from XXCLOUD.dbo.T_MJAPReadHeadInf,XXCLOUD.dbo.T_MJAPMachineInf  ";//读取门信息
                strSQL += " where T_MJAPReadHeadInf.MachineId=T_MJAPMachineInf.MachineId ";
                HeadReadTable = SQLHelper.DTQuery(strSQL);

                #region//设置grid1样式
                this.grid_Door.AutoRedraw = false;
                this.grid_Door.Rows = 1;
                this.grid_Door.Cols = 1;
                this.grid_Door.DisplayRowNumber = true;
                this.grid_Door.DisplayRowArrow = true;
                this.grid_Door.MultiSelect = false;
                this.grid_Door.Row(0).Visible = false;
                this.grid_Door.Column(0).Visible = false;
                this.grid_Door.EnableTabKey = true;
                this.grid_Door.EnableVisualStyles = true;
                this.grid_Door.DisplayFocusRect = true;//返回或设置控件在当前活动单元格是否显示一个虚框
                this.grid_Door.FixedRowColStyle = FlexCell.FixedRowColStyleEnum.Light3D;//返回或设置固定行/列的样式
                this.grid_Door.DefaultFont = new Font("宋体", 9);
                this.grid_Door.BorderStyle = FlexCell.BorderStyleEnum.Light3D;
                this.grid_Door.BackColorFixed = Color.FromArgb(225, 225, 225);//固定行／列的颜色
                this.grid_Door.BackColorFixedSel = Color.FromArgb(225, 225, 225);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_Door.BackColorSel = Color.FromArgb(210, 255, 166);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_Door.BackColorBkg = Color.FromArgb(255, 255, 255);//返回和设置空白区域的背景色(这里为白色)
                this.grid_Door.CellBorderColorFixed = Color.Red;//返回或设置固定行和固定列上的单元格边框的颜色
                this.grid_Door.GridColor = Color.FromArgb(148, 190, 231);//返回或设置网格线的颜色
                this.grid_Door.AllowUserReorderColumn = false;//允许操作员拖动列标题来移动整列
                this.grid_Door.AllowUserResizing = FlexCell.ResizeEnum.Both;
                this.grid_Door.AllowUserSort = true;

                this.grid_Door.HideGridLines = true;
                this.grid_Door.EnableVisualStyles = true;//显示XP效果
                this.grid_Door.DefaultRowHeight = 27;//默认行高
                this.grid_Door.EnableTabKey = true;//按Tab键时移动活动单元格
                this.grid_Door.EnterKeyMoveTo = FlexCell.MoveToEnum.NextCol;//按回车键时自动跳到下一列
                this.grid_Door.SelectionBorderColor = Color.Red;//设置selection边框的颜色
                this.grid_Door.ShowResizeTip = true;//返回或设置一个值，该值决定了操作员用鼠标调整FlexCell表格的行高或列宽时，是否显示行高或列宽的提示窗口。
                this.grid_Door.AutoRedraw = true;
                this.grid_Door.Refresh();

                this.grid_Door.Images.Add(XXY_VisitorMJAsst.Properties.Resources.Door, strImageName_SucLinked);
                this.grid_Door.Images.Add(XXY_VisitorMJAsst.Properties.Resources.OpenDoor, strImageName_OpendLinked);
                this.grid_Door.Images.Add(XXY_VisitorMJAsst.Properties.Resources.DoorOff, strImageName_ErrorLinked);
                #endregion

                #region//设置grid_GORecord样式
                this.grid_GORecord.AutoRedraw = false;
                this.grid_GORecord.Rows = 1;
                this.grid_GORecord.Cols = 1;
                this.grid_GORecord.DisplayRowArrow = true;
                this.grid_GORecord.DisplayRowNumber = true;
                this.grid_GORecord.MultiSelect = true;
                this.grid_GORecord.StartRowNumber = 1;
                this.grid_GORecord.EnableTabKey = true;
                this.grid_GORecord.EnableVisualStyles = true;
                this.grid_GORecord.SelectionMode = FlexCell.SelectionModeEnum.ByRow;//选中整行
                this.grid_GORecord.DisplayFocusRect = true;//返回或设置控件在当前活动单元格是否显示一个虚框
                this.grid_GORecord.FixedRowColStyle = FlexCell.FixedRowColStyleEnum.Light3D;//返回或设置固定行/列的样式
                this.grid_GORecord.ExtendLastCol = true;//扩展最后一列
                this.grid_GORecord.DefaultFont = new Font("宋体", 9);
                this.grid_GORecord.BorderStyle = FlexCell.BorderStyleEnum.Light3D;
                this.grid_GORecord.BackColorFixed = Color.FromArgb(225, 225, 225);//固定行／列的颜色
                this.grid_GORecord.BackColorFixedSel = Color.FromArgb(225, 225, 225);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_GORecord.BackColorSel = Color.FromArgb(210, 255, 166);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_GORecord.BackColorBkg = Color.FromArgb(255, 255, 255);//返回和设置空白区域的背景色(这里为白色)
                this.grid_GORecord.BackColor1 = Color.FromArgb(231, 235, 247);//返回或设置表格中奇数行的背景色
                this.grid_GORecord.BackColor2 = Color.FromArgb(239, 243, 255);//返回或设置表格中偶数行的背景色
                this.grid_GORecord.CellBorderColorFixed = Color.Red;//返回或设置固定行和固定列上的单元格边框的颜色
                this.grid_GORecord.GridColor = Color.FromArgb(148, 190, 231);//返回或设置网格线的颜色
                this.grid_GORecord.AllowUserReorderColumn = true;//允许操作员拖动列标题来移动整列
                this.grid_GORecord.AllowUserResizing = FlexCell.ResizeEnum.Both;
                this.grid_GORecord.AllowUserSort = true;
                this.grid_GORecord.DrawMode = FlexCell.DrawModeEnum.OwnerDraw;
                this.grid_GORecord.EnableVisualStyles = true;//显示XP效果
                this.grid_GORecord.DefaultRowHeight = 27;//默认行高
                this.grid_GORecord.EnableTabKey = true;//按Tab键时移动活动单元格
                this.grid_GORecord.EnterKeyMoveTo = FlexCell.MoveToEnum.NextCol;//按回车键时自动跳到下一列
                this.grid_GORecord.SelectionBorderColor = Color.Red;//设置selection边框的颜色
                this.grid_GORecord.ShowResizeTip = true;//返回或设置一个值，该值决定了操作员用鼠标调整FlexCell表格的行高或列宽时，是否显示行高或列宽的提示窗口。
                this.grid_GORecord.AutoRedraw = true;
                this.grid_GORecord.Refresh();

                this.grid_GORecord.Cols = 11;
                this.grid_GORecord.Cell(0, 1).Text = "设备名称";
                this.grid_GORecord.Cell(0, 2).Text = "门名称";
                this.grid_GORecord.Cell(0, 3).Text = "读头";
                this.grid_GORecord.Cell(0, 4).Text = "卡号";
                this.grid_GORecord.Cell(0, 5).Text = "事件时间";
                this.grid_GORecord.Cell(0, 6).Text = "进出状态";
                this.grid_GORecord.Cell(0, 7).Text = "备注";
                this.grid_GORecord.Cell(0, 8).Text = "姓名";
                this.grid_GORecord.Cell(0, 9).Text = "所属部门/单位";
                this.grid_GORecord.Cell(0, 10).Text = "访客类型";


                this.grid_GORecord.Column(1).Width = 120;
                this.grid_GORecord.Column(2).Width = 120;
                this.grid_GORecord.Column(3).Width = 80;
                this.grid_GORecord.Column(4).Width = 100;
                this.grid_GORecord.Column(5).Width = 150;
                this.grid_GORecord.Column(6).Width = 80;
                this.grid_GORecord.Column(7).Width = 150;
                this.grid_GORecord.Column(8).Width = 100;
                this.grid_GORecord.Column(9).Width = 200;
                this.grid_GORecord.Column(10).Width = 80;
                #endregion

                #region//设置grid_AlarmRecord样式
                this.grid_AlarmRecord.AutoRedraw = false;
                this.grid_AlarmRecord.Rows = 1;
                this.grid_AlarmRecord.Cols = 1;
                this.grid_AlarmRecord.DisplayRowArrow = true;
                this.grid_AlarmRecord.DisplayRowNumber = true;
                this.grid_AlarmRecord.MultiSelect = true;
                this.grid_AlarmRecord.StartRowNumber = 1;
                this.grid_AlarmRecord.EnableTabKey = true;
                this.grid_AlarmRecord.EnableVisualStyles = true;
                this.grid_AlarmRecord.SelectionMode = FlexCell.SelectionModeEnum.ByRow;//选中整行
                this.grid_AlarmRecord.DisplayFocusRect = true;//返回或设置控件在当前活动单元格是否显示一个虚框
                this.grid_AlarmRecord.FixedRowColStyle = FlexCell.FixedRowColStyleEnum.Light3D;//返回或设置固定行/列的样式
                this.grid_AlarmRecord.ExtendLastCol = true;//扩展最后一列
                this.grid_AlarmRecord.DefaultFont = new Font("宋体", 9);
                this.grid_AlarmRecord.BorderStyle = FlexCell.BorderStyleEnum.Light3D;
                this.grid_AlarmRecord.BackColorFixed = Color.FromArgb(225, 225, 225);//固定行／列的颜色
                this.grid_AlarmRecord.BackColorFixedSel = Color.FromArgb(225, 225, 225);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_AlarmRecord.BackColorSel = Color.FromArgb(210, 255, 166);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_AlarmRecord.BackColorBkg = Color.FromArgb(255, 255, 255);//返回和设置空白区域的背景色(这里为白色)
                this.grid_AlarmRecord.BackColor1 = Color.FromArgb(231, 235, 247);//返回或设置表格中奇数行的背景色
                this.grid_AlarmRecord.BackColor2 = Color.FromArgb(239, 243, 255);//返回或设置表格中偶数行的背景色
                this.grid_AlarmRecord.CellBorderColorFixed = Color.Red;//返回或设置固定行和固定列上的单元格边框的颜色
                this.grid_AlarmRecord.GridColor = Color.FromArgb(148, 190, 231);//返回或设置网格线的颜色
                this.grid_AlarmRecord.AllowUserReorderColumn = true;//允许操作员拖动列标题来移动整列
                this.grid_AlarmRecord.AllowUserResizing = FlexCell.ResizeEnum.Both;
                this.grid_AlarmRecord.AllowUserSort = true;
                this.grid_AlarmRecord.DrawMode = FlexCell.DrawModeEnum.OwnerDraw;
                this.grid_AlarmRecord.EnableVisualStyles = true;//显示XP效果
                this.grid_AlarmRecord.DefaultRowHeight = 27;//默认行高
                this.grid_AlarmRecord.EnableTabKey = true;//按Tab键时移动活动单元格
                this.grid_AlarmRecord.EnterKeyMoveTo = FlexCell.MoveToEnum.NextCol;//按回车键时自动跳到下一列
                this.grid_AlarmRecord.SelectionBorderColor = Color.Red;//设置selection边框的颜色
                this.grid_AlarmRecord.ShowResizeTip = true;//返回或设置一个值，该值决定了操作员用鼠标调整FlexCell表格的行高或列宽时，是否显示行高或列宽的提示窗口。
                this.grid_AlarmRecord.AutoRedraw = true;
                this.grid_AlarmRecord.Refresh();

                this.grid_AlarmRecord.Cols = 11;
                this.grid_AlarmRecord.Cell(0, 1).Text = "设备名称";
                this.grid_AlarmRecord.Cell(0, 2).Text = "门名称";
                this.grid_AlarmRecord.Cell(0, 3).Text = "读头";
                this.grid_AlarmRecord.Cell(0, 4).Text = "卡号";
                this.grid_AlarmRecord.Cell(0, 5).Text = "事件时间";
                this.grid_AlarmRecord.Cell(0, 6).Text = "进出状态";
                this.grid_AlarmRecord.Cell(0, 7).Text = "备注";
                this.grid_AlarmRecord.Cell(0, 8).Text = "姓名";
                this.grid_AlarmRecord.Cell(0, 9).Text = "所属部门/单位";
                this.grid_AlarmRecord.Cell(0, 10).Text = "访客类型";


                this.grid_AlarmRecord.Column(1).Width = 120;
                this.grid_AlarmRecord.Column(2).Width = 120;
                this.grid_AlarmRecord.Column(3).Width = 80;
                this.grid_AlarmRecord.Column(4).Width = 100;
                this.grid_AlarmRecord.Column(5).Width = 150;
                this.grid_AlarmRecord.Column(6).Width = 80;
                this.grid_AlarmRecord.Column(7).Width = 150;
                this.grid_AlarmRecord.Column(8).Width = 100;
                this.grid_AlarmRecord.Column(9).Width = 200;
                this.grid_AlarmRecord.Column(10).Width = 80;
                #endregion

                #region//设置grid_NoCardRecord样式
                this.grid_NoCardRecord.AutoRedraw = false;
                this.grid_NoCardRecord.Rows = 1;
                this.grid_NoCardRecord.Cols = 1;
                this.grid_NoCardRecord.DisplayRowArrow = true;
                this.grid_NoCardRecord.DisplayRowNumber = true;
                this.grid_NoCardRecord.MultiSelect = true;
                this.grid_NoCardRecord.StartRowNumber = 1;
                this.grid_NoCardRecord.EnableTabKey = true;
                this.grid_NoCardRecord.EnableVisualStyles = true;
                this.grid_NoCardRecord.SelectionMode = FlexCell.SelectionModeEnum.ByRow;//选中整行
                this.grid_NoCardRecord.DisplayFocusRect = true;//返回或设置控件在当前活动单元格是否显示一个虚框
                this.grid_NoCardRecord.FixedRowColStyle = FlexCell.FixedRowColStyleEnum.Light3D;//返回或设置固定行/列的样式
                this.grid_NoCardRecord.ExtendLastCol = true;//扩展最后一列
                this.grid_NoCardRecord.DefaultFont = new Font("宋体", 9);
                this.grid_NoCardRecord.BorderStyle = FlexCell.BorderStyleEnum.Light3D;
                this.grid_NoCardRecord.BackColorFixed = Color.FromArgb(225, 225, 225);//固定行／列的颜色
                this.grid_NoCardRecord.BackColorFixedSel = Color.FromArgb(225, 225, 225);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_NoCardRecord.BackColorSel = Color.FromArgb(210, 255, 166);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_NoCardRecord.BackColorBkg = Color.FromArgb(255, 255, 255);//返回和设置空白区域的背景色(这里为白色)
                this.grid_NoCardRecord.BackColor1 = Color.FromArgb(231, 235, 247);//返回或设置表格中奇数行的背景色
                this.grid_NoCardRecord.BackColor2 = Color.FromArgb(239, 243, 255);//返回或设置表格中偶数行的背景色
                this.grid_NoCardRecord.CellBorderColorFixed = Color.Red;//返回或设置固定行和固定列上的单元格边框的颜色
                this.grid_NoCardRecord.GridColor = Color.FromArgb(148, 190, 231);//返回或设置网格线的颜色
                this.grid_NoCardRecord.AllowUserReorderColumn = true;//允许操作员拖动列标题来移动整列
                this.grid_NoCardRecord.AllowUserResizing = FlexCell.ResizeEnum.Both;
                this.grid_NoCardRecord.AllowUserSort = true;
                this.grid_NoCardRecord.DrawMode = FlexCell.DrawModeEnum.OwnerDraw;
                this.grid_NoCardRecord.EnableVisualStyles = true;//显示XP效果
                this.grid_NoCardRecord.DefaultRowHeight = 27;//默认行高
                this.grid_NoCardRecord.EnableTabKey = true;//按Tab键时移动活动单元格
                this.grid_NoCardRecord.EnterKeyMoveTo = FlexCell.MoveToEnum.NextCol;//按回车键时自动跳到下一列
                this.grid_NoCardRecord.SelectionBorderColor = Color.Red;//设置selection边框的颜色
                this.grid_NoCardRecord.ShowResizeTip = true;//返回或设置一个值，该值决定了操作员用鼠标调整FlexCell表格的行高或列宽时，是否显示行高或列宽的提示窗口。
                this.grid_NoCardRecord.AutoRedraw = true;
                this.grid_NoCardRecord.Refresh();

                this.grid_NoCardRecord.Cols = 11;
                this.grid_NoCardRecord.Cell(0, 1).Text = "设备名称";
                this.grid_NoCardRecord.Cell(0, 2).Text = "门名称";
                this.grid_NoCardRecord.Cell(0, 3).Text = "读头";
                this.grid_NoCardRecord.Cell(0, 4).Text = "卡号";
                this.grid_NoCardRecord.Cell(0, 5).Text = "事件时间";
                this.grid_NoCardRecord.Cell(0, 6).Text = "进出状态";
                this.grid_NoCardRecord.Cell(0, 7).Text = "备注";
                this.grid_NoCardRecord.Cell(0, 8).Text = "姓名";
                this.grid_NoCardRecord.Cell(0, 9).Text = "所属部门/单位";
                this.grid_NoCardRecord.Cell(0, 10).Text = "访客类型";


                this.grid_NoCardRecord.Column(1).Width = 120;
                this.grid_NoCardRecord.Column(2).Width = 120;
                this.grid_NoCardRecord.Column(3).Width = 80;
                this.grid_NoCardRecord.Column(4).Width = 100;
                this.grid_NoCardRecord.Column(5).Width = 150;
                this.grid_NoCardRecord.Column(6).Width = 80;
                this.grid_NoCardRecord.Column(7).Width = 150;
                this.grid_NoCardRecord.Column(8).Width = 100;
                this.grid_NoCardRecord.Column(9).Width = 200;
                this.grid_NoCardRecord.Column(10).Width = 80;
                #endregion

                #region//设置grid_SpecialRecord样式
                this.grid_SpecialRecord.AutoRedraw = false;
                this.grid_SpecialRecord.Rows = 1;
                this.grid_SpecialRecord.Cols = 1;
                this.grid_SpecialRecord.DisplayRowArrow = true;
                this.grid_SpecialRecord.DisplayRowNumber = true;
                this.grid_SpecialRecord.MultiSelect = true;
                this.grid_SpecialRecord.StartRowNumber = 1;
                this.grid_SpecialRecord.EnableTabKey = true;
                this.grid_SpecialRecord.EnableVisualStyles = true;
                this.grid_SpecialRecord.SelectionMode = FlexCell.SelectionModeEnum.ByRow;//选中整行
                this.grid_SpecialRecord.DisplayFocusRect = true;//返回或设置控件在当前活动单元格是否显示一个虚框
                this.grid_SpecialRecord.FixedRowColStyle = FlexCell.FixedRowColStyleEnum.Light3D;//返回或设置固定行/列的样式
                this.grid_SpecialRecord.ExtendLastCol = true;//扩展最后一列
                this.grid_SpecialRecord.DefaultFont = new Font("宋体", 9);
                this.grid_SpecialRecord.BorderStyle = FlexCell.BorderStyleEnum.Light3D;
                this.grid_SpecialRecord.BackColorFixed = Color.FromArgb(225, 225, 225);//固定行／列的颜色
                this.grid_SpecialRecord.BackColorFixedSel = Color.FromArgb(225, 225, 225);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_SpecialRecord.BackColorSel = Color.FromArgb(210, 255, 166);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
                this.grid_SpecialRecord.BackColorBkg = Color.FromArgb(255, 255, 255);//返回和设置空白区域的背景色(这里为白色)
                this.grid_SpecialRecord.BackColor1 = Color.FromArgb(231, 235, 247);//返回或设置表格中奇数行的背景色
                this.grid_SpecialRecord.BackColor2 = Color.FromArgb(239, 243, 255);//返回或设置表格中偶数行的背景色
                this.grid_SpecialRecord.CellBorderColorFixed = Color.Red;//返回或设置固定行和固定列上的单元格边框的颜色
                this.grid_SpecialRecord.GridColor = Color.FromArgb(148, 190, 231);//返回或设置网格线的颜色
                this.grid_SpecialRecord.AllowUserReorderColumn = true;//允许操作员拖动列标题来移动整列
                this.grid_SpecialRecord.AllowUserResizing = FlexCell.ResizeEnum.Both;
                this.grid_SpecialRecord.AllowUserSort = true;
                this.grid_SpecialRecord.DrawMode = FlexCell.DrawModeEnum.OwnerDraw;
                this.grid_SpecialRecord.EnableVisualStyles = true;//显示XP效果
                this.grid_SpecialRecord.DefaultRowHeight = 27;//默认行高
                this.grid_SpecialRecord.EnableTabKey = true;//按Tab键时移动活动单元格
                this.grid_SpecialRecord.EnterKeyMoveTo = FlexCell.MoveToEnum.NextCol;//按回车键时自动跳到下一列
                this.grid_SpecialRecord.SelectionBorderColor = Color.Red;//设置selection边框的颜色
                this.grid_SpecialRecord.ShowResizeTip = true;//返回或设置一个值，该值决定了操作员用鼠标调整FlexCell表格的行高或列宽时，是否显示行高或列宽的提示窗口。
                this.grid_SpecialRecord.AutoRedraw = true;
                this.grid_SpecialRecord.Refresh();

                this.grid_SpecialRecord.Cols = 11;
                this.grid_SpecialRecord.Cell(0, 1).Text = "设备名称";
                this.grid_SpecialRecord.Cell(0, 2).Text = "门名称";
                this.grid_SpecialRecord.Cell(0, 3).Text = "读头";
                this.grid_SpecialRecord.Cell(0, 4).Text = "卡号";
                this.grid_SpecialRecord.Cell(0, 5).Text = "事件时间";
                this.grid_SpecialRecord.Cell(0, 6).Text = "进出状态";
                this.grid_SpecialRecord.Cell(0, 7).Text = "备注";
                this.grid_SpecialRecord.Cell(0, 8).Text = "姓名";
                this.grid_SpecialRecord.Cell(0, 9).Text = "所属部门/单位";
                this.grid_SpecialRecord.Cell(0, 10).Text = "访客类型";



                this.grid_SpecialRecord.Column(1).Width = 120;
                this.grid_SpecialRecord.Column(2).Width = 120;
                this.grid_SpecialRecord.Column(3).Width = 80;
                this.grid_SpecialRecord.Column(4).Width = 100;
                this.grid_SpecialRecord.Column(5).Width = 150;
                this.grid_SpecialRecord.Column(6).Width = 80;
                this.grid_SpecialRecord.Column(7).Width = 150;
                this.grid_SpecialRecord.Column(8).Width = 100;
                this.grid_SpecialRecord.Column(9).Width = 200;
                this.grid_SpecialRecord.Column(10).Width = 80;
                #endregion

                if (XXCLOUDDLL.A_AreaAndMachineFrm_LoadAreaInf(TV_Area, "1") == "0")
                {
                    MessageBox.Show("区域类别结构图加载失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                LoadDoorInf();//根据区域，加载门信息

                eventHandler = new XXY_VisitorMJAsst.AcsTcpClass.TOnEventHandler(EventHandler);
                accessMan = new AccessMan(int.Parse(this.TV_Area.SelectedNode.Name), EventHandler, 8000);



            }
            catch (Exception exp)
            {
                ExecuteSql();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.TopMost = false;

            timerRunByMinute.Start();
            timer_FromMobileLeave.Start();
            timer_DelMachinePowerByMinute.Start();
            //每次打开先执行一遍


            if (LoginFrm.iFlag_LockEncryption == 1 && LoginFrm.iFlag_OpenMesCommunication == 1)
            {
                try
                {
                    //实际使用时要将Dns.GetHostName()改为服务器名
                    client = new TcpClient(LoginFrm.strSqlServer, 51888);
                    //获取网络流
                    NetworkStream netStream = client.GetStream();
                    sr = new StreamReader(netStream, System.Text.Encoding.UTF8);
                    sw = new StreamWriter(netStream, System.Text.Encoding.UTF8);
                    Client = new Client(listBox1, sw);
                    //登录服务器，获取服务器各桌信息
                    //格式:Login,昵称
                    Client.SendToServer("Login," + Dns.GetHostName());

                    //Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
                    //threadReceive.Start();
                    //blKZZX = true;
                    LoginFrm.iFlag_OpenMesCommunication = 1;
                }
                catch
                {
                    ExecuteSql();
                    //MessageBox.Show(exp.ToString());
                    LoginFrm.iFlag_OpenMesCommunication = 0;
                    MessageBox.Show("连接网络通信助手失败，系统将启用网络版定时刷新模式，请确认服务器上已开启网络通信助手！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            //if (LoginFrm.strAllowedOpenMSCat == "1")
            //{
            //    XXCLOUDDLL.ClosedMSCat();//关闭短信猫
            //    if (XXCLOUDDLL.blOpendMSCat(LoginFrm.iFlag_MesCatPort) == true)
            //    {


            //    }
            //    else
            //    {
            //        MessageBox.Show("短信猫加载失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            timer_auto_connect.Start();//监听门禁链接状态
            timer_RunningTime.Start();
            timer_SendMS.Start();//开启短信检测



        }

        private void TSB_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TV_Area_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                LoadDoorInf();
                //if (this.TSB_RealTimeMonitoring.Visible == true)
                //{
                //    if (this.TSB_RealTimeMonitoring.Text == "实时监控")
                //    {
                //    }
                //    else
                //    {
                //        this.TSB_RealTimeMonitoring_Click(sender, e);//停止监控
                //    }
                //}
                //if (this.TSB_RealTimeCollection.Visible == true)
                //{
                //    if (this.TSB_RealTimeCollection.Text == "实时采集")
                //    {
                //    }
                //    else
                //    {
                //        this.TSB_RealTimeCollection_Click(sender, e);//停止采集
                //    }
                //}
            }
            catch (Exception exp)
            {
                ExecuteSql();
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void grid_GORecord_Click(object Sender, EventArgs e)
        {
            grid_GORecord_Click();
        }

        private void D_RemoterControlFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.timer_SendMS.Stop();
            OperatorLog("退出云访客门禁网络通信助手,结束运行时间：" + System.DateTime.Now.ToString() + ",已运行时长：" + strRunningTime, "1");
            try
            {
                Application.ExitThread();
            }
            catch
            {

            }
            Boolean re = false;

            try
            {
                accessMan.closeAccess();
            }
            catch  
            {

            }
            try
            {
                XXCLOUDDLL.ClosedMSCat();//关闭短信猫
            }
            catch
            {

            }
            btn_Stop_Click(sender, e);
            ExecuteSql();



        }

        private string strSQL_OpenDoor = "";
        private DataTable dtOpenDoor = new DataTable();
        private string strFKId = "0";//把T_LongTemCardInf的Id存入T_MJGORecordInf的FKId字段
        private bool blValidCardOpenDoor = false;//当合法卡刷卡时，由软件发起开门命令。

        private bool blRemoteOpenDoor = true;//true:远程开门 false:临时开门
        private string strNoCardMName = "";//无卡事件下的控制器名称
        private string strNoCardDName = "";//无卡事件下的门名称
        private string strNoCardMSNo = "";//无卡事件下的控制器序列号
        private string strNoCardDoorId = "";//无卡事件下的门编号
        private void TSB_OpenDoor_Click(object sender, EventArgs e)
        {
            try
            {
                if (iRow_Sum <= 1)
                {
                    MessageBox.Show("没有门信息，无法远程开门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (this.grid_Door.Cell(this.grid_Door.Selection.FirstRow, this.grid_Door.Selection.FirstCol).Tag.Contains("CzJDooR") == false)
                {
                    MessageBox.Show("请先选择门图标，再进行远程开门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string strLinkTextTemp = this.grid_Door.Cell(this.grid_Door.Selection.FirstRow, this.grid_Door.Selection.FirstCol - 1).Tag;
                int iCount = 0;
                string strSQL_Temp = "";
                string[] ASetings_Temp = new string[10];
                for (int i = 0; i < ASetings_Temp.Length; i++)
                {
                    ASetings_Temp[i] = "";
                }
                for (int n = 0; n < strLinkTextTemp.Length; n++)
                {
                    if (strLinkTextTemp.Substring(n, 1).ToString() != ",")
                    {
                        strSQL_Temp += strLinkTextTemp.Substring(n, 1).ToString().Trim();
                    }
                    else
                    {
                        ASetings_Temp[iCount++] = strSQL_Temp;
                        strSQL_Temp = "";
                    }
                }

                strNoCardMName = ASetings_Temp[3].ToString();
                strNoCardDName = ASetings_Temp[4].ToString();
                strNoCardMSNo = ASetings_Temp[5].ToString();
                strSQL_Temp = this.grid_Door.Cell(this.grid_Door.Selection.FirstRow, this.grid_Door.Selection.FirstCol).Tag.Replace("CzJDooR", "");
                //D_RemoterControlFrm_AwhileOpenDoor newFrm = new D_RemoterControlFrm_AwhileOpenDoor();
                //newFrm.ShowDialog();
                //if (D_RemoterControlFrm_AwhileOpenDoor.strZT == "OK")
                //{


                if (!accessMan.OpenDoor(strNoCardMSNo, byte.Parse(strSQL_Temp)))
                {
                    MessageBox.Show("设备连接异常，无法远程开门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                this.grid_Door.Cell(this.grid_Door.Selection.FirstRow, this.grid_Door.Selection.FirstCol).SetImage(strImageName_OpendLinked);

                //插入记录到无卡记录表中
                SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_NoCardRecord);
                setUIControlsValue.Invoke(strMName, "", System.DateTime.Now.ToString(), "远程开门", "OpenDoor");
                //}
                //else
                //{
                //    //不开门
                //}



            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TSB_CloseDoor_Click(object sender, EventArgs e)
        {
            try
            {
                if (iRow_Sum <= 1)
                {
                    MessageBox.Show("没有门信息，无法远程关门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (this.grid_Door.Cell(this.grid_Door.Selection.FirstRow, this.grid_Door.Selection.FirstCol).Tag.Contains("CzJDooR") == false)
                {
                    MessageBox.Show("请先选择门图标，再进行远程关门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                string strLinkTextTemp = this.grid_Door.Cell(this.grid_Door.Selection.FirstRow, this.grid_Door.Selection.FirstCol - 1).Tag;
                int iCount = 0;
                string strSQL_Temp = "";
                string[] ASetings_Temp = new string[10];
                for (int i = 0; i < ASetings_Temp.Length; i++)
                {
                    ASetings_Temp[i] = "";
                }
                for (int n = 0; n < strLinkTextTemp.Length; n++)
                {
                    if (strLinkTextTemp.Substring(n, 1).ToString() != ",")
                    {
                        strSQL_Temp += strLinkTextTemp.Substring(n, 1).ToString().Trim();
                    }
                    else
                    {
                        ASetings_Temp[iCount++] = strSQL_Temp;
                        strSQL_Temp = "";
                    }
                }

                strNoCardMName = ASetings_Temp[3].ToString();
                strNoCardDName = ASetings_Temp[4].ToString();
                strNoCardMSNo = ASetings_Temp[5].ToString();
                strSQL_Temp = this.grid_Door.Cell(this.grid_Door.Selection.FirstRow, this.grid_Door.Selection.FirstCol).Tag.Replace("CzJDooR", "");
                strNoCardDoorId = strSQL_Temp;


                if (!accessMan.CloseDoor(strNoCardMSNo, byte.Parse(strNoCardDoorId)))
                {
                    MessageBox.Show("设备连接异常，无法远程关门!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                this.grid_Door.Cell(this.grid_Door.Selection.FirstRow, this.grid_Door.Selection.FirstCol).SetImage(strImageName_SucLinked);
                //插入记录到无卡记录表中
                SetUIControlsValue setUIControlsValue = new SetUIControlsValue(ALLOCationSetUIControlsValue_NoCardRecord);
                setUIControlsValue.Invoke(strMName, "", System.DateTime.Now.ToString(), "远程关门", "CloseDoor");



            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TSB_AllSelected_Click(object sender, EventArgs e)
        {
            if (iRow_Sum <= 1)
            {
                MessageBox.Show("没有门信息，无法全部选中!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            for (int i = 1; i < iRow_Sum; i = i + 2)
            {
                for (int j = 1; j < iCol_Sum; j = j + 2)
                {
                    if (this.grid_Door.Cell(i, j).Tag != "")
                    {
                        this.grid_Door.Cell(i, j).Text = "1";
                    }
                }
            }
        }

        private void TSB_NoneSelected_Click(object sender, EventArgs e)
        {
            if (iRow_Sum <= 1)
            {
                MessageBox.Show("没有门信息，无法全部取消选中!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            for (int i = 1; i < iRow_Sum; i = i + 2)
            {
                for (int j = 1; j < iCol_Sum; j = j + 2)
                {
                    if (this.grid_Door.Cell(i, j).Tag != "")
                    {
                        this.grid_Door.Cell(i, j).Text = "0";
                    }
                }
            }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            try
            {
                this.listBox1.Items.Clear();
                myListener = new TcpListener(localAddress, port);
                myListener.Start();
                service.SetListBox(string.Format("开始在{0}:{1}监听客户连接", localAddress, port));
                //创建一个线程监听客户端连接请求
                ThreadStart ts = new ThreadStart(ListenClientConnect);
                Thread myThread = new Thread(ts);
                myThread.Start();
                btn_Start.Enabled = false;
                btn_Stop.Enabled = true;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                service.SetListBox(string.Format("目前连接用户数：{0}", userList.Count.ToString()));
                service.SetListBox("开始停止通信，并依次使用户退出!");
                for (int i = 0; i < userList.Count; i++)
                {
                    //关闭后，客户端接收字符串为null
                    //使接收该客户的线程ReceiveData接收的字符串也为null
                    //从而达到结束线程的目的
                    userList[i].client.Close();
                }
                //通过停止监听让myListener.AcceptTcpClient()产生异常退出监听线程
                myListener.Stop();
                btn_Start.Enabled = true;
                btn_Stop.Enabled = false;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void D_RemoterControlFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (blExit == false)
            {
                if (MessageBox.Show("若本机为数据库服务器,请不要关闭此助手! 您确定关闭此助手吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //e.Cancel = false ;
                    btn_Stop_Click(sender, e);
                    //try
                    //{
                    //    System.Environment.Exit(System.Environment.ExitCode);
                    //}
                    //catch
                    //{

                    //}
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //显示主窗体
            this.Visible = true;
            this.notifyIcon1.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }

        private void TSB_ToSystemSpool_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.notifyIcon1.Visible = true;
        }

        int iCurHour = 0, iCurMinute = 0;

        private int iRestartCollectByFiveMinute = 0;//每五分钟重新进行开户实时采集一次
        private void timerRunByMinute_Tick(object sender, EventArgs e)
        {

            iCurHour = System.DateTime.Now.Hour;
            iCurMinute = System.DateTime.Now.Minute;

            #region//1.每天自动定时签离全部未签离来访记录并记为正常签离
            if (LoginFrm.strAllowedAllLeaveNormal == "1")
            {
                if (iCurHour == Convert.ToInt32(LoginFrm.strAllowedAllLeaveNormal_Hour) && iCurMinute == Convert.ToInt32(LoginFrm.strAllowedAllLeaveNormal_Minute))
                {
                    AllowedAllLeaveNormal();
                    #region//4.每天自动定时根据预约有效期限把过期的会议预约删除掉
                    OrderEnabledByConferenceInf();
                    #endregion
                    #region//5.每天自动定时签离全部紧急会议记录并记为正常签离(不涉及到卡片注销)
                    AllowedAllEmergencyConference();
                    #endregion
                }
            }
            #endregion

            #region//2.每天自动定时从门禁控制器上注销所有未注销的访客证权限并记为正常注销,时间为
            if (LoginFrm.strAllowedLogoutRedCardNormal == "1")
            {
                if (iCurHour == Convert.ToInt32(LoginFrm.strAllowedLogoutRedCardNormal_Hour) && iCurMinute == Convert.ToInt32(LoginFrm.strAllowedLogoutRedCardNormal_Minute))
                {
                    AllowedLogoutRedCardNormal();

                }
            }
            #endregion

            #region//3.每天自动定时根据预约有效期限把过期预约设为过期预约
            if (LoginFrm.strAllowedOverdueOrder == "1")
            {
                if (iCurHour == Convert.ToInt32(LoginFrm.strAllowedOverdueOrder_Hour) && iCurMinute == Convert.ToInt32(LoginFrm.strAllowedOverdueOrder_Minute))
                {
                    OrderEnabledByPastDue();
                }
            }
            #endregion

            #region//6.更改预约表里的工作证号，去掉No.或No。这几个字符
            ModifyIdCardNoFromReservationRecord();
            #endregion

            //if (System.DateTime.Now.Hour == 0 && System.DateTime.Now.Minute == 0 && System.DateTime.Now.Second==1)
            //{
            // new Thread(BlueCardLogout).Start();
            //    new Thread(OrderEnabledByPastDue).Start();
            //}

        }

        private void timer_FromMobileLeave_Tick(object sender, EventArgs e)
        {
            //每隔五分钟执行一次
            new Thread(TemCardLogoutFromMobile).Start();
        }

        private void timer_StartService_Tick(object sender, EventArgs e)
        {
            timer_StartService.Stop();
            if (iRow_Sum > 1)
            {
                TSB_ToSystemSpool_Click(sender, e);
            }
        }

        private void TSB_SysSetting_Click(object sender, EventArgs e)
        {
            D_RemoterControlFrm_SysSetting newFrm = new D_RemoterControlFrm_SysSetting();
            newFrm.ShowDialog();
        }

        private void 清空当前列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                this.grid_GORecord.AutoRedraw = false;
                this.grid_GORecord.Rows = 1;
                this.grid_GORecord.AutoRedraw = true;
                this.grid_GORecord.Refresh();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                this.grid_AlarmRecord.AutoRedraw = false;
                this.grid_AlarmRecord.Rows = 1;
                this.grid_AlarmRecord.AutoRedraw = true;
                this.grid_AlarmRecord.Refresh();
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                this.grid_NoCardRecord.AutoRedraw = false;
                this.grid_NoCardRecord.Rows = 1;
                this.grid_NoCardRecord.AutoRedraw = true;
                this.grid_NoCardRecord.Refresh();
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                this.grid_SpecialRecord.AutoRedraw = false;
                this.grid_SpecialRecord.Rows = 1;
                this.grid_SpecialRecord.AutoRedraw = true;
                this.grid_SpecialRecord.Refresh();
            }
        }


        private const string CONNECTION_STATE_OUT = "终端链接";


        private bool hasOutAccess = false;//存在断开在设备
        List<AccessModel> outAccess;

        private void timer_auto_connect_Tick(object sender, EventArgs e)
        {

            try
            {
                accessMan.RefreshState(out outAccess);
                string strMSNo = string.Empty;
                foreach (AccessModel model in outAccess)
                {
                    strMSNo += model.MSNo + ";";
                }
                if (strMSNo != string.Empty)
                {
                    strMSNo = strMSNo.Substring(0, strMSNo.Length - 1);
                }
                refreshAccessIcon(outAccess);
                if (hasOutAccess && outAccess.Count == 0)
                {
                    //重连事件
                    // Console.WriteLine("设备重连============================================");
                    OperatorLog("云访客门禁网络通信助手与所有门禁设备重新连接成功", "1");
                }
                if (!hasOutAccess && outAccess.Count > 0)
                {
                    // Console.WriteLine("设备断开============================================");
                    Client.SendToServer("InternetOff," + strMSNo);//发送消息通知前台访客软件提醒登记人员有设备与通信助手失联。
                    OperatorLog("云访客门禁网络通信助手与门禁设备失去连接，设备序列号为:" + strMSNo, "0");
                }
                hasOutAccess = !(outAccess.Count == 0);
            }
            catch
            {
                OperatorLog("执行云访客门禁网络通信助手与门禁设备重新连接出现错误", "0");
                //MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="outList">已断开在设备</param>
        private void refreshAccessIcon(List<AccessModel> outList)
        {
            List<AccessModel> allList = accessMan.AccessList;//所有在设备

            foreach (AccessModel model in allList)
            {
                if (outList.Exists(m => m.MSNo == model.MSNo))
                {
                    // MSNO 设备断开处理图标断开

                    // Console.WriteLine(model.MSNo + ":断开" );
                    refreshAccessIconTemp(model.MSNo, false);
                }
                else
                {
                    // MSNO 处理设备图标链接
                    // Console.WriteLine(model.MSNo + ":链接");
                    refreshAccessIconTemp(model.MSNo, true);
                }
            }
        }

        private void refreshAccessIconTemp(string para_strMSNo, bool para_bl)
        {
            if (iRow_Sum > 0 || iCol_Sum > 0)
            {
                for (int i = 1; i < iRow_Sum; i = i + 2)
                {
                    for (int j = 1; j < iCol_Sum; j = j + 2)
                    {
                        if (para_bl == false)//断开
                        {
                            if (this.grid_Door.Cell(i, j).Tag.Contains(para_strMSNo) == true)
                            {
                                this.grid_Door.AutoRedraw = false;
                                this.grid_Door.Cell(i, j + 1).SetImage(strImageName_ErrorLinked);
                                this.grid_Door.AutoRedraw = true;
                                this.grid_Door.Refresh();
                            }
                        }
                        else//已重连
                        {
                            if (this.grid_Door.Cell(i, j).Tag.Contains(para_strMSNo) == true)
                            {
                                this.grid_Door.AutoRedraw = false;
                                this.grid_Door.Cell(i, j + 1).SetImage(strImageName_SucLinked);
                                this.grid_Door.AutoRedraw = true;
                                this.grid_Door.Refresh();
                            }
                        }
                    }
                }
            }
        }
        private void grid_Door_Click(object Sender, EventArgs e)
        {
            //MessageBox.Show(this.grid_Door.Cell(this.grid_Door.Selection.FirstRow, this.grid_Door.Selection.FirstCol).Tag);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            strRunningTime = DateDiff(System.DateTime.Now, dtStart);
            this.Text = "云访客门禁网络通信助手" + "      已运行时长：" + strRunningTime;
        }

        private void timer_DelMachinePowerByMinute_Tick(object sender, EventArgs e)
        {
            DelCardPowerFromMachine();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                this.grid_GORecord.AutoRedraw = false;

                this.grid_GORecord.AutoRedraw = true;
                this.grid_GORecord.Refresh();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                this.grid_AlarmRecord.AutoRedraw = false;

                this.grid_AlarmRecord.AutoRedraw = true;
                this.grid_AlarmRecord.Refresh();
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                this.grid_NoCardRecord.AutoRedraw = false;

                this.grid_NoCardRecord.AutoRedraw = true;
                this.grid_NoCardRecord.Refresh();
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                this.grid_SpecialRecord.AutoRedraw = false;

                this.grid_SpecialRecord.AutoRedraw = true;
                this.grid_SpecialRecord.Refresh();
            }
        }

        private void TSB_SystemLog_Click(object sender, EventArgs e)
        {
            XXY_VisitorMJAsst._3SystemMaintenance.D_OperatorLogFrm newFrm = new _3SystemMaintenance.D_OperatorLogFrm();
            newFrm.ShowDialog();
        }

        private bool blFlag = false;
        private void timer_SendMS_Tick(object sender, EventArgs e)
        {
            //if (blFlag == false)
            //{
            //    blFlag = true;
            //    try
            //    {
            //        dtMSSend.Rows.Clear();
            //        strSQLMSSend = "select top 1 * from XXCLOUD.dbo.T_MessageSendInf  where (Flag ='" + "0" + "' or  Flag is null ) order by Id desc ";
            //        dtMSSend = SQLHelper.DTQuery(strSQLMSSend);
            //        if (dtMSSend.Rows.Count > 0)
            //        {
            //            for (int i = 0; i < dtMSSend.Rows.Count; i++)
            //            {
            //                string strMes = "[" + LoginFrm.strEnduserName + "]验证码:" + dtMSSend.Rows[i]["OrderCode"].ToString().Trim() + "(泄露有风险)，您正在进行访客自助登记，请于2分钟内填写，如非本人操作，请忽略本短信。";
            //                if (XXCLOUDDLL.SendMesByPhoneAndContext(dtMSSend.Rows[i]["VPhone"].ToString().Trim(), strMes) == true)
            //                {
            //                    strSQLMSSend = "update  XXCLOUD.dbo.T_MessageSendInf set SendDT ='" + System.DateTime.Now.ToString() + "',Flag='" + "1" + "' where Id='" + dtMSSend.Rows[i]["Id"].ToString().Trim() + "'";
            //                    if (SQLHelper.ExecuteSql(strSQLMSSend) != 0)
            //                    {
            //                        // OperatorLog("服务器向访客" + dtMSSend.Rows[i]["VName"].ToString().Trim() + "[" + dtMSSend.Rows[i]["VPhone"].ToString().Trim() + "]发送手机验证短信成功", "1");
            //                    }
            //                }
            //                else
            //                {
            //                    // MessageBox.Show("短信发送失败，无法继续登记，请联系设备管理员！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //                    // OperatorLog("服务器向访客" + dtMSSend.Rows[i]["VName"].ToString().Trim() + "[" + dtMSSend.Rows[i]["VPhone"].ToString().Trim() + "]发送手机验证短信失败", "0");
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception exp)
            //    {
            //        OperatorLog("服务器向访客发送手机验证短信失败--" + exp.Message.ToString(), "0");
            //    }
            //}
            //else
            //{
            //    blFlag = false;
                
            
            //try
            //    {
            //        dtMSSend_OrderCode.Rows.Clear();
            //        strSQLMSSend_OrderCode = "select top 1 * from LogisticsDB.dbo.ReservationRecord  where (MSSend ='" + "0" + "' or  MSSend is null ) ";
            //        strSQLMSSend_OrderCode += " and (ApiType='" + "1" + "' or ApiType='" + "2" + "' or ApiType='" + "3" + "' )";
            //        dtMSSend_OrderCode = SQLHelper_Remote.DTQuery(strSQLMSSend_OrderCode);
            //        if (dtMSSend_OrderCode.Rows.Count > 0)
            //        {
            //            for (int i = 0; i < dtMSSend_OrderCode.Rows.Count; i++)
            //            {
            //                string strMes = "";// 
            //                if (dtMSSend_OrderCode.Rows[0]["VisitDate"].ToString().Trim() != "" && dtMSSend_OrderCode.Rows[0]["VisitDate"].ToString().Trim() != null)
            //                {
            //                    try
            //                    {
            //                        string strDT = Convert.ToDateTime(dtMSSend_OrderCode.Rows[0]["VisitDate"].ToString()).ToShortDateString();
            //                        strMes = "您好，欢迎您访问" + LoginFrm.strEnduserName + "，预约码为" + dtMSSend_OrderCode.Rows[i]["InvitationNo"].ToString().Trim() + "(泄露有风险)，请在" + strDT + "带好身份证或办理的工作证(月卡)到自助机上办理登记手续。[" + LoginFrm.strEnduserName + "]";
            //                    }
            //                    catch
            //                    {
            //                        strMes = strMes = "您好，欢迎您访问" + LoginFrm.strEnduserName + "，预约码为" + dtMSSend_OrderCode.Rows[i]["InvitationNo"].ToString().Trim() + "(泄露有风险)，来访时请带好身份证或办理的工作证(月卡)到自助机上办理登记手续。[" + LoginFrm.strEnduserName + "]";
            //                    }
            //                }
            //                else if (dtMSSend_OrderCode.Rows[0]["StartDate"].ToString().Trim() != "" && dtMSSend_OrderCode.Rows[0]["StartDate"].ToString().Trim() != null)
            //                {
            //                    try
            //                    {
            //                        string strDT = Convert.ToDateTime(dtMSSend_OrderCode.Rows[0]["StartDate"].ToString()).ToShortDateString();
            //                        strMes = "您好，欢迎您访问" + LoginFrm.strEnduserName + "，预约码为" + dtMSSend_OrderCode.Rows[i]["InvitationNo"].ToString().Trim() + "(泄露有风险)，请在" + strDT + "带好身份证或办理的工作证(月卡)到自助机上办理登记手续。[" + LoginFrm.strEnduserName + "]";
            //                    }
            //                    catch
            //                    {
            //                        strMes = strMes = "您好，欢迎您访问" + LoginFrm.strEnduserName + "，预约码为" + dtMSSend_OrderCode.Rows[i]["InvitationNo"].ToString().Trim() + "(泄露有风险)，来访时请带好身份证或办理的工作证(月卡)到自助机上办理登记手续。[" + LoginFrm.strEnduserName + "]";
            //                    }
            //                }
            //                {

            //                }

                            
            //                if (XXCLOUDDLL.SendMesByPhoneAndContext(dtMSSend_OrderCode.Rows[i]["VisitorPhone"].ToString().Trim(), strMes) == true)
            //                {
            //                    strSQLMSSend_OrderCode = "update  LogisticsDB.dbo.ReservationRecord   set MSSendDT ='" + System.DateTime.Now.ToString() + "',MSSend='" + "1" + "' where Id='" + dtMSSend_OrderCode.Rows[i]["Id"].ToString().Trim() + "'";
            //                    if (SQLHelper_Remote.ExecuteSql(strSQLMSSend_OrderCode) != 0)
            //                    {
            //                        //OperatorLog("服务器向访客" + dtMSSend_OrderCode.Rows[i]["VisitorName"].ToString().Trim() + "[" + dtMSSend_OrderCode.Rows[i]["VisitorPhone"].ToString().Trim() + "]发送预约码短信成功", "1");
            //                    }
            //                }
            //                else
            //                {
            //                    // MessageBox.Show("短信发送失败，无法继续登记，请联系设备管理员！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //                    // OperatorLog("服务器向访客" + dtMSSend_OrderCode.Rows[i]["VisitorName"].ToString().Trim() + "[" + dtMSSend_OrderCode.Rows[i]["VisitorPhone"].ToString().Trim() + "]发送预约码短信失败", "0");
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception exp)
            //    {
            //        OperatorLog("服务器向访客发送预约码短信失败--" + exp.Message.ToString(), "0");
            //    }
            ////}
        }

        private void button15_Click(object sender, EventArgs e)
        {
            ////LCD显示
            //string str;
            //Boolean re = false;
            //byte page = Convert.ToByte(textBoxLCDpage.Text);
            //byte line = Convert.ToByte(textBoxLCDLine.Text);
            //str = textBoxLCD.Text;

            //// setmsg();
            //re = TcpipObj.ShowToLCDPage(page, line, 5, str);
            //showResult(re, TcpipObj.TCPLastError);
        }

        int iCloseDoor = 0;
      
        private void timer_CloseDoor_Tick(object sender, EventArgs e)
        {
            iCloseDoor++;
            if (iCloseDoor >= Convert.ToUInt32(D_RemoterControlFrm_AwhileOpenDoor.strSecond))
            {
                Boolean re = false;
      
                accessMan.CloseDoor(strNoCardMSNo,byte.Parse(strNoCardDoorId));
                    
                iCloseDoor = 0;
                timer_CloseDoor.Stop();
            }
        }

        private void grid1_Load(object sender, EventArgs e)
        {

        }

        private void timer_SendOrderCode_Tick(object sender, EventArgs e)
        {
            //try
            //{
            //    dtMSSend_OrderCode.Rows.Clear();
            //    strSQLMSSend_OrderCode = "select * from LogisticsDB.dbo.ReservationRecord  where (MSSend ='" + "0" + "' or  MSSend is null ) ";
            //    strSQLMSSend_OrderCode += " and (ApiType='" + "1" + "' or ApiType='" + "2" + "' or ApiType='" + "3" + "' )";
            //    dtMSSend_OrderCode = SQLHelper_Remote.DTQuery(strSQLMSSend_OrderCode);
            //    if (dtMSSend_OrderCode.Rows.Count > 0)
            //    {
            //        for (int i = 0; i < dtMSSend_OrderCode.Rows.Count; i++)
            //        {
            //            string strMes = "您好，欢迎您访问" + LoginFrm.strEnduserName + "，预约码为" + dtMSSend_OrderCode.Rows[i]["InvitationNo"].ToString().Trim() + "(泄露有风险)，来访时请出示预约码及身份证件到自助机办理登记手续。[" + LoginFrm.strEnduserName + "]";
            //            if (XXCLOUDDLL.SendMesByPhoneAndContext(dtMSSend_OrderCode.Rows[i]["VisitorPhone"].ToString().Trim(), strMes) == true)
            //            {
            //                Thread.Sleep(500);
            //                strSQLMSSend_OrderCode = "update  LogisticsDB.dbo.ReservationRecord   set MSSendDT ='" + System.DateTime.Now.ToString() + "',MSSend='" + "1" + "' where Id='" + dtMSSend_OrderCode.Rows[i]["Id"].ToString().Trim() + "'";
            //                if (SQLHelper_Remote.ExecuteSql(strSQLMSSend_OrderCode) != 0)
            //                {
            //                    //OperatorLog("服务器向访客" + dtMSSend_OrderCode.Rows[i]["VisitorName"].ToString().Trim() + "[" + dtMSSend_OrderCode.Rows[i]["VisitorPhone"].ToString().Trim() + "]发送预约码短信成功", "1");
            //                }
            //            }
            //            else
            //            {
            //                // MessageBox.Show("短信发送失败，无法继续登记，请联系设备管理员！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //                // OperatorLog("服务器向访客" + dtMSSend_OrderCode.Rows[i]["VisitorName"].ToString().Trim() + "[" + dtMSSend_OrderCode.Rows[i]["VisitorPhone"].ToString().Trim() + "]发送预约码短信失败", "0");
            //            }
            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //    OperatorLog("服务器向访客发送预约码短信失败--" + exp.Message.ToString(), "0");
            //}
        }

    }
}
