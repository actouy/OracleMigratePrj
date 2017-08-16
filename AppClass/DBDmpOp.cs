using System;
using System.Diagnostics;
using Model;

namespace AppClass
{

    /// <summary>
    /// 数据操作类
    /// </summary>
    public static class DBDmpOp
    {

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="mydbinfo">数据类</param>
        /// <param name="filepath">文件保存的目录</param>
        /// <returns>成功返回值为真</returns>
        //public static bool OracleExp(DBConfig mydbinfo,string filepath)
        //{
        //    bool flag = false;
        //    string message = "";            
        //    try
        //    {
        //        Process process = new Process();
        //        string FilePath = filepath + DateTime.Today.ToString("yyyyMMdd") + ".dmp";
        //        process.StartInfo.FileName = "cmd.exe";
        //        process.StartInfo.UseShellExecute = true;
        //        process.StartInfo.CreateNoWindow = false;//显示CMD命令窗口
        //        process.StartInfo.Arguments = "/c exp " + mydbinfo.DBUserName + "/" + mydbinfo.DBPassword + "@" + mydbinfo.OracleSID + " owner=" + mydbinfo.DBUserName + " file=" + FilePath;
        //        //process.StartInfo.Arguments = "/c exp BIGDATAUSER/bigdatauser@BIGDATABASE owner=BIGDATAUSER file=F:/Debug/20160316.dmp";
        //        message = process.StartInfo.Arguments.ToString();
        //        LogHelper.Log(message);
        //        process.Start();
        //        process.WaitForExit();
        //        process.Dispose();
        //        flag = true;
        //    }
        //    catch (Exception exception)
        //    {
        //        message = exception.Message;
        //        //LogHelper.Log(message);
        //    }
        //    return flag;
        //}
        public static string OracleExp(DBConfig mydbinfo, string filepath) 
        {
            string FilePath = filepath + DateTime.Today.ToString("yyyyMMdd") + ".dmp";
            string cmd = "exp " + mydbinfo.DBUserName + "/" + mydbinfo.DBPassword + "@" + mydbinfo.OracleSID + " owner=" + mydbinfo.DBUserName + " file=" + FilePath;
            return RunCmd(cmd);
        }
        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="mydbinfo">数据类</param>
        /// <param name="filepath">文件保存的目录</param>
        /// <returns>成功返回值为真</returns>
        public static string OracleExp(DBConfig mydbinfo, string filepath,string tablenames)
        {
            string FilePath = filepath + DateTime.Today.ToString("yyyyMMdd") + ".dmp";
            string cmd = "exp " + mydbinfo.DBUserName + "/" + mydbinfo.DBPassword + "@" + mydbinfo.OracleSID + " owner=" + mydbinfo.DBUserName + " tables=(" + tablenames + ")" + " file=" + FilePath;
            return RunCmd(cmd);
        }
        /// <summary>
        /// 导入数据库文件
        /// </summary>
        /// <param name="mydbinfo">数据库配置类</param>
        /// <param name="filepath">DMP文件路径</param>
        //public static void OracleImp(DBConfig mydbinfo, string filepath)
        //{
        //    Process process = new Process();
        //    string message = "";
        //    try 
        //    { 
        //        process.StartInfo.FileName = "cmd.exe";
        //        process.StartInfo.UseShellExecute = true;
        //        process.StartInfo.CreateNoWindow = false;

        //        process.StartInfo.Arguments = "/c imp " + mydbinfo.DBUserName + "/" + mydbinfo.DBPassword + "@" + mydbinfo.OracleSID + "  file=" + filepath + " FULL=Y ignore=y";



        //        message = process.StartInfo.Arguments.ToString();
                
        //        //LogHelper.Log(message);
        //        process.Start();
        //        process.Dispose();
        //    //}
        //    }
        //    catch (Exception exception)
        //    {
        //        message = exception.Message;
        //        //LogHelper.Log(message);
        //    }
        //}
        public static string OracleImp(DBConfig mydbinfo, string filepath) 
        {
            string cmd = "imp " + mydbinfo.DBUserName + "/" + mydbinfo.DBPassword + "@" + mydbinfo.OracleSID + "  file=" + filepath + " FULL=Y ignore=y";
            return RunCmd(cmd);
        }

        /// <summary>
        /// 异步显示信息执行CMD语句
        /// </summary>
        /// <param name="cmd">要执行的CMD命令</param>
        private static string RunCmd(string cmd)
        {
            string message = "";
            try
            {
                Process proc = new Process();
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();
                proc.StandardInput.WriteLine(cmd);
                proc.StandardInput.WriteLine("exit ");
                string outStr = proc.StandardOutput.ReadToEnd();
                string errorInfo = proc.StandardError.ReadToEnd();
                proc.Close();
                message = outStr + errorInfo;
            }
            catch (Exception exception)
            {
                message = exception.Message;
                //LogHelper.Log(message);
            }
            return message;
        } 
    }
}

