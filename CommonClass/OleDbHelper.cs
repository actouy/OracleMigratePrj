using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>  
    /// 数据访问基础类(基于OleDb)   
    /// 可以用户可以修改满足自己项目的需要。  
    /// </summary>
    public abstract class OleDbHelper
    {

        //数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.       
        private static string connectionString = "连接字符串";


        #region 公用方法

        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool Exists(string strSql, params OleDbParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region  执行简单SQL语句

        /// <summary>  
        /// 执行SQL语句，返回影响的记录数  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSql(string SQLString)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.OleDb.OleDbException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        /// <summary>  
        /// 执行多条SQL语句，实现数据库事务。  
        /// </summary>  
        /// <param name="SQLStringList">多条SQL语句</param>       
        public static void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                OleDbTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (System.Data.OleDb.OleDbException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
            }
        }
        /// <summary>  
        /// 执行带一个存储过程参数的的SQL语句。  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSql(string SQLString, string content)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand cmd = new OleDbCommand(SQLString, connection);
                System.Data.OleDb.OleDbParameter myParameter = new System.Data.OleDb.OleDbParameter("@content", OleDbType.VarChar);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.OleDb.OleDbException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>  
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)  
        /// </summary>  
        /// <param name="strSQL">SQL语句</param>  
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand cmd = new OleDbCommand(strSQL, connection);
                System.Data.OleDb.OleDbParameter myParameter = new System.Data.OleDb.OleDbParameter("@fs", OleDbType.Binary);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.OleDb.OleDbException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>  
        /// 执行一条计算查询结果语句，返回查询结果（object）。  
        /// </summary>  
        /// <param name="SQLString">计算查询结果语句</param>  
        /// <returns>查询结果（object）</returns>  
        public static object GetSingle(string SQLString)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.OleDb.OleDbException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        /// <summary>  
        /// 执行查询语句，返回OleDbDataReader  
        /// </summary>  
        /// <param name="strSQL">查询语句</param>  
        /// <returns>OleDbDataReader</returns>  
        public static OleDbDataReader ExecuteReader(string strSQL)
        {
            OleDbConnection connection = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand(strSQL, connection);
            try
            {
                connection.Open();
                OleDbDataReader myReader = cmd.ExecuteReader();
                return myReader;
            }
            catch (System.Data.OleDb.OleDbException e)
            {
                throw new Exception(e.Message);
            }

        }
        /// <summary>  
        /// 执行查询语句，返回DataSet  
        /// </summary>  
        /// <param name="SQLString">查询语句</param>  
        /// <returns>DataSet</returns>  
        public static DataSet Query(string SQLString)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OleDbDataAdapter command = new OleDbDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }


        #endregion

        #region 执行带参数的SQL语句

        /// <summary>  
        /// 执行SQL语句，返回影响的记录数  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSql(string SQLString, params OleDbParameter[] cmdParms)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.OleDb.OleDbException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }


        /// <summary>  
        /// 执行多条SQL语句，实现数据库事务。  
        /// </summary>  
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OleDbParameter[]）</param>  
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                using (OleDbTransaction trans = conn.BeginTransaction())
                {
                    OleDbCommand cmd = new OleDbCommand();
                    try
                    {
                        //循环  
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OleDbParameter[] cmdParms = (OleDbParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }


        /// <summary>  
        /// 执行一条计算查询结果语句，返回查询结果（object）。  
        /// </summary>  
        /// <param name="SQLString">计算查询结果语句</param>  
        /// <returns>查询结果（object）</returns>  
        public static object GetSingle(string SQLString, params OleDbParameter[] cmdParms)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.OleDb.OleDbException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>  
        /// 执行查询语句，返回OleDbDataReader  
        /// </summary>  
        /// <param name="strSQL">查询语句</param>  
        /// <returns>OleDbDataReader</returns>  
        public static OleDbDataReader ExecuteReader(string SQLString, params OleDbParameter[] cmdParms)
        {
            OleDbConnection connection = new OleDbConnection(connectionString);
            OleDbCommand cmd = new OleDbCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                OleDbDataReader myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.OleDb.OleDbException e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>  
        /// 执行查询语句，返回DataSet  
        /// </summary>  
        /// <param name="SQLString">查询语句</param>  
        /// <returns>DataSet</returns>  
        public static DataSet Query(string SQLString, params OleDbParameter[] cmdParms)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.OleDb.OleDbException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, string cmdText, OleDbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;  
            if (cmdParms != null)
            {
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion
        #region 用户自定义方法
        /// <summary>
        /// 打开Excel文件
        /// </summary>
        /// <param name="filepath">文件的全路径</param>
        private static void GetConStr(string filepath)
        {
            string fileType = System.IO.Path.GetExtension(filepath);
            if (fileType == ".xls")
            {
                connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0\"";
            }
            else
            {
                connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filepath + ";" + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"";
            }
        }

        /// <summary>
        /// 查询数据返回数据集合
        /// </summary>
        /// <param name="filepath">Excel数据文件</param>
        /// <param name="SQLString">查询语句</param>
        /// <returns>返回数据集合</returns>
        public static DataSet Query(string filepath, string SQLString)
        {
            GetConStr(filepath);
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OleDbDataAdapter command = new OleDbDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 查询数据返回数据表
        /// </summary>
        /// <param name="filepath">Excel数据文件</param>
        /// <param name="SQLString">查询语句</param>
        /// <returns>返回数据表</returns>
        public static DataTable QueryDt(string filepath, string SQLString)
        {
            GetConStr(filepath);
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    OleDbDataAdapter command = new OleDbDataAdapter(SQLString, connection);
                    command.Fill(dt);
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dt;
            }
        }

        /// <summary>  
        /// 执行SQL语句，返回影响的记录数  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSql(string filepath, string SQLString, params OleDbParameter[] cmdParms)
        {
            GetConStr(filepath);
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.OleDb.OleDbException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }
        /// <summary>
        /// DataTable导入到excel文档
        /// </summary>
        /// <param name="filepath">excel文档</param>
        /// <param name="myTable">数据DataTable</param>
        /// <param name="SQLString">更新的Sql语句</param>
        /// <returns>成功返回结果为真</returns>
        public bool DTToExcel(string filepath, DataTable myTable, string SQLString)
        {
            //先得到汇总EXCEL的DataSet 主要目的是获得EXCEL在DataSet中的结构
            bool result = true;
            GetConStr(filepath);
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, connection, null, SQLString, null);
                using (OleDbDataAdapter myAdaper = new OleDbDataAdapter(cmd))
                {
                    try
                    {
                        OleDbCommandBuilder builder = new OleDbCommandBuilder(myAdaper);
                        //QuotePrefix和QuoteSuffix主要是对builder生成InsertComment命令时使用。 
                        builder.QuotePrefix = "["; //获取insert语句中保留字符（起始位置） 
                        builder.QuoteSuffix = "]"; //获取insert语句中保留字符（结束位置） 
                        DataTable newds = new DataTable();
                        myAdaper.Fill(newds);
                        for (int i = 0; i < myTable.Rows.Count; i++)
                        {
                            //在这里不能使用ImportRow方法将一行导入到news中，因为ImportRow将保留原来DataRow的所有设置(DataRowState状态不变)。
                            //在使用ImportRow后newds内有值，但不能更新到Excel中因为所有导入行的DataRowState!=Added 
                            DataRow nrow = newds.NewRow();
                            nrow.ItemArray = myTable.Rows[i].ItemArray;
                            newds.Rows.Add(nrow);
                        }
                        DataTable inserttable = newds.GetChanges(DataRowState.Added);
                        myAdaper.UpdateBatchSize = 1000;
                        if (inserttable != null)
                            myAdaper.Update(newds);
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(ex);
                        result = false;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 获得Excel文档的数据表名
        /// </summary>
        /// <param name="filePath">Excel文档</param>
        /// <returns>返回表名的字符串集合</returns>
        public static Collection<string> GetExlShsNames(string filePath)
        {
            Collection<string> list = new Collection<string>();
            GetConStr(filePath);
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                DataTable sheetNames = conn.GetOleDbSchemaTable
                (System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                conn.Close();
                foreach (DataRow dr in sheetNames.Rows)
                {
                    list.Add((string)dr[2]);
                }
                return list;
            }
        }
        #endregion
    }
}
