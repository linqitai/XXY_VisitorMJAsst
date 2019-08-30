using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//用于保存与该用户进行通信所需要的信息
using System.Net.Sockets;
using System.IO;

namespace XXY_VisitorMJAsst
{
    class User
    {
        public readonly TcpClient client;
        public readonly StreamReader sr;
        public readonly StreamWriter sw;
        public string userName;
        public User(TcpClient client)
        {
            this.client = client;
            this.userName = "";
            NetworkStream netStream = client.GetStream();
            sr = new StreamReader(netStream, System.Text.Encoding.UTF8);
            sw = new StreamWriter(netStream, System.Text.Encoding.UTF8);
        }
    }
}
