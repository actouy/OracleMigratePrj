using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace AppClass
{
    public class ExcelOper
    {
        /// <summary>
        /// 将DataTable转化为XML输出
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <param name="fileName">文件名称</param>
        public static void DataTableToXML(DataTable dataTable, string fileName)
        {
            //指定程序安装目录
            string filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + fileName;
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(fs))
                {
                    dataTable.WriteXml(xmlWriter, XmlWriteMode.IgnoreSchema);
                }
            }
            Process.Start(filePath);
        }

        /// <summary>
        /// 将Excel多单一表转化为DataSet数据集对象
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <returns>转化的数据集</returns>
        public static DataSet ExcelToDataSet(string filePath)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SpreadsheetDocument spreadDocument = SpreadsheetDocument.Open(filePath, false))
                {
                    //指定WorkbookPart对象
                    WorkbookPart workBookPart = spreadDocument.WorkbookPart;
                    //获取Excel中SheetName集合
                    Collection<string> sheetNames = GetSheetNames(workBookPart);

                    foreach (string sheetName in sheetNames)
                    {
                        DataTable dataTable = WorkSheetToTable(workBookPart, sheetName);
                        if (dataTable != null)
                        {
                            dataSet.Tables.Add(dataTable);//将表添加到数据集
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
            return dataSet;
        }

        /// <summary>
        /// 将Excel单一表转化为DataTable对象
        /// </summary>
        /// <param name="sheetName">SheetName</param>
        /// <param name="stream">Excel文件路径</param>
        /// <returns>DataTable对象</returns>
        public static DataTable ExcelToDataTable(string sheetName, string filePath)
        {
            DataTable dataTable = new DataTable();
            try
            {
                //根据Excel流转换为spreadDocument对象
                using (SpreadsheetDocument spreadDocument = SpreadsheetDocument.Open(filePath, false))//Excel文档包
                {
                    //Workbook workBook = spreadDocument.WorkbookPart.Workbook;//主文档部件的根元素
                    //Sheets sheets = workBook.Sheets;//块级结构（如工作表、文件版本等）的容器
                    WorkbookPart workBookPart = spreadDocument.WorkbookPart;
                    //获取Excel中SheetName集合
                    Collection<string> sheetNames = GetSheetNames(workBookPart);

                    if (sheetNames.Contains(sheetName))
                    {
                        //根据WorkSheet转化为Table
                        dataTable = WorkSheetToTable(workBookPart, sheetName);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
            return dataTable;
        }

        /// <summary>
        /// 根据WorkbookPart获取所有SheetName
        /// </summary>
        /// <param name="workBookPart"></param>
        /// <returns>SheetName集合</returns>
        private static Collection<string> GetSheetNames(WorkbookPart workBookPart)
        {
            Collection<string> sheetNames = new Collection<string>();
            Sheets sheets = workBookPart.Workbook.Sheets;
            foreach (Sheet sheet in sheets)
            {
                string sheetName = sheet.Name;
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheetNames.Add(sheetName);
                }
            }
            return sheetNames;
        }

        /// <summary>
        /// 根据WorkbookPart和sheetName获取该Sheet下所有Row数据
        /// </summary>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <param name="sheetName">SheetName</param>
        /// <returns>该SheetName下的所有Row数据</returns>
        public static IEnumerable<Row> GetWorkBookPartRows(WorkbookPart workBookPart, string sheetName)
        {
            IEnumerable<Row> sheetRows = null;
            //根据表名在WorkbookPart中获取Sheet集合
            IEnumerable<Sheet> sheets = workBookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName);
            if (sheets.Count() == 0)
            {
                return null;//没有数据
            }

            WorksheetPart workSheetPart = workBookPart.GetPartById(sheets.First().Id) as WorksheetPart;
            //获取Excel中得到的行
            sheetRows = workSheetPart.Worksheet.Descendants<Row>();

            return sheetRows;
        }

        /// <summary>
        /// 根据WorkbookPart和表名创建DataTable对象
        /// </summary>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <param name="tableName">表名</param>
        /// <returns>转化后的DataTable</returns>
        private static DataTable WorkSheetToTable(WorkbookPart workBookPart, string sheetName)
        {
            //创建Table
            DataTable dataTable = new DataTable(sheetName);

            //根据WorkbookPart和sheetName获取该Sheet下所有行数据
            IEnumerable<Row> sheetRows = GetWorkBookPartRows(workBookPart, sheetName);
            if (sheetRows == null || sheetRows.Count() <= 0)
            {
                return null;
            }

            //将数据导入DataTable,假定第一行为列名,第二行以后为数据
            foreach (Row row in sheetRows)
            {
                //获取Excel中的列头
                if (row.RowIndex == 1)
                {
                    Collection<DataColumn> listCols = GetDataColumn(row, workBookPart);
                    dataTable.Columns.AddRange(listCols.ToArray());
                }
                else
                {
                    //Excel第二行同时为DataTable的第一行数据
                    DataRow dataRow = GetDataRow(row, dataTable, workBookPart);
                    if (dataRow != null)
                    {
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }
            return dataTable;
        }

        /// <summary>
        /// 根据WorkbookPart获取NumberingFormats样式集合
        /// </summary>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <returns>NumberingFormats样式集合</returns>
        private static Collection<string> GetNumberFormatsStyle(WorkbookPart workBookPart)
        {
            Collection<string> dicStyle = new Collection<string>();
            Stylesheet styleSheet = workBookPart.WorkbookStylesPart.Stylesheet;
            OpenXmlElementList list = null;
            if (styleSheet.NumberingFormats !=null)
            {
                list = styleSheet.NumberingFormats.ChildElements;//获取NumberingFormats样式集合

                foreach (var element in list)//格式化节点
                {
                    if (element.HasAttributes)
                    {
                        using (OpenXmlReader reader = OpenXmlReader.Create(element))
                        {
                            if (reader.Read())
                            {
                                if (reader.Attributes.Count > 0)
                                {
                                    string numFmtId = reader.Attributes[0].Value;//格式化ID
                                    string formatCode = reader.Attributes[1].Value;//格式化Code
                                    dicStyle.Add(formatCode);//将格式化Code写入List集合
                                }
                            }
                        }
                    }
                }
            }
            return dicStyle;
        }

        /// <summary>
        /// 根据行对象和WorkbookPart对象获取DataColumn集合
        /// </summary>
        /// <param name="row">Excel中行记录</param>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <returns>返回DataColumn对象集合</returns>
        private static Collection<DataColumn> GetDataColumn(Row row, WorkbookPart workBookPart)
        {
            Collection<DataColumn> listCols = new Collection<DataColumn>();
            foreach (Cell cell in row)
            {
                string cellValue = GetCellValue(cell, workBookPart);
                DataColumn col = new DataColumn(cellValue);
                listCols.Add(col);
            }
            return listCols;
        }

        /// <summary>
        /// 根据Excel行\数据库表\WorkbookPart对象获取数据DataRow
        /// </summary>
        /// <param name="row">Excel中行对象</param>
        /// <param name="dateTable">数据表</param>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <returns>返回一条数据记录</returns>
        private static DataRow GetDataRow(Row row, DataTable dateTable, WorkbookPart workBookPart)
        {
            //读取Excel中数据,一一读取单元格,若整行为空则忽视该行
            DataRow dataRow = dateTable.NewRow();
            IEnumerable<Cell> cells = row.Elements<Cell>();

            int cellIndex = 0;//单元格索引
            int nullCellCount = cellIndex;//空行索引
            foreach (Cell cell in row)
            {
                string cellVlue = GetCellValue(cell, workBookPart);
                if (string.IsNullOrEmpty(cellVlue))
                {
                    nullCellCount++;
                }

                dataRow[cellIndex] = cellVlue;
                cellIndex++;
            }
            if (nullCellCount == cellIndex)//剔除空行
            {
                dataRow = null;//一行中单元格索引和空行索引一样
            }
            return dataRow;
        }

        /// <summary>
        /// 根据Excel单元格和WorkbookPart对象获取单元格的值
        /// </summary>
        /// <param name="theCell">Excel单元格对象</param>
        /// <param name="workBookPart">Excel WorkbookPart对象</param>
        /// <returns>单元格的值</returns>
        public static string GetCellValue(Cell theCell, WorkbookPart workBookPart)
        {
            string Value = string.Empty;
            if (theCell.ChildElements.Count == 0)//Cell节点下没有子节点
            {
                return Value;
            }
            string cellRefId = theCell.CellReference.InnerText;//获取引用相对位置
            string cellInnerText = theCell.CellValue.InnerText;//获取Cell的InnerText
            Value = cellInnerText;//指定默认值(其实用来处理Excel中的数字)

            //获取WorkbookPart中NumberingFormats样式集合
            Collection<string> dicStyles = GetNumberFormatsStyle(workBookPart);
            //获取WorkbookPart中共享String数据
            SharedStringTable sharedTable = workBookPart.SharedStringTablePart.SharedStringTable;

            try
            {
                EnumValue<CellValues> cellType = theCell.DataType;//获取Cell数据类型
                if (cellType != null)//Excel对象数据
                {
                    switch (cellType.Value)
                    {
                        case CellValues.SharedString://字符串
                            //获取该Cell的所在的索引
                            int cellIndex = int.Parse(cellInnerText);
                            Value = sharedTable.ChildElements[cellIndex].InnerText;
                            break;
                        case CellValues.Boolean://布尔
                            Value = (cellInnerText == "1") ? "TRUE" : "FALSE";
                            break;
                        case CellValues.Date://日期
                            Value = Convert.ToDateTime(cellInnerText).ToString();
                            break;
                        case CellValues.Number://数字
                            Value = Convert.ToDecimal(cellInnerText).ToString();
                            break;
                        default: Value = cellInnerText; break;
                    }
                }
                else//格式化数据
                {
                    if (dicStyles.Count > 0 && theCell.StyleIndex != null)//对于数字,cell.StyleIndex==null
                    {
                        int styleIndex = Convert.ToInt32(theCell.StyleIndex.Value);
                        string cellStyle = dicStyles[styleIndex - 1];//获取该索引的样式
                        if (cellStyle.Contains("yyyy") || cellStyle.Contains("h")
                            || cellStyle.Contains("dd") || cellStyle.Contains("ss"))
                        {
                            //如果为日期或时间进行格式处理,去掉“;@”
                            cellStyle = cellStyle.Replace(";@", "");
                            while (cellStyle.Contains("[") && cellStyle.Contains("]"))
                            {
                                int otherStart = cellStyle.IndexOf('[');
                                int otherEnd = cellStyle.IndexOf("]");

                                cellStyle = cellStyle.Remove(otherStart, otherEnd - otherStart + 1);
                            }
                            double doubleDateTime = double.Parse(cellInnerText);
                            DateTime dateTime = DateTime.FromOADate(doubleDateTime);//将Double日期数字转为日期格式
                            if (cellStyle.Contains("m")) { cellStyle = cellStyle.Replace("m", "M"); }
                            if (cellStyle.Contains("AM/PM")) { cellStyle = cellStyle.Replace("AM/PM", ""); }
                            Value = dateTime.ToString(cellStyle);//不知道为什么Excel 2007中格式日期为yyyy/m/d
                        }
                        else//其他的货币、数值
                        {
                            cellStyle = cellStyle.Substring(cellStyle.LastIndexOf('.') - 1).Replace("\\", "");
                            decimal decimalNum = decimal.Parse(cellInnerText);
                            Value = decimal.Parse(decimalNum.ToString(cellStyle)).ToString();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                //string expMessage = string.Format("Excel中{0}位置数据有误,请确认填写正确！", cellRefId);
                //throw new Exception(expMessage);
                Value = "N/A";
                throw new Exception(exp.Message);
            }
            return Value;
        }
        public static string GetCellValue(string fileName, string sheetName, string addressName) 
        {
            string value = null;

            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, false))
            {
                // Retrieve a reference to the workbook part.
                WorkbookPart wbPart = document.WorkbookPart;

                // Find the sheet with the supplied name, and then use that 
                // Sheet object to retrieve a reference to the first worksheet.
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().
                  Where(s => s.Name == sheetName).FirstOrDefault();

                // Throw an exception if there is no sheet.
                if (theSheet == null)
                {
                    throw new ArgumentException("sheetName");
                }

                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));                

                // Use its Worksheet property to get a reference to the cell 
                // whose address matches the address you supplied.
                Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                  Where(c => c.CellReference == addressName).FirstOrDefault();
                SharedStringItem myshar = new SharedStringItem();

                // If the cell does not exist, return an empty string.
                if (theCell != null)
                {
                    value = GetValue(theCell, wbPart.SharedStringTablePart);
                    

                    // If the cell represents an integer number, you are done. 
                    // For dates, this code returns the serialized value that 
                    // represents the date. The code handles strings and 
                    // Booleans individually. For shared strings, the code 
                    // looks up the corresponding value in the shared string 
                    // table. For Booleans, the code converts the value into 
                    // the words TRUE or FALSE.             
                }

            }
            return value;
        }
        //
        private static String GetValue(Cell cell, SharedStringTablePart stringTablePart)//未搞明白的用法
        {

            if (cell.ChildElements.Count == 0)

                return null;

            //get cell value

            String value = cell.CellValue.InnerText;

            //Look up real value from shared string table

            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))

                value = stringTablePart.SharedStringTable

                .ChildElements[Int32.Parse(value)]

                .InnerText;

            return value;

        }
        /// <summary>
        /// 更改单元格值
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="sheetName">工作表</param>
        /// <param name="celladdress">单元格地址</param>
        /// <param name="cellvalue">修改的值</param>
        public static void SetCellValue(string fileName, string sheetName, string celladdress, string cellvalue)
        {
                        
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, true))
            {
               
            //    WorkbookPart wbPart = document.WorkbookPart;
                
            //    Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().
            //      Where(s => s.Name == sheetName).FirstOrDefault();

                
            //    if (theSheet == null)
            //    {
            //        throw new ArgumentException("sheetName");
            //    }

               
            //    WorksheetPart wsPart =
            //        (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                
            //    Cell theCell = wsPart.Worksheet.Descendants<Cell>().
            //      Where(c => c.CellReference == addressName).FirstOrDefault();

                
            //    if (theCell != null)
            //    {
            //        theCell.CellValue.Text = addressvalue;            
            //    }
            //    wsPart.Worksheet.Save();
                Worksheet myworksheet = ReturnWorsheet(document, sheetName);
                UpdateCellValue(myworksheet, celladdress, cellvalue);
            }
            

        }
        /// <summary>
        /// 更改单元格值
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="sheetName">工作表</param>
        /// <param name="celladdress">单元格地址</param>
        /// <param name="cellvalue">修改的值</param>
        public static void SetCellValue(string fileName, string sheetName, string celladdress, int cellvalue)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, true))
            {

            Worksheet myworksheet = ReturnWorsheet(document, sheetName);
            UpdateCellValue(myworksheet, celladdress, cellvalue);
            }

        }
        private static Worksheet ReturnWorsheet(SpreadsheetDocument document, string sheetName) 
        {
            WorkbookPart wbPart = document.WorkbookPart;

            Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().
                Where(s => s.Name == sheetName).FirstOrDefault();


            if (theSheet == null)
            {
                throw new ArgumentException("sheetName");
            }


            WorksheetPart wsPart =
                (WorksheetPart)(wbPart.GetPartById(theSheet.Id));
            Worksheet mysheet = wsPart.Worksheet;

            return mysheet;
        }
        
        private static void UpdateCellValue(Worksheet worksheet, string celladdress, string value)
        {
            if (worksheet == null)
            {
                throw new ArgumentException("sheetName");
            }
                        
            Cell cell = worksheet.Descendants<Cell>().
              Where(c => c.CellReference == celladdress).FirstOrDefault();
            //if (cell.CellValue == null)
                cell.CellValue = new CellValue(value.ToString());
            //else
            //    cell.CellValue.Text = value.ToString(); 
            cell.DataType = new EnumValue<CellValues>(CellValues.String);
            worksheet.Save();
        }
        private static void UpdateCellValue(Worksheet worksheet, string celladdress, int value)
        {
            if (worksheet == null)
            {
                throw new ArgumentException("sheetName");
            }
            Cell cell = worksheet.Descendants<Cell>().
              Where(c => c.CellReference == celladdress).FirstOrDefault();
            //if (cell.CellValue == null) 
                cell.CellValue = new CellValue(value.ToString());
            //else
            //    cell.CellValue.Text = value.ToString();           
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
            worksheet.Save();
        }

        /// <summary>
        /// 获取Excel中多表的表名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Collection<string> GetExcelSheetNames(string filePath)
        {
            string sheetName = string.Empty;
            Collection<string> sheetNames = new Collection<string>();//所有Sheet表名
            using (SpreadsheetDocument spreadDocument = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart workBook = spreadDocument.WorkbookPart;
                Stream stream = workBook.GetStream(FileMode.Open);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(stream);

                XmlNamespaceManager xmlNSManager = new XmlNamespaceManager(xmlDocument.NameTable);
                xmlNSManager.AddNamespace("default", xmlDocument.DocumentElement.NamespaceURI);
                XmlNodeList nodeList = xmlDocument.SelectNodes("//default:sheets/default:sheet", xmlNSManager);

                foreach (XmlNode node in nodeList)
                {
                    sheetName = node.Attributes["name"].Value;
                    sheetNames.Add(sheetName);
                }
            }
            return sheetNames;
        }
       

        #region SaveCell
        private void InsertTextCellValue(Worksheet worksheet, string column, uint row, string value)
        {
            Cell cell = ReturnCell(worksheet, column, row);
            CellValue v = new CellValue();
            v.Text = value;
            cell.AppendChild(v);
            cell.DataType = new EnumValue<CellValues>(CellValues.String);
            worksheet.Save();
        }
        private void InsertNumberCellValue(Worksheet worksheet, string column, uint row, string value)
        {
            Cell cell = ReturnCell(worksheet, column, row);
            CellValue v = new CellValue();
            v.Text = value;
            cell.AppendChild(v);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
            worksheet.Save();
        }
        private static Cell ReturnCell(Worksheet worksheet, string columnName, uint row)
        {
            Row targetRow = ReturnRow(worksheet, row);

            if (targetRow == null)
                return null;

            return targetRow.Elements<Cell>().Where(c =>
               string.Compare(c.CellReference.Value, columnName + row,
               true) == 0).First();
        }
        private static Row ReturnRow(Worksheet worksheet, uint row)
        {
            return worksheet.GetFirstChild<SheetData>().
            Elements<Row>().Where(r => r.RowIndex == row).First();
        }
        #endregion
    }
}
