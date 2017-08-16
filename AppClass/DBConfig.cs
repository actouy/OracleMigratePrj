using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClass;
using System.Reflection;

namespace AppClass
{
    /// <summary>
    /// 数据配置类
    /// </summary>
    public class DBConOP
    {
        /// <summary>
        /// 读取配置文件中的数据库配置信息
        /// </summary>
        /// <returns></returns>
        public static DBConfig readConfig() 
        {
            DBConfig mydb = new DBConfig();
            Type t = mydb.GetType();
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            foreach (System.Reflection.PropertyInfo info in properties)
            {
                string strValue = PubConstant.GetConfigurationString(info.Name);
                //PubConstant.ChangeConValue(info.Name, strValue);
                info.SetValue(mydb, strValue);
                
            }
            return mydb;
        }
        public static bool saveConfig(DBConfig mydb) 
        {
            //Type type = typeof(T);
            Type t = mydb.GetType();
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            try 
            { 
                foreach (System.Reflection.PropertyInfo info in properties)
                {
                    string strValue = GetObjectPropertyValue<DBConfig>(mydb, info.Name);
                    PubConstant.ChangeConValue(info.Name, strValue);                   
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static string GetObjectPropertyValue<T>(T t, string propertyname)
        {
            Type type = typeof(T);

            PropertyInfo property = type.GetProperty(propertyname);

            if (property == null) return string.Empty;

            object o = property.GetValue(t, null);     

            if (o == null) return string.Empty;

            return o.ToString();
        }
    }

    public class DBConfig
    {
        public string DBServerIP
        {
            get;
            set;
        }
        public string OracleSID
        {
            get;
            set;
        }
        public string DBUserName
        {
            get;
            set;
        }
        public string DBPassword
        {
            get;
            set;
        }
        //public string DBPath
        //{
        //    get;
        //    set;
        //}
    }
}
