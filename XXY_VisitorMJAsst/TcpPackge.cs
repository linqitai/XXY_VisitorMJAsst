using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using System.Runtime.InteropServices;
using System.Reflection.Emit;


using System.Security.Cryptography;

namespace XXY_VisitorMJAsst
{
    /*
        tcp包裹 打包处理部分
     * 处理数据的组包，和解包
     * 各个控制器指令的组包和应答包
     * 
    */

    #region 变量结构类型
    public struct RAcsEvent
    {
        public byte EType;
        public DateTime Datetime;
        public string SerialNo, ID;
        public byte Reader;
        public byte Door;
        public byte EventType;
        public byte Alarm;
        public string Value;
        public byte DoorStatus;
        public Boolean Online;
        public byte TModel;


    }

    public struct RAcsStatus
    {
        public DateTime Datetime;
        public string SerialNo, ID;
        public byte Reader;
        public byte Door;
        public byte Panel;
        public byte DoorStatus;
        public byte Ver;
        public Boolean Online;
        public byte TModel;

        public float T1;
        public float T2;
        public float H1;
        public float H2;
        public string Version;
        public byte NextNum;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RChinaCardByte
    {

        public byte HasImage;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] Name;
        public byte Sex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Nation;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Birthday;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 70)]
        public byte[] Address;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] Card;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public byte[] Dept;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] DateFrm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] DateTo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] Photo;
    }

    public struct RChinaCard
    {
        public string Name;
        public string Sex;
        public string Nation;
        public DateTime Birthday;
        public string Address;
        public string Card;
        public string Dept;
        public DateTime DateFrm;
        public DateTime DateTo;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RTCPStatus
    {
        public byte stx;
        public byte temp;
        public byte cmd;
        public byte addr;
        public byte door;
        public UInt16 len;
        public byte N1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Time;
        public byte DoorStatus;
        public byte N2;
        public byte DirPass;
        public byte N3;
        public byte ControlType;
        public byte RelayOut;
        public UInt16 Output;
        public byte O1;
        public byte O2;
        public byte AlarmOut;

        public byte Ver;
        public UInt16 OEMCODE;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Serial;

        public UInt16 Input;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ID;
        public float T1;
        public float T2;
        public float H1;
        public float H2;

        public UInt32 Version;  // 版本，用于自动升级
        public byte NextNum;    // 备用功能 剩余通过人数
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RTCPCardData
    {
        public byte stx;
        public byte temp;
        public byte cmd;
        public byte addr;
        public byte door;
        public UInt16 len;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Serial;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ID;
        public byte Reader;
        public byte Penal;
        public UInt32 Data;
        public byte DataType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] DateTime;
        public UInt32 Card;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]  //2017-10-11  //1300
        public byte[] Value;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RTCPAlarmData
    {
        public byte stx;
        public byte temp;
        public byte cmd;
        public byte addr;
        public byte door;
        public UInt16 len;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Serial;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ID;
        public byte Reader;
        public byte Penal;
        public UInt32 Data;
        public byte DataType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] DateTime;
        public byte Alarm; // 报警记录号
        public byte Pass;  // 是否通过
        public byte n1;
        public byte n2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] Value;
    }


    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RTCPAckWork
    {
        public byte Open;
        public byte relay;
        public UInt16 time;
        public byte Reader;
        public byte LCDWaitTime;//显示保持多少秒后显示默认首页 为0则不切换
        public byte CardType; //1表示下面的卡号为4字节整数，0为字符串
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] Card;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] Voice;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Event;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] Time;
        public byte Times;
    }
    #endregion

    public class TcpPackge
    {
        [DllImport("WltRS.dll", EntryPoint = "GetBmp", CallingConvention = CallingConvention.StdCall)]
        static extern int GetBmp(string pucPHMsg, int intf);


        #region 常量
        public const byte NET_DATA_TYPE_CARD = 0;  //卡  1二维码 2密码 3超级密码
        public const byte NET_DATA_TYPE_ORCode = 1;  //
        public const byte NET_DATA_TYPE_PIN = 2;  //
        public const byte NET_DATA_TYPE_PC = 4;  //
        public const byte NET_DATA_TYPE_ALARM = 5;  //
        public const byte NET_DATA_TYPE_CHINA = 6;  //
        public const byte NET_DATA_TYPE_DATA = 7;  //
        public const byte NET_DATA_TYPE_BIGDATA = 8;  //
        public const byte NET_DATA_TYPE_BASE64 = 9;
        public const byte NET_DATA_TYPE_FINGER = 10;
        public const byte NET_DATA_TYPE_Vien = 11;
        public const byte NET_DATA_TYPE_RFID = 12;

        
        private const byte Loc_Begin = 0;
        private const byte Loc_Temp = 1;
        public const byte Loc_Command = 2;
        private const byte Loc_Address = 3;
        private const byte Loc_DoorAddr = 4;
        private const byte Loc_Len = 5;
        private const byte Loc_Data = 7;

        private int OEMCode = 23456;
        private byte LastCmd;
        #endregion

        private const UInt16 MAXRQCode = 320;

        #region 内部变量
        private byte[] BufferRX = new byte[2176];
        public byte[] BufferTX = new byte[512];

        private int nBytesWrite = 0;
        public int WriteNum;
        public string Serial;
        public byte Ver;
        #endregion

        #region 事件声明
        public delegate void TOnEventHandler(RAcsEvent Event, ref UInt16 time, ref string card, ref string voice, ref string name, ref string note, ref string etime, ref byte relay, ref byte OpenDoor, ref Boolean Ack);   //声明委托 
        public event TOnEventHandler OnEventHandler;        //声明事件 

        public delegate void TOnStatusHandler(RAcsStatus Event, ref byte relay, ref byte OpenDoor, ref Boolean Ack);   //声明委托 
        public event TOnStatusHandler OnStatusHandler;        //声明事件 

        public delegate void TOnSetTcpTick();   //声明委托 
        public event TOnSetTcpTick OnSetTcpTick;        //声明事件

        public delegate void TOnClearWait(); //声明委托
        public event TOnClearWait OnClearWait; //声明事件

        public delegate Boolean TOnDoSenddata(); //声明委托
        public event TOnDoSenddata OnDoSenddata; //声明事件
        #endregion

        #region 数据包装

        public static object ByteToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }
            //分配结构体内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷贝到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        protected void SetBufCommand(byte command)
        {
            BufferTX[Loc_Begin] = 0x02;
            nBytesWrite = Loc_Data;
            BufferTX[Loc_Command] = command;
            BufferTX[Loc_DoorAddr] = 0;
            BufferTX[Loc_Len] = 0;
            BufferTX[Loc_Len + 1] = 0;
            BufferTX[Loc_Address] = 0xff;
        }

        protected void SetBufDoorAddr(byte ADoorAddr)
        {
            BufferTX[Loc_DoorAddr] = ADoorAddr;
        }

        protected void PutBuf(byte AData)
        {
            BufferTX[nBytesWrite] = AData;
            nBytesWrite++;
        }

        protected void PutBuf0(byte Len)
        {
            int i;
            for (i = 0; i < Len; i++)
                PutBuf((byte)0);
        }

        protected void PutBuf(DateTime AData)
        {
            PutBuf(Convert.ToByte(AData.Hour));
            PutBuf(Convert.ToByte(AData.Minute));
        }

        protected void PutBufDate(DateTime AData)
        {
            if (AData.Year >= 2000)
                PutBuf(Convert.ToByte((AData.Year - 2000) & 0xff));
            else
                PutBuf(Convert.ToByte(AData.Year & 0xff));

            PutBuf(Convert.ToByte(AData.Month));
            PutBuf(Convert.ToByte(AData.Day));
        }

        protected void PutBufCard(UInt64 AData)
        {
            PutBuf(Convert.ToByte((AData) & 0xff));
            PutBuf(Convert.ToByte((AData >> 8) & 0xff));
            PutBuf(Convert.ToByte((AData >> 16) & 0xff));
            PutBuf(Convert.ToByte((AData >> 24) & 0xff));
        }

        protected void PutBufPin2(string AData)
        {
            UInt64 vPin = Convert.ToUInt64(AData);

            PutBuf(Convert.ToByte((vPin >> 8) & 0xff));
            PutBuf(Convert.ToByte(vPin & 0xff));
        }

        protected void PutBufPin4(string AData)
        {
            int i, len;
            byte[] p = new byte[8];
            byte[] v = new byte[4];

            byte[] ap = UTF8Encoding.UTF8.GetBytes(AData);

            try
            {
                len = ap.Length;
                for (i = 0; i < 8; i++) p[i] = 0xFF;

                if (len > 8) len = 8;
                for (i = 0; i < len; i++)
                    p[i] = Convert.ToByte(ap[i] - 0x30);

                for (i = 0; i < 4; i++)
                    v[i] = Convert.ToByte(((p[i * 2] << 4) & 0xF0) + (p[i * 2 + 1] & 0x0F));

                PutBuf(Convert.ToByte(v[0]));
                PutBuf(Convert.ToByte(v[1]));
                PutBuf(Convert.ToByte(v[2]));
                PutBuf(Convert.ToByte(v[3]));
            }
            catch
            {
            }
        }

        protected byte GetStringLen(string AData, byte MaxLen)
        {
            int len;
            byte[] aname = UTF8Encoding.Default.GetBytes(AData);
            len = aname.Length;
            if (len > MaxLen) len = MaxLen;
            return Convert.ToByte(len);
        }

        protected void PutBufString(string AData, byte MaxLen)
        {
            int i, len;
            byte[] aname = UTF8Encoding.Default.GetBytes(AData);

            byte[] p = new byte[MaxLen];
            try
            {
                len = aname.Length;
                if (len > MaxLen) len = MaxLen;

                for (i = 0; i < MaxLen; i++) p[i] = 0;

                for (i = 0; i < len; i++)
                    p[i] = Convert.ToByte(aname[i]);

                for (i = 0; i < MaxLen; i++)
                    PutBuf(Convert.ToByte(p[i]));   // 178 
            }
            catch
            {
            }
        }

        public DateTime GetDatetime(byte Second, byte Minute, byte Hour, byte Day, byte Month, int Year)
        {
            try
            {
                return new DateTime(Year, Month, Day, Hour, Minute, Second);
            }
            catch { return new DateTime(); }
        }

        private static String bytesToHexString(byte[] src)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (src == null || src.Length <= 0)
            {
                return null;
            }
            for (int i = 0; i < src.Length; i++)
            {
                int v = src[i] & 0xFF;

                String hv = v.ToString("X2");
                if (hv.Length < 2)
                {
                    stringBuilder.Append(0);
                }
                stringBuilder.Append(hv);
            }
            return stringBuilder.ToString();
        }


        public String bcd2Str(byte[] bytes)
        {
            StringBuilder temp = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
            {
                temp.Append((byte)((bytes[i] & 0xf0) >> 4));
                temp.Append((byte)(bytes[i] & 0x0f));
            }
            return temp.ToString();//.Substring(0, 1).Equals("0") ? temp.ToString().Substring(1) : temp.ToString();
        }

        public static String ASCii2Char(byte[] bytes)
        {
            byte c;
            StringBuilder temp = new StringBuilder(bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
            {
                c = bytes[i];
                if ((c >= 0x30) && (c <= 0x39))
                    temp.Append((byte)(c & 0x0f));
                else
                    temp.Append((byte)(c));
            }
            return temp.ToString(); //.Substring(0, 1).Equals("0") ? temp.ToString().Substring(1) : temp.ToString();
        }

        public static DateTime StringToDateTime(string date)
        {
            try
            {
                return DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            }
            catch
            {
                return DateTime.Now;
            }

        }

        private byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        #endregion

        #region 发送数据
        private Boolean CheckCs(byte[] buff, int loc)
        {
            int i;

            if (buff[loc] != 0x02) return false;
            if (buff[loc + 3] == 0x03)
                buff[loc + 3] = Convert.ToByte(0x03 + loc);

            int Bufferlen = buff[Loc_Len + 1 + loc] + buff[Loc_Len + 0 + loc] * 256 + Loc_Data + 2;
            if (Bufferlen > BufferRX.Length) return false;
            if (buff[Bufferlen - 1 + loc] != 0x03) return false;

            Boolean result = false;
            byte cs = 0;
            int len = Bufferlen - 2;
            for (i = 0; i < len; i++)
            {
                cs ^= buff[i + loc];
            }
            result = (cs == buff[Bufferlen + loc - 2]);
            return result;
        }

        private Boolean CheckRxDataCS(byte[] buffRX, int len)
        {
            int i;
            if (len < 4) return false;

            int L = 0;
            Boolean re = false;

            for (i = 0; i < 20; i++)
            {
                re = CheckCs(buffRX, i);
                if (re) { L = i; break; }
            }

            if (L > 0)
            {
                for (i = 0; i < len; i++)
                {
                    buffRX[i] = buffRX[i + L];
                }
                len = len - L;
            }

            return re;
        }

        public void BeforeSend()
        {
            int i, datalen;
            byte OutBufferCS, cmd;

            datalen = nBytesWrite - Loc_Data;
            BufferTX[Loc_Len] = Convert.ToByte(datalen & 0xFF);
            BufferTX[Loc_Len + 1] = Convert.ToByte((datalen >> 8) & 0xFF);
            BufferTX[Loc_Temp] = Convert.ToByte(OEMCode & 0xff);

            OutBufferCS = 0;
            for (i = 0; i < nBytesWrite; i++)
                OutBufferCS = Convert.ToByte(OutBufferCS ^ BufferTX[i]);

            BufferTX[nBytesWrite] = OutBufferCS;
            BufferTX[nBytesWrite + 1] = 0x03;
            WriteNum = nBytesWrite + 2;

            cmd = BufferTX[Loc_Command];
            LastCmd = BufferTX[Loc_Command];
        }

        //===================================================================================================
        private void AnsEvent(byte Command, byte Door, byte opendoor, UInt16 time, byte reader, byte delay, string card, string voice, string name, string note, string etime)
        {
            SetBufCommand(Command);
            SetBufDoorAddr(Convert.ToByte(Door + 1));
                     
            PutBuf(Convert.ToByte(opendoor));
            PutBuf(Convert.ToByte(Door));
            PutBuf(Convert.ToByte(time));
            PutBuf(Convert.ToByte(time >> 8));
            PutBuf(Convert.ToByte(reader));
            PutBuf(Convert.ToByte(delay)); // 延迟多少时间 显示首页，0不切换
            int len = card.Length;
            if (len <= 10)
            {
                UInt32 Card32 = Convert.ToUInt32(card);
                PutBuf(Convert.ToByte(1)); // CardIsDWord 
                PutBuf(Convert.ToByte((Card32) & 0xff));
                PutBuf(Convert.ToByte((Card32 >> 8) & 0xff));
                PutBuf(Convert.ToByte((Card32 >> 16) & 0xff));
                PutBuf(Convert.ToByte((Card32 >> 24) & 0xff));
                PutBuf0(14);
            }
            else
            {
                PutBuf(Convert.ToByte(0)); // CardIsDWord
                PutBufString(card, 18);
            }
            PutBufString(voice, 40);
            PutBufString(name, 16);
            PutBufString(note, 32);
            PutBufString(etime, 20); 
            OnDoSenddata();
        }

        public void OpenToEvent(byte relay, byte opendoor, UInt16 time, byte reader, byte delay, string card, string voice, string name, string note, string etime)
        {
            SetBufCommand(0x73);
            SetBufDoorAddr(Convert.ToByte(1));
            PutBuf(Convert.ToByte(opendoor));
            PutBuf(Convert.ToByte(relay));
            PutBuf(Convert.ToByte(time));
            PutBuf(Convert.ToByte(time >> 8));
            PutBuf(Convert.ToByte(reader));
            PutBuf(Convert.ToByte(delay)); // 延迟多少时间 显示首页，0不切换
            PutBuf(Convert.ToByte(0));
            PutBufString(card, 18);
            PutBufString(voice, 40);
            PutBufString(name, 16);
            PutBufString(note, 32);
            PutBufString(etime, 20);
        }

        public void OpenToEvent(byte relay, byte opendoor, UInt16 time, byte reader, byte delay, UInt32 card, string voice, string name, string note, string etime)
        {
            SetBufCommand(0x73);
            SetBufDoorAddr(Convert.ToByte(1));
            PutBuf(Convert.ToByte(opendoor));
            PutBuf(Convert.ToByte(relay));
            PutBuf(Convert.ToByte(time));
            PutBuf(Convert.ToByte(time >> 8));
            PutBuf(Convert.ToByte(reader));
            PutBuf(Convert.ToByte(delay)); // 延迟多少时间 显示首页，0不切换
            PutBuf(Convert.ToByte(1)); // CardIsDWord 
            PutBuf(Convert.ToByte((card >> 24) & 0xff));
            PutBuf(Convert.ToByte((card >> 16) & 0xff));
            PutBuf(Convert.ToByte((card >> 8) & 0xff));
            PutBuf(Convert.ToByte((card) & 0xff));
            PutBuf0(14);
            PutBufString(voice, 40);
            PutBufString(name, 16);
            PutBufString(note, 32);
            PutBufString(etime, 20);
        }

        public void ShowToLCD(byte opendoor, byte reader, byte delay, string card, string name, string note, string etime)
        {
            SetBufCommand(0x81);
            PutBuf(Convert.ToByte(delay)); // 延迟多少时间 显示首页，0不切换
            PutBuf(Convert.ToByte(opendoor));
            PutBufString(card, 32);
            PutBufString(name, 32);
            PutBufString(note, 32);
            PutBufString(etime, 32);
        }

        public void ShowToLCDPage(byte page, byte line, byte delay, string value)
        {
            SetBufCommand(0x83);
            PutBuf(page);
            PutBuf(line);
            PutBuf(delay);
            PutBufString(value, 32);
        }

        public void SendTo485(byte com, byte[] value)
        {
            int i, len;
            SetBufCommand(0xB1);
            BufferTX[Loc_Address] = com;
            //SetBufDoorAddr(com);
            len = value.Length;
            for (i = 0; i < len; i++)
                PutBuf(Convert.ToByte(value[i]));
        }

        public void UpdateSendFirmware(UInt16 index, UInt16 crc, byte[] value)
        {
            int i, len;
            SetBufCommand(0x11);
            BufferTX[Loc_Address] = Convert.ToByte(crc >> 8);
            BufferTX[Loc_DoorAddr] = Convert.ToByte(crc & 0xFF);
            PutBuf(Convert.ToByte(index >> 8));
            PutBuf(Convert.ToByte(index & 0xFF));
            len = value.Length;
            for (i = 0; i < len; i++)
                PutBuf(Convert.ToByte(value[i]));
        }

        public void UpdateCheckFirmware(UInt16 crc)
        {
            SetBufCommand(0x12);
            PutBuf(Convert.ToByte(0));
            PutBuf(Convert.ToByte(crc >> 8));
            PutBuf(Convert.ToByte(crc & 0xff));
        }

        public void UpdateRestart()
        {
            SetBufCommand(0x12);
            PutBuf(Convert.ToByte(1));
        }

        public void Speek(byte Reader, string Speek)
        {
            byte len = 0;
            SetBufCommand(0x80);
            len = GetStringLen(Speek, 54);
            PutBuf(Convert.ToByte(Reader));
            PutBuf(Convert.ToByte(len));
            PutBufString(Speek, 54);
        }

        private void AnsEvent(byte Command, byte index, byte Door, Boolean opendoor)
        {
            SetBufCommand(Command);
            SetBufDoorAddr(Door);
            PutBuf(index);
            PutBuf(Convert.ToByte(opendoor));
            OnDoSenddata();
        }

        private void AnsEvent(byte Command, byte index)
        {
            SetBufCommand(Command);
            SetBufDoorAddr(0);
            PutBuf(index);
            OnDoSenddata();
        }

        private void AskHeart()
        {
            SetBufCommand(0x56);
            PutBuf(Convert.ToByte(OEMCode >> 8));
            PutBuf(Convert.ToByte(OEMCode & 0xFF));
            OnDoSenddata();
        }
        #endregion

        #region 参数类指令
        public void SetTime(DateTime datetime)
        {
            DateTime dt = datetime;
            SetBufCommand(0x07);
            PutBuf(Convert.ToByte(dt.Second));
            PutBuf(Convert.ToByte(dt.Minute));
            PutBuf(Convert.ToByte(dt.Hour));
            PutBuf(Convert.ToByte(dt.DayOfWeek + 1));
            PutBuf(Convert.ToByte(dt.Day));
            PutBuf(Convert.ToByte(dt.Month));
            if (dt.Year >= 2000)
                PutBuf(Convert.ToByte((dt.Year - 2000) & 0xff));
            else
                PutBuf(Convert.ToByte(dt.Year & 0xff));
        }

        /*
         * OpenTime         开门时间
         * OpenOutTime      开门时间过后多久没有关门
         * TooLongAlarm     开门太长时间后产生一个记录
         * AlarmMast        报警项目
         * AlarmTime        报报警项目的警输出时间
         * FireTime         火警时间
         * AlarmInTime      报警输入时候产生报警输出时间
         * EveryCard        任意卡开门
         * CloseAPass       关门门磁时候关门
         * DuressPIN        应急密码 8
         * ChinaCard        应急身份证 18
         * MCard            应急卡 10
         * MQRCode          应急二维码条码等字符串 20
        */
        public void SetControl(UInt16 OpenTime, byte OpenOutTime, Boolean TooLongAlarm, byte AlarmMast, UInt16 AlarmTime, UInt16 FireTime,
                            UInt16 AlarmInTime, Boolean EveryCard, Boolean CloseAPass, string DuressPIN, string ChinaCard, string MCard, string MQRCode)
        {
            SetBufCommand(0x63);
            PutBuf(Convert.ToByte(OpenTime));
            PutBuf(Convert.ToByte(OpenTime >> 8));
            PutBuf(Convert.ToByte(OpenOutTime));
            PutBuf(Convert.ToByte(TooLongAlarm));
            PutBuf(Convert.ToByte(AlarmMast));
            PutBuf(Convert.ToByte(AlarmTime));
            PutBuf(Convert.ToByte(AlarmTime >> 8));

            PutBuf(Convert.ToByte(FireTime));// 火警时间
            PutBuf(Convert.ToByte(FireTime >> 8));
            PutBuf(Convert.ToByte(AlarmInTime));// 报警输入时候产生报警输出时间
            PutBuf(Convert.ToByte(AlarmInTime >> 8));
            PutBuf(Convert.ToByte(EveryCard));// 任意卡开门
            PutBuf(Convert.ToByte(CloseAPass));// 关门门磁时候关门
            PutBufString(DuressPIN, 8); // 应急密码
            PutBufString(ChinaCard, 18);
            PutBufString(MCard, 10);
            PutBufString(MQRCode, 20);
        }
        #endregion

        #region 辅助类指令

        public void Reset()
        {
            SetBufCommand(0x04);
        }

        public void Restart()
        {
            SetBufCommand(0x05);
        }
        #endregion

        #region 控制类指令
        public void Opendoor(byte index)
        {
            SetBufCommand(0x2C);
            SetBufDoorAddr(Convert.ToByte(index + 1));
        }

        public void CloseDoor(byte index)
        {
            SetBufCommand(0x2e);
            SetBufDoorAddr(Convert.ToByte(index + 1));
        }
        public void SetPass(byte index, byte Reader, Boolean Pass)
        {
            SetBufCommand(0x5A);
            SetBufDoorAddr(Convert.ToByte(index + 1));
            PutBuf(Convert.ToByte(0));
            PutBuf(Convert.ToByte(Reader));
            PutBuf(Convert.ToByte(0));
            PutBuf(Convert.ToByte(!Pass));
            PutBuf(Convert.ToByte(0));
        }

        public void LockDoor(byte index, Boolean Lock)
        {
            SetBufCommand(0x2f);
            SetBufDoorAddr(Convert.ToByte(index + 1));
            PutBuf(Convert.ToByte(Lock));
            PutBuf(Convert.ToByte(Lock));
        }

        public void OpenDoorLong(byte index)
        {
            SetBufCommand(0x2d);
            SetBufDoorAddr(Convert.ToByte(index + 1));
        }

        public void SetAlarm(Boolean AClose, Boolean ALong)
        {
            SetBufCommand(0x18);
            PutBuf(Convert.ToByte(AClose));
            PutBuf(Convert.ToByte(ALong));
        }

        public void SetFire(Boolean AClose, Boolean ALong)
        {
            SetBufCommand(0x19);
            PutBuf(Convert.ToByte(AClose));
            PutBuf(Convert.ToByte(ALong));
        }
        #endregion

        #region 数据结构处理

        private static string SaveEmpwltPhotoBytes(string card, byte[] value)
        {
            if (value.Length < 1000) return "";
            string photoname = "";
            try
            {
                string rpath = System.AppDomain.CurrentDomain.BaseDirectory;
                string fname = card;// System.Guid.NewGuid().ToString();
                string photonamewlt = fname + ".wlt";
                photoname = fname + ".bmp";
                string f2 = string.Format("{0}{1}", rpath, photonamewlt);

                // if (!System.IO.File.Exists(f2)) return ""; 
                System.IO.FileStream fs = System.IO.File.Create(f2);

                fs.Write(value, 0, value.Length);
                fs.Close();
                int r = GetBmp(f2, 2);
                /*
                1 相片解码解码正确
                0 调用sdtapi.dll错误
                -1 相片解码错误
                -2 wlt文件后缀错误
                -3 wlt文件打开错误
                -4 wlt文件格式错误
                -5 软件未授权
                -6 设备连接错误 
                 */

                if (r != 1) return "";
                return photoname;
            }
            catch (Exception ex)
            {
                return photoname;
            }
        }

        private static RChinaCard GetChinaCard(RChinaCardByte Data)
        {
            RChinaCard ChinaCard = new RChinaCard();

            ChinaCard.Name = Encoding.Unicode.GetString(Data.Name);
            ChinaCard.Name = ChinaCard.Name.Replace("\0", "");
            ChinaCard.Name = ChinaCard.Name.Trim();

            ChinaCard.Sex = "男";
            if (Data.Sex == 0x30)
                ChinaCard.Sex = "女";

            string na = Encoding.Unicode.GetString(Data.Nation);
            string[] nationality ={"汉","蒙古","回","藏","维吾尔","苗","彝","壮","布依",
                                  "朝鲜","满","侗","瑶","白","土家","哈尼","哈萨克","傣","黎","傈僳","佤","畲","高山","拉祜",
                                  "水","东乡","纳西","景颇","柯尔克孜","土","达斡尔","仫佬","羌","布朗","撒拉","毛南","仡佬",
                                  "锡伯","阿昌","普米","塔吉克","怒","乌孜别克","俄罗斯","鄂温克","德昂","保安","裕固","京",
                                  "塔塔尔","独龙","鄂伦春","赫哲","门巴","珞巴","基诺"};

            int c = Convert.ToInt16(na);
            if (c > 0) c--;
            if (c < nationality.Length)
            {
                ChinaCard.Nation = nationality[c];
            }
            ChinaCard.Birthday = StringToDateTime(ASCii2Char(Data.Birthday));
            ChinaCard.Address = Encoding.Unicode.GetString(Data.Address);
            ChinaCard.Card = ASCii2Char(Data.Card);
            ChinaCard.Dept = Encoding.Unicode.GetString(Data.Dept);
            ChinaCard.DateFrm = StringToDateTime(ASCii2Char(Data.DateFrm));
            ChinaCard.DateTo = StringToDateTime(ASCii2Char(Data.DateTo));
            return ChinaCard;
        }
        #endregion

        #region 数据接收处理
        private void DoFormatStatusvent()
        {
            int vOEM;
            byte Second, Minute, Hour, Day, Month;
            int Year;
            Boolean Ack;
            byte vopenDoor = 0;
            RAcsStatus Event = new RAcsStatus();
            RTCPStatus Status = new RTCPStatus();

            Status = (RTCPStatus)ByteToStruct(BufferRX, typeof(RTCPStatus));

            Event.SerialNo = Encoding.ASCII.GetString(Status.Serial);
            Serial = Event.SerialNo;
            Event.ID = Encoding.ASCII.GetString(Status.ID);
            Event.DoorStatus = Status.DoorStatus;
            Event.Ver = Status.Ver;
            Ver = Event.Ver;

            vOEM = Status.OEMCODE;
            Second = Status.Time[5];
            Minute = Status.Time[4];
            Hour = Status.Time[3];
            Day = Status.Time[2];
            Month = Status.Time[1];
            Year = Status.Time[0] + 2000;
            Event.Datetime = GetDatetime(Second, Minute, Hour, Day, Month, Year);

            Event.H1 = Status.H1;
            Event.H2 = Status.H2;
            Event.T1 = Status.T1;
            Event.T2 = Status.T2;
            Event.Version = Status.Version.ToString();

            Event.Online = true;
            Ack = true;
            byte relay = 0;
            OnStatusHandler(Event, ref relay, ref vopenDoor, ref Ack);
            if (Ack)
                AskHeart();
        }

        private void DoFormCardEvent()
        {
            Boolean Ack;
            RTCPCardData CardData;
            byte vopenDoor = 0;
            UInt16 time = 5; string card = ""; string voice = ""; string name = ""; string note = ""; string etime = "";
            byte times = 0;
            byte Second, Minute, Hour, Day, Month;
            int Year;

            int Valuelen = BufferRX[Loc_Len + 1] + BufferRX[Loc_Len + 0] * 256 - 6 - 10 - 7 - 16 - 4;
            if (Valuelen > 2048) Valuelen = 0;

            RAcsEvent Event = new RAcsEvent();
            CardData = (RTCPCardData)ByteToStruct(BufferRX, typeof(RTCPCardData));

            Event.SerialNo = Encoding.ASCII.GetString(CardData.Serial);
            Serial = Event.SerialNo;
            Event.ID = Encoding.ASCII.GetString(CardData.ID);
            Event.Reader = Convert.ToByte(CardData.Reader & 0x01);
            Event.Door = CardData.door;
            Event.EventType = CardData.DataType;

            Second = CardData.DateTime[5];
            Minute = (CardData.DateTime[4]);
            Hour = (CardData.DateTime[3]);
            Day = (CardData.DateTime[2]);
            Month = (CardData.DateTime[1]);
            Year = (CardData.DateTime[0]) + 2000;
            Event.Datetime = GetDatetime(Second, Minute, Hour, Day, Month, Year);
            Event.Value = Convert.ToString(CardData.Card);

            switch (Event.EventType)
            {
                case NET_DATA_TYPE_CARD:
                    Event.Value = Convert.ToString(CardData.Card);// Convert.ToString(CardData.Value);
                    card = Convert.ToString(CardData.Card);
                    break;

                case NET_DATA_TYPE_ORCode:
                    Event.Value = Encoding.UTF8.GetString(CardData.Value);
                    if (!string.IsNullOrEmpty(Event.Value))
                    {
                        Event.Value = Event.Value.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\0", string.Empty);
                    }
                    break;  //

                case NET_DATA_TYPE_DATA: 
                    Event.Value = Encoding.ASCII.GetString(CardData.Value);
                    if (!string.IsNullOrEmpty(Event.Value))
                    {
                        Event.Value = Event.Value.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\0", string.Empty);
                    }
                    break;  //

                case NET_DATA_TYPE_CHINA:
                    RChinaCardByte ChinaCardData = (RChinaCardByte)ByteToStruct(CardData.Value, typeof(RChinaCardByte));
                    RChinaCard ChinaCard = GetChinaCard(ChinaCardData);
                    if (!string.IsNullOrEmpty(ChinaCard.Name))
                    {
                        Event.Value = ChinaCard.Name + " " + ChinaCard.Card;
                    }
                    //if (ChinaCardData.HasImage == 0x01)
                    //    SaveEmpwltPhotoBytes(ChinaCard.Card , ChinaCardData.Photo);
                    break;

                case NET_DATA_TYPE_PIN:
                    Event.Value = Convert.ToString(CardData.Value);
                    if (!string.IsNullOrEmpty(Event.Value))
                    {
                        Event.Value = Event.Value.Trim().Replace("\n", "").Replace("\r", "").Replace("\0", "");
                    }
                    break;

                case NET_DATA_TYPE_FINGER:
                case NET_DATA_TYPE_BIGDATA:
                    int len = BufferRX[5] * 256 + BufferRX[6] - (6 + 10 + 1 + 1 + 4 + 1 + 16 + 4); // clear head
                    byte[] vdata = new byte[len];
                    Array.Copy(CardData.Value, 0, vdata, 0, len);
                    Event.Value = BitConverter.ToString(vdata).Replace("-", " ");
                    if (!string.IsNullOrEmpty(Event.Value))
                    {
                        Event.Value = Event.Value.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\0", string.Empty);
                    }
                    break;  //

                case NET_DATA_TYPE_BASE64:
                    CardData.Value[Valuelen] = 0;
                    Event.Value = Encoding.ASCII.GetString(CardData.Value, 0, Valuelen);
                    if (!string.IsNullOrEmpty(Event.Value))
                    {
                        Event.Value = Event.Value.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\0", string.Empty);
                    }
                    break;  //

                case NET_DATA_TYPE_RFID:
                    Event.Value = bytesToHexString(CardData.Value);
                    if (!string.IsNullOrEmpty(Event.Value))
                    {
                        Event.Value = Event.Value.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\0", string.Empty);
                    }
                    break;  //


            }
            Event.Online = true;
            Event.EType = 1;
            Ack = true;
            vopenDoor = 0;

            byte relay = Event.Reader;

            OnEventHandler(Event, ref  time, ref card, ref  voice, ref  name, ref  note, ref  etime, ref relay, ref vopenDoor, ref Ack);
            if (Ack)
                AnsEvent(0x54, relay, vopenDoor, time, Event.Reader, 5, card, voice, name, note, etime);
        }

        private void DoFormAlarmEvent()
        {
            Boolean Ack;
            RTCPAlarmData AlarmData;
            byte vopenDoor = 0;
            UInt16 time = 5; string card = ""; string voice = ""; string name = ""; string note = ""; string etime = "";
            byte times = 0;
            byte Second, Minute, Hour, Day, Month;
            int Year;

            RAcsEvent Event = new RAcsEvent();
            AlarmData = (RTCPAlarmData)ByteToStruct(BufferRX, typeof(RTCPAlarmData));

            Event.SerialNo = Encoding.ASCII.GetString(AlarmData.Serial);
            Serial = Event.SerialNo;
            Event.ID = Encoding.ASCII.GetString(AlarmData.ID);
            Event.Reader = Convert.ToByte(AlarmData.Reader & 0x01);
            Event.Door = AlarmData.door;
            Event.EventType = AlarmData.DataType;

            Second = AlarmData.DateTime[5];
            Minute = (AlarmData.DateTime[4]);
            Hour = (AlarmData.DateTime[3]);
            Day = (AlarmData.DateTime[2]);
            Month = (AlarmData.DateTime[1]);
            Year = (AlarmData.DateTime[0]) + 2000;
            Event.Datetime = GetDatetime(Second, Minute, Hour, Day, Month, Year);
            Event.Alarm = AlarmData.Alarm;
            Event.Value = Encoding.Default.GetString(AlarmData.Value);

            Event.Online = true;
            Event.EType = 1;
            Ack = true;
            vopenDoor = 0;
            byte relay = Event.Reader;
            OnEventHandler(Event, ref  time, ref card, ref  voice, ref  name, ref  note, ref  etime, ref relay, ref vopenDoor, ref Ack);
            // 可以不应答
            //if (Ack)
            //  AnsEvent(0x54, relay, vopenDoor, time, Event.Reader, 5, card, voice, name, note, etime);
        }

        public void HandleMessage(byte rt, byte[] buffRX, int len)
        {
            if (rt != 0) return; // 不是接收数据
            try
            {
                Array.ConstrainedCopy(buffRX, 0, BufferRX, 0, len);
                if (CheckRxDataCS(BufferRX, BufferRX.Length))
                {
                    OnSetTcpTick();
                    switch (BufferRX[Loc_Command])
                    {

                        case 0x56: DoFormatStatusvent(); break;
                        case 0x55: DoFormAlarmEvent(); break;
                        case 0x53: DoFormCardEvent(); break;   // card event 

                        default:
                            if (BufferRX[Loc_Command] == LastCmd) OnClearWait();
                            break;
                    }
                }
            }
            catch { }
        }

        #endregion

        static public string EventTypeStr(byte EventType)
        {
            switch (EventType)
            {
                case NET_DATA_TYPE_CARD: return "刷卡";
                case NET_DATA_TYPE_ORCode: return "二维码";
                case NET_DATA_TYPE_DATA: return "数据";
                case NET_DATA_TYPE_PIN: return "密码";
                case NET_DATA_TYPE_CHINA: return "身份证";
                case NET_DATA_TYPE_BIGDATA: return "大数据包";
                case NET_DATA_TYPE_ALARM: return "报警数据";
                case NET_DATA_TYPE_BASE64: return "BASE64数据";
                //ublic const byte NET_DATA_TYPE_PC = 4;  // 
            }
            return "";

        }
    }
}
