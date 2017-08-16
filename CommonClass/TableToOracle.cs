using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TableToOracle
    {
        public static bool UpdateData(DataTable dt, string tablename, string ID, string idValue)
        {
            try
            {
                string SQLString = @"select * from " + tablename + " where instr(" + ID + ",'" + idValue + "')=1";



                return OracleHelper.UpdateToOrcl(dt, SQLString, ID);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 得到不为零的户编号
        /// </summary>
        /// <param name="idValue">村ID</param>
        /// <returns>户编号表</returns>
        public static DataTable GetFmNoTb(DBConfig mycon, string idValue)
        {
            string SQLString = @"SELECT TO_NUMBER(substr(HOUSE_BASE_INFO.HHID,12,4))  as SHHID,CARDID,HHNAME FROM   HOUSE_BASE_INFO  where instr(HHID ," + Convert.ToInt64(idValue) + ")=1 order by HOUSE_BASE_INFO.HHID";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        /// <summary>
        /// 得到重复户编号,依照村编号排序
        /// </summary>
        /// <returns>户编号表</returns>
        public static DataTable GetRepeatFamiTb(DBConfig mycon)
        {
            string SQLString = @"SELECT   HGID,CARDID,HHNAME,HHID,VID  FROM   HOUSE_BASE_INFO  where rowid in (select max(rowid) from HOUSE_BASE_INFO group by HHID having count(HHID)>1) order by VID";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        public static bool DBExist(string idValue)
        {
            string SQLString = @"SELECT * FROM  REGION_FILE  where REGION_FILE.RF_OWNER =  " + idValue;
            return OracleHelper.Exist(SQLString);
        }
        public static bool DBExistByHHID(DBConfig mycon,string idValue)
        {
            string SQLString = @"SELECT * FROM  REGION_FILE  where REGION_FILE.RF_OWNER =  '" + idValue + "'";
            return OracleHelper.Exist(SQLString, mycon);
        }
        public static bool DBExistByRF(DBConfig mycon,string idValue)
        {
            string SQLString = @"SELECT * FROM  REGION_FILE  where RF_FID =  '" + idValue + "'"; ;
            return OracleHelper.Exist(SQLString, mycon);
        }

        public static DataTable GetData(string tablename, string ID, string idValue) 
        {
            string SQLString = @"select * from " + tablename + " where instr(" + ID + ",'" + idValue + "')>0 order by " + ID;
            return OracleHelper.QueryTable(SQLString);
        }
        public static DataTable GetData(string tablename)
        {
            string SQLString = @"select * from " + tablename + " where rownum<=10";
            return OracleHelper.QueryTable(SQLString);
        }


        public static bool InsertData(string exdbname, string imdbname, string exportDBID, string importDBID)
        {
            throw new NotImplementedException();
        }
        #region 向临时表里面添加数据
        public static DataTable GetADDTb()
        {
            string SQLString = @"select HOUSE_BASIC.HB_ID,HB_HEADID,HB_LONGITUDE,HB_LATITUDE,HB_GDATE,HB_MEMO,PEOPLE_BASE.PB_CERT_NO,PEOPLE_BASE.PB_NAME from HOUSE_BASIC,PEOPLE_BASE where HOUSE_BASIC.HB_GDATE>20160615 and HOUSE_BASIC.HB_ID = PEOPLE_BASE.PB_HHID order by HB_ID";
            return OracleHelper.QueryTable(SQLString);
        }
        public static bool DBExistInTempHouse(string TableField, string idValue)
        {
            string SQLString = @"select * from TEMP_HOUSE_NINFO where " + TableField + "=  '" + idValue + "'";
            return OracleHelper.Exist(SQLString);
        }

        public static DataTable GetFmNoByID(string TableField, string idValue)
        {
            string SQLString = @"SELECT TEMP_HOUSE_NINFO.HGID,TEMP_HOUSE_NINFO.CARDID,TEMP_HOUSE_NINFO.HHNAME,TEMP_HOUSE_NINFO.PB_HHID FROM   TEMP_HOUSE_NINFO  where instr(" + TableField + " ," + Convert.ToInt64(idValue) + ")=1 order by CARDID";
            return OracleHelper.QueryTable(SQLString);
        }

        public static DataTable Get45HHIDTb(string idValue)
        {
            string SQLString = @"SELECT   TEMP_HOUSE_NINFO.HGID,TEMP_HOUSE_NINFO.CARDID,TEMP_HOUSE_NINFO.HHNAME,TEMP_HOUSE_NINFO.POP,TEMP_HOUSE_NINFO.PB_HHID,TEMP_HOUSE_NINFO.VGROUP,TEMP_HOUSE_NINFO.VID,TEMP_HOUSE_NINFO.VILL,TEMP_HOUSE_NINFO.TOWN  FROM   TEMP_HOUSE_NINFO  where TEMP_HOUSE_NINFO.VID =" + Convert.ToInt64(idValue) + " order by TEMP_HOUSE_NINFO.CARDID";
            return OracleHelper.QueryTable(SQLString);
        }

        public static DataTable Get45TargetTb(string idValue)
        {
            string SQLString = @"SELECT   TID, TOWN, VID, VILL, HHID, HHNAME, POP, SQCID
FROM      BIGDATAUSER.TEMP_FOUR_FIVE
WHERE   VID=" + Convert.ToInt64(idValue) + " and HHID = 0 order by VID";
            return OracleHelper.QueryTable(SQLString);
        }


        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="tablename">数据表名</param>
        /// <param name="ID">关键字</param>
        /// <param name="idValue">关键字值</param>
        /// <returns>成功返回TRUE</returns>
        public static bool InsertData(DataTable dt, string tablename, string ID, string idValue,Hashtable maptable)
        {
            try
            {
                string SQLString = @"select * from " + tablename + " where instr(" + ID + ",'" + idValue + "')=1";



                return OracleHelper.InsertToOrcl(dt, SQLString, ID, maptable);
            }
            catch
            {
                return false;
            }
        }
        #endregion
        public static int updateonesql(string sql) 
        {
            return OracleHelper.ExecuteSql(sql);
        }
        public static DataTable GetVillGRadeTb()
        {
            string SQLString = @"SELECT   VOLDID,VID,VILL, CTID, TID
FROM      BIGDATAUSER.VILL_GRADE
 where VID=0";
            return OracleHelper.QueryTable(SQLString);
        }

        /// <summary>
        /// 得到区域ID
        /// </summary>
        /// <returns>区域编号表</returns>

        public static DataTable GetMapFd(string tablename)
        {
            string SQLString = @"select ORACLFIELDNAME,EXCELFIELDNAME from ORACLEMAPEXCEL where ORACLETABLENAME='" + tablename + "'";
            return OracleHelper.QueryTable(SQLString);
        }

        #region 数据库语句生成
        public static string GetOrclStr(DataTable fieldtable)
        {
            string myoracle = "";
            StringBuilder strSql = new StringBuilder();
            string tablename = "";
            foreach (DataRow row in fieldtable.Rows)
            {

                if (tablename == "") 
                { 
                    tablename = row["TABLE_NAME"].ToString();
                    strSql.Append("create table ");
                    strSql.Append(tablename);
                    strSql.Append("(");
                }
                string cname = row["COLUMN_NAME"].ToString();
                string datatype = row["DATA_TYPE"].ToString();
                //string datalength = 

                strSql.Append(cname);
                strSql.Append(" ");
                strSql.Append(datatype);
                if (row["DATA_LENGTH"] != null)
                {
                    if ((datatype != "DATE") && (datatype != "CLOB") && (datatype != "TIMESTAMP(6)")) //oracle数据类型DATE和CLOB没有字段长度
                    { 
                        strSql.Append("(");
                        strSql.Append(row["DATA_LENGTH"].ToString());
                        strSql.Append(")");
                    }
                }
                strSql.Append(","); 
            }
            if (strSql.Length > 1)
            {
                strSql.Remove(strSql.Length - 1, 1);
                strSql.Append(")");
                myoracle = strSql.ToString();
            }
            return myoracle;
        }
        public static int CreateTable(string sqlstr, string DBServerIP, string OracleSID, string DBUserName, string DBPassword) 
        {
            return OracleHelper.ExecuteSql(sqlstr, DBServerIP, OracleSID, DBUserName, DBPassword);


         }
        public static DataTable GetFieldsName(string tablename, string DBServerIP, string OracleSID, string DBUserName, string DBPassword)
        {
            string SQLString = @"select column_name from user_tab_columns where table_name=upper('" + tablename + @"')";
            return OracleHelper.QueryTable(SQLString, DBServerIP, OracleSID, DBUserName, DBPassword);

        }

        #endregion

        public static bool InsertData(DataTable dt,string tablename, DBConfig mycon)
        {
                string SQLString = @"select * from " + tablename + " where rownum<11";



                return OracleHelper.InsertToOrcl(dt,SQLString, mycon);

        }

        public static bool OracleBulkCopy(DataTable dt, DBConfig mycon)
        {
            return OracleHelper.OracleBulkCopy(dt,mycon);

        }

        public static void DropTable(string tablename)
        {
            string SQLString = @"truncate table upper('" + tablename + @"')";
            int rslt = OracleHelper.ExecuteSql(SQLString);
        }

        public static void DropTable(string tablename, DBConfig mycon)
        {
            string SQLString = @"drop table " + tablename;
            int rslt = OracleHelper.ExecuteSql(SQLString, mycon);
        }
        public static bool OracleBulkCopy(DataSet ds, DBConfig mycon)
        {
            return OracleHelper.OracleBulkCopy(ds, mycon);

        }
        public static DataSet GetDataSet(string tablename)
        {
            string SQLString = @"select * from FPY." + tablename;

                return OracleHelper.Query(SQLString);
          
        }
        public static int CountData(string tablename)
        {
            string SQLString = @"select count(*) from FPY." + tablename;
            int rslt = Convert.ToInt32(OracleHelper.GetSingle(SQLString));
            return rslt;
        }
        public static int CountData(string tablename, DBConfig mycon)
        {
            string SQLString = @"select count(*) from " + tablename;
            int rslt = Convert.ToInt32(OracleHelper.GetSingle(SQLString,mycon));
            return rslt;
        }




        
        
        public static DataTable QueryZeroNO(string tablename, DBConfig mycon, string idValue)
        {
            string SQLString = @"SELECT   HGID,CARDID,HHNAME FROM  " + tablename + "  where HHID is null and  VID =" + Convert.ToInt64(idValue) + " order by CARDID";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        public static DataTable QueryZeroNoFromFPY(DBConfig mycon, string idValue)
        {
            return QueryZeroNO("HOUSE_BASE_INFO_FPY", mycon, idValue);
        }
        public static DataTable QueryNoTb(DBConfig mycon, string idValue)
        {
            string SQLString = @"SELECT TO_NUMBER(substr(HHID,12,4))  as SHHID,CARDID,HHNAME FROM   HOUSE_BASE_INFO_FPY  where HHID is not null and   instr(HHID ," + Convert.ToInt64(idValue) + ")=1 order by HHID";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        public static DataTable GetPBHHID(DBConfig mycon, string idValue)
        {
            string SQLString = @"SELECT  TO_NUMBER(substr(HHID,12,4)) as HHID   FROM   PEOPLE_INFO  where CARDID ='" + idValue + "'";
            return OracleHelper.QueryTable(SQLString, mycon);
        }
        public static DataTable GetVillTb(DBConfig mycon)
        {
            string SQLString = @"select REGION_DICT.RD_ID,REGION_DICT.RD_SHORTNAME,REGION_DICT.RD_PARENT_ID from REGION_DICT where REGION_DICT.RD_TYPE = '5' order by REGION_DICT.RD_ID";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        public static bool DBExist(DBConfig mycon, string idValue)
        {
            string SQLString = @"SELECT * FROM  REGION_FILE  where REGION_FILE.RF_OWNER =  " + idValue;
            return OracleHelper.Exist(SQLString, mycon);
        }
        public static DataTable QueryOwnerTb(DBConfig mycon, string idValue)
        {
            string SQLString = @"SELECT distinct TO_NUMBER(substr(RF_OWNER,12,4))  as SHHID FROM   REGION_FILE  where instr(VID ," + Convert.ToInt64(idValue) + ")=1 order by SHHID";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        public static DataTable GetHHIDFromPB(DBConfig mycon, string idValue)
        {
            string SQLString = @"SELECT distinct TO_NUMBER(substr(HHID,12,4)) as HHID,CARDID   FROM   PEOPLE_INFO  where VID =" + idValue + " and TO_NUMBER(substr(HHID,12,4)) >0 order by CARDID";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mycon"></param>
        /// <param name="tablename"></param>
        /// <param name="ID"></param>
        /// <param name="idValue"></param>
        /// <returns></returns>
        public static DataTable GetData(DBConfig mycon,string tablename, string ID, string idValue)
        {
            string SQLString = @"select * from " + tablename + " where instr(" + ID + ",'" + idValue + "')>0 order by " + ID;
            return OracleHelper.QueryTable(SQLString,mycon);
        }

        public static DataTable GetData(DBConfig mycon, string tablename)
        {
            string SQLString = @"select * from " + tablename;
            return OracleHelper.QueryTable(SQLString, mycon);
        }

        public static DataAdapter QueryZeroNoFromFPY(DBConfig mycon, string tablename, string ID, string idValue)
        {
            //DataAdapter myda = new DataAdapter();

            string SQLString = @"select PGID,HGID,PID,HHID,RELATION from " + tablename + " where PID is null and instr(" + ID + ",'" + idValue + "')>0 order by HHID";
            return OracleHelper.OracleBulkData(SQLString, mycon);
        }

        

        public static DataTable QueryNoTb(DBConfig mycon, string tablename, string ID, string idValue)
        {
            string SQLString = @"SELECT HHID,HGID FROM " + tablename + " where instr(" + ID + ",'" + idValue + "')>0 and HHID is not null  order by " + ID;
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        
        public static DataTable GetLatLgnTable(DBConfig mycon, string tablename, string idValue) 
        {
            string Fieldsname = " LONGITUDE,LATITUDE,HHID,VID";
            string strWhere = " instr(VID,'" + idValue + "')>0 ";
            string OrderField = " VID ";
            return QueryTable(mycon, tablename, Fieldsname, strWhere, OrderField);
        }

        
        public static int CreateTable(string sqlstr, DBConfig mycon)
        {
            return OracleHelper.ExecuteSql2(sqlstr, mycon);
        }


        #region 公共方法
        /// <summary>
        /// 清空数据，插入数据
        /// </summary>
        /// <param name="mycon">要操作的数据库</param>
        /// <param name="sourcetable">来源数据表</param>
        /// <param name="sourcefields">来源数据对应字段</param>
        /// <param name="targettable">目标数据表</param>
        /// <param name="targetfields">目标数据字段</param>
        /// <param name="strWhere">条件</param>
        /// <param name="OrderField">插入排序字段</param>
        /// <returns>成功返回TRUE，否则FALSE</returns>
        public static bool InsertFromOrcl(DBConfig mycon, string sourcetable, string sourcefields, string targettable, string targetfields, string strWhere, string OrderField)
        {
            string msg = "";
            bool rlst = false;
            string ClearSQL = @"Truncate table " + targettable;//清空目标数据库数据
            OracleHelper.ExecuteSql2(ClearSQL, mycon);


            StringBuilder InsertSQL = new StringBuilder("INSERT INTO " + targettable + "(" + targetfields + ")" + @"SELECT " + sourcefields + " FROM " + sourcetable);
            if (strWhere != "")
            {
                InsertSQL.Append(@" where " + strWhere);
            }
            if (OrderField != "")
            {
                InsertSQL.Append(@" order by " + OrderField);
            }
            int rslnum = OracleHelper.ExecuteSql2(InsertSQL.ToString(), mycon);
            if (rslnum > 0)
            {

                msg = targettable + " 从 " + sourcetable + " 添加了 " + rslnum + "条数据成功!\n\r";
                rlst = true;
            }
            else
            {
                msg = targettable + " 从 " + sourcetable + " 添加数据失败!\n\r";
                rlst = false;
            }
            return rlst; //OracleHelper.ExecuteSql2(sqlstr, mycon);
        }

        /// <summary>
        /// 清空数据，插入数据
        /// </summary>
        /// <param name="mycon">要操作的数据库</param>
        /// <param name="sourcetable">来源数据表</param>
        /// <param name="sourcefields">来源数据对应字段</param>
        /// <param name="targettable">目标数据表</param>
        /// <param name="targetfields">目标数据字段</param>
        /// <param name="strWhere">条件</param>
        /// <param name="OrderField">插入排序字段</param>
        /// <returns>成功返回TRUE，否则FALSE</returns>
        public static bool InsertFromOrcl(DBConfig mycon, string sourcetable, Collection<string> sourcefields, string targettable, Collection<string> targetfields, string strWhere, string OrderField)
        {
            string msg = "";
            bool rlst = false;
            StringBuilder Tfields = LinkFields(targetfields);
            StringBuilder SFields = LinkFields(sourcefields);
            string ClearSQL = @"Truncate table " + targettable;//清空目标数据库数据
            OracleHelper.ExecuteSql2(ClearSQL, mycon);


            StringBuilder InsertSQL = new StringBuilder("INSERT INTO " + targettable + "(" + Tfields + ")" + @"SELECT " + SFields + " FROM " + sourcetable);
            if (strWhere != "")
            {
                InsertSQL.Append(@" where " + strWhere);
            }
            if (OrderField != "")
            {
                InsertSQL.Append(@" order by " + OrderField);
            }
            int rslnum = OracleHelper.ExecuteSql2(InsertSQL.ToString(), mycon);
            if (rslnum > 0)
            {

                msg = targettable + " 从 " + sourcetable + " 添加了 " + rslnum + "条数据成功!\n\r";
                rlst = true;
            }
            else
            {
                msg = targettable + " 从 " + sourcetable + " 添加数据失败!\n\r";
                rlst = false;
            }
            return rlst; //OracleHelper.ExecuteSql2(sqlstr, mycon);
        }

        static string prostr(string excollumstr, string imcollumstr)
        {
            string resulstr = "";
            string[] s1 = excollumstr.Split(',');
            string[] s2 = imcollumstr.Split(',');
            for (int i = 0; i < s1.Length; i++)
            {

                if (i < s1.Length - 1)
                {
                    resulstr += s2[i] + "=" + s1[i] + ",";
                }
                else
                {
                    resulstr += s2[i] + "=" + s1[i];
                }
            }
            return resulstr;
        }

        /// <summary>
        /// orcl添加数据
        /// </summary>
        /// <param name="extablename">导出数据的表</param>
        /// <param name="imtablename">导入数据的表</param>
        /// <param name="exKeyID">导出关键字</param>
        /// <param name="imkeyID">导入关键字</param>
        /// <param name="excollumstr">导出字符串</param>
        /// <param name="imcollumstr">导入字符串</param>
        /// <returns></returns>
        public static int InsertFromOrcl(string extablename, string imtablename, string exKeyID, string imkeyID, string excollumstr, string imcollumstr)
        {
            try
            {
                //string exSQLString = "update HOUSE_INFO set (HOUSE_INFO.PID,HOUSE_INFO.HID) = (select PEOPLE_INFO.PB_ID,PEOPLE_INFO.PB_HHID from PEOPLE_INFO where PEOPLE_INFO.PB_CERT_NO = HOUSE_INFO.CARDID)";
                string exSQLString = prostr(excollumstr, imcollumstr);
                string imSQLString = "MERGE INTO " + imtablename
                    + " USING " + extablename
                    + " ON (" + imkeyID + " = " + exKeyID + ")"
                    + " WHEN MATCHED THEN "
                    + " UPDATE SET " + exSQLString
                    + " WHEN NOT MATCHED THEN "
                    + " INSERT (" + imkeyID + "," + imcollumstr + ") VALUES (" + exKeyID + "," + excollumstr + ")";
                return OracleHelper.ExecuteSql(imSQLString);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="mycon">要操作的数据库</param>
        /// <param name="sourcetable">来源数据表</param>
        /// <param name="sourcefields">来源数据对应字段</param>
        /// <param name="targettable">目标数据表</param>
        /// <param name="targetfields">目标数据字段</param>
        /// <param name="strWhere">条件</param>
        /// <returns>成功返回TRUE，否则FALSE</returns>
        public static bool UpdateFromOrcl(DBConfig mycon, string sourcetable, string sourcefields, string targettable, string targetfields, string strWhere, string OrderField)
        {
            string msg = "";
            bool rlst = false;



            StringBuilder InsertSQL = new StringBuilder(@"MERGE INTO (select " + targetfields + " from " + targettable + @" )a1
                                    USING (select " + sourcefields + " from " + sourcetable + @" order by " + OrderField + @" )a2
                                    ON ( " + strWhere + @") 
                                    WHEN MATCHED THEN 
                                    UPDATE SET
                                           a1.HHID = a2.HHID");
           
            int rslnum = OracleHelper.ExecuteSql2(InsertSQL.ToString(), mycon);
            if (rslnum > 0)
            {

                msg = targettable + " 从 " + sourcetable + " 更新了 " + rslnum + "条数据成功!\n\r";
                rlst = true;
            }
            else
            {
                msg = targettable + " 从 " + sourcetable + " 添加数据失败!\n\r";
                rlst = false;
            }
            return rlst; //OracleHelper.ExecuteSql2(sqlstr, mycon);
        }



        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="mycon">要操作的数据库</param>
        /// <param name="sourcetable">来源数据表</param>
        /// <param name="sourcefields">来源数据字段</param>
        /// <param name="sourcekeyfield">来源数据表主键</param>
        /// <param name="targettable">目标数据表</param>
        /// <param name="targetfields">目标数据字段</param>
        /// <param name="targetkeyfield">目标数据表主键</param>
        /// <param name="targetOrderField">源数据排序字段</param>
        /// <returns>成功返回TRUE，否则FALSE</returns>
        public static bool UpdateFromOrcl(DBConfig mycon, string sourcetable, Collection<string> sourcefields, string sourcekeyfield, string targettable, Collection<string> targetfields, string targetkeyfield, string targetOrderField)
        {
            string msg = "";
            bool rlst = false;
            StringBuilder Tfields = LinkFields(targetfields);
            Tfields.Append("," + targetkeyfield);
            StringBuilder SFields = LinkFields(sourcefields);
            SFields.Append("," + sourcekeyfield);
            StringBuilder strWhere = new StringBuilder("a1." + targetkeyfield + " = " + "a2." + sourcekeyfield);
            StringBuilder strEqlFields = LinkEqualFields("a1.", targetfields, "a2.", sourcefields);

            StringBuilder InsertSQL = new StringBuilder(@"MERGE INTO ( select " + Tfields.ToString() + " from " + targettable + @" )a1
                                    USING ( select " + SFields.ToString() + " from " + sourcetable + @" order by " + targetOrderField + @" )a2
                                    ON ( " + strWhere.ToString() + @" ) 
                                    WHEN MATCHED THEN 
                                    UPDATE SET 
                                    " + strEqlFields.ToString());
            //InsertSQL.Append();

            int rslnum = OracleHelper.ExecuteSql2(InsertSQL.ToString(), mycon);
            if (rslnum > 0)
            {

                msg = targettable + " 从 " + sourcetable + " 更新了 " + rslnum + "条数据成功!\n\r";
                rlst = true;
            }
            else
            {
                msg = targettable + " 从 " + sourcetable + " 添加数据失败!\n\r";
                rlst = false;
            }
            return rlst; //OracleHelper.ExecuteSql2(sqlstr, mycon);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="mycon">要操作的数据库</param>
        /// <param name="sourcetable">来源数据表</param>
        /// <param name="sourcefields">来源数据字段</param>
        /// <param name="sourcekeyfield">来源数据表主键</param>
        /// <param name="sourceWhere">源数据筛选条件</param>
        /// <param name="targettable">目标数据表</param>
        /// <param name="targetfields">目标数据字段</param>
        /// <param name="targetkeyfield">目标数据表主键</param>
        /// <param name="targetWhere">目标数据更新条件 </param>
        /// <param name="sourceOrderField">源数据排序字段</param>
        /// <returns>成功返回TRUE，否则FALSE</returns>
        public static bool UpdateFromOrcl(DBConfig mycon, string sourcetable, Collection<string> sourcefields, string sourcekeyfield, string sourceWhere, string targettable, Collection<string> targetfields, string targetkeyfield, string targetWhere, string sourceOrderField)
        {
            string msg = "";
            bool rlst = false;
            StringBuilder Tfields = LinkFields(targetfields);
            Tfields.Append("," + targetkeyfield);
            StringBuilder SFields = LinkFields(sourcefields);
            SFields.Append("," + sourcekeyfield);
            StringBuilder strWhere = new StringBuilder("a1." + targetkeyfield + " = " + "a2." + sourcekeyfield);
            StringBuilder strEqlFields = LinkEqualFields("a1.", targetfields, "a2.", sourcefields);
            StringBuilder InsertSQL = new StringBuilder("");
            string sourcesql = @" select " + SFields.ToString() + " from " + sourcetable;
            string targetsql = @" select " + Tfields.ToString() + " from " + targettable;

            if (sourceWhere != "")
            {
                sourcesql += " where " + sourceWhere;
            }
            if (sourceOrderField != "")
            {
                sourcesql += " order by " + sourceOrderField;
            }

            
            if (targetWhere != "")
            {
                targetsql += " where " + targetWhere;
            }

            InsertSQL.Append("MERGE INTO ( " + targetsql + @" )a1
                                    USING (  " + sourcesql  + @" )a2
                                    ON ( " + strWhere.ToString() + @" ) 
                                    WHEN MATCHED THEN 
                                    UPDATE SET 
                                    " + strEqlFields.ToString());


            int rslnum = OracleHelper.ExecuteSql2(InsertSQL.ToString(), mycon);
            if (rslnum > 0)
            {

                msg = targettable + " 从 " + sourcetable + " 更新了 " + rslnum + "条数据成功!\n\r";
                rlst = true;
            }
            else
            {
                msg = targettable + " 从 " + sourcetable + " 添加数据失败!\n\r";
                rlst = false;
            }
            return rlst; //OracleHelper.ExecuteSql2(sqlstr, mycon);
        }

        static StringBuilder LinkFields(Collection<string> strarry) 
        {
            StringBuilder strtmp = new StringBuilder();

            foreach (string strtemp in strarry)
            {
                strtmp.Append( strtemp + ",");
            }

            strtmp.Remove(strtmp.Length - 1, 1);
            return strtmp;

        }
        static StringBuilder LinkEqualFields(string aprefix, Collection<string> astrarry, string bprefix, Collection<string> bstrarry)
        {
            StringBuilder strtmp = new StringBuilder();
            if (astrarry.Count != bstrarry.Count)
                return null;
            if ((bprefix != "") && (aprefix != ""))
            {
                for (int i = 0; i < astrarry.Count; i++)
                {
                    strtmp.Append(aprefix + astrarry[i] + " = " + bprefix + bstrarry[i] + ",");

                }
            }
            else
            {
                for (int i = 0; i < astrarry.Count; i++)
                {
                    strtmp.Append(aprefix + astrarry[i] + " = " + bprefix + bstrarry[i] + ",");

                }
            }
            strtmp.Remove(strtmp.Length - 1, 1);
            return strtmp;
        }
        public static bool InsertIntoOrcl(DBConfig mycon, string tablename, DataTable mytable, string Fieldsname, string strWhere) 
        {
            DataAdapter myda = GetOracleBulkData(mycon, tablename, "", Fieldsname+ "='"+strWhere + "'", "");
            foreach (DataRow myrow in mytable.Rows) 
            {
                DataRow dr = myda.Mytable.NewRow();
                try
                {
                    dr.ItemArray = myrow.ItemArray;
                    myda.Mytable.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(myrow,ex);
                    continue;
                }
               
            }
            return ModiDB(myda, mycon);
        }
        public static DataAdapter GetOracleBulkData(DBConfig mycon, string tablename, string Fieldsname, string strWhere, string OrderField)
        {
            StringBuilder SQLString = new StringBuilder();
            if (Fieldsname != "")
            {
                SQLString.Append(@"select " + Fieldsname + " from " + tablename + " ");
            }
            else
            {
                SQLString.Append(@"select * from " + tablename + " ");
            }
            if (strWhere != "")
            {
                SQLString.Append(@" where " + strWhere);
            }
            if (OrderField != "")
            {
                SQLString.Append(@" order by " + OrderField);
            }

            //string SQLString = @"select PGID,HGID,PID,HHID,RELATION from " + tablename + " where PID is null and instr(" + ID + ",'" + strWhere + "')>0 order by " + ID;
            return OracleHelper.OracleBulkData(SQLString.ToString(), mycon);
        }
        public static bool ModiDB(DataAdapter myda, DBConfig mycon)
        {
            return OracleHelper.OracleBulkModi(myda, mycon);
        }
        public static DataTable QueryTable(DBConfig mycon, string tablename, string Fieldsname, string strWhere, string OrderField)
        {
            StringBuilder SQLString = new StringBuilder();
            if (Fieldsname != "")
            {
                SQLString.Append(@"select " + Fieldsname + " from " + tablename + " ");
            }
            else
            {
                SQLString.Append(@"select * from " + tablename + " ");
            }
            if (strWhere != "")
            {
                SQLString.Append(@" where " + strWhere);
            }
            if (OrderField != "")
            {
                SQLString.Append(@" order by " + OrderField);
            }
            return OracleHelper.QueryTable2(SQLString.ToString(), mycon);
        }
        public static void CloseCon()
        {
            OracleHelper.CloseConnection();
        }


        public static DataTable QueryTable(string p)
        {
            return OracleHelper.QueryTable(p);
        }

        public static DataTable QueryTable(string p, DBConfig mycon)
        {
            return OracleHelper.QueryTable2(p, mycon);
        }
        public static int ExecuteSql(string SQLString, DBConfig mycon)
        {
            //string SQLString = @"drop table " + tablename;
            return OracleHelper.ExecuteSql2(SQLString, mycon);
        }


        public static DataTable GetTablesName()
        {
            string SQLString = "select table_name from user_tables";
            return OracleHelper.QueryTable(SQLString);

        }
        public static DataTable GetSynonyms(string owner)
        {
            string SQLString = "SELECT * FROM sys.all_synonyms where owner = '" + owner + "'";
            return OracleHelper.QueryTable(SQLString);

        }
        public static DataTable GetFieldsName(string tablename)
        {
            string SQLString = @"select column_name from user_tab_columns where table_name=upper('" + tablename + @"')";
            return OracleHelper.QueryTable(SQLString);

        }
        public static DataTable GetFieldsName(string tablename, string owner)
        {
            string SQLString = @"select column_name from sys.all_tab_columns where table_name =upper('" + tablename + @"') and owner =upper('" + owner + "')";
            return OracleHelper.QueryTable(SQLString);

        }
        public static DataTable GetFieldsProperty(string tablename, string owner)
        {
            string SQLString = @"select * from sys.all_tab_columns where table_name =upper('" + tablename + @"') and owner =upper('" + owner + "') order by column_id";
            return OracleHelper.QueryTable(SQLString);

        }
        public static DataTable GetFieldsProperty(string tablename, DBConfig mycon)
        {
            string SQLString = @"select * from sys.all_tab_columns where table_name =upper('" + tablename + @"') order by column_id";
            return OracleHelper.QueryTable(SQLString, mycon);

        }
        #endregion


        #region 应用方法

        public static DataAdapter GetNOHHIDTable(DBConfig mycon, string tablename)
        {
            string Fieldsname = " HGID,HHID,CARDID,VID,MEMO";
            string strWhere = "substr(HHID,0,11) <> vid";// "  HHID is null ";
            string OrderField = " VID ";
            //string SQLString = @"SELECT HGID,HHID,CARDID,VID FROM   HOUSE_BASE_INFO_BJ  where HHID is null and   instr(VID," + Convert.ToInt64(idValue) + ")=1 order by VID";
            return GetOracleBulkData(mycon, tablename, Fieldsname, strWhere, OrderField);
        }
        public static DataAdapter GetNoVidInHelpStu(DBConfig mycon)
        {
            string Fieldsname = " COUNTY,ADDRESS,ISTRUE,VID,SGID";
            string strWhere = " VID is null ";
            string OrderField = " COUNTY ";
            //string SQLString = @"SELECT HGID,HHID,CARDID,VID FROM   HOUSE_BASE_INFO_BJ  where HHID is null and   instr(VID," + Convert.ToInt64(idValue) + ")=1 order by VID";
            return GetOracleBulkData(mycon, "HELP_STU", Fieldsname, strWhere, OrderField);
        }
        public static DataAdapter QueryNULLPID(DBConfig mycon, string tablename, string ID, string idValue)
        {
            //DataAdapter myda = new DataAdapter();

            string FieldsString = @" PGID,HGID,PID,HHID,RELATION ";
            string strWhere = " PID is null and instr(" + ID + ",'" + idValue + "')>0 ";
            return GetOracleBulkData(mycon,tablename,FieldsString,strWhere,"HHID");
        }

        public static DataTable GetRDIDTbByType(DBConfig mycon, string idValue)
        {
            string SQLString = @"select REGION_DICT.RD_ID,REGION_DICT.RD_SHORTNAME,REGION_DICT.RD_PARENT_ID from REGION_DICT where REGION_DICT.RD_TYPE ='" + idValue + "' order by REGION_DICT.RD_ID";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }

        public static DataTable GetRDIDTbLikeID(DBConfig mycon, string idValue)
        {
            string SQLString = @"select REGION_DICT.RD_ID,REGION_DICT.RD_SHORTNAME,REGION_DICT.RD_PARENT_ID from REGION_DICT where instr(REGION_DICT.RD_ID," + Convert.ToInt64(idValue) + ")>0 order by REGION_DICT.RD_ID";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }

        public static DataTable GetTablesName(DBConfig mycon)
        {
            string SQLString = "select table_name from user_tables";
            return OracleHelper.QueryTable2(SQLString, mycon);

        }
        public static DataTable GetRSByID(DBConfig mycon,string idValue)
        {
            string SQLString = @"select HOUSE_BASE_INFO.MAIN_REASON as REASON, count(*) as num from HOUSE_BASE_INFO where instr(HOUSE_BASE_INFO.VID," + Convert.ToInt64(idValue) + ")>0 and HOUSE_BASE_INFO.OUTPOORTATE <> '2' group by HOUSE_BASE_INFO.MAIN_REASON";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        #region 统计分析数据函数
        public static void CountAllRs(DBConfig mycon, string idValue) 
        {
            Collection<string> mystrs = new Collection<string>();
            Collection<GROUPREASON> myrss = new Collection<GROUPREASON>();
            Collection<string> otstrs = new Collection<string>();
            Collection<string> psstrs = new Collection<string>();
            otstrs.Add("all");
            otstrs.Add("1");
            otstrs.Add("2");
            psstrs.Add("all");
            psstrs.Add("国家标准");
            psstrs.Add("省定标准");

            foreach (string myout in otstrs) 
            {
                foreach (string myps in psstrs) 
                {
                    myrss = GetRsByCon(mycon, idValue, myout, myps);
                    if (myrss.Count > 0) 
                    { 
                        string mySqlstr = GetStrRsCol(myrss);
                        ExecuteSql(mySqlstr, mycon);
                    }
                }
            }
        }
        static Collection<GROUPREASON> GetRsByCon(DBConfig mycon, string idValue, string OUTTYPE, string POORSTANDARD) 
        {
            Collection<GROUPREASON> myrss = new Collection<GROUPREASON>();
            StringBuilder SQLString = new StringBuilder(@"select HOUSE_BASE_INFO.MAIN_REASON as REASON, count(*) as num, sum(HOUSE_BASE_INFO.POP) as pop from HOUSE_BASE_INFO");
            string sqlWhere = " where instr(HOUSE_BASE_INFO.VID," + Convert.ToInt64(idValue) + ")>0 ";
            if ((OUTTYPE != "all") && (OUTTYPE != "")) 
            {
                sqlWhere += @" and OUTPOORTATE ='" + OUTTYPE + "' ";
            }
            if ((POORSTANDARD != "all") && (POORSTANDARD != "")) 
            {
                sqlWhere += @" and PSTANDARD ='" + POORSTANDARD + "' ";
            }
            SQLString.Append(sqlWhere);
            string strGroup = "group by MAIN_REASON";
            SQLString.Append(strGroup);
            DataTable mytable = OracleHelper.QueryTable2(SQLString.ToString(), mycon);
            foreach (DataRow myrow in mytable.Rows)
            {
                GROUPREASON myreason = new GROUPREASON();
                myreason.REGIONID = Convert.ToDecimal(idValue);
                myreason.REASON = myrow["REASON"].ToString();
                if (myreason.REASON == "")
                {
                    myreason.REASON = "未知";
                }
                myreason.NUM = Convert.ToDecimal(myrow["num"].ToString());
                string popstr = myrow["pop"].ToString();
                if (popstr == "")
                    popstr = "1";
                myreason.POP = Convert.ToDecimal(popstr);
                myreason.OUTTYPE = OUTTYPE;
                myreason.POORSTANDARD = POORSTANDARD;
                myrss.Add(myreason);
            }
            return myrss;
        }
        static string GetStrRsCol(Collection<GROUPREASON> myreasons) 
        {
            StringBuilder SqlBatch = new StringBuilder();
            SqlBatch.Append("BEGIN ");
           
            foreach (GROUPREASON ms in myreasons) 
            {
                string sqlstr = @"insert into GROUPBYREASON values(";
                sqlstr += ms.REGIONID.ToString() + ",'";
                sqlstr += ms.REASON + "',";
                sqlstr += ms.NUM.ToString() + ",'";
                sqlstr += ms.POORSTANDARD + "','";
                sqlstr += ms.OUTTYPE + "',";
                sqlstr += ms.POP.ToString() + ",'";
                sqlstr += ms.PKID + "')";
                SqlBatch.Append(sqlstr);
                SqlBatch.Append(";");
            }
            SqlBatch.Append("End;");
            return SqlBatch.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mycon"></param>
        /// <param name="idValue"></param>
        /// <param name="list"></param>
        public static void InsertCountDataToTable<T>(DBConfig mycon, string idValue, Collection<T> list)
        {
            StringBuilder SqlBatch = new StringBuilder();
            SqlBatch.Append("BEGIN ");

            if (list.Count > 0)
            {
                Type entityType = typeof(T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
                string mySqlstr = @"insert into " + entityType.Name + " values(";
                foreach (T item in list)
                {

                    int i = 0;
                    foreach (PropertyDescriptor prop in properties)
                    {
                        i++;
                        if (i < properties.Count)
                        {                            
                            if (prop.PropertyType.Name == "String")
                                mySqlstr += "'" + prop.GetValue(item).ToString() + "',";
                            else
                                mySqlstr += prop.GetValue(item).ToString() + ",";
                        }
                        else
                        {
                            if (prop.PropertyType.Name == "String")
                                mySqlstr += "'" + prop.GetValue(item).ToString() + "'";
                            else
                                mySqlstr += prop.GetValue(item).ToString();
                        }                        
                    }
                    mySqlstr += ");";
                    SqlBatch.Append(mySqlstr);
                }   
            }
            SqlBatch.Append("End;");
            ExecuteSql(SqlBatch.ToString(), mycon);
        }

        //static Collection<T> GetCountData<T>(DBConfig mycon, string idValue)
        //{
        //    //Collection<GROUPREASON> myrss = new Collection<GROUPREASON>();
        //    //StringBuilder SQLString = new StringBuilder(@"select HOUSE_BASE_INFO.MAIN_REASON as REASON, count(*) as num, sum(HOUSE_BASE_INFO.POP) as pop from HOUSE_BASE_INFO");
        //    //string sqlWhere = " where instr(HOUSE_BASE_INFO.VID," + Convert.ToInt64(idValue) + ")>0 ";

        //    //if ((POORSTANDARD != "all") && (POORSTANDARD != ""))
        //    //{
        //    //    sqlWhere += @" and PSTANDARD ='" + POORSTANDARD + "' ";
        //    //}
        //    //SQLString.Append(sqlWhere);
        //    //string strGroup = "group by MAIN_REASON";
        //    //SQLString.Append(strGroup);
        //    //DataTable mytable = OracleHelper.QueryTable2(SQLString.ToString(), mycon);
        //    //foreach (DataRow myrow in mytable.Rows)
        //    //{
        //    //    GROUPREASON myreason = new GROUPREASON();
        //    //    myreason.REGIONID = Convert.ToDecimal(idValue);
        //    //    myreason.REASON = myrow["REASON"].ToString();
        //    //    if (myreason.REASON == "")
        //    //    {
        //    //        myreason.REASON = "未知";
        //    //    }
        //    //    myreason.NUM = Convert.ToDecimal(myrow["num"].ToString());
        //    //    string popstr = myrow["pop"].ToString();
        //    //    if (popstr == "")
        //    //        popstr = "1";
        //    //    myreason.POP = Convert.ToDecimal(popstr);
        //    //    myreason.OUTTYPE = OUTTYPE;
        //    //    myreason.POORSTANDARD = POORSTANDARD;
        //    //    myrss.Add(myreason);
        //    //}
        //    //return myrss;
        //}

        static DataTable GetCountDataTable(DBConfig mycon, string tablename, string groupfield, string idValue, string POORSTANDARD)
        {
            Collection<GROUPREASON> myrss = new Collection<GROUPREASON>();
            string mysqlstr = @"select " + groupfield + ", count(*) as num from " + tablename;
            if (tablename == "HOUSE_BASE_INFO")
                mysqlstr = @"select " + groupfield + ", count(*) as num, sum(POP) as pop from " + tablename;
            else
                mysqlstr = @"select " + groupfield + ", count(*) as num from " + tablename;
            StringBuilder SQLString = new StringBuilder(mysqlstr);
            string sqlWhere = " where instr(VID," + Convert.ToInt64(idValue) + ")>0 ";
            
            if ((POORSTANDARD != "0") && (POORSTANDARD != ""))
            {
                sqlWhere += @" and PSTANDARD ='" + POORSTANDARD + "' ";
            }
            SQLString.Append(sqlWhere);
            string strGroup = "group by " + groupfield;
            SQLString.Append(strGroup);
            return  OracleHelper.QueryTable2(SQLString.ToString(), mycon);
        }
        static Collection<GROUPBYFITNESS> GetCountCollection(DBConfig mycon, string idValue)
        {
            Collection<GROUPBYFITNESS> myrss = new Collection<GROUPBYFITNESS>();
            string tablename = "PEOPLE_INFO"; 
            string groupfield = "HEALTH";
            string POORSTANDARD = "0";

            DataTable mytable = GetCountDataTable(mycon, tablename, groupfield, idValue, POORSTANDARD);
            GROUPBYFITNESS myobj = new GROUPBYFITNESS();
            myobj.AAD105 = "0";
            foreach (DataRow myrow in mytable.Rows)
            {
                
                myobj.RID = Convert.ToDecimal(idValue);
                string health = myrow["HEALTH"].ToString();
                decimal NUM = Convert.ToDecimal(myrow["num"].ToString());
                switch (health) 
                {
                    case "健康":
                        myobj.GOODHEALTH = NUM;
                        break;
                    case "长期慢性病":
                        myobj.CHRONICAILMENT = NUM;
                        break;
                    case "患有大病":
                        myobj.SERIOUSILLNESS = NUM;
                        break;
                    case "残疾":
                        myobj.DISABILITY = NUM;
                        break;
                    default:
                        break;
                }                
            }
            myobj.ALLPOPULATION = myobj.GOODHEALTH + myobj.CHRONICAILMENT + myobj.SERIOUSILLNESS + myobj.DISABILITY;
            myrss.Add(myobj);
            POORSTANDARD = "1";

            return myrss;
        }
        public static void CountRs(DBConfig mycon, string idValue)
        {
            Collection<string> mystrs = new Collection<string>();
            Collection<GROUPREASON> myrss = new Collection<GROUPREASON>();
            Collection<string> otstrs = new Collection<string>();
            Collection<string> psstrs = new Collection<string>();
            otstrs.Add("all");
            otstrs.Add("1");
            otstrs.Add("2");
            psstrs.Add("all");
            psstrs.Add("国家标准");
            psstrs.Add("省定标准");

            foreach (string myout in otstrs)
            {
                foreach (string myps in psstrs)
                {
                    myrss = GetRsByCon(mycon, idValue, myout, myps);
                    if (myrss.Count > 0)
                    {
                        InsertCountDataToTable<GROUPREASON>(mycon, idValue, myrss);
                    }
                }
            }
        }
        #endregion
        public static DataTable GetDGByID(DBConfig mycon, string idValue)
        {
            string SQLString = @"select PEOPLE_INFO.DEGREE as DEGREE, count(*) as num from PEOPLE_INFO where instr(PEOPLE_INFO.VID," + Convert.ToInt64(idValue) + ")>0 and PEOPLE_INFO.HHID in (Select HOUSE_BASE_INFO.HHID from HOUSE_BASE_INFO where HOUSE_BASE_INFO.OUTPOORTATE <> '2') group by PEOPLE_INFO.DEGREE";
            return OracleHelper.QueryTable2(SQLString, mycon);
        }
        #endregion

        public static void ExecuteSqlTran(ArrayList sqllist, DBConfig mycon)
        {
           OracleHelper.ExecuteSqlTran(sqllist, mycon);
        }
    }

}
