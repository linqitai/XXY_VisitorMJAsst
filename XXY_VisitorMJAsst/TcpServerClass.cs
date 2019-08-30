using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;

namespace XXY_VisitorMJAsst
{
    /* tcp传输通信
     * TCP客户端  连接控制器硬件
     * 异步通信模式
     * 
    */

    /// 与客户端的连接已建立事件参数 
    public class TcpClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// 与客户端的连接已建立事件参数
        /// </summary>
        /// <param name="tcpClient">客户端</param>
        public TcpClientConnectedEventArgs(System.Net.Sockets.TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            this.TcpClient = tcpClient;
        }

        /// <summary>
        /// 客户端
        /// </summary>
        public System.Net.Sockets.TcpClient TcpClient { get; private set; }
    }

    /// 与客户端的连接已断开事件参数 
    public class TcpClientDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// 与客户端的连接已断开事件参数
        /// </summary>
        /// <param name="tcpClient">客户端</param>
        public TcpClientDisconnectedEventArgs(System.Net.Sockets.TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");
            this.TcpClient = tcpClient;
        }
        /// <summary>
        /// 客户端
        /// </summary>
        public System.Net.Sockets.TcpClient TcpClient { get; private set; }
    }

    internal class TcpClientState
    {
        /// <summary>
        /// Constructor for a new Client
        /// </summary>
        /// <param name="tcpClient">The TCP client</param>
        /// <param name="buffer">The byte array buffer</param>
        public TcpClientState(System.Net.Sockets.TcpClient tcpClient, byte[] buffer)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            this.TcpClient = tcpClient;
            this.Buffer = buffer;
        }

        public System.Net.Sockets.TcpClient TcpClient { get; private set; }
        public byte[] Buffer { get; private set; }
        public NetworkStream NetworkStream
        {
            get { return TcpClient.GetStream(); }
        }
    }

    /// 接收到数据报文事件参数
    /// <typeparam name="T">报文类型</typeparam>
    public class TcpDatagramReceivedEventArgs<T> : EventArgs
    {
        /// 接收到数据报文事件参数
        /// <param name="tcpClient">客户端</param>
        /// <param name="datagram">报文</param>
        public TcpDatagramReceivedEventArgs(System.Net.Sockets.TcpClient tcpClient, T datagram)
        {
            TcpClient = tcpClient;
            Datagram = datagram;
        }
        /// 客户端
        public System.Net.Sockets.TcpClient TcpClient { get; private set; }
        /// 报文
        public T Datagram { get; private set; }
    }

    class TcpServerClass : TcpBaseClass, IDisposable
    {
        #region 内部变量
        private Socket sock;
        private IPEndPoint iep;
        private System.Timers.Timer timer;
        #endregion

        #region Properties
        public bool IsRunning { get; private set; }
        public IPAddress Address { get; private set; }
        public int Port { get; private set; }
        public Encoding Encoding { get; set; }
        #endregion


        private TcpListener listener;
        private List<TcpClientState> clients;
        private bool disposed = false;

        public TcpServerClass()
        {
            this.Encoding = Encoding.Default;
            clients = new List<TcpClientState>();

            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Tick);
            timer.Interval = 500;
            timer.Enabled = false;
        }

        private void BeginStart(string LocalIP, int listenPort)
        {
            //    "127.0,0,1"
            Address = IPAddress.Parse(LocalIP);
            iep = new IPEndPoint(Address, listenPort);
            Port = listenPort;

            clients = new List<TcpClientState>();
            listener = new TcpListener(Address, Port);
            listener.AllowNatTraversal(true);
        }

        #region Server
        public TcpServerClass Start(string LocalIP, int listenPort)
        {
            if (!IsRunning)
            {
                BeginStart(LocalIP, listenPort);
                IsRunning = true;
                listener.Start();
                listener.BeginAcceptTcpClient(new AsyncCallback(HandleTcpClientAccepted), listener);
            }
            return this;
        } 

        /// 服务器所允许的挂起连接序列的最大长度 
        public TcpServerClass Start(string LocalIP, int listenPort,int backlog)
        {
            if (!IsRunning)
            {
                BeginStart(LocalIP, listenPort);
                IsRunning = true;
                listener.Start(backlog);
                listener.BeginAcceptTcpClient(
                 new AsyncCallback(HandleTcpClientAccepted), listener);
            }
            return this;
        }
         
        /// 停止服务器  
        public TcpServerClass Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                listener.Stop(); 
                lock (this.clients)
                {
                    for (int i = 0; i < this.clients.Count; i++)
                    {
                        this.clients[i].TcpClient.Client.Disconnect(false);
                    }
                    this.clients.Clear();
                }

            }
            return this;
        }
        #endregion

        #region Receive

        private void HandleTcpClientAccepted(IAsyncResult ar)
        {
            if (IsRunning)
            {
                TcpListener tcpListener = (TcpListener)ar.AsyncState;

                System.Net.Sockets.TcpClient tcpClient = tcpListener.EndAcceptTcpClient(ar);
                byte[] buffer = new byte[tcpClient.ReceiveBufferSize];

                TcpClientState internalClient
                  = new TcpClientState(tcpClient, buffer);
                lock (this.clients)
                {
                    this.clients.Add(internalClient);
                    RaiseClientConnected(tcpClient);
                }

                NetworkStream networkStream = internalClient.NetworkStream;
                networkStream.BeginRead(
                  internalClient.Buffer,
                  0,
                  internalClient.Buffer.Length,
                  HandleDatagramReceived,
                  internalClient);

                tcpListener.BeginAcceptTcpClient(new AsyncCallback(HandleTcpClientAccepted), ar.AsyncState);
            }
        }

        private void HandleDatagramReceived(IAsyncResult ar)
        {
            if (IsRunning)
            {
                TcpClientState internalClient = (TcpClientState)ar.AsyncState;
                NetworkStream networkStream = internalClient.NetworkStream;

                int numberOfReadBytes = 0;
                try
                {
                    numberOfReadBytes = networkStream.EndRead(ar);
                }
                catch
                {
                    numberOfReadBytes = 0;
                }

                if (numberOfReadBytes == 0)
                {
                    // connection has been closed
                    lock (this.clients)
                    {
                        this.clients.Remove(internalClient);
                        RaiseClientDisconnected(internalClient.TcpClient);
                        return;
                    }
                }

                // received byte and trigger event notification
                byte[] receivedBytes = new byte[numberOfReadBytes];
                Buffer.BlockCopy(
                  internalClient.Buffer, 0,
                  receivedBytes, 0, numberOfReadBytes);
                RaiseDatagramReceived(internalClient.TcpClient, receivedBytes);
                RaisePlaintextReceived(internalClient.TcpClient, receivedBytes);

                // continue listening for tcp datagram packets
                networkStream.BeginRead(
                  internalClient.Buffer,
                  0,
                  internalClient.Buffer.Length,
                  HandleDatagramReceived,
                  internalClient);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// 接收到数据报文事件
        /// </summary>
        public event EventHandler<TcpDatagramReceivedEventArgs<byte[]>> DatagramReceived;
        /// <summary>
        /// 接收到数据报文明文事件
        /// </summary>
        public event EventHandler<TcpDatagramReceivedEventArgs<string>> PlaintextReceived;

        private void RaiseDatagramReceived(System.Net.Sockets.TcpClient sender, byte[] datagram)
        {
            if (DatagramReceived != null)
            {
                DatagramReceived(this, new TcpDatagramReceivedEventArgs<byte[]>(sender, datagram));
            }
        }

        private void RaisePlaintextReceived(System.Net.Sockets.TcpClient sender, byte[] datagram)
        {
            if (PlaintextReceived != null)
            {
                PlaintextReceived(this, new TcpDatagramReceivedEventArgs<string>(
                  sender, this.Encoding.GetString(datagram, 0, datagram.Length)));
            }
        }

        /// <summary>
        /// 与客户端的连接已建立事件
        /// </summary>
        public event EventHandler<TcpClientConnectedEventArgs> ClientConnected;
        /// <summary>
        /// 与客户端的连接已断开事件
        /// </summary>
        public event EventHandler<TcpClientDisconnectedEventArgs> ClientDisconnected;

        private void RaiseClientConnected(System.Net.Sockets.TcpClient tcpClient)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, new TcpClientConnectedEventArgs(tcpClient));
            }
        }

        private void RaiseClientDisconnected(System.Net.Sockets.TcpClient tcpClient)
        {
            if (ClientDisconnected != null)
            {
                ClientDisconnected(this, new TcpClientDisconnectedEventArgs(tcpClient));
            }
        }
        #endregion

        #region Send
        /// 发送报文至指定的客户端
        public void Send(System.Net.Sockets.TcpClient tcpClient, byte[] datagram)
        {
            if (!IsRunning)
                throw new InvalidProgramException("This TCP server has not been started.");

            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            if (datagram == null)
                throw new ArgumentNullException("datagram");

            tcpClient.GetStream().BeginWrite( datagram, 0, datagram.Length, HandleDatagramWritten, tcpClient);
        }

        private void HandleDatagramWritten(IAsyncResult ar)
        {
            ((System.Net.Sockets.TcpClient)ar.AsyncState).GetStream().EndWrite(ar);
        }

        /// 发送报文至指定的客户端
        public void Send(System.Net.Sockets.TcpClient tcpClient, string datagram)
        {
            Send(tcpClient, this.Encoding.GetBytes(datagram));
        }

        /// 发送报文至所有客户端
        public void SendAll(byte[] datagram)
        {
            if (!IsRunning)
                throw new InvalidProgramException("This TCP server has not been started.");

            for (int i = 0; i < this.clients.Count; i++)
            {
                Send(this.clients[i].TcpClient, datagram);
            }
        }
       
        public void SendAll(string datagram)
        {
            if (!IsRunning)
                throw new InvalidProgramException("This TCP server has not been started.");

            SendAll(this.Encoding.GetBytes(datagram));
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release 
        /// both managed and unmanaged resources; <c>false</c> 
        /// to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    try
                    {
                        Stop();

                        if (listener != null)
                        {
                            listener = null;
                        }
                    }
                    catch (SocketException ex)
                    {
                        //ExceptionHandler.Handle(ex);
                    }
                }

                disposed = true;
            }
        }

        #endregion

        //================================================================================================================================
        
        override public bool CloseTcpip()
        {
            this.Stop();
            return true;
        }

        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                isHeartTime++;
                if (isHeartTime > 7)
                {
                    isHeartTime = 0;
                    if (sock == null)
                    {
                        timer.Enabled = false;
                    }
                    else // if
                    {
                        if ((!sock.Connected))
                        {
                            timer.Enabled = false;
                            IsconnectSuccess = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SockErrorStr = ex.ToString();
                IsconnectSuccess = false;
                Console.WriteLine(string.Format("timer_Tick {0}", ex.Message));
                timer.Enabled = true;
            }
        }

        override public byte DoSendData(byte[] buffTX, int WriteNum)
        {
            StartTick = Environment.TickCount;
            try
            {
                if (IsconnectSuccess)
                    lock (buffTX)
                    {
                        if (WriteNum > 512) WriteNum = 512;
                        Array.ConstrainedCopy(buffTX, 0, BufferTX, 0, WriteNum);

                        DoOnRxTxDataEvent(1, BufferTX, WriteNum);

                        //     lock(sock)
                        //    sock.BeginSend(buffTX, 0, WriteNum, SocketFlags.None, new AsyncCallback(SendDataEnd), sock); 

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
