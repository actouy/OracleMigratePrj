using Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Data;
using System.Threading;

namespace Common
{
    /// <summary>
    /// A helper class used to execute queries against an Oracle database
    /// </summary>
    public abstract class OracleHelper
    {

        //Read the connection strings from the configuration file
        //public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.AppSettings["OraConnString1"];
        //public static readonly string ConnectionStringInventoryDistributedTransaction = ConfigurationManager.AppSettings["OraConnString2"];
        //public static readonly string ConnectionStringOrderDistributedTransaction = ConfigurationManager.AppSettings["OraConnString3"];
        //public static readonly string ConnectionStringProfile = ConfigurationManager.AppSettings["OraProfileConnString"];
        //public static readonly string ConnectionStringMembership = ConfigurationManager.AppSettings["OraMembershipConnString"];
        //public static string connectionString = "DATA SOURCE=MYDB;PERSIST SECURITY INFO=True;USER ID=BIGDATAUSER;Password=bigdatauser";
        public static string connectionString = PubConstant.ConnectionString;
        //Create a hashtable for the parameter cached
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
        public static bool Exist(string strSql)
        {
            object obj = GetSingle(strSql);
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool Exists(string strSql, params OracleParameter[] cmdParms)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OracleException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="mycon">链接的数据库</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString,DBConfig mycon)
        {
            string text = PubConstant.GetConfigurationString("ConnectionString");
            string constr = string.Format(text, new object[] { mycon.DBServerIP, mycon.OracleSID, mycon.DBUserName, mycon.DBPassword });
            using (OracleConnection connection = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OracleException E)
                    {
                        connection.Close();
                        //throw new Exception(E.Message);
                        return 0;
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
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                OracleTransaction tx = conn.BeginTransaction();
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
                catch (OracleException E)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand(SQLString, connection);
                OracleParameter myParameter = new OracleParameter("@content", OracleDbType.Varchar2, 200);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (OracleException E)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand(strSQL, connection);
                OracleParameter myParameter = new OracleParameter("@fs", OracleDbType.LongRaw);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (OracleException E)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
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
                    catch (OracleException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(string strSQL)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand cmd = new OracleCommand(strSQL, connection);
            try
            {
                connection.Open();
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (OracleException e)
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
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public static DataTable QueryTable(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    //throw new Exception(ex.Message);
                    return new DataTable();
                }
                return ds.Tables[0];
            }
        }


        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (OracleException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OracleParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] cmdParms = (OracleParameter[])myDE.Value;
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
        public static object GetSingle(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
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
                    catch (OracleException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(string SQLString, params OracleParameter[] cmdParms)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (OracleException e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (OracleException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
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
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion

        #region 创建数据库
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string DBServerIP, string OracleSID, string DBUserName, string DBPassword)
        {
            string text = PubConstant.GetConfigurationString("ConnectionString");
            string constr = string.Format(text, new object[] { DBServerIP, OracleSID, DBUserName, DBPassword });
            using (OracleConnection connection = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        //if(Exist())
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OracleException E)
                    {
                        connection.Close();
                        //throw new Exception(E.Message);
                        LogHelper.WriteLog(E);;
                    }
                }
            }
            return 0;
        }
#endregion
        /// <summary>
        /// 批量数据导入Oracle
        /// </summary>
        /// <param name="dt">导入数据的DataTable</param>
        /// <param name="SQLString">SQL命令</param>
        /// <returns></returns>
        internal static bool InsertToOrcl(DataTable dt, string SQLString)
        {
            bool result = true;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, conn, null, SQLString, null);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    int count = 0;
                    string errorid = "";
                    string errormessage = string.Empty;
                    try
                    {
                        OracleCommandBuilder bd = new OracleCommandBuilder(da);
                        DataTable temp = new DataTable();

                        da.Fill(temp);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {


                            DataRow dr = temp.NewRow();
                            try
                            {
                                dr.ItemArray = dt.Rows[i].ItemArray;
                                temp.Rows.Add(dr);
                            }
                            catch (Exception ex)
                            {
                                errorid = errorid + "\r\n" + ex.Message;

                                errormessage += errorid;
                                continue;
                            }

                        }
                        DataTable inserttable = temp.GetChanges(DataRowState.Added);
                        da.UpdateBatchSize = 500;
                        if (inserttable != null)
                            count = da.Update(inserttable);
                        result = true;
                    }
                    catch (Exception ex)
                    {

                        LogHelper.WriteLog(ex, errormessage);
                        result = false;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 批量数据导入Oracle
        /// </summary>
        /// <param name="dt">导入数据的DataTable</param>
        /// <param name="SQLString">SQL命令</param>
        /// <param name="ID">导入数据的主关键字</param>
        /// <returns></returns>

        public static bool InsertToOrcl(DataTable dt, string SQLString, string ID)
        {
            bool result = true;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, conn, null, SQLString, null);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    int count = 0;
                    string errorid = "";
                    string errormessage = string.Empty;
                    try
                    {
                        OracleCommandBuilder bd = new OracleCommandBuilder(da);
                        DataTable temp = new DataTable();

                        da.Fill(temp);
                        //temp.DefaultView.Sort = "ID ASC";

                        //temp.PrimaryKey = new DataColumn[] { temp.Columns[ID] };
                        //dt.PrimaryKey = new DataColumn[] { dt.Columns[ID] };
                        //temp.Merge(dt,false);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            //errorid = dt.Rows[i][ID].ToString();

                            //string expression = ID + " = " + "'" + errorid + "'";
                            ////if (temp.Select(expression).Length >= 1)
                            //if (temp.Rows.Find(errorid) != null)
                            //{
                            //    errormessage += errorid + "已存在表中" + "\r\n";
                            //    //LogHelper.Log(errorid.ToString());
                            //    continue;
                            //}
                            
                            DataRow dr = temp.NewRow();
                            try
                            {
                                for(int j=0;j<dt.Columns.Count;j++)
                                {
                                    string cname = dt.Columns[j].ToString().Trim();
                                    if (temp.Columns.Contains(cname))
                                    {
                                        dr[cname] = dt.Rows[i][j];
                                    }                                                                   
                                }
                                temp.Rows.Add(dr); 
                            }
                            catch (Exception ex)
                            {
                                errorid = ID + "是" + errorid + "\r\n" + ex.Message;

                                errormessage += errorid;
                                //LogHelper.Log(errorid.ToString() + ex.Message);
                                continue;
                            }

                        }
                        DataTable inserttable = temp.GetChanges(DataRowState.Added);
                        da.UpdateBatchSize = 500;
                        if (inserttable != null)
                            count = da.Update(inserttable);

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        errorid = ID + "是" + errorid + "\r\n";
                        LogHelper.WriteLog(ex, errormessage);
                        result = false;
                    }
                }
            }
            return result;
        }

        public static bool UpdateToOrcl(DataTable dt, string SQLString, string ID)
        {
            bool result = true;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, conn, null, SQLString, null);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    int count = 0;
                    string pkid = "";
                    string errormessage = string.Empty;
                    try
                    {
                        OracleCommandBuilder bd = new OracleCommandBuilder(da);
                        DataTable temp = new DataTable();

                        da.Fill(temp);
                        temp.PrimaryKey = new DataColumn[] { temp.Columns[ID] };
                        dt.PrimaryKey = new DataColumn[] { dt.Columns[ID] };
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            pkid = dt.Rows[i][ID].ToString();

                            string expression = ID + " = " + "'" + pkid + "'";
                            if (temp.Rows.Find(pkid) != null)
                            {
                                temp.Rows.Find(pkid)["RD_LONGITUDE"] = dt.Rows[i]["longitude"];
                                temp.Rows.Find(pkid)["RD_LATITUDE"] = dt.Rows[i]["latitude"];
                                //temp.Rows[ID][LGID] = dt.Rows[i][LGID];
                                // dt.Rows[i][LTID]
                            }


                        }
                        DataTable updatetable = temp.GetChanges(DataRowState.Modified);
                        da.UpdateBatchSize = 500;
                        if (updatetable != null)
                            count = da.Update(updatetable);

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(ex, errormessage);
                        result = false;
                    }
                }
            }
            return result;
        }

        public static bool UpdateOracle(DataTable dt, DataTable vdt, string SQLString)
        {
            bool result = true;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, conn, null, SQLString, null);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    int count = 0;
                    string vid = "";
                    string errormessage = string.Empty;
                    try
                    {
                        OracleCommandBuilder bd = new OracleCommandBuilder(da);
                        DataTable temp = new DataTable();

                        da.Fill(temp);
                        temp.PrimaryKey = new DataColumn[] { temp.Columns["AAA001"] };
                        dt.PrimaryKey = new DataColumn[] { dt.Columns["AZC005"] };
                        vdt.PrimaryKey = new DataColumn[] { vdt.Columns["rd_oldid"] };
                        for (int i = 0; i < temp.Rows.Count; i++)
                        {

                            vid = temp.Rows[i]["AZC005"].ToString();

                            //string expression = ID + " = " + "'" + pkid + "'";
                            DataRow dr = dt.Rows.Find(vid);
                            if (dr != null)
                            {
                                Int64 myInt = Convert.ToInt64(dr["HHID"]);
                                myInt++;
                                dr["HHID"] = myInt;
                                temp.Rows[i]["HHID"] = myInt;
                            }
                            else 
                            {
                                DataRow vdr = vdt.Rows.Find(vid);
                                string vnid = vdr["rd_newid"].ToString();
                                Int64 myInt = Convert.ToInt64(vnid);
                                myInt = myInt * 10000 + 1;
                                DataRow newdr = dt.NewRow();
                                newdr["HHID"] = myInt;
                                newdr["AZC005"] = vid;
                                temp.Rows[i]["HHID"] = myInt;
                                dt.Rows.Add(newdr);
                            }


                        }
                        DataTable updatetable = temp.GetChanges(DataRowState.Modified);
                        da.UpdateBatchSize = 500;
                        if (updatetable != null)
                            count = da.Update(updatetable);

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        vid = "\t" + vid + "\r\n";
                        LogHelper.WriteLog(ex, errormessage);
                        result = false;
                    }
                }
            }
            return result;
        }



       

        public static bool UpdateFM(string SQLString)
        {
            bool result = true;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, conn, null, SQLString, null);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    int count = 0;
                    
                    string hhid = "";
                    string relaship = "";
                    string errormessage = string.Empty;
                   
                    try
                    {
                        OracleCommandBuilder bd = new OracleCommandBuilder(da);
                        DataTable temp = new DataTable();

                        da.Fill(temp);
                        //temp.PrimaryKey = new DataColumn[] { temp.Columns["PBID"] }; 
                        Hashtable maxpbidht = new Hashtable();
                        //dt.PrimaryKey = new DataColumn[] { dt.Columns["AZC005"] };                        
                        for (int i = 0; i < temp.Rows.Count; i++)
                        {                            
                            relaship = temp.Rows[i]["AAD004"].ToString();
                            switch(relaship)
                            {
                                case "户主":
                                    hhid = temp.Rows[i]["HHID"].ToString();
                                    string pbid = hhid+"01";
                                    temp.Rows[i]["PBID"] = Convert.ToInt64(pbid);
                                    break;
                                case "配偶":
                                    hhid = temp.Rows[i]["HHID"].ToString();
                                    pbid = hhid+"02";
                                    temp.Rows[i]["PBID"] = Convert.ToInt64(pbid);
                                    break;
                                default:
                                    hhid = temp.Rows[i]["HHID"].ToString();

                                    if (maxpbidht.Contains(hhid))
                                    {
                                        pbid = maxpbidht[hhid].ToString();
                                        Int64 myId = Convert.ToInt64(pbid)+1;
                                        maxpbidht[hhid] = myId;
                                        temp.Rows[i]["PBID"] = myId;
                                    }
                                    else
                                    {
                                        pbid = hhid+"03";
                                        maxpbidht[hhid] = pbid;
                                        temp.Rows[i]["PBID"] = Convert.ToInt64(pbid);
                                    }
                                        
                                    break;

                            }
                        }
                        DataTable updatetable = temp.GetChanges(DataRowState.Modified);
                        da.UpdateBatchSize = 500;
                        if (updatetable != null)
                            count = da.Update(updatetable);

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        hhid = "\t" + hhid + "\r\n";
                        LogHelper.WriteLog(ex, errormessage);
                        result = false;
                    }
                }
            }
            return result;
        }

        public static bool UpdateVillFm(DataTable dt, string SQLString)
        {
            bool result = true;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();

                PrepareCommand(cmd, conn, null, SQLString, null);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    int count = 0;

                    string errormessage = string.Empty;
                    try
                    {
                        OracleCommandBuilder bd = new OracleCommandBuilder(da);
                        DataTable temp = new DataTable();

                        da.Fill(temp);
                        count = dt.Rows.Count;
                        for (int i = 0; i < count; i++) 
                        {
                            temp.Rows[i]["PB_HHID"] = dt.Rows[i]["PB_HHID"];
                        }
                        
                        
                        DataTable updatetable = temp.GetChanges(DataRowState.Modified);
                        da.UpdateBatchSize = 500;
                        if (updatetable != null)
                            count = da.Update(updatetable);

                        result = true;
                    }
                    catch (Exception ex)
                    {

                        LogHelper.WriteLog(ex, errormessage);
                        result = false;
                    }
                }
            }
            return result;
        }


        internal static bool InsertToOrcl(DataTable dt, string SQLString, string ID, Hashtable maptable)
        {
            bool result = false;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, conn, null, SQLString, null);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    int count = 0;
                    string errorid = "";
                    string errormessage = string.Empty;
                    try
                    {
                        OracleCommandBuilder bd = new OracleCommandBuilder(da);
                        DataTable temp = new DataTable();

                        da.Fill(temp);

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {


                            DataRow dr = temp.NewRow();
                            try
                            {
                                foreach (DictionaryEntry de in maptable)
                                {

                                    string orclfield = de.Value.ToString();
                                    string exclfield = de.Key.ToString();
                                    if (temp.Columns.Contains(orclfield) && dt.Columns.Contains(exclfield))
                                        dr[orclfield] = dt.Rows[i][exclfield];
                                    else
                                        continue;

                                }
                                temp.Rows.Add(dr);
                            }
                            catch (Exception ex)
                            {
                                errorid = ID + "是" + errorid + "\r\n" + ex.Message;
                                errormessage += errorid;                               
                                continue;
                            }

                        }
                        DataTable inserttable = temp.GetChanges(DataRowState.Added);
                        da.UpdateBatchSize = 500;
                        if (inserttable != null)
                            count = da.Update(inserttable);

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(ex, errormessage);
                        result = false;
                    }
                }
            }
            return result;
        }

        internal static DataTable QueryTable(string SQLString, string DBServerIP, string OracleSID, string DBUserName, string DBPassword)
        {
            string text = PubConstant.GetConfigurationString("ConnectionString");
            string constr = string.Format(text, new object[] { DBServerIP, OracleSID, DBUserName, DBPassword });
            using (OracleConnection connection = new OracleConnection(constr))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    //throw new Exception(ex.Message);
                    LogHelper.WriteLog(ex);
                }
                return ds.Tables[0];
            }
        }

        internal static bool InsertToOrcl(DataTable dt, string SQLString, DBConfig mycon)
        {
            string text = PubConstant.GetConfigurationString("ConnectionString");
            string constr = string.Format(text, new object[] { mycon.DBServerIP, mycon.OracleSID, mycon.DBUserName, mycon.DBPassword });
            bool result = false;
            using (OracleConnection connection = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, SQLString, null);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    int count = 0;                    
                    string errormessage = string.Empty;
                    try
                    {
                        OracleCommandBuilder bd = new OracleCommandBuilder(da);
                        DataTable temp = new DataTable();

                        da.Fill(temp);

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {


                            DataRow dr = temp.NewRow();
                            try
                            {
                                dr.ItemArray = dt.Rows[i].ItemArray;
                                temp.Rows.Add(dr);
                            }
                            catch (Exception ex)
                            {
                               
                                errormessage += ex.Message+ "\r\n";
                                continue;
                            }

                        }
                        DataTable inserttable = temp.GetChanges(DataRowState.Added);
                        da.UpdateBatchSize = 500;
                        if (inserttable != null)
                            count = da.Update(inserttable);

                        result = true;
                    }
                    catch (Exception ex)
                    {

                        LogHelper.WriteLog(ex, errormessage);
                        result = false;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// DataTable批量写入到Oracle 
        /// </summary>
        /// <param name="dataTable">待写入DataTable</param>
        /// <returns>true:写入成功；false:写入失败</returns>
        public static bool OracleBulkCopy(DataTable dataTable, DBConfig mycon)
        {
            string text = PubConstant.GetConfigurationString("ConnectionString");
            string constr = string.Format(text, new object[] { mycon.DBServerIP, mycon.OracleSID, mycon.DBUserName, mycon.DBPassword });
            using (OracleConnection oracleConnection = new OracleConnection(constr))
            {
                oracleConnection.Open();

                Oracle.DataAccess.Client.OracleBulkCopy oracleBulkCopy = new Oracle.DataAccess.Client.OracleBulkCopy(constr);
                oracleBulkCopy.DestinationTableName = dataTable.TableName;
                try
                {
                    oracleBulkCopy.WriteToServer(dataTable);
                    return true;
                }
                catch(OracleException ex)
                {
                    LogHelper.WriteLog(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// DataTable批量写入到Oracle 
        /// </summary>
        /// <param name="dataTable">待写入DataTable</param>
        /// <returns>true:写入成功；false:写入失败</returns>
        public static bool OracleBulkCopy(DataSet ds, DBConfig mycon)
        {
            string text = PubConstant.GetConfigurationString("ConnectionString");
            string constr = string.Format(text, new object[] { mycon.DBServerIP, mycon.OracleSID, mycon.DBUserName, mycon.DBPassword });
            using (OracleConnection oracleConnection = new OracleConnection(constr))
            {
                oracleConnection.Open();

                Oracle.DataAccess.Client.OracleBulkCopy oracleBulkCopy = new Oracle.DataAccess.Client.OracleBulkCopy(constr);
                oracleBulkCopy.DestinationTableName = ds.DataSetName;
                try
                {
                    foreach (DataTable mytable in ds.Tables) 
                    {   
                        oracleBulkCopy.WriteToServer(mytable);
                        mytable.Clear();
                    }  
                    return true;
                }
                catch (OracleException ex)
                {
                    LogHelper.WriteLog(ex);
                    return false;
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, DBConfig mycon)
        {
            string text = PubConstant.GetConfigurationString("ConnectionString");
            string constr = string.Format(text, new object[] { mycon.DBServerIP, mycon.OracleSID, mycon.DBUserName, mycon.DBPassword });
            using (OracleConnection connection = new OracleConnection(constr))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    LogHelper.WriteLog(ex);
                }
                return ds;
            }
        }
        public static DataTable QueryTable(string SQLString, DBConfig mycon)
        {
            string text = PubConstant.GetConfigurationString("ConnectionString");
            string constr = string.Format(text, new object[] { mycon.DBServerIP, mycon.OracleSID, mycon.DBUserName, mycon.DBPassword });
            using (OracleConnection connection = new OracleConnection(constr))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    LogHelper.WriteLog(ex);
                }
                return ds.Tables[0];
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString,int RowsCount)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    int count = RowsCount / 100000;
                    int i = 0;
                    for (i = 0; i < count; i++)
                    {
                        command.Fill(ds, i * 100000, 100000 + i * 100000, "ds" + i.ToString());
                    }
                    command.Fill(ds, i * 100000, RowsCount, "ds" + i.ToString());
                }
                catch (OracleException ex)
                {
                    LogHelper.WriteLog(ex);
                }
                return ds;
            }
        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        internal static object GetSingle(string SQLString, DBConfig mycon)
        {
            if ((myconn == null) || (myconn.State != ConnectionState.Open))
                if (!OpenConnection(mycon)) 
                {
                    //LogHelper.WriteLog("数据库无法打开");
                    return null;
                }

            try
            {
                OracleCommand cmd = new OracleCommand(SQLString, myconn);
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
            catch (OracleException e)
            {
                LogHelper.WriteLog(e); ;
                return null;
            }

        }

        internal static bool Exist(string SQLString, DBConfig mycon)
        {
            object obj = GetSingle(SQLString, mycon);
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static OracleConnection myconn = null;

        public static bool OpenConnection (DBConfig mycon)
       {
            string text = PubConstant.GetConfigurationString("ConnectionString");
            string constr = string.Format(text, new object[] { mycon.DBServerIP, mycon.OracleSID, mycon.DBUserName, mycon.DBPassword });
                     
             bool restbool = false ;
                //OracleConnection connection  ;

            try
            {
                myconn = new OracleConnection(constr); 
                myconn.Open();
                restbool = true;
            }
            catch (OracleException e)
            {
                restbool = false;
                Thread.Sleep(500);
                LogHelper.WriteLog(e);;
            }


            return  restbool;
         }
        public static void CloseConnection ()
       {
           
             try
                  {
                      
                       
                      myconn.Close();
                     
                    }
                    catch (OracleException e)
                    {
                        LogHelper.WriteLog(e);;
                    }

           
                }


        public static DataTable QueryTable2(string SQLString, DBConfig mycon)
        {

            if ((myconn == null) || (myconn.State != ConnectionState.Open))
            {
                if (!OpenConnection(mycon))
                    return null;

            }   

            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
 
                OracleDataAdapter command = new OracleDataAdapter(SQLString, myconn);
                command.Fill(ds, "ds");
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (OracleException ex)
            {
                LogHelper.WriteLog(ex);
            }
            return dt;
         }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="mycon">链接的数据库</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql2(string SQLString, DBConfig mycon)
        {
            if ((myconn == null) || (myconn.State != ConnectionState.Open))
                if(!OpenConnection(mycon))
                    return 0;
    
            try
            {
                OracleCommand cmd = new OracleCommand(SQLString, myconn);
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (OracleException E)
            {

                LogHelper.WriteLog(E);;                
                return 0;

            }
        }

        /// <summary>
        /// 批量操作Oracle数据 
        /// </summary>
        /// <param name="dataTable">待写入DataTable</param>
        /// <returns>true:写入成功；false:写入失败</returns>
        public static bool OracleBulkModi(DataAdapter myda, DBConfig mycon)
        {
            bool restbool = false;
            if ((myconn == null) || (myconn.State != ConnectionState.Open))
                if (!OpenConnection(mycon))
                    return restbool;

            int count = 0;
            DataAdapter Dbda = myda;
            //OracleBulkCopy oracleBulkCopy = new OracleBulkCopy(constr);
            //oracleBulkCopy.DestinationTableName = dataTable.TableName;
            try
            {
                //oracleBulkCopy.WriteToServer(dataTable);
                DataTable inserttable = Dbda.Mytable.GetChanges(DataRowState.Added);

                if (inserttable != null)
                {
                    Dbda.Mydapter.UpdateBatchSize = 1000;
                    count = Dbda.Mydapter.Update(inserttable);
                    if (count == inserttable.Rows.Count)
                        restbool = true;
                }
                else
                {
                    DataTable moditable = Dbda.Mytable.GetChanges(DataRowState.Modified);

                    if (moditable != null)
                    {
                        moditable.TableName = "moditable";
                        Dbda.Mydapter.UpdateBatchSize = 1000;


                        count = Dbda.Mydapter.Update(moditable);

                        if (count == moditable.Rows.Count)
                            restbool = true;
                    }
                }
            }
            catch(OracleException ex)
            {
                restbool = false;
                LogHelper.WriteLog(ex);
            }
            return restbool;
        }
        public static DataAdapter OracleBulkData(string SQLString,DBConfig mycon) 
        {
            //bool result = true;
            DataAdapter myda = new DataAdapter();
            if ((myconn == null) || (myconn.State != ConnectionState.Open))
                if (!OpenConnection(mycon))
                    return myda;
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, myconn, null, SQLString, null);
            myda.Mydapter = new OracleDataAdapter(cmd);
            try
            {
                OracleCommandBuilder bd = new OracleCommandBuilder(myda.Mydapter);
                myda.Mydapter.Fill(myda.Mytable);
            }
            catch(OracleException ex)
            {
                LogHelper.WriteLog(ex);
                return myda;
            }
            return myda;
        }




        public static void ExecuteSqlTran(ArrayList sqllist, DBConfig mycon)
        {

            if ((myconn == null) || (myconn.State != ConnectionState.Open))
                if (!OpenConnection(mycon))
                    return;
            
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = myconn;

            OracleTransaction tx = myconn.BeginTransaction();
            cmd.Transaction = tx;
            try
            {
                for (int n = 0; n < sqllist.Count; n++)
                {
                    string strsql = sqllist[n].ToString();
                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
            }
            catch (OracleException E)
            {
                tx.Rollback();
                LogHelper.WriteLog(E);;
            }
        }
    }

    public class DataAdapter
    {
        public DataAdapter() 
        {
            Mydapter = new OracleDataAdapter();
            Mytable = new DataTable();
        }
        public OracleDataAdapter Mydapter
        {
            get;
            set;
        }
        public DataTable Mytable
        {
            get;
            set;
        }
    }
}
