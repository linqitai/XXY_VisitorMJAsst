using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.OleDb;
using System.Data.Sql;
using Microsoft.Win32;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Drawing;
using System.ServiceProcess;

//正在表达式
using System.Text.RegularExpressions;
using System.Web;
using System.Runtime.InteropServices;

//串口短信猫
using System.IO.Ports;
 
namespace XXY_VisitorMJAsst
{
    public class AccessMan
    {
        //Int32 index = 0;
        //AcsTcpClass TcpipObj;
        //UpdateFimware UpdateSys;

        private SQLHelper SQLHelper = new SQLHelper();
        public int DoorCount { set; get; }

        public int CommonPort { set; get; }

        private List<AccessModel> AccessList_;
        public List<AccessModel> AccessList
        {
            get
            {
                return AccessList_;
            }
        }


        public AccessMan(int aid, XXY_VisitorMJAsst.AcsTcpClass.TOnEventHandler uploadArrived, int port)
        {
            this.UploadArrived = uploadArrived;
            LoadDoorInf(aid);
            CommonPort = port;
        }

        public void AccessMan_NoListening(int aid, int port)
        {
            LoadDoorInf(aid);
            CommonPort = port;
        }

        public bool isExist(string MSNo)
        {
            bool blIsExist = AccessList_.Exists(a => a.MSNo == MSNo);
            return blIsExist ;
        }

        public AccessModel getAccessModel(string MSNo) 
        {
            if (isExist(MSNo))
            {
                return AccessList_.Find(a => a.MSNo == MSNo);
            }
            return null;
        }
        public LBDoorControlDLL.AccessV2 getAccess(string MSNo)
        { 
            //if(null != getAccessModel(MSNo))
            //{
            //    return getAccessModel(MSNo).Access;
            //}

            return null;
        }

        #region//根据设备序列号和门号，实现开门
        public bool CloseDoor(string MSNo, byte doorid) 
        {
            var accessModel = getAccessModel(MSNo);
            var accessV2 = accessModel.TcpipObj;

            if (null != accessV2 && accessModel.IsActive)
            {
                return accessV2.CloseDoor(doorid) == true;
            }
            return false;  
        }


        #endregion

        #region//根据设备序列号和门号，实现开门
        public bool OpenDoor(string MSNo, byte doorid)
        {
            var accessModel = getAccessModel(MSNo);
            var accessV2 = accessModel.TcpipObj;

            if (null != accessV2 && accessModel.IsActive)
            {
                return accessV2.OpenDoor(doorid) == true;
            }
            return false;  
        }
        #endregion

        #region//根据设备序列号和门号删除卡片信息
        public bool RemoveRegister(string msno, byte doorid, UInt32 cardNo)
        {
            //var accessModel = getAccessModel(msno);
            //var accessV2 = accessModel.Access;
            //if (null != accessV2 && accessModel.IsActive)
            //{
            //    return accessV2.RemoveRegister(doorid, cardNo) == true;
            //}

            return false;
        }
        #endregion

        #region// 同时开启两个门
        public bool OpenToEvent(string MSNo, UInt16 time)
        {
            var accessModel = getAccessModel(MSNo);
            var accessV2 = accessModel.TcpipObj;

            if (null != accessV2 && accessModel.IsActive)
            {
                return accessV2.OpenToEvent(byte.Parse("0"), byte.Parse("100"), time, byte.Parse("1"), 5, "0", "", "", "", "");
            }
            return false;

            //re = TcpipObj.OpenToEvent(0, byte.Parse("100"), time, byte.Parse("1"), 5, "", "", "", "", "");
        }

        #endregion

        public void closeAccess() 
        {
            foreach (var access in AccessList)
            {
                if (access.IsListening)
                {
                    var accessV2 = access.TcpipObj;

                    bool result = accessV2.CloseTcpip( );//1.关闭监听
                    if (result)
                    {
                        access.IsListening = false;
                    }
                }
            }

            LBDoorControlDLL.AccessV2.ListenerClose(ListenerHandler);//2.关闭监听器
            ListenerHandler = 0;
        }

        public bool listenAccess(string msno)
        {
            //var accessV2 = getAccessModel(msno);
            //if (!accessV2.IsListening)
            //{
            //    bool result = accessV2.Access.ListenerAdd(getListenerHandler(), 0);
            //    accessV2.IsListening = result;
            //    return result;
            //}
            return false;
        }

        /// <summary>
        /// 重新监听所有在线设备(序列号重复，则自动过滤)
        /// </summary>
        public int listenAllAccessAgain() 
        {
            int outlineCount = 0;
            //foreach (AccessModel model in AccessList)
            //{
            //    if (model.IsActive)
            //    {
            //        bool result = model.Access.ListenerAdd(getListenerHandler(), 0);
            //        model.IsListening = result;
            //    }
            //    else
            //    {
            //        outlineCount++;
            //    }
            //}
            return outlineCount;
        }



        public int ListenerHandler { set; get; }
        public XXY_VisitorMJAsst.AcsTcpClass.TOnEventHandler UploadArrived { set; get; }

        private int getListenerHandler()
        {
            //if (ListenerHandler == 0) {
            //    ListenerHandler = LBDoorControlDLL.AccessV2.ListenerOpen(Convert.ToInt32(CommonPort), UploadArrived);
            //}
            return ListenerHandler;
        }



        #region//根据区域编号得到门信息和控制器连接信息
        private void LoadDoorInf(int aid)
        {
            AccessList_ = new List<AccessModel>();

            try
            {
                string strSQL = "";
                if (aid != 0)
                {
                    strSQL = "select * from  XXCLOUD.dbo.T_MJAPMachineInf where AId='" + aid + "' ";
                }
                else
                {
                    //全部区域
                    strSQL = "select * from  XXCLOUD.dbo.T_MJAPMachineInf  ";
                }
                DataTable myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DoorCount = myTable.Rows.Count;//保存总门数

                    for (int i = 0; i < myTable.Rows.Count; i++)
                    {

                        string MSNo =  myTable.Rows[i]["MSNo"].ToString().Trim() ;
                        string MIPAddress = myTable.Rows[i]["MIPAddress"].ToString();
                        string MCommType = myTable.Rows[i]["MCommType"].ToString().Trim();
                        uint MCommPort = Convert.ToUInt32(myTable.Rows[i]["MCommPort"].ToString().Trim());
                        string MCommPwd = myTable.Rows[i]["MCommPwd"].ToString().Trim();

                        if (!isExist(MSNo + ""))
                        {
                            UpdateFimware UpdateSys1 = new UpdateFimware();
                            AcsTcpClass TcpipObj1 = new AcsTcpClass(true);
                            TcpipObj1.OnEventHandler += XXY_VisitorMJAsst.D_RemoterControlFrm.eventHandler;
                            TcpipObj1.OnStatusHandler += XXY_VisitorMJAsst.D_RemoterControlFrm.statusHandler;
                            TcpipObj1.OnDisconnect += XXY_VisitorMJAsst.D_RemoterControlFrm.disconnect;
                            //TcpipObj.OnDataDebug += XXY_VisitorMJAsst.D_RemoterControlFrm.showHexMsg;
                            bool blIsActive = TcpipObj1.SetControl(1, 2, false, 0, 10, 180, 5, false, false, "1234578", "ab", "cd", "ef123");

                            var model = new AccessModel()
                            {
                                MSNo = MSNo + "",
                                IPAddress = myTable.Rows[i]["MIPAddress"].ToString().Trim(),
                                MCommPort = MCommPort + "",
                                MCommPwd = MCommPwd,
                                MCommType = MCommType,
                                AId = aid,
                                IsActive = true,

                                TcpipObj = TcpipObj1,
                                UpdateSys = UpdateSys1,
                            };
                    
                            if (TcpipObj1.OpenIP(myTable.Rows[i]["MIPAddress"].ToString().Trim(), Convert.ToInt32(myTable.Rows[i]["MCommPort"].ToString().Trim()),MCommPwd) == true)
                            {
                                // MessageBox.Show ("连接成功");
                            }
                            else
                            {
                                // MessageBox.Show("连接失败");
                            }
                            AccessList.Add(model);
                        }
                    }

           
                }
                else
                {
                    //没有控制器及其门信息
                }
            }
            catch  (Exception exp)
            {
                MessageBox.Show(exp.ToString ());
            }
        }
        #endregion


        #region 更新设备连接状态
        /// <summary>
        /// 更新链接状态
        /// </summary>
        /// <param name="outAccess">返回断开链接的设备</param>
        /// <param name="connectingAccess">返回正常设备</param>
        public void RefreshState(out List<AccessModel> outAccess)
        {
            outAccess = new List<AccessModel>();
            //foreach (AccessModel model in AccessList)
            //{
            //    XXY_VisitorMJAsst.LBDoorControlDLL.AccessV2.SETTING setting;
            //    bool active = model.Access.ReadSetting(out setting);
            //    model.IsActive = active;

            //    if (!active) {
            //        outAccess.Add(model);
            //    }
            //}
        }
        #endregion

   
    }

    public class AccessModel
    {
        public string MSNo { set; get; }
     
   
        public bool IsListening { set; get; }
        public string IPAddress { set; get; }
        public string MCommType { set; get; }
        public string MCommPort { set; get; }
        public string MCommPwd { set; get; }
        public int AId { set; get; }
        public bool IsActive { set; get; }

        public XXY_VisitorMJAsst.AcsTcpClass TcpipObj { set; get; }
        public XXY_VisitorMJAsst.UpdateFimware UpdateSys { set; get; }
  
    }
}
