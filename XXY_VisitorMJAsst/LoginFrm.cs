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
using Microsoft.Win32;
using System.Net;
using System.Threading;
using System.IO.Ports;
using System.Runtime.InteropServices;  //一定要加入这个引用才能读用IC读卡器
using System.ServiceProcess;
using System.Drawing.Imaging;

namespace XXY_VisitorMJAsst
{
    public partial class LoginFrm : Form
    {

        #region//清除回收站中的文件
        const int SHERB_NOCONFIRMATION = 0x000001;
        const int SHERB_NOPROGRESSUI = 0x000002;
        const int SHERB_NOSOUND = 0x000004;
        [DllImportAttribute("shell32.dll")]//声明API函数
        private static extern int SHEmptyRecycleBin(IntPtr handle, string root, int falgs);
        //handle:父窗口句柄
        //root:将要清空的回收站的地址，如果为NUll值，将清空所有驱动器上的回收站
        //falgs:用于清空回收站的功能参数
        #endregion

        #region//窗体动画效果展示
        [DllImportAttribute("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        public const Int32 AW_HOR_POSITIVE = 0x00000001;
        public const Int32 AW_HOR_NEGATIVE = 0x00000002;
        public const Int32 AW_VER_POSITIVE = 0x00000004;
        public const Int32 AW_VER_NEGATIVE = 0x00000008;
        public const Int32 AW_CENTER = 0x00000010;
        public const Int32 AW_HIDE = 0x00010000;
        public const Int32 AW_ACTIVATE = 0x00020000;
        public const Int32 AW_SLIDE = 0x00040000;
        public const Int32 AW_BLEND = 0x00080000;
        #endregion

        #region//新中新二合一读卡器(IC卡)
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct IDCardData
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public byte[] Name; //姓名   
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] Sex;   //性别
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Nation; //名族
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Born; //出生日期
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 70)]
            public byte[] Address; //住址
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            public byte[] IDCardNo; //证件号码
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public byte[] GrantDept; //发证机关
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] UserLife; // 有效日期
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] PhotoFileName; // 头像路径
        }

        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_Init", CharSet = CharSet.Ansi)]
        public static extern Boolean XZX_Init(int port, long baud);
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_Close", CharSet = CharSet.Ansi)]
        public static extern Boolean XZX_Close(int port);
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_Beep2", CharSet = CharSet.Ansi)]
        public static extern void XZX_Beep2(byte ON_100ms);
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_Led", CharSet = CharSet.Ansi)]
        public static extern void XZX_Led(Boolean bOn);
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_GT_Halt", CharSet = CharSet.Ansi)]
        public static extern Boolean XZX_GT_Halt();
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_GT_Reset", CharSet = CharSet.Ansi)]
        public static extern Boolean XZX_GT_Reset();
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_LookupCard", CharSet = CharSet.Ansi)]
        public static extern Boolean XZX_LookupCard(byte nType, ref byte nSerialNum, ref byte nSerialLen);
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_LoadKey", CharSet = CharSet.Ansi)]
        public static extern Boolean XZX_LoadKey(byte mode, byte sectionNum, ref byte key);
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_Authenticate", CharSet = CharSet.Ansi)]
        public static extern Boolean XZX_Authenticate(byte mode, byte sectionNum);
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_ReadMefire", CharSet = CharSet.Ansi)]
        public static extern int XZX_ReadMefire(byte Block_Adr, ref byte _Data);
        [DllImport("ThirdInOne.dll", EntryPoint = "XZX_WriteMefire", CharSet = CharSet.Ansi)]
        public static extern Boolean XZX_WriteMefire(byte Start_Block, ref byte Data);
        #endregion

        #region//检测是否已经开启大小写
        //[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        //private static extern short GetKeyState(int keyCode);
        //private static int HIWORD(int n)
        //{
        //    return ((n >> 16) & 0xffff/*=~0x0000*/);
        //}
        //private static int LOWORD(int n)
        //{
        //    return (n & 0xffff/*=~0x0000*/);
        //}
        ///// <summary>
        ///// 得到当前是否开启大写(开启为true)
        ///// </summary>
        ///// <returns>开启为true</returns>
        //public static bool GetKeyState()
        //{
        //    short state = GetKeyState(0x14   /*VK_CAPTIAL*/);
        //    if (LOWORD(state).ToString() == "1")
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}
        //if (GetKeyState() == true)
        //{
        //    MessageBox.Show("小写");
        //}
        //else
        //{
        //    MessageBox.Show("大写");
        //}
        #endregion

        #region//隐藏任务栏
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public struct APPBARDATA
        {
            public int cbSize;
            public int hwnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public int lParam;
        }

        public const int ABS_ALWAYSONTOP = 0x002;
        public const int ABS_AUTOHIDE = 0x001;
        public const int ABS_BOTH = 0x003;
        public const int ABM_ACTIVATE = 0x006;
        public const int ABM_GETSTATE = 0x004;
        public const int ABM_GETTASKBARPOS = 0x005;
        public const int ABM_NEW = 0x000;
        public const int ABM_QUERYPOS = 0x002;
        public const int ABM_SETAUTOHIDEBAR = 0x008;
        public const int ABM_SETSTATE = 0x00A;

        /// 
        /// 向系统任务栏发送消息
        /// 
        [DllImport("shell32.dll")]
        public static extern int SHAppBarMessage(int dwmsg, ref APPBARDATA app);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        /// 
        /// 设置系统任务栏是否自动隐藏
        /// 
        /// 


        //True 设置为自动隐藏，False 取消自动隐藏

        public static void SetAppBarAutoDisplay(bool IsAuto)
        {
            APPBARDATA abd = new APPBARDATA();
            abd.hwnd = FindWindow("Shell_TrayWnd", "");
            //abd.lParam = ABS_ALWAYSONTOP Or ABS_AUTOHIDE   '自动隐藏,且位于窗口前
            //abd.lParam = ABS_ALWAYSONTOP                   '不自动隐藏,且位于窗口前
            //abd.lParam = ABS_AUTOHIDE                       '自动隐藏,且不位于窗口前
            if (IsAuto)
            {
                abd.lParam = ABS_AUTOHIDE;
                SHAppBarMessage(ABM_SETSTATE, ref abd);
            }
            else
            {
                abd.lParam = ABS_ALWAYSONTOP;
                SHAppBarMessage(ABM_SETSTATE, ref abd);
            }
        }

        #endregion

        //获取默认打印机
        [DllImport("Winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDefaultPrinter(string printerName);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int pcchBuffer);


        public LoginFrm()
        {
            InitializeComponent();
            //调用ShowWindow和FindWindow函数，实现任务栏的隐藏
           // SetAppBarAutoDisplay(true);
            AnimateWindow(this.Handle, 700, AW_SLIDE + AW_CENTER);
            #region//设置grid_AccoutList样式
            this.grid_AccoutList.AutoRedraw = false;
            this.grid_AccoutList.Rows = 1;
            this.grid_AccoutList.Cols = 1;
            this.grid_AccoutList.DisplayRowArrow = true;
            this.grid_AccoutList.MultiSelect = false;
            this.grid_AccoutList.StartRowNumber = 1;
            this.grid_AccoutList.EnableTabKey = true;
            this.grid_AccoutList.EnableVisualStyles = true;
            //this.grid_AccoutList.SelectionMode = FlexCell.SelectionModeEnum.ByRow;//选中整行
            this.grid_AccoutList.DisplayFocusRect = true;//返回或设置控件在当前活动单元格是否显示一个虚框
            this.grid_AccoutList.ExtendLastCol = true;//扩展最后一列
            this.grid_AccoutList.FixedRowColStyle = FlexCell.FixedRowColStyleEnum.Light3D;//返回或设置固定行/列的样式
            this.grid_AccoutList.DefaultFont = new Font("宋体", 11);
            //this.grid_AccoutList.BorderStyle = FlexCell.BorderStyleEnum.Light3D;
            //this.grid_AccoutList.BackColorFixed = Color.FromArgb(210, 210, 210);//固定行／列的颜色
            //this.grid_AccoutList.BackColorFixedSel = Color.FromArgb(210, 210, 210);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
            //this.grid_AccoutList.BackColorSel = Color.FromArgb(210, 255, 166);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
            this.grid_AccoutList.BackColorBkg = Color.FromArgb(255, 255, 255);//返回和设置空白区域的背景色(这里为白色)
            //this.grid_AccoutList.BackColor1 = Color.FromArgb(231, 235, 247);//返回或设置表格中奇数行的背景色
            //this.grid_AccoutList.BackColor2 = Color.FromArgb(239, 243, 255);//返回或设置表格中偶数行的背景色
            this.grid_AccoutList.CellBorderColorFixed = Color.White;//返回或设置固定行和固定列上的单元格边框的颜色
            this.grid_AccoutList.GridColor = Color.White;//返回或设置网格线的颜色
            this.grid_AccoutList.AllowUserReorderColumn = true;//允许操作员拖动列标题来移动整列
            this.grid_AccoutList.AllowUserResizing = FlexCell.ResizeEnum.Both;
            this.grid_AccoutList.AllowUserSort = true;
            this.grid_AccoutList.EnableVisualStyles = true;//显示XP效果
            this.grid_AccoutList.DefaultRowHeight = 30;//默认行高
            this.grid_AccoutList.EnableTabKey = true;//按Tab键时移动活动单元格
            this.grid_AccoutList.EnterKeyMoveTo = FlexCell.MoveToEnum.NextCol;//按回车键时自动跳到下一列
            this.grid_AccoutList.SelectionBorderColor = Color.Red;//设置selection边框的颜色
            this.grid_AccoutList.ShowResizeTip = true;//返回或设置一个值，该值决定了操作员用鼠标调整FlexCell表格的行高或列宽时，是否显示行高或列宽的提示窗口。

            grid_AccoutList.Cols = 3;
            grid_AccoutList.Column(0).Visible = false;
            grid_AccoutList.Row(0).Visible = false;
            grid_AccoutList.Column(1).Width = 225;
            grid_AccoutList.Column(2).Width = 25;

            this.grid_AccoutList.AutoRedraw = true;
            this.grid_AccoutList.Refresh();
            #endregion

            #region//设置grid_Operator样式
            this.grid_Operator.AutoRedraw = false;
            this.grid_Operator.Rows = 1;
            this.grid_Operator.Cols = 1;
            this.grid_Operator.DisplayRowArrow = true;
            this.grid_Operator.MultiSelect = false;
            this.grid_Operator.StartRowNumber = 1;
            this.grid_Operator.EnableTabKey = true;
            this.grid_Operator.EnableVisualStyles = true;
            //this.grid_Operator.SelectionMode = FlexCell.SelectionModeEnum.ByRow;//选中整行
            this.grid_Operator.DisplayFocusRect = true;//返回或设置控件在当前活动单元格是否显示一个虚框
            this.grid_Operator.ExtendLastCol = true;//扩展最后一列
            this.grid_Operator.FixedRowColStyle = FlexCell.FixedRowColStyleEnum.Light3D;//返回或设置固定行/列的样式
            this.grid_Operator.DefaultFont = new Font("宋体", 11);
            //this.grid_Operator.BorderStyle = FlexCell.BorderStyleEnum.Light3D;
            //this.grid_Operator.BackColorFixed = Color.FromArgb(210, 210, 210);//固定行／列的颜色
            //this.grid_Operator.BackColorFixedSel = Color.FromArgb(210, 210, 210);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
            //this.grid_Operator.BackColorSel = Color.FromArgb(210, 255, 166);//返回或设置表格中选定单元格（Selection）在固定行/列上对应单元格的背景色
            this.grid_Operator.BackColorBkg = Color.FromArgb(255, 255, 255);//返回和设置空白区域的背景色(这里为白色)
            //this.grid_Operator.BackColor1 = Color.FromArgb(231, 235, 247);//返回或设置表格中奇数行的背景色
            //this.grid_Operator.BackColor2 = Color.FromArgb(239, 243, 255);//返回或设置表格中偶数行的背景色
            this.grid_Operator.CellBorderColorFixed = Color.White;//返回或设置固定行和固定列上的单元格边框的颜色
            this.grid_Operator.GridColor = Color.White;//返回或设置网格线的颜色
            this.grid_Operator.AllowUserReorderColumn = true;//允许操作员拖动列标题来移动整列
            this.grid_Operator.AllowUserResizing = FlexCell.ResizeEnum.Both;
            this.grid_Operator.AllowUserSort = true;
            this.grid_Operator.EnableVisualStyles = true;//显示XP效果
            this.grid_Operator.DefaultRowHeight = 30;//默认行高
            this.grid_Operator.EnableTabKey = true;//按Tab键时移动活动单元格
            this.grid_Operator.EnterKeyMoveTo = FlexCell.MoveToEnum.NextCol;//按回车键时自动跳到下一列
            this.grid_Operator.SelectionBorderColor = Color.Red;//设置selection边框的颜色
            this.grid_Operator.ShowResizeTip = true;//返回或设置一个值，该值决定了操作员用鼠标调整FlexCell表格的行高或列宽时，是否显示行高或列宽的提示窗口。

            grid_Operator.Cols = 3;
            grid_Operator.Column(0).Visible = false;
            grid_Operator.Row(0).Visible = false;
            grid_Operator.Column(1).Width = 225;
            grid_Operator.Column(2).Width = 25;

            this.grid_Operator.AutoRedraw = true;
            this.grid_Operator.Refresh();
            #endregion

            this.grid_AccoutList.Visible = false;
            this.grid_Operator.Visible = false;

            this.grid_AccoutList.Images.Add(XXY_VisitorMJAsst.Properties.Resources.exit, @"Sample1");
            this.grid_Operator.Images.Add(XXY_VisitorMJAsst.Properties.Resources.exit, @"Sample1");


        }  
        //操作员自定义全局实例变量
        public static string strSoftwareName = "云访客门禁网络通信助手V2019";
        public static string strSoftwareProvider = " ";
        public static string strSoftwarePhone = " ";
        public static string strSoftwareWebsite = "";// ";

        public static string strWTUserId = "68295668254632015789";//温州捷亚：68295668254632015789
        public static bool blWTSucLoaded = true;//true 文通证件识别设备加载成功
        public static string strWTScannerName = "";//文通证件识别设备的名称

        private SQLHelper SQLHelper = new SQLHelper();
        private SQLHelper_Remote SQLHelper_Remote = new SQLHelper_Remote();
        private XXCLOUDDLL XXCLOUDDLL =new XXCLOUDDLL();
    
       
        private int[] keyHandles = new int[8];
        public static string strFlagDBLink = "YEAR";
        public static string strReportType = "1";//报表类型
        public static string strPrintReport = "1";//登记时,允许打印出入证
        public static string strAutoRecognition = "0";//登记时,允许智能识别卡片
        public static string strLastVisitInf = "0";//登记时,允许自动读取访客最后一次访问信息
        public static string strDisplayQuantity = " All ";//登记时,只显示被访对象列表前N位
        public static string strDisplayQuantityValue = "0";//位数
        public static string strDisplayLastSix = "0";//登记时,只显示有效证件号码后六位
        public static string strManyCard = "0";//登记时,允许同张证件签发多张常客卡
        public static string strImmediatePrint = "0";//登记时,允许不选被访对象可直接登记打印
        public static string strPrintTemCardReport = "0";//登记时,允许签发临时卡后打印出入证
        public static string strDisplayStaffImage = "0";//登记时,允许被访对象列表显示头像
        public static string strPrintVIdImageFirst = "0";//登记时,若访客头像未采集,则打印证件图像
        public static string strIdNoLeave = "0";//签离时,允许采用证件号码进行签离
        public static string strBarcodeAffirm = "0";//条码签离时,允许弹出确认框
        public static string strMLeaderMP = "";//领导手机（短信发送）
        public static string strMUser = "";//操作员名（短信发送）
        public static string strMPassword = "";//密码（短信发送）
        public static string strMOpenMS = "0";//开启短信发送功能
        public static string strMBlackList = "0";//登记时,若发现黑名单,则短信通知负责人（短信发送）
        public static string strMFixedTime = "0";//每天定时发送一天登记情况,时间（短信发送）
        public static string strMFixedTime_Hour = "18";//小时（短信发送）
        public static string strMFixedTime_Minute = "0";//分钟（短信发送）
        public static string strMBackup = "0";//发现有备份数据库时,则短信通知负责人（短信发送）
        public static string strSWindows = "0";//登记时,允许屏蔽Windows键和任务管理器 
        public static string strAutoBackupBA = "0";//自动压缩备份基础年份,时间为每个月的
        public static string strAutoBackupBADayValue = "1";//多少号
        public static string strAutoBackupGA = "0";//自动压缩备份来访年份,时间为每个月的
        public static string strAutoBackupGADayValue = "1";//多少号
        public static string strAutoBackupCNo = "";//要备份的来访年份
        public static string strAutoBackupCPath = "";//要备份的来访年份所存放的路径
        public static string strBarrierGate = "0";  //道闸开启后,延迟多少秒后自动回落
        public static string strBarrierGateSecond = "5";  //道闸开启后,延迟多少秒后自动回落
        public static string strAutoTimeBackupBA = "0";  //每天定时备份基础年份,时间为
        public static string strAutoTimeBackupBA_Hour = "18";  // 
        public static string strAutoTimeBackupBA_Minute = "0";  // 
        public static string strAutoTimeBackupGA = "0";  //每天定时备份来访记录年份,时间为
        public static string strAutoTimeBackupGA_Hour = "18";  // 
        public static string strAutoTimeBackupGA_Minute = "0";  // 
        public static string strAutoTimeBackupGACNo = "";//要定时备份的来访年份
        public static string strAutoBackUpPath = "";//要备份年份的路径
        public static string strQPrintReport = "";//快速登记时，允许打印出入证
        public static string strBPRegister = "";//登记时,允许发现黑名单人员后能继续登记
        public static string strVisitorIsOrder = "";//登记时,允许开启访客预约功能
        public static string strVisitorIsAffirm = "";//登记时,允许开启访客查询功能
        public static string strTemParkingCard = "0";//登记时,允许开启停车证号功能
        public static string strLeftLuggage = "0";//登记时,允许开启物品寄存柜功能
        public static string strTemDoorControl = "0";//登记时,允许开启临时门禁卡号功能(证件号)
        public static string strClearDB = "1";//每个月1号自动整理基础年份和来访年份
        public static string strClearDBMonth = "1";//当前月份
        public static string strVisitorIsNull = "0";//登记时，允许不录入来访人信息
        public static string strAllowedSecondWay = "0";//登记时，允许开启二道岗亭功能
        public static string strAllowedFDepSStaff = "0";//登记时，允许先选择被访部门后选择被访人员
        public static string strAllowedApplyForJob = "0";//登记时,允许开启应聘招工人员登记功能
        public static string strAllowedInfrastructure = "0";//登记时,允许开启基建施工人员登记功能
        public static string strAbnormalLeaved = "1";//登记时,若访客非正常签离次数达到多少次则提醒
        public static string strAbnormalLeavedValue = "3";//次数
        public static string strAllowedMultiVisitorReg = "1";//登记时,允许开启同一批次多人来访登记功能
        public static string strAllowedSameIDRepeatedReg = "0";//登记时,允许开启同张证件重复登记功能
        public static string strAllowedOnlyDep = "1";//登记时,被访对象窗体里只允许选择被访部门
        public static string strAllowedVisitorFingerprint = "0";//登记时,允许开启访客指纹登记功能

        public static string strAllowedSoundRecord = "0";//登记时,允许开启现场录音功能,每个录音时长
        public static string strAllowedSoundRecord_Minute = "30";  //每个录音时长30分
        public static string strAllowedSoundRecordPath ="";  //每个录音存放的地方
        public static string strAllowedWEvaluatingDevice = "0";//登记时,允许开启窗口评价器功能


        public static string strSMSCatComPort = "1";//短信猫端口号
        public static string strSMSCatMBlackListForOfficer = "0";//登记时,若发现黑名单,则短信通知负责人(短信猫)
        public static string strSMSCatVisitorForStaff = "0";//登记时,短信通知被访人有人来访(短信猫)
        public static string strSMSCatSMSForGoods = "0";//门卫室物品代收短信通知(短信猫)
        public static string strSMSCatMFixedTime = "0";//每天定时发送一天登记情况,时间（短信发送）(短信猫)
        public static string strSMSCatMFixedTime_Hour = "18";//小时（短信发送）(短信猫)
        public static string strSMSCatMFixedTime_Minute = "0";//分钟（短信发送）(短信猫)
        public static string strSMSCatMBackupForOfficer = "0";//发现有备份数据库时,则短信通知负责人(短信猫)
        public static string strAllowedIDCardValidedVerification = "1";//登记时，允许开启访客证件有效期验证功能

        public static string strAllowedOpenDoorDelay = "1";//是否开启
        public static string strAllowedOpenDoorDelaySecond = "3";//临时开门的时长，单位：秒
        public static string strAllowedEnterAndLeaveCount = "1";//是否开启
        public static string strAllowedEnterCount = "1";//门禁卡片在有效时间段内只允许刷卡进入1次
        public static string strAllowedLeaveCount = "1";//门禁卡片在有效时间段内只允许刷开离开1次

        public static string strAllowedOpenMSCat= "1";//自助登记时，允许开启手机短信验证功能
        public static string strAllowedVIPOpenDoorDelay = "1";//VIP卡功能是否开启
        public static string strAllowedVIPOpenDoorDelaySecond = "3";//临时开门的时长，单位：秒


        public static int iFlag_SApproverIsStaff = 1;//登记时，访客来访消息确认由被访对象审批
        public static string strAppNo = "", strAppActualNo = "", strAppName = "", strAppDetailName = "", strAppHostIP = "";//访客来访提醒助手和预约助手里的审批人员信息

        public static DataSet ds_AccoutInf = new DataSet();
        private DataSet ds_Operator = new DataSet();
        public static DataSet ds_Configuration = new DataSet();
        private DataTable dt_Operator = new DataTable();
        private DataTable dt_AccoutInf = new DataTable();
        public static string strT_OperateLogInf = " XXCLOUD.dbo.T_OperateLogInf ";//字符串前后加上空格，确保SQL字符串不会出错。
        public static string strFWQIP = "";             //数据库服务器名称或IP地址
        public static string strConn = "";
        public static string strConn_Remote = "";//远程连接，用于保存远程图片
        public static bool blCanRegist = false;         //记录此系统是否已注册 false:未注册  true:已注册
        public static string strFlag_Login = "";		//记录检验正确操作员是否通过 true:通过
        public static string strGLJS = "1";
        public static string strNLJS = "1";
        public static string strByInternet = "0";//1:不在同一局域网的多机互联  0：同个局域网内的多机互联
        public static string strOperatorNo = "";        //记录正确操作员编号
        public static string strOperatorActualNo = "";   //记录正确操作员操作员编号
        public static string strOperatorName = "";  	//记录正确操作员姓名
        public static string strEvaluateImageIndex = "";  	//记录正确操作员的评价器中相对应的图片索引值

        public static string strCAccout = "";              //当前年份   
        public static string strOperatorPower = "0";    //操作员级别  1:前台  0:后台
        public static string strAccessPwd = "XXCLOUD";
        public static string strGuardRoomId = "";  //当前门岗编号
        public static string strGuardRoomName = "";  //当前门岗名称
        public static string strRubbishClear = "";  //垃圾清除
        public static string strEnduserNo = "";  //本机使用单位代码
        public static string strEnduserName = "";  //本机使用单位 
        public static string strSupplierNo = "";  //本机供应商名称代码
        public static string strSupplierName = "";  //本机供应商名称
        public static string strMachineKey = "";//数字签名，用于数据上传匹配(本机使用单位代码+CPU序列号)
        public static string strSelectSecondWay = "";
        private string strSQL = "";
        private DataTable myTable = new DataTable();
        private GetHardwareInf GHI = new GetHardwareInf();
        public static string strSqlServer = "";//数据库服务器名称
        private string strDDIdConfirm = "0";//身份验证  0：Windows身份验证  1:SQL Server身份验证
        private string strDLoginName = "";//数据库登录名称
        private string strDLoginPwd = "";//数据库登录密码
        private string strCardNo = "";
        private string strSIDNo = "";
        private string strCardCom_Ic = "1";//IC串口
        public static string strCardCom_Id = "2";//ID串口
        public static string strSMSCatCom= "3";//短信猫串口号
        private string strOperatorNo_Temp = "";
        private bool bl_IDDL = false;
        public static int iFlag_FingerprintSucOpened = 1;//0:中控URU5000指纹采集仪加载失败  1：指纹采集仪加载成功
        public static int iFlag_SingleToDoubleEncrption = 1;//0:正常网络狗版软件  1：用单机狗来当网络狗使用
        public static int iFlag_LockEncryptionType = 1;//1:普通加密锁  0：时针锁
        public static int iFlag_LockEncryption = 1;//0:单机锁  1：网络锁  2：没有检测到加密锁
        public static int iFlag_OpenMesCommunication =0;//在网络版中开启消息队列  0:不开启，即采用定时刷新 1：开启，即采用消息队列
        public static int iFlag_IdCardReaderType = 1;//0:XZX三合一  1:PT 国腾 精伦单一版 2:SS二合一  3:JL二合一  
        public static int iFlag_SMSCatIsEnabled = 0;//0:不开启短信猫收发短信功能   1：开启
        public static int iFlag_RemoteServerIsEnabled = 0;//0:没有远程服务版  1：开启远程服务器版
        public static Boolean blSucOpened = false;//新中新读卡器 false:加载失败 true:加载成功
        public static int iSucOpened = 1;//神思二合一读卡器 1:加载失败 0:加载成功
        public static int iSucOpenedSMS = 1;//短信发送设备  1：端口打开成功   0：端口打开失败   
        public static int iFlag_IDReader = 0;//O:采用普通IC读卡器， 1：采用ID读卡器 2：采用可以接两个USB读卡器的IC读卡器
        public static int iFlag_OpenVideo = 0;//0:普通登记  1：包括内部员工刷卡
        private string inputData = string.Empty;
        public static int iFlag_OpenSIdReaderPT = 1;//打开普天读卡器   0：成功
        public static int iFlag_OpenSIdReaderJL = 1;//打开精伦二合一读卡器   0：成功
        public static int iFlag_OpenSIdReaderZK = 1;//打开中控三合一读卡器   0：成功

        public static int iImageNo_Id = 1;//证件上的图片编号
        private string[] ASetings = new string[500];
        private Int64 iCount = 0;//新中新读卡器用来区分读卡和二代证
  
        private long nSerialNo;//新中新IC卡
        public static int iFlag_BeepIsEnabled = 1;//1:主板蜂鸣器发出声音  0：不发声
        public static string strSqlServer_Remote = "";//远程数据库服务器名称 //本机若为服务器，则默认为127.0.0.1
        public static string strDDIdConfirm_Remote = "1";//身份验证  0：Windows身份验证  1:SQL Server身份验证
        public static string strDLoginName_Remote = "";//远程数据库登录名称
        public static string strDLoginPwd_Remote = "";//远程数据库登录密码
        public static string strDLinked_Remote = "0";//0:未连接上  1:已连接
        public static DataTable dt_Visitor = new DataTable();
        public static int iPrinterType = 0;//0:BT-T080  1:TX 80 Thermal
        public static string strPrinterSPace = "  ";//出入证上的标题空格数量
        public static string strPeopleCameraName = "";//人像捕捉摄像头名称

        public static string strAutoAllLeaveTime = "0";//每天定时签离来访记录,时间（上传来访记录）
        public static string strAutoAllLeave_Hour = "8";//小时（上传来访记录）
        public static string strAutoAllLeave_Minute = "0";//分钟（上传来访记录）
        public static string strAllowedRefresh = "0";//登记时，允许登记完成后自动清空登记界面
        public static int iFlag_ICCardReadPort = 5;//中控IC读卡器的端口
        public static string strOrderServerIP = "0";//预约表
        public static string strOrderLoginName = "0";
        public static string strOrderLoginPwd = "0";
        public static string strOrderDBName = "";
        public static int iFlag_MesCatPort = 7;//短信猫端口

        #region//本地Access的系统配置
        public static string strAllowedAllLeaveNormal = "0";//每天自动定时签离全部未签离来访记录并记为正常签离,时间为
        public static string strAllowedAllLeaveNormal_Hour = "0";
        public static string strAllowedAllLeaveNormal_Minute = "0";

        public static string strAllowedAllLeaveAbNormal = "0";//每天自动定时签离全部未签离来访记录并记为1次非正常签离,时间为
        public static string strAllowedAllLeaveAbNormal_Hour = "0";
        public static string strAllowedAllLeaveAbNormal_Minute = "0";

        public static string strAllowedLogoutRedCardNormal = "0";//每天自动定时从门禁控制器上注销所有未注销的红卡权限并记为正常注销,时间为
        public static string strAllowedLogoutRedCardNormal_Hour = "0";
        public static string strAllowedLogoutRedCardNormal_Minute = "0";


        public static string strAllowedLogoutRedCardAbNormal = "0";//每天自动定时从门禁控制器上注销所有未注销的红卡权限并记为1次非正常注销,时间为
        public static string strAllowedLogoutRedCardAbNormal_Hour = "0";
        public static string strAllowedLogoutRedCardAbNormal_Minute = "0";

        public static string strAllowedLogoutBlueCard = "0";//每天自动定时根据蓝卡有效期限从门禁控制器上注销所有未注销的蓝卡权限,时间为
        public static string strAllowedLogoutBlueCard_Hour = "0";
        public static string strAllowedLogoutBlueCard_Minute = "0";

        public static string strAllowedOverdueOrder = "0";//每天自动定时根据预约有效期限把过期预约设为异常预约,时间为
        public static string strAllowedOverdueOrder_Hour = "0";
        public static string strAllowedOverdueOrder_Minute = "0";
        
        #endregion

        #region//读写Ini文件
        #region 变量声明区
        public string strINIPath = Application.StartupPath.Trim() + "\\ConnectString\\ConnectString.ini";//该变量保存INI文件所在的具体物理位置
        public string strININame = "";//获取INI文件的文件名
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName);

        public string ContentReader(string area, string key, string def)
        {
            StringBuilder stringBuilder = new StringBuilder(1024); 				//定义一个最大长度为1024的可变字符串
            GetPrivateProfileString(area, key, def, stringBuilder, 1024, strINIPath); 			//读取INI文件
            return stringBuilder.ToString();								//返回INI文件的内容
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string mpAppName,
            string mpKeyName,
            string mpDefault,
            string mpFileName);

        #endregion
        #endregion

        #region//操作日志
        private void OperatorLog(string para_strOperateDescribe, string para_strResult)
        {
            try
            {
                string strOperatorLog = "insert into " + LoginFrm.strT_OperateLogInf + " (OperatorNo,OperatorActualNo,OperatorName,";
                strOperatorLog += " FormName,OperateDescribe,Result,OperateDT,Flag,CAccoutNo)values('" + LoginFrm.strOperatorNo + "',";
                strOperatorLog += "'" + LoginFrm.strOperatorActualNo + "','" + LoginFrm.strOperatorName + "','" + "登录窗体" + "',";
                strOperatorLog += "'" + para_strOperateDescribe + "','" + para_strResult + "','" + System.DateTime.Now.ToString() + "','" + "访客系统" + "',";
                strOperatorLog += "'" + LoginFrm.strCAccout.Substring(1, 4).Trim() + "')";
                SQLHelper.ExecuteSql(strOperatorLog);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "日志写入错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//清除系统垃圾文件
        private void RubblishClear(string para_strCleared)
        {
            try
            {
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + LoginFrm.strAccessPwd + "'";
                strSQL = "update T_LocConfigurationInf set A11 ='" + para_strCleared + "'";
                System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
                OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.ToString ());
            }
        }
        #endregion

        #region//加载本地年份信息
        private void LoadAccoutList()
        {
            this.grid_AccoutList.AutoRedraw = false;
            this.grid_AccoutList.Height = 1;
            this.grid_AccoutList.Rows = 1;
            ds_AccoutInf.Tables.Clear();
            ds_AccoutInf = XXCLOUDDLL.LoginFrm_LoadLocalAccoutInf(Application.StartupPath.Trim(), strAccessPwd);//加载年份
            if (ds_AccoutInf.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds_AccoutInf.Tables[0].Rows.Count; i++)
                {
                    //this.cmbox_AccoutList.Items.Add();
                    this.grid_AccoutList.Rows++;
                    this.grid_AccoutList.Cell(this.grid_AccoutList.Rows - 1, 1).Tag = ds_AccoutInf.Tables[0].Rows[i]["ANo"].ToString();
                    this.grid_AccoutList.Cell(this.grid_AccoutList.Rows - 1, 1).Text = "[" + ds_AccoutInf.Tables[0].Rows[i]["ANo"].ToString() + "]" + ds_AccoutInf.Tables[0].Rows[i]["AName"].ToString();
                    this.grid_AccoutList.Cell(this.grid_AccoutList.Rows - 1, 2).SetImage(@"Sample1");
                    this.grid_AccoutList.Cell(this.grid_AccoutList.Rows - 1, 2).Alignment = FlexCell.AlignmentEnum.CenterCenter;
                    this.grid_AccoutList.Cell(this.grid_AccoutList.Rows - 1, 1).ForeColor = Color.Blue;
                    this.grid_AccoutList.Row(this.grid_AccoutList.Rows - 1).Locked = true;
                    this.grid_AccoutList.Height += 28;
                }
                this.txt_AccoutList.Text = this.grid_AccoutList.Cell(1, 1).Text.Trim();
            }
            if (this.grid_AccoutList.Rows >= 9)
            {
                this.grid_AccoutList.Height = 250;
            }
            this.grid_AccoutList.AutoRedraw = true;
            this.grid_AccoutList.Refresh();
        }
        #endregion

        #region//加载本地操作员信息
        private void LoadOperatorList()
        {
            this.grid_Operator.AutoRedraw = false;
            this.grid_Operator.Height = 1;
            this.grid_Operator.Rows = 1;
            ds_AccoutInf.Tables.Clear();
            ds_AccoutInf = XXCLOUDDLL.LoginFrm_LoadLocalOperatorInf(Application.StartupPath.Trim(), strAccessPwd); ;//加载操作员,即同一操作员可登录任何年份
            if (ds_AccoutInf.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds_AccoutInf.Tables[0].Rows.Count; i++)
                {
                    //this.cmbox_AccoutList.Items.Add();
                    this.grid_Operator.Rows++;
                    this.grid_Operator.Cell(this.grid_Operator.Rows - 1, 1).Tag = ds_AccoutInf.Tables[0].Rows[i]["ONo"].ToString();
                    this.grid_Operator.Cell(this.grid_Operator.Rows - 1, 1).Text =  ds_AccoutInf.Tables[0].Rows[i]["OName"].ToString();
                    this.grid_Operator.Cell(this.grid_Operator.Rows - 1, 2).SetImage(@"Sample1");
                    this.grid_Operator.Cell(this.grid_Operator.Rows - 1, 2).Alignment = FlexCell.AlignmentEnum.CenterCenter;
                    this.grid_Operator.Cell(this.grid_Operator.Rows - 1, 1).ForeColor = Color.Blue;
                    this.grid_Operator.Row(this.grid_Operator.Rows - 1).Locked = true;
                    this.grid_Operator.Height += 28;
                }
                this.txt_Operator.Text = this.grid_Operator.Cell(1, 1).Text.Trim();
                strOperatorNo = this.grid_Operator.Cell(1, 1).Tag.Trim();
            }
            if (this.grid_Operator.Rows >= 9)
            {
               this.grid_Operator.Height =250;
            }
            this.grid_Operator.AutoRedraw = true;
            this.grid_Operator.Refresh();
        }
        #endregion

        #region//获取本机默认打印机
        static string GetDefaultPrinter()
        {
            const int ERROR_FILE_NOT_FOUND = 2;

            const int ERROR_INSUFFICIENT_BUFFER = 122;

            int pcchBuffer = 0;

            if (GetDefaultPrinter(null, ref pcchBuffer))
            {
                return null;
            }

            int lastWin32Error = Marshal.GetLastWin32Error();

            if (lastWin32Error == ERROR_INSUFFICIENT_BUFFER)
            {
                StringBuilder pszBuffer = new StringBuilder(pcchBuffer);
                if (GetDefaultPrinter(pszBuffer, ref pcchBuffer))
                {
                    return pszBuffer.ToString();
                }

                lastWin32Error = Marshal.GetLastWin32Error();
            }
            if (lastWin32Error == ERROR_FILE_NOT_FOUND)
            {
                return null;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        #endregion

        private void LoginFrm_Load(object sender, EventArgs e)
        {
       
            try
            {
                this.Cursor = Cursors.WaitCursor;
                clsIme.SetIme(this);//控制输入法的状态
                if (GetDefaultPrinter().ToString().Contains("BT-T080") == true)
                {
                    iPrinterType = 0;
                    strPrinterSPace = " ";
                }
                else if (GetDefaultPrinter().ToString().Contains("TX 80 Thermal") == true)
                {
                    iPrinterType = 1;
                    strPrinterSPace = "  ";
                }
                else
                {
                    iPrinterType = 1;
                    strPrinterSPace = "  ";
                }
                dt_Visitor.Columns.Add("Id");
                Application.DoEvents();
                if (Directory.Exists(Application.StartupPath.Trim() + "\\Camera") == false)
                {
                    Directory.CreateDirectory(Application.StartupPath.Trim() + "\\Camera");//新建目录
                }
                if (Directory.Exists(Application.StartupPath.Trim() + "\\CatchImages") == false)
                {
                    Directory.CreateDirectory(Application.StartupPath.Trim() + "\\CatchImages");//新建目录
                }
                if (Directory.Exists(Application.StartupPath.Trim() + "\\ConnectString") == false)//判断此文件夹是否存在
                {
                    Directory.CreateDirectory(Application.StartupPath.Trim() + "\\ConnectString");
                }
                #region//读取INI文件
                strININame = System.IO.Path.GetFileNameWithoutExtension(strINIPath);
                if (File.Exists(strINIPath))//判断是否存在该INI文件
                {

                    iFlag_MesCatPort = Convert.ToInt32(ContentReader(strININame, "MesCatPort", "7"));
                    strOrderServerIP = ContentReader(strININame, "OrderServerIP", "192.168.1.15");
                    strOrderLoginName = ContentReader(strININame, "OrderLoginName", "sa");
                    strOrderLoginPwd = ContentReader(strININame, "OrderLoginPwd", "123456");
                    strOrderDBName = ContentReader(strININame, "OrderDBName", "LogisticsDB.dbo.ReservationRecord");
              
                }
                else
                {
                    WritePrivateProfileString(strININame, "MesCatPort", "7", strINIPath);
                    WritePrivateProfileString(strININame, "OrderServerIP", "192.168.1.15", strINIPath); 		//修改INI文件中服务器节点的内容
                    WritePrivateProfileString(strININame, "OrderLoginName", "sa", strINIPath); 		//修改INI文件中服务器节点的内容
                    WritePrivateProfileString(strININame, "OrderLoginPwd", "123456", strINIPath); 		//修改INI文件中服务器节点的内容
                    WritePrivateProfileString(strININame, "OrderDBName", "LogisticsDB.dbo.ReservationRecord", strINIPath); 		//修改INI文件中服务器节点的内容
       
                }

                if (iFlag_IdCardReaderType != 0 && iFlag_IdCardReaderType != 1 && iFlag_IdCardReaderType != 2 && iFlag_IdCardReaderType != 3 && iFlag_IdCardReaderType != 4 && iFlag_IdCardReaderType != 5)
                {
                    iFlag_IdCardReaderType = 5;
                }
                if (iFlag_BeepIsEnabled != 0 && iFlag_BeepIsEnabled != 1)
                {
                    iFlag_BeepIsEnabled = 1;
                }
                if (iFlag_RemoteServerIsEnabled != 0 && iFlag_RemoteServerIsEnabled != 1)
                {
                    iFlag_RemoteServerIsEnabled = 0;
                }
                if (iFlag_SMSCatIsEnabled != 0 && iFlag_SMSCatIsEnabled != 1)
                {
                    iFlag_SMSCatIsEnabled = 0;
                }
                if (iFlag_SApproverIsStaff != 0 && iFlag_SApproverIsStaff != 1)
                {
                    iFlag_SApproverIsStaff = 1;
                }

                if (File.Exists(Application.StartupPath.Trim() + "\\ConnectString\\ShuoMing.txt") != true)
                {
                    FileStream fs1 = new FileStream(Application.StartupPath.Trim() + "\\ConnectString\\ShuoMing.txt", FileMode.Create, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs1);

                    sw.WriteLine("/**************关于ConnectString.ini文件各行内容介绍如下**************/");//开始写入值
                    sw.WriteLine("     ");//开始写入值
                    sw.WriteLine("一、MesCatPort：指短信猫的通信端口。");//开始写入值
                    sw.WriteLine("      ");//开始写入值
                    sw.WriteLine("二、OrderServerIP：指预约端服务器的IP地址。");//开始写入值
                    sw.WriteLine("      ");//开始写入值
                    sw.WriteLine("三、OrderLoginName：指预约端服务器的SQL数据库登录名。");//开始写入值
                    sw.WriteLine("      ");//开始写入值
                    sw.WriteLine("四、OrderLoginPwd：指预约端服务器的SQL数据库登录密码。");//开始写入值
                    sw.WriteLine("      ");//开始写入值
                    sw.WriteLine("五、OrderDBName：指预约端服务器的数据库及其表名。");//开始写入值
                    sw.WriteLine("      ");//开始写入值
                    sw.Close();
                    fs1.Close();
                }
                #endregion


                for (int i = 0; i < ASetings.Length; i++)
                {
                    ASetings[i] = "0";
                }

                dt_Operator.Columns.Add("strOperatorNo");
                dt_Operator.Columns.Add("strOperatorActualNo");
                dt_Operator.Columns.Add("strOperatorName");
                dt_Operator.Rows.Clear();
                try
                {
                    //1.读取本地全部年份信息
                    LoadAccoutList();
                    //2.读取本地全部操作员信息
                    LoadOperatorList();
                    //3.加载本地配置信息
                    //A1:服务器名称，A2：身份验证  A3：登录名  A4：登录密码  A5:当前门岗编号  A6：当前门岗名称
                    //A7:软键盘     A8:IC卡端口号  A9：ID卡端口号             A11:清除垃圾（0：未清除 1：已清除 ）
                    //A12:短信猫端口号
                    ds_Configuration = XXCLOUDDLL.LoginFrm_LoadLocalConfigurationInf(Application.StartupPath.Trim(), strAccessPwd);
                    if (ds_Configuration.Tables[0].Rows.Count > 0)
                    {
                        if (ds_Configuration.Tables[0].Rows[0]["A1"].ToString().Trim() != "" && ds_Configuration.Tables[0].Rows[0]["A1"].ToString().Trim() != null)
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A1"].ToString().Trim());
                            strSqlServer = strSQL.Substring(0, strSQL.Length - 2);
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A2"].ToString().Trim() != "" && ds_Configuration.Tables[0].Rows[0]["A2"].ToString().Trim() != null)
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A2"].ToString().Trim());
                            strDDIdConfirm = strSQL.Substring(0, strSQL.Length - 2);
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A3"].ToString().Trim() != "" && ds_Configuration.Tables[0].Rows[0]["A3"].ToString().Trim() != null)
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A3"].ToString().Trim());
                            strDLoginName = strSQL.Substring(0, strSQL.Length - 2);
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A4"].ToString().Trim() != "" && ds_Configuration.Tables[0].Rows[0]["A4"].ToString().Trim() != null)
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A4"].ToString().Trim());
                            strSQL = strSQL.Substring(0, strSQL.Length - 2);
                            if (strSQL != "A4")
                            {
                                strDLoginPwd = strSQL;
                            }
                            else
                            {
                                strDLoginPwd = "";
                            }
                        }

                        if (ds_Configuration.Tables[0].Rows[0]["A5"].ToString().Trim() != "")
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A5"].ToString().Trim());
                            strGuardRoomId = strSQL.Substring(0, strSQL.Length - 2);
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A6"].ToString().Trim() != "")
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A6"].ToString().Trim());
                            strGuardRoomName = strSQL.Substring(0, strSQL.Length - 2);
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A8"].ToString().Trim() == "")
                        {
                            strCardCom_Ic = "1";
                        }
                        else
                        {
                            strCardCom_Ic = ds_Configuration.Tables[0].Rows[0]["A8"].ToString().Trim();
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A9"].ToString().Trim() == "")
                        {
                            strCardCom_Id = "2";
                        }
                        else
                        {
                            strCardCom_Id = ds_Configuration.Tables[0].Rows[0]["A9"].ToString().Trim();
                        }
                        //if (ds_Configuration.Tables[0].Rows[0]["A10"].ToString().Trim() == "1")
                        //{
                        //    iFlag_LockEncryption = 1;
                        //}
                        //else
                        //{
                        //    iFlag_LockEncryption = 0;
                        //}
                        iFlag_LockEncryption = 1;
                        if (ds_Configuration.Tables[0].Rows[0]["A11"].ToString().Trim() == "1")
                        {
                            strRubbishClear = "1";
                        }
                        else
                        {
                            strRubbishClear = "0";
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A12"].ToString().Trim() == "")//短信猫端口号
                        {
                            strSMSCatCom = "3";
                        }
                        else
                        {
                            strSMSCatCom = ds_Configuration.Tables[0].Rows[0]["A12"].ToString().Trim();
                            if (strSMSCatCom.Length >= 4)
                            {
                                strSMSCatCom = strSMSCatCom.Substring(3);
                            }
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A15"].ToString().Trim() != "")//远程数据库服务器名称
                        {
                            strSqlServer_Remote = ds_Configuration.Tables[0].Rows[0]["A15"].ToString().Trim();
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A16"].ToString().Trim() != "")//身份验证  0：Windows身份验证  1:SQL Server身份验证
                        {
                            strDDIdConfirm_Remote = ds_Configuration.Tables[0].Rows[0]["A16"].ToString().Trim();
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A17"].ToString().Trim() != "")//远程数据库登录名称
                        {
                            strDLoginName_Remote = ds_Configuration.Tables[0].Rows[0]["A17"].ToString().Trim();
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A18"].ToString().Trim() != "")//远程数据库登录密码
                        {
                            strDLoginPwd_Remote = ds_Configuration.Tables[0].Rows[0]["A18"].ToString().Trim();
                        }

                        
                    }
                    else
                    {
                        strSqlServer = Dns.GetHostName();
                        strDDIdConfirm = "1";
                        strDLoginName = "sa";
                        strDLoginPwd = "123456";
                        strCardCom_Ic = "1";
                    }
                     
                    this.timer_CardRead.Start();
                    this.timer_SIdRead.Start();//操作员可用IC或ID卡登录
                    this.Cursor = Cursors.Default;
                    if (this.txt_AccoutList.Text.Trim() == "")
                    {
                        this.txt_AccoutList.Focus();
                    }
                    else if (this.txt_Operator.Text.Trim() == "")
                    {
                        this.txt_Operator.Focus();
                    }
                    else
                    {
                        this.txt_Password.Focus();
                    }
                   this.txt_Operator.Text = "后台";
                   this.txt_Password.Text = "";
                  // pic_Enter_Click(sender, e);
                }
                catch (Exception exp)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString ());
            }
        }

        private void pic_Enter_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < ASetings.Length; i++)
                {
                    ASetings[i] = "";
                }
                this.Cursor = Cursors.WaitCursor;
                this.grid_AccoutList.Visible = false;
                this.grid_Operator.Visible = false;
                string strAccout = this.txt_AccoutList.Text.Trim().Trim();
                strOperatorName = this.txt_Operator.Text.Trim();
                if (strAccout == "")
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("请选择或新建年份!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (strOperatorName == "")
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("请选择或输入操作员姓名!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (SQLHelper.CheckFromInput(strOperatorName.ToLower()) == false)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("输入的操作员中包含非法字符，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.txt_Operator.Text = "";
                    this.txt_Operator.Focus();
                    return;
                }
                string strPassword = this.txt_Password.Text.Trim();
                if (bl_IDDL == false)
                {
                    strOperatorNo_Temp = "";
                    try
                    {
                        strOperatorNo_Temp = strOperatorNo;
                    }
                    catch
                    {
                        strOperatorNo_Temp = "";
                    }
                }
                if (strPassword != "")
                {
                    strPassword = SQLHelper.EncryptString(strPassword);//对输入的密码进行加密
                }
                string strANo = strAccout.Substring(1, 4).Trim();
                string strAName = strAccout.Substring(6).Trim();
                try
                {
                    if (SQLHelper.DBLink(strDDIdConfirm, strSqlServer, "XXCLOUD", strDLoginName, strDLoginPwd, strFlagDBLink, LoginFrm.strByInternet) == false)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("系统检测到SQL数据库未启动或SQL数据库连接失败，请稍等几秒钟后点击【登陆】按钮重试!\n\n***注意：如果是网络版，请确认数据库服务器已开机使用。***", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    //if (SQLHelper_Remote.DBLink("1", strOrderServerIP, "1", strOrderLoginName, strOrderLoginPwd,  strFlagDBLink, LoginFrm.strByInternet) == false)
                    //{
                    //    this.Cursor = Cursors.Default;
                    //    MessageBox.Show("无法连接到预约数据库服务器,请确认配置正确！\n\n***注意：如果是网络版，请检查以下原因：\n\n 1、请确保本机与数据库服务器之间的网络畅通 \n 2、数据库服务器已开机使用，且关闭防火墙***", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //    return;
                    //}
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //if (strOperatorNo_Temp == "")
                //{
                    strSQL = "select * from XXCLOUD.dbo.T_OperatorInf where OName='" + this.txt_Operator.Text.Trim() + "' and OPassword='" + strPassword + "' and Power ='" + "1" + "'";
                //}
                //else
                //{
                //    strSQL = "select * from XXCLOUD.dbo.T_OperatorInf where OName ='" + this.txt_Operator.Text.Trim() + "' and ONo ='" + strOperatorNo_Temp + "' and OPassword='" + strPassword + "' and Power ='" + "1" + "'";
                //}
                DataTable dtTemp = new DataTable();
                try
                {
                    dtTemp = SQLHelper.DTQuery(strSQL);
                }
                catch
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("系统检测到SQL数据库未启动或SQL数据库连接失败，请稍等几秒钟后点击【登陆】按钮重试!\n\n***注意：如果是网络版，请确认数据库服务器已开机使用。***", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                myTable = dtTemp.Copy();
                if (myTable.Rows.Count > 0)
                {
                    strCAccout = strAccout;//当前年份
                    strOperatorNo = myTable.Rows[0]["ONo"].ToString().Trim();//操作员编号
                    strOperatorActualNo = myTable.Rows[0]["OActualNo"].ToString().Trim();//操作员编号
                    strOperatorName = myTable.Rows[0]["OName"].ToString().Trim();//操作员姓名
                    strEvaluateImageIndex = myTable.Rows[0]["EvaluateImageIndex"].ToString().Trim();//操作员在评价器中的图片索引值
                    strOperatorPower = myTable.Rows[0]["Power"].ToString().Trim();//操作员级别
                    if (LoginFrm.iFlag_SMSCatIsEnabled == 1)//若开启短信猫收发短信功能，则读取短信猫
                    {
                        myTable.Rows.Clear();
                        strSQL = "select * from XXCLOUD.dbo.T_MSCatSettingInf  ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count <= 0)
                        {
                            strSQL = "insert into XXCLOUD.dbo.T_MSCatSettingInf (ASetings)values('" + "1,1,1,1,1,18,0,1,0,0,1,3,1,0,0,0,0,0,0,0,0,0," + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            strSQL = "select * from XXCLOUD.dbo.T_MSCatSettingInf  ";
                            myTable = SQLHelper.DTQuery(strSQL);
                        }
                        if (myTable.Rows.Count > 0)
                        {
                            int iCount = 0;
                            strSQL = "";
                            for (int i = 0; i < myTable.Rows[0]["ASetings"].ToString().Trim().Length; i++)
                            {
                                if (myTable.Rows[0]["ASetings"].ToString().Trim().Substring(i, 1).ToString() != ",")
                                {
                                    strSQL += myTable.Rows[0]["ASetings"].ToString().Trim().Substring(i, 1).ToString().Trim();
                                }
                                else
                                {
                                    ASetings[iCount++] = strSQL;
                                    strSQL = "";
                                }
                            }

                            strSMSCatComPort = ASetings[0].ToString().Trim();////短信猫端口号
                            strSMSCatMBlackListForOfficer = ASetings[1].ToString().Trim();//登记时,若发现黑名单,则短信通知负责人(短信猫)
                            strSMSCatVisitorForStaff = ASetings[2].ToString().Trim();//登记时,短信通知被访人有人来访(短信猫)
                            strSMSCatSMSForGoods = ASetings[3].ToString().Trim();//门卫室物品代收短信通知(短信猫)
                            strSMSCatMFixedTime = ASetings[4].ToString().Trim();//每天定时发送一天登记情况,时间（短信发送）(短信猫)
                            strSMSCatMFixedTime_Hour = ASetings[5].ToString().Trim();//小时（短信发送）(短信猫)
                            strSMSCatMFixedTime_Minute = ASetings[6].ToString().Trim();//分钟（短信发送）(短信猫)
                            strSMSCatMBackupForOfficer = ASetings[7].ToString().Trim();//发现有备份数据库时,则短信通知负责人(短信猫)
                            if (strSMSCatComPort == "" || strSMSCatComPort == null)
                            {
                                strPrintReport = "1";
                            }
                            if (strSMSCatMBlackListForOfficer == "" || strSMSCatMBlackListForOfficer == null || strSMSCatMBlackListForOfficer == "0")
                            {
                                strSMSCatMBlackListForOfficer = "0";
                            }
                            else
                            {
                                strSMSCatMBlackListForOfficer = "1";
                            }
                            if (strSMSCatVisitorForStaff == "" || strSMSCatVisitorForStaff == null || strSMSCatVisitorForStaff == "0")
                            {
                                strSMSCatVisitorForStaff = "0";
                            }
                            else
                            {
                                strSMSCatVisitorForStaff = "1";
                            }
                            if (strSMSCatVisitorForStaff == "" || strSMSCatVisitorForStaff == null || strSMSCatVisitorForStaff == "0")
                            {
                                strSMSCatVisitorForStaff = "0";
                            }
                            else
                            {
                                strSMSCatVisitorForStaff = "1";
                            }
                            if (strSMSCatMFixedTime == "" || strSMSCatMFixedTime == null || strSMSCatMFixedTime == "0")
                            {
                                strSMSCatMFixedTime = "0";
                            }
                            else
                            {
                                strSMSCatMFixedTime = "1";
                            }
                            if (strSMSCatMFixedTime_Hour == "" || strSMSCatMFixedTime_Hour == null)
                            {
                                strSMSCatMFixedTime_Hour = "18";
                            }


                            if (strSMSCatMFixedTime_Minute == "" || strSMSCatMFixedTime_Minute == null)
                            {
                                strSMSCatMFixedTime_Minute = "0";
                            }
                            if (strSMSCatMBackupForOfficer == "" || strSMSCatMFixedTime == null || strSMSCatMBackupForOfficer == "0")
                            {
                                strSMSCatMBackupForOfficer = "0";
                            }
                            else
                            {
                                strSMSCatMBackupForOfficer = "1";
                            }
                        }


                    }

                    myTable.Rows.Clear();
                    strSQL = "select * from XXCLOUD.dbo.T_CloudVisitorRegSettingInf  ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count <= 0)
                    {
                        strSQL = "insert into XXCLOUD.dbo.T_CloudVisitorRegSettingInf (ASetings,MJASetings)values('" + "2,1,1,1,1,200,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,1,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,3,1,0,0,0,0,30,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0," + "','" + "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0," + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        strSQL = "select * from XXCLOUD.dbo.T_CloudVisitorRegSettingInf  ";
                        myTable = SQLHelper.DTQuery(strSQL);
                    }
                    if (myTable.Rows.Count > 0)
                    {
                        //try
                        //{
                        //    if (myTable.Rows[0]["AItem11"].ToString().Trim() != "" && myTable.Rows[0]["AItem11"].ToString().Trim() != null)
                        //    {
                        //        byte[] Data = (byte[])myTable.Rows[0]["AItem11"];
                        //        MemoryStream VImage = new MemoryStream(Data);
                        //        this.pbox_Logo.Image = Image.FromStream(VImage);
                        //        this.pbox_Logo.Image.Save(Application.StartupPath + "\\ReportType\\logo.jpg", ImageFormat.Jpeg);//图片另存为
                        //    }
                        //}
                        //catch
                        //{
                        //    this.pbox_Logo.Image = null;
                        //}
                        //MainBFrm.strEndUserName = myTable.Rows[0]["EndUserName"].ToString();
                        strPeopleCameraName = myTable.Rows[0]["PeopleCameraName"].ToString();//人像捕捉摄像头名称
                        strAllowedSoundRecordPath = myTable.Rows[0]["SoundRecordPath"].ToString();
                        strAutoBackUpPath = myTable.Rows[0]["BPath"].ToString();
                        strClearDBMonth = myTable.Rows[0]["AItem3"].ToString();
                        //ASetings = myTable.Rows[0]["ASetings"].ToString().Trim().Split(',');//以","分离字符串
                        int iCount = 0;
                        strSQL = "";
                        for (int i = 0; i < myTable.Rows[0]["ASetings"].ToString().Trim().Length; i++)
                        {
                            if (myTable.Rows[0]["ASetings"].ToString().Trim().Substring(i, 1).ToString() != ",")
                            {
                                strSQL += myTable.Rows[0]["ASetings"].ToString().Trim().Substring(i, 1).ToString().Trim();
                            }
                            else
                            {
                                ASetings[iCount++] = strSQL;
                                strSQL = "";
                            }
                        }
                        strReportType = ASetings[0].ToString().Trim();//报表类型
                        strPrintReport = ASetings[1].ToString().Trim();//登记时,允许打印出入证
                        strAutoRecognition = ASetings[2].ToString().Trim();//登记时,允许智能识别卡片
                        strLastVisitInf = ASetings[3].ToString().Trim();//登记时,允许自动读取访客最后一次访问信息
                        strDisplayQuantity = ASetings[4].ToString().Trim();//登记时,只显示被访对象列表前N位
                        strDisplayQuantityValue = ASetings[5].ToString().Trim();//位数
                        strDisplayLastSix = ASetings[6].ToString().Trim();//登记时,只显示有效证件号码后六位
                        strManyCard = ASetings[7].ToString().Trim();//登记时,允许同张证件签发多张常客卡
                        strImmediatePrint = ASetings[8].ToString().Trim();//登记时,允许不选被访对象可直接登记打印
                        strPrintTemCardReport = ASetings[9].ToString().Trim();//登记时,允许签发临时卡后打印出入证
                        strDisplayStaffImage = ASetings[10].ToString().Trim();//登记时,允许被访对象列表显示头像
                        strPrintVIdImageFirst = ASetings[11].ToString().Trim();//登记时,若访客头像未采集,则打印证件图像
                        strIdNoLeave = ASetings[12].ToString().Trim();//签离时,允许采用证件号码进行签离
                        strBarcodeAffirm = ASetings[13].ToString().Trim();//条码签离时,允许弹出确认框
                        strMLeaderMP = myTable.Rows[0]["MLeaderMP"].ToString().Trim();//领导手机（短信发送）
                        strMUser = myTable.Rows[0]["MUser"].ToString().Trim();//操作员名（短信发送）
                        if (myTable.Rows[0]["MPassowrd"].ToString().Trim() != "")
                        {
                            strMPassword = SQLHelper.DecryptString(myTable.Rows[0]["MPassowrd"].ToString().Trim());//密码（短信发送）
                        }
                        else
                        {
                            strMPassword = "";
                        }
                        strMOpenMS = ASetings[14].ToString().Trim();//开启短信发送功能
                        strMBlackList = ASetings[15].ToString().Trim();//登记时,若发现黑名单,则短信通知负责人（短信发送）
                        strMFixedTime = ASetings[16].ToString().Trim();//每天定时发送一天登记情况,时间（短信发送）
                        strMFixedTime_Hour = ASetings[17].ToString().Trim();//小时（短信发送）
                        strMFixedTime_Minute = ASetings[18].ToString().Trim();//分钟（短信发送）
                        strMBackup = ASetings[19].ToString().Trim();//发现有备份数据库时,则短信通知负责人（短信发送）
                        strSWindows = ASetings[20].ToString().Trim();//登记时,允许屏蔽Windows键和任务管理器
                        strAutoBackupBA = ASetings[21].ToString().Trim();//自动备份基础年份,间隔时间为多少天
                        strAutoBackupBADayValue = ASetings[22].ToString().Trim();//多少天
                        strAutoBackupGA = ASetings[23].ToString().Trim();//自动备份基础年份,间隔时间为多少天
                        strAutoBackupGADayValue = ASetings[24].ToString().Trim();//多少天
                        strAutoBackupCNo = ASetings[25].ToString().Trim();//要备份的来访年份编号
                        strAutoBackupCPath = myTable.Rows[0]["BPath"].ToString().Trim();//要备份的来访年份所存放的路径
                        strManyCard = "0";
                        strBarrierGate = ASetings[26].ToString().Trim();//道闸开启后,延迟多少秒后自动回落
                        strBarrierGateSecond = ASetings[27].ToString().Trim();//道闸开启后,延迟多少秒后自动回落
                        //ASetings[28]禁用本机USB口

                        strAutoTimeBackupBA = ASetings[29].ToString().Trim();//每天定时备份基础年份,时间为
                        strAutoTimeBackupBA_Hour = ASetings[30].ToString().Trim();//小时 
                        strAutoTimeBackupBA_Minute = ASetings[31].ToString().Trim();//分钟 

                        strAutoTimeBackupGA = ASetings[32].ToString().Trim();//每天定时备份基础年份,时间为
                        strAutoTimeBackupGA_Hour = ASetings[33].ToString().Trim();//小时 
                        strAutoTimeBackupGA_Minute = ASetings[34].ToString().Trim();//分钟 
                        strAutoTimeBackupGACNo = ASetings[35].ToString().Trim();//要定时备份的来访年份编号
                        strQPrintReport = ASetings[36].ToString().Trim();//快速登记时，允许打印出入证
                        strBPRegister = ASetings[37].ToString().Trim();//登记时,允许发现黑名单人员后能继续登记
                        strVisitorIsOrder = ASetings[38].ToString().Trim();//登记时,允许开启访客预约功能
                        strVisitorIsAffirm = ASetings[39].ToString().Trim();//登记时,允许开启访客查询功能
                        strTemParkingCard = ASetings[40].ToString().Trim();//登记时,允许开启停车证号功能
                        strLeftLuggage = ASetings[41].ToString().Trim();//登记时,允许开启物品寄存柜功能
                        strTemDoorControl = ASetings[42].ToString().Trim();//登记时,允许开启临时门禁卡号功能
                        strClearDB = ASetings[43].ToString().Trim();//每个月1号自动整理基础年份和来访年份
                        strVisitorIsNull = ASetings[44].ToString().Trim();//登记时，允许不录入来访人信息
                        strAllowedSecondWay = ASetings[45].ToString().Trim();//登记时，允许开启二道岗亭功能
                        strAllowedFDepSStaff = ASetings[46].ToString().Trim();//登记时，允许先选择被访部门后选择被访人员
                        strAllowedApplyForJob = ASetings[47].ToString().Trim();//登记时,允许开启应聘招工人员登记功能
                        strAllowedInfrastructure = ASetings[48].ToString().Trim();//登记时,允许开启基建施工人员登记功能
                        strAbnormalLeaved = ASetings[49].ToString().Trim();//登记时,若访客非正常签离次数达到多少次则提醒
                        strAbnormalLeavedValue = ASetings[50].ToString().Trim();//次数
                        strAllowedMultiVisitorReg = ASetings[51].ToString().Trim();//登记时,允许开启同一批次多人来访登记功能
                        strAllowedSameIDRepeatedReg = ASetings[52].ToString().Trim();//登记时,允许开启同张证件重复登记功能
                        strAllowedMultiVisitorReg = "1";
                        strAllowedOnlyDep = ASetings[53].ToString().Trim();//登记时,被访对象窗体里只允许选择被访部门
                        strAllowedVisitorFingerprint = ASetings[54].ToString().Trim();//登记时,允许开启访客指纹登记功能
                        strAllowedSoundRecord = ASetings[55].ToString().Trim();//登记时,允许开启现场录音功能,每个录音时长
                        strAllowedSoundRecord_Minute = ASetings[56].ToString().Trim();// 每个录音时长多少分钟
                        strAllowedWEvaluatingDevice = ASetings[57].ToString().Trim();//登记时,允许开启窗口评价器功能
                        strAllowedIDCardValidedVerification = ASetings[58].ToString().Trim();//登记时,允许开启访客证件有效期功能
                        strAutoAllLeaveTime = ASetings[59].ToString().Trim();//每天定时签离基础年份,时间为
                        strAutoAllLeave_Hour = ASetings[60].ToString().Trim();//小时 
                        strAutoAllLeave_Minute = ASetings[61].ToString().Trim();//分钟 
                        strAllowedRefresh = ASetings[62].ToString().Trim();//登记时，允许读取完身份证信息后直接登记打印
                        #region
                        if (strAllowedRefresh == "" || strAllowedRefresh == null || strAllowedRefresh == "0")
                        {
                            strAllowedRefresh = "0";
                        }
                        else
                        {
                            strAllowedRefresh = "1";
                        }
                        if (strAutoAllLeaveTime == "" || strAutoAllLeaveTime == null || strAutoAllLeaveTime == "0")
                        {
                            strAutoAllLeaveTime = "0";
                        }
                        else
                        {
                            strAutoAllLeaveTime = "1";
                        }
                        if (strAutoAllLeave_Hour == "" || strAutoAllLeave_Hour == null)
                        {
                            strAutoAllLeave_Hour = "8";
                        }
                        if (strAutoAllLeave_Minute == "" || strAutoAllLeave_Minute == null)
                        {
                            strAutoAllLeave_Minute = "0";
                        }
                        if (strPrintReport == "" || strPrintReport == null || strPrintReport == "0")
                        {
                            strPrintReport = "0";
                        }
                        else
                        {
                            strPrintReport = "1";
                        }
                        if (strAutoRecognition == "" || strAutoRecognition == null || strAutoRecognition == "0")
                        {
                            strAutoRecognition = "0";
                        }
                        else
                        {
                            strAutoRecognition = "1";
                        }
                        if (strLastVisitInf == "" || strLastVisitInf == null || strLastVisitInf == "0")
                        {
                            strLastVisitInf = "0";
                        }
                        else
                        {
                            strLastVisitInf = "1";
                        }

                        if (strDisplayQuantity == "" || strDisplayQuantity == null)
                        {
                            strDisplayQuantity = " All ";
                        }
                        if (strDisplayQuantityValue == "" || strDisplayQuantityValue == null)
                        {
                            strDisplayQuantityValue = " 100 ";
                        }
                        if (strDisplayLastSix == "" || strDisplayLastSix == null || strDisplayLastSix == "0")
                        {
                            strDisplayLastSix = "0";
                        }
                        else
                        {
                            strDisplayLastSix = "1";
                        }
                        if (strManyCard == "" || strManyCard == null || strManyCard == "0")
                        {
                            strManyCard = "0";
                        }
                        else
                        {
                            strManyCard = "1";
                        }

                        if (strImmediatePrint == "" || strImmediatePrint == null || strImmediatePrint == "0")
                        {
                            strImmediatePrint = "0";
                        }
                        else
                        {
                            strImmediatePrint = "1";
                        }

                        if (strPrintTemCardReport == "" || strPrintTemCardReport == null || strPrintTemCardReport == "0")
                        {
                            strPrintTemCardReport = "0";
                        }
                        else
                        {
                            strPrintTemCardReport = "1";
                        }
                        if (strDisplayStaffImage == "" || strDisplayStaffImage == null || strDisplayStaffImage == "0")
                        {
                            strDisplayStaffImage = "0";
                        }
                        else
                        {
                            strDisplayStaffImage = "1";
                        }
                        if (strIdNoLeave == "" || strIdNoLeave == null || strIdNoLeave == "0")
                        {
                            strIdNoLeave = "0";
                        }
                        else
                        {
                            strIdNoLeave = "1";
                        }
                        if (strBarcodeAffirm == "" || strBarcodeAffirm == null || strBarcodeAffirm == "0")
                        {
                            strBarcodeAffirm = "0";
                        }
                        else
                        {
                            strBarcodeAffirm = "1";
                        }
                        if (strSWindows == "" || strSWindows == null || strSWindows == "0")
                        {
                            strSWindows = "0";
                        }
                        else
                        {
                            strSWindows = "1";
                        }
                        if (strAutoBackupBA == "" || strAutoBackupBA == null || strAutoBackupBA == "0")
                        {
                            strAutoBackupBA = "0";
                        }
                        else
                        {
                            strAutoBackupBA = "1";
                        }
                        if (strAutoBackupBADayValue == "" || strAutoBackupBADayValue == null || strAutoBackupBADayValue == "0")
                        {
                            strAutoBackupBADayValue = "1";
                        }

                        if (strAutoBackupGA == "" || strAutoBackupGA == null || strAutoBackupGA == "0")
                        {
                            strAutoBackupGA = "0";
                        }
                        else
                        {
                            strAutoBackupGA = "1";
                        }
                        if (strAutoBackupGADayValue == "" || strAutoBackupGADayValue == null || strAutoBackupGADayValue == "0")
                        {
                            strAutoBackupGADayValue = "1";
                        }

                        if (strAutoBackupCNo == "" || strAutoBackupCNo == null || strAutoBackupCNo == "0")
                        {
                            strAutoBackupCNo = "";
                        }
                        if (strAutoBackupCPath == "" || strAutoBackupCPath == null || strAutoBackupCPath == "0")
                        {
                            strAutoBackupCPath = @"E:\";
                        }
                        if (strBarrierGate == "" || strBarrierGate == null || strBarrierGate == "0")
                        {
                            strBarrierGate = "1";
                        }
                        if (strBarrierGateSecond == "" || strBarrierGateSecond == null || strBarrierGateSecond == "0")
                        {
                            strBarrierGateSecond = "5";
                        }

                        if (strAutoTimeBackupBA == "" || strAutoTimeBackupBA == null || strAutoTimeBackupBA == "0")
                        {
                            strAutoTimeBackupBA = "0";
                        }
                        else
                        {
                            strAutoTimeBackupBA = "1";
                        }

                        if (strAutoTimeBackupBA_Hour == "" || strAutoTimeBackupBA_Hour == null)
                        {
                            strAutoTimeBackupBA_Hour = "18";
                        }


                        if (strAutoTimeBackupBA_Minute == "" || strAutoTimeBackupBA_Minute == null)
                        {
                            strAutoTimeBackupBA_Minute = "0";
                        }



                        if (strAutoTimeBackupGA == "" || strAutoTimeBackupGA == null || strAutoTimeBackupGA == "0")
                        {
                            strAutoTimeBackupGA = "0";
                        }
                        else
                        {
                            strAutoTimeBackupGA = "1";
                        }

                        if (strAutoTimeBackupGA_Hour == "" || strAutoTimeBackupGA_Hour == null)
                        {
                            strAutoTimeBackupGA_Hour = "18";
                        }


                        if (strAutoTimeBackupGA_Minute == "" || strAutoTimeBackupGA_Minute == null)
                        {
                            strAutoTimeBackupGA_Minute = "0";
                        }
                        if (strQPrintReport == "" || strQPrintReport == null || strQPrintReport == "0")
                        {
                            strQPrintReport = "0";
                        }
                        else
                        {
                            strQPrintReport = "1";
                        }
                        if (strBPRegister == "" || strBPRegister == null || strBPRegister == "0")
                        {
                            strBPRegister = "0";
                        }
                        else
                        {
                            strBPRegister = "1";
                        }
                        if (strVisitorIsOrder == "" || strVisitorIsOrder == null || strVisitorIsOrder == "0")
                        {
                            strVisitorIsOrder = "0";
                        }
                        else
                        {
                            strVisitorIsOrder = "1";
                        }
                        if (strVisitorIsAffirm == "" || strVisitorIsAffirm == null || strVisitorIsAffirm == "0")
                        {
                            strVisitorIsAffirm = "0";
                        }
                        else
                        {
                            strVisitorIsAffirm = "1";
                        }
                        if (strTemParkingCard == "" || strTemParkingCard == null || strTemParkingCard == "0")
                        {
                            strTemParkingCard = "0";
                        }
                        else
                        {
                            strTemParkingCard = "1";
                        }
                        if (strLeftLuggage == "" || strLeftLuggage == null || strLeftLuggage == "0")
                        {
                            strLeftLuggage = "0";
                        }
                        else
                        {
                            strLeftLuggage = "1";
                        }
                        if (strTemDoorControl == "" || strTemDoorControl == null || strTemDoorControl == "0")
                        {
                            strTemDoorControl = "0";
                        }
                        else
                        {
                            strTemDoorControl = "1";
                        }
                        if (strClearDB == "" || strClearDB == null)
                        {
                            strClearDB = "1";
                        }
                        if (strVisitorIsNull == "" || strVisitorIsNull == null)
                        {
                            strVisitorIsNull = "0";
                        }
                        if (strAllowedSecondWay == "" || strAllowedSecondWay == null)
                        {
                            strAllowedSecondWay = "0";
                        }
                        if (strAllowedFDepSStaff == "" || strAllowedFDepSStaff == null)
                        {
                            strAllowedFDepSStaff = "0";
                        }
                        if (strAbnormalLeaved == "" || strAbnormalLeaved == null)
                        {
                            strAbnormalLeaved = "1";
                        }
                        if (strAbnormalLeavedValue == "" || strAbnormalLeavedValue == null)
                        {
                            strAbnormalLeavedValue = "3";
                        }
                        if (strAllowedApplyForJob == "" || strAllowedApplyForJob == null || strAllowedApplyForJob == "0")
                        {
                            strAllowedApplyForJob = "0";
                        }
                        else
                        {
                            strAllowedApplyForJob = "1";
                        }
                        if (strAllowedInfrastructure == "" || strAllowedInfrastructure == null || strAllowedInfrastructure == "0")
                        {
                            strAllowedInfrastructure = "0";
                        }
                        else
                        {
                            strAllowedInfrastructure = "1";
                        }
                        if (strAllowedMultiVisitorReg == "" || strAllowedMultiVisitorReg == null || strAllowedMultiVisitorReg == "0")
                        {
                            strAllowedMultiVisitorReg = "0";
                        }
                        else
                        {
                            strAllowedMultiVisitorReg = "1";
                        }
                        if (strAllowedSameIDRepeatedReg == "" || strAllowedSameIDRepeatedReg == null || strAllowedSameIDRepeatedReg == "0")
                        {
                            strAllowedSameIDRepeatedReg = "0";
                        }
                        else
                        {
                            strAllowedSameIDRepeatedReg = "1";
                        }
                        if (strAllowedOnlyDep == "" || strAllowedOnlyDep == null || strAllowedOnlyDep == "0")
                        {
                            strAllowedOnlyDep = "0";
                        }
                        else
                        {
                            strAllowedOnlyDep = "1";
                        }
                        if (strAllowedVisitorFingerprint == "" || strAllowedVisitorFingerprint == null || strAllowedVisitorFingerprint == "0")
                        {
                            strAllowedVisitorFingerprint = "0";
                        }
                        else
                        {
                            strAllowedVisitorFingerprint = "1";
                        }
                        if (strAllowedSoundRecord == "" || strAllowedSoundRecord == null || strAllowedSoundRecord == "0")
                        {
                            strAllowedSoundRecord = "0";
                        }
                        else
                        {
                            strAllowedSoundRecord = "1";
                        }
                        if (strAllowedSoundRecord == "1")
                        {
                            if (strAllowedSoundRecordPath == "")
                            {
                                try
                                {
                                    if (Directory.Exists(@"E:\SoundRecord") == false)//判断此文件夹是否存在
                                    {
                                        Directory.CreateDirectory(@"E:\SoundRecord");
                                        strAllowedSoundRecordPath = @"E:\SoundRecord";
                                    }
                                }
                                catch
                                {
                                    if (Directory.Exists(Application.StartupPath.Trim() + "\\SoundRecord") == false)//判断此文件夹是否存在
                                    {
                                        Directory.CreateDirectory(Application.StartupPath.Trim() + "\\SoundRecord");
                                        strAllowedSoundRecordPath = Application.StartupPath.Trim() + "\\SoundRecord";
                                    }
                                }
                            }
                        }
                        if (strAllowedSoundRecord_Minute == "" || strAllowedSoundRecord_Minute == null)
                        {
                            strAutoTimeBackupBA_Hour = "30";
                        }
                        if (strAllowedWEvaluatingDevice == "" || strAllowedWEvaluatingDevice == null || strAllowedWEvaluatingDevice == "0")
                        {
                            strAllowedWEvaluatingDevice = "0";
                        }
                        else
                        {
                            strAllowedWEvaluatingDevice = "1";
                        }
                        if (strAllowedIDCardValidedVerification == "" || strAllowedIDCardValidedVerification == null || strAllowedIDCardValidedVerification == "0")
                        {
                            strAllowedIDCardValidedVerification = "0";
                        }
                        else
                        {
                            strAllowedIDCardValidedVerification = "1";
                        }
                        #endregion

                        //strSQL = "select * from XXCLOUD.dbo.T_BaseCategroyInf where Flag='" + "本软件使用单位" + "'";
                        //myTable = SQLHelper.DTQuery(strSQL);
                        //if (myTable.Rows.Count > 0)
                        //{
                        //    MainBFrm.strEndUserName = myTable.Rows[0]["BClassName"].ToString();
                        //}
                        //else
                        //{
                        //    MainBFrm.strEndUserName = "";
                        //}
                        for (int i = 0; i < ASetings.Length; i++)
                        {
                            ASetings[i] = "";
                        }
                        iCount = 0;
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

                        strAllowedAllLeaveNormal = ASetings[0].ToString().Trim();//每天自动定时签离全部未签离来访记录并记为正常签离,时间为
                        strAllowedAllLeaveNormal_Hour = ASetings[1].ToString().Trim();
                        strAllowedAllLeaveNormal_Minute = ASetings[2].ToString().Trim();

                        strAllowedAllLeaveAbNormal = ASetings[3].ToString().Trim();//每天自动定时签离全部未签离来访记录并记为1次非正常签离,时间为
                        strAllowedAllLeaveAbNormal_Hour = ASetings[4].ToString().Trim();
                        strAllowedAllLeaveAbNormal_Minute = ASetings[5].ToString().Trim();

                        strAllowedLogoutRedCardNormal = ASetings[6].ToString().Trim();//每天自动定时从门禁控制器上注销所有未注销的红卡权限并记为正常注销,时间为
                        strAllowedLogoutRedCardNormal_Hour = ASetings[7].ToString().Trim();
                        strAllowedLogoutRedCardNormal_Minute = ASetings[8].ToString().Trim();

                        strAllowedLogoutRedCardAbNormal = ASetings[9].ToString().Trim();//每天自动定时从门禁控制器上注销所有未注销的红卡权限并记为正常注销,时间为
                        strAllowedLogoutRedCardAbNormal_Hour = ASetings[10].ToString().Trim();
                        strAllowedLogoutRedCardAbNormal_Minute = ASetings[11].ToString().Trim();

                        strAllowedLogoutBlueCard = ASetings[12].ToString().Trim();//每天自动定时根据蓝卡有效期限从门禁控制器上注销所有未注销的蓝卡权限,时间为
                        strAllowedLogoutBlueCard_Hour = ASetings[13].ToString().Trim();
                        strAllowedLogoutBlueCard_Minute = ASetings[14].ToString().Trim();

                        strAllowedOverdueOrder = ASetings[15].ToString().Trim();//每天自动定时根据预约有效期限把过期预约设为异常预约,时间为
                        strAllowedOverdueOrder_Hour = ASetings[16].ToString().Trim();
                        strAllowedOverdueOrder_Minute = ASetings[17].ToString().Trim();


                        //                    public static string strAllowedOpenDoorDelay = "1";//是否开启
                        //public static string strAllowedOpenDoorDelaySecond = "3";//临时开门的时长，单位：秒
                        //public static string strAllowedEnterAndLeaveCount = "1";//是否开启
                        //public static string strAllowedEnterCount = "1";//门禁卡片在有效时间段内只允许刷卡进入1次
                        //public static string strAllowedLeaveCount = "1";//门禁卡片在有效时间段内只允许刷开离开1次

                        strAllowedOpenDoorDelay = ASetings[18].ToString().Trim();//是否开启
                        strAllowedOpenDoorDelaySecond = ASetings[19].ToString().Trim();//临时开门的时长，单位：秒
                        strAllowedEnterAndLeaveCount = ASetings[20].ToString().Trim();//是否开启
                        strAllowedEnterCount = ASetings[21].ToString().Trim();//门禁卡片在有效时间段内只允许刷卡进入1次
                        strAllowedLeaveCount = ASetings[22].ToString().Trim();//门禁卡片在有效时间段内只允许刷开离开1次

                        strAllowedOpenMSCat = ASetings[23].ToString().Trim();

                        strAllowedVIPOpenDoorDelay = ASetings[24].ToString().Trim();//VIP卡功能是否开启
                        strAllowedVIPOpenDoorDelaySecond = ASetings[25].ToString().Trim();//临时开门的时长，单位：秒




                        #region//判断是否启动自动备份基础年份
                        //try
                        //{
                        //    if (strAutoBackupBA == "1")
                        //    {
                        //        if (myTable.Rows[0]["BBMonth"].ToString().Trim() == "" || myTable.Rows[0]["BBMonth"].ToString().Trim() == null)
                        //        {
                        //            MessageBox.Show("系统检测到每月的今天作为自动备份【基础年份】的日期，点击【确定】后便开始自动备份，备份时间视年份大小，请您稍后.....", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //            if (myTable.Rows[0]["BPath"].ToString().Trim() != "" && myTable.Rows[0]["BPath"].ToString().Trim() != null)
                        //            {
                        //                strSQL = "use master;backup database XXCLOUD to disk ='" + myTable.Rows[0]["BPath"].ToString().Trim() + "XXCLOUD" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8) + "' with init";//  
                        //            }
                        //            else
                        //            {
                        //                strSQL = "use master;backup database XXCLOUD to disk ='" + @"E:\XXCLOUD" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8) + "' with init";//  
                        //            }
                        //            if (SQLHelper.ExecuteSql(strSQL) != 0)
                        //            {
                        //                strSQL = "update XXCLOUD.dbo.T_YearInf set BBMonth='" + System.DateTime.Now.AddMonths(1).Month.ToString() + "' ";
                        //                strSQL += " where ANo='" + myTable.Rows[0]["ANo"].ToString().Trim() + "'";
                        //                if (SQLHelper.ExecuteSql(strSQL) != 0)
                        //                {
                        //                    if (myTable.Rows[0]["BPath"].ToString().Trim() != "" && myTable.Rows[0]["BPath"].ToString().Trim() != null)
                        //                    {
                        //                        MessageBox.Show("基础年份备份成功!备份路径为：" + myTable.Rows[0]["BPath"].ToString().Trim() + "XXCLOUD" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //                    }
                        //                    else
                        //                    {
                        //                        MessageBox.Show("基础年份备份成功!备份路径为: E:\\XXCLOUD" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //                    }
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (Convert.ToInt32(myTable.Rows[0]["BBMonth"].ToString().Trim()) == Convert.ToInt32(System.DateTime.Now.Month))
                        //            {
                        //                if (Convert.ToInt32(strAutoBackupBADayValue) == Convert.ToInt32(System.DateTime.Now.Day))
                        //                {
                        //                    MessageBox.Show("系统检测到每月的今天作为自动备份【基础年份】的日期，点击【确定】后便开始自动备份，备份时间视年份大小，请您稍后.....", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //                    if (myTable.Rows[0]["BPath"].ToString().Trim() != "" && myTable.Rows[0]["BPath"].ToString().Trim() != null)
                        //                    {
                        //                        strSQL = "use master;backup database XXCLOUD to disk ='" + myTable.Rows[0]["BPath"].ToString().Trim() + "XXCLOUD" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8) + "' with init";//  
                        //                    }
                        //                    else
                        //                    {
                        //                        strSQL = "use master;backup database XXCLOUD to disk ='" + @"E:\XXCLOUD" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8) + "' with init";//  
                        //                    }
                        //                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                        //                    {
                        //                        strSQL = "update XXCLOUD.dbo.T_YearInf set BBMonth='" + System.DateTime.Now.AddMonths(1).Month.ToString() + "' ";
                        //                        strSQL += " where ANo='" + myTable.Rows[0]["ANo"].ToString().Trim() + "'";
                        //                        if (SQLHelper.ExecuteSql(strSQL) != 0)
                        //                        {
                        //                            if (myTable.Rows[0]["BPath"].ToString().Trim() != "" && myTable.Rows[0]["BPath"].ToString().Trim() != null)
                        //                            {
                        //                                MessageBox.Show("基础年份备份成功!备份路径为：" + myTable.Rows[0]["BPath"].ToString().Trim() + "XXCLOUD" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //                            }
                        //                            else
                        //                            {
                        //                                MessageBox.Show("基础年份备份成功!备份路径为: E:\\XXCLOUD" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        //catch
                        //{
                        //    MessageBox.Show("请确认要备份的数据库是否存在或备份路径是否正确!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //}
                        #endregion

                        #region//判断是否启动自动备份来访年份
                        //try
                        //{
                        //    if (strAutoBackupGA == "1")
                        //    {
                        //        if (myTable.Rows[0]["BGMonth"].ToString().Trim() == "" || myTable.Rows[0]["BGMonth"].ToString().Trim() == null)
                        //        {
                        //            MessageBox.Show("系统检测到每月的今天作为自动备份【来访年份】的日期，点击【确定】后便开始自动备份，备份时间视年份大小，请您稍后.....", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //            if (myTable.Rows[0]["BPath"].ToString().Trim() != "" && myTable.Rows[0]["BPath"].ToString().Trim() != null)
                        //            {
                        //                strSQL = "use master;backup database YEAR" + strAutoBackupCNo + " to disk ='" + myTable.Rows[0]["BPath"].ToString().Trim() + "YEAR" + strAutoBackupCNo + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8) + "' with init";//  
                        //            }
                        //            else
                        //            {
                        //                strSQL = "use master;backup database YEAR" + strAutoBackupCNo + "  to disk ='" + @"E:\YEAR" + strAutoBackupCNo + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8) + "' with init";//  
                        //            }
                        //            if (SQLHelper.ExecuteSql(strSQL) != 0)
                        //            {
                        //                strSQL = "update XXCLOUD.dbo.T_YearInf set BGMonth='" + System.DateTime.Now.AddMonths(1).Month.ToString() + "' ";
                        //                strSQL += " where ANo='" + myTable.Rows[0]["ANo"].ToString().Trim() + "'";
                        //                if (SQLHelper.ExecuteSql(strSQL) != 0)
                        //                {
                        //                    if (myTable.Rows[0]["BPath"].ToString().Trim() != "" && myTable.Rows[0]["BPath"].ToString().Trim() != null)
                        //                    {
                        //                        MessageBox.Show("来访年份备份成功!备份路径为：" + myTable.Rows[0]["BPath"].ToString().Trim() + "YEAR" + strAutoBackupCNo + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //                    }
                        //                    else
                        //                    {
                        //                        MessageBox.Show("来访年份备份成功!备份路径为: E:\\YEAR" + strAutoBackupCNo + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //                    }
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (Convert.ToInt32(myTable.Rows[0]["BGMonth"].ToString().Trim()) == Convert.ToInt32(System.DateTime.Now.Month))
                        //            {
                        //                if (Convert.ToInt32(strAutoBackupBADayValue) == Convert.ToInt32(System.DateTime.Now.Day))
                        //                {
                        //                    MessageBox.Show("系统检测到每月的今天作为自动备份【来访年份】的日期，点击【确定】后便开始自动备份，备份时间视年份大小，请您稍后.....", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //                    if (myTable.Rows[0]["BPath"].ToString().Trim() != "" && myTable.Rows[0]["BPath"].ToString().Trim() != null)
                        //                    {
                        //                        strSQL = "use master;backup database YEAR" + strAutoBackupCNo + "  to disk ='" + myTable.Rows[0]["BPath"].ToString().Trim() + "YEAR" + strAutoBackupCNo + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8) + "' with init";//  
                        //                    }
                        //                    else
                        //                    {
                        //                        strSQL = "use master;backup database YEAR" + strAutoBackupCNo + "  to disk ='" + @"E:\YEAR" + strAutoBackupCNo + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8) + "' with init";//  
                        //                    }
                        //                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                        //                    {
                        //                        strSQL = "update XXCLOUD.dbo.T_YearInf set BGMonth='" + System.DateTime.Now.AddMonths(1).Month.ToString() + "' ";
                        //                        strSQL += " where ANo='" + myTable.Rows[0]["ANo"].ToString().Trim() + "'";
                        //                        if (SQLHelper.ExecuteSql(strSQL) != 0)
                        //                        {
                        //                            if (myTable.Rows[0]["BPath"].ToString().Trim() != "" && myTable.Rows[0]["BPath"].ToString().Trim() != null)
                        //                            {
                        //                                MessageBox.Show("来访年份备份成功!备份路径为：" + myTable.Rows[0]["BPath"].ToString().Trim() + "YEAR" + strAutoBackupCNo + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //                            }
                        //                            else
                        //                            {
                        //                                MessageBox.Show("来访年份备份成功!备份路径为: E:\\YEAR" + strAutoBackupCNo + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(0, 8), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        //catch
                        //{
                        //    MessageBox.Show("请确认要备份的数据库是否存在或备份路径是否正确!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //}
                        #endregion

                        strSQL = "select * from XXCLOUD.dbo.T_BaseCategroyInf where Flag='" + "本软件使用单位" + "'";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            D_RemoterControlFrm.strEndUserName = myTable.Rows[0]["BClassName"].ToString();
                            LoginFrm.strEnduserName = D_RemoterControlFrm.strEndUserName;
                        }
                        else
                        {
                            D_RemoterControlFrm.strEndUserName = "";
                            LoginFrm.strEnduserName = "";
                        }

                    }
                    else
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("请选择或新建或加入合法的年份，登录失败!\n\n***注意：如果是网络版，请确认数据库服务器已开机使用。***", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    if (strOperatorNo_Temp == "")
                    {
                        //如果是新操作登录，而记录在本地Access数据库
                        DataSet ds1 = new DataSet();
                        strSQL = "select * from T_LocOperatorInf where  OName ='" + strOperatorName + "'";
                        string strConn_1 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + strAccessPwd + "'";
                        System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn_1);
                        System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(strSQL, conn);
                        da.Fill(ds1);
                        if (ds1.Tables[0].Rows.Count <= 0)
                        {
                            strSQL = "insert into T_LocOperatorInf(ONo,OName)Values('" + strOperatorNo + "','" + strOperatorName + "')";
                            OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    bool blLJOK = false;//成功连接到所选年份
                    try
                    {
                        blLJOK = SQLHelper.DBLink(strDDIdConfirm, strSqlServer, strANo, strDLoginName, strDLoginPwd, strFlagDBLink, LoginFrm.strByInternet);
                        if (blLJOK == true)
                        {
                            string strConn1 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + LoginFrm.strAccessPwd + "'";
                            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn1);
                            if (ds_Configuration.Tables[0].Rows.Count <= 0)
                            {
                                string strServerName1 = SQLHelper.EncryptString(Dns.GetHostName().Trim() + "A1");//服务器名称
                                string strDDIdConfirm1 = SQLHelper.EncryptString("1A2");//连接方式
                                string strDLoginName1 = SQLHelper.EncryptString("saA3");//登录名
                                string strDLoginPwd1 = SQLHelper.EncryptString("123456A4");//登录密码
                                string strGuardRoomId1 = SQLHelper.EncryptString("A5");//本机所在门岗的编号
                                string strGuardRoomName1 = SQLHelper.EncryptString("A6");//本机所在门岗的名称
                                //A1:服务器名称，A2：身份验证  A3：登录名  A4：登录密码  A5:当前门岗编号  A6：当前门岗名称
                                //A7:软键盘     A8:IC卡端口号  A9：ID卡端口号  A11:清除垃圾（0：未清除 1：已清除 ）
                                strSQL = "insert into T_LocConfigurationInf(A1,A2,A3,A4,A5,A6)Values('" + strServerName1 + "','" + strDDIdConfirm1 + "', ";
                                strSQL += "'" + strDLoginName1 + "','" + strDLoginPwd1 + "','" + strGuardRoomId1 + "','" + strGuardRoomName1 + "')";
                                OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                                ds_Configuration.Tables.Clear();
                                ds_Configuration = XXCLOUDDLL.LoginFrm_LoadLocalConfigurationInf(Application.StartupPath.Trim(), strAccessPwd);
                            }
                            //数据库连接成功
                            strFWQIP = strSqlServer;
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    try
                    {
                        strSQL = "select top 1 * from YEAR" + strANo + ".DBO.T_VisitorAccessInf ";
                        DataTable dt_SS = new DataTable();
                        dt_SS = SQLHelper.DTQuery(strSQL);
                    }
                    catch
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("名称为 YEAR" + strANo + " 的年份不存在或无法连接到SQL数据库服务器，登录失败!\n\n***注意：如果是网络版，请确认数据库服务器已开机使用。***", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    this.timer_SIdRead.Stop();
                    this.timer_CardRead.Stop();
                    blCanRegist = true;//已注册
                    if (strOperatorPower.Trim() == "0")
                    {
                        strFlag_Login = "0";
                    }
                    else if (strOperatorPower.Trim() == "1")
                    {
                        strFlag_Login = "1";
                    }
                    strConn = SQLHelper.connectionString_1.Trim();
                    OperatorLog("操作员[" + this.txt_Operator.Text.Trim() + "]登录", "1");
                    this.Cursor = Cursors.Default;
                    if (this.txt_Password.Text.Trim() == "")
                    {
                        if (strCardNo == "")
                        {
                            // MessageBox.Show("您的登录密码为空，请及时建立密码以防帐号被盗用!", "安全提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    #region//网络时，自动检测两个服务和网络通信助手、网络狗服务是否开启，没有开启，则自动开启.
                    string strSHostName = Dns.GetHostName();//读取本计算机名称
                    string strSHostIP = "";
                    IPHostEntry host = Dns.GetHostEntry(strSHostName);
                    foreach (IPAddress ip in host.AddressList)
                    {
                        strSHostIP = ip.ToString();//读取本计算机IP地址
                        if (strSHostIP.Contains(":") == false)
                        {
                            break;//针对双网卡的情况，需屏蔽掉其中一个网卡
                        }
                    }
                    if (LoginFrm.strFWQIP.Trim() == strSHostIP)//判断本机是否为数据库服务器
                    {
                        //bool blStringToIpAddress = true;
                        //try
                        //{
                        //    IPAddress ip = IPAddress.Parse(LoginFrm.strSqlServer);
                        //}
                        //catch
                        //{
                        //    blStringToIpAddress = false;
                        //}
                        //MessageBox.Show(blStringToIpAddress.ToString ());
                        //if (LoginFrm.strSqlServer.Contains("192.168") == true || LoginFrm.strSqlServer.Contains("10.16") == true || LoginFrm.strSqlServer.Contains("127.0") == true)
                        //{
                        #region//开启Distributed Transaction Coordinator和NT LM Security Support Provider两个服务
                        //ServiceController service = new ServiceController();//创建服务控制对象
                        //service.ServiceName = "Distributed Transaction Coordinator";//启动Windows信史服务
                        ////判断当前服务状态
                        //if (service.Status == ServiceControllerStatus.Stopped)
                        //{
                        //    try
                        //    {
                        //        service.Start();// 启动服务
                        //        service.WaitForStatus(ServiceControllerStatus.Running);
                        //    }
                        //    catch (InvalidOperationException)
                        //    {
                        //        MessageBox.Show("系统不能自启动Distributed Transaction Coordinator服务！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    }
                        //}

                        //ServiceController service1 = new ServiceController();//创建服务控制对象
                        //service1.ServiceName = "NT LM Security Support Provider";//启动Windows信史服务
                        ////判断当前服务状态
                        //if (service1.Status == ServiceControllerStatus.Stopped)
                        //{
                        //    try
                        //    {
                        //        service1.Start();// 启动服务
                        //        service1.WaitForStatus(ServiceControllerStatus.Running);
                        //    }
                        //    catch (InvalidOperationException)
                        //    {
                        //        MessageBox.Show("系统不能自启动NT LM Security Support Provider服务！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    }
                        //}
                        #endregion
                        try
                        {
                            if (XXCLOUDDLL.LoginFrm_iNCAPromgramIsRun() >= 1)
                            {
                                //网络通信助手已经在运行
                            }
                            else
                            {
                                string strHelpFN = Application.StartupPath + "\\NCA\\网络通信助手.exe";
                                System.Diagnostics.Process.Start(strHelpFN);
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            if (XXCLOUDDLL.LoginFrm_iDogServerPromgramIsRun() >= 1)
                            {
                                //网络狗服务已经在运行
                            }
                            else
                            {
                                if (LoginFrm.iFlag_SingleToDoubleEncrption == 0)//0:正常网络版软件  1：用单机狗来当网络狗使用
                                {
                                    string strHelpFN = Application.StartupPath + "\\DogServer\\DogServer.exe";
                                    System.Diagnostics.Process.Start(strHelpFN);
                                }
                            }
                        }
                        catch
                        {

                        }

                        //}
                    }
                    #endregion
                    strSQL = "delete from XXCLOUD.dbo.T_StaffLoginInf where SNo='" + strOperatorNo + "' and  SActualNo ='" + strOperatorActualNo + "' and Flag='" + "O" + "'";
                    SQLHelper.ExecuteSql(strSQL);
                    strSQL = "insert into XXCLOUD.dbo.T_StaffLoginInf(SNo,SActualNo,SName,SHostName,SHostIP,Power,Flag)values(";
                    strSQL += "'" + strOperatorNo + "','" + strOperatorActualNo + "','" + strOperatorName + "','" + strSHostName + "','" + strSHostIP + "','" + strOperatorPower + "','" + "O" + "')";
                    SQLHelper.ExecuteSql(strSQL);

                    D_RemoterControlFrm.strT_VisitorAccessInf = "  YEAR" + LoginFrm.strCAccout.Substring(1, 4).Trim() + ".DBO.T_VisitorAccessInf";//当前表
                    D_RemoterControlFrm.strT_MJRecordAccessInf = "  YEAR" + LoginFrm.strCAccout.Substring(1, 4).Trim() + ".DBO.T_MJRecordAccessInf";//当前表
                    this.Close();
                }
                else
                {
                    this.timer_SIdRead.Stop();
                    this.Cursor = Cursors.Default;
                    if (MessageBox.Show("操作员或密码或权限错误!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        this.txt_Password.Text = "";
                        this.timer_SIdRead.Start();
                        strCAccout = this.txt_AccoutList.Text.Trim();
                    }
                    strOperatorNo = strOperatorNo_Temp;
                    strOperatorName = this.txt_Operator.Text.Trim();
                    OperatorLog("操作员[" + this.txt_Operator.Text.Trim() + "]登录", "0");
                    strOperatorNo = "";
                    strOperatorName = "";
                    return;
                }

                //保存数据库服务器名，登录名及访问时间等；
            }
            catch (Exception exp)
            {
                if (exp.ToString().Contains("T_OperatorInf") == true)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("数据库服务器中不存在基础年份，请先单击【配置】按钮，在弹出的窗体中新建年份！\n\n***注意：如果是网络版，请确认数据库服务器已开机使用。***", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            this.Cursor = Cursors.Default;
        }
 
        private void pic_Exit_Click(object sender, EventArgs e)
        {
            this.grid_AccoutList.Visible = false;
            this.grid_Operator.Visible = false;
            try
            {
                //启用本机的任务管理器
                Registry.SetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableTaskMgr", 0);//启用本机的任务管理器
                //启用当前用户的任务管理器
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableTaskMgr", 0);//启用当前用户的任务管理器
            }
            catch
            {

            }
            if (iFlag_IDReader == 1)
            {
                if (sP1.IsOpen == true)
                {
                    sP1.Close();
                }
            }
            SetAppBarAutoDisplay(false);
           
            strFlag_Login = "";
            if (LoginFrm.iFlag_IdCardReaderType == 0)
            {
                Boolean bRet;
                bRet = LoginFrm.XZX_Close(1);    // 端口号在这里修改
            }
            AnimateWindow(this.Handle, 500, AW_SLIDE + AW_VER_POSITIVE + AW_HIDE);
            // ShowWindow(FindWindow("Shell_TrayWnd", null), SW_HIDE);//隐藏任务栏

            Application.Exit();
        }

        private void pic_Configuration_Click(object sender, EventArgs e)
        {
            this.grid_AccoutList.Visible = false;
            this.grid_Operator.Visible = false;
            this.timer_SIdRead.Stop();
            this.timer_CardRead.Stop();
            LoginFrm_Configuration newFrm = new LoginFrm_Configuration();
            newFrm.ShowDialog();
            if (newFrm.strZT == "0")
            {
                this.timer_SIdRead.Start();
                this.timer_CardRead.Start();
                return;
            }
            if (newFrm.strZT == "1")
            {
                strSqlServer = newFrm.strServerName;
                strDDIdConfirm = newFrm.strDDIdConfirm;
                strDLoginName = newFrm.strDLoginName;
                if (newFrm.strDLoginPwd.Trim() != "")
                {
                    strDLoginPwd = SQLHelper.DecryptString(newFrm.strDLoginPwd.Trim());
                    strDLoginPwd = strDLoginPwd.Substring(0, strDLoginPwd.Length - 2);
                }
                else
                {
                    strDLoginPwd = "";
                }
                ds_Configuration.Tables.Clear();
                ds_Configuration = XXCLOUDDLL.LoginFrm_LoadLocalConfigurationInf(Application.StartupPath.Trim(), strAccessPwd);
            }
            else
            {
                dt_Operator.Rows.Clear();
                try
                {

                    LoadAccoutList();
                    LoadOperatorList();
                    ds_Configuration.Tables.Clear();
                    ds_Configuration = XXCLOUDDLL.LoginFrm_LoadLocalConfigurationInf(Application.StartupPath.Trim(), strAccessPwd);
                    if (ds_Configuration.Tables[0].Rows.Count > 0)
                    {
                        if (ds_Configuration.Tables[0].Rows[0]["A1"].ToString().Trim() != "" && ds_Configuration.Tables[0].Rows[0]["A1"].ToString().Trim() != null)
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A1"].ToString().Trim());
                            strSqlServer = strSQL.Substring(0, strSQL.Length - 2);
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A2"].ToString().Trim() != "" && ds_Configuration.Tables[0].Rows[0]["A2"].ToString().Trim() != null)
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A2"].ToString().Trim());
                            strDDIdConfirm = strSQL.Substring(0, strSQL.Length - 2);
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A3"].ToString().Trim() != "" && ds_Configuration.Tables[0].Rows[0]["A3"].ToString().Trim() != null)
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A3"].ToString().Trim());
                            strDLoginName = strSQL.Substring(0, strSQL.Length - 2);
                        }
                        if (ds_Configuration.Tables[0].Rows[0]["A4"].ToString().Trim() != "" && ds_Configuration.Tables[0].Rows[0]["A4"].ToString().Trim() != null)
                        {
                            strSQL = SQLHelper.DecryptString(ds_Configuration.Tables[0].Rows[0]["A4"].ToString().Trim());
                            strSQL = strSQL.Substring(0, strSQL.Length - 2);
                        }
                        if (strSQL != "A4")
                        {
                            strDLoginPwd = strSQL;
                        }
                        else
                        {
                            strDLoginPwd = "";
                        }
                    }
                    else
                    {
                        strSqlServer = Dns.GetHostName();
                        strDDIdConfirm = "1";
                        strDLoginName = "sa";
                        strDLoginPwd = "123456";
                    }
                }
                catch (Exception exp)
                {
                    this.timer_SIdRead.Start();
                    this.timer_CardRead.Start();
                    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.timer_SIdRead.Start();
                this.timer_CardRead.Start();
            }
        }

        private void pic_Help_Click(object sender, EventArgs e)
        {
            try
            {
                this.grid_AccoutList.Visible = false;
                this.grid_Operator.Visible = false;
                LoginFrm_Help newFrm = new LoginFrm_Help();
                newFrm.ShowDialog();
            }
            catch
            {
                MessageBox.Show("对不起，找不到相应的帮助文件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void picbox_ClearRubbish_Click(object sender, EventArgs e)
        {
            try
            {
                this.grid_AccoutList.Visible = false;
                this.grid_Operator.Visible = false;
                if (MessageBox.Show("定期清除系统垃圾文件有助于提高设备运行速度，确定需要清除系统垃圾文件吗?   清除周期建议为一个星期一次!", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string strClearRubbish = Application.StartupPath + "//ClearRubbish.bat";
                    System.Diagnostics.Process.Start(strClearRubbish);
                }
            }
            catch
            {
                MessageBox.Show("对不起，找不到相应的清除文件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txt_Password_Enter(object sender, EventArgs e)
        {
            this.grid_AccoutList.Visible = false;
            this.grid_Operator.Visible = false;
            this.txt_Password.BackColor = Color.Wheat;
        }

        private void txt_Password_Leave(object sender, EventArgs e)
        {
            this.txt_Password.BackColor = Color.White;
        }

        private void txt_Password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                pic_Enter_Click(sender, e);
            }
        }

 
        private void txt_AccoutList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                this.txt_Operator.Focus();
            }
        }

        private void txt_Operator_KeyPress(object sender, KeyPressEventArgs e)
        {
            strOperatorNo = "";
            strOperatorName = "";
            strOperatorActualNo = "";
            if (e.KeyChar == 13)
            {
                this.txt_Password.Focus();
            }
            
        }

        private void picbox_AccoutList_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                this.picbox_AccoutList.Image = XXY_VisitorMJAsst.Properties.Resources._2;
            }
            catch
            {

            }
        }

        private void picbox_AccoutList_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                this.picbox_AccoutList.Image = XXY_VisitorMJAsst.Properties.Resources._1;
            }
            catch
            {

            }
        }

        private void picbox_Operator_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                this.picbox_Operator.Image = XXY_VisitorMJAsst.Properties.Resources._2;
            }
            catch
            {

            }
        }

        private void picbox_Operator_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                this.picbox_Operator.Image = XXY_VisitorMJAsst.Properties.Resources._1;
            }
            catch
            {

            }
        }

        private void picbox_AccoutList_Click(object sender, EventArgs e)
        {
             this.grid_Operator.Visible = false;
            if (this.grid_AccoutList.Visible == false)
            {
                this.grid_AccoutList.Visible = true;
            }
            else
            {
                this.grid_AccoutList.Visible = false;
            }
           // this.txt_AccoutList.Focus();
        }

        private void picbox_Operator_Click(object sender, EventArgs e)
        {
            this.grid_AccoutList.Visible = false;
            if (this.grid_Operator.Visible == false)
            {
                this.grid_Operator.Visible = true;
            }
            else
            {
                this.grid_Operator.Visible = false;
            }
           // this.txt_Operator.Focus();
        }

        private void grid_AccoutList_Click(object Sender, EventArgs e)
        {
            if (this.grid_AccoutList.ActiveCell.Col != 1)
            {
                string strSQL = "确认删除名称为 " + this.grid_AccoutList.Cell(this.grid_AccoutList.ActiveCell.Row, 1).Text.Trim() + " 的登录年份吗?";
                if (MessageBox.Show(strSQL, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (XXCLOUDDLL.A_AccAndOperatorFrm_DeleteAccoutInf_Loc(this.grid_AccoutList.Cell(this.grid_AccoutList.ActiveCell.Row, 1).Tag.Trim(), LoginFrm.strAccessPwd, "", false) == "1")
                    {
                        this.grid_AccoutList.Row(this.grid_AccoutList.ActiveCell.Row).Delete();
                        this.grid_AccoutList.Visible = false;
                         LoadAccoutList();
                    }
   
                }
            }
            else
            {
                this.txt_AccoutList.Text = this.grid_AccoutList.Cell(this.grid_AccoutList.ActiveCell.Row, this.grid_AccoutList.ActiveCell.Col).Text.Trim();
                this.grid_AccoutList.Visible = false;
            }
            if (this.txt_AccoutList.Text.Trim() == "")
            {
                this.txt_AccoutList.Focus();
            }
            else if (this.txt_Operator.Text.Trim() == "")
            {
                this.txt_Operator.Focus();
            }
            else
            {
                this.txt_Password.Focus();
            }
        }

        private void grid_Operator_Click(object Sender, EventArgs e)
        {
            if (this.grid_Operator.ActiveCell.Col != 1)
            {
                string strSQL = "确认删除姓名为 " + this.grid_Operator.Cell(this.grid_Operator.ActiveCell.Row, 1).Text.Trim() + " 的操作员吗?";
                if (MessageBox.Show(strSQL, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (XXCLOUDDLL.A_AccAndOperatorFrm_DeleteOperatorInf_Loc(this.grid_Operator.Cell(this.grid_Operator.ActiveCell.Row, 1).Tag.Trim(), LoginFrm.strAccessPwd, "", false) == "1")
                    {
                        this.grid_Operator.Row(this.grid_Operator.ActiveCell.Row).Delete();
                        this.grid_Operator.Visible = false;
                        LoadOperatorList();
                        strOperatorNo = "";
                        strOperatorName = "";
                        strOperatorActualNo = "";
                    }

                }
            }
            else
            {
                this.txt_Operator.Text = this.grid_Operator.Cell(this.grid_Operator.ActiveCell.Row, this.grid_Operator.ActiveCell.Col).Text.Trim();
                strOperatorNo = this.grid_Operator.Cell(this.grid_Operator.ActiveCell.Row, this.grid_Operator.ActiveCell.Col).Tag.Trim();
                this.grid_Operator.Visible = false;
            }
            if (this.txt_AccoutList.Text.Trim() == "")
            {
                this.txt_AccoutList.Focus();
            }
            else if (this.txt_Operator.Text.Trim() == "")
            {
                this.txt_Operator.Focus();
            }
            else
            {
                this.txt_Password.Focus();
            }
        }
   
        private void LoginFrm_Click(object sender, EventArgs e)
        {
            this.grid_AccoutList.Visible = false;
            this.grid_Operator.Visible = false;

         
        }

        private void txt_AccoutList_Enter(object sender, EventArgs e)
        {
            //this.txt_AccoutList.BackColor = Color.Wheat;
            this.grid_AccoutList.Visible = false;
        }

        private void txt_AccoutList_Leave(object sender, EventArgs e)
        {
            this.txt_AccoutList.BackColor = Color.White;
        }

        private void txt_Operator_Enter(object sender, EventArgs e)
        {
            //this.txt_Operator.BackColor = Color.Wheat;
            this.grid_Operator.Visible = false;
        }

        private void txt_Operator_Leave(object sender, EventArgs e)
        {
            this.txt_Operator.BackColor = Color.White;
        }

        private void txt_AccoutList_Click(object sender, EventArgs e)
        {
            this.grid_AccoutList.Visible = false;
        }

        private void txt_Operator_Click(object sender, EventArgs e)
        {
            this.grid_Operator.Visible = false;
        }

        private void timer_CardRead_Tick(object sender, EventArgs e)
        {

        }

        private void timer_SIdRead_Tick(object sender, EventArgs e)
        {

        }




    }
}

