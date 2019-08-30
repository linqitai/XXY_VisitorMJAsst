using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//用于提供公用的方法
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;

namespace XXY_VisitorMJAsst
{
    class Service
    {
        private ListBox listbox;
        //用于一个线程操作另一个线程的控件
        private delegate void SetListBoxCallback(string str);
        private SetListBoxCallback setListBoxCallback;
        public Service(ListBox listbox)
        {
            this.listbox = listbox;
            setListBoxCallback = new SetListBoxCallback(SetListBox);
        }

        public void SetListBox(string str)
        {
            try
            {
                //比较调用SetListBox方法的线程和创建ListBox1的线程是否同一个线程
                //如果不是，则listBox1的InvokeRequired为true
                if (listbox.InvokeRequired == true)
                {
                    //备注：windows应用程序中的每一个控件都对象都有一个InvokeRequired属性，用于检查是否
                    //需要通过调用Invoke方法完成其他线程对该控件的操作，如果该属性为true，说明是其他线程操作该控件，
                    //这时可以创建一个委托实例，然后调用控件对象的Invoke方法，并传入需要的参数完成相应操作，否则可以
                    //直接对该控件对象进行操作，从而保证了其他线程安全操作本线程中的控件。

                    //结果为true,则通过代理执行else中的代码，并传入需要的参数
                    listbox.Invoke(setListBoxCallback, str);
                }
                else
                {
                    //结果为false,直接执行
                    listbox.Items.Add(str);
                    listbox.SelectedIndex = listbox.Items.Count - 1;
                    listbox.ClearSelected();
                }
            }
            catch
            {

            }
        }

        public void SendToOne(User user, string str)
        {
            try
            {
                user.sw.WriteLine(str);
                user.sw.Flush();
                SetListBox(string.Format("向{0}发送{1}", user.userName, str));
            }
            catch
            {
                SetListBox(string.Format("向{0}发送信息失败", user.userName));
            }
        }

        public void SendToAll(System.Collections.Generic.List<User> userList, string str)
        {
            for (int i = 0; i < userList.Count; i++)
            {
                SendToOne(userList[i], str);
            }
        }
    }
}
