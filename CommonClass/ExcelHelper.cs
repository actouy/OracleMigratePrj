using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ExcelHelper
    {
        /// <summary>
        /// 删除Excel中一行数据
        /// </summary>
        /// <param name="ExcelPath">文件</param>
        /// <param name="SheetName">工作表</param>
        /// <param name="LineSNumber">行数,从0计数</param>
        /// <returns></returns>
        public static bool DeleteRow(string ExcelPath, string SheetName, int LineSNumber)
        {
            bool isDelete = false;
            try
            {
                Workbook workbook = new Workbook(ExcelPath);
                Worksheet sheet = workbook.Worksheets[SheetName];
                sheet.Cells.DeleteRow(LineSNumber);
                workbook.Save(ExcelPath);
                isDelete = true;
            }
            catch (Exception ex)
            {
                isDelete = false;
            }
            return isDelete;
        }

        public static bool DeleteRows(string ExcelPath, Collection<string> SheetNames, int LineSNumber) 
        {
            bool isDelete = false;
            try
            {
                Workbook workbook = new Workbook(ExcelPath);
                foreach (string SheetName in SheetNames) 
                { 
                    Worksheet sheet = workbook.Worksheets[SheetName];
                    sheet.Cells.DeleteRow(LineSNumber);
                    workbook.Save(ExcelPath);
                }                
                isDelete = true;
            }
            catch (Exception ex)
            {
                isDelete = false;
            }
            return isDelete;
        }
        /// <summary> 
        /// 导出数据到本地 
        /// </summary> 
        /// <param name="dt">要导出的数据</param> 
        /// <param name="tableName">表格标题</param> 
        /// <param name="path">保存路径</param> 
        public static void OutFileToDisk(DataTable dt, string tableName, string path)
        {
            Workbook workbook;
            //if (File.Exists(path))
            //    workbook = new Workbook(path); //工作簿
            //else
                workbook = new Workbook();
            //if(workbook.o)
            //int sheetnumber = workbook.Worksheets.Count
            Worksheet sheet = workbook.Worksheets[0]; //工作表
            sheet.Name = tableName;
            Cells cells = sheet.Cells;//单元格 

            //为标题设置样式     
            Style styleTitle = workbook.CreateStyle();//新增样式 
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleTitle.Font.Name = "宋体";//文字字体 
            styleTitle.Font.Size = 18;//文字大小 
            styleTitle.Font.IsBold = true;//粗体 

            //样式2 
            Style style2 = workbook.CreateStyle();//新增样式 
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style2.Font.Name = "宋体";//文字字体 
            style2.Font.Size = 14;//文字大小 
            style2.Font.IsBold = true;//粗体 
            style2.IsTextWrapped = true;//单元格内容自动换行 
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //样式3 
            Style style3 = workbook.CreateStyle();//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style3.Font.Name = "宋体";//文字字体 
            style3.Font.Size = 12;//文字大小 
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            int Colnum = dt.Columns.Count;//表格列数 
            int Rownum = dt.Rows.Count;//表格行数 

            //生成行1 标题行    
            cells.Merge(0, 0, 1, Colnum);//合并单元格 
            cells[0, 0].PutValue(tableName);//填写内容 
            cells[0, 0].SetStyle(styleTitle);
            cells.SetRowHeight(0, 38);

            //生成行2 列名行 
            for (int i = 0; i < Colnum; i++)
            {
                cells[1, i].PutValue(dt.Columns[i].ColumnName);
                cells[1, i].SetStyle(style2);
                cells.SetRowHeight(1, 25);
            }

            //生成数据行 
            for (int i = 0; i < Rownum; i++)
            {
                for (int k = 0; k < Colnum; k++)
                {
                    cells[2 + i, k].PutValue(dt.Rows[i][k].ToString());
                    cells[2 + i, k].SetStyle(style3);
                }
                cells.SetRowHeight(2 + i, 24);
            }

            workbook.Save(path);
        }


        public MemoryStream OutFileToStream(DataTable dt, string tableName)
        {
            Workbook workbook = new Workbook(); //工作簿 
            Worksheet sheet = workbook.Worksheets[0]; //工作表 
            Cells cells = sheet.Cells;//单元格 

            //为标题设置样式     
            Style styleTitle = workbook.CreateStyle();//新增样式 
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleTitle.Font.Name = "宋体";//文字字体 
            styleTitle.Font.Size = 18;//文字大小 
            styleTitle.Font.IsBold = true;//粗体 

            //样式2 
            Style style2 = workbook.CreateStyle();//新增样式 
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style2.Font.Name = "宋体";//文字字体 
            style2.Font.Size = 14;//文字大小 
            style2.Font.IsBold = true;//粗体 
            style2.IsTextWrapped = true;//单元格内容自动换行 
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //样式3 
            Style style3 = workbook.CreateStyle();//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style3.Font.Name = "宋体";//文字字体 
            style3.Font.Size = 12;//文字大小 
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            int Colnum = dt.Columns.Count;//表格列数 
            int Rownum = dt.Rows.Count;//表格行数 

            //生成行1 标题行    
            cells.Merge(0, 0, 1, Colnum);//合并单元格 
            cells[0, 0].PutValue(tableName);//填写内容 
            cells[0, 0].SetStyle(styleTitle);
            cells.SetRowHeight(0, 38);

            //生成行2 列名行 
            for (int i = 0; i < Colnum; i++)
            {
                cells[1, i].PutValue(dt.Columns[i].ColumnName);
                cells[1, i].SetStyle(style2);
                cells.SetRowHeight(1, 25);
            }

            //生成数据行 
            for (int i = 0; i < Rownum; i++)
            {
                for (int k = 0; k < Colnum; k++)
                {
                    cells[2 + i, k].PutValue(dt.Rows[i][k].ToString());
                    cells[2 + i, k].SetStyle(style3);
                }
                cells.SetRowHeight(2 + i, 24);
            }

            MemoryStream ms = workbook.SaveToStream();
            return ms;
        }
        public static void WriteFormulaToFile(string myvalue, string path)
        {
            Workbook workbook = new Workbook(path);

            //Get the first worksheet in the book.
            Worksheet sheet = workbook.Worksheets[0];

            Cells cells = sheet.Cells;
            //样式3 
            Style style3 = workbook.CreateStyle();//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style3.Font.Name = "宋体";//文字字体 
            style3.Font.Size = 12;//文字大小 
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            cells[1, 0].Formula = myvalue;


            sheet.AutoFitColumns();
            sheet.AutoFitRows();
            //Save the excel file.
            workbook.Save(path);
        }
        public static Collection<string> GetSheetnames(string fullFilename) 
        {
            try
            {
                Collection<string> sheetnames  = new Collection<string>();
                Workbook book = new Workbook(fullFilename);
                
                foreach (Worksheet sheet in book.Worksheets) 
                {
                    sheetnames.Add(sheet.Name);
                }
                return sheetnames;
            }
            catch (Exception ex) 
            {
                LogHelper.WriteLog(ex);
                return null;
            }

        }
        public static DataTable ExcelToDataTable(string fullFilename)//导入
        {
            try
            {
                Workbook book = new Workbook(fullFilename);
                book.CalculateFormula();//先进行公式运算，得到运算的结果
                //book.o(fullFilename);
                Worksheet sheet = book.Worksheets[0];
                Cells cells = sheet.Cells;
                //获取excel中的数据保存到一个datatable中
                DataTable dt_Import = cells.ExportDataTable(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
                return dt_Import;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return null;
            }
            //string myvalue = cells.FirstCell.StringValue;
            // dt_Import.
            
        }
        public static DataTable ExcelToDataTable(string fullFilename, string sheetname)//导入
        {
            try
            {
                Workbook book = new Workbook(fullFilename);
                //book.o(fullFilename);
                Worksheet sheet = book.Worksheets[sheetname];
                Cells cells = sheet.Cells;
                //获取excel中的数据保存到一个datatable中
                DataTable dt_Import = cells.ExportDataTable(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);

                // dt_Import.
                return dt_Import;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return null;
            }
        }

        public static void DataTableToExcel(string fullFilename, string sheetname, DataTable dt)
        {
            try
            {
                Workbook book = new Workbook(fullFilename);
                //book.o(fullFilename);
                Worksheet sheet = book.Worksheets[sheetname];
                Cells cells = sheet.Cells;
                cells.ImportDataTable(dt, true, 0, 0, false);
                sheet.AutoFitColumns();
                sheet.AutoFitRows();
                book.Save(fullFilename);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
            }
        }
    }
}
