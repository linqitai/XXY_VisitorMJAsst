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

//正在表达式
using System.Text.RegularExpressions;
using System.Web;


//串口短信猫
using System.IO.Ports;
 

namespace XXY_VisitorMJAsst
{
    public class XXCLOUDDLL
    {
        private int[] keyHandles = new int[8];
        private byte[] memRData = new byte[8];
        private StringBuilder sGUID_new = new StringBuilder(0x40);
        private StringBuilder sGUID_old = new StringBuilder(0x40);
        private SQLHelper SQLHelper = new SQLHelper();
        private StringBuilder sqlList = new StringBuilder();
        private string strConn = SQLHelper.connectionString;
        private string strMCode = "";
        //private string strMCodeAll = "";//
        private string strSQL = "";
        private DataTable myTable = new DataTable();
        private DataView DV;

        public bool blOpenMSCatSuc = false;//false;加载短信猫失败  true:加载短信猫成功
        public SerialPort serialPort = new SerialPort();//用于短信猫

        #region//生成随机四位验证码
        //zwcomrevstr = "";
        bool zwsndok = false;
        public string GetVerCode()
        {
            string vc = "";
            Random rNum = new Random();//随机生成类
            int num1 = rNum.Next(0, 9);//返回指定范围内的随机数
            int num2 = rNum.Next(0, 9);
            int num3 = rNum.Next(0, 9);
            int num4 = rNum.Next(0, 9);

            int[] nums = new int[4] { num1, num2, num3, num4 };
            for (int i = 0; i < nums.Length; i++)//循环添加四个随机生成数
            {
                vc += nums[i].ToString();
            }
            return vc;
        }

        #endregion

        #region//加载短信猫
        public bool blOpendMSCat(int para_iMSCatPort)
        {
            try
            {
                serialPort.Close();//先关闭再打开
                serialPort.PortName = "COM" + para_iMSCatPort.ToString();
                serialPort.BaudRate = 115200;
                serialPort.Open();
                blOpenMSCatSuc = true;
                return true;
            }
            catch
            {
                blOpenMSCatSuc = false;
                return false;
            }


        }
        #endregion

        #region//关闭短信猫
        public void ClosedMSCat()
        {
            try
            {
                serialPort.Close();//先关闭再打开
            }
            catch
            {
            }
        }
        #endregion

        #region//根据手机号码和内容发送短信
        public bool SendMesByPhoneAndContext(string para_strPhone, string para_strContext)
        {
            try
            {
                if (blOpenMSCatSuc == false)
                {
                    return false;
                }
                //短信发送
                if (para_strPhone.Length != 11)
                {
                    return false;
                }
                if (para_strContext.Length == 0)
                {

                    return false;
                }

                string cmd = "ZWZDFS";
                cmd = cmd + "<" + para_strPhone + ">";
                cmd = cmd + para_strContext;


                zwsndok = false;

                Encoding gb = Encoding.GetEncoding("gb2312");
                byte[] bytes = gb.GetBytes(cmd);
                serialPort.Write(bytes, 0, bytes.Length);
                if (waitandchk(2))
                {
                    // MessageBox.Show("发送成功AA！\n");
                }

                else
                {
                    // MessageBox.Show("发送失败！\n");
                }
                return true;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
                return false;
            }
        }

        bool waitandchk(int ns)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < ns * 1000)
            {
                Application.DoEvents();
                if (zwsndok)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region//检测访客门禁网络通信助手服务是否已经在运行
        public int LoginFrm_iMJServerPromgramIsRun()
        {
            strSQL = "云访客门禁网络通信助手";
            int iCount = 0;
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.ToString().Trim().ToLower().Contains(strSQL.ToLower()))
                {
                    iCount++;
                }
            }
            return iCount;
        }
        #endregion

        #region//卡号进制互转
        /// <summary>
        /// 给定十进制数字字符串，转成16进制数（去符号）
        /// </summary>
        /// <param name="DNumber"></parm>
        /// <returns></returns>
        public string ConvertToABSHex(string DNumber)
        {
            #region//蓝本配老的黑我读头  老方法
            Int64 d = Int64.Parse(DNumber);//转换成4字长度在整数
            string HexStr = d.ToString("x8").ToLower();//转16进制
            HexStr = HexStr.TrimStart("ffffffff".ToCharArray());

            return Int64.Parse(HexStr, System.Globalization.NumberStyles.HexNumber) + "";
            #endregion

            //#region//蓝本配新的身份证和IC卡二合一读头
            //Int64 d = Int64.Parse(DNumber);//转换成4字长度在整数
            //string HexStr = d.ToString("x8").ToLower();//转16进制
            //if (HexStr.Length == 8)
            //{
            //    HexStr = HexStr.Substring(2);
            //}

            //HexStr = HexStr.TrimStart("ffffffff".ToCharArray());

            //return System.Convert.ToInt32(HexStr, 16).ToString();
            //#endregion
        }
        #endregion

        //#region//计算唯一的二维码编号
        //public string GetVBarCodeNo()
        //{


        //    string strVBarCodeNo = "";
        //    int iBarCodeLable = 1;
        //    int iBarCodeNoDigits = 7;
        //    try
        //    {
        //        iBarCodeLable = Convert.ToInt32(LoginFrm.strBarCodeLable);
        //    }
        //    catch
        //    {
        //        iBarCodeLable = 1;
        //    }
        //    try
        //    {
        //        iBarCodeNoDigits = Convert.ToInt32(LoginFrm.strBarCodeDigits);
        //    }
        //    catch
        //    {
        //        iBarCodeNoDigits = 7;
        //    }
        //    if (System.DateTime.Now.Day.ToString().Length == 2)
        //    {
        //        strVBarCodeNo = iBarCodeLable.ToString() + System.DateTime.Now.Day.ToString().Substring(1, 1);
        //    }
        //    else
        //    {
        //        strVBarCodeNo = iBarCodeLable.ToString() + System.DateTime.Now.Day.ToString();
        //    }

        //    try
        //    {
        //        //查找某年某月某日的符合strVBarCodeNo开头的最大条码
        //        strSQL = "select max(BarCodeNo) BarCodeNo from " + XXY_VisitorMJAsst._4CloudVisitorReg.VisitorRegFrm.strT_VisitorAccessInf + " where (LeaDDetailName ='" + "" + "' ) ";
        //        strSQL += " and RegYear = '" + System.DateTime.Now.Year.ToString() + " ' and RegMonth='" + System.DateTime.Now.Month.ToString() + "' and RegDay='" + System.DateTime.Now.Day.ToString() + "' ";
        //        strSQL += " and BarCodeNo like '" + strVBarCodeNo + "%'";
        //        strSQL += LoginFrm.strLoginFrmSelectFlag;
        //        myTable = SQLHelper.DTQuery(strSQL);
        //        if (myTable.Rows.Count > 0)
        //        {
        //            if (myTable.Rows[0]["BarCodeNo"].ToString().Trim() != "" && myTable.Rows[0]["BarCodeNo"].ToString().Trim() != null)
        //            {
        //                strVBarCodeNo = Convert.ToString(Convert.ToInt32(myTable.Rows[0]["BarCodeNo"].ToString().Trim()) + 1);
        //            }
        //            else
        //            {
        //                #region
        //                if (iBarCodeNoDigits == 5)
        //                {
        //                    strVBarCodeNo = strVBarCodeNo + "001";
        //                }
        //                else if (iBarCodeNoDigits == 6)
        //                {
        //                    strVBarCodeNo = strVBarCodeNo + "0001";
        //                }
        //                else if (iBarCodeNoDigits == 7)
        //                {
        //                    strVBarCodeNo = strVBarCodeNo + "00001";
        //                }
        //                else if (iBarCodeNoDigits == 8)
        //                {
        //                    strVBarCodeNo = strVBarCodeNo + "000001";
        //                }
        //                else if (iBarCodeNoDigits == 9)
        //                {
        //                    strVBarCodeNo = strVBarCodeNo + "0000001";
        //                }
        //                #endregion
        //            }
        //        }
        //        else
        //        {
        //            #region
        //            if (iBarCodeNoDigits == 5)
        //            {
        //                strVBarCodeNo = strVBarCodeNo + "001";
        //            }
        //            else if (iBarCodeNoDigits == 6)
        //            {
        //                strVBarCodeNo = strVBarCodeNo + "0001";
        //            }
        //            else if (iBarCodeNoDigits == 7)
        //            {
        //                strVBarCodeNo = strVBarCodeNo + "00001";
        //            }
        //            else if (iBarCodeNoDigits == 8)
        //            {
        //                strVBarCodeNo = strVBarCodeNo + "000001";
        //            }
        //            else if (iBarCodeNoDigits == 9)
        //            {
        //                strVBarCodeNo = strVBarCodeNo + "0000001";
        //            }
        //            #endregion
        //        }
        //        return strVBarCodeNo;
        //    }
        //    catch (Exception exp)
        //    {

        //        MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        #region
        //        if (iBarCodeNoDigits == 5)
        //        {
        //            strVBarCodeNo = strVBarCodeNo + "001";
        //        }
        //        else if (iBarCodeNoDigits == 6)
        //        {
        //            strVBarCodeNo = strVBarCodeNo + "0001";
        //        }
        //        else if (iBarCodeNoDigits == 7)
        //        {
        //            strVBarCodeNo = strVBarCodeNo + "00001";
        //        }
        //        else if (iBarCodeNoDigits == 8)
        //        {
        //            strVBarCodeNo = strVBarCodeNo + "000001";
        //        }
        //        else if (iBarCodeNoDigits == 9)
        //        {
        //            strVBarCodeNo = strVBarCodeNo + "0000001";
        //        }
        //        #endregion
        //        return strVBarCodeNo;
        //    }
        //}
        //#endregion

        #region//数字转民族汉字(华旭读卡器专用)
        public string DigitToChineseCharacter(string para_DigitStr)
        {
            try
            {
                if (para_DigitStr == null)
                {
                    return null;
                }
                int i = 0;
                for (; i < SQLHelper._Nation.Length; i++)
                {
                    if (para_DigitStr == SQLHelper._Nation[i][1].ToString().Trim())
                    {
                        break;
                    }
                }
                return SQLHelper._Nation[i][0];
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region//检测中控URU5000指纹采集仪序列号
        public bool VerifyURUFPDeviceSerialNumber(string para_strFTDSNo)
        {
            return true;
            //bool bl = false;
            //string[] FTDSNo = { "{613A0888-3777-4B40-A8C0-1DC1AC57604A}", "{FD6C4A5D-E4D8-9849-942A-3BACB69FDE63}", 
            //                      "{AA8F2196-A8B8-5149-A5A5-C594AFB02D9A}", "{81F0754A-D676-A641-98BE-86C1AC5E1809}" ,"3832161703142"};
            //for (int i = 0; i < FTDSNo.Length; i++)
            //{
            //    if (FTDSNo[i].ToString().Trim() == para_strFTDSNo)
            //    {
            //        bl = true;
            //        break;
            //    }
            //}
            //return bl;
        }
        #endregion

        #region//筛选多音字
        public string FilteringPolyphone(DataTable para_Dt, string para_strName, string para_strActualNo)
        {
            string strPH = "";
            //if (para_Dt.Rows.Count > 0)
            //{
            //    for (int i = 0; i < para_Dt.Rows.Count; i++)
            //    {
            //        for (int j = 0; j < SQLHelper.polyphone.Length; j++)
            //        {
            //            if (para_Dt.Rows[i][para_strName].ToString().Trim().Contains(SQLHelper.polyphone[j].ToString().Trim()))
            //            {
            //                strPH += para_Dt.Rows[i][para_strActualNo].ToString().Trim() + ",";
            //                break;
            //            }
            //        }
            //    }
            //}
            return strPH;
        }
        #endregion

        #region//获取公网或内网的IP地址
        public string GetIPAddress()
        {
            string tempip = "";
            try
            {
                //IP地址分下面两种情况
                //1.公网或带公网的局域网获得的IP地址是一样的.
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ips138.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                string all = sr.ReadToEnd(); //读取网站的数据

                int start = all.IndexOf("您的IP地址是：[") + 9;
                int end = all.IndexOf("]", start);
                tempip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
            }
            catch
            {
            }
            if (tempip.Trim() != "")
            {
                return tempip;
            }
            else
            {
                //2.不带公网的局域网(IP地址类似于192.168.XX.XX)或无网络环境(IP地址就是127.0.0.1)
                string LocalName = Dns.GetHostName();
                IPHostEntry host = Dns.GetHostEntry(LocalName);
                foreach (IPAddress ip in host.AddressList)
                {
                    tempip = ip.ToString().Trim();
                    if (tempip.Contains(":") == false)
                    {
                        break;
                    }
                }
                return tempip;
            }
        }

        //1。获取局域网ip      

        //IPAddress ipAddr = Dns.Resolve(Dns.GetHostName()).AddressList[0];//获得当前IP地址
        //string ip=ipAddr.ToString() ;

        //3.如果是ADSL上网，获取公网ip

        //string tempIP =string.Empty;
        //if (System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Length >1)
        //tempIP = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[1].ToString();
        #endregion

        #region//11位手机号码的正则表达式
        public static bool isMobilePhone(string mobileNum)
        {
            return true;
            /**
             * 手机号码: 
             * 13[0-9], 14[5,7], 15[0, 1, 2, 3, 5, 6, 7, 8, 9], 17[6, 7, 8], 18[0-9], 170[0-9]
             * 移动号段: 134,135,136,137,138,139,150,151,152,157,158,159,182,183,184,187,188,147,178,1705
             * 联通号段: 130,131,132,155,156,185,186,145,176,1709
             * 电信号段: 133,153,180,181,189,177,1700
             */
            string MOBILE = @"(^((1[3,5,8][0-9])|(14[5,7])$|(17[0,1,6,7,8]))\d{8}$)";//手机号正则表达式
            //string COMMON = @"^1[3,4,5,7,8]\d{9}$";//手机号正则表达式

            string tel400 = @"(^400[0-9]{7}$)";//400电话正则表达式 例如：4000000011 
            string tel800 = @"(^800[0-9]{7}$)";//800电话 例如：8000000011 

            string tel400_ = @"(^400-[0-9]{3}-[0-9]{4}$)";//400电话 例如：400-000-0011
            string tel800_ = @"(^800-[0-9]{3}-[0-9]{4}$)";//400电话 例如：800-000-0011

            string tel12 = @"(^0[0-9]{2,3}-[0-9]{8}$)";//区号-电话号码  例如：0577-64859188
            string tel8 = @"(^[0-9]{8}$)";//最普通的8位座机号 64859189


            string regexString = MOBILE + "|" +
                                tel400 + "|" +
                                tel800 + "|" +
                                tel400_ + "|" +
                                tel800_ + "|" +
                                tel12 + "|" +
                                tel8;
            Regex regex = new Regex(regexString);

            return regex.IsMatch(mobileNum);

        }
        #endregion

        #region//将传入的字符串中间部分字符替换成特殊字符
        /// <summary>
        /// 将传入的字符串中间部分字符替换成特殊字符
        /// </summary>
        /// <param name="value">需要替换的字符串</param>
        /// <param name="startLen">前保留长度</param>
        /// <param name="endLen">尾保留长度</param>
        /// <param name="replaceChar">特殊字符</param>
        /// <returns>被特殊字符替换的字符串</returns>
        public string ReplaceWithSpecialChar(string para_strValue)
        {
            if (LoginFrm.strDisplayLastSix == "0")
            {
                return para_strValue;
            }
            else
            {
                int para_iStartLen;
                int para_iEndLen;
                char para_CpecialChar = '*';
                if (para_strValue.Trim().Length >= 18)//针对身份证
                {
                    para_iStartLen = 8;
                    para_iEndLen = 4;
                }
                else if (para_strValue.Trim().Length >= 11)//针对手机号码
                {
                    para_iStartLen = 3;
                    para_iEndLen = 4;
                }
                else if (para_strValue.Trim().Length >= 9)//针对护照
                {
                    para_iStartLen = 3;
                    para_iEndLen = 4;
                }
                else if (para_strValue.Trim().Length >= 6)//针对其他证件
                {
                    para_iStartLen = 2;
                    para_iEndLen = 2;
                }
                else if (para_strValue.Trim().Length == 5)//针对姓名
                {
                    para_iStartLen = 1;
                    para_iEndLen = 0;
                }
                else if (para_strValue.Trim().Length == 4)//针对姓名
                {
                    para_iStartLen = 1;
                    para_iEndLen = 0;
                }
                else if (para_strValue.Trim().Length == 3)//针对姓名
                {
                    para_iStartLen = 1;
                    para_iEndLen = 0;
                }
                else if (para_strValue.Trim().Length == 2)//针对姓名
                {
                    para_iStartLen = 1;
                    para_iEndLen = 0;
                }
                else//长度小于6，则直接返回原值
                {
                    return para_strValue;
                }

                try
                {
                    int lenth = para_strValue.Length - para_iStartLen - para_iEndLen;
                    string replaceStr = para_strValue.Substring(para_iStartLen, lenth);
                    string specialStr = string.Empty;
                    for (int i = 0; i < replaceStr.Length; i++)
                    {
                        specialStr += para_CpecialChar;
                    }
                    para_strValue = para_strValue.Replace(replaceStr, specialStr);
                }
                catch (Exception)
                {
                    throw;
                }
                return para_strValue;
            }

        }
        #endregion


        public void DisplayInf(string para_strFormName, string para_strRemarks, string para_strDisplayFlag, string para_strOperatorNo)
        {
            strSQL = "select * from T_FormDisplayItemInf where FormName ='" + para_strFormName + "' and OperatorNo='" + para_strOperatorNo + "'";
            if (SQLHelper.DTQuery(strSQL).Rows.Count > 0)
            {
                strSQL = "update T_FormDisplayItemInf set Flag ='" + para_strDisplayFlag + "',Remarks ='" + para_strRemarks + "'  where FormName ='" + para_strFormName + "' and OperatorNo='" + para_strOperatorNo + "'";
                SQLHelper.ExecuteSql(strSQL);
            }
            else
            {
                strSQL = "insert into T_FormDisplayItemInf( FormName,Remarks,Flag,OperatorNo)values('" + para_strFormName + "','" + para_strRemarks + "','" + para_strDisplayFlag + "','" + para_strOperatorNo + "')";
                SQLHelper.ExecuteSql(strSQL);
            }
        }
        private string PolyphoneToLetter(string para_strSQL)
        {

            #region
            if (para_strSQL.Contains("重") == true)
            {
                para_strSQL = para_strSQL.Replace("重", "C");
            }
            else if (para_strSQL.Contains("行") == true)
            {
                para_strSQL = para_strSQL.Replace("行", "H");
            }
            else if (para_strSQL.Contains("睿") == true)
            {
                para_strSQL = para_strSQL.Replace("睿", "R");
            }
            else if (para_strSQL.Contains("苴") == true)
            {
                para_strSQL = para_strSQL.Replace("苴", "J");
            }
            else if (para_strSQL.Contains("邴") == true)
            {
                para_strSQL = para_strSQL.Replace("邴", "B");
            }
            else if (para_strSQL.Contains("犇") == true)
            {
                para_strSQL = para_strSQL.Replace("犇", "B");
            }
            else if (para_strSQL.Contains("鑫") == true)
            {
                para_strSQL = para_strSQL.Replace("鑫", "X");
            }

            else if (para_strSQL.Contains("姣") == true)
            {
                para_strSQL = para_strSQL.Replace("姣", "J");
            }
            else if (para_strSQL.Contains("辰") == true)
            {
                para_strSQL = para_strSQL.Replace("辰", "C");
            }
            else if (para_strSQL.Contains("缪") == true)
            {
                para_strSQL = para_strSQL.Replace("缪", "M");
            }
            else if (para_strSQL.Contains("邬") == true)
            {
                para_strSQL = para_strSQL.Replace("邬", "W");
            }
            else if (para_strSQL.Contains("琪") == true)
            {
                para_strSQL = para_strSQL.Replace("琪", "Q");
            }
            else if (para_strSQL.Contains("倩") == true)
            {
                para_strSQL = para_strSQL.Replace("倩", "Q");
            }
            else if (para_strSQL.Contains("婷") == true)
            {
                para_strSQL = para_strSQL.Replace("婷", "T");
            }
            else if (para_strSQL.Contains("浚") == true)
            {
                para_strSQL = para_strSQL.Replace("浚", "J");
            }
            else if (para_strSQL.Contains("婧") == true)
            {
                para_strSQL = para_strSQL.Replace("婧", "J");
            }
            else if (para_strSQL.Contains("麒") == true)
            {
                para_strSQL = para_strSQL.Replace("麒", "Q");
            }
            else if (para_strSQL.Contains("丞") == true)
            {
                para_strSQL = para_strSQL.Replace("丞", "C");
            }
            else if (para_strSQL.Contains("琦") == true)
            {
                para_strSQL = para_strSQL.Replace("琦", "Q");
            }
            else if (para_strSQL.Contains("宸") == true)
            {
                para_strSQL = para_strSQL.Replace("宸", "C");
            }
            else if (para_strSQL.Contains("宓") == true)
            {
                para_strSQL = para_strSQL.Replace("宓", "M");
            }
            else if (para_strSQL.Contains("琰") == true)
            {
                para_strSQL = para_strSQL.Replace("琰", "Y");
            }
            else if (para_strSQL.Contains("晟") == true)
            {
                para_strSQL = para_strSQL.Replace("晟", "S");
            }
            else if (para_strSQL.Contains("雯") == true)
            {
                para_strSQL = para_strSQL.Replace("雯", "W");
            }
            else if (para_strSQL.Contains("岚") == true)
            {
                para_strSQL = para_strSQL.Replace("岚", "L");
            }


            else if (para_strSQL.Contains("菁") == true)
            {
                para_strSQL = para_strSQL.Replace("菁", "J");
            }
            else if (para_strSQL.Contains("琦") == true)
            {
                para_strSQL = para_strSQL.Replace("琦", "Q");
            }
            else if (para_strSQL.Contains("婷") == true)
            {
                para_strSQL = para_strSQL.Replace("婷", "T");
            }
            else if (para_strSQL.Contains("禺") == true)
            {
                para_strSQL = para_strSQL.Replace("禺", "Y");
            }
            else if (para_strSQL.Contains("钰") == true)
            {
                para_strSQL = para_strSQL.Replace("钰", "Y");
            }
            else if (para_strSQL.Contains("榛") == true)
            {
                para_strSQL = para_strSQL.Replace("榛", "Z");
            }
            else if (para_strSQL.Contains("薇") == true)
            {
                para_strSQL = para_strSQL.Replace("薇", "W");
            }
            else if (para_strSQL.Contains("妃") == true)
            {
                para_strSQL = para_strSQL.Replace("妃", "F");
            }
            else if (para_strSQL.Contains("堃") == true)
            {
                para_strSQL = para_strSQL.Replace("堃", "K");
            }
            else if (para_strSQL.Contains("洮") == true)
            {
                para_strSQL = para_strSQL.Replace("洮", "T");
            }
            else if (para_strSQL.Contains("旻") == true)
            {
                para_strSQL = para_strSQL.Replace("旻", "M");
            }
            else if (para_strSQL.Contains("琛") == true)
            {
                para_strSQL = para_strSQL.Replace("琛", "C");
            }
            else if (para_strSQL.Contains("钿") == true)
            {
                para_strSQL = para_strSQL.Replace("钿", "D");
            }
            else if (para_strSQL.Contains("郦") == true)
            {
                para_strSQL = para_strSQL.Replace("郦", "L");
            }
            else if (para_strSQL.Contains("璇") == true)
            {
                para_strSQL = para_strSQL.Replace("璇", "X");
            }
            else if (para_strSQL.Contains("澄") == true)
            {
                para_strSQL = para_strSQL.Replace("澄", "C");
            }
            else if (para_strSQL.Contains("尧") == true)
            {
                para_strSQL = para_strSQL.Replace("尧", "Y");
            }
            else if (para_strSQL.Contains("乾") == true)
            {
                para_strSQL = para_strSQL.Replace("乾", "Q");
            }
            else if (para_strSQL.Contains("瀚") == true)
            {
                para_strSQL = para_strSQL.Replace("瀚", "H");
            }
            else if (para_strSQL.Contains("皓") == true)
            {
                para_strSQL = para_strSQL.Replace("皓", "H");
            }
            else if (para_strSQL.Contains("瞿") == true)
            {
                para_strSQL = para_strSQL.Replace("瞿", "Q");
            }
            else if (para_strSQL.Contains("翟") == true)
            {
                para_strSQL = para_strSQL.Replace("翟", "Z");
            }
            else if (para_strSQL.Contains("於") == true)
            {
                para_strSQL = para_strSQL.Replace("於", "Y");
            }
            else if (para_strSQL.Contains("珺") == true)
            {
                para_strSQL = para_strSQL.Replace("珺", "J");
            }

            else if (para_strSQL.Contains("瑜") == true)
            {
                para_strSQL = para_strSQL.Replace("瑜", "Y");
            }
            else if (para_strSQL.Contains("滔") == true)
            {
                para_strSQL = para_strSQL.Replace("滔", "T");
            }
            else if (para_strSQL.Contains("麒") == true)
            {
                para_strSQL = para_strSQL.Replace("麒", "Q");
            }
            else if (para_strSQL.Contains("钏") == true)
            {
                para_strSQL = para_strSQL.Replace("钏", "C");
            }
            else if (para_strSQL.Contains("霆") == true)
            {
                para_strSQL = para_strSQL.Replace("霆", "T");
            }
            else if (para_strSQL.Contains("舜") == true)
            {
                para_strSQL = para_strSQL.Replace("舜", "S");
            }
            else if (para_strSQL.Contains("珈") == true)
            {
                para_strSQL = para_strSQL.Replace("珈", "J");
            }
            else if (para_strSQL.Contains("淦") == true)
            {
                para_strSQL = para_strSQL.Replace("淦", "G");
            }
            else if (para_strSQL.Contains("淳") == true)
            {
                para_strSQL = para_strSQL.Replace("淳", "C");
            }
            else if (para_strSQL.Contains("玟") == true)
            {
                para_strSQL = para_strSQL.Replace("玟", "W");
            }
            else if (para_strSQL.Contains("兮") == true)
            {
                para_strSQL = para_strSQL.Replace("兮", "X");
            }
            else if (para_strSQL.Contains("梵") == true)
            {
                para_strSQL = para_strSQL.Replace("梵", "F");
            }
            else if (para_strSQL.Contains("浚") == true)
            {
                para_strSQL = para_strSQL.Replace("浚", "J");
            }
            else if (para_strSQL.Contains("荃") == true)
            {
                para_strSQL = para_strSQL.Replace("荃", "Q");
            }
            else if (para_strSQL.Contains("韬") == true)
            {
                para_strSQL = para_strSQL.Replace("韬", "T");
            }
            else if (para_strSQL.Contains("桢") == true)
            {
                para_strSQL = para_strSQL.Replace("桢", "Z");
            }


            else if (para_strSQL.Contains("廖") == true)
            {
                para_strSQL = para_strSQL.Replace("廖", "L");
            }
            else if (para_strSQL.Contains("嵘") == true)
            {
                para_strSQL = para_strSQL.Replace("嵘", "R");
            }
            else if (para_strSQL.Contains("侗") == true)
            {
                para_strSQL = para_strSQL.Replace("侗", "T");
            }
            else if (para_strSQL.Contains("囡") == true)
            {
                para_strSQL = para_strSQL.Replace("囡", "N");
            }
            else if (para_strSQL.Contains("茗") == true)
            {
                para_strSQL = para_strSQL.Replace("茗", "M");
            }
            else if (para_strSQL.Contains("怡") == true)
            {
                para_strSQL = para_strSQL.Replace("怡", "Y");
            }
            else if (para_strSQL.Contains("菱") == true)
            {
                para_strSQL = para_strSQL.Replace("菱", "L");
            }

            else if (para_strSQL.Contains("炜") == true)
            {
                para_strSQL = para_strSQL.Replace("炜", "W");
            }
            else if (para_strSQL.Contains("煊") == true)
            {
                para_strSQL = para_strSQL.Replace("煊", "X");
            }
            else if (para_strSQL.Contains("瑾") == true)
            {
                para_strSQL = para_strSQL.Replace("瑾", "J");
            }
            else if (para_strSQL.Contains("濠") == true)
            {
                para_strSQL = para_strSQL.Replace("濠", "J");
            }

            else if (para_strSQL.Contains("垚") == true)
            {
                para_strSQL = para_strSQL.Replace("垚", "Y");
            }
            else if (para_strSQL.Contains("桧") == true)
            {
                para_strSQL = para_strSQL.Replace("桧", "H");
            }
            else if (para_strSQL.Contains("茜") == true)
            {
                para_strSQL = para_strSQL.Replace("茜", "Q");
            }
            else if (para_strSQL.Contains("璐") == true)
            {
                para_strSQL = para_strSQL.Replace("璐", "L");
            }

            else if (para_strSQL.Contains("臧") == true)
            {
                para_strSQL = para_strSQL.Replace("臧", "Z");
            }
            else if (para_strSQL.Contains("锴") == true)
            {
                para_strSQL = para_strSQL.Replace("锴", "K");
            }
            else if (para_strSQL.Contains("垲") == true)
            {
                para_strSQL = para_strSQL.Replace("垲", "K");
            }
            else if (para_strSQL.Contains("晗") == true)
            {
                para_strSQL = para_strSQL.Replace("晗", "H");
            }
            else if (para_strSQL.Contains("斐") == true)
            {
                para_strSQL = para_strSQL.Replace("斐", "F");
            }
       

            else if (para_strSQL.Contains("颉") == true)
            {
                para_strSQL = para_strSQL.Replace("颉", "J");
            }
            else if (para_strSQL.Contains("铟") == true)
            {
                para_strSQL = para_strSQL.Replace("铟", "Y");
            }
            else if (para_strSQL.Contains("晏") == true)
            {
                para_strSQL = para_strSQL.Replace("晏", "Y");
            }
            else if (para_strSQL.Contains("昱") == true)
            {
                para_strSQL = para_strSQL.Replace("昱", "Y");
            }
            else if (para_strSQL.Contains("炜") == true)
            {
                para_strSQL = para_strSQL.Replace("炜", "W");
            }
            else if (para_strSQL.Contains("焱") == true)
            {
                para_strSQL = para_strSQL.Replace("焱", "Y");
            }
            else if (para_strSQL.Contains("璐") == true)
            {
                para_strSQL = para_strSQL.Replace("璐", "L");
            }
            else if (para_strSQL.Contains("熠") == true)
            {
                para_strSQL = para_strSQL.Replace("熠", "Y");
            }
            else if (para_strSQL.Contains("昊") == true)
            {
                para_strSQL = para_strSQL.Replace("昊", "H");
            }
            else if (para_strSQL.Contains("媛") == true)
            {
                para_strSQL = para_strSQL.Replace("媛", "Y");
            }
            else if (para_strSQL.Contains("偲") == true)
            {
                para_strSQL = para_strSQL.Replace("偲", "S");
            }
            else if (para_strSQL.Contains("婕") == true)
            {
                para_strSQL = para_strSQL.Replace("婕", "J");
            }
            else if (para_strSQL.Contains("宓") == true)
            {
                para_strSQL = para_strSQL.Replace("宓", "M");
            }
            else if (para_strSQL.Contains("锫") == true)
            {
                para_strSQL = para_strSQL.Replace("锫", "P");
            }
            else if (para_strSQL.Contains("荥") == true)
            {
                para_strSQL = para_strSQL.Replace("荥", "Y");
            }
            else if (para_strSQL.Contains("苓") == true)
            {
                para_strSQL = para_strSQL.Replace("苓", "L");
            }
            else if (para_strSQL.Contains("楠") == true)
            {
                para_strSQL = para_strSQL.Replace("楠", "N");
            }
            else if (para_strSQL.Contains("覃") == true)
            {
                para_strSQL = para_strSQL.Replace("覃", "T");
            }
            else if (para_strSQL.Contains("铄") == true)
            {
                para_strSQL = para_strSQL.Replace("铄", "S");
            }
            else if (para_strSQL.Contains("嫄") == true)
            {
                para_strSQL = para_strSQL.Replace("嫄", "Y");
            }
       


            else if (para_strSQL.Contains("滢") == true)
            {
                para_strSQL = para_strSQL.Replace("滢", "Y");
            }

            else if (para_strSQL.Contains("馨") == true)
            {
                para_strSQL = para_strSQL.Replace("馨", "X");
            }

            else if (para_strSQL.Contains("煜") == true)
            {
                para_strSQL = para_strSQL.Replace("煜", "Y");
            }

            else if (para_strSQL.Contains("懿") == true)
            {
                para_strSQL = para_strSQL.Replace("懿", "Y");
            }

            else if (para_strSQL.Contains("烨") == true)
            {
                para_strSQL = para_strSQL.Replace("烨", "Y");
            }

            else if (para_strSQL.Contains("奕") == true)
            {
                para_strSQL = para_strSQL.Replace("奕", "Y");
            }

            else if (para_strSQL.Contains("暨") == true)
            {
                para_strSQL = para_strSQL.Replace("暨", "J");
            }

            else if (para_strSQL.Contains("汶") == true)
            {
                para_strSQL = para_strSQL.Replace("汶", "W");
            }

            else if (para_strSQL.Contains("臻") == true)
            {
                para_strSQL = para_strSQL.Replace("臻", "Z");
            }

            else if (para_strSQL.Contains("淇") == true)
            {
                para_strSQL = para_strSQL.Replace("淇", "Q");
            }

            else if (para_strSQL.Contains("莞") == true)
            {
                para_strSQL = para_strSQL.Replace("莞", "G");
            }

            else if (para_strSQL.Contains("淼") == true)
            {
                para_strSQL = para_strSQL.Replace("淼", "M");
            }

            else if (para_strSQL.Contains("臧") == true)
            {
                para_strSQL = para_strSQL.Replace("臧", "Z");
            }

            else if (para_strSQL.Contains("慜") == true)
            {
                para_strSQL = para_strSQL.Replace("慜", "M");
            }

            else if (para_strSQL.Contains("姵") == true)
            {
                para_strSQL = para_strSQL.Replace("姵", "P");
            }

            else if (para_strSQL.Contains("鋆") == true)
            {
                para_strSQL = para_strSQL.Replace("鋆", "Y");
            }

            else if (para_strSQL.Contains("裘") == true)
            {
                para_strSQL = para_strSQL.Replace("裘", "Q");
            }

            else if (para_strSQL.Contains("嫄") == true)
            {
                para_strSQL = para_strSQL.Replace("嫄", "Y");
            }

            else if (para_strSQL.Contains("泓") == true)
            {
                para_strSQL = para_strSQL.Replace("泓", "H");
            }

            else if (para_strSQL.Contains("晖") == true)
            {
                para_strSQL = para_strSQL.Replace("晖", "H");
            }






            else if (para_strSQL.Contains("蛟") == true)
            {
                para_strSQL = para_strSQL.Replace("蛟", "J");
            }


            else if (para_strSQL.Contains("烔") == true)
            {
                para_strSQL = para_strSQL.Replace("烔", "T");
            }


            else if (para_strSQL.Contains("庄") == true)
            {
                para_strSQL = para_strSQL.Replace("庄", "Z");
            }


            else if (para_strSQL.Contains("潇") == true)
            {
                para_strSQL = para_strSQL.Replace("潇", "X");
            }


            else if (para_strSQL.Contains("镪") == true)
            {
                para_strSQL = para_strSQL.Replace("镪", "Q");
            }


            else if (para_strSQL.Contains("灏") == true)
            {
                para_strSQL = para_strSQL.Replace("灏", "H");
            }

            #endregion


            else if (para_strSQL.Contains("晖") == true)
            {
                para_strSQL = para_strSQL.Replace("晖", "H");
            }


            else if (para_strSQL.Contains("晖") == true)
            {
                para_strSQL = para_strSQL.Replace("晖", "H");
            }


            else if (para_strSQL.Contains("晖") == true)
            {
                para_strSQL = para_strSQL.Replace("晖", "H");
            }


            else if (para_strSQL.Contains("晖") == true)
            {
                para_strSQL = para_strSQL.Replace("晖", "H");
            }


            else if (para_strSQL.Contains("晖") == true)
            {
                para_strSQL = para_strSQL.Replace("晖", "H");
            }




            return para_strSQL;
        }
 
        private string MCode(string para_strSQL)
        {
            strMCode = "";
            try
            {
                strMCode = SQLHelper.HzToPy_SZM(para_strSQL.Trim ()).ToUpper();
                return strMCode;
            }
            catch
            {
                if (para_strSQL != "")
                {
                    para_strSQL = para_strSQL.Substring(0, para_strSQL.Length - 1);
                    MCode(para_strSQL);
                    return strMCode;
                }
                return strMCode;
            }
        }

        public string MCodeAll(string para_strSQL)
        {

            try
            {
                para_strSQL = para_strSQL.Trim();
                string str = "";
                string strCol = "";
                //strMCode = SQLHelper.GetFistCharacter(para_strSQL);
                //strSQL = strMCode.Trim();
                // strMCode = SQLHelper.GetFistCharacter(para_strSQL);
                para_strSQL = PolyphoneToLetter(para_strSQL);
                if (para_strSQL.Trim().Contains("\0") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("\0", "");
                }
                if (para_strSQL.Trim().Contains("/0") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("/0", "");
                }
                if (para_strSQL.Trim().Contains("'") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("'", "");
                }
                if (para_strSQL.Trim().Contains(":") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace(":", "");
                }
                if (para_strSQL.Trim().Contains("、") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("、", "");
                }
                if (para_strSQL.Trim().Contains(",") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace(",", "");
                }
                if (para_strSQL.Trim().Contains("：") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("：", "");
                }
                if (para_strSQL.Trim().Contains("“") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("“", "");
                }
                if (para_strSQL.Trim().Contains("”") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("”", "");
                }
                if (para_strSQL.Trim().Contains("、") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("、", "");
                }
                if (para_strSQL.Trim().Contains("，") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("，", "");
                }
                if (para_strSQL.Trim().Contains("（") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("（", "");
                }
                if (para_strSQL.Trim().Contains("）") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("）", "");
                }
                if (para_strSQL.Trim().Contains("\\") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("\\", "");
                }
                if (para_strSQL.Trim().Contains("/") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("/", "");
                }
                if (para_strSQL.Trim().Contains("<") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("<", "");
                }
                if (para_strSQL.Trim().Contains(">") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace(">", "");
                }
                if (para_strSQL.Trim().Contains("(") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("(", "");
                }
                if (para_strSQL.Trim().Contains(")") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace(")", "");
                }
                if (para_strSQL.Trim().Contains("《") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("《", "");
                }
                if (para_strSQL.Trim().Contains("》") == true)
                {
                    para_strSQL = para_strSQL.Trim().Replace("》", "");
                }
                strMCode = para_strSQL.Trim();
                strSQL = strMCode;
                for (int m = 0; m < strSQL.Length; m++)
                {
                    str = strSQL.Substring(m, 1).Trim();
                    for (int i = 0; i < SQLHelper.AllNameRareWord.GetLength(0); i++)//得到二维数组的长度
                    {
                        strCol = SQLHelper.AllNameRareWord[i, 1].Trim();
                        for (int j = 0; j < strCol.Length; j++)//得到二维数组的宽度
                        {
                            if (strCol.Substring(j, 1).Trim() == str)
                            {
                                strMCode = strMCode.Replace(str, SQLHelper.AllNameRareWord[i, 0].ToString().Trim());
                                break;
                            }
                        }
                    }
                }
                strMCode = SQLHelper.GetFistCharacter(strMCode);
                return strMCode;
            }
            catch
            {
                return strMCode;
            }
        }

        #region//LoginFrm_Configuration
        public string CreateDBTable(string para_strAccout, string para_strConn, string para_strPath, string para_strAControllerNo, string para_strAControllerName,
            string para_strOActualNo, string para_strNewAccoutNo, string para_strAccoutName, string para_strEndUserName, string para_strAccessPwd, 
            string para_strGuardRoomId, string para_strGuardRoomName, string para_strServerName, string para_strDDIdConfirm, string para_strDLoginName,
            string para_strDLoginPwd, string para_strCardCom_IC, string para_strCardCom_ID, string para_strSMSCatCom,int para_iFlag_LockEncryption, 
            string para_strRubbishClear, string para_strServerName_Remote, string para_strDDIdConfirm_Remote, string para_strDLoginName_Remote, 
            string para_strDLoginPwd_Remote)
        {
            bool blXXCLOUDIsExist = false;//判断XXCLOUD数据库是否存在 false:不存在  true:存在
            try
            {
                DataTable table = new DataTable();
                try
                {
                    strSQL = "select * from XXCLOUD.DBO.T_OperatorInf ";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count >= 0)
                    {
                        blXXCLOUDIsExist = true;
                    }
                }
                catch
                {
                    blXXCLOUDIsExist = false;
                }
                bool blXXCLOUDDLLANoIsExist = false;//判断XXCLOUDDLLANo数据库是否存在,XXCLOUDDLLANo数据库是即将要创建的数据库 false:不存在  true:存在.
                try
                {
                    strSQL = "select * from YEAR" + para_strNewAccoutNo + ".DBO.T_VisitorAccessInf ";
                    if (SQLHelper.DTQuery(this.strSQL).Rows.Count >= 0)
                    {
                        blXXCLOUDDLLANoIsExist = true;
                    }
                }
                catch
                {
                    blXXCLOUDDLLANoIsExist = false;
                }
                if (blXXCLOUDDLLANoIsExist == true)
                {
                    return ("编号为 [" + para_strNewAccoutNo + "] 的年份已经存在，请重新输入年份编号并新建!");
                }
                OperatorNo(ref para_strAControllerNo, "XXCLOUD.DBO.T_OperatorInf", "ONo", "0");
                OperatorActualNo(ref para_strOActualNo, "XXCLOUD.DBO.T_OperatorInf", "OActualNo", "0");
                strConn = para_strConn;
                if (blXXCLOUDIsExist == false)//不存在，则创建XXCLOUD数据库
                {
                    ExecuteSql("master", "Create Database  XXCLOUD");
                    ExecuteSql("XXCLOUD", GetSql(para_strPath, "XXCLOUD.txt"));
                }
                ExecuteSql("XXCLOUD", GetSql_AccoutInfAndOperatorInf(para_strPath, para_strAControllerNo, para_strAControllerName, para_strOActualNo, para_strNewAccoutNo, para_strAccoutName, para_strEndUserName, "T_OperatorInf.txt"));
                ExecuteSql("master", "Create Database  " + para_strAccout);
                ExecuteSql(para_strAccout, GetSql(para_strPath, "YEAR.txt"));

                try
                {
                    //这个sp_addlogin语句的主要作用是为新数据库创建一个登录(可以登录SQL数据库，在安全性结点下面的登录名里删除para_strDLoginName名，再执行新建，则会提示正常)。
                    //如果SQL数据库中已经存在para_strDLoginName登录名，则会报错：服务器主体para_strDLoginName已存在。
                    //如果没有，则这条语句会正常执行。
                    ExecuteSql("master", "exec sp_addlogin '" + para_strDLoginName + "','" + para_strDLoginPwd + "','" + para_strAccout + "',Null,Null");
                }
                catch
                {

                }
                try
                {
                    //这个sp_grantdbaccess语句的主要作用是为刚才这个登录指定数据库访问权限
                    //如果SQL数据库中已经存在para_strDLoginName登录名，则会报错：服务器主体para_strDLoginName已存在。
                    //如果没有，则这条语句会正常执行。
                    ExecuteSql(para_strAccout, "exec sp_grantdbaccess  '" + para_strDLoginName + "', '" + para_strDLoginPwd + "'");
                }
                catch
                {

                }
                try
                {
                    //这个sp_addrolemember的主要作用是指定myoamaster有db_owner的权限
                    //如果SQL数据库中已经存在para_strDLoginName登录名，则会报错：服务器主体para_strDLoginName已存在。
                    //如果没有，则这条语句会正常执行。
                    ExecuteSql(para_strAccout, "exec sp_addrolemember  'db_owner','" + para_strDLoginName + "'");
                }
                catch
                {

                }
            }
            catch 
            {
                
            }
            InsertIntoAccess(para_strPath, para_strNewAccoutNo, para_strAccoutName, para_strAControllerName, para_strAControllerNo,
                     para_strAccessPwd, para_strGuardRoomId, para_strGuardRoomName, para_strServerName, para_strDDIdConfirm,
                     para_strDLoginName, para_strDLoginPwd, para_strCardCom_IC, para_strCardCom_ID,para_strSMSCatCom, para_iFlag_LockEncryption,
                     para_strRubbishClear, para_strServerName_Remote, para_strDDIdConfirm_Remote, para_strDLoginName_Remote, para_strDLoginPwd_Remote);
            return "1";
        }

        public string InsertIntoAccess(string para_strPath, string para_strNewAccoutNo, string para_strAccoutName, string para_strAControllerName,
          string para_strAControllerNo, string para_strAccessPwd, string para_strGuardRoomId, string para_strGuardRoomName, string para_strServerName,
          string para_strDDIdConfirm, string para_strDLoginName, string para_strDLoginPwd, string para_strCardCom_IC, string para_strCardCom_ID,
         string para_strSMSCatCom, int para_iFlag_LockEncryption, string para_strRubbishClear,
          string para_strServerName_Remote, string para_strDDIdConfirm_Remote, string para_strDLoginName_Remote, string para_strDLoginPwd_Remote)
        {
            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + para_strPath + @"\XXCLOUD.mdb;Jet OLEDB:Database Password='" + para_strAccessPwd + "'");
            strSQL = "insert into T_LocYearInf(ANo,AName )Values('" + para_strNewAccoutNo + "','" + para_strAccoutName + "'  )";
            OleDbCommand command = new OleDbCommand(strSQL, connection);
            connection.Open();
            if (command.ExecuteNonQuery() != 0)
            {
                if (T_OperatorInfISEXIST(para_strAControllerNo, para_strAControllerName, para_strPath, para_strAccessPwd) == false)
                {
                    strSQL = "insert into T_LocOperatorInf( ONo,OName)Values('" + para_strAControllerNo + "','" + para_strAControllerName + "')";
                    new OleDbCommand(strSQL, connection).ExecuteNonQuery();
                }
                if (FWQSZ(para_strPath, para_strGuardRoomId, para_strGuardRoomName, para_strAccessPwd, para_strServerName, para_strDDIdConfirm, para_strDLoginName, para_strDLoginPwd, para_strCardCom_IC, para_strCardCom_ID,para_strSMSCatCom, para_iFlag_LockEncryption, para_strRubbishClear, para_strServerName_Remote, para_strDDIdConfirm_Remote, para_strDLoginName_Remote, para_strDLoginPwd_Remote))
                {
                    return ("名称为 [" + para_strNewAccoutNo + "]" + para_strAccoutName + " 的年份新建成功!");
                }
            }
            return "";
        }

        public bool T_OperatorInfISEXIST(string para_strONo, string para_strOName, string para_strPath, string para_strAccessPwd)
        {
            DataSet DS = new DataSet();
            strSQL = "select distinct ONo from T_LocOperatorInf  where ONo='" + para_strONo + "' and OName='" + para_strOName + "'";
            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + para_strPath + @"\XXCLOUD.mdb;Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
            OleDbConnection selectConnection = new OleDbConnection(strConn);
            new OleDbDataAdapter(strSQL, selectConnection).Fill(DS);
            return (DS.Tables[0].Rows.Count > 0);
        }

        public void OperatorNo(ref string para_strAControllerNo, string para_strDTableName, string para_strDisplayOrder, string para_strFlag)
        {
            if (para_strFlag == "0")
            {
                try
                {
                    para_strAControllerNo = "001"; ;
                    myTable = SQLHelper.DTQuery(" select ONo from " + para_strDTableName + " order by   " + para_strDisplayOrder);
                    if (myTable.Rows.Count <= 0)
                    {
                        para_strAControllerNo = "001";
                    }
                    else if (myTable.Rows[myTable.Rows.Count - 1]["ONo"].ToString().Trim() != "")
                    {
                        int num = Convert.ToInt16(myTable.Rows[myTable.Rows.Count - 1]["ONo"].ToString()) + 1;
                        switch (num.ToString().Length)
                        {
                            case 1:
                                strSQL = "00" + num.ToString();
                                break;

                            case 2:
                                strSQL = "0" + num.ToString();
                                break;

                            default:
                                strSQL = num.ToString();
                                break;
                        }
                        para_strAControllerNo = strSQL;
                        if (strSQL == "")
                        {
                            para_strAControllerNo = "001";
                        }
                    }
                    else
                    {
                        para_strAControllerNo = "001";
                    }
                }
                catch
                {
                    para_strAControllerNo = "001";
                }
            }
        }

        public void OperatorActualNo(ref string para_strOActualNo, string para_strDTableName, string para_strDisplayOrder, string para_strFlag)
        {
            if (para_strFlag == "0")
            {
                try
                {
                    myTable = SQLHelper.DTQuery(" select OActualNo from " + para_strDTableName + " order by  " + para_strDisplayOrder);
                    if (myTable.Rows.Count <= 0)
                    {
                        para_strOActualNo = "001";
                    }
                    else if (myTable.Rows[myTable.Rows.Count - 1]["OActualNo"].ToString().Trim() != "")
                    {
                        int iOActualNoNew = Convert.ToInt16(myTable.Rows[myTable.Rows.Count - 1]["OActualNo"].ToString()) + 1;
                        switch (iOActualNoNew.ToString().Length)
                        {
                            case 1:
                                strSQL = "00" + iOActualNoNew.ToString();
                                break;

                            case 2:
                                strSQL = "0" + iOActualNoNew.ToString();
                                break;

                            default:
                                strSQL = iOActualNoNew.ToString();
                                break;
                        }
                        para_strOActualNo = strSQL.Trim();
                    }
                    else
                    {
                        para_strOActualNo = "001";
                    }
                }
                catch
                {
                    para_strOActualNo = "001";
                }
            }
        }

        private void ExecuteSql(string para_strDatabaseName, string para_strSQL)
        {
            SqlConnection connection = new SqlConnection(strConn);
            SqlCommand command = new SqlCommand(para_strSQL, connection);
            command.Connection.Open();
            command.Connection.ChangeDatabase(para_strDatabaseName);
            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                command.Connection.Close();
            }
        }

        public bool FWQSZ(string para_strPath, string para_strGuardRoomId, string para_strGuardRoomName, string para_strAccessPwd, string para_strServerName, string para_strDDIdConfirm, string para_strDLoginName, string para_strDLoginPwd, string para_strCardCom_IC, string para_strCardCom_ID,string para_strSMSCatCom, int para_iFlag_LockEncryption, string para_strRubbishClear, string para_strServerName_Remote, string para_strDDIdConfirm_Remote, string para_strDLoginName_Remote, string para_strDLoginPwd_Remote)
        {
            try
            {
                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + para_strPath + @"\XXCLOUD.mdb;Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
                strSQL = "delete from T_LocConfigurationInf ";
                OleDbConnection connection = new OleDbConnection(connectionString);
                OleDbCommand command = new OleDbCommand(strSQL, connection);
                connection.Open();
                command.ExecuteNonQuery();
                string strA5 = SQLHelper.EncryptString(para_strGuardRoomId + "A5");
                string strA6 = SQLHelper.EncryptString(para_strGuardRoomName + "A6");
                string strA7 = "0";
                strSQL = " insert into T_LocConfigurationInf(A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A15,A16,A17,A18)Values('" + SQLHelper.EncryptString(para_strServerName + "A1") + "',";
                strSQL += "'" + SQLHelper.EncryptString(para_strDDIdConfirm + "A2") + "','" + SQLHelper.EncryptString(para_strDLoginName + "A3") + "','" + para_strDLoginPwd + "',";
                strSQL += "'" + strA5 + "','" + strA6 + "','" + strA7 + "','" + para_strCardCom_IC.Trim() + "','" + para_strCardCom_ID.Trim() + "','" + para_iFlag_LockEncryption + "','" + para_strRubbishClear + "',";
                strSQL += "'" + para_strSMSCatCom + "','" + para_strServerName_Remote + "','" + para_strDDIdConfirm_Remote + "','" + para_strDLoginName_Remote + "','" + para_strDLoginPwd_Remote + "')";
                new OleDbCommand(strSQL, connection).ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetSql(string para_strPath, string para_strJBName)
        {
            try
            {
                string path = para_strPath + @"\" + para_strJBName;
                if (File.Exists(path))
                {
                    StreamReader reader = new StreamReader(path);
                    return reader.ReadToEnd().Replace("\r\n", " ").Replace(" GO ", " ; ");
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private string GetSql_AccoutInfAndOperatorInf(string para_strPath, string para_strAControllerNo, string para_strAControllerName, string para_strOActualNo, 
            string para_strNewAccoutNo, string para_strAccoutName, string para_strEndUserName, string strName)
        {
            try
            {
                string path = para_strPath + @"\" + strName;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                string strSQLNew = "use XXCLOUD ";
                try
                {
                    strSQL = "select * from XXCLOUD.DBO.T_OperatorInf where ONo='" + para_strAControllerNo + "' and OName ='" + para_strAControllerName + "'";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count <= 0)
                    {
                        strSQLNew += "  insert into T_OperatorInf(ONo,OActualNo,OName,OPassword,OCardNo,OIdNo,Power)Values('" + para_strAControllerNo + "',";
                        strSQLNew += "'" + para_strOActualNo + "','" + para_strAControllerName + "','','','', '1') ";
                    }
                }
                catch
                {
                    strSQLNew += "  insert into T_OperatorInf(ONo,OActualNo,OName,OPassword,OCardNo,OIdNo,Power)Values('" + para_strAControllerNo + "',";
                    strSQLNew += "'" + para_strOActualNo + "','" + para_strAControllerName + "','','','', '1') ";
                }
                try
                {
                    strSQL = "select * from XXCLOUD.DBO.T_YearInf where ANo='" + para_strNewAccoutNo + "' and EndUserName='" + para_strEndUserName + "'";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count <= 0)
                    {
                        strSQLNew += "  insert into T_YearInf(ANo,AName,EndUserName,AControllerNo,AControllerName,IsPrintProvider,ASetings)";
                        strSQLNew += " Values('" + para_strNewAccoutNo + "','" + para_strAccoutName + "', '" + para_strEndUserName + "','" + para_strAControllerNo + "',";
                        strSQLNew += "'" + para_strAControllerName + "','0','" + "2,1,1,1,1,100,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,1,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,3,1,0,0,0,0,0,0,0,0,0,0,0," + "')";
                    }
                }
                catch
                {
                    strSQLNew += "  insert into T_YearInf(ANo,AName,EndUserName,AControllerNo,AControllerName)Values('" + para_strNewAccoutNo + "','" + para_strAccoutName + "',";
                    strSQLNew += "'" + para_strEndUserName + "','" + para_strAControllerNo + "','" + para_strAControllerName + "')";
                }
                try
                {
                    strSQL = "select * from XXCLOUD.DBO.T_BaseCategroyInf where   Flag ='" + "本软件使用单位" + "'";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count <= 0)
                    {
                        strSQLNew += "  insert into XXCLOUD.DBO.T_BaseCategroyInf(BNo,BSNo,BClassName,Flag)";
                        strSQLNew += "  Values('" + "001" + "','" + "1" + "','" + para_strEndUserName + "', '" + "本软件使用单位" + "');";
                        strSQLNew += "  delete from XXCLOUD.DBO.T_GlobalSettingInf  where Flag='" + "本软件使用单位" + "';";
                        strSQLNew += "  insert into XXCLOUD.DBO.T_GlobalSettingInf(GNo,GName,Flag)";
                        strSQLNew += "  Values('" + "001" + "','" + para_strEndUserName + "', '" + "本软件使用单位" + "');";
                    }
                }
                catch
                {
                    
                }
                StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode, 0x800);
                writer.WriteLine(strSQLNew);
                writer.Close();
                strSQL = para_strPath + @"\" + strName;
                StreamReader reader = new StreamReader(strSQL);
                return reader.ReadToEnd();
            }
            catch
            {

                return null;
            }
        }

        public string AddAccoutInfToLoc(string para_strANo, ref string para_strAccoutName, string para_strPath, string para_strAccessPwd, string para_strGuardRoomId, 
            string para_strGuardRoomName, string para_strServerName, string para_strDDIdConfirm, string para_strDLoginName, string para_strDLoginPwd, 
            string para_strCardCom_IC, string para_strCardCom_ID,string para_strSMSCatCom,int para_iFlag_LockEncryption, string para_strRubbishClear, 
            string para_strServerName_Remote, string para_strDDIdConfirm_Remote, string para_strDLoginName_Remote, string para_strDLoginPwd_Remote)
        {
            try
            {
                strSQL = "select * from XXCLOUD.DBO.T_YearInf where ANo='" + para_strANo + "'";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DataSet dataSet = new DataSet();
                    strSQL = "select * from T_LocYearInf where ANo='" + myTable.Rows[0]["ANo"].ToString().Trim() + "' and AName='" + myTable.Rows[0]["AName"].ToString().Trim() + "'  ";
                    OleDbConnection selectConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + para_strPath + @"\XXCLOUD.mdb;Jet OLEDB:Database Password='" + para_strAccessPwd + "'");
                    new OleDbDataAdapter(strSQL, selectConnection).Fill(dataSet);
                    if (dataSet.Tables[0].Copy().Rows.Count <= 0)
                    {
                        strSQL = "insert into T_LocYearInf(ANo,AName )Values('" + myTable.Rows[0]["ANo"].ToString().Trim() + "', '" + myTable.Rows[0]["AName"].ToString().Trim() + "')";
                        OleDbCommand command = new OleDbCommand(strSQL, selectConnection);
                        selectConnection.Open();
                        if (command.ExecuteNonQuery() != 0)
                        {
                            if (!T_OperatorInfISEXIST(myTable.Rows[0]["AControllerNo"].ToString().Trim(), myTable.Rows[0]["AControllerName"].ToString().Trim(), para_strPath, para_strAccessPwd))
                            {
                                strSQL = "insert into T_LocOperatorInf(ONo,OName)Values('" + myTable.Rows[0]["AControllerNo"].ToString().Trim() + "','" + myTable.Rows[0]["AControllerName"].ToString().Trim() + "')";
                                new OleDbCommand(strSQL, selectConnection).ExecuteNonQuery();
                            }
                            if (FWQSZ(para_strPath, para_strGuardRoomId, para_strGuardRoomName, para_strAccessPwd, para_strServerName, para_strDDIdConfirm, para_strDLoginName, para_strDLoginPwd, para_strCardCom_IC, para_strCardCom_ID,para_strSMSCatCom, para_iFlag_LockEncryption, para_strRubbishClear, para_strServerName_Remote, para_strDDIdConfirm_Remote, para_strDLoginName_Remote, para_strDLoginPwd_Remote))
                            {
                                para_strAccoutName = myTable.Rows[0]["AName"].ToString().Trim();
                                return ("名称为 [" + myTable.Rows[0]["ANo"].ToString().Trim() + "]" + myTable.Rows[0]["AName"].ToString().Trim() + " 的年份加入成功!");
                            }
                            return "3";
                        }
                        return "2";
                    }
                    para_strAccoutName = myTable.Rows[0]["AName"].ToString().Trim();
                    return "0";
                }
                return "1";
            }
            catch
            {
                return "2";
            }
        }

        #endregion

        #region//LoginFrm
        #region//检测猩猩云访客V2019是否已经在运行
        public int LoginFrm_iPromgramIsRun()
        {
            string strModuleName = Process.GetCurrentProcess().MainModule.ModuleName;
            string strProcessName = System.IO.Path.GetFileNameWithoutExtension(strModuleName);
            Process[] processes = Process.GetProcessesByName(strProcessName);
            return processes.Length;
        }
        #endregion

        #region//检测网络通信助手是否已经在运行
        public int LoginFrm_iNCAPromgramIsRun()
        {
            strSQL = "网络通信助手";
            int iCount = 0;
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.ToString().Trim().Contains(strSQL.ToLower()))
                {
                    iCount++;
                }
            }
            return iCount;
        }
        #endregion

        #region//检测网络锁服务是否已经在运行
        public int LoginFrm_iDogServerPromgramIsRun()
        {
            strSQL = "DogServer";
            int iCount = 0;
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.ToString().Trim().ToLower().Contains(strSQL.ToLower()))
                {
                    iCount++;
                }
            }
            return iCount;
        }
        #endregion

        #region//加载本地的年份档案表(Access文件)
        public DataSet LoginFrm_LoadLocalAccoutInf(string para_strPath, string para_strAccessPwd)
        {
            DataSet DS = new DataSet();
            strSQL = "select distinct ANo,AName from T_LocYearInf order by ANo ";
            OleDbConnection selectConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + para_strPath + @"\XXCLOUD.mdb;Jet OLEDB:Database Password='" + para_strAccessPwd + "'");
            new OleDbDataAdapter(strSQL, selectConnection).Fill(DS);
            return DS;
        }
        #endregion

        #region//加载本地的配置档案表(Access文件)
        public DataSet LoginFrm_LoadLocalConfigurationInf(string para_strPath, string para_strAccessPwd)
        {
            DataSet DS = new DataSet();
            strSQL = "select * from T_LocConfigurationInf ";
            OleDbConnection selectConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + para_strPath + @"\XXCLOUD.mdb;Jet OLEDB:Database Password='" + para_strAccessPwd + "'");
            new OleDbDataAdapter(this.strSQL, selectConnection).Fill(DS);
            return DS;
        }
        #endregion

        #region//加载本地的操作员档案表(Access文件)
        public DataSet LoginFrm_LoadLocalOperatorInf(string para_strPath, string para_strAccessPwd)
        {
            DataSet DS = new DataSet();
            strSQL = "select  distinct ONo ,OName from T_LocOperatorInf  order by ONo desc   ";
            OleDbConnection selectConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + para_strPath + @"\XXCLOUD.mdb;Jet OLEDB:Database Password='" + para_strAccessPwd + "'");
            new OleDbDataAdapter(this.strSQL, selectConnection).Fill(DS);
            return DS;
        }
        #endregion

        #region//检测软键盘是否已经在运行
        public DataSet LoginFrm_RegSBoardIsRun(string para_strPath, string para_strAccessMM)
        {
            DataSet DS = new DataSet();
            strSQL = "select A7 from T_LocConfigurationInf ";
            OleDbConnection selectConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + para_strPath + @"\XXCLOUD.mdb;Jet OLEDB:Database Password='" + para_strAccessMM + "'");
            new OleDbDataAdapter(strSQL, selectConnection).Fill(DS);
            return DS;
        }
        #endregion

        #region//设置软键盘为运行状态
        public int LoginFrm_SetSBoardIsRun(string para_strPath, string para_strAccessMM)
        {
            try
            {
                strSQL = "update T_LocConfigurationInf set  A7 ='1'";
                OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + para_strPath + @"\XXCLOUD.mdb;Jet OLEDB:Database Password='" + para_strAccessMM + "'");
                OleDbCommand command = new OleDbCommand(strSQL, connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        #endregion
        #endregion

        #region//MainBFrm
        #region//删除特定目录下的所有文件
        public void MainBFrm_DeleteFiles(string para_strDir)
        {
            if (!Directory.Exists(para_strDir))
            {
                File.Delete(para_strDir);
            }
            else
            {
                string[] Directories = Directory.GetDirectories(para_strDir);
                string[] Files = Directory.GetFiles(para_strDir);
                if (Directories.Length != 0)
                {
                    foreach (string str in Directories)
                    {
                        if (Directory.GetFiles(str) == null)
                        {
                            return;
                        }
                        this.MainBFrm_DeleteFiles(str);
                    }
                }
                if (Files.Length != 0)
                {
                    foreach (string strSubFile in Files)
                    {
                        File.Delete(strSubFile);
                    }
                }
            }
        }
        #endregion
        #endregion

        #region//1BasisFiles_A_GLobalDEFinationFrm

        public int A_GLobalDEFinationFrm_InsertBasisClass(out string para_strBSNo_Last, string para_strBSNo, string para_strBClassName, string para_strBMCode, string para_strTitle, string para_strCardType)
        {
            if (para_strTitle == "来访单位")
            {
               // para_strBSNo_Last = para_strBSNo;
               // strSQL = "select * from T_BaseCategroyInf where BClassName ='" + para_strBClassName + "'   and Flag ='" + para_strTitle + "'";
               //// strSQL += LoginFrm.strLoginFrmSelectFlag;
               // if (SQLHelper.DTQuery(strSQL).Rows.Count <= 0)
               // {
               //     if (para_strBSNo_Last == "")
               //     {
               //         para_strBSNo_Last = "1";
               //     }
               //     else
               //     {
               //         para_strBSNo_Last = Convert.ToString((int)(Convert.ToInt32((string)para_strBSNo_Last) + 1));
               //     }
               //     strSQL = "insert into T_BaseCategroyInf( BSNo,BClassName,BMCode,Flag,VPlace)values('" + para_strBSNo_Last + "','" + para_strBClassName + "','" + para_strBMCode + "','" + para_strTitle + "','" + MainBFrm.strEndUserName + "')";
               //     if (SQLHelper.ExecuteSql(strSQL) != 0)
               //     {
               //         return 1;
               //     }
               //     return 2;
               // }
                para_strBSNo_Last = "";
                return 0;
            }
            else if (para_strTitle == "门禁卡号")
            {
               // para_strBSNo_Last = para_strBMCode;
               // if (para_strBMCode.Trim() != "")
               // {
               //     strSQL = "select * from T_BaseCategroyInf where BValue ='" + para_strBMCode + "'   and Flag ='" + para_strTitle + "'";
               //     //strSQL += LoginFrm.strLoginFrmSelectFlag;
               //     if (SQLHelper.DTQuery(strSQL).Rows.Count > 0)
               //     {
               //         return 3;
               //     }
               // }
               // para_strBSNo_Last = para_strBSNo;
               // strSQL = "select * from T_BaseCategroyInf where BClassName ='" + para_strBClassName + "'   and Flag ='" + para_strTitle + "'";
               //// strSQL += LoginFrm.strLoginFrmSelectFlag;
               // if (SQLHelper.DTQuery(strSQL).Rows.Count <= 0)
               // {
               //     if (para_strBSNo_Last == "")
               //     {
               //         para_strBSNo_Last = "1";
               //     }
               //     else
               //     {
               //         para_strBSNo_Last = Convert.ToString((int)(Convert.ToInt32((string)para_strBSNo_Last) + 1));
               //     }
               //     strSQL = "insert into T_BaseCategroyInf( BSNo,BClassName,BValue,Flag,VPlace,BStatus,OperatDT,BCustomField1)values('" + para_strBSNo_Last + "',";
               //     strSQL += "'" + para_strBClassName + "','" + para_strBMCode + "','" + para_strTitle + "','" + MainBFrm.strEndUserName + "','" + "0" + "','" + System.DateTime.Now.ToString() + "',";
               //     strSQL += "'" + para_strCardType + "')";
               //     if (SQLHelper.ExecuteSql(strSQL) != 0)
               //     {
               //         return 1;
               //     }
               //     return 2;
               // }
                 para_strBSNo_Last = "";
                return 0;
            }
            else
            {
               // para_strBSNo_Last = para_strBSNo;
               // strSQL = "select * from T_BaseCategroyInf where BClassName ='" + para_strBClassName + "' and Flag ='" + para_strTitle + "'";
               //// strSQL += LoginFrm.strLoginFrmSelectFlag;
               // if (SQLHelper.DTQuery(strSQL).Rows.Count <= 0)
               // {
               //     if (para_strBSNo_Last == "")
               //     {
               //         para_strBSNo_Last = "1";
               //     }
               //     else
               //     {
               //         para_strBSNo_Last = Convert.ToString((int)(Convert.ToInt32((string)para_strBSNo_Last) + 1));
               //     }
               //     strSQL = "insert into T_BaseCategroyInf( BSNo,BClassName,Flag,VPlace)values('" + para_strBSNo_Last + "','" + para_strBClassName + "','" + para_strTitle + "','" + MainBFrm.strEndUserName + "')";
               //     if (SQLHelper.ExecuteSql(strSQL) != 0)
               //     {
               //         return 1;
               //     }
               //     return 2;
               // }
                para_strBSNo_Last = "";
                return 0;
            }
        }


        public int A_GLobalDEFinationFrm_EditBasisClassByBClassID(string para_strFlagBClassID, string para_strOldBClassName, string para_strOldBMCode, string para_strNewBClassName, string para_strNewBMCode, string para_strTitle, string para_strBStatus, string para_strBCardType)
        {
            if (para_strTitle == "来访单位")
            {
                strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strNewBClassName + "',BMCode='" + para_strNewBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    strSQL = "select * from T_BaseCategroyInf where BClassName='" + para_strNewBClassName + "' and Flag='" + para_strTitle + "'";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count < 2)
                    {

                        return 1;
                    }
                    strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strOldBClassName + "',BMCode='" + para_strOldBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        return 0;
                    }
                }
                return 2;
            }
            else if (para_strTitle == "门禁卡号")
            {
                strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strNewBClassName + "' where BClassID ='" + para_strFlagBClassID + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    strSQL = "select * from T_BaseCategroyInf where BClassName='" + para_strNewBClassName + "' and Flag='" + para_strTitle + "'";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count < 2)
                    {
                        // return 1;
                    }
                    else
                    {
                        strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strOldBClassName + "'  where BClassID ='" + para_strFlagBClassID + "'";
                        if (SQLHelper.ExecuteSql(strSQL) != 0)
                        {
                            return 0;//代表门禁卡号重复
                        }
                    }
                }
                strSQL = "update T_BaseCategroyInf set BValue ='" + para_strNewBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    strSQL = "select * from T_BaseCategroyInf where BValue='" + para_strNewBMCode + "' and Flag='" + para_strTitle + "'";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count < 2)
                    {
                        strSQL = "update T_BaseCategroyInf set BStatus ='" + para_strBStatus + "',OperatDT='" + System.DateTime.Now.ToString() + "',BCustomField1='" + para_strBCardType + "' where BClassID ='" + para_strFlagBClassID + "'";
                        SQLHelper.ExecuteSql(strSQL);
                        //同时更改XXCLOUD.dbo.T_MJCardLostInf中的卡片状态  Flag:0有效，即正在使用中  1：无效，即已遗失
                        strSQL = "update XXCLOUD.dbo.T_MJCardLostInf set Flag ='" + para_strBStatus + "'  where VCardNo ='" + para_strNewBClassName + "'";
                        SQLHelper.ExecuteSql(strSQL);
                        return 1;
                    }
                    else
                    {
                        strSQL = "update T_BaseCategroyInf set BValue ='" + para_strOldBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
                        if (SQLHelper.ExecuteSql(strSQL) != 0)
                        {
                            return 3;//代表卡面编号重复
                        }
                    }
                }
                return 2;
            }
            else
            {
                strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strNewBClassName + "' where BClassID ='" + para_strFlagBClassID + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    strSQL = "select * from T_BaseCategroyInf where BClassName='" + para_strNewBClassName + "' and Flag='" + para_strTitle + "'";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count < 2)
                    {
                        return 1;
                    }
                    strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strOldBClassName + "' where BClassID ='" + para_strFlagBClassID + "'";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        return 0;
                    }
                }
                return 2;
            }
            //if (para_strTitle != "来访单位")
            //{
            //    strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strNewBClassName + "' where BClassID ='" + para_strFlagBClassID + "'";
            //    if (SQLHelper.ExecuteSql(strSQL) != 0)
            //    {
            //        strSQL = "select * from T_BaseCategroyInf where BClassName='" + para_strNewBClassName + "' and Flag='" + para_strTitle + "'";
            //        if (SQLHelper.DTQuery(strSQL).Rows.Count < 2)
            //        {
            //            return 1;
            //        }
            //        strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strOldBClassName + "' where BClassID ='" + para_strFlagBClassID + "'";
            //        if (SQLHelper.ExecuteSql(strSQL) != 0)
            //        {
            //            return 0;
            //        }
            //    }
            //    return 2;
            //}
            //strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strNewBClassName + "',BMCode='" + para_strNewBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
            //if (SQLHelper.ExecuteSql(strSQL) != 0)
            //{
            //    strSQL = "select * from T_BaseCategroyInf where BClassName='" + para_strNewBClassName + "' and Flag='" + para_strTitle + "'";
            //    if (SQLHelper.DTQuery(strSQL).Rows.Count < 2)
            //    {
            //        return 1;
            //    }
            //    strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strOldBClassName + "',BMCode='" + para_strOldBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
            //    if (SQLHelper.ExecuteSql(strSQL) != 0)
            //    {
            //        return 0;
            //    }
            //}
            //return 2;
        }


        public int A_GLobalDEFinationFrm_DelBasisClassByBClassID(string para_strFlagBClassID, string para_strFlag)
        {
            if (para_strFlag == "0")
            {
                strSQL = "delete from T_BaseCategroyInf where BClassID ='" + para_strFlagBClassID + "' ";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return 1;
                }
                return 0;
            }
            strSQL = "delete from T_BaseCategroyInf where " + para_strFlagBClassID;
            if (SQLHelper.ExecuteSql(strSQL) != 0)
            {
                return 1;
            }
            return 0;
        }

        public int A_GLobalDEFinationFrm_EditBasisClassByBClassID(string para_strFlagBClassID, string para_strOldBClassName, string para_strOldBMCode, string para_strNewBClassName, string para_strNewBMCode, string para_strTitle)
        {
            if (para_strTitle != "来访单位"&&para_strTitle != "证件类别")
            {
                strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strNewBClassName + "' where BClassID ='" + para_strFlagBClassID + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    strSQL = "select * from T_BaseCategroyInf where BClassName='" + para_strNewBClassName + "' and Flag='" + para_strTitle + "'";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count < 2)
                    {
                        return 1;
                    }
                    strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strOldBClassName + "' where BClassID ='" + para_strFlagBClassID + "'";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        return 0;
                    }
                }
                return 2;
            }
            else if (para_strTitle == "证件类别")
            {
                strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strNewBClassName + "',BMCode='" + para_strNewBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    strSQL = "select * from T_BaseCategroyInf where BClassName='" + para_strNewBClassName + "' and Flag='" + para_strTitle + "'";
                    if (SQLHelper.DTQuery(strSQL).Rows.Count < 2)
                    {
                        strSQL = "update T_CertificateAndBarCodeInf set CDetailName ='" + para_strNewBClassName + "'  where CDetailName ='" + para_strOldBClassName + "'";
                        SQLHelper.ExecuteSql(strSQL);//修改证件与条码档案里的证件类别
                        return 1;
                    }
                    strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strOldBClassName + "',BMCode='" + para_strOldBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        return 0;
                    }
                }
                return 2;

            }

            strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strNewBClassName + "',BMCode='" + para_strNewBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
            if (SQLHelper.ExecuteSql(strSQL) != 0)
            {
                strSQL = "select * from T_BaseCategroyInf where BClassName='" + para_strNewBClassName + "' and Flag='" + para_strTitle + "'";
                if (SQLHelper.DTQuery(strSQL).Rows.Count < 2)
                {
                    return 1;
                }
                strSQL = "update T_BaseCategroyInf set BClassName ='" + para_strOldBClassName + "',BMCode='" + para_strOldBMCode + "' where BClassID ='" + para_strFlagBClassID + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return 0;
                }
            }
            return 2;
        }

        public int A_GLobalDEFinationFrm_InsertBasisClass(out string para_strBSNo_Last, string para_strBSNo, string para_strBClassName, string para_strBMCode, string para_strTitle)
        {
            if (para_strTitle != "来访单位")
            {
                para_strBSNo_Last = para_strBSNo;
                strSQL = "select * from T_BaseCategroyInf where BClassName ='" + para_strBClassName + "' and Flag ='" + para_strTitle + "'";
                if (SQLHelper.DTQuery(strSQL).Rows.Count <= 0)
                {
                    if (para_strBSNo_Last == "")
                    {
                        para_strBSNo_Last = "1";
                    }
                    else
                    {
                        para_strBSNo_Last = Convert.ToString((int)(Convert.ToInt32((string)para_strBSNo_Last) + 1));
                    }
                    strSQL = "insert into T_BaseCategroyInf( BSNo,BClassName,Flag)values('" + para_strBSNo_Last + "','" + para_strBClassName + "','" + para_strTitle + "')";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        return 1;
                    }
                    return 2;
                }
                para_strBSNo_Last = "";
                return 0;
            }
            para_strBSNo_Last = para_strBSNo;
            strSQL = "select * from T_BaseCategroyInf where BClassName ='" + para_strBClassName + "'   and Flag ='" + para_strTitle + "'";
            if (SQLHelper.DTQuery(strSQL).Rows.Count <= 0)
            {
                if (para_strBSNo_Last == "")
                {
                    para_strBSNo_Last = "1";
                }
                else
                {
                    para_strBSNo_Last = Convert.ToString((int)(Convert.ToInt32((string)para_strBSNo_Last) + 1));
                }
                strSQL = "insert into T_BaseCategroyInf( BSNo,BClassName,BMCode,Flag)values('" + para_strBSNo_Last + "','" + para_strBClassName + "','" + para_strBMCode + "','" + para_strTitle + "')";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return 1;
                }
                return 2;
            }
            para_strBSNo_Last = "";
            return 0;
        }

        public DataTable A_GLobalDEFinationFrm_LoadBasisClass(string para_strTitle)
        {
            strSQL = "select * from T_BaseCategroyInf where Flag ='" + para_strTitle + "' order by BSNo ";
            return SQLHelper.DTQuery(this.strSQL);
        }

        public int A_GLobalDEFinationFrm_SetDefaultBasisClass(string para_strTitle)
        {
            StringBuilder SBder = new StringBuilder();
            if (para_strTitle == "证件类别")
            {
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('二代身份证','1','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('临时身份证','2','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('驾驶证','3','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf (BClassName,BSNo,Flag)values('行驶证','4','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf (BClassName,BSNo,Flag)values('名片','5','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('律师证(照片页)','6','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('警官证(照片页)','7','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('军官证(信息页)','8','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('士兵证','9','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('户口本','10','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('居住证','11','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('香港身份证','12','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('澳门身份证','13','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('往来港澳通行证','14','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('中国道路运输证','15','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('中国护照','16','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('指纹','17','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('其他证件','18','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                if (SBder.Length != 0)
                {
                    if (!SQLHelper.ExecuteNonQueryTran(SBder.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
                    {
                        return 0;
                    }
                    return 1;
                }
            }
            else if (para_strTitle == "车牌类别")
            {
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('北京市[京]','1','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('天津市[津]','2','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('上海市[沪]','3','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('重庆市[渝]','4','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('河北省[冀]','5','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('河南省[豫]','6','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('云南省[云]','7','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('辽宁省[辽]','8','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('黑龙江省[黑]','9','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('湖南省[湘]','10','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('安徽省[皖]','11','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('山东省[鲁]','12','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('江苏省[苏]','13','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('浙江省[浙]','14','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('江西省[赣]','15','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('湖北省[鄂]','16','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('广西壮族[桂]','17','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('甘肃省[甘]','18','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('山西省[晋]','19','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('新疆维吾尔[新]','20','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('内蒙古[蒙]','21','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('陕西省[陕]','22','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('吉林省[吉]','23','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('福建省[闽]','24','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('贵州省[贵]','25','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('广东省[粤]','26','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('青海省[青]','27','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('西藏[藏]','28','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('四川省[川]','29','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('宁夏回族[宁]','30','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('海南省[琼]','31','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('军车[军]','32','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('北京军区[北]','33','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('南京军区[南]','34','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('广州军区[广]','35','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('沈阳军区[沈]','36','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('成都军区[成]','37','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('兰州军区[兰]','38','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('济南军区[济]','39','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('空军[空]','40','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('海军[海]','41','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('武警[WJ]','42','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('国务院[国]','43','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('使馆[使]','44','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                if (SBder.Length != 0)
                {
                    if (!SQLHelper.ExecuteNonQueryTran(SBder.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
                    {
                        return 0;
                    }
                    return 1;
                }
            }
            else if (para_strTitle == "来访事由")
            {
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('个人来访','1','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('公务往来','2','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('会议','3','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('快递','4','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('物流','5','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('参观','6','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('信访','7','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('传送文件','8','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('应聘招工','9','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('基建施工','10','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                if (SBder.Length != 0)
                {
                    if (!SQLHelper.ExecuteNonQueryTran(SBder.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
                    {
                        return 0;
                    }
                    return 1;
                }
            }
            else if (para_strTitle == "职务类别")
            {
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('董事长','1','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('总经理','2','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('部门经理','3','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('主任','4','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('职员','5','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                if (SBder.Length != 0)
                {
                    if (!SQLHelper.ExecuteNonQueryTran(SBder.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
                    {
                        return 0;
                    }
                    return 1;
                }
            }
            else if (para_strTitle == "携带物品")
            {
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('手提','1','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('文件','2','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                strSQL = "insert into T_BaseCategroyInf ( BClassName,BSNo,Flag)values('公文包','3','" + para_strTitle + "');";
                SBder.AppendFormat(strSQL, new object[0]);
                if (SBder.Length != 0)
                {
                    if (!SQLHelper.ExecuteNonQueryTran(SBder.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
                    {
                        return 0;
                    }
                    return 1;
                }
            }
            return 0;
        }
        #endregion

        #region//1BasisFiles_B_BuildingAndRoomFrm
        public string B_BuildingAndRoomFrm_LoadBuildingInf(TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_BuildingCategroyInf  order by BId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "BItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["BName"].ToString().Trim());
                            node.Name = DVRow["BId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["BItemLevel"]);
                            para_TV.Nodes.Add(node);
                            B_BuildingAndRoomFrm_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string B_BuildingAndRoomFrm_AddBuildingInf(TreeView para_TV, string para_strBName,string para_strSecondWay, string para_strBDetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select BItemLevel,BParentIndex from T_BuildingCategroyInf where BId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strBDetailName = "";
                            B_BuildingAndRoomFrm_GetBDetailName1(out strBDetailName, para_strBDetailName, para_TV.SelectedNode, false, para_strBName);
                            strBDetailName = para_strBDetailName + "/" + para_strBName;
                            TreeNode NodeNew = new TreeNode(para_strBName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strBItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["BItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strBItemLevelNew);
                            string str6 = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_BuildingCategroyInf(BName,BSecondWay,BItemLevel,BParentIndex,BDetailName,BMCode )Values('" + para_strBName + "',";
                            strSQL += "'"+para_strSecondWay+"','" + strBItemLevelNew + "','" + str6 + "','" + strBDetailName + "','" + MCodeAll(strBDetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(BId) BId from T_BuildingCategroyInf where BName='" + para_strBName + "' and BParentIndex='" + str6 + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["BId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        B_BuildingAndRoomFrm_GetBDetailName1(out str, para_strBDetailName, para_TV.SelectedNode, true, para_strBName);
                        if (para_strBDetailName.Contains("/"))
                        {
                            str = para_strBDetailName.Substring(0, para_strBDetailName.LastIndexOf("/")) + "/" + para_strBName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strBName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strBItemLevelNew = myTable.Rows[0]["BItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strBItemLevelNew);
                        string strBParentIndexNew = myTable.Rows[0]["BParentIndex"].ToString();
                        strSQL = "insert into T_BuildingCategroyInf(BName,BSecondWay,BItemLevel,BParentIndex,BDetailName,BMCode)Values('" + para_strBName + "',";
                        strSQL += "'"+para_strSecondWay+"','" + strBItemLevelNew + "','" + strBParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(BId) BId from T_BuildingCategroyInf where BName='" + para_strBName + "' and BParentIndex='" + strBParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["BId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_BuildingCategroyInf(BName,BSecondWay,BItemLevel,BParentIndex,BDetailName,BMCode )Values('" + para_strBName + "',";
                    strSQL += "'"+para_strSecondWay+"','" + "0" + "','" + "-1" + "','" + para_strBName + "','" + MCodeAll(para_strBName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strBName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(BId) BId from T_BuildingCategroyInf where BName='" + para_strBName + "' and BParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["BId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string B_BuildingAndRoomFrm_EditBuildingInf(TreeView para_TV)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                B_BuildingAndRoomFrm_RecursionEditBuildingInfAndRoomInf(para_TV.SelectedNode.Parent);//递归修改当前结点及所包含的办公室档案
            }
            else
            {
                strSQL = "update T_OfficeInf set BDetailName ='" + para_TV.SelectedNode.Text.Trim() + "' where T_OfficeInf.BId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
                strMCode = MCodeAll(para_TV.SelectedNode.Text.Trim());
                strSQL = "update T_BuildingCategroyInf set BName ='" + para_TV.SelectedNode.Text.Trim() + "',BDetailName ='" + para_TV.SelectedNode.Text.Trim() + "',BMCode='" + strMCode + "' where T_BuildingCategroyInf.BId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
                B_BuildingAndRoomFrm_RecursionEditBuildingInfAndRoomInf(para_TV.SelectedNode);
            }
            if ((sqlList.Length != 0) && SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        public string B_BuildingAndRoomFrm_DeleteBuildingInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select ID from T_OfficeInf where BId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_BuildingCategroyInf where BId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        public string B_BuildingAndRoomFrm_DeleteRoomInf(string para_strId, string para_strFilter, bool para_blFlag)
        {
            if (para_blFlag == false)
            {
                strSQL = "delete from T_OfficeInf where Id ='" + para_strId + "'";
            }
            else
            {
                strSQL = "delete from T_OfficeInf where  " + para_strFilter;
            }
            if (SQLHelper.ExecuteSql(strSQL) != 0)
            {
                return "1";
            }
            return "0";
        }

        private void B_BuildingAndRoomFrm_RecursionEditBuildingInfAndRoomInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        B_BuildingAndRoomFrm_GetBDetailName(strSQLTemp, para_TNode.Nodes[i]);
                        B_BuildingAndRoomFrm_RecursionEditBuildingInfAndRoomInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        B_BuildingAndRoomFrm_GetBDetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_OfficeInf set BDetailName ='" + para_TNode.Text.Trim() + "' where T_OfficeInf.BId ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        public string B_BuildingAndRoomFrm_GetBDetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }

        private void B_BuildingAndRoomFrm_GetBDetailName(string para_strBDetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strBDetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strBDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strBDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strBDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strBDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strBDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_OfficeInf set BDetailName ='" + para_strBDetailName + "' where T_OfficeInf.BId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strBDetailName);
            strSQL = "update T_BuildingCategroyInf set BDetailName='" + para_strBDetailName + "',BMCode='" + strMCode + "' where T_BuildingCategroyInf.BId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        private void B_BuildingAndRoomFrm_GetBDetailName1(out string strBDetailName, string strBDetailName1, TreeNode para_TNode, bool para_blFlag, string para_strBName)
        {
            strBDetailName = strBDetailName1;
            if (para_TNode.Parent == null)
            {
                if (para_blFlag)
                {
                    strBDetailName = para_strBName;
                }
                else
                {
                    strBDetailName = para_TNode.Text.Trim() + "/" + para_strBName;
                }
            }
            else
            {
                if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
                {
                    strBDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
                {
                    strBDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
                {
                    strBDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
                {
                    strBDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
                {
                    strBDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                if (!para_blFlag)
                {
                    strBDetailName = strBDetailName + "/" + para_strBName;
                }
            }
        }

        public string B_BuildingAndRoomFrm_GetRoomQuantity()
        {
            strSQL = "select Id from T_OfficeInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                return ("共有办公室档案:" + myTable.Rows.Count.ToString() + " 条");
            }
            return "共有办公室档案:0 条";
        }

        public string B_BuildingAndRoomFrm_LoadBuildingInf(out DataTable para_dt_Building1, DataTable para_dt_Building, TreeView para_TV)
        {
            para_dt_Building1 = para_dt_Building;
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_BuildingCategroyInf order by BId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DataRow dr;
                    for (int i = 0; i < myTable.Rows.Count; i++)
                    {
                        dr = para_dt_Building1.NewRow();
                        dr["BId"] = myTable.Rows[i]["BId"].ToString().Trim();
                        dr["BName"] = myTable.Rows[i]["BName"].ToString().Trim();
                        dr["BSecondWay"] = myTable.Rows[i]["BSecondWay"].ToString().Trim();
                        para_dt_Building1.Rows.Add(dr);
                    }
                    DV = new DataView(myTable);
                    DV.RowFilter = "BItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["BName"].ToString().Trim());
                            node.Name = DVRow["BId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt16(DVRow["BItemLevel"]);
                            para_TV.Nodes.Add(node);
                            B_BuildingAndRoomFrm_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
                return "0";
            }
            
        }

        public string B_BuildingAndRoomFrm_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "BParentIndex = '" + para_parentRow["BId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["BName"].ToString().Trim());
                    Node.Name = DVRow["BId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt16(DVRow["BItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    B_BuildingAndRoomFrm_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.ToString ());
                return "0";
            }
        }

        #region //自动产生办公室编号
        public void B_BuildingAndRoomFrm_AddRoom_RoomId(string para_strBName, string para_strBId, out string para_strId_Temp, out string para_strBillId_Old, out bool para_blBillId)
        {
            para_strId_Temp = "";
            para_strBillId_Old = "";
            para_blBillId = false;
            string strBDetailName_JX = MCodeAll(para_strBName);
            string strTempID = "";
            DataTable myTable = new DataTable();
            try
            {
                myTable = SQLHelper.DTQuery("select max(RNo) RNo from T_OfficeInf where BId ='" + para_strBId + "'");
                if (myTable.Rows.Count <= 0)
                {
                    #region
                    myTable = SQLHelper.DTQuery("select BNo from T_NumberTableInf where Flag ='" + "T_OfficeInf_RNo" + "'");
                    if (myTable.Rows.Count <= 0)
                    {
                        para_strId_Temp = strBDetailName_JX + "001";//默认为001
                        para_strBillId_Old = "";
                        SQLHelper.ExecuteSql("insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strId_Temp.Trim() + "','" + "T_OfficeInf_RNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        string maxID = myTable.Rows[0]["Id"].ToString().Trim();
                        para_strBillId_Old = maxID;
                        string strBillId_Old_ZF = "";//员工编号字符部分
                        string strBillId_Old_SZ = "0";//员工编号数字部分
                        for (int i = 0; i < maxID.Length; i++)
                        {
                            strSQL = maxID.Substring(i, 1).Trim();
                            if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                            {
                                strBillId_Old_ZF += strSQL;
                                continue;
                            }
                            else
                            {
                                strBillId_Old_SZ = maxID.Substring(i);
                                break;
                            }
                        }
                        int ilength_Old = strBillId_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBillId_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        para_strId_Temp = strBillId_Old_ZF + strTempID.Trim();
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + para_strId_Temp.Trim() + "' where Flag='" + "T_OfficeInf_RNo" + "'");
                    }
                    para_blBillId = true;
                    #endregion
                }
                else
                {
                    #region
                    string maxID = myTable.Rows[0]["RNo"].ToString().Trim();
                    para_strBillId_Old = maxID;
                    string strBillId_Old_ZF = "";//员工编号字符部分
                    string strBillId_Old_SZ = "";//员工编号数字部分
                    for (int i = 0; i < maxID.Length; i++)
                    {
                        strSQL = maxID.Substring(i, 1).Trim();
                        if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                        {
                            strBillId_Old_ZF += strSQL;
                            continue;
                        }
                        else
                        {
                            strBillId_Old_SZ = maxID.Substring(i);
                            break;
                        }
                    }
                    if (strBillId_Old_SZ.Trim() != "")
                    {
                        int ilength_Old = strBillId_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBillId_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        para_strId_Temp = strBillId_Old_ZF + strTempID.Trim();
                    }
                    else
                    {
                        para_strId_Temp = strBDetailName_JX + "001";//默认为001
                        para_strBillId_Old = "";
                        if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag='" + "T_OfficeInf_RNo" + "'").Rows.Count <= 0)
                        {
                            SQLHelper.ExecuteSql("insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strId_Temp.Trim() + "','" + "T_OfficeInf_RNo" + "','" + "" + "','" + "" + "')");
                        }
                    }
                    if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag='" + "T_OfficeInf_RNo" + "'").Rows.Count <= 0)
                    {
                        SQLHelper.ExecuteSql("insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strId_Temp.Trim() + "','" + "T_OfficeInf_RNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + para_strId_Temp.Trim() + "' where Flag ='" + "T_OfficeInf_RNo" + "'");
                    }
                    #endregion
                }
                para_blBillId = true;

            }
            catch
            {

                try
                {
                    #region
                    string maxID = myTable.Rows[0]["Id"].ToString().Trim();
                    para_strBillId_Old = maxID;
                    string strBillId_Old_ZF = "";//员工编号字符部分
                    string strBillId_Old_SZ = "";//员工编号数字部分
                    for (int i = maxID.Length - 1; i >= 0; i--)
                    {
                        strSQL = maxID.Substring(i, 1).Trim();
                        if (strSQL.CompareTo("0") >= 0 && strSQL.CompareTo("9") <= 0)
                        {
                            strBillId_Old_SZ = strSQL + strBillId_Old_SZ;
                            continue;
                        }
                        else
                        {
                            strBillId_Old_ZF = maxID.Substring(0, i + 1);
                            break;
                        }
                    }
                    if (strBillId_Old_SZ.Trim() != "")
                    {
                        int ilength_Old = strBillId_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBillId_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        para_strId_Temp = strBillId_Old_ZF + strTempID.Trim();
                    }
                    if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag='" + "T_OfficeInf_RNo" + "'").Rows.Count <= 0)
                    {
                        SQLHelper.ExecuteSql("insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strId_Temp.Trim() + "','" + "T_OfficeInf_RNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + para_strId_Temp.Trim() + "' where Flag='" + "T_OfficeInf_RNo" + "'");
                    }
                    #endregion
                }
                catch
                {
                    para_strId_Temp = strBDetailName_JX + "001";
                    // MessageBox.Show("员工编号自动识别失败，请手工输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region//加载楼宇详细档案
        public DataTable B_BuildingAndRoomFrm_SeparateBuilding(ListBox para_lst_Building, TreeView para_TV)
        {
            DataTable dt_Building = new DataTable();
            dt_Building.Columns.Add("BId");
            dt_Building.Columns.Add("BName");
            dt_Building.Columns.Add("BSecondWay");
            strSQL = "select * from T_BuildingCategroyInf  order by BId ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    DataRow dr = dt_Building.NewRow();
                    dr["BId"] = myTable.Rows[i]["BId"].ToString().Trim();
                    dr["BName"] = myTable.Rows[i]["BName"].ToString().Trim();
                    dr["BSecondWay"] = myTable.Rows[i]["BSecondWay"].ToString().Trim();
                    dt_Building.Rows.Add(dr);
                }
            }
            para_lst_Building.Items.Clear();
            for (int i = 0; i < dt_Building.Rows.Count; i++)
            {
                if (para_TV.SelectedNode.Name.Trim() == dt_Building.Rows[i]["BId"].ToString().Trim())
                {
                    para_lst_Building.Items.Add("楼宇名称:  " + dt_Building.Rows[i]["BName"].ToString().Trim());
                    para_lst_Building.Items.Add("二道岗亭:  " + dt_Building.Rows[i]["BSecondWay"].ToString().Trim());
 
                    break;
                }
            }
            return dt_Building;
        }
        #endregion

        #region//更改办公室所在楼宇
        public string B_BuildingAndRoomFrm_EditCurRoom(DataTable para_DT, string para_strBId, string para_strBDetailName)
        {
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  T_OfficeInf.Id  = " + "'" + dt.Rows[i]["Id"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_OfficeInf set BID='" + para_strBId + "',BDetailName ='" + para_strBDetailName + "' where " + strFilter + "";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }


                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }
        #endregion

        #region//更改员工所属二道岗亭
        public string B_BuildingAndRoomFrm_EditCurSecondWay(DataTable para_DT, string para_strSSecondWay)
        {
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  T_OfficeInf.Id  = " + "'" + dt.Rows[i]["Id"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_OfficeInf set RSecondWay='" + para_strSSecondWay + "' where " + strFilter + "";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }


                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }
        #endregion


        #endregion

        #region//1BasisFiles_C_GuardRoomAndDKeeperFrm
        public string C_GuardRoomAndDKeeperFrm_LoadGuardRoomInf(TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_GuardRoomCategroyInf  order by GId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "GItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["GName"].ToString().Trim());
                            node.Name = DVRow["GId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["GItemLevel"]);
                            para_TV.Nodes.Add(node);
                            C_GuardRoomAndDKeeperFrm_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string C_GuardRoomAndDKeeperFrm_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "GParentIndex = '" + para_parentRow["GId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["GName"].ToString().Trim());
                    Node.Name = DVRow["GId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt32(DVRow["GItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    C_GuardRoomAndDKeeperFrm_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string C_GuardRoomAndDKeeperFrm_AddGuardRoomInf(TreeView para_TV, string para_strGName, string para_strGDetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select GItemLevel,GParentIndex from T_GuardRoomCategroyInf where GId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strGDetailName = "";
                            C_GuardRoomAndDKeeperFrm_GetGDetailName1(out strGDetailName, para_strGDetailName, para_TV.SelectedNode, false, para_strGName);
                            strGDetailName = para_strGDetailName + "/" + para_strGName;
                            TreeNode NodeNew = new TreeNode(para_strGName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strGItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["GItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                            string strGParentIndexNew = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_GuardRoomCategroyInf(GName,GItemLevel,GParentIndex,GDetailName,GMCode )Values('" + para_strGName + "',";
                            strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + strGDetailName + "','" + MCodeAll(strGDetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(GId) GId from T_GuardRoomCategroyInf where GName='" + para_strGName + "' and GParentIndex='" + strGParentIndexNew + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["GId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        C_GuardRoomAndDKeeperFrm_GetGDetailName1(out str, para_strGDetailName, para_TV.SelectedNode, true, para_strGName);
                        if (para_strGDetailName.Contains("/"))
                        {
                            str = para_strGDetailName.Substring(0, para_strGDetailName.LastIndexOf("/")) + "/" + para_strGName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strGName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strGItemLevelNew = myTable.Rows[0]["GItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                        string strGParentIndexNew = myTable.Rows[0]["GParentIndex"].ToString();
                        strSQL = "insert into T_GuardRoomCategroyInf(GName,GItemLevel,GParentIndex,GDetailName,GMCode)Values('" + para_strGName + "',";
                        strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(GId) GId from T_GuardRoomCategroyInf where GName='" + para_strGName + "' and GParentIndex='" + strGParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["GId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_GuardRoomCategroyInf(GName,GItemLevel,GParentIndex,GDetailName,GMCode )Values('" + para_strGName + "',";
                    strSQL += "'" + "0" + "','" + "-1" + "','" + para_strGName + "','" + MCodeAll(para_strGName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strGName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(GId) GId from T_GuardRoomCategroyInf where GName='" + para_strGName + "' and GParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["GId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string C_GuardRoomAndDKeeperFrm_EditGuardRoomInf(TreeView para_TV)
        {
            if (this.sqlList.Length != 0)
            {
                this.sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                C_GuardRoomAndDKeeperFrm_RecursionEditGuardRoomAndDoorKeeperInf(para_TV.SelectedNode.Parent);
            }
            else
            {
                this.strSQL = "update T_DoorKeeperInf set DGDetailName='" + para_TV.SelectedNode.Text.Trim() + "' where T_DoorKeeperInf.DGId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                this.strMCode = this.MCodeAll(para_TV.SelectedNode.Text.Trim());
                this.strSQL = "update T_GuardRoomCategroyInf set GName ='" + para_TV.SelectedNode.Text.Trim() + "',GDetailName ='" + para_TV.SelectedNode.Text.Trim() + "',GMCode='" + this.strMCode + "' where T_GuardRoomCategroyInf.GId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                C_GuardRoomAndDKeeperFrm_RecursionEditGuardRoomAndDoorKeeperInf(para_TV.SelectedNode);
            }
            if ((this.sqlList.Length != 0) && this.SQLHelper.ExecuteNonQueryTran(this.sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                this.sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        public string C_GuardRoomAndDKeeperFrm_DeleteGuardRoomInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select ID from T_DoorKeeperInf where DGId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_GuardRoomCategroyInf where GId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        private void C_GuardRoomAndDKeeperFrm_GetGDetailName1(out string strGDetailName, string strGDetailName1, TreeNode para_TNode, bool para_blFlag, string para_strGName)
        {
            strGDetailName = strGDetailName1;
            if (para_TNode.Parent == null)
            {
                if (para_blFlag)
                {
                    strGDetailName = para_strGName;
                }
                else
                {
                    strGDetailName = para_TNode.Text.Trim() + "/" + para_strGName;
                }
            }
            else
            {
                if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                if (!para_blFlag)
                {
                    strGDetailName = strGDetailName + "/" + para_strGName;
                }
            }
        }

        public string C_GuardRoomAndDKeeperFrm_GetGDetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }

        public string C_GuardRoomAndDKeeperFrm_GetDoorKeeperQuantity(string para_strGuardRoomName)
        {
            strSQL = "select distinct DActualNo,DSex from T_DoorKeeperInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                int iDSex_M = 0;
                int iDSex_W = 0;
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    if (myTable.Rows[i]["DSex"].ToString().Trim() == "男")
                    {
                        iDSex_M++;
                    }
                    else if (myTable.Rows[i]["DSex"].ToString().Trim() == "女")
                    {
                        iDSex_W++;
                    }
                }
                strSQL = "共有门卫档案:" + myTable.Rows.Count.ToString() + " 条,其中男:" + iDSex_M.ToString() + "  女:" + iDSex_W.ToString();
                if (para_strGuardRoomName != "")
                {
                    return (strSQL + "　　  本机所在门岗为:" + para_strGuardRoomName);
                }
                return (strSQL + "　　  还未设定本机所在门岗");
            }
            if (para_strGuardRoomName != "")
            {
                return ("共有门卫档案:0 条　　  本机所在门岗为:" + para_strGuardRoomName);
            }
            return ("共有门卫档案:0 条　　  还未设定本机所在门岗");
        }

        private void C_GuardRoomAndDKeeperFrm_RecursionEditGuardRoomAndDoorKeeperInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        C_GuardRoomAndDKeeperFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                        C_GuardRoomAndDKeeperFrm_RecursionEditGuardRoomAndDoorKeeperInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        C_GuardRoomAndDKeeperFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_DoorKeeperInf set DGDetailName ='" + para_TNode.Text.Trim() + "' where T_DoorKeeperInf.DGID ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        private void C_GuardRoomAndDKeeperFrm_GetGDetailName(string para_strGDetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strGDetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_DoorKeeperInf set DGDetailName ='" + para_strGDetailName + "' where T_DoorKeeperInf.DGID ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strGDetailName);
            strSQL = "update T_GuardRoomCategroyInf set GDetailName='" + para_strGDetailName + "',GMCode='" + strMCode + "' where T_GuardRoomCategroyInf.GId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        #region //自动产生门卫编号
        public void C_GuardRoomAndDKeeperFrm_AddDKeeper_DKeeperNo(string para_strGName, string para_strGId, out string para_strDNo_Temp, out string para_strBillId_Old, out bool para_blBillId)
        {
            para_strGName = para_strGName.Replace("（", "");
            para_strGName = para_strGName.Replace("）", "");
            para_strGName = para_strGName.Replace("(", "");
            para_strGName = para_strGName.Replace("(", "");
            para_strGName = para_strGName.Replace(" ", "");
            para_strDNo_Temp = "";
            para_strBillId_Old = "";
            para_blBillId = false;
            string strGDetailName_JX = MCodeAll(para_strGName);
            string strTempID = "";
            try
            {
                myTable = SQLHelper.DTQuery("select max(DNo) DNo from T_DoorKeeperInf where DGID ='" + para_strGId + "'");
                if (myTable.Rows.Count <= 0)
                {
                    #region
                    myTable = SQLHelper.DTQuery("select BNo from T_NumberTableInf where Flag ='" + "T_DoorKeeperInf_DNo" + "'");
                    if (myTable.Rows.Count <= 0)
                    {
                        para_strDNo_Temp = strGDetailName_JX + "0001";//默认为0001
                        para_strBillId_Old = "";
                        SQLHelper.ExecuteSql("insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strDNo_Temp.Trim() + "','" + "T_DoorKeeperInf_DNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        string maxID = myTable.Rows[0]["BNo"].ToString().Trim();
                        para_strBillId_Old = maxID;
                        string strBNo_Old_ZF = "";//员工编号字符部分
                        string strBNo_Old_SZ = "0";//员工编号数字部分
                        for (int i = 0; i < maxID.Length; i++)
                        {
                            strSQL = maxID.Substring(i, 1).Trim();
                            if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                            {
                                strBNo_Old_ZF += strSQL;
                                continue;
                            }
                            else
                            {
                                strBNo_Old_SZ = maxID.Substring(i);
                                break;
                            }
                        }
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        para_strDNo_Temp = strBNo_Old_ZF + strTempID.Trim();
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + para_strDNo_Temp.Trim() + "' where Flag ='" + "T_DoorKeeperInf_DNo" + "'");
                    }
                    para_blBillId = true;
                    #endregion
                }
                else
                {
                    #region
                    string maxID = myTable.Rows[0]["DNo"].ToString().Trim();
                    para_strBillId_Old = maxID;
                    string strBNo_Old_ZF = "";//员工编号字符部分
                    string strBNo_Old_SZ = "";//员工编号数字部分
                    for (int i = 0; i < maxID.Length; i++)
                    {
                        strSQL = maxID.Substring(i, 1).Trim();
                        if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                        {
                            strBNo_Old_ZF += strSQL;
                            continue;
                        }
                        else
                        {
                            strBNo_Old_SZ = maxID.Substring(i);
                            break;
                        }
                    }
                    if (strBNo_Old_SZ.Trim() != "")
                    {
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        para_strDNo_Temp = strBNo_Old_ZF + strTempID.Trim();
                    }
                    else
                    {
                        para_strDNo_Temp = strGDetailName_JX + "0001";//默认为0001
                        para_strBillId_Old = "";
                        if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_DoorKeeperInf_DNo" + "'").Rows.Count <= 0)
                        {
                            SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo )Values('" + para_strDNo_Temp.Trim() + "','" + "T_DoorKeeperInf_DNo" + "','" + "" + "','" + "" + "')");
                        }
                    }
                    if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_DoorKeeperInf_DNo" + "'").Rows.Count <= 0)
                    {
                        SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strDNo_Temp.Trim() + "','" + "T_DoorKeeperInf_DNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + para_strDNo_Temp.Trim() + "' where Flag='" + "T_DoorKeeperInf_DNo" + "'");
                    }
                    #endregion
                }
                para_blBillId = true;

            }
            catch
            {

                try
                {
                    #region
                    string maxID = myTable.Rows[0]["DNo"].ToString().Trim();
                    para_strBillId_Old = maxID;
                    string strBNo_Old_ZF = "";//员工编号字符部分
                    string strBNo_Old_SZ = "";//员工编号数字部分
                    for (int i = maxID.Length - 1; i >= 0; i--)
                    {
                        strSQL = maxID.Substring(i, 1).Trim();
                        if (strSQL.CompareTo("0") >= 0 && strSQL.CompareTo("9") <= 0)
                        {
                            strBNo_Old_SZ = strSQL + strBNo_Old_SZ;
                            continue;
                        }
                        else
                        {
                            strBNo_Old_ZF = maxID.Substring(0, i + 1);
                            break;
                        }
                    }
                    if (strBNo_Old_SZ.Trim() != "")
                    {
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        para_strDNo_Temp = strBNo_Old_ZF + strTempID.Trim();
                    }
                    if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_DoorKeeperInf_DNo" + "'").Rows.Count <= 0)
                    {
                        SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strDNo_Temp.Trim() + "','" + "T_DoorKeeperInf_DNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + para_strDNo_Temp.Trim() + "' where Flag='" + "T_DoorKeeperInf_DNo" + "'");
                    }
                    #endregion
                }
                catch
                {
                    para_strDNo_Temp = strGDetailName_JX + "0001";
                    // MessageBox.Show("门卫编号自动识别失败，请手工输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region //自动产生门卫实际编号
        public void C_GuardRoomAndDKeeperFrm_AddDKeeper_DKeeperActualNo(out  string para_strDActualNo)
        {
            para_strDActualNo = "0000000001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(DActualNo) DActualNo from T_DoorKeeperInf order by  DActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["DActualNo"].ToString().Trim() != "" && myTable.Rows[0]["DActualNo"].ToString().Trim() != null)
                    {
                        int iDActualNoNew = Convert.ToInt16(myTable.Rows[0]["DActualNo"].ToString()) + 1;
                        switch (iDActualNoNew.ToString().Length)
                        {
                            case 1:
                                para_strDActualNo = "000000000" + iDActualNoNew.ToString(); break;
                            case 2:
                                para_strDActualNo = "00000000" + iDActualNoNew.ToString(); break;
                            case 3:
                                para_strDActualNo = "0000000" + iDActualNoNew.ToString(); break;
                            case 4:
                                para_strDActualNo = "000000" + iDActualNoNew.ToString(); break;
                            case 5:
                                para_strDActualNo = "00000" + iDActualNoNew.ToString(); break;
                            case 6:
                                para_strDActualNo = "0000" + iDActualNoNew.ToString(); break;
                            case 7:
                                para_strDActualNo = "000" + iDActualNoNew.ToString(); break;
                            case 8:
                                para_strDActualNo = "00" + iDActualNoNew.ToString(); break;
                            case 9:
                                para_strDActualNo = "0" + iDActualNoNew.ToString(); break;
                            default:
                                para_strDActualNo = iDActualNoNew.ToString(); break;
                        }
                    }
                }
                // return strDActualNo;
            }
            catch
            {
                //  return strDActualNo;

            }
        }

        #endregion

        #region//更改门卫所在门岗
        public string C_GuardRoomAndDKeeperFrm_EditCurGuardRoom(DataTable para_DT, string para_strGId, string para_strGDetailName)
        {
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  T_DoorKeeperInf.DActualNo  = " + "'" + dt.Rows[i]["DActualNo"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_DoorKeeperInf set DGID='" + para_strGId + "',DGDetailName ='" + para_strGDetailName + "' where " + strFilter + "";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }


                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }
        #endregion

        #region//更改本机所在门岗(即捷访通所在门岗)
        public string C_GuardRoomAndDKeeperFrm_SetCurGuardRoom(TreeView para_TV, string para_strAccessPwd)
        {
            try
            {
                string strGId = SQLHelper.EncryptString(para_TV.SelectedNode.Name.Trim() + "A5");
                string strGDetailName = SQLHelper.EncryptString(para_TV.SelectedNode.Text.Trim() + "A6");
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
                strSQL = "update T_LocConfigurationInf set A5='" + strGId + "',A6='" + strGDetailName + "'";
                System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
                OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return "1";
            }
            catch
            {
                return "0";
            }
        }
        #endregion

        #region//更改本机所在门岗(即捷访通所在门岗)
        public string C_GuardRoomAndDKeeperFrm_EditGuardRoom(string para_strGId, string para_strGDetailName, string para_strAccessPwd)
        {
            try
            {
                string strGId = SQLHelper.EncryptString(para_strGId + "A5");
                string strGDetailName = SQLHelper.EncryptString(para_strGDetailName + "A6");
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
                strSQL = "update T_LocConfigurationInf set A5='" + strGId + "',A6='" + strGDetailName + "'";
                System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
                OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return "1";
            }
            catch
            {
                return "0";
            }
        }
        #endregion

        #region//删除门卫档案
        public string C_GuardRoomAndDKeeperFrm_DeleteDoorKeeperInf(string para_strDActualNo, string para_strFilter, string para_strFilter_CustomItemInf, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条门卫档案
            {
                strSQL = "delete from T_DoorKeeperInf where T_DoorKeeperInf.DActualNo ='" + para_strDActualNo + "';";
                sqlList.AppendFormat(strSQL);
                strSQL = "delete from T_DoorKeeperUCIInf where DActualNo ='" + para_strDActualNo + "' and Flag ='" + "T_DoorKeeperInf" + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    strSQL = "select id from T_DoorKeeperInf ";
                    DataTable dtTemp = SQLHelper.DTQuery(strSQL);
                    if (dtTemp.Rows.Count <= 0)
                    {
                        sqlList.Length = 0;
                        strSQL = "delete from T_DoorKeeperInf ";
                        sqlList.AppendFormat(strSQL);
                        strSQL = "delete from T_DoorKeeperUCIInf ";
                        sqlList.AppendFormat(strSQL);
                        strSQL = "delete from T_NumberTableInf where Flag='" + "T_DoorKeeperInf_DNo" + "'; ";
                        sqlList.AppendFormat(strSQL);
                        if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                        {

                        }

                    }
                    return "1";
                }
            }
            else//删除多选门卫档案
            {
                strSQL = "delete from T_DoorKeeperInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                strSQL = "delete from T_DoorKeeperUCIInf where " + para_strFilter_CustomItemInf + " and Flag ='" + "T_DoorKeeperInf" + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    strSQL = "select id from T_DoorKeeperInf ";
                    DataTable dtTemp = SQLHelper.DTQuery(strSQL);
                    if (dtTemp.Rows.Count <= 0)
                    {
                        sqlList.Length = 0;
                        strSQL = "delete from T_DoorKeeperInf ";
                        sqlList.AppendFormat(strSQL);
                        strSQL = "delete from T_DoorKeeperUCIInf ";
                        sqlList.AppendFormat(strSQL);
                        strSQL = "delete from T_NumberTableInf where Flag='" + "T_DoorKeeperInf_DNo" + "'; ";
                        sqlList.AppendFormat(strSQL);
                        if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                        {

                        }

                    }
                    return "1";
                }
            }
            return "0";
        }
        #endregion
        #endregion

        #region//1BasisFiles_D_DepartmentAndStaffFrm
        public string D_DepartmentAndStaffFrm_LoadDepartmentInf(out DataTable para_dt_Department1, DataTable para_dt_Department, TreeView para_TV)
        {
            para_dt_Department1 = para_dt_Department;
            try
            {
                strSQL = "select * from T_DepartmentCategroyInf  order by DId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DataRow dr;
                    for (int i = 0; i < myTable.Rows.Count; i++)
                    {
                        dr = para_dt_Department1.NewRow();
                        dr["DId"] = myTable.Rows[i]["DId"].ToString().Trim();
                        dr["DPrincipal"] = myTable.Rows[i]["DPrincipal"].ToString().Trim();
                        dr["DRNo"] = myTable.Rows[i]["DRNo"].ToString().Trim();
                        dr["DOPhone"] = myTable.Rows[i]["DOPhone"].ToString().Trim();
                        dr["DEPhone"] = myTable.Rows[i]["DEPhone"].ToString().Trim();
                        dr["DSecondWay"] = myTable.Rows[i]["DSecondWay"].ToString().Trim();
                        para_dt_Department1.Rows.Add(dr);
                    }
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "DItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["DName"].ToString().Trim());
                            node.Name = DVRow["DId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["DItemLevel"]);
                            para_TV.Nodes.Add(node);
                            D_DepartmentAndStaffFrm_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string D_DepartmentAndStaffFrm_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "DParentIndex = '" + para_parentRow["DId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["DName"].ToString().Trim());
                    Node.Name = DVRow["DId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt32(DVRow["DItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    D_DepartmentAndStaffFrm_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string D_DepartmentAndStaffFrm_AddDepartmentInf(TreeView para_TV, string para_strDName,  string para_strDPrincipal, string para_strDRNo, string para_strDOPhone, string para_strDEPhone, string para_strDSecondWay, string para_strDDetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select DItemLevel,DParentIndex from T_DepartmentCategroyInf where DId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strDDetailName = "";
                            D_DepartmentAndStaffFrm_GetDDetailName1(out strDDetailName, para_strDDetailName, para_TV.SelectedNode, false, para_strDName);
                            strDDetailName = para_strDDetailName + "/" + para_strDName;
                            TreeNode NodeNew = new TreeNode(para_strDName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strDItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["DItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strDItemLevelNew);
                            string strDParentIndexNew = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_DepartmentCategroyInf(DName,DPrincipal,DRNo,DOPhone,DEPhone,DSecondWay,DItemLevel,DParentIndex,DDetailName,DMCode )Values('" + para_strDName + "',";
                            strSQL += "'" + para_strDPrincipal + "','" + para_strDRNo + "','" + para_strDOPhone + "','" + para_strDEPhone + "','"+para_strDSecondWay+"',";
                            strSQL += "'" + strDItemLevelNew + "','" + strDParentIndexNew + "','" + strDDetailName + "','" + MCodeAll(strDDetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(DId) DId from T_DepartmentCategroyInf where DName='" + para_strDName + "' and DParentIndex='" + strDParentIndexNew + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["DId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        D_DepartmentAndStaffFrm_GetDDetailName1(out str, para_strDDetailName, para_TV.SelectedNode, true, para_strDName);
                        if (para_strDDetailName.Contains("/"))
                        {
                            str = para_strDDetailName.Substring(0, para_strDDetailName.LastIndexOf("/")) + "/" + para_strDName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strDName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strDItemLevelNew = myTable.Rows[0]["DItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strDItemLevelNew);
                        string strDParentIndexNew = myTable.Rows[0]["DParentIndex"].ToString();
                        strSQL = "insert into T_DepartmentCategroyInf(DName,DPrincipal,DRNo,DOPhone,DEPhone,DSecondWay,DItemLevel,DParentIndex,DDetailName,DMCode )Values('" + para_strDName + "',";
                        strSQL += "'" + para_strDPrincipal + "','" + para_strDRNo + "','" + para_strDOPhone + "','" + para_strDEPhone + "','"+para_strDSecondWay+"',";
                        strSQL += "'" + strDItemLevelNew + "','" + strDParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(DId) DId from T_DepartmentCategroyInf where DName='" + para_strDName + "' and DParentIndex='" + strDParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["DId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_DepartmentCategroyInf(DName,DPrincipal,DRNo,DOPhone,DEPhone,DSecondWay,DItemLevel,DParentIndex,DDetailName,DMCode )Values('" + para_strDName + "',";
                    strSQL += "'" + para_strDPrincipal + "','" + para_strDRNo + "','" + para_strDOPhone + "','" + para_strDEPhone + "','"+para_strDSecondWay+"',";
                    strSQL += "'" + "0" + "','" + "-1" + "','" + para_strDName + "','" + MCodeAll(para_strDName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strDName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(DId) DId from T_DepartmentCategroyInf where DName='" + para_strDName + "' and DParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["DId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string D_DepartmentAndStaffFrm_DeleteDepartmentInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select ID from T_StaffInf where SDId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_DepartmentCategroyInf where DId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        private void D_DepartmentAndStaffFrm_GetDDetailName1(out string strDDetailName, string strDDetailName1, TreeNode para_TNode, bool para_blFlag, string para_strDName)
        {
            strDDetailName = strDDetailName1;
            if (para_TNode.Parent == null)
            {
                if (para_blFlag)
                {
                    strDDetailName = para_strDName;
                }
                else
                {
                    strDDetailName = para_TNode.Text.Trim() + "/" + para_strDName;
                }
            }
            else
            {
                if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
                {
                    strDDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
                {
                    strDDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
                {
                    strDDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
                {
                    strDDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
                {
                    strDDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                if (!para_blFlag)
                {
                    strDDetailName = strDDetailName + "/" + para_strDName;
                }
            }
        }

        public string D_DepartmentAndStaffFrm_GetDDetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }


        #region//加载部门详细档案
        public DataTable D_DepartmentAndStaffFrm_SeparateDepartment(ListBox para_lst_Department, TreeView para_TV)
        {
            DataTable dt_Department = new DataTable();
            dt_Department.Columns.Add("DId");
            dt_Department.Columns.Add("DPrincipal");
            dt_Department.Columns.Add("DRNo");
            dt_Department.Columns.Add("DOPhone");
            dt_Department.Columns.Add("DEPhone");
            dt_Department.Columns.Add("DSecondWay");
            strSQL = "select * from T_DepartmentCategroyInf  order by DId ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    DataRow dr = dt_Department.NewRow();
                    dr["DId"] = myTable.Rows[i]["DId"].ToString().Trim();
                    dr["DPrincipal"] = myTable.Rows[i]["DPrincipal"].ToString().Trim();
                    dr["DRNo"] = myTable.Rows[i]["DRNo"].ToString().Trim();
                    dr["DOPhone"] = myTable.Rows[i]["DOPhone"].ToString().Trim();
                    dr["DEPhone"] = myTable.Rows[i]["DEPhone"].ToString().Trim();
                    dr["DSecondWay"] = myTable.Rows[i]["DSecondWay"].ToString().Trim();
                    dt_Department.Rows.Add(dr);
                }
            }
            para_lst_Department.Items.Clear();
            for (int i = 0; i < dt_Department.Rows.Count; i++)
            {
                if (para_TV.SelectedNode.Name.Trim() == dt_Department.Rows[i]["DId"].ToString().Trim())
                {
                    para_lst_Department.Items.Add("负责人:    " + dt_Department.Rows[i]["DPrincipal"].ToString().Trim());
                    para_lst_Department.Items.Add("办公室:    " + dt_Department.Rows[i]["DRNo"].ToString().Trim());
                    para_lst_Department.Items.Add("办公电话:  " + dt_Department.Rows[i]["DOPhone"].ToString().Trim());
                    para_lst_Department.Items.Add("分机号:    " + dt_Department.Rows[i]["DEPhone"].ToString().Trim());
                    para_lst_Department.Items.Add("二道岗亭:  " + dt_Department.Rows[i]["DSecondWay"].ToString().Trim());
                    break;
                }
            }
            return dt_Department;
        }
        #endregion

        public string D_DepartmentAndStaffFrm_EditDepartment(TreeView para_TV)
        {
            if (this.sqlList.Length != 0)
            {
                this.sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                D_DepartmentAndStaffFrm_RecursionEditDepartmentAndStaffInf(para_TV.SelectedNode.Parent);
            }
            else
            {
                this.strSQL = "update T_StaffInf set SDDetailName='" + para_TV.SelectedNode.Text.Trim() + "' where T_StaffInf.SDId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                this.strMCode = this.MCodeAll(para_TV.SelectedNode.Text.Trim());
                this.strSQL = "update T_DepartmentCategroyInf set DName ='" + para_TV.SelectedNode.Text.Trim() + "',DDetailName ='" + para_TV.SelectedNode.Text.Trim() + "',DMCode='" + this.strMCode + "' where T_DepartmentCategroyInf.DId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                D_DepartmentAndStaffFrm_RecursionEditDepartmentAndStaffInf(para_TV.SelectedNode);
            }
            if ((this.sqlList.Length != 0) && this.SQLHelper.ExecuteNonQueryTran(this.sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                this.sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        private void D_DepartmentAndStaffFrm_RecursionEditDepartmentAndStaffInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        D_DepartmentAndStaffFrm_GetDDetailName(strSQLTemp, para_TNode.Nodes[i]);
                        D_DepartmentAndStaffFrm_RecursionEditDepartmentAndStaffInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        D_DepartmentAndStaffFrm_GetDDetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_StaffInf set SDDetailName ='" + para_TNode.Text.Trim() + "' where T_StaffInf.SDId ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        private void D_DepartmentAndStaffFrm_GetDDetailName(string para_strGDetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strGDetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_StaffInf set SDDetailName ='" + para_strGDetailName + "' where T_StaffInf.SDId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strGDetailName);
            strSQL = "update T_DepartmentCategroyInf set DDetailName='" + para_strGDetailName + "',DMCode='" + strMCode + "' where T_DepartmentCategroyInf.DId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        public string D_DepartmentAndStaffFrm_GetStaffQuantity()
        {
            strSQL = "select distinct SActualNo,SNo,SSex from T_StaffInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                int iSSex_M = 0;
                int iSSex_W = 0;
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    if (myTable.Rows[i]["SSex"].ToString().Trim() == "男")
                    {
                        iSSex_M++;
                    }
                    else if (myTable.Rows[i]["SSex"].ToString().Trim() == "女")
                    {
                        iSSex_W++;
                    }
                }
                return "共有员工档案:" + myTable.Rows.Count.ToString() + " 条,其中男:" + iSSex_M.ToString() + "  女:" + iSSex_W.ToString();

            }
            return "共有员工档案:0 条";
        }

        #region //自动产生员工编号
        public void D_DepartmentAndStaffFrm_AddDStaff_SNo(string para_strDName, string para_strDId, out string para_strSNo_Temp, out string para_strBillId_Old, out bool para_blBillId)
        {
            para_strDName = para_strDName.Replace("（", "");
            para_strDName = para_strDName.Replace("）", "");
            para_strDName = para_strDName.Replace("(", "");
            para_strDName = para_strDName.Replace("(", "");
            para_strDName = para_strDName.Replace(" ", "");
            para_strSNo_Temp = "";
            para_strBillId_Old = "";
            para_blBillId = false;
            string strGDetailName_JX = MCodeAll(para_strDName);
            string strTempID = "";
            try
            {
                myTable = SQLHelper.DTQuery("select max(SNo) SNo from T_StaffInf where SDId ='" + para_strDId + "'");
                if (myTable.Rows.Count <= 0)
                {
                    #region
                    myTable = SQLHelper.DTQuery("select BNo from T_NumberTableInf where Flag ='" + "T_StaffInf_SNo" + "'");
                    if (myTable.Rows.Count <= 0)
                    {
                        para_strSNo_Temp = strGDetailName_JX + "0001";//默认为0001
                        para_strBillId_Old = "";
                        SQLHelper.ExecuteSql("insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strSNo_Temp.Trim() + "','" + "T_StaffInf_SNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        string maxID = myTable.Rows[0]["BNo"].ToString().Trim();
                        para_strBillId_Old = maxID;
                        string strBNo_Old_ZF = "";//员工编号字符部分
                        string strBNo_Old_SZ = "0";//员工编号数字部分
                        for (int i = 0; i < maxID.Length; i++)
                        {
                            strSQL = maxID.Substring(i, 1).Trim();
                            if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                            {
                                strBNo_Old_ZF += strSQL;
                                continue;
                            }
                            else
                            {
                                strBNo_Old_SZ = maxID.Substring(i);
                                break;
                            }
                        }
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        para_strSNo_Temp = strBNo_Old_ZF + strTempID.Trim();
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + para_strSNo_Temp.Trim() + "' where Flag ='" + "T_StaffInf_SNo" + "'");
                    }
                    para_blBillId = true;
                    #endregion
                }
                else
                {
                    #region
                    string maxID = myTable.Rows[0]["SNo"].ToString().Trim();
                    para_strBillId_Old = maxID;
                    string strBNo_Old_ZF = "";//员工编号字符部分
                    string strBNo_Old_SZ = "";//员工编号数字部分
                    for (int i = 0; i < maxID.Length; i++)
                    {
                        strSQL = maxID.Substring(i, 1).Trim();
                        if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                        {
                            strBNo_Old_ZF += strSQL;
                            continue;
                        }
                        else
                        {
                            strBNo_Old_SZ = maxID.Substring(i);
                            break;
                        }
                    }
                    if (strBNo_Old_SZ.Trim() != "")
                    {
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        para_strSNo_Temp = strBNo_Old_ZF + strTempID.Trim();
                    }
                    else
                    {
                        para_strSNo_Temp = strGDetailName_JX + "0001";//默认为0001
                        para_strBillId_Old = "";
                        if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_StaffInf_SNo" + "'").Rows.Count <= 0)
                        {
                            SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo )Values('" + para_strSNo_Temp.Trim() + "','" + "T_StaffInf_SNo" + "','" + "" + "','" + "" + "')");
                        }
                    }
                    if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_StaffInf_SNo" + "'").Rows.Count <= 0)
                    {
                        SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strSNo_Temp.Trim() + "','" + "T_StaffInf_SNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + para_strSNo_Temp.Trim() + "' where Flag='" + "T_StaffInf_SNo" + "'");
                    }
                    #endregion
                }
                para_blBillId = true;

            }
            catch
            {

                try
                {
                    #region
                    string maxID = myTable.Rows[0]["SNo"].ToString().Trim();
                    para_strBillId_Old = maxID;
                    string strBNo_Old_ZF = "";//员工编号字符部分
                    string strBNo_Old_SZ = "";//员工编号数字部分
                    for (int i = maxID.Length - 1; i >= 0; i--)
                    {
                        strSQL = maxID.Substring(i, 1).Trim();
                        if (strSQL.CompareTo("0") >= 0 && strSQL.CompareTo("9") <= 0)
                        {
                            strBNo_Old_SZ = strSQL + strBNo_Old_SZ;
                            continue;
                        }
                        else
                        {
                            strBNo_Old_ZF = maxID.Substring(0, i + 1);
                            break;
                        }
                    }
                    if (strBNo_Old_SZ.Trim() != "")
                    {
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        para_strSNo_Temp = strBNo_Old_ZF + strTempID.Trim();
                    }
                    if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_StaffInf_SNo" + "'").Rows.Count <= 0)
                    {
                        SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + para_strSNo_Temp.Trim() + "','" + "T_StaffInf_SNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + para_strSNo_Temp.Trim() + "' where Flag='" + "T_StaffInf_SNo" + "'");
                    }
                    #endregion
                }
                catch
                {
                    para_strSNo_Temp = strGDetailName_JX + "0001";
                    // MessageBox.Show("门卫编号自动识别失败，请手工输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region //自动产生员工实际编号
        public void D_DepartmentAndStaffFrm_AddDStaff_SActualNo(out  string para_strSActualNo)
        {
            para_strSActualNo = "0000000001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(SActualNo) SActualNo from T_StaffInf order by  SActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["SActualNo"].ToString().Trim() != "" && myTable.Rows[0]["SActualNo"].ToString().Trim() != null)
                    {
                        int iSActualNoNew = Convert.ToInt16(myTable.Rows[0]["SActualNo"].ToString()) + 1;
                        switch (iSActualNoNew.ToString().Length)
                        {
                            case 1:
                                para_strSActualNo = "000000000" + iSActualNoNew.ToString(); break;
                            case 2:
                                para_strSActualNo = "00000000" + iSActualNoNew.ToString(); break;
                            case 3:
                                para_strSActualNo = "0000000" + iSActualNoNew.ToString(); break;
                            case 4:
                                para_strSActualNo = "000000" + iSActualNoNew.ToString(); break;
                            case 5:
                                para_strSActualNo = "00000" + iSActualNoNew.ToString(); break;
                            case 6:
                                para_strSActualNo = "0000" + iSActualNoNew.ToString(); break;
                            case 7:
                                para_strSActualNo = "000" + iSActualNoNew.ToString(); break;
                            case 8:
                                para_strSActualNo = "00" + iSActualNoNew.ToString(); break;
                            case 9:
                                para_strSActualNo = "0" + iSActualNoNew.ToString(); break;
                            default:
                                para_strSActualNo = iSActualNoNew.ToString(); break;
 
                        }
                    }
                }
                // return strDActualNo;
            }
            catch
            {
                //  return strDActualNo;

            }
        }

        #endregion

        #region//删除员工档案
        public string D_DepartmentAndStaffFrm_DeleteStaffInf(string para_strSActualNo, string para_strFilter, string para_strFilter_CustomItemInf, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条员工档案
            {
                strSQL = "delete from T_StaffInf where T_StaffInf.SActualNo ='" + para_strSActualNo + "';";
                sqlList.AppendFormat(strSQL);
                strSQL = "delete from T_StaffUCIInf where SActualNo ='" + para_strSActualNo + "' and Flag ='" + "T_StaffInf" + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    strSQL = "select id from T_StaffInf ";
                    DataTable dtTemp = SQLHelper.DTQuery(strSQL);
                    if (dtTemp.Rows.Count <= 0)
                    {
                        sqlList.Length = 0;
                        strSQL = "delete from T_StaffInf ";
                        sqlList.AppendFormat(strSQL);
                        strSQL = "delete from T_StaffUCIInf ";
                        sqlList.AppendFormat(strSQL);
                        strSQL = "delete from T_NumberTableInf where Flag='" + "T_StaffInf_SNo" + "'; ";
                        sqlList.AppendFormat(strSQL);
                        if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                        {

                        }

                    }
                    return "1";
                }
            }
            else//删除多选员工档案
            {
                strSQL = "delete from T_StaffInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                strSQL = "delete from T_StaffUCIInf where " + para_strFilter_CustomItemInf + " and Flag ='" + "T_StaffInf" + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    strSQL = "select id from T_StaffInf ";
                    DataTable dtTemp = SQLHelper.DTQuery(strSQL);
                    if (dtTemp.Rows.Count <= 0)
                    {
                        sqlList.Length = 0;
                        strSQL = "delete from T_StaffInf ";
                        sqlList.AppendFormat(strSQL);
                        strSQL = "delete from T_StaffUCIInf ";
                        sqlList.AppendFormat(strSQL);
                        strSQL = "delete from T_NumberTableInf where Flag='" + "T_StaffInf_SNo" + "'; ";
                        sqlList.AppendFormat(strSQL);
                        if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                        {

                        }

                    }
                    return "1";
                }
            }
            return "0";
        }
        #endregion

        public string D_DepartmentAndStaffFrm_LoadDepartmentInf(TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_DepartmentCategroyInf  order by DId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "DItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["DName"].ToString().Trim());
                            node.Name = DVRow["DId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["DItemLevel"]);
                            para_TV.Nodes.Add(node);
                            D_DepartmentAndStaffFrm_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
                return "0";
            }
        }

        #region//更改员工所在部门
        public string D_DepartmentAndStaffFrm_EditCurDepartment(DataTable para_DT, string para_strDId, string para_strSDDetailName)
        {
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  T_StaffInf.SActualNo  = " + "'" + dt.Rows[i]["SActualNo"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_StaffInf set SDId='" + para_strDId + "',SDDetailName ='" + para_strSDDetailName + "' where " + strFilter + "";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }


                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }
        #endregion

        #region//更改员工所属二道岗亭
        public string D_DepartmentAndStaffFrm_EditCurSecondWay(DataTable para_DT,  string para_strSSecondWay)
        {
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  T_StaffInf.SActualNo  = " + "'" + dt.Rows[i]["SActualNo"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_StaffInf set SSecondWay='"+para_strSSecondWay+  "' where " + strFilter + "";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }


                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }
        #endregion

        #region//选择查询
        public DataTable D_DepartmentAndStaffFrm_StaffSearch(string para_strStaffSearch, string para_strDDetailName, string para_strStaffSearch_Temp, string para_strDisplayOrder)
        {
            strSQL = para_strStaffSearch;
            if (strSQL.Contains("T_StaffInf.SDDetailName") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("T_StaffInf.SDDetailName"));
                if (para_strDDetailName == "")//表示在全部类别中查询
                {
                    strSQL = strSQL.Substring(0, strSQL.Length - 5);
                }
                else//表示在当前类别中查询
                {
                    strSQL += " T_StaffInf.SDDetailName ='" + para_strDDetailName.Trim () + "'";
                }
            }
            if (strSQL.Contains("order") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("order"));
            }
            if (strSQL.Contains("orde") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("orde"));
            }
            strSQL += "  and " + para_strStaffSearch_Temp + para_strDisplayOrder;
            myTable = SQLHelper.DTQuery(strSQL);
            return myTable;
        }
        #endregion
        #endregion

        #region//1BasisFiles_E_BlacklistClassAndInfFrm
        public string E_BlacklistClassAndInf_LoadBlacklistClassInf(TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_BlacklistCategroyInf  order by BId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "BItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["BName"].ToString().Trim());
                            node.Name = DVRow["BId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["BItemLevel"]);
                            para_TV.Nodes.Add(node);
                            E_BlacklistClassAndInf_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string E_BlacklistClassAndInf_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "BParentIndex = '" + para_parentRow["BId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["BName"].ToString().Trim());
                    Node.Name = DVRow["BId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt32(DVRow["BItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    E_BlacklistClassAndInf_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string E_BlacklistClassAndInf_AddBPCInf(TreeView para_TV, string para_strBName, string para_strBDetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select BItemLevel,BParentIndex from T_BlacklistCategroyInf where BId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strBDetailName = "";
                            B_BuildingAndRoomFrm_GetBDetailName1(out strBDetailName, para_strBDetailName, para_TV.SelectedNode, false, para_strBName);
                            strBDetailName = para_strBDetailName + "/" + para_strBName;
                            TreeNode NodeNew = new TreeNode(para_strBName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strBItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["BItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strBItemLevelNew);
                            string str6 = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_BlacklistCategroyInf(BName,BItemLevel,BParentIndex,BDetailName,BMCode )Values('" + para_strBName + "',";
                            strSQL += "'" + strBItemLevelNew + "','" + str6 + "','" + strBDetailName + "','" + MCodeAll(strBDetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(BId) BId from T_BlacklistCategroyInf where BName='" + para_strBName + "' and BParentIndex='" + str6 + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["BId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        B_BuildingAndRoomFrm_GetBDetailName1(out str, para_strBDetailName, para_TV.SelectedNode, true, para_strBName);
                        if (para_strBDetailName.Contains("/"))
                        {
                            str = para_strBDetailName.Substring(0, para_strBDetailName.LastIndexOf("/")) + "/" + para_strBName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strBName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strBItemLevelNew = myTable.Rows[0]["BItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strBItemLevelNew);
                        string strBParentIndexNew = myTable.Rows[0]["BParentIndex"].ToString();
                        strSQL = "insert into T_BlacklistCategroyInf(BName,BItemLevel,BParentIndex,BDetailName,BMCode)Values('" + para_strBName + "',";
                        strSQL += "'" + strBItemLevelNew + "','" + strBParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(BId) BId from T_BlacklistCategroyInf where BName='" + para_strBName + "' and BParentIndex='" + strBParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["BId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_BlacklistCategroyInf(BName,BItemLevel,BParentIndex,BDetailName,BMCode )Values('" + para_strBName + "',";
                    strSQL += "'" + "0" + "','" + "-1" + "','" + para_strBName + "','" + MCodeAll(para_strBName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strBName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(BId) BId from T_BlacklistCategroyInf where BName='" + para_strBName + "' and BParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["BId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string E_BlacklistClassAndInf_GetBDetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }

        public string E_BlacklistClassAndInf_DeleteBPCInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select ID from T_BlacklistInf where BPCId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_BlacklistCategroyInf where BId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        public string E_BlacklistClassAndInf_EditBPCInf(TreeView para_TV)
        {
            if (this.sqlList.Length != 0)
            {
                this.sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                E_BlacklistClassAndInf_RecursionEditBPCAndInf(para_TV.SelectedNode.Parent);
            }
            else
            {
                this.strSQL = "update T_BlacklistInf set BPCDetailName='" + para_TV.SelectedNode.Text.Trim() + "' where T_BlacklistInf.BPCId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                this.strMCode = this.MCodeAll(para_TV.SelectedNode.Text.Trim());
                this.strSQL = "update T_BlacklistCategroyInf set BName ='" + para_TV.SelectedNode.Text.Trim() + "',BDetailName ='" + para_TV.SelectedNode.Text.Trim() + "',BMCode='" + this.strMCode + "' where T_BlacklistCategroyInf.BId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                E_BlacklistClassAndInf_RecursionEditBPCAndInf(para_TV.SelectedNode);
            }
            if ((this.sqlList.Length != 0) && this.SQLHelper.ExecuteNonQueryTran(this.sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                this.sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        private void E_BlacklistClassAndInf_RecursionEditBPCAndInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        E_BlacklistClassAndInf_GetBDetailName(strSQLTemp, para_TNode.Nodes[i]);
                        E_BlacklistClassAndInf_RecursionEditBPCAndInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        E_BlacklistClassAndInf_GetBDetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_BlacklistInf set BPCDetailName ='" + para_TNode.Text.Trim() + "' where T_BlacklistInf.BPCId ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        private void E_BlacklistClassAndInf_GetBDetailName(string para_strGDetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strGDetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_BlacklistInf set BPCDetailName ='" + para_strGDetailName + "' where T_BlacklistInf.BPCId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strGDetailName);
            strSQL = "update T_BlacklistCategroyInf set BDetailName='" + para_strGDetailName + "',BMCode='" + strMCode + "' where T_BlacklistCategroyInf.BId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        public string E_BlacklistClassAndInf_GetBlacklistQuantity()
        {
            strSQL = "select distinct BPActualNo,BPNo,BPSex from T_BlacklistInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                int iSSex_M = 0;
                int iSSex_W = 0;
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    if (myTable.Rows[i]["BPSex"].ToString().Trim() == "男")
                    {
                        iSSex_M++;
                    }
                    else if (myTable.Rows[i]["BPSex"].ToString().Trim() == "女")
                    {
                        iSSex_W++;
                    }
                }
                return "共有人员档案:" + myTable.Rows.Count.ToString() + " 条,其中男:" + iSSex_M.ToString() + "  女:" + iSSex_W.ToString();

            }
            return "共有人员档案:0 条";
        }

        public void E_BlacklistClassAndInf_AddBP_BPActualNo(out  string para_strBPActualNo)
        {
            para_strBPActualNo = "0000000001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(BPActualNo) BPActualNo from T_BlacklistInf order by  BPActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["BPActualNo"].ToString().Trim() != "" && myTable.Rows[0]["BPActualNo"].ToString().Trim() != null)
                    {
                        int iBPActualNoNew = Convert.ToInt16(myTable.Rows[0]["BPActualNo"].ToString()) + 1;
                        switch (iBPActualNoNew.ToString().Length)
                        {
                            case 1:
                                para_strBPActualNo = "000000000" + iBPActualNoNew.ToString(); break;
                            case 2:
                                para_strBPActualNo = "00000000" + iBPActualNoNew.ToString(); break;
                            case 3:
                                para_strBPActualNo = "0000000" + iBPActualNoNew.ToString(); break;
                            case 4:
                                para_strBPActualNo = "000000" + iBPActualNoNew.ToString(); break;
                            case 5:
                                para_strBPActualNo = "00000" + iBPActualNoNew.ToString(); break;
                            case 6:
                                para_strBPActualNo = "0000" + iBPActualNoNew.ToString(); break;
                            case 7:
                                para_strBPActualNo = "000" + iBPActualNoNew.ToString(); break;
                            case 8:
                                para_strBPActualNo = "00" + iBPActualNoNew.ToString(); break;
                            case 9:
                                para_strBPActualNo = "0" + iBPActualNoNew.ToString(); break;
                            default:
                                para_strBPActualNo = iBPActualNoNew.ToString(); break;

                      
                        }
                    }
                }
                // return strDActualNo;
            }
            catch
            {
                //  return strDActualNo;

            }
        }

        public string E_BlacklistClassAndInf_EditCurBLC(DataTable para_DT, string para_strBPCId, string para_strBPCDetailName)
        {
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  T_BlacklistInf.BPActualNo  = " + "'" + dt.Rows[i]["BPActualNo"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_BlacklistInf set BPCId='" + para_strBPCId + "',BPCDetailName ='" + para_strBPCDetailName + "' where " + strFilter + "";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }

        public DataTable E_BlacklistClassAndInf_BPSearch(string para_strBPSearch, string para_strBDetailName, string para_strBPSearch_Temp, string para_strDisplayOrder)
        {
            strSQL = para_strBPSearch;
            if (strSQL.Contains("T_BlacklistInf.BPCDetailName") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("T_BlacklistInf.BPCDetailName"));
                if (para_strBDetailName == "")//表示在全部类别中查询
                {
                    strSQL = strSQL.Substring(0, strSQL.Length - 6);
                }
                else//表示在当前类别中查询
                {
                    strSQL += " T_BlacklistInf.BPCDetailName ='" + para_strBDetailName.Trim () + "'";
                }
            }
            if (strSQL.Contains("order") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("order"));
            }
            if (strSQL.Contains("orde") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("orde"));
            }
            if (strSQL.ToLower().Contains("where") == true)
            {
                strSQL += "  and  " + para_strBPSearch_Temp + para_strDisplayOrder;
            }
            else
            {
                strSQL += "  where  " + para_strBPSearch_Temp + para_strDisplayOrder;
            }
            myTable = SQLHelper.DTQuery(strSQL);
            return myTable;
        }

        public string E_BlacklistClassAndInf_DeleteBPInf(string para_strBPActualNo, string para_strFilter, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条人员档案
            {
                strSQL = "delete from T_BlacklistInf where T_BlacklistInf.BPActualNo ='" + para_strBPActualNo + "';";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return "1";
                }
            }
            else//删除多选人员档案
            {
                strSQL = "delete from T_BlacklistInf where " + para_strFilter + ";";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return "1";
                }
            }
            return "0";
        }
        #endregion

        #region//1BasisFiles_F_WhitelistClassAndInfFrm
        public string F_WhitelistClassAndInf_LoadWhitelistClassInf(TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_WhitelistCategroyInf  order by WId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "WItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["WName"].ToString().Trim());
                            node.Name = DVRow["WId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["WItemLevel"]);
                            para_TV.Nodes.Add(node);
                            F_WhitelistClassAndInf_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string F_WhitelistClassAndInf_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "WParentIndex = '" + para_parentRow["WId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["WName"].ToString().Trim());
                    Node.Name = DVRow["WId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt32(DVRow["WItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    F_WhitelistClassAndInf_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string F_WhitelistClassAndInf_AddWCInf(TreeView para_TV, string para_strWName, string para_strWDetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select WItemLevel,WParentIndex from T_WhitelistCategroyInf where WId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strWDetailName = "";
                            B_BuildingAndRoomFrm_GetBDetailName1(out strWDetailName, para_strWDetailName, para_TV.SelectedNode, false, para_strWName);
                            strWDetailName = para_strWDetailName + "/" + para_strWName;
                            TreeNode NodeNew = new TreeNode(para_strWName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strWItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["WItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strWItemLevelNew);
                            string str6 = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_WhitelistCategroyInf(WName,WItemLevel,WParentIndex,WDetailName,WMCode )Values('" + para_strWName + "',";
                            strSQL += "'" + strWItemLevelNew + "','" + str6 + "','" + strWDetailName + "','" + MCodeAll(strWDetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(WId) WId from T_WhitelistCategroyInf where WName='" + para_strWName + "' and WParentIndex='" + str6 + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["WId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        B_BuildingAndRoomFrm_GetBDetailName1(out str, para_strWDetailName, para_TV.SelectedNode, true, para_strWName);
                        if (para_strWDetailName.Contains("/"))
                        {
                            str = para_strWDetailName.Substring(0, para_strWDetailName.LastIndexOf("/")) + "/" + para_strWName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strWName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strWItemLevelNew = myTable.Rows[0]["WItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strWItemLevelNew);
                        string strWParentIndexNew = myTable.Rows[0]["WParentIndex"].ToString();
                        strSQL = "insert into T_WhitelistCategroyInf(WName,WItemLevel,WParentIndex,WDetailName,WMCode)Values('" + para_strWName + "',";
                        strSQL += "'" + strWItemLevelNew + "','" + strWParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(WId) WId from T_WhitelistCategroyInf where WName='" + para_strWName + "' and WParentIndex='" + strWParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["WId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_WhitelistCategroyInf(WName,WItemLevel,WParentIndex,WDetailName,WMCode )Values('" + para_strWName + "',";
                    strSQL += "'" + "0" + "','" + "-1" + "','" + para_strWName + "','" + MCodeAll(para_strWName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strWName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(WId) WId from T_WhitelistCategroyInf where WName='" + para_strWName + "' and WParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["WId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string F_WhitelistClassAndInf_GetWDetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }

        public string F_WhitelistClassAndInf_GetWhitelistQuantity()
        {
            strSQL = "select distinct WPActualNo,WPNo,WPSex from T_WhitelistInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                int iSSex_M = 0;
                int iSSex_W = 0;
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    if (myTable.Rows[i]["WPSex"].ToString().Trim() == "男")
                    {
                        iSSex_M++;
                    }
                    else if (myTable.Rows[i]["WPSex"].ToString().Trim() == "女")
                    {
                        iSSex_W++;
                    }
                }
                return "共有人员档案:" + myTable.Rows.Count.ToString() + " 条,其中男:" + iSSex_M.ToString() + "  女:" + iSSex_W.ToString();

            }
            return "共有人员档案:0 条";
        }

        public string F_WhitelistClassAndInf_EditWCInf(TreeView para_TV)
        {
            if (this.sqlList.Length != 0)
            {
                this.sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                F_WhitelistClassAndInf_RecursionEditWCAndInf(para_TV.SelectedNode.Parent);
            }
            else
            {
                this.strSQL = "update T_WhitelistInf set WPCDetailName='" + para_TV.SelectedNode.Text.Trim() + "' where T_WhitelistInf.WPCId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                this.strMCode = this.MCodeAll(para_TV.SelectedNode.Text.Trim());
                this.strSQL = "update T_WhitelistCategroyInf set WName ='" + para_TV.SelectedNode.Text.Trim() + "',WDetailName ='" + para_TV.SelectedNode.Text.Trim() + "',WMCode='" + this.strMCode + "' where T_WhitelistCategroyInf.WId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                F_WhitelistClassAndInf_RecursionEditWCAndInf(para_TV.SelectedNode);
            }
            if ((this.sqlList.Length != 0) && this.SQLHelper.ExecuteNonQueryTran(this.sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                this.sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        private void F_WhitelistClassAndInf_RecursionEditWCAndInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        F_WhitelistClassAndInf_GetWDetailName(strSQLTemp, para_TNode.Nodes[i]);
                        F_WhitelistClassAndInf_RecursionEditWCAndInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        F_WhitelistClassAndInf_GetWDetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_WhitelistInf set WPCDetailName ='" + para_TNode.Text.Trim() + "' where T_WhitelistInf.WPCId ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        private void F_WhitelistClassAndInf_GetWDetailName(string para_strGDetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strGDetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_WhitelistInf set WPCDetailName ='" + para_strGDetailName + "' where T_WhitelistInf.WPCId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strGDetailName);
            strSQL = "update T_WhitelistCategroyInf set WDetailName='" + para_strGDetailName + "',WMCode='" + strMCode + "' where T_WhitelistCategroyInf.WId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        public string F_WhitelistClassAndInf_DeleteWCInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select ID from T_WhitelistInf where WPCId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_WhitelistCategroyInf where WId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        public void F_WhitelistClassAndInf_AddWP_WPActualNo(out  string para_strWPActualNo)
        {
            para_strWPActualNo = "0000000001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(WPActualNo) WPActualNo from T_WhitelistInf order by  WPActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["WPActualNo"].ToString().Trim() != "" && myTable.Rows[0]["WPActualNo"].ToString().Trim() != null)
                    {
                        int iBPActualNoNew = Convert.ToInt16(myTable.Rows[0]["WPActualNo"].ToString()) + 1;
                        switch (iBPActualNoNew.ToString().Length)
                        {
                            case 1:
                                para_strWPActualNo = "000000000" + iBPActualNoNew.ToString(); break;
                            case 2:
                                para_strWPActualNo = "00000000" + iBPActualNoNew.ToString(); break;
                            case 3:
                                para_strWPActualNo = "0000000" + iBPActualNoNew.ToString(); break;
                            case 4:
                                para_strWPActualNo = "000000" + iBPActualNoNew.ToString(); break;
                            case 5:
                                para_strWPActualNo = "00000" + iBPActualNoNew.ToString(); break;
                            case 6:
                                para_strWPActualNo = "0000" + iBPActualNoNew.ToString(); break;
                            case 7:
                                para_strWPActualNo = "000" + iBPActualNoNew.ToString(); break;
                            case 8:
                                para_strWPActualNo = "00" + iBPActualNoNew.ToString(); break;
                            case 9:
                                para_strWPActualNo = "0" + iBPActualNoNew.ToString(); break;
                            default:
                                para_strWPActualNo = iBPActualNoNew.ToString(); break;
                                 
                        }
                    }
                }
                // return strDActualNo;
            }
            catch
            {
                //  return strDActualNo;

            }
        }

        public string F_WhitelistClassAndInf_DeleteWPInf(string para_strWPActualNo, string para_strFilter, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条人员档案
            {
                strSQL = "delete from T_WhitelistInf where T_WhitelistInf.WPActualNo ='" + para_strWPActualNo + "';";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return "1";
                }
            }
            else//删除多选人员档案
            {
                strSQL = "delete from T_WhitelistInf where " + para_strFilter + ";";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return "1";
                }
            }
            return "0";
        }

        public string F_WhitelistClassAndInf_EditCurWLC(DataTable para_DT, string para_strWPCId, string para_strWPCDetailName)
        {
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  T_WhitelistInf.WPActualNo  = " + "'" + dt.Rows[i]["WPActualNo"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_WhitelistInf set WPCId='" + para_strWPCId + "',WPCDetailName ='" + para_strWPCDetailName + "' where " + strFilter + "";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }

        public DataTable F_WhitelistClassAndInf_WPSearch(string para_strWPSearch, string para_strWDetailName, string para_strWPSearch_Temp, string para_strDisplayOrder)
        {
            strSQL = para_strWPSearch;
            if (strSQL.Contains("T_WhitelistInf.WPCDetailName") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("T_WhitelistInf.WPCDetailName"));
                if (para_strWDetailName == "")//表示在全部类别中查询
                {
                    strSQL = strSQL.Substring(0, strSQL.Length - 6);
                }
                else//表示在当前类别中查询
                {
                    strSQL += " T_WhitelistInf.WPCDetailName ='" + para_strWDetailName.Trim() + "'";
                }
            }
            if (strSQL.Contains("order") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("order"));
            }
            if (strSQL.Contains("orde") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("orde"));
            }
            if (strSQL.ToLower().Contains("where") == true)
            {
                strSQL += "  and  " + para_strWPSearch_Temp + para_strDisplayOrder;
            }
            else
            {
                strSQL += "  where  " + para_strWPSearch_Temp + para_strDisplayOrder;
            }
            myTable = SQLHelper.DTQuery(strSQL);
            return myTable;
        }


        public string G_AOPersonsClassAndInf_AddAOCInf(TreeView para_TV, string para_strAName, string para_strADetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select AItemLevel,AParentIndex from T_AOPersonCategroyInf where AId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strADetailName = "";
                            B_BuildingAndRoomFrm_GetBDetailName1(out strADetailName, para_strADetailName, para_TV.SelectedNode, false, para_strAName);
                            strADetailName = para_strADetailName + "/" + para_strAName;
                            TreeNode NodeNew = new TreeNode(para_strAName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strWItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["AItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strWItemLevelNew);
                            string str6 = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_AOPersonCategroyInf(AName,AItemLevel,AParentIndex,ADetailName,AMCode )Values('" + para_strAName + "',";
                            strSQL += "'" + strWItemLevelNew + "','" + str6 + "','" + strADetailName + "','" + MCodeAll(strADetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(AId) AId from T_AOPersonCategroyInf where AName='" + para_strAName + "' and AParentIndex='" + str6 + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["AId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        B_BuildingAndRoomFrm_GetBDetailName1(out str, para_strADetailName, para_TV.SelectedNode, true, para_strAName);
                        if (para_strADetailName.Contains("/"))
                        {
                            str = para_strADetailName.Substring(0, para_strADetailName.LastIndexOf("/")) + "/" + para_strAName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strAName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strAItemLevelNew = myTable.Rows[0]["AItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strAItemLevelNew);
                        string strWParentIndexNew = myTable.Rows[0]["AParentIndex"].ToString();
                        strSQL = "insert into T_AOPersonCategroyInf(AName,AItemLevel,AParentIndex,ADetailName,AMCode)Values('" + para_strAName + "',";
                        strSQL += "'" + strAItemLevelNew + "','" + strWParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(AId) AId from T_AOPersonCategroyInf where AName='" + para_strAName + "' and AParentIndex='" + strWParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["AId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_AOPersonCategroyInf(AName,AItemLevel,AParentIndex,ADetailName,AMCode )Values('" + para_strAName + "',";
                    strSQL += "'" + "0" + "','" + "-1" + "','" + para_strAName + "','" + MCodeAll(para_strAName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strAName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(AId) AId from T_AOPersonCategroyInf where AName='" + para_strAName + "' and AParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["AId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string G_AOPersonsClassAndInf_LoadAOPClassInf(TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_AOPersonCategroyInf  order by AId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "AItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["AName"].ToString().Trim());
                            node.Name = DVRow["AId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["AItemLevel"]);
                            para_TV.Nodes.Add(node);
                            G_AOPersonsClassAndInf_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string G_AOPersonsClassAndInf_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "AParentIndex = '" + para_parentRow["AId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["AName"].ToString().Trim());
                    Node.Name = DVRow["AId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt32(DVRow["AItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    G_AOPersonsClassAndInf_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string G_AOPersonsClassAndInf_GetADetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }

        public string G_AOPersonsClassAndInf_GetAOPQuantity()
        {
            strSQL = "select distinct AOPActualNo,AOPNo,AOPSex from T_AOPersonInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                int iSSex_M = 0;
                int iSSex_W = 0;
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    if (myTable.Rows[i]["AOPSex"].ToString().Trim() == "男")
                    {
                        iSSex_M++;
                    }
                    else if (myTable.Rows[i]["AOPSex"].ToString().Trim() == "女")
                    {
                        iSSex_W++;
                    }
                }
                return "共有其他人员档案:" + myTable.Rows.Count.ToString() + " 条,其中男:" + iSSex_M.ToString() + "  女:" + iSSex_W.ToString();

            }
            return "共有其他人员档案:0 条";
        }

 

        public string G_AOPersonsClassAndInf_DeleteACInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select ID from T_AOPersonInf where AOPCId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_AOPersonCategroyInf where AId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        public string G_AOPersonsClassAndInf_EditACInf(TreeView para_TV)
        {
            if (this.sqlList.Length != 0)
            {
                this.sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                G_AOPersonsClassAndInf_RecursionEditACAndInf(para_TV.SelectedNode.Parent);
            }
            else
            {
                this.strSQL = "update T_AOPersonInf set AOPCDetailName='" + para_TV.SelectedNode.Text.Trim() + "' where T_AOPersonInf.AOPCId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                this.strMCode = this.MCodeAll(para_TV.SelectedNode.Text.Trim());
                this.strSQL = "update T_AOPersonCategroyInf set AName ='" + para_TV.SelectedNode.Text.Trim() + "',ADetailName ='" + para_TV.SelectedNode.Text.Trim() + "',AMCode='" + this.strMCode + "' where T_AOPersonCategroyInf.AId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                G_AOPersonsClassAndInf_RecursionEditACAndInf(para_TV.SelectedNode);
            }
            if ((this.sqlList.Length != 0) && this.SQLHelper.ExecuteNonQueryTran(this.sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                this.sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        private void G_AOPersonsClassAndInf_RecursionEditACAndInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        G_AOPersonsClassAndInf_GetADetailName(strSQLTemp, para_TNode.Nodes[i]);
                        G_AOPersonsClassAndInf_RecursionEditACAndInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        G_AOPersonsClassAndInf_GetADetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_AOPersonInf set AOPCDetailName ='" + para_TNode.Text.Trim() + "' where T_AOPersonInf.AOPCId ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        private void G_AOPersonsClassAndInf_GetADetailName(string para_strADetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strADetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strADetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strADetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strADetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strADetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strADetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_AOPersonInf set AOPCDetailName ='" + para_strADetailName + "' where T_AOPersonInf.AOPCId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strADetailName);
            strSQL = "update T_AOPersonCategroyInf set ADetailName='" + para_strADetailName + "',AMCode='" + strMCode + "' where T_AOPersonCategroyInf.AId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        public void G_AOPersonsClassAndInf_AddAOP_AOPActualNo(out  string para_strAOPActualNo)
        {
            para_strAOPActualNo = "0000000001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(AOPActualNo) AOPActualNo from T_AOPersonInf order by  AOPActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["AOPActualNo"].ToString().Trim() != "" && myTable.Rows[0]["AOPActualNo"].ToString().Trim() != null)
                    {
                        int iBPActualNoNew = Convert.ToInt16(myTable.Rows[0]["AOPActualNo"].ToString()) + 1;
                        switch (iBPActualNoNew.ToString().Length)
                        {
                            case 1:
                                para_strAOPActualNo = "000000000" + iBPActualNoNew.ToString(); break;
                            case 2:
                                para_strAOPActualNo = "00000000" + iBPActualNoNew.ToString(); break;
                            case 3:
                                para_strAOPActualNo = "0000000" + iBPActualNoNew.ToString(); break;
                            case 4:
                                para_strAOPActualNo = "000000" + iBPActualNoNew.ToString(); break;
                            case 5:
                                para_strAOPActualNo = "00000" + iBPActualNoNew.ToString(); break;
                            case 6:
                                para_strAOPActualNo = "0000" + iBPActualNoNew.ToString(); break;
                            case 7:
                                para_strAOPActualNo = "000" + iBPActualNoNew.ToString(); break;
                            case 8:
                                para_strAOPActualNo = "00" + iBPActualNoNew.ToString(); break;
                            case 9:
                                para_strAOPActualNo = "0" + iBPActualNoNew.ToString(); break;
                            default:
                                para_strAOPActualNo = iBPActualNoNew.ToString(); break;

                          
                        }
                    }
                }
                // return strDActualNo;
            }
            catch
            {
                //  return strDActualNo;

            }
        }

        public string G_AOPersonsClassAndInf_DeleteAOPInf(string para_strAOPActualNo, string para_strFilter, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条人员档案
            {
                strSQL = "delete from T_AOPersonInf where T_AOPersonInf.AOPActualNo ='" + para_strAOPActualNo + "';";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return "1";
                }
            }
            else//删除多选人员档案
            {
                strSQL = "delete from T_AOPersonInf where " + para_strFilter + ";";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return "1";
                }
            }
            return "0";
        }

        public string G_AOPersonsClassAndInf_EditCurALC(DataTable para_DT, string para_strAOPCId, string para_strAOPCDetailName)
        {
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  T_AOPersonInf.AOPActualNo  = " + "'" + dt.Rows[i]["AOPActualNo"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_AOPersonInf set AOPCId='" + para_strAOPCId + "',AOPCDetailName ='" + para_strAOPCDetailName + "' where " + strFilter + "";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }



        public DataTable G_AOPersonsClassAndInf_AOPSearch(string para_strWPSearch, string para_strWDetailName, string para_strWPSearch_Temp, string para_strDisplayOrder)
        {
            strSQL = para_strWPSearch;
            if (strSQL.Contains("T_AOPersonInf.AOPCDetailName") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("T_AOPersonInf.AOPCDetailName"));
                if (para_strWDetailName == "")//表示在全部类别中查询
                {
                    strSQL = strSQL.Substring(0, strSQL.Length - 6);
                }
                else//表示在当前类别中查询
                {
                    strSQL += " T_AOPersonInf.AOPCDetailName ='" + para_strWDetailName.Trim() + "'";
                }
            }
            if (strSQL.Contains("order") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("order"));
            }
            if (strSQL.Contains("orde") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("orde"));
            }
            if (strSQL.ToLower().Contains("where") == true)
            {
                strSQL += "  and  " + para_strWPSearch_Temp + para_strDisplayOrder;
            }
            else
            {
                strSQL += "  where  " + para_strWPSearch_Temp + para_strDisplayOrder;
            }
            myTable = SQLHelper.DTQuery(strSQL);
            return myTable;
        }
        #endregion

        #region//1BasisFiles_I_CertificateAndBarCodeFrm
        public string I_CertificateAndBarCodeFrm_LoadCertificateInf(string para_strFlag, string para_strCondition, TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                if (para_strFlag != "" && para_strCondition != "")
                {
                    strSQL = "select * from XXCLOUD.dbo.T_BaseCategroyInf where Flag='" + para_strFlag + "' and BClassName like '%" + para_strCondition + "%'  order by BSNo ";
                }
                else if (para_strFlag != "" && para_strCondition == "")
                {
                    strSQL = "select * from XXCLOUD.dbo.T_BaseCategroyInf where Flag='" + para_strFlag + "' order by BSNo ";
                }
                else if (para_strFlag == "" && para_strCondition == "")
                {
                    strSQL = "select * from XXCLOUD.dbo.T_BaseCategroyInf  order by BSNo ";
                }
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    for (int i = 0; i < myTable.Rows.Count; i++)
                    {
                        TreeNode node = new TreeNode(myTable.Rows[i]["BClassName"].ToString().Trim());
                        node.Name = myTable.Rows[i]["BClassId"].ToString().Trim();
                        node.ImageIndex = Convert.ToInt32(0);
                        para_TV.Nodes.Add(node);
                    }

                    para_TV.ExpandAll();

                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        #region //自动产生证件档案实际编号
        public void I_CertificateAndBarCodeFrm_AddCertificate_CertificateActualNo(out  string para_strActualNo)
        {
            para_strActualNo = "0000000001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(ActualNo) ActualNo from T_CertificateAndBarCodeInf order by  ActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["ActualNo"].ToString().Trim() != "" && myTable.Rows[0]["ActualNo"].ToString().Trim() != null)
                    {
                        int iDActualNoNew = Convert.ToInt16(myTable.Rows[0]["ActualNo"].ToString()) + 1;
                        switch (iDActualNoNew.ToString().Length)
                        {
                            case 1:
                                para_strActualNo = "000000000" + iDActualNoNew.ToString(); break;
                            case 2:
                                para_strActualNo = "00000000" + iDActualNoNew.ToString(); break;
                            case 3:
                                para_strActualNo = "0000000" + iDActualNoNew.ToString(); break;
                            case 4:
                                para_strActualNo = "000000" + iDActualNoNew.ToString(); break;
                            case 5:
                                para_strActualNo = "00000" + iDActualNoNew.ToString(); break;
                            case 6:
                                para_strActualNo = "0000" + iDActualNoNew.ToString(); break;
                            case 7:
                                para_strActualNo = "000" + iDActualNoNew.ToString(); break;
                            case 8:
                                para_strActualNo = "00" + iDActualNoNew.ToString(); break;
                            case 9:
                                para_strActualNo = "0" + iDActualNoNew.ToString(); break;
                            default:
                                para_strActualNo = iDActualNoNew.ToString(); break;
                                 
                        }
                    }
                }
                // return strDActualNo;
            }
            catch
            {
                //  return strDActualNo;

            }
        }

        #endregion

        #region//统计证件档案数量
        public string I_CertificateAndBarCodeFrm_GetCertificateQuantity()
        {
            strSQL = "select distinct Name,IdNo,Sex from T_CertificateAndBarCodeInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                int iSSex_M = 0;
                int iSSex_W = 0;
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    if (myTable.Rows[i]["Sex"].ToString().Trim() == "男")
                    {
                        iSSex_M++;
                    }
                    else if (myTable.Rows[i]["Sex"].ToString().Trim() == "女")
                    {
                        iSSex_W++;
                    }
                }
                return "共有证件档案:" + myTable.Rows.Count.ToString() + " 条,其中男:" + iSSex_M.ToString() + "  女:" + iSSex_W.ToString();

            }
            return "共有证件档案:0 条";
        }
        #endregion

        public string I_CertificateAndBarCodeFrm_DeleteCertificateInf(string para_strId, string para_strFilter, bool para_blFlag)
        {
            if (para_blFlag == false)
            {
                strSQL = "delete from T_CertificateAndBarCodeInf where Id ='" + para_strId + "'";
            }
            else
            {
                strSQL = "delete from T_CertificateAndBarCodeInf where  " + para_strFilter;
            }
            if (SQLHelper.ExecuteSql(strSQL) != 0)
            {
                return "1";
            }
            return "0";
        }

        #endregion

        #region//2QueryStatistics_A_VisitorGOFrm
        #region//删除来访记录
        public string A_VisitorGOFrm_DeleteVisitorGOInf(string para_strGONo, string para_strFilter, string para_strCNo, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条进出记录
            {
                strSQL = "delete from YEAR" + para_strCNo + ".dbo.T_VisitorAccessInf where GONo ='" + para_strGONo + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//删除多选进出记录
            {
                strSQL = "delete from YEAR" + para_strCNo + ".dbo.T_VisitorAccessInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }
        #endregion

        #region//删除内部员工进出记录
        public string A_VisitorGOFrm_DeleteStaffGOInf(string para_strId, string para_strFilter, string para_strCNo, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条进出记录
            {
                strSQL = "delete from YEAR" + para_strCNo + ".dbo.T_StaffAccessInf where Id ='" + para_strId + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//删除多选进出记录
            {
                strSQL = "delete from YEAR" + para_strCNo + ".dbo.T_StaffAccessInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }
        #endregion

   
        #endregion

        #region//2QueryStatistics_C_VisitorFrm
        #region//将历史访客的非正常签离次数清零
        public string C_VisitorFrm_VAbnormalLCountToZero(DataTable para_DT )
        {
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  T_HistoryVisitorInf.Id  = " + "'" + dt.Rows[i]["Id"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_HistoryVisitorInf set VAbnormalLCount='" + "0" + "' where " + strFilter + "";
                    if (SQLHelper.ExecuteSql(strSQL) != 0)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }


                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }
        #endregion
        #endregion

        #region//2QueryStatistics_F_VisitorOrderFrm
        #region//删除预约记录
        public string F_VisitorOrderFrm_DeleteVisitorOrderInf(string para_strId, string para_strFilter, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条预约记录
            {
                strSQL = "delete from XXCLOUD.dbo.T_VisitorAppointmenInf where Id ='" + para_strId + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//删除多选预约记录
            {
                strSQL = "delete from XXCLOUD.dbo.T_VisitorAppointmenInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }
        #endregion

        #region//标记预约记录为已经登记过
        public string F_VisitorOrderFrm_SetVisitorOrderInfRegistered(string para_strId, string para_strFilter, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)
            {
                strSQL = "  update T_VisitorAppointmenInf set IsRegistered ='" + "1" + "',ANo='"+LoginFrm.strCAccout.Substring(1, 4).Trim()+"' where Id ='" + para_strId + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else
            {
                strSQL = " update T_VisitorAppointmenInf set IsRegistered ='" + "1" + "',ANo='" + LoginFrm.strCAccout.Substring(1, 4).Trim() + "' where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }
        #endregion

        #region//修改预约记录的批复状态
        public string F_VisitorOrderFrm_ModifyIsAllowedStatus(string para_strId, string para_strFilter, bool para_blFlag,string str_IsAllowed,string para_strAllowedRPNo,string para_strAllowedPRActualNo,string para_strAllowedRPName)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//修改当前选中的一条预约记录
            {
                strSQL = "update  XXCLOUD.dbo.T_VisitorAppointmenInf set IsAllowed='" + str_IsAllowed + "' ,AllowedRPNo ='" + para_strAllowedRPNo + "', ";
                strSQL += " AllowedPRActualNo='" + para_strAllowedPRActualNo + "' ,AllowedRPName='" + para_strAllowedRPName + "' ,AllowedDT='"+System .DateTime.Now.ToString()+"'  where Id ='" + para_strId + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//修改多选预约记录
            {
                strSQL = "update  XXCLOUD.dbo.T_VisitorAppointmenInf set IsAllowed='" + str_IsAllowed + "' ,AllowedRPNo ='" + para_strAllowedRPNo + "', ";
                strSQL += " AllowedPRActualNo='" + para_strAllowedPRActualNo + "' ,AllowedRPName='" + para_strAllowedRPName + "' ,AllowedDT='" + System.DateTime.Now.ToString() + "' where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }
        #endregion
        #endregion

        #region//2QueryStatistics_G_FingerprintVFrm
        #region//删除指纹访客记录
        public string G_FingerprintVFrm_DeleteVPVisitorInf(string para_strId, string para_strFilter, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条预约记录
            {
                strSQL = "delete from XXCLOUD.dbo.T_VisitorFingerprintInf where Id ='" + para_strId + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//删除多选预约记录
            {
                strSQL = "delete from XXCLOUD.dbo.T_VisitorFingerprintInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }
        #endregion

        #region//修改指纹访客记录的批复状态
        public string G_FingerprintVFrm_ModifyVFPIsValidStatus(string para_strId, string para_strFilter, bool para_blFlag, string str_IsAllowed, string para_strAllowedONo, string para_strAllowedOActualNo, string para_strAllowedOName)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//修改当前选中的一条预约记录
            {
                strSQL = "update  XXCLOUD.dbo.T_VisitorFingerprintInf set IsValid='" + str_IsAllowed + "' ,VFPAllowedONo ='" + para_strAllowedONo + "', ";
                strSQL += " VFPAllowedOActualNo='" + para_strAllowedOActualNo + "' ,VFPAllowedOName='" + para_strAllowedOName + "' ,VFPAllowedDT='" + System.DateTime.Now.ToString() + "'  where Id ='" + para_strId + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//修改多选预约记录
            {
                strSQL = "update  XXCLOUD.dbo.T_VisitorFingerprintInf set IsValid='" + str_IsAllowed + "' ,VFPAllowedONo ='" + para_strAllowedONo + "', ";
                strSQL += " VFPAllowedOActualNo='" + para_strAllowedOActualNo + "' ,VFPAllowedOName='" + para_strAllowedOName + "' ,VFPAllowedDT='" + System.DateTime.Now.ToString() + "' where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }
        #endregion
        #endregion

        #region//3SystemMaintenance_A_AccAndOperatorFrm
        public string A_AccAndOperatorFrm_DeleteAccoutInf(string para_strANo, string para_strAccessPwd, string para_strFilter, bool para_blFlag)
        {
            if (para_blFlag == false)
            {
                strSQL = "delete from T_YearInf where ANo ='" + para_strANo + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {

                    try
                    {
                        string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
                        strSQL = "delete from T_LocYearInf where ANo='" + para_strANo + "'  ";
                        System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
                        OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch
                    {

                    }
                    return "1";
                }
            }
            else
            {
                strSQL = "delete from T_YearInf where  " + para_strFilter;
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    try
                    {
                        string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
                        strSQL = "delete from T_LocYearInf where " + para_strFilter;
                        System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
                        OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch
                    {

                    }
                    return "1";
                }
            }
            return "0";
        }

        public string A_AccAndOperatorFrm_DeleteOperatorInf(string para_strONo, string para_strOName, string para_strPower, string para_strAccessPwd, string para_strFilter, bool para_blFlag)
        {
            if (para_blFlag == false)
            {
                if (para_strPower == "后台")
                {
                    strSQL = "select * from T_OperatorInf where Power  ='" + "1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count <= 1)
                    {
                        // MessageBox.Show("无法删除，请保留至少一个具有后台操作权限的操作员，否则无法登录后台!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return "2";
                    }
                }
                strSQL = "delete from T_OperatorInf where ONo='" + para_strONo + "' and OName='" + para_strOName + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    try
                    {
                        string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
                        strSQL = "delete from T_LocOperatorInf where ONo='" + para_strONo + "' and OName='" + para_strOName + "'";
                        System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
                        OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch
                    {

                    }
                    return "1";
                }
            }
            else
            {
                strSQL = "delete from T_OperatorInf where " + para_strFilter + ";";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    try
                    {
                        string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
                        strSQL = "delete from T_LocOperatorInf where " + para_strFilter;
                        System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
                        OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch
                    {

                    }
                    return "1";
                }
            }
            return "0";
        }

        public string A_AccAndOperatorFrm_DeleteAccoutInf_Loc(string para_strANo, string para_strAccessPwd, string para_strFilter, bool para_blFlag)
        {
            try
            {
                if (para_blFlag == false)
                {
                    strSQL = "delete from T_LocYearInf where ANo ='" + para_strANo + "'";
                }
                else
                {
                    strSQL = "delete from T_LocYearInf where  " + para_strFilter;
                }
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
                System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
                OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string A_AccAndOperatorFrm_DeleteOperatorInf_Loc(string para_strONo, string para_strAccessPwd, string para_strFilter, bool para_blFlag)
        {
            try
            {
                if (para_blFlag == false)
                {
                    strSQL = "delete from T_LocOperatorInf where ONo ='" + para_strONo + "'";
                }
                else
                {
                    strSQL = "delete from T_LocOperatorInf where  " + para_strFilter;
                }
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\XXCLOUD.mdb" + ";Jet OLEDB:Database Password='" + para_strAccessPwd + "'";
                System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
                OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string A_AccAndOperatorFrm_AddOperator_OperatorNo()
        {
            string strON0 = "001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(ONo) ONo from T_OperatorInf order by  ONo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["ONo"].ToString().Trim() != "" && myTable.Rows[0]["ONo"].ToString().Trim() != null)
                    {
                        int iONoNew = Convert.ToInt16(myTable.Rows[0]["ONo"].ToString()) + 1;
                        switch (iONoNew.ToString().Length)
                        {
                            case 1:
                                strON0 = "00" + iONoNew.ToString(); break;
                            case 2:
                                strON0 = "0" + iONoNew.ToString(); break;
                            default:
                                strON0 = iONoNew.ToString(); break;
                        }
                    }
                }
                return strON0;
            }
            catch
            {
                return strON0;

            }
        }

        public string A_AccAndOperatorFrm_AddOperator_OperatorActualNo()
        {
            string strOActualNo = "001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(OActualNo) OActualNo from T_OperatorInf order by  OActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["OActualNo"].ToString().Trim() != "" && myTable.Rows[0]["OActualNo"].ToString().Trim() != null)
                    {
                        int iOActualNo = Convert.ToInt16(myTable.Rows[0]["OActualNo"].ToString()) + 1;
                        switch (iOActualNo.ToString().Length)
                        {
                            case 1:
                                strOActualNo = "00" + iOActualNo.ToString(); break;
                            case 2:
                                strOActualNo = "0" + iOActualNo.ToString(); break;
                            default:
                                strOActualNo = iOActualNo.ToString(); break;
                        }
                    }
                }
                return strOActualNo;
            }
            catch
            {
                return strOActualNo;

            }
        }


        public string A_AccAndOperatorFrm_AddApprover_ApproverNo()
        {
            string strAN0 = "001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(ANo) ANo from T_ApproverInf order by  ANo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["ANo"].ToString().Trim() != "" && myTable.Rows[0]["ANo"].ToString().Trim() != null)
                    {
                        int iONoNew = Convert.ToInt16(myTable.Rows[0]["ANo"].ToString()) + 1;
                        switch (iONoNew.ToString().Length)
                        {
                            case 1:
                                strAN0 = "00" + iONoNew.ToString(); break;
                            case 2:
                                strAN0 = "0" + iONoNew.ToString(); break;
                            default:
                                strAN0 = iONoNew.ToString(); break;
                        }
                    }
                }
                return strAN0;
            }
            catch
            {
                return strAN0;

            }
        }

        public string A_AccAndOperatorFrm_AddApprover_ApproverActualNo()
        {
            string strAActualNo = "001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(AActualNo) AActualNo from T_ApproverInf order by  AActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["AActualNo"].ToString().Trim() != "" && myTable.Rows[0]["AActualNo"].ToString().Trim() != null)
                    {
                        int iOActualNo = Convert.ToInt16(myTable.Rows[0]["AActualNo"].ToString()) + 1;
                        switch (iOActualNo.ToString().Length)
                        {
                            case 1:
                                strAActualNo = "00" + iOActualNo.ToString(); break;
                            case 2:
                                strAActualNo = "0" + iOActualNo.ToString(); break;
                            default:
                                strAActualNo = iOActualNo.ToString(); break;
                        }
                    }
                }
                return strAActualNo;
            }
            catch
            {
                return strAActualNo;

            }
        }

        public string A_AccAndOperatorFrm_DeleteApproverInf(string para_strANo, string para_strAName, string para_strPower, string para_strAccessPwd, string para_strFilter, bool para_blFlag)
        {
            if (para_blFlag == false)
            {
                strSQL = "delete from T_ApproverInf where ANo='" + para_strANo + "' and AName='" + para_strAName + "'";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return "1";
                }
            }
            else
            {
                strSQL = "delete from T_ApproverInf where " + para_strFilter + ";";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return "1";
                }
            }
            return "0";
        }


        public int F_SMSCatFrm_DelOfficerInfByID(string para_strID, string para_strFlag)
        {
            if (para_strFlag == "0")
            {
                strSQL = "delete from T_MSLeaderInf where Id ='" + para_strID + "' ";
                if (SQLHelper.ExecuteSql(strSQL) != 0)
                {
                    return 1;
                }
                return 0;
            }
            strSQL = "delete from T_MSLeaderInf where " + para_strID;
            if (SQLHelper.ExecuteSql(strSQL) != 0)
            {
                return 1;
            }
            return 0;
        }

        #endregion

        #region//4ProsceniumRegisterFrm

        #region //自动产生来访时的登记编号(新方法)
        public string PRegisterFrm_RigisterNo(string para_strANo)
        {

            string strGONOTemp = "";
            //int iGONOLable = 1;
            //int iGONODigits = 7;
            //try
            //{
            //    iGONOLable = Convert.ToInt32(LoginFrm.strGONOLable);
            //}
            //catch
            //{
            //    iGONOLable = 1;
            //}
            //try
            //{
            //    iGONODigits = Convert.ToInt32(LoginFrm.strGONODigits);
            //}
            //catch
            //{
            //    iGONODigits = 8;
            //}
            //strGONOTemp = "D" + iGONOLable.ToString();
            //try
            //{
            //    //查找当前设备最大的出入证号
            //    strSQL = "select max(GONO) GONO from " + XXY_VisitorMJAsst._4CloudVisitorReg.VisitorRegFrm.strT_VisitorAccessInf + " where ";
            //    strSQL += "   GONO like '" + strGONOTemp + "%'";
            //    strSQL += LoginFrm.strLoginFrmSelectFlag;
            //    myTable = SQLHelper.DTQuery(strSQL);
            //    if (myTable.Rows.Count > 0)
            //    {
            //        if (myTable.Rows[0]["GONO"].ToString().Trim() != "" && myTable.Rows[0]["GONO"].ToString().Trim() != null)
            //        {
            //            strGONOTemp = Convert.ToString(Convert.ToInt32(myTable.Rows[0]["GONO"].ToString().Trim().Substring(2)) + 1);

            //            if (strGONOTemp.Length == 1)
            //            {
            //                strGONOTemp = "D" + iGONOLable.ToString() + "000000" + strGONOTemp;
            //            }
            //            else if (strGONOTemp.Length == 2)
            //            {
            //                strGONOTemp = "D" + iGONOLable.ToString() + "00000" + strGONOTemp;
            //            }
            //            else if (strGONOTemp.Length == 3)
            //            {
            //                strGONOTemp = "D" + iGONOLable.ToString() + "0000" + strGONOTemp;
            //            }
            //            else if (strGONOTemp.Length == 4)
            //            {
            //                strGONOTemp = "D" + iGONOLable.ToString() + "000" + strGONOTemp;
            //            }
            //            else if (strGONOTemp.Length == 5)
            //            {
            //                strGONOTemp = "D" + iGONOLable.ToString() + "00" + strGONOTemp;
            //            }
            //            else if (strGONOTemp.Length == 6)
            //            {
            //                strGONOTemp = "D" + iGONOLable.ToString() + "0" + strGONOTemp;
            //            }
            //            else if (strGONOTemp.Length == 7)
            //            {
            //                strGONOTemp = "D" + iGONOLable.ToString() + strGONOTemp;
            //            }
            //            else
            //            {
            //                //位数过大，基本不可能
            //            }

            //        }
            //        else
            //        {

            //            strGONOTemp = strGONOTemp + "0000001";
            //        }
            //    }
            //    else
            //    {
            //        strGONOTemp = strGONOTemp + "0000001";
            //    }
                return strGONOTemp;
            //}
            //catch (Exception exp)
            //{

            //    MessageBox.Show(exp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    strGONOTemp = strGONOTemp + "0000001";
            //    return strGONOTemp;
            //}
        }

        #endregion
     
        #region //自动产生签发卡片时的访客编号，字母C代表是Card，指发卡的访客
        public string PRegisterFrm_VNo_Card()
        {
            string strVNo = "", strTempID = "";
            try
            {
                myTable = SQLHelper.DTQuery("select max(VNo) VNo from T_LongTemCardInf order by VNo ");
                if (myTable.Rows.Count <= 0)
                {
                    #region
                    myTable = SQLHelper.DTQuery("select BNo from T_NumberTableInf where Flag ='" + "T_LongTemCardInf_VNo" + "'");
                    if (myTable.Rows.Count <= 0)
                    {
                        strVNo = "C000000001";//默认C0000001
                        SQLHelper.ExecuteSql("insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + strVNo + "','" + "T_LongTemCardInf_VNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        string maxID = myTable.Rows[0]["BNo"].ToString().Trim();
                        string strBNo_Old_ZF = "";//员工编号字符部分
                        string strBNo_Old_SZ = "0";//员工编号数字部分
                        for (int i = 0; i < maxID.Length; i++)
                        {
                            strSQL = maxID.Substring(i, 1).Trim();
                            if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                            {
                                strBNo_Old_ZF += strSQL;
                                continue;
                            }
                            else
                            {
                                strBNo_Old_SZ = maxID.Substring(i);
                                break;
                            }
                        }
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        strVNo = strBNo_Old_ZF + strTempID.Trim();
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + strVNo + "' where Flag ='" + "T_LongTemCardInf_VNo" + "'");
                    }
                    #endregion
                }
                else
                {
                    #region
                    string maxID = myTable.Rows[0]["VNo"].ToString().Trim();
                    string strBNo_Old_ZF = "";//员工编号字符部分
                    string strBNo_Old_SZ = "";//员工编号数字部分
                    for (int i = 0; i < maxID.Length; i++)
                    {
                        strSQL = maxID.Substring(i, 1).Trim();
                        if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                        {
                            strBNo_Old_ZF += strSQL;
                            continue;
                        }
                        else
                        {
                            strBNo_Old_SZ = maxID.Substring(i);
                            break;
                        }
                    }
                    if (strBNo_Old_SZ.Trim() != "")
                    {
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        strVNo = strBNo_Old_ZF + strTempID.Trim();
                    }
                    else
                    {
                        strVNo = "C000000001";//默认为C0000001
                        if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_LongTemCardInf_VNo" + "'").Rows.Count <= 0)
                        {
                            SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo )Values('" + strVNo.Trim() + "','" + "T_LongTemCardInf_VNo" + "','" + "" + "','" + "" + "')");
                        }
                    }
                    if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_LongTemCardInf_VNo" + "'").Rows.Count <= 0)
                    {
                        SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + strVNo.Trim() + "','" + "T_LongTemCardInf_VNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + strVNo.Trim() + "' where Flag='" + "T_LongTemCardInf_VNo" + "'");
                    }
                    #endregion
                }
            }
            catch
            {
                strVNo = "C000000001";
            }
            return strVNo;
        }

        #endregion

        #region//自动产生签发卡片时的访客实际编号
        public string PRegisterFrm_VActualNo_Card()
        {
            string strVActualNo = "0000000001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(VActualNo) VActualNo from T_LongTemCardInf order by  VActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["VActualNo"].ToString().Trim() != "" && myTable.Rows[0]["VActualNo"].ToString().Trim() != null)
                    {
                        int iVActualNo = Convert.ToInt16(myTable.Rows[0]["VActualNo"].ToString()) + 1;
                        switch (iVActualNo.ToString().Length)
                        {
                            case 1:
                                strVActualNo = "000000000" + iVActualNo.ToString(); break;
                            case 2:
                                strVActualNo = "00000000" + iVActualNo.ToString(); break;
                            case 3:
                                strVActualNo = "0000000" + iVActualNo.ToString(); break;
                            case 4:
                                strVActualNo = "000000" + iVActualNo.ToString(); break;
                            case 5:
                                strVActualNo = "00000" + iVActualNo.ToString(); break;
                            case 6:
                                strVActualNo = "0000" + iVActualNo.ToString(); break;
                            case 7:
                                strVActualNo = "000" + iVActualNo.ToString(); break;
                            case 8:
                                strVActualNo = "00" + iVActualNo.ToString(); break;
                            case 9:
                                strVActualNo = "0" + iVActualNo.ToString(); break;
                            default:
                                strVActualNo = iVActualNo.ToString(); break;
                                 
                        }
                    }
                }
                return strVActualNo;
            }
            catch
            {
                return strVActualNo;

            }
        }
        #endregion

        #region //自动产生指纹访客首次来访时的访客编号，字母F代表是Fingerprint，指首次采集指纹进行登记的访客
        public string PRegisterFrm_VNo_Fingerprint()
        {
            string strVNo = "", strTempID = "";
            try
            {
                myTable = SQLHelper.DTQuery("select max(VNo) VNo from T_VisitorFingerprintInf order by VNo ");
                if (myTable.Rows.Count <= 0)
                {
                    #region
                    myTable = SQLHelper.DTQuery("select BNo from T_NumberTableInf where Flag ='" + "T_VisitorFingerprintInf_VNo" + "'");
                    if (myTable.Rows.Count <= 0)
                    {
                        strVNo = "F000000001";//默认F000000001
                        SQLHelper.ExecuteSql("insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + strVNo + "','" + "T_VisitorFingerprintInf_VNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        string maxID = myTable.Rows[0]["BNo"].ToString().Trim();
                        string strBNo_Old_ZF = "";//员工编号字符部分
                        string strBNo_Old_SZ = "0";//员工编号数字部分
                        for (int i = 0; i < maxID.Length; i++)
                        {
                            strSQL = maxID.Substring(i, 1).Trim();
                            if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                            {
                                strBNo_Old_ZF += strSQL;
                                continue;
                            }
                            else
                            {
                                strBNo_Old_SZ = maxID.Substring(i);
                                break;
                            }
                        }
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        strVNo = strBNo_Old_ZF + strTempID.Trim();
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + strVNo + "' where Flag ='" + "T_VisitorFingerprintInf_VNo" + "'");
                    }
                    #endregion
                }
                else
                {
                    #region
                    string maxID = myTable.Rows[0]["VNo"].ToString().Trim();
                    string strBNo_Old_ZF = "";//员工编号字符部分
                    string strBNo_Old_SZ = "";//员工编号数字部分
                    for (int i = 0; i < maxID.Length; i++)
                    {
                        strSQL = maxID.Substring(i, 1).Trim();
                        if ((strSQL.CompareTo("A") >= 0 && strSQL.CompareTo("Z") <= 0) || strSQL.CompareTo("a") >= 0 && strSQL.CompareTo("z") <= 0)
                        {
                            strBNo_Old_ZF += strSQL;
                            continue;
                        }
                        else
                        {
                            strBNo_Old_SZ = maxID.Substring(i);
                            break;
                        }
                    }
                    if (strBNo_Old_SZ.Trim() != "")
                    {
                        int ilength_Old = strBNo_Old_SZ.Trim().Length;//计算数字部分的总长度
                        int inewID = Convert.ToInt32(strBNo_Old_SZ) + 1;//转换成数字型后加1;(长度可能改变[长度只会变小])
                        int ilength_New = inewID.ToString().Length;
                        if (ilength_Old > ilength_New)
                        {
                            int ilength_Sub = ilength_Old - ilength_New;
                            for (int i = 1; i <= ilength_Sub; i++)
                            {
                                strTempID += "0";
                            }
                            strTempID += inewID.ToString();
                        }
                        else
                        {
                            strTempID = inewID.ToString();
                        }
                        strVNo = strBNo_Old_ZF + strTempID.Trim();
                    }
                    else
                    {
                        strVNo = "F000000001";//默认为F000000001
                        if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_VisitorFingerprintInf_VNo" + "'").Rows.Count <= 0)
                        {
                            SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo )Values('" + strVNo.Trim() + "','" + "T_VisitorFingerprintInf_VNo" + "','" + "" + "','" + "" + "')");
                        }
                    }
                    if (SQLHelper.DTQuery("select * from T_NumberTableInf where Flag ='" + "T_VisitorFingerprintInf_VNo" + "'").Rows.Count <= 0)
                    {
                        SQLHelper.ExecuteSql(" insert into T_NumberTableInf(BNo,Flag,OperatorNo,CAccoutNo)Values('" + strVNo.Trim() + "','" + "T_VisitorFingerprintInf_VNo" + "','" + "" + "','" + "" + "')");
                    }
                    else
                    {
                        SQLHelper.ExecuteSql(" update T_NumberTableInf set BNo ='" + strVNo.Trim() + "' where Flag='" + "T_VisitorFingerprintInf_VNo" + "'");
                    }
                    #endregion
                }
            }
            catch
            {
                strVNo = "F000000001";
            }
            return strVNo;
        }

        #endregion

        #region//自动产生指纹访客首次来访时的访客实际编号
        public string PRegisterFrm_VActualNo_Fingerprint()
        {
            string strVActualNo = "0000000001";
            try
            {
                myTable = SQLHelper.DTQuery(" select max(VActualNo) VActualNo from T_VisitorFingerprintInf order by  VActualNo ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["VActualNo"].ToString().Trim() != "" && myTable.Rows[0]["VActualNo"].ToString().Trim() != null)
                    {
                        int iVActualNo = Convert.ToInt16(myTable.Rows[0]["VActualNo"].ToString()) + 1;
                        switch (iVActualNo.ToString().Length)
                        {
                            case 1:
                                strVActualNo = "000000000" + iVActualNo.ToString(); break;
                            case 2:
                                strVActualNo = "00000000" + iVActualNo.ToString(); break;
                            case 3:
                                strVActualNo = "0000000" + iVActualNo.ToString(); break;
                            case 4:
                                strVActualNo = "000000" + iVActualNo.ToString(); break;
                            case 5:
                                strVActualNo = "00000" + iVActualNo.ToString(); break;
                            case 6:
                                strVActualNo = "0000" + iVActualNo.ToString(); break;
                            case 7:
                                strVActualNo = "000" + iVActualNo.ToString(); break;
                            case 8:
                                strVActualNo = "00" + iVActualNo.ToString(); break;
                            case 9:
                                strVActualNo = "0" + iVActualNo.ToString(); break;
                            default:
                                strVActualNo = iVActualNo.ToString(); break;
                        }
                    }
                }
                return strVActualNo;
            }
            catch
            {
                return strVActualNo;

            }
        }
        #endregion

        #endregion


        #region//蓝本门禁控制器

        #region//加载区域信息
        public void A_AreaAndDeviceFrm_LoaddtAreaInf(out DataTable dtAreaInf)
        {
            strSQL = "select * from T_MJAPAreaCategroyInf  order by AId ";
            dtAreaInf = SQLHelper.DTQuery(strSQL);
        }
        #endregion


        #region//区域和设备

        #region//判断输入的IP要求是首字节不能为00, 最后字节不能为255
        public Boolean IsIPAddress(string para_strIP)
        {
            Boolean ret = false;
            try
            {
                if (string.IsNullOrEmpty(para_strIP))
                {
                }
                else
                {

                    string[] strIPInput = para_strIP.Split('.');
                    if (strIPInput.Length == 4)
                    {
                        int itemp;
                        ret = true;
                        for (int i = 0; i <= 3; i++)
                        {
                            //'数值0到255
                            if (!int.TryParse(strIPInput[i], out itemp))
                            {
                                ret = false;

                                break;
                            }

                            if (!((itemp >= 0) && (itemp <= 255)))
                            {
                                ret = false;
                                break;
                            }
                        }
                        if (int.Parse(strIPInput[0]) == 0) // '第一个值不能为0 
                        {
                            ret = false;

                        }
                        else if (int.Parse(strIPInput[3]) == 255) //最后一个值不能为255 
                        {
                            ret = false;

                        }
                    }
                }
            }
            catch
            {
                ret = false;
            }
            finally
            {
            }
            return ret;
        }
        #endregion
        public string A_AreaAndMachineFrm_LoadAreaInf(TreeView para_TV, string para_strFlag)
        {
            try
            {
                para_TV.Nodes.Clear();
                if (para_strFlag == "1")
                {
                    TreeNode node1 = new TreeNode("全部区域");
                    node1.Name = "0";
                    para_TV.Nodes.Add(node1);
                }
                strSQL = "select * from T_MJAPAreaCategroyInf  order by AId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "AItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["AName"].ToString().Trim());
                            node.Name = DVRow["AId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["AItemLevel"]);
                            para_TV.Nodes.Add(node);
                            A_AreaAndMachineFrm_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string A_AreaAndMachineFrm_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "AParentIndex = '" + para_parentRow["AId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["AName"].ToString().Trim());
                    Node.Name = DVRow["AId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt32(DVRow["AItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    A_AreaAndMachineFrm_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string A_AreaAndMachineFrm_AddAreaInf(TreeView para_TV, string para_strGName, string para_strGDetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select AItemLevel,AParentIndex from T_MJAPAreaCategroyInf where AId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strGDetailName = "";
                            A_AreaAndMachineFrm_GetGDetailName1(out strGDetailName, para_strGDetailName, para_TV.SelectedNode, false, para_strGName);
                            strGDetailName = para_strGDetailName + "/" + para_strGName;
                            TreeNode NodeNew = new TreeNode(para_strGName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strGItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["AItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                            string strGParentIndexNew = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_MJAPAreaCategroyInf(AName,AItemLevel,AParentIndex,ADetailName,AMCode )Values('" + para_strGName + "',";
                            strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + strGDetailName + "','" + MCodeAll(strGDetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(AId) AId from T_MJAPAreaCategroyInf where AName='" + para_strGName + "' and AParentIndex='" + strGParentIndexNew + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["AId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        A_AreaAndMachineFrm_GetGDetailName1(out str, para_strGDetailName, para_TV.SelectedNode, true, para_strGName);
                        if (para_strGDetailName.Contains("/"))
                        {
                            str = para_strGDetailName.Substring(0, para_strGDetailName.LastIndexOf("/")) + "/" + para_strGName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strGName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strGItemLevelNew = myTable.Rows[0]["AItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                        string strGParentIndexNew = myTable.Rows[0]["AParentIndex"].ToString();
                        strSQL = "insert into T_MJAPAreaCategroyInf(AName,AItemLevel,AParentIndex,ADetailName,AMCode)Values('" + para_strGName + "',";
                        strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(AId) AId from T_MJAPAreaCategroyInf where AName='" + para_strGName + "' and AParentIndex='" + strGParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["AId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_MJAPAreaCategroyInf(AName,AItemLevel,AParentIndex,ADetailName,AMCode )Values('" + para_strGName + "',";
                    strSQL += "'" + "0" + "','" + "-1" + "','" + para_strGName + "','" + MCodeAll(para_strGName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strGName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(AId) AId from T_MJAPAreaCategroyInf where AName='" + para_strGName + "' and AParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["AId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string A_AreaAndMachineFrm_EditAreaInf(TreeView para_TV)
        {
            if (this.sqlList.Length != 0)
            {
                this.sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                A_AreaAndMachineFrm_RecursionEditDCInf(para_TV.SelectedNode.Parent);
            }
            else
            {
                this.strSQL = "update T_MJAPMachineInf set ADDetailName='" + para_TV.SelectedNode.Text.Trim() + "' where T_MJAPMachineInf.ADId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                this.strMCode = this.MCodeAll(para_TV.SelectedNode.Text.Trim());
                this.strSQL = "update T_MJAPAreaCategroyInf set AName ='" + para_TV.SelectedNode.Text.Trim() + "',ADetailName ='" + para_TV.SelectedNode.Text.Trim() + "',AMCode='" + this.strMCode + "' where T_MJAPAreaCategroyInf.AId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                A_AreaAndMachineFrm_RecursionEditDCInf(para_TV.SelectedNode);
            }
            if ((this.sqlList.Length != 0) && this.SQLHelper.ExecuteNonQueryTran(this.sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                this.sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        public string A_AreaAndMachineFrm_DeleteAreaInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select Id from T_MJAPMachineInf where ADId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_MJAPAreaCategroyInf where AId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        private void A_AreaAndMachineFrm_GetGDetailName1(out string strGDetailName, string strGDetailName1, TreeNode para_TNode, bool para_blFlag, string para_strGName)
        {
            strGDetailName = strGDetailName1;
            if (para_TNode.Parent == null)
            {
                if (para_blFlag)
                {
                    strGDetailName = para_strGName;
                }
                else
                {
                    strGDetailName = para_TNode.Text.Trim() + "/" + para_strGName;
                }
            }
            else
            {
                if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                if (!para_blFlag)
                {
                    strGDetailName = strGDetailName + "/" + para_strGName;
                }
            }
        }

        public string A_AreaAndMachineFrm_GetGDetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }

        public string A_AreaAndMachineFrm_GetDCQuantity()
        {
            strSQL = "select distinct Id from T_MJAPMachineInf ";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                strSQL = "共有设备档案:" + myTable.Rows.Count.ToString() + " 条";
                return (strSQL);
            }
            return ("共有设备档案:0 条");
        }

        private void A_AreaAndMachineFrm_RecursionEditDCInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        A_AreaAndMachineFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                        A_AreaAndMachineFrm_RecursionEditDCInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        A_AreaAndMachineFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_MJAPMachineInf set ADDetailName ='" + para_TNode.Text.Trim() + "' where T_MJAPMachineInf.ADID ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        private void A_AreaAndMachineFrm_GetGDetailName(string para_strGDetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strGDetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_DCDoorInf set ADDetailName ='" + para_strGDetailName + "' where T_DCDoorInf.ADID ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strGDetailName);
            strSQL = "update T_MJAPAreaCategroyInf set ADetailName='" + para_strGDetailName + "',AMCode='" + strMCode + "' where T_MJAPAreaCategroyInf.AId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        //加载设备(设备)信息表
        public DataTable A_AreaAndMachineFrm_AutoSearch_LoadMachineInf(string para_strMSNo)
        {
            DataTable dt = new DataTable();
            if (para_strMSNo.Trim() == "")
            {
                strSQL = "select *  from XXCLOUD.dbo.T_MJAPMachineInf ";
            }
            else
            {
                strSQL = "select *  from XXCLOUD.dbo.T_MJAPMachineInf where MSNo='" + para_strMSNo + "'";
            }
            dt = SQLHelper.DTQuery(strSQL);
            return dt;
        }

        //自动生成设备编号
        public int A_AreaAndMachineFrm_MachineId()
        {
            int iMachineId = 1;
            try
            {
                myTable = SQLHelper.DTQuery(" select max(MachineId) MachineId from XXCLOUD.dbo.T_MJAPMachineInf order by  MachineId ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["MachineId"].ToString().Trim() != "" && myTable.Rows[0]["MachineId"].ToString().Trim() != null)
                    {
                        iMachineId = Convert.ToInt32(myTable.Rows[0]["MachineId"].ToString()) + 1;
                    }
                }
                return iMachineId;
            }
            catch
            {
                return iMachineId;

            }
        }

        //自动生成门编号
        public int A_AreaAndMachineFrm_DoorId()
        {
            int iDoorId = 1;
            try
            {
                myTable = SQLHelper.DTQuery(" select max(DoorId) DoorId from XXCLOUD.dbo.T_MJAPDoorInf order by  DoorId ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["DoorId"].ToString().Trim() != "" && myTable.Rows[0]["DoorId"].ToString().Trim() != null)
                    {
                        iDoorId = Convert.ToInt32(myTable.Rows[0]["DoorId"].ToString()) + 1;
                    }
                }
                return iDoorId;
            }
            catch
            {
                return iDoorId;
            }
        }
        //自动生成读头编号
        public int A_AreaAndMachineFrm_ReadHeadId()
        {
            int iReadHeadId = 1;
            try
            {
                myTable = SQLHelper.DTQuery(" select max(ReadHeadId) ReadHeadId from XXCLOUD.dbo.T_MJAPReadHeadInf order by  ReadHeadId ");
                if (myTable.Rows.Count > 0)
                {
                    if (myTable.Rows[0]["ReadHeadId"].ToString().Trim() != "" && myTable.Rows[0]["ReadHeadId"].ToString().Trim() != null)
                    {
                        iReadHeadId = Convert.ToInt32(myTable.Rows[0]["ReadHeadId"].ToString()) + 1;
                    }
                }
                return iReadHeadId;
            }
            catch
            {
                return iReadHeadId;

            }
        }

        #region//删除设备档案
        public string A_AreaAndMachineFrm_DeleteMachineInf(string para_strMId, string para_strFilter, string para_strFilter_CustomItemInf, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条设备档案
            {
                strSQL = "delete from T_MJAPMachineInf where T_MJAPMachineInf.MachineId ='" + para_strMId + "';";
                sqlList.AppendFormat(strSQL);
                strSQL = "delete from T_MJAPDoorInf where T_MJAPDoorInf.MachineId ='" + para_strMId + "';";
                sqlList.AppendFormat(strSQL);
                strSQL = "delete from T_MJAPReadHeadInf where T_MJAPReadHeadInf.MachineId ='" + para_strMId + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//删除多选设备档案
            {
                strSQL = "delete from T_MJAPMachineInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                strSQL = "delete from T_MJAPDoorInf where  " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                strSQL = "delete from T_MJAPReadHeadInf where  " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }
        #endregion

        #region//选择查询
        public DataTable A_AreaAndMachineFrm_MachineSearch(string para_strMachineSearch, string para_strDDetailName, string para_strMachineSearch_Temp, string para_strDisplayOrder)
        {
            strSQL = para_strMachineSearch;
            if (strSQL.Contains("T_MJAPMachineInf.ADDetailName") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("T_MJAPMachineInf.ADDetailName"));
                if (para_strDDetailName == "")//表示在全部类别中查询
                {
                    strSQL = strSQL.Substring(0, strSQL.Length - 5);
                    if (strSQL.Length > 0)
                    {
                        if (strSQL.Substring(strSQL.Length - 1, 1) == "w")
                        {
                            strSQL = strSQL.Substring(0, strSQL.Length - 1);
                        }
                    }
                }
                else//表示在当前类别中查询
                {
                    strSQL += " T_MJAPMachineInf.ADDetailName ='" + para_strDDetailName.Trim() + "'";
                }
            }
            if (strSQL.Contains("order") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("order"));
            }
            if (strSQL.Contains("orde") == true)
            {
                strSQL = strSQL.Substring(0, strSQL.LastIndexOf("orde"));
            }
            strSQL += "  where " + para_strMachineSearch_Temp + para_strDisplayOrder;
            myTable = SQLHelper.DTQuery(strSQL);
            return myTable;
        }
        #endregion

        #region//更改设备所在区域
        public string A_AreaAndMachineFrm_EditCurArea(DataTable para_DT, string para_strDId, string para_strSDDetailName)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            DataTable dt = new DataTable();
            dt = para_DT.Copy();
            if (dt.Rows.Count > 0)
            {
                string strFilter = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strFilter += "  MachineId  = " + "'" + dt.Rows[i]["MachineId"].ToString().Trim() + "'" + " or ";
                }
                if (strFilter != "")
                {
                    strFilter = strFilter.Substring(0, strFilter.Length - 4);
                    strSQL = "update  T_MJAPMachineInf set ADId='" + para_strDId + "',ADDetailName ='" + para_strSDDetailName + "' where " + strFilter + ";";
                    sqlList.AppendFormat(strSQL);
                    strSQL = "update  T_MJAPDoorInf set ADId='" + para_strDId + "'  where " + strFilter + ";";
                    sqlList.AppendFormat(strSQL);
                    if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                    {
                        if (dt.Rows.Count > 1)
                        {
                            return "2";
                        }
                        else
                        {
                            return "1";
                        }


                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            return "0";
        }
        #endregion
        #endregion

        #region//门与读头

        public string B_DoorAndReadHeadFrm_GetDCQuantity(string para_strMachineId)
        {
            strSQL = "select distinct Id from T_MJAPDoorInf where MachineId =" + para_strMachineId;
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                strSQL = "当前设备共有门档案:" + myTable.Rows.Count.ToString() + " 条";
                return (strSQL);
            }
            return ("当前设备共有门档案:0 条");
        }
        #endregion

        #region//门禁时间段
        public string C_TimeClassAndZoneFrm_LoadTZInf(TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_DCTimeClassInf  order by TId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "TItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["TName"].ToString().Trim());
                            node.Name = DVRow["TId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["TItemLevel"]);
                            para_TV.Nodes.Add(node);
                            C_TimeClassAndZoneFrm_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string C_TimeClassAndZoneFrm_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "TParentIndex = '" + para_parentRow["TId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["TName"].ToString().Trim());
                    Node.Name = DVRow["TId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt32(DVRow["TItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    C_TimeClassAndZoneFrm_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string C_TimeClassAndZoneFrm_AddTZInf(TreeView para_TV, string para_strGName, string para_strGDetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select TItemLevel,TParentIndex from T_DCTimeClassInf where TId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strGDetailName = "";
                            C_TimeClassAndZoneFrm_GetGDetailName1(out strGDetailName, para_strGDetailName, para_TV.SelectedNode, false, para_strGName);
                            strGDetailName = para_strGDetailName + "/" + para_strGName;
                            TreeNode NodeNew = new TreeNode(para_strGName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strGItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["TItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                            string strGParentIndexNew = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_DCTimeClassInf(TName,TItemLevel,TParentIndex,TDetailName,TMCode )Values('" + para_strGName + "',";
                            strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + strGDetailName + "','" + MCodeAll(strGDetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(TId) TId from T_DCTimeClassInf where TName='" + para_strGName + "' and TParentIndex='" + strGParentIndexNew + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["TId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        C_TimeClassAndZoneFrm_GetGDetailName1(out str, para_strGDetailName, para_TV.SelectedNode, true, para_strGName);
                        if (para_strGDetailName.Contains("/"))
                        {
                            str = para_strGDetailName.Substring(0, para_strGDetailName.LastIndexOf("/")) + "/" + para_strGName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strGName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strGItemLevelNew = myTable.Rows[0]["TItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                        string strGParentIndexNew = myTable.Rows[0]["TParentIndex"].ToString();
                        strSQL = "insert into T_DCTimeClassInf(TName,TItemLevel,TParentIndex,TDetailName,TMCode)Values('" + para_strGName + "',";
                        strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(TId) TId from T_DCTimeClassInf where TName='" + para_strGName + "' and TParentIndex='" + strGParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["TId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_DCTimeClassInf(TName,TItemLevel,TParentIndex,TDetailName,TMCode )Values('" + para_strGName + "',";
                    strSQL += "'" + "0" + "','" + "-1" + "','" + para_strGName + "','" + MCodeAll(para_strGName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strGName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(TId) TId from T_DCTimeClassInf where TName='" + para_strGName + "' and TParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["TId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string C_TimeClassAndZoneFrm_EditTZInf(TreeView para_TV)
        {
            if (this.sqlList.Length != 0)
            {
                this.sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                C_TimeClassAndZoneFrm_RecursionEditDCInf(para_TV.SelectedNode.Parent);
            }
            else
            {
                this.strSQL = "update T_DCTimezoneInf set TDDetailName='" + para_TV.SelectedNode.Text.Trim() + "' where T_DCTimezoneInf.TId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                this.strMCode = this.MCodeAll(para_TV.SelectedNode.Text.Trim());
                this.strSQL = "update T_DCTimeClassInf set TName ='" + para_TV.SelectedNode.Text.Trim() + "',TDetailName ='" + para_TV.SelectedNode.Text.Trim() + "',TMCode='" + this.strMCode + "' where T_DCTimeClassInf.TId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                C_TimeClassAndZoneFrm_RecursionEditDCInf(para_TV.SelectedNode);
            }
            if ((this.sqlList.Length != 0) && this.SQLHelper.ExecuteNonQueryTran(this.sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                this.sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        public string C_TimeClassAndZoneFrm_DeleteTZInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select ID from T_DCTimezoneInf where TId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_DCTimeClassInf where TId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        private void C_TimeClassAndZoneFrm_GetGDetailName1(out string strGDetailName, string strGDetailName1, TreeNode para_TNode, bool para_blFlag, string para_strGName)
        {
            strGDetailName = strGDetailName1;
            if (para_TNode.Parent == null)
            {
                if (para_blFlag)
                {
                    strGDetailName = para_strGName;
                }
                else
                {
                    strGDetailName = para_TNode.Text.Trim() + "/" + para_strGName;
                }
            }
            else
            {
                if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                if (!para_blFlag)
                {
                    strGDetailName = strGDetailName + "/" + para_strGName;
                }
            }
        }

        public string C_TimeClassAndZoneFrm_GetGDetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }

        public string C_TimeClassAndZoneFrm_GetDCQuantity(string para_strTId)
        {
            strSQL = "select distinct Id from T_DCTimezoneInf  where TId ='" + para_strTId + "'";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                strSQL = "共有时间段档案:" + myTable.Rows.Count.ToString() + " 条";
                return (strSQL);
            }
            return ("共有时间段档案:0 条");
        }

        private void C_TimeClassAndZoneFrm_RecursionEditDCInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        C_TimeClassAndZoneFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                        C_TimeClassAndZoneFrm_RecursionEditDCInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        C_TimeClassAndZoneFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_DCTimezoneInf set TDDetailName ='" + para_TNode.Text.Trim() + "' where T_DCTimezoneInf.TId ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        private void C_TimeClassAndZoneFrm_GetGDetailName(string para_strGDetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strGDetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_DCTimezoneInf set TDDetailName ='" + para_strGDetailName + "' where T_DCTimezoneInf.TId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strGDetailName);
            strSQL = "update T_DCTimeClassInf set TDetailName='" + para_strGDetailName + "',TMCode='" + strMCode + "' where T_DCTimeClassInf.TId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        public string C_TimeClassAndZoneFrm_DeleteTimezoneInf(string para_strSActualNo, string para_strFilter, string para_strFilter_CustomItemInf, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条时区档案
            {
                strSQL = "delete from T_DCTimezoneInf where T_DCTimezoneInf.Id ='" + para_strSActualNo + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//删除多选时区档案
            {
                strSQL = "delete from T_DCTimezoneInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }

        #endregion

        #region//门禁节假日
        public string B_HolidaysClassAndInfFrm_LoadHolidaysInf(TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_DCHolidaysClass  order by HId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "HItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["HName"].ToString().Trim());
                            node.Name = DVRow["HId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["HItemLevel"]);
                            para_TV.Nodes.Add(node);
                            B_HolidaysClassAndInfFrm_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string B_HolidaysClassAndInfFrm_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "HParentIndex = '" + para_parentRow["HId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["HName"].ToString().Trim());
                    Node.Name = DVRow["HId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt32(DVRow["HItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    B_HolidaysClassAndInfFrm_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string B_HolidaysClassAndInfFrm_AddHolidaysInf(TreeView para_TV, string para_strGName, string para_strGDetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select HItemLevel,HParentIndex from T_DCHolidaysClass where HId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strGDetailName = "";
                            B_HolidaysClassAndInfFrm_GetGDetailName1(out strGDetailName, para_strGDetailName, para_TV.SelectedNode, false, para_strGName);
                            strGDetailName = para_strGDetailName + "/" + para_strGName;
                            TreeNode NodeNew = new TreeNode(para_strGName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strGItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["HItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                            string strGParentIndexNew = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_DCHolidaysClass(HName,HItemLevel,HParentIndex,HDetailName,HMCode )Values('" + para_strGName + "',";
                            strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + strGDetailName + "','" + MCodeAll(strGDetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(HId) HId from T_DCHolidaysClass where HName='" + para_strGName + "' and HParentIndex='" + strGParentIndexNew + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["HId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        B_HolidaysClassAndInfFrm_GetGDetailName1(out str, para_strGDetailName, para_TV.SelectedNode, true, para_strGName);
                        if (para_strGDetailName.Contains("/"))
                        {
                            str = para_strGDetailName.Substring(0, para_strGDetailName.LastIndexOf("/")) + "/" + para_strGName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strGName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strGItemLevelNew = myTable.Rows[0]["HItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                        string strGParentIndexNew = myTable.Rows[0]["HParentIndex"].ToString();
                        strSQL = "insert into T_DCHolidaysClass(HName,HItemLevel,HParentIndex,HDetailName,HMCode)Values('" + para_strGName + "',";
                        strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(HId) HId from T_DCHolidaysClass where HName='" + para_strGName + "' and HParentIndex='" + strGParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["HId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_DCHolidaysClass(HName,HItemLevel,HParentIndex,HDetailName,HMCode )Values('" + para_strGName + "',";
                    strSQL += "'" + "0" + "','" + "-1" + "','" + para_strGName + "','" + MCodeAll(para_strGName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strGName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(HId) HId from T_DCHolidaysClass where HName='" + para_strGName + "' and HParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["HId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string B_HolidaysClassAndInfFrm_EditHolidaysInf(TreeView para_TV)
        {
            if (this.sqlList.Length != 0)
            {
                this.sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                B_HolidaysClassAndInfFrm_RecursionEditDCInf(para_TV.SelectedNode.Parent);
            }
            else
            {
                this.strSQL = "update T_DCHolidaysInf set HDDetailName='" + para_TV.SelectedNode.Text.Trim() + "' where T_DCHolidaysInf.HId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                this.strMCode = this.MCodeAll(para_TV.SelectedNode.Text.Trim());
                this.strSQL = "update T_DCHolidaysClass set HName ='" + para_TV.SelectedNode.Text.Trim() + "',HDetailName ='" + para_TV.SelectedNode.Text.Trim() + "',HMCode='" + this.strMCode + "' where T_DCHolidaysClass.HId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                B_HolidaysClassAndInfFrm_RecursionEditDCInf(para_TV.SelectedNode);
            }
            if ((this.sqlList.Length != 0) && this.SQLHelper.ExecuteNonQueryTran(this.sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                this.sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        public string B_HolidaysClassAndInfFrm_DeleteHolidaysInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select ID from T_DCHolidaysInf where HId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_DCHolidaysClass where HId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        private void B_HolidaysClassAndInfFrm_GetGDetailName1(out string strGDetailName, string strGDetailName1, TreeNode para_TNode, bool para_blFlag, string para_strGName)
        {
            strGDetailName = strGDetailName1;
            if (para_TNode.Parent == null)
            {
                if (para_blFlag)
                {
                    strGDetailName = para_strGName;
                }
                else
                {
                    strGDetailName = para_TNode.Text.Trim() + "/" + para_strGName;
                }
            }
            else
            {
                if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                if (!para_blFlag)
                {
                    strGDetailName = strGDetailName + "/" + para_strGName;
                }
            }
        }

        public string B_HolidaysClassAndInfFrm_GetGDetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }

        public string B_HolidaysClassAndInfFrm_GetDCQuantity(string para_strHId)
        {
            strSQL = "select distinct Id from T_DCHolidaysInf  where HId ='" + para_strHId + "'";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                strSQL = "共有节假日档案:" + myTable.Rows.Count.ToString() + " 条";
                return (strSQL);
            }
            return ("共有节假日档案:0 条");
        }

        private void B_HolidaysClassAndInfFrm_RecursionEditDCInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        B_HolidaysClassAndInfFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                        B_HolidaysClassAndInfFrm_RecursionEditDCInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        B_HolidaysClassAndInfFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_DCHolidaysInf set HDDetailName ='" + para_TNode.Text.Trim() + "' where T_DCHolidaysInf.HId ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        private void B_HolidaysClassAndInfFrm_GetGDetailName(string para_strGDetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strGDetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_DCHolidaysInf set HDDetailName ='" + para_strGDetailName + "' where T_DCHolidaysInf.HId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strGDetailName);
            strSQL = "update T_DCHolidaysClass set HDetailName='" + para_strGDetailName + "',HMCode='" + strMCode + "' where T_DCHolidaysClass.HId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        public string B_HolidaysClassAndInfFrm_DeleteHolidaysInf(string para_strSActualNo, string para_strFilter, string para_strFilter_CustomItemInf, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条时区档案
            {
                strSQL = "delete from T_DCHolidaysInf where T_DCHolidaysInf.Id ='" + para_strSActualNo + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//删除多选时区档案
            {
                strSQL = "delete from T_DCHolidaysInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }

        #endregion

        #region//门禁权限组
        public string E_AuthAndGroupFrm_LoadHolidaysInf(TreeView para_TV)
        {
            try
            {
                para_TV.Nodes.Clear();
                strSQL = "select * from T_DCAuthGroupClass  order by AId ";
                myTable = SQLHelper.DTQuery(strSQL);
                if (myTable.Rows.Count > 0)
                {
                    DV = new DataView(myTable.Copy());
                    DV.RowFilter = "AItemLevel =0";
                    if (DV.Count > 0)
                    {
                        int iTreeNodeNo = 0;
                        foreach (DataRowView DVRow in DV)
                        {
                            TreeNode node = new TreeNode(DVRow["AName"].ToString().Trim());
                            node.Name = DVRow["AId"].ToString().Trim();
                            node.ImageIndex = Convert.ToInt32(DVRow["AItemLevel"]);
                            para_TV.Nodes.Add(node);
                            E_AuthAndGroupFrm_LoadTreeView(para_TV.Nodes[iTreeNodeNo], DVRow, myTable);
                            iTreeNodeNo++;
                        }
                        para_TV.ExpandAll();
                    }
                }
                if (para_TV.Nodes.Count > 0)
                {
                    para_TV.SelectedNode = para_TV.Nodes[0];
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string E_AuthAndGroupFrm_LoadTreeView(TreeNode para_parentNode, DataRowView para_parentRow, DataTable para_myTable)
        {
            try
            {
                DV = new DataView(para_myTable);
                DV.RowFilter = "AParentIndex = '" + para_parentRow["AId"].ToString().Trim() + "'";
                foreach (DataRowView DVRow in DV)
                {
                    TreeNode Node = new TreeNode(DVRow["AName"].ToString().Trim());
                    Node.Name = DVRow["AId"].ToString().Trim();
                    Node.ImageIndex = Convert.ToInt32(DVRow["AItemLevel"]);
                    para_parentNode.Nodes.Add(Node);
                    E_AuthAndGroupFrm_LoadTreeView(Node, DVRow, para_myTable);
                }
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        public string E_AuthAndGroupFrm_AddHolidaysInf(TreeView para_TV, string para_strGName, string para_strGDetailName, bool para_blAddInThisLevel)
        {
            try
            {
                if (para_TV.Nodes.Count > 0)
                {
                    strSQL = "select AItemLevel,AParentIndex from T_DCAuthGroupClass where AId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (para_blAddInThisLevel == false)//新增下级
                    {
                        if (myTable.Rows.Count > 0)
                        {
                            string strGDetailName = "";
                            E_AuthAndGroupFrm_GetGDetailName1(out strGDetailName, para_strGDetailName, para_TV.SelectedNode, false, para_strGName);
                            strGDetailName = para_strGDetailName + "/" + para_strGName;
                            TreeNode NodeNew = new TreeNode(para_strGName);
                            para_TV.SelectedNode.Nodes.Add(NodeNew);
                            string strGItemLevelNew = Convert.ToString((int)(Convert.ToInt32(myTable.Rows[0]["AItemLevel"].ToString()) + 1));
                            NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                            string strGParentIndexNew = para_TV.SelectedNode.Name.ToString();
                            strSQL = "insert into T_DCAuthGroupClass(AName,AItemLevel,AParentIndex,ADetailName,AMCode )Values('" + para_strGName + "',";
                            strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + strGDetailName + "','" + MCodeAll(strGDetailName) + "')";
                            SQLHelper.ExecuteSql(strSQL);
                            para_TV.SelectedNode = NodeNew;
                            strSQL = "select max(AId) AId from T_DCAuthGroupClass where AName='" + para_strGName + "' and AParentIndex='" + strGParentIndexNew + "' ";
                            myTable = SQLHelper.DTQuery(strSQL);
                            if (myTable.Rows.Count > 0)
                            {
                                para_TV.SelectedNode.Name = myTable.Rows[0]["AId"].ToString().Trim();
                            }
                            return "2";
                        }
                    }
                    else if (myTable.Rows.Count > 0)//新增同级
                    {
                        string str = "";
                        E_AuthAndGroupFrm_GetGDetailName1(out str, para_strGDetailName, para_TV.SelectedNode, true, para_strGName);
                        if (para_strGDetailName.Contains("/"))
                        {
                            str = para_strGDetailName.Substring(0, para_strGDetailName.LastIndexOf("/")) + "/" + para_strGName;
                        }
                        TreeNode NodeNew = new TreeNode(para_strGName);
                        if ((para_TV.SelectedNode.Parent == null) || (para_TV.SelectedNode == null))
                        {
                            para_TV.Nodes.Add(NodeNew);
                        }
                        else
                        {
                            para_TV.SelectedNode.Parent.Nodes.Add(NodeNew);
                        }
                        string strGItemLevelNew = myTable.Rows[0]["AItemLevel"].ToString();
                        NodeNew.ImageIndex = Convert.ToInt32(strGItemLevelNew);
                        string strGParentIndexNew = myTable.Rows[0]["AParentIndex"].ToString();
                        strSQL = "insert into T_DCAuthGroupClass(AName,AItemLevel,AParentIndex,ADetailName,AMCode)Values('" + para_strGName + "',";
                        strSQL += "'" + strGItemLevelNew + "','" + strGParentIndexNew + "','" + str + "','" + MCodeAll(str) + "')";
                        SQLHelper.ExecuteSql(strSQL);
                        para_TV.SelectedNode = NodeNew;
                        strSQL = "select max(AId) AId from T_DCAuthGroupClass where AName='" + para_strGName + "' and AParentIndex='" + strGParentIndexNew + "' ";
                        myTable = SQLHelper.DTQuery(strSQL);
                        if (myTable.Rows.Count > 0)
                        {
                            para_TV.SelectedNode.Name = myTable.Rows[0]["AId"].ToString().Trim();
                        }
                        return "1";
                    }
                }
                else//新增第一级
                {
                    strSQL = "insert into T_DCAuthGroupClass(AName,AItemLevel,AParentIndex,ADetailName,AMCode )Values('" + para_strGName + "',";
                    strSQL += "'" + "0" + "','" + "-1" + "','" + para_strGName + "','" + MCodeAll(para_strGName) + "')";
                    SQLHelper.ExecuteSql(strSQL);
                    TreeNode NodeNew = new TreeNode(para_strGName);
                    para_TV.Nodes.Add(NodeNew);
                    NodeNew.ImageIndex = 0;
                    para_TV.SelectedNode = NodeNew;
                    strSQL = "select max(AId) AId from T_DCAuthGroupClass where AName='" + para_strGName + "' and AParentIndex='" + "-1" + "' ";
                    myTable = SQLHelper.DTQuery(strSQL);
                    if (myTable.Rows.Count > 0)
                    {
                        para_TV.SelectedNode.Name = myTable.Rows[0]["AId"].ToString().Trim();
                    }
                    return "0";
                }
                return "-1";
            }
            catch
            {
                return "-1";
            }
        }

        public string E_AuthAndGroupFrm_EditHolidaysInf(TreeView para_TV)
        {
            if (this.sqlList.Length != 0)
            {
                this.sqlList.Length = 0;
            }
            if (para_TV.SelectedNode.Parent != null)
            {
                E_AuthAndGroupFrm_RecursionEditDCInf(para_TV.SelectedNode.Parent);
            }
            else
            {
                this.strSQL = "update T_DCAuthGroupInf set HDDetailName='" + para_TV.SelectedNode.Text.Trim() + "' where T_DCAuthGroupInf.AId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                this.strMCode = this.MCodeAll(para_TV.SelectedNode.Text.Trim());
                this.strSQL = "update T_DCAuthGroupClass set AName ='" + para_TV.SelectedNode.Text.Trim() + "',ADetailName ='" + para_TV.SelectedNode.Text.Trim() + "',AMCode='" + this.strMCode + "' where T_DCAuthGroupClass.AId ='" + para_TV.SelectedNode.Name.ToString() + "';";
                this.sqlList.AppendFormat(this.strSQL, new object[0]);
                E_AuthAndGroupFrm_RecursionEditDCInf(para_TV.SelectedNode);
            }
            if ((this.sqlList.Length != 0) && this.SQLHelper.ExecuteNonQueryTran(this.sqlList.ToString().TrimEnd(new char[] { ';' }).Split(new char[] { ';' })))
            {
                this.sqlList.Length = 0;
                return "1";
            }
            return "0";
        }

        public string E_AuthAndGroupFrm_DeleteHolidaysInf(TreeView para_TV)
        {
            try
            {
                bool blHaveNode = false;
                if (para_TV.SelectedNode.Nodes.Count > 0)
                {
                    blHaveNode = true;
                }
                if (!blHaveNode)
                {
                    this.strSQL = "select ID from T_DCAuthGroupInf where AId ='" + para_TV.SelectedNode.Name.ToString() + "'";
                    if (this.SQLHelper.DTQuery(this.strSQL).Rows.Count > 0)
                    {
                        blHaveNode = true;
                    }
                }
                if (!blHaveNode && (this.SQLHelper.ExecuteSql("delete from T_DCAuthGroupClass where AId = '" + para_TV.SelectedNode.Name.Trim() + "'") != 0))
                {
                    return "1";
                }
                return "0";
            }
            catch
            {
                return "-1";
            }
        }

        private void E_AuthAndGroupFrm_GetGDetailName1(out string strGDetailName, string strGDetailName1, TreeNode para_TNode, bool para_blFlag, string para_strGName)
        {
            strGDetailName = strGDetailName1;
            if (para_TNode.Parent == null)
            {
                if (para_blFlag)
                {
                    strGDetailName = para_strGName;
                }
                else
                {
                    strGDetailName = para_TNode.Text.Trim() + "/" + para_strGName;
                }
            }
            else
            {
                if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
                {
                    strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
                }
                if (!para_blFlag)
                {
                    strGDetailName = strGDetailName + "/" + para_strGName;
                }
            }
        }

        public string E_AuthAndGroupFrm_GetGDetailName(TreeView para_TV)
        {
            if (para_TV.SelectedNode.Parent == null)
            {
                return para_TV.SelectedNode.Text;
            }
            if ((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && (para_TV.SelectedNode.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if (((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            if ((((para_TV.SelectedNode.Parent != null) && (para_TV.SelectedNode.Parent.Parent != null)) && ((para_TV.SelectedNode.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent != null))) && ((para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                return (para_TV.SelectedNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Parent.Text.Trim() + "/" + para_TV.SelectedNode.Text.Trim());
            }
            return "";
        }

        public string E_AuthAndGroupFrm_GetDCQuantity(string para_strAId)
        {
            strSQL = "select distinct Id from T_DCAuthGroupInf  where AId ='" + para_strAId + "'";
            myTable = SQLHelper.DTQuery(strSQL);
            if (myTable.Rows.Count > 0)
            {
                strSQL = "共有节假日档案:" + myTable.Rows.Count.ToString() + " 条";
                return (strSQL);
            }
            return ("共有节假日档案:0 条");
        }

        private void E_AuthAndGroupFrm_RecursionEditDCInf(TreeNode para_TNode)
        {
            if (para_TNode.Nodes.Count != 0)
            {
                for (int i = 0; i < para_TNode.Nodes.Count; i++)
                {
                    string strSQLTemp = "";
                    if (para_TNode.Nodes[i].Nodes.Count != 0)
                    {
                        E_AuthAndGroupFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                        E_AuthAndGroupFrm_RecursionEditDCInf(para_TNode.Nodes[i]);
                    }
                    else
                    {
                        E_AuthAndGroupFrm_GetGDetailName(strSQLTemp, para_TNode.Nodes[i]);
                    }
                }
            }
            else
            {
                strSQL = "update T_DCAuthGroupInf set HDDetailName ='" + para_TNode.Text.Trim() + "' where T_DCAuthGroupInf.AId ='" + para_TNode.Name.Trim() + "';";
                sqlList.AppendFormat(strSQL, new object[0]);
            }
        }

        private void E_AuthAndGroupFrm_GetGDetailName(string para_strGDetailName, TreeNode para_TNode)
        {
            if (para_TNode.Parent == null)
            {
                para_strGDetailName = para_TNode.Text;
            }
            else if ((para_TNode.Parent != null) && (para_TNode.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && (para_TNode.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if (((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && (para_TNode.Parent.Parent.Parent.Parent.Parent == null))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            else if ((((para_TNode.Parent != null) && (para_TNode.Parent.Parent != null)) && ((para_TNode.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent != null))) && ((para_TNode.Parent.Parent.Parent.Parent.Parent != null) && (para_TNode.Parent.Parent.Parent.Parent.Parent.Parent == null)))
            {
                para_strGDetailName = para_TNode.Parent.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Parent.Text.Trim() + "/" + para_TNode.Parent.Text.Trim() + "/" + para_TNode.Text.Trim();
            }
            strSQL = "update T_DCAuthGroupInf set HDDetailName ='" + para_strGDetailName + "' where T_DCAuthGroupInf.AId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
            strMCode = MCodeAll(para_strGDetailName);
            strSQL = "update T_DCAuthGroupClass set ADetailName='" + para_strGDetailName + "',AMCode='" + strMCode + "' where T_DCAuthGroupClass.AId ='" + para_TNode.Name.Trim() + "';";
            sqlList.AppendFormat(strSQL, new object[0]);
        }

        public string E_AuthAndGroupFrm_DeleteHolidaysInf(string para_strSActualNo, string para_strFilter, string para_strFilter_CustomItemInf, bool para_blFlag)
        {
            if (sqlList.Length != 0)
            {
                sqlList.Length = 0;//先清空所有先前执行过的语句
            }
            if (para_blFlag == false)//删除当前选中的一条时区档案
            {
                strSQL = "delete from T_DCAuthGroupInf where T_DCAuthGroupInf.Id ='" + para_strSActualNo + "';";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            else//删除多选时区档案
            {
                strSQL = "delete from T_DCAuthGroupInf where " + para_strFilter + ";";
                sqlList.AppendFormat(strSQL);
                if (SQLHelper.ExecuteNonQueryTran(sqlList.ToString().TrimEnd(';').Split(';')) == true)
                {
                    return "1";
                }
            }
            return "0";
        }

        #endregion
        #endregion



    }

}
