using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class CsvHelper
    {
        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string filePath)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            //StreamReader sr = new StreamReader(fs, encoding);
            //string fileContent = sr.ReadToEnd();
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        tableHead[i] = tableHead[i].Replace("\"", "");
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j].Replace("\"", "");
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[2] + " " + "DESC";
            }
            sr.Close();
            fs.Close();
            return dt;
        }

        // 只要将csv文件解析到程序里边就可以了,可以解析成dataset  然后循环插入数据库就可以了,这有根据csv得到dataset的代码   
        /// <summary>
        /// 将csv格式文件导成dataset
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static DataSet getCsv(string filePath, string fileName)
        {
            string strConn = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=";
            strConn += filePath;
            strConn += ";Extensions=asc,csv,tab,txt;";
            OdbcConnection con = new OdbcConnection(strConn);
            DataSet data = new DataSet();
            string sql = "select * from " + fileName;
            OdbcDataAdapter adp = new OdbcDataAdapter(sql, con);
            con.Open();
            adp.Fill(data, "csv");        
            return data;
        } 
    }
}
