using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//用于提供公用的方法
using System.Windows.Forms;
using System.IO;

namespace XXY_VisitorMJAsst
{
    class Client
    {
        ListBox listbox;
        StreamWriter sw;
        public Client(ListBox listbox, StreamWriter sw)
        {
            this.listbox = listbox;
            this.sw = sw;
        }

        //方法1：用于发服务器发送数据
        public void SendToServer(string str)
        {
            try
            {
                sw.WriteLine(str);
                sw.Flush();
            }
            catch
            {
                SetListBox("发送数据失败");
            }
        }
        //方法2：用于在对应的界面上显示相关信息
        delegate void ListBoxCallback(string str);
        public void SetListBox(string str)
        {
            try
            {
                if (listbox.InvokeRequired == true)
                {
                    ListBoxCallback d = new ListBoxCallback(SetListBox);
                    listbox.Invoke(d, str);
                }
                else
                {
                    listbox.Items.Add(str);
                    listbox.SelectedIndex = listbox.Items.Count - 1;
                    listbox.ClearSelected();
                }
            }
            catch
            {

            }
        }
    }
}
