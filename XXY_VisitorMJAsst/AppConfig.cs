using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXY_VisitorMJAsst
{
/// <summary>
    /// 配置信息维护
    /// </summary>
    public class AppConfig
    {
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
 
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="key">配置标识</param>
        /// <returns></returns>
        public static string GetValue(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            return sr.ReadLine();
        }
 
        /// <summary>
        /// 修改或增加配置值
        /// </summary>
        /// <param name="key">配置标识</param>
        /// <param name="value">配置值</param>
        public static void SetValue(string path, string value)
        {
            FileStream aFile = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);
            sw.Write(value);
            sw.Close();
        }
 
        /// <summary>
        /// 删除配置值
        /// </summary>
        /// <param name="key">配置标识</param>
        public static void DeleteValue(string key)
        {
            config.AppSettings.Settings.Remove(key);
        }
    }
}
