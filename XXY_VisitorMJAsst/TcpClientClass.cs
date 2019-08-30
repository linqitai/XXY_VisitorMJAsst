using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;

using System.Security.Cryptography;

namespace XXY_VisitorMJAsst
{
    /* tcp传输通信
     * TCP客户端  连接控制器硬件
     * 异步通信模式
     * 
    */
   public  class TcpClientClass : TcpBaseClass
    {
        #region 内部变量
        private Socket sock;
        private IPEndPoint iep;
        private System.Timers.Timer timer;
        private Boolean Reconnet = false;
        public string remoteHost = "192.168.0.71";
        public int remotePort = 8000;
        private Boolean Enable=true;
        public string AESPIN = "abcdefgh20161234";
        public Boolean AES182=false;
        #endregion
        
        public TcpClientClass(Boolean enable = true)
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Tick);
            timer.Interval = 500;
            timer.Enabled = false;
            Enable = enable;
        }

        private bool IsSocketConnected()
        {
            bool connectState = true;
            bool blockingState = sock.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                sock.Blocking = false;
                sock.Send(tmp, 1, 0);
                connectState = true; //若Send错误会跳去执行catch体，而不会执行其try体里其之后的代码
            }
            catch (SocketException e)
            {
                if (e.NativeErrorCode.Equals(10035))
                {
                    connectState = true;
                }
                else
                {
                    SockErrorStr = e.ToString();
                    connectState = false;
                    IsconnectSuccess = false;
                }
            }
            finally
            {
                sock.Blocking = blockingState;
            }
            return connectState;
        }

        /// 另一种判断connected的方法，但未检测对端网线断开或ungraceful的情况 
        private bool IsSocketConnected(Socket s)
        {
            #region remarks
            /* As zendar wrote, it is nice to use the Socket.Poll and Socket.Available, but you need to take into consideration 
             * that the socket might not have been initialized in the first place. 
             * This is the last (I believe) piece of information and it is supplied by the Socket.Connected property. 
             * The revised version of the method would looks something like this: 
             * from：http://stackoverflow.com/questions/2661764/how-to-check-if-a-socket-is-connected-disconnected-in-c */
            #endregion

            try
            {
                if (s == null)
                    return false;
                return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
            }
            catch (SocketException e)
            {
                IsconnectSuccess = false;
                SockErrorStr = e.ToString();
                return false;
            }
        }
        //================================================================================================================================
        /// 创建套接字+异步连接函数
        private bool socket_create_connect()
        {
            if (remoteHost == "") return false;
            try
            {
                IPAddress serverIp = IPAddress.Parse(remoteHost);
                int serverPort = Convert.ToInt32(remotePort);
                iep = new IPEndPoint(serverIp, serverPort);

                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.SendTimeout = 500;
                sock.BeginConnect(iep, new AsyncCallback(connectedCallback), sock);
            }
            catch (Exception err)
            { 
                SockErrorStr = err.ToString();
                return false;
            }
            return true;
        }

        /// 异步连接回调函数
        private void connectedCallback(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;
            try
            {
                timer.Enabled = true;
                if (sock.Connected)
                {
                    sock.EndConnect(iar);
                    IsconnectSuccess = true;
                    sock.BeginReceive(BufferRX, 0, BufferRX.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), sock);
                }
                else
                {
                    IsconnectSuccess = false;
                }
            }
            catch (Exception e)
            {
                SockErrorStr = e.ToString();
                IsconnectSuccess = false;
               // Console.WriteLine("connectedCallback ");
            }
        }


        public static byte[] Decrypt(byte[] value, string key,int len)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            Aes Aes = Aes.Create();
            Aes.KeySize = 128;
            Aes.Key = keyArray;
            Aes.Mode = CipherMode.ECB;
            Aes.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = Aes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(value, 0, len);
            return resultArray;
        }

        // 接收数据回调函数
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                lock (BufferRX)
                {
                    Socket peerSock = (Socket)ar.AsyncState;
                    int BytesRead = peerSock.EndReceive(ar);
                    if (BytesRead > 0)
                    {
                        if (BytesRead > 2176) 
                            BytesRead = 2176;

                        if (AES182)
                        {
                            byte[] Data = Decrypt(BufferRX, AESPIN,BytesRead);
                            BytesRead = Data.Length;
                            Array.ConstrainedCopy(Data, 0, BufferRX , 0, BytesRead);
                        }

                        DoOnRxTxDataEvent(0, BufferRX, BytesRead);
                    }
                    else//对端gracefully关闭一个连接
                    {
                        if (sock.Connected)//上次socket的状态
                        {
                            DosocketDisconnected();
                            return;
                        }
                    }
                    sock.BeginReceive(BufferRX, 0, BufferRX.Length, 0, new AsyncCallback(ReceiveCallBack), sock);
                }
            }
            catch (Exception ex)
            {
                SockErrorStr = ex.ToString();                
                DosocketDisconnected(); 
                return;
            }
        }

        private bool Reconnect()
        {
            try
            {
                sock.Shutdown(SocketShutdown.Both);
                sock.Disconnect(true);
                IsconnectSuccess = false;
                sock.Close();
            }
            catch (Exception ex)
            {
                SockErrorStr = ex.ToString();
            }
            return socket_create_connect();
        }

        override public bool OpenIP(string ip, int port)
        {
            if (ip == "") return false;
            Reconnet = true;
            if (IsconnectSuccess) return true;
            
            remoteHost = ip;
            remotePort = port;
          
            return socket_create_connect();
        }

        override public bool CloseTcpip()
        {
            timer.Enabled = false;
            Reconnet = false;
            DosocketDisconnected();

            lock (this)
            {
                if (sock != null)
                    if (IsconnectSuccess)
                    {
                        try
                        {
                            //关闭socket 
                            timer.Enabled = false;
                            sock.Disconnect(false);
                            IsconnectSuccess = false;
                        }
                        catch (Exception ex)
                        {
                            SockErrorStr = ex.ToString();
                        }
                    }
            }
            timer.Enabled = false;
            return true;
        }

        private void socketDisconnectedHandler()
        {
            //IsconnectSuccess = false;
           // if (timer.Enabled)
            //    Reconnect();
        }

        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (!Enable) { timer.Enabled = false; return; }
            if (!Reconnet) { timer.Enabled = Reconnet; return; }
            try
            {
                isHeartTime++;
                if (isHeartTime >10)
                {
                    isHeartTime = 0;
                   
                    if (sock == null)
                    {
                        timer.Enabled = false;
                        DosocketDisconnected();
                        socket_create_connect();
                    }
                    else // if
                    {
                        if ((!sock.Connected) || (!IsSocketConnected(sock)) || (!IsSocketConnected()))
                        {
                            DosocketDisconnected();
                            timer.Enabled = false;
                            IsconnectSuccess = false;
                            Reconnect();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SockErrorStr = ex.ToString();
                IsconnectSuccess = false;
                timer.Enabled = true;
            }
        }

        private void SendDataEnd(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
        }

        public static byte[] Encrypt(byte[] value, string key, int len )
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            Aes Aes = Aes.Create();
            Aes.KeySize = 128;
            Aes.Key = keyArray;
            Aes.Mode = CipherMode.ECB;
            Aes.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = Aes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(value, 0, len);
            return resultArray;
        }


        override public byte DoSendData(byte[] buffTX, int WriteNum)
        {
            byte[] bufTX;
            StartTick = Environment.TickCount;
            try
            {
                if (IsconnectSuccess)
                    lock (buffTX)
                    {
                        if (WriteNum > 512) WriteNum = 512;
                        Array.ConstrainedCopy(buffTX, 0, BufferTX, 0, WriteNum);

                        DoOnRxTxDataEvent(1, BufferTX, WriteNum);

                        if (AES182)
                        {
                            bufTX = Encrypt(BufferTX, AESPIN , WriteNum); 
                            WriteNum = bufTX.Length;
                            lock (sock)
                                sock.BeginSend(bufTX, 0, WriteNum, SocketFlags.None, new AsyncCallback(SendDataEnd), sock);
                        }
                        else
                        {
                            lock (sock)
                                sock.BeginSend(buffTX, 0, WriteNum, SocketFlags.None, new AsyncCallback(SendDataEnd), sock);
                        }

                        return 0;
                    }
                return 4;
            }
            catch (Exception ex)
            {
                SockErrorStr = ex.ToString();
                return 7;
            }
        }

        override public void SetTcpTick()
        {
            isHeartTime = 0;
        }
    }
}
