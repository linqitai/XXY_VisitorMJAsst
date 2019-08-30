using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;
using XXY_VisitorMJAsst;

namespace XXY_VisitorMJAsst
{

    public   class AcsTcpClass
    {
        #region constdata
        public const byte TcpErr_OK = 0;
        public const byte TcpErr_NotExists = 1; // 对象不存在
        public const byte TcpErr_DataErr = 2; // 数据超出边界
        public const byte TcpErr_OutTime = 3; // 操作超时
        public const byte TcpErr_UnLink = 4; //
        public const byte TcpErr_ReData = 5; // 返回数据错误
        public const byte TcpErr_Working = 6; //
        public const byte TcpErr_Unknow = 7; //
        #endregion


        public byte TCPLastError = 0;

        #region 内部变量
        private Boolean Busy;
        private volatile Boolean FisWaiting;
        private TcpPackge TcpPackge;
        private Boolean HasTcpObj;
        public TcpClientClass TcpIpObj;
        #endregion

        public string SerialNo, ID;

        #region 委托事件声明
        //下面这句后加的
        public delegate void TOnEventHandler(XXY_VisitorMJAsst.RAcsEvent Event,ref UInt16 time,  ref string card, ref string voice, ref string name, ref string note, ref string etime, ref byte relay, ref byte OpenDoor, ref Boolean Ack);   //声明委托 
       
        public event TOnEventHandler OnEventHandler;        //声明事件

 
        public delegate void TOnStatusHandler(XXY_VisitorMJAsst.RAcsStatus Status, string SerialNo, string Version, string ID, DateTime Datetime, byte reader, byte Door,
            byte DoorStatus, byte Ver, Boolean Online, ref byte relay, ref byte OpenDoor, ref Boolean Ack);   //声明委托 
        public event TOnStatusHandler OnStatusHandler;        //声明事件


        public delegate void TOnDisconnect();
        public event TOnDisconnect OnDisconnect;        //声明事件

        //发送和接收事件，用于调试
        public delegate void TOnDataDebug(byte[] buffRX, int len, string str);   //声明委托
        public event TOnDataDebug OnDataDebug;        //声明事件 
        #endregion

        public AcsTcpClass(bool hasTcpObj)
        {
            TcpPackge = new TcpPackge();
            HasTcpObj = hasTcpObj;
            {
                TcpIpObj = new TcpClientClass(HasTcpObj);
                TcpIpObj.OnRxTxDataEvent += DoOnDataDebug;// 调试
                TcpIpObj.OnRxTxDataEvent += TcpPackge.HandleMessage;
                TcpIpObj.OnDisconnected += EventDisConnect;

                TcpPackge.OnSetTcpTick += TcpIpObj.SetTcpTick;
                TcpPackge.OnClearWait += this.ClearWait;
                TcpPackge.OnEventHandler += EventHandler;
                TcpPackge.OnStatusHandler += StatusHandler;

                TcpPackge.OnDoSenddata += this.SendAndNOReturn;
            }
        }

        public Boolean IsconnectSuccess()
        {
            return TcpIpObj.IsconnectSuccess;
        }

        protected Boolean isWorking()
        {
            if (Busy) TCPLastError = TcpErr_Working;
            return Busy;
        }

        private void ClearWait()
        {
            FisWaiting = false;
        }

        //===============================================================
        private Boolean WaitReturn(int delay)
        {
            Boolean te, result;

            while (FisWaiting)
            {

                Thread.Sleep(2);

                TcpIpObj.SetTcpTick();

                int StartTick = Environment.TickCount - TcpIpObj.StartTick;

                te = StartTick > (200 + delay);
                if (te)
                {
                    break;
                }
            }
            result = (!FisWaiting);
            if (result)
            {
                FisWaiting = false;
            }
            else
                TCPLastError = TcpErr_OutTime;
            return result;
        }

        private Boolean WaitReturnx(int delay)
        {
            Boolean te, result;
            int t1 = 0;
            while (FisWaiting)
            {
                Thread.Sleep(2);
                TcpIpObj.SetTcpTick();
                t1++;
                te = t1 > (300 + delay);
                if (te)
                {
                    break;
                }
            }
            result = (!FisWaiting);
            if (result)
            {
                FisWaiting = false;
            }
            else
                TCPLastError = TcpErr_OutTime;
            return result;
        }


        private Boolean SendAndNOReturn()
        {
            byte re;
            TcpPackge.BeforeSend();
            re = TcpIpObj.DoSendData(TcpPackge.BufferTX, TcpPackge.WriteNum);
            TCPLastError = re;
            return (re == 0);
        }

        protected Boolean SendAndReturn(int delay)
        {
            Boolean result = false;
            byte re;
            Busy = true;
            try
            {
                FisWaiting = true;
                TcpPackge.BeforeSend();
                re = TcpIpObj.DoSendData(TcpPackge.BufferTX, TcpPackge.WriteNum);
                TCPLastError = re;
                if (re == 0)
                    result = WaitReturn(delay);
                return result;
            }
            finally
            {
                Busy = false;
            }
        }

        // =====================================================================================================
        public void EventHandler(XXY_VisitorMJAsst.RAcsEvent Event, ref UInt16 time, ref  string card, ref string voice, ref string name, ref string note, ref string etime, ref byte relay, ref byte OpenDoor, ref Boolean Ack)
        {
            relay = Event.Reader;
            if (OnEventHandler != null)
                OnEventHandler(Event, ref time, ref  card, ref  voice, ref  name, ref  note, ref  etime, ref relay, ref OpenDoor, ref Ack);
        }

        public void StatusHandler(XXY_VisitorMJAsst.RAcsStatus Event, ref byte relay, ref byte OpenDoor, ref Boolean Ack)
        {
            OpenDoor = 0;
            Ack = true;
            SerialNo = Event.SerialNo;
            ID = Event.ID;
            relay = Event.Reader;
            {
                TimeSpan ds = DateTime.Now - Event.Datetime;
                if (System.Math.Abs(ds.Seconds) >= 5)
                {
                    TcpPackge.SetTime(DateTime.Now);
                    SendAndNOReturn();
                }
            }
            if (OnStatusHandler != null)
                OnStatusHandler(Event, Event.SerialNo,Event.Version, Event.ID, Event.Datetime, Event.Reader, Event.Door, Event.DoorStatus, Event.Ver, Event.Online, ref relay, ref OpenDoor, ref Ack);
        }

        public void EventDisConnect()
        {
            if (OnDisconnect != null) OnDisconnect();
        }

        public void DoOnDataDebug(byte rt, byte[] buff, int len)
        {
            // 调试用，用于显示传输的实际数据 
            if (OnDataDebug != null)
            {
                byte[] returnBytes = new byte[len];
                Array.ConstrainedCopy(buff, 0, returnBytes, 0, len);
                string str = BitConverter.ToString(returnBytes).Replace("-", " ");

                if (rt == 0)
                { str = string.Concat("接收 ", str); }
                else
                { str = string.Concat("发送 ", str); }
                OnDataDebug(buff, len, str);/* */
            }
        }

        #region 网络指令
        public bool OpenIP(string ip, int port,string pwd)
        {
            if (pwd.Trim() == "")
            {
                return TcpIpObj.OpenIP(ip, port);
            }
            else
            {
                TcpIpObj.AES182 = true;
                TcpIpObj.AESPIN = pwd;
                return TcpIpObj.OpenIP(ip, port);
            }
        }

        public bool CloseTcpip()
        {
            return TcpIpObj.CloseTcpip();
        }
        #endregion

        #region 参数类指令
        public bool SetTime(DateTime datetime)
        {
            if (isWorking()) return false;
            TcpPackge.SetTime(datetime);
            return SendAndReturn(100);
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
        public Boolean SetControl(UInt16 OpenTime, byte OpenOutTime, Boolean TooLongAlarm, byte AlarmMast, UInt16 AlarmTime,
            UInt16 FireTime, UInt16 AlarmInTime, Boolean EveryCard, Boolean CloseAPass, string DuressPIN, string ChinaCard, string MCard, string MQRCode)
        {
            if (isWorking()) return false;
            TcpPackge.SetControl(OpenTime, OpenOutTime, TooLongAlarm, AlarmMast, AlarmTime, FireTime, AlarmInTime, EveryCard, CloseAPass, DuressPIN, ChinaCard, MCard, MQRCode);
            return SendAndReturn(100);
        }
        #endregion

        #region 系统类指令
        public Boolean Reset()
        {
            if (isWorking()) return false;
            TcpPackge.Reset();
            return SendAndReturn(3000);
        }

        public Boolean Restart()
        {
            if (isWorking()) return false;
            TcpPackge.Restart();
            return SendAndReturn(100);
        }
        #endregion

        #region 控制类指令

        public Boolean SetPass(byte index, byte Reader, Boolean Pass)
        {
            if (isWorking()) return false;
            TcpPackge.SetPass(index, Reader, Pass);
            return SendAndReturn(50);
        }

        public Boolean OpenDoor(byte index)
        {
            if (isWorking()) return false;
            TcpPackge.Opendoor(index);
            return SendAndReturn(50);
        }

        public Boolean CloseDoor(byte index)
        {
            if (isWorking()) return false;
            TcpPackge.CloseDoor(index);
            return SendAndReturn(50);
        }

        public Boolean LockDoor(byte index, Boolean Lock)
        {
            if (isWorking()) return false;
            TcpPackge.LockDoor(index, Lock);
            return SendAndReturn(50);
        }

        public Boolean OpenDoorLong(byte index)
        {
            if (isWorking()) return false;
            TcpPackge.OpenDoorLong(index);
            return SendAndReturn(50);
        }

        public Boolean SetAlarm(Boolean AClose, Boolean ALong)
        {
            if (isWorking()) return false;
            TcpPackge.SetAlarm(AClose, ALong);
            return SendAndReturn(50);
        }

        public Boolean SetFire(Boolean AClose, Boolean ALong)
        {
            if (isWorking()) return false;
            TcpPackge.SetFire(AClose, ALong);
            return SendAndReturn(50);
        }
        #endregion


        public Boolean OpenToEvent(byte relay, byte opendoor, UInt16 time, byte reader, byte delay, string card, string voice, string name, string note, string etime)
        {
            if (isWorking()) return false;
            TcpPackge.OpenToEvent(relay, opendoor, time,reader, delay, card, voice, name, note, etime);
            return SendAndReturn(100);
        }

        public Boolean OpenToEvent(byte relay, byte opendoor, UInt16 time, byte reader, byte delay, UInt32 card, string voice, string name, string note, string etime)
        {
            if (isWorking()) return false;
            TcpPackge.OpenToEvent(relay, opendoor, time, reader, delay, card, voice, name, note, etime);
            return SendAndReturn(100);
        }

        public Boolean ShowToLCD(byte opendoor, byte reader, byte delay, string card, string name, string note, string etime)
        {
            if (isWorking()) return false;
            TcpPackge.ShowToLCD(opendoor, reader, delay, card, name, note, etime);
            return SendAndReturn(500);
        }

        public Boolean ShowToLCDPage(byte page, byte line, byte delay, string value)
        {
            if (isWorking()) return false;
            TcpPackge.ShowToLCDPage(page, line, delay, value);
            return SendAndReturn(500);
        }

        public Boolean Speek(byte Reader, string Speek)
        {
            if (isWorking()) return false;
            TcpPackge.Speek(Reader, Speek);
            return SendAndReturn(500);
        }

        public Boolean SendTo485(byte com, byte[] value)
        {
            if (isWorking()) return false;
            TcpPackge.SendTo485(com, value);
            return SendAndNOReturn();
        }

        public Boolean UpdateSendFirmware(UInt16 index, UInt16 crc,byte[] value)
        {
            if (isWorking()) return false;
            TcpPackge.UpdateSendFirmware(index,  crc, value);
            return SendAndReturn(300);
        }

        public Boolean UpdateCheckFirmware(UInt16 crc)
        {
            if (isWorking()) return false;
            TcpPackge.UpdateCheckFirmware(crc);
            return SendAndReturn(8000);
        }

        public Boolean UpdateRestart()
        {
            if (isWorking()) return false;
            TcpPackge.UpdateRestart();
            return SendAndReturn(2000);
        }

    }

}
