using System;
using System.Configuration;

namespace Common
{
    public class PubConstant
    {
        public static void ChangeConValue(string AppKey, string AppValue)
        {
            string text = AppValue;
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection section = (AppSettingsSection) configuration.GetSection("appSettings");
            string str2 = ConfigurationManager.AppSettings["ConStringEncrypt"];
            if (str2 == "true")
            {
                text = DESEncrypt.Encrypt(text);
            }
            section.Settings.Remove(AppKey);
            section.Settings.Add(AppKey, AppValue);
            configuration.Save();
        }
        /// <summary>
        /// 得到config里配置项的数据库连接字符串。
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConfigurationString(string configName)
        {
            try 
            { 
                string text = ConfigurationManager.AppSettings[configName];
                string str2 = ConfigurationManager.AppSettings["ConStringEncrypt"];
                if (str2 == "true")
                {
                    text = DESEncrypt.Decrypt(text);
                }
                return text;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string text = GetConfigurationString("ConnectionString");
                string DBServerIP = GetConfigurationString("DBServerIP");
                string OracleSID = GetConfigurationString("OracleSID");
                string DBUserName = GetConfigurationString("DBUserName");
                string DBPassword = GetConfigurationString("DBPassword");                
                return string.Format(text, new object[] { DBServerIP, OracleSID, DBUserName, DBPassword });
            }
        }
    }
}

