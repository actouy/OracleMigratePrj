using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace AppClass
{
    public class ExcelDal
    {
        public static DataSet GetDataSet(string filepath, string sheetname, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM [");
            strSql.Append(sheetname);
            strSql.Append("$]");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return OleDbHelper.Query(filepath, strSql.ToString());
        }
        public static DataTable GetDataSet(string filepath, string sheetname, string columname , string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            strSql.Append(columname);
            strSql.Append(" FROM [");
            strSql.Append(sheetname);
            strSql.Append("$]");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return OleDbHelper.QueryDt(filepath, strSql.ToString());
        }
        public static Collection<string> GetSheetNames(string filepath)
        {
            return OleDbHelper.GetExlShsNames(filepath);
        }

    }
}
