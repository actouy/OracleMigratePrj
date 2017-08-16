using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using log4net;

namespace Common
{
    public class LogHelper
    {

        #region  日志操作类

        private static String logFileName = DateTime.Now.ToString("yyyyMMddHHmm") + ".log";
        public static string ErrorMessage = "";
        public static void Log(String message)
        {
            try 
            { 
                string filepath = logFileName;
                if (!File.Exists(filepath))
                {
                    File.CreateText(filepath);
                }

                using (StreamWriter sw = File.AppendText(filepath))
                {
                    //获取调用者的信息
                    StackTrace trace = new StackTrace();
                    MethodBase method = trace.GetFrame(1).GetMethod();
                    String methodInfo = method.DeclaringType.FullName + "." + method.Name + "()";

                    //输出日期、调用者信息、message
                    ErrorMessage = methodInfo + "\r\n" + message;
                    String strDate = DateTime.Now.ToString();
                    sw.WriteLine("[ " + strDate + " ]: " + ErrorMessage);
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 写日志
        /// </summary>

        public static void WriteLog(Exception ex)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //记录错误日志
            log.Error(ex.Message, ex);
        }
        public static void WriteLog(object myobj,Exception ex)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //记录错误日志
            log.Error(myobj, ex);
        }
        public static void WriteLog(Exception ex,string errmessage)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //记录错误日志
            log.Error(ex.Message + errmessage, ex);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logpath">日志路径</param>
        /// <param name="erroinfo">日志信息</param>
        public static void WriteLog(string logpath, string erroinfo)
        {
            FileInfo fi = new FileInfo(logpath);
            try 
            { 
                if (!fi.Exists)
                {
                    FileStream fs = fi.Create();

                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(string.Format("{0}:{1}", DateTime.Now, erroinfo));
                    sw.WriteLine("--------------------------------------");
                    
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
                else
                {

                    //true参数可以设置为AppText在日志文件中追加日志，若不设置，则会先清空，然后写入，每次得到的都是最后一条
                    StreamWriter sw = new StreamWriter(logpath, true);
                    sw.WriteLine(string.Format("{0}:{1}", DateTime.Now, erroinfo));
                    sw.WriteLine("--------------------------------------");
                    sw.Flush();
                    sw.Close();

                }
            }
            catch(IOException ex)
            {
                //throw ex;
            }
        }


        /// <summary>
        /// 读取日志
        /// </summary>
        /// <param name="logpath">日志路径</param>
        /// <returns>返回日志内容</returns>
        public static string ReadLog(string logpath)
        {
            string loginfo = "";
            FileInfo fi = new FileInfo(logpath);
            if (!fi.Exists)
            {
                return "该路径下不存在日志！";
            }
            else
            {
                StreamReader sr = new StreamReader(logpath, ASCIIEncoding.UTF8);
                string line = sr.ReadLine();
                while (line != null)
                {
                    loginfo += string.Format("{0}" + "\n", line);
                    line = sr.ReadLine();

                }
                sr.Close();
                return loginfo;


            }
        }

        /// <summary>
        /// 读取日志
        /// </summary>
        /// <param name="logpath">日志路径</param>
        /// <returns>返回日志内容</returns>
        public static string ReadLog()
        {
            string logpath = logFileName;
            string loginfo = "";
            FileInfo fi = new FileInfo(logpath);
            if (!fi.Exists)
            {
                return "该路径下不存在日志！";
            }
            else
            {
                StreamReader sr = new StreamReader(logpath, ASCIIEncoding.UTF8);
                string line = sr.ReadLine();
                while (line != null)
                {
                    loginfo += string.Format("{0}" + "\n", line);
                    line = sr.ReadLine();

                }
                sr.Close();
                return loginfo;


            }
        }
        #endregion
    }
}
