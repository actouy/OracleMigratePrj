using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace AppClass
{
    public class ExcelBll
    {
        public static DataSet GetSheet(string sheetname, string filepath)
        {
            if (filepath == null || filepath.Trim() == "" || sheetname == null || sheetname.Trim() == "" || !File.Exists(filepath))
            {
                return null;
            }
            return ExcelDal.GetDataSet(filepath, sheetname, "");
        }
        public static DataSet GetSheet(string sheetname, FileInfo filepath)
        {
            string sourcefile = filepath.FullName.ToString();
            return ExcelDal.GetDataSet(sourcefile, sheetname, "");
        }
        public static DataTable GetChData(string sheetname, string columname , FileInfo filepath)
        {
            string sourcefile = filepath.FullName.ToString();
            return ExcelDal.GetDataSet(sourcefile, sheetname, columname, "");
        }
        public static string GetdSheetName(string filepath) 
        {
            string vName = string.Empty;
            Collection<string> vSheets = GetSheetNames(filepath);
            foreach (string tempstr in vSheets) 
            {
                DataTable mytable = GetSheet(tempstr, filepath).Tables[0];
                if (mytable.Rows.Count > 1) 
                {
                    vName = tempstr;
                    break;
                }
            }
            return vName;
        }
        public static Collection<string> GetSheetNames(string filepath) 
        {
            //对特殊字符进行规范处理
            Collection<string> vSheets = ExcelDal.GetSheetNames(filepath);
            Collection<string> vList = new Collection<string>();
            string vName = string.Empty;
            string pSheetName = string.Empty;
            for (int i = 0; i < vSheets.Count; i++)
            {
                string pStart = vSheets[i].Substring(0, 1);
                string pEnd = vSheets[i].Substring(vSheets[i].Length - 1, 1);
                if (pStart == "'" && pEnd == "'")
                {
                    vSheets[i] = vSheets[i].Substring(1, vSheets[i].Length - 2);
                }
                Char[] pChar = vSheets[i].ToCharArray();
                pSheetName = string.Empty;
                for (int j = 0; j < pChar.Length; j++)
                {
                    if (pChar[j].ToString() == "'" && pChar[j + 1].ToString() == "'")
                    {
                        pSheetName += pChar[j].ToString();
                        j++;
                    }
                    else
                    {
                        pSheetName += pChar[j].ToString();
                    }
                }
                vSheets[i] = pSheetName;
            }
            //当最后字符为$时移除
            for (int i = 0; i < vSheets.Count; i++)
            {
                pSheetName = vSheets[i];
                if (pSheetName.Substring(pSheetName.Length - 1, 1) == "$")
                {
                    vSheets[i] = pSheetName.Substring(0, pSheetName.Length - 1);
                }
            }
            //移除重复的Sheet名(因为特殊原因，通过这个方法获取的Sheet会有重名)
            for (int i = 0; i < vSheets.Count; i++)
            {
                if (vList.IndexOf(vSheets[i].ToLower()) == -1)
                {
                    vList.Add(vSheets[i]);
                }
            }
            return vList;
        }

        #region 生成用户代码块
        public static DataTable GetRepeatFields(string sheetname, string filepath, string id)
        {
            if (filepath == null || filepath.Trim() == "" || sheetname == null || sheetname.Trim() == "" || !File.Exists(filepath))
            {
                return null;
            }
            string sqlStr = id + " in (select " + id + " from [" + sheetname + "$] group by " + id + " having count(" + id + ") > 1)";
            DataTable myTable = ExcelDal.GetDataSet(filepath, sheetname, sqlStr).Tables[0];
            return myTable;
        }

        #endregion
    }
}
