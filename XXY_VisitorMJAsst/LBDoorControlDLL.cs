using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace XXY_VisitorMJAsst
{
    class Add
    {
        [DllImport("testQTDll.dll", EntryPoint = "add2")]
        public static extern int add(int a);
    }

    public class LBDoorControlDLL
    {
        public class AccessV2
        {
            private UInt32 m_CurrentDevice = 0;     //当前设备
            public struct SYSTEMTIME
            {
                public UInt16 wYear;
                public UInt16 wMonth;
                public UInt16 wDayOfWeek;
                public UInt16 wDay;
                public UInt16 wHour;
                public UInt16 wMinute;
                public UInt16 wSecond;
                public UInt16 wMilliseconds;
            }

            //设备网络信息
            public struct NETWORK
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string DevAddr;    //设备IP地址
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string GateWay;      //网关
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string NetMask;      //掩码
                public UInt32 DevPort;    //设备端口
            }

            //搜索设备
            public struct NET_SEARCH
            {
                public UInt32 DevSN;        //设备序列号
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string DevAddr;    //设备IP地址
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string GateWay;      //网关
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string NetMask;      //掩码
                public UInt32 DevPort;    //设备端口
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string DevMac;       //设备mac地址
            }
            //设备配置
            public struct SETTING
            {
                public SYSTEMTIME DateTime;			//日期
                public UInt32 LogCount; 				//记录个数
                public UInt32 RegCount; 				//注册权限个数
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public UInt32[] DoorState; 			//门状态
                public UInt32 Version; 			    //硬件版本号
                public UInt32 RightType; 				//权限类型
                public UInt32 Restrict; 				//互锁参数
                public UInt32 AntiReturn; 				//反潜回参数
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
                public string StressCodeA;			    //胁迫密码A
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
                public string StressCodeB;			    //胁迫密码B
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string LocalAddr; 			    //设备IP
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string GateWay;				    //网关
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string NetMask;				    //掩码
                public UInt32 LocalPort;				//端口
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string NetWorkAddr;			    //mac地址
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
                public string ServerIP;				//服务端IP
                public UInt32 ServerPort;				//服务端端口
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
                public string CommPassword;			//通讯密码
                public UInt32 EnableRecCover; 			//允许循环覆盖
            }
            //记录
            public struct LOG
            {
                public UInt32 DevSN;        //设备序列号
                public UInt32 Index;        //记录索引
                public UInt32 CardNo;       //卡号
                public UInt32 Door;         //门号
                public UInt32 Reader;       //读头号
                public SYSTEMTIME DateTime; //事件时间
                public UInt32 WarnCode;     //记录标志
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public UInt32[] DoorState; 			//门状态
            }
            //注册权限
            public struct REGISTER
            {
                public UInt32 CardNo;           //卡号
                public UInt32 Door;             //门号
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
                public string Password;         //密码
                public UInt32 TimeGroup;        //时间组    
                public SYSTEMTIME DateBegin;           //有效期开始时间
                public SYSTEMTIME DateEnd;             //有效期结束时间
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
                public string UserID;                  //用户ID
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
                public string UserName;                //用户姓名
                public UInt32 DepartmentCode;          //部门编码
            }
 
            //门基础信息
            public struct DOOR_BASIC
            {
                public UInt32 State;			//门状态
                public UInt32 Interval;			//刷卡间隔
                public UInt32 LockedDelay;		//开门延时
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
                public string PasswordA;		//开门密码
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
                public string PasswordB;
                public UInt32 CtrlMode;			//门控制方式
            }

            //门高级信息_工作模式
            public struct DOOR_EXPERT_WORKMODE
            {
                public UInt32 DayOfWeek;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.Struct)]
                public SYSTEMTIME[] TimeBegin;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.Struct)]
                public SYSTEMTIME[] TimeEnd;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public UInt32[] Mode;
            }
            //门高级信息_定时开关门
            public struct DOOR_EXPERT_TIMING
            {
                public UInt32 DayOfWeek;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.Struct)]
                public SYSTEMTIME[] TimeBegin;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.Struct)]
                public SYSTEMTIME[] TimeEnd;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public UInt32[] Ctrl;
            }
            //门高级信息_首卡
            public struct DOOR_EXPERT_FIRST
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                public UInt32[] CardNo;			//卡号
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public byte[] WeekConfig;		//星期配置
                public SYSTEMTIME TimeBegin;	//开始时间
                public UInt32 InsideMode;		//范围内工作模式
                public SYSTEMTIME TimeEnd;		//结束时间
                public UInt32 OutsideMode;		//时间范围外工作模式
            }
            //门高级信息
            public struct DOOR_EXPERT
            {
                public UInt32 EnableExpert;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7, ArraySubType = UnmanagedType.Struct)]
                public DOOR_EXPERT_WORKMODE[] ExpertWorkMode;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7, ArraySubType = UnmanagedType.Struct)]
                public DOOR_EXPERT_TIMING[] ExpertTiming;
                public DOOR_EXPERT_FIRST ExpertFist;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
                public UInt32[] GuardCard;
            }

            //多卡开门数据
            public struct MULIT_CARD_DATA
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupA;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupB;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupC;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupD;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupE;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupF;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupG;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupH;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupI;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
                public UInt32[] GroupJ;
            }

            //多卡开门组合
            public struct MULIT_CARD_MAP
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                public UInt32[] ProgramA;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                public UInt32[] ProgramB;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                public UInt32[] ProgramC;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                public UInt32[] ProgramD;
            }
            //UDP端点
            public struct UDPEndPoint
            {
                public UInt32 Address;
                public UInt32 Port;
            }

            public struct Comment
            {
                public UInt32 Size;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
                public string Data;
            }
            //时间段
            public struct AccessV2_TimeSegment
            {
                public int Index;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
                public int[] Times;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
                public SYSTEMTIME[] TimeBegin;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
                public SYSTEMTIME[] TimeEnd;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
                public int[] Mode;
            }
            public struct AccessV2_TimeSegmentPack
            {
                public int Count;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.Struct)]
                public AccessV2_TimeSegment[] Data;
            }
            //时间组
            public struct AccessV2_TimeGroup
            {
                public int Index;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
                public int[] WeekDay;
                public int Holiday;
                public AccessV2_TimeGroup(int index)
                {
                    this.Index = index;
                    this.WeekDay = new int[7] { index, index, index, index, index, index, index };
                    this.Holiday = 0;
                }
            }
            public struct AccessV2_TimeGroupPack
            {
                public int Count;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.Struct)]
                public AccessV2_TimeGroup[] Data;
            }

            public struct LOG_PACK
            {
                public UInt32 Type;
                public UInt32 Count;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
                public LOG[] Log;
            }



            //监听的回调原型
            public delegate void UPLOAD_ARRIVED(ref UDPEndPoint remote, byte dataType, ref LOG log);


            //搜索设备
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_NetSearchOpen")]
            private static extern int AccessV2_NetSearchOpen(string password, UInt32 timeOut);
            //获取搜索到的设备
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_NetSearchGet")]
            private static extern int AccessV2_NetSearchGet(UInt32 index, ref NET_SEARCH search);
            //修改网络参数
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_NetWorkSet")]
            public static extern int AccessV2_NetWorkSet(UInt32 devSN, string password, ref NETWORK network);
            //创建设备对象
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_NewDevice")]
            private static extern uint AccessV2_NewDevice(UInt32 dwDevSN, string zCommPassword, string zCommMode
                , string zCommParam1, UInt32 nCommParam2, UInt32 nCommTimeOut);
            //读取设备信息
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_GetDeviceInfo")]
            private static extern int AccessV2_GetDeviceInfo(UInt32 devNum, ref SETTING setting);

            //写入设备信息
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_SetDeviceInfo")]
            private static extern int AccessV2_SetDeviceInfo(UInt32 devNum, ref SETTING setting);

            //同步设备时间
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_SetDateTime")]
            private static extern int AccessV2_SetDateTime(UInt32 devNum);

            //重启设备
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_RebootDevice")]
            private static extern int AccessV2_RebootDevice(UInt32 devNum);

            //初始化设备
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_InitDevice")]
            private static extern int AccessV2_InitDevice(UInt32 devNum, byte regType);


            //删除新记录(不清空的都是新记录)
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_ClrNewLog")]
            private static extern int AccessV2_ClrNewLog(UInt32 devNum);

            //读取门信息
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_GetDoorInfo")]
            private static extern int AccessV2_GetDoorInfo(UInt32 devNum, byte door
                                                           , ref DOOR_BASIC basic, ref DOOR_EXPERT expert);
            //写入门基础信息
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_SetDoorBascInfo")]
            private static extern int AccessV2_SetDoorBascInfo(UInt32 devNum, byte door
                                                           , ref DOOR_BASIC basic);
            //写入门高级信息
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_SetDoorExpertInfo")]
            private static extern int AccessV2_SetDoorExpertInfo(UInt32 devNum, byte door
                                                           , ref DOOR_EXPERT expert);
            //写入多卡开门数据
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_SetMultiCardDat")]
            private static extern int AccessV2_SetMultiCardDat(UInt32 devNum, byte door
                                                           , ref MULIT_CARD_DATA mulData);
            //写入多卡开门方案
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_SetMultiCardMap")]
            private static extern int AccessV2_SetMultiCardMap(UInt32 devNum, byte door
                                                           , ref MULIT_CARD_MAP mulMap);

            //远程开门
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_RemoteOpen")]
            private static extern int AccessV2_RemoteOpen(UInt32 devNum, byte door);

            //临时开门
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_AwhileOpen")]
            private static extern int AccessV2_AwhileOpen(UInt32 devNum, byte door,UInt32 Seconds);

            //设置门状态
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_SetDoorMode")]
            private static extern int AccessV2_SetDoorMode(UInt32 devNum, byte door, byte mode);
            //读取记录
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_GetOneLog")]
            private static extern int AccessV2_GetOneLog(UInt32 devNum, UInt32 index, ref LOG log);
            //新增权限
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_AddRight")]
            private static extern int AccessV2_AddRight(UInt32 devNum, ref REGISTER register);
            //移除权限
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_DelRight")]
            private static extern int AccessV2_DelRight(UInt32 devNum, byte door, UInt32 cardNo);
            //清空权限
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_ClrRight")]
            private static extern int AccessV2_ClrRight(UInt32 devNum);
            //开启监听器
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_ListenerCreate")]
            private static extern int AccessV2_ListenerCreate(int port, UPLOAD_ARRIVED uploadArrived);
            //关闭监听器
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_ListenerDestroy")]
            private static extern int AccessV2_ListenerDestroy(int hUDP);
            //设备开启监听
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_ListenerAdd")]
            private static extern int AccessV2_ListenerAdd(int hUDP, UInt32 devNum, int port);
            //设备关闭监听
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_ListenerDel")]
            private static extern int AccessV2_ListenerDel(int hUDP, UInt32 devNum, int port);

            //读取时间段
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_TimeSegmentRead")]
            private static extern int AccessV2_TimeSegmentRead(UInt32 devNum, ref AccessV2_TimeSegmentPack segmentPack);

            //下发时间段
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_TimeSegmentWrite")]
            private static extern int AccessV2_TimeSegmentWrite(UInt32 devNum, ref AccessV2_TimeSegmentPack segmentPack);

            //读取时间组
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_TimeGroupRead")]
            private static extern int AccessV2_TimeGroupRead(UInt32 devNum, ref AccessV2_TimeGroupPack groupPack);

            //下发时间组
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_TimeGroupWrite")]
            private static extern int AccessV2_TimeGroupWrite(UInt32 devNum, ref AccessV2_TimeGroupPack groupPack);


            //读取块记录
            [DllImport("NBSDK.dll", EntryPoint = "AccessV2_GetBlockLogNew")]
            private static extern int AccessV2_GetBlockLogNew(UInt32 devNum, UInt32 block, ref LOG_PACK pack);


            ////监听事件
            //public delegate void UploadArrivedEventHandler(UDPEndPoint remote, byte dataType, LOG log);

            //public static event UploadArrivedEventHandler UploadArrivedEvent;
            /// <summary>
            /// 创建设备对象,只是创建对象，不判断通讯
            /// </summary>
            /// <param name="dwDevSN">设备序列号</param>
            /// <param name="zCommPassword">通讯密码</param>
            /// <param name="zCommMode">通讯类型</param>
            /// <param name="zCommParam1">通讯参数1</param>
            /// <param name="nCommParam2">通讯参数2</param>
            /// <param name="nCommTimeOut">通讯超时</param>
            /// <returns></returns>
            public AccessV2(UInt32 dwDevSN, string zCommPassword, string zCommMode
                , string zCommParam1, UInt32 nCommParam2, UInt32 nCommTimeOut)
            {
                m_CurrentDevice = AccessV2_NewDevice(dwDevSN, zCommPassword, zCommMode, zCommParam1, nCommParam2, nCommTimeOut);
            }
            /// <summary>
            /// 搜索设备
            /// </summary>
            /// <param name="password">通讯密码</param>
            /// <param name="timeOut">超时时间</param>
            /// <returns></returns>
            public static List<NET_SEARCH> Search(string password, UInt32 timeOut)
            {
                int count = AccessV2_NetSearchOpen(password, timeOut);
                List<NET_SEARCH> list = new List<NET_SEARCH>();
                for (UInt32 index = 0; index < count; index++)
                {
                    NET_SEARCH one = new NET_SEARCH();
                    int success = AccessV2_NetSearchGet(index, ref one);
                    if (success != 0)
                    {
                        list.Add(one);
                    }
                }
                return list;
            }
            /// <summary>
            /// 修改网络参数
            /// </summary>
            /// <param name="devSN">设备序列号</param>
            /// <param name="password">通讯密码</param>
            /// <param name="network">网络参数</param>
            /// <returns></returns>
            public static bool SetNetwork(UInt32 devSN, string password, NETWORK network)
            {
                int success = AccessV2_NetWorkSet(devSN, password, ref network);
                return success != 0;
            }

            /// <summary>
            /// 读取设备信息
            /// </summary>
            /// <param name="setting">查看结构体详情</param>
            /// <returns></returns>
            public bool ReadSetting(out SETTING setting)
            {
                setting = new SETTING();
                int success = AccessV2_GetDeviceInfo(m_CurrentDevice, ref setting);
                return success != 0;
            }

            /// <summary>
            /// 写入设备信息
            /// </summary>
            /// <param name="setting"></param>
            /// <returns></returns>
            public bool SetSetting(SETTING setting)
            {
                int success = AccessV2_SetDeviceInfo(m_CurrentDevice, ref setting);
                return success != 0;
            }

            /// <summary>
            /// 同步设备时间
            /// </summary>
            /// <returns></returns>
            public bool SetDateTime()
            {
                int success = AccessV2_SetDateTime(m_CurrentDevice);
                return success != 0;
            }

            /// <summary>
            /// 重启设备
            /// </summary>
            /// <returns></returns>
            public bool RebootDevice()
            {
                int success = AccessV2_RebootDevice(m_CurrentDevice);
                return success != 0;
            }

            /// <summary>
            /// 初始化设备
            /// </summary>
            /// <returns></returns>
            public bool InitDevice(byte regType = 2)
            {
                int success = AccessV2_InitDevice(m_CurrentDevice, regType);
                return success != 0;
            }

            /// <summary>
            /// 读取门信息
            /// </summary>
            /// <param name="door"></param>
            /// <param name="basic"></param>
            /// <param name="expert"></param>
            /// <returns></returns>
            public bool ReadDoorInfo(byte door, out DOOR_BASIC basic, out DOOR_EXPERT expert)
            {
                basic = new DOOR_BASIC();
                expert = new DOOR_EXPERT();
                int success = AccessV2_GetDoorInfo(m_CurrentDevice, door, ref basic, ref expert);
                return success != 0;
            }

            /// <summary>
            /// 写入门基础信息
            /// </summary>
            /// <param name="door"></param>
            /// <param name="basic"></param>
            /// <returns></returns>
            public bool SetDoorBasicInfo(byte door, DOOR_BASIC basic)
            {
                int success = AccessV2_SetDoorBascInfo(m_CurrentDevice, door, ref basic);
                return success != 0;
            }

            /// <summary>
            /// 写入高级信息
            /// </summary>
            /// <param name="door"></param>
            /// <param name="expert"></param>
            /// <returns></returns>
            public bool SetDoorExpertInfo(byte door, DOOR_EXPERT expert)
            {
                int success = AccessV2_SetDoorExpertInfo(m_CurrentDevice, door, ref expert);
                return success != 0;
            }

            /// <summary>
            /// 远程开门
            /// </summary>
            /// <param name="door">门号</param>
            /// <returns></returns>
            public bool RemoteOpen(byte door)
            {
                int success = AccessV2_RemoteOpen(m_CurrentDevice, door);
                return success != 0;
            }

            /// <summary>
            /// 临时开门
            /// </summary>
            /// <param name="door">门号</param>
            /// <param name="Seconds">秒数</param>
            /// <returns></returns>
            public bool AwhileOpen(byte door, UInt32 Seconds)
            {
                int success = AccessV2_AwhileOpen(m_CurrentDevice, door, Seconds);
                return success != 0;
            }

            //门常开
            public bool DoorOpen(byte door)
            {
                int success = AccessV2_SetDoorMode(m_CurrentDevice, door, 1);
                return success != 0;
            }
            //门在线
            public bool DoorOnline(byte door)
            {
                int success = AccessV2_SetDoorMode(m_CurrentDevice, door, 2);
                return success != 0;
            }
            //门常闭(锁死)
            public bool DoorClose(byte door)
            {
                int success = AccessV2_SetDoorMode(m_CurrentDevice, door, 3);
                return success != 0;
            }

            /// <summary>
            /// 读取记录
            /// </summary>
            /// <param name="index">记录索引号</param>
            /// <param name="log">记录结构体</param>
            /// <returns></returns>
            public bool ReadLog(UInt32 index, out LOG log)
            {
                log = new LOG();
                int success = AccessV2_GetOneLog(m_CurrentDevice, index, ref log);
                return success != 0;
            }

            /// <summary>
            /// 删除新记录（不清空的都是新记录）
            /// </summary>
            /// <param name="m_CurrentDevice">设备索引号</param>
            /// <returns></returns>
            public bool ClrNewLog()
            {
                int success = AccessV2_ClrNewLog(m_CurrentDevice);
                return success != 0;
            }

            /// <summary>
            /// 新增权限
            /// </summary>
            /// <param name="register">权限结构体</param>
            /// <returns></returns>
            public bool AddRegister(REGISTER register)
            {
                int success = AccessV2_AddRight(m_CurrentDevice, ref register);
                return success != 0;
            }
            /// <summary>
            /// 删除权限
            /// </summary>
            /// <param name="door">门号</param>
            /// <param name="cardNo">卡号</param>
            /// <returns></returns>
            public bool RemoveRegister(byte door, UInt32 cardNo)
            {
                int success = AccessV2_DelRight(m_CurrentDevice, door, cardNo);
                return success != 0;
            }
            /// <summary>
            /// 清空权限
            /// </summary>
            /// <returns></returns>
            public bool ClearRegister()
            {
                int success = AccessV2_ClrRight(m_CurrentDevice);
                return success != 0;
            }

            /// <summary>
            /// 开启监听
            /// </summary>
            /// <param name="port">端口</param>
            /// <param name="uploadArrived">回调函数</param>
            /// <returns></returns>
            public static int ListenerOpen(int port, UPLOAD_ARRIVED uploadArrived)
            {
                int hUDP = AccessV2_ListenerCreate(port, uploadArrived);
                return hUDP;
            }

            /// <summary>
            /// 关闭监听器
            /// </summary>
            /// <param name="hUDP"></param>
            /// <returns></returns>
            public static bool ListenerClose(int hUDP)
            {

                return true;

            }
            //监听新增
            public bool ListenerAdd(int hUDP, int num)
            {
                int success = AccessV2_ListenerAdd(hUDP, m_CurrentDevice, num);
                return success != 0;
            }
            //监听移除
            public bool ListenerClose(int hUDP, int num)
            {
                int success = AccessV2_ListenerDel(hUDP, m_CurrentDevice, num);
                return success != 0;
            }
            /// <summary>
            /// 读取时间段
            /// </summary>
            /// <returns></returns>
            public List<AccessV2_TimeSegment> ReadTimeSegment()
            {
                var pack = new AccessV2_TimeSegmentPack();
                var list = new List<AccessV2_TimeSegment>();
                int success = AccessV2_TimeSegmentRead(m_CurrentDevice, ref pack);
                if (success == 0) return null;
                for (int index = 0; index < pack.Count; index++)
                {
                    list.Add(pack.Data[index]);
                }
                return list;
            }

            /// <summary>
            /// 写入时间段
            /// </summary>
            /// <returns></returns>
            public bool WriteTimeSegment(List<AccessV2_TimeSegment> list)
            {
                var pack = new AccessV2_TimeSegmentPack();
                pack.Data = new AccessV2_TimeSegment[50];
                pack.Count = list.Count;
                for (int index = 0; index < pack.Count; index++)
                {
                    pack.Data[index] = list[index];
                }
                int success = AccessV2_TimeSegmentWrite(m_CurrentDevice, ref pack);
                return success != 0;
            }

            /// <summary>
            /// 读取时间组
            /// </summary>
            /// <returns></returns>
            public List<AccessV2_TimeGroup> ReadTimeGroup()
            {
                var pack = new AccessV2_TimeGroupPack();
                var list = new List<AccessV2_TimeGroup>();
                int success = AccessV2_TimeGroupRead(m_CurrentDevice, ref pack);
                if (success == 0) return null;
                for (int index = 0; index < pack.Count; index++)
                {
                    list.Add(pack.Data[index]);
                }
                return list;
            }

            /// <summary>
            /// 写入时间组
            /// </summary>
            /// <param name="list"></param>
            /// <returns></returns>
            public bool WriteTimeGroup(List<AccessV2_TimeGroup> list)
            {
                var pack = new AccessV2_TimeGroupPack();
                pack.Data = new AccessV2_TimeGroup[50];
                pack.Count = list.Count;
                for (int index = 0; index < pack.Count; index++)
                {
                    pack.Data[index] = list[index];
                }
                int success = AccessV2_TimeGroupWrite(m_CurrentDevice, ref pack);
                return success != 0;
            }


            /// <summary>
            /// 读取块记录
            /// </summary>
            /// <param name="block">块索引</param>
            /// <param name="pack"></param>
            /// <returns>总块数</returns>
            public int ReadBlockLog(UInt32 block, ref LOG_PACK pack)
            {
                int count = AccessV2_GetBlockLogNew(m_CurrentDevice, block, ref pack);
                return count;
            }


            #region//定义事件类型
            public static readonly string[][] RecordType = new string[][]
         {
           new string[]{"解除报警","0"} ,//无
           new string[]{"门长时间未关报警","1"} ,//门
           new string[]{"非法闯入报警","2"} ,//门
           new string[]{"火警","3"} ,//门
           new string[]{"胁迫报警","4"} ,//读头
           new string[]{"戒严报警","5"} ,//无
           new string[]{"强制锁门报警","6"} ,
           new string[]{"门长时间未关报警","8"} ,//门
           new string[]{"非法闯入报警","9"} ,//门
           new string[]{"火警","10"} ,//？
           new string[]{"胁迫报警","11"} ,//门
           new string[]{"反潜回报警","12"} ,//门
           new string[]{"强制锁门报警","13"} ,
           //new string[]{"联动","13"} ,//门
           new string[]{"非法卡或非法二维码","14"} ,//门

           new string[]{"按钮开门","16"} ,//门
           new string[]{"电脑开门","17"} ,//门
           new string[]{"电脑临时开门","18"} ,//门
           new string[]{"超级密码开门","19"} ,//读头
           new string[]{"胁迫密码开门","20"} ,

           new string[]{"通信设置","31"} ,
           new string[]{"通信设置","30"} ,

           new string[]{"紧急开门","32"} ,//门
           new string[]{"紧急关门","33"} ,//门
           new string[]{"紧急恢复","34"} ,//门
           new string[]{"定时开门","35"} ,//门
           new string[]{"定时关门","36"} ,//门
           new string[]{"定时恢复","37"} ,//门
           new string[]{"首卡定时开门","38"} ,//门
           new string[]{"首卡定时关门","39"} ,//门
           new string[]{"首卡定时在线","40"} ,//门

           new string[]{"反潜不通过","80"} ,//读头
           new string[]{"多门互锁不通过","81"} ,//读头
           new string[]{"不够刷卡时间","82"} ,//读头 
           new string[]{"超过有效使用期","83"} ,//读头
           new string[]{"不在时间段内刷卡","84"} ,//读头
           new string[]{"同一时间段内刷卡超次数","85"} ,//读头
           new string[]{"按钮多门互锁不通过","86"} ,//门
           new string[]{"普通卡(戒严，紧急关门，定时关门，首卡没通过等)","88"} ,//读头
           new string[]{"临时卡超时","89"} ,//读头
           new string[]{"临时卡(戒严，紧急关门，定时关门等)","90"} ,//读头
           new string[]{"多卡开门不通过","91"} ,//读头
           new string[]{"多卡开门不通过(戒严，紧急关门，定时关门等)","92"} ,//读头
           new string[]{"多卡开门显示","97"} ,//读头
           new string[]{"多卡开门，已注册但不在多卡组合里","98"} ,//读头
           new string[]{"首卡开门不通过(戒严，紧急关门等)","93"} ,//门
           new string[]{"按钮(戒严，紧急关门，定时关门，首卡没通过等)","94"} ,//门
           new string[]{"卫兵卡主卡","95"} ,//读头
           new string[]{"卫兵卡普通卡","96"} ,//读头

           new string[]{"黑名单","100"} ,//读头
           new string[]{"非法卡或非法二维码","101"} ,//读头
           new string[]{"非法密码","102"} ,//读头
           new string[]{"戒严","112"} ,//读头
           new string[]{"取消戒严","113"} ,//读头
           new string[]{"开机记录","114"} ,//包涵是否有改记录，版本号更新等信息
           new string[]{"软件改序列号","115"} ,//读头
           new string[]{"按了复位按错","116"} ,//读头
           new string[]{"首卡开门","128"} ,//门
           new string[]{"普通卡开门","129"} ,//读头
           new string[]{"临时卡开门","130"} ,//读头
           new string[]{"多卡开门通过","131"} ,//读头
           new string[]{"卫兵卡主卡开门","132"} ,//读头
           new string[]{"卫兵卡普通卡开门","133"} ,//读头
           new string[]{"超级卡开门","134"}  //读头
         };

            #region//数字转事件类型汉字 
            public static string RecordTypeTochineseContent(string para_DigitStr)
            {
                if (para_DigitStr == null)
                {
                    return null;
                }
                int i = 0;
                for (; i < RecordType.Length; i++)
                {
                    if (para_DigitStr == RecordType[i][1].ToString().Trim())
                    {
                        break;
                    }
                }
                if (i < RecordType.Length)
                {
                    return RecordType[i][0];
                }
                else
                {
                    return "未知事件标志,事件标志代码：" + para_DigitStr;
                }
            }
            #endregion
            #endregion
        }
    }
}
