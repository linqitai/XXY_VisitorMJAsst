using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XXY_VisitorMJAsst
{
    public partial class D_RemoterControlFrm_AwhileOpenDoor : Form
    {
        public D_RemoterControlFrm_AwhileOpenDoor()
        {
            InitializeComponent();
        }
        public static string strZT = "";
        public static string strSecond = "60";

        private void D_RemoterControlFrm_AwhileOpenDoor_Load(object sender, EventArgs e)
        {
            strZT = "";
        }

        private void TSB_Save_Click(object sender, EventArgs e)
        {
            strSecond = this.nud_Second.Value.ToString();
            strZT = "OK";
            this.Close();
        }

        private void TSB_Exit_Click(object sender, EventArgs e)
        {
            strZT = "";
            this.Close();
        }
    }
}
