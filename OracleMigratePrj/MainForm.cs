using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using AppClass;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections;
using Model;
using Common;
using System.IO;
using Microsoft.Win32;
using System.Threading;


namespace OracleMigratePrj
{

    public partial class MainForm : OfficeForm
    {
        public MainForm()
        {
            InitializeComponent();            //GetDBName();
            
        }
        //DBConfig mycon = new DBConfig("10.0.10.3", "NBIGDATA", "BIGDATAUSER", "bigdatauser");
        DBConfig mycon = new DBConfig("61.159.180.163", "NBIGDATA", "BIGDATAUSER", "bigdatauser");
        //DBConfig mycon = new DBConfig("61.159.180.163", "oddatabase", "appuser", "appuser");
        private void GetDBName()
        {
            //DataTable mytable = TableToOracle.GetTablesName();
            cmbxIMDBName.DataSource = TableToOracle.GetTablesName();
            cmbxIMDBName.DisplayMember = "table_name";
        }


        private void btnBrowsSYS_Click_1(object sender, EventArgs e)
        {
            //FolderBrowserDialog myfolderdialog = new FolderBrowserDialog();
            //myfolderdialog.RootFolder = Environment.SpecialFolder.MyComputer;
            //if (myfolderdialog.ShowDialog() == DialogResult.OK)
            //{
            //    txbSysPath.Text = myfolderdialog.SelectedPath + @"\";
            //}
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "选择一个dmp数据文件",
                Filter = "*.dmp(*.dmp)|*.*"
            };

            dialog.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txbDataFile.Text = dialog.FileName;
            }
        
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            DBConfig mydb = new DBConfig();
            mydb = DBConOP.readConfig();
            //DBDmpOp.OracleImp(mydb, txbDataFile.Text);
            textBoxX1.Clear();
            //textBoxX1.Text = DBDmpOp.RunCmd("ping 172.18.101.200 ");
            textBoxX1.Text = DBDmpOp.OracleImp(mydb, txbDataFile.Text);
            //DBDmpOp.OracleImp(mydb, txbDataFile.Text);
            
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            DBConfig mydb = new DBConfig();
            mydb.DBServerIP = txbDBServerIP.Text;
            mydb.OracleSID = txbOracleSID.Text;
            mydb.DBUserName = txbDBUserName.Text;
            mydb.DBPassword = txbDBServerIP.Text;
            mycon = mydb;
            if(DBConOP.saveConfig(mydb))
                MessageBox.Show("成功保存数据库配置~！");
        }

        private void btnXClearAll_Click(object sender, EventArgs e)
        {
            ClearAllText(Controls);
        }
        private void ClearAllText(Control.ControlCollection myCons)
        {
            foreach (Control ctr in myCons)
            {
                if (ctr is TextBox)//考虑是文本框的话
                {
                    ((TextBox)ctr).Text = String.Empty;
                }
            }
        }

        private static string filePath = "";
        Collection<string> myworkbook = new Collection<string>();
        private void btnXBrowesExcel_Click(object sender, EventArgs e)
        {
            //OpenFileDialog myExlOpenDialog = new OpenFileDialog();
            //myExlOpenDialog.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
            //myExlOpenDialog.Filter = "Excel 2010 files (*.xlsx)|*.xlsx|Excel97-03 files (*.xls)|*.xls|All files (*.*)|*.*";
            //myExlOpenDialog.FilterIndex = 1;
            //myExlOpenDialog.RestoreDirectory = true;
            //if (myExlOpenDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    exlpathtextBox.Text = myExlOpenDialog.FileName;
            //    filePath = myExlOpenDialog.FileName;
            //    Collection<string> mysheetnames = ExcelOper.GetExcelSheetNames(filePath);
            //    listBoxAdv1.DataSource = mysheetnames;
            //}
            FolderBrowserDialog myfolderdialog = new FolderBrowserDialog();
            myfolderdialog.SelectedPath = @"E:\精准扶贫\数据";
            if (myfolderdialog.ShowDialog() == DialogResult.OK)
            {
                exlpathtextBox.Text = myfolderdialog.SelectedPath;
                myworkbook = IOHelper.GetFilePath(exlpathtextBox.Text, "*.xls");
                listBoxAdv1.DataSource = myworkbook;
            }
        }
        DataTable mytable = new DataTable();
        private void listBoxAdv1_ItemClick(object sender, EventArgs e)
        {

            filePath = sender.ToString();

            //Collection<string> mysheetnames = ExcelOper.GetExcelSheetNames(filePath);
            //Collection<string> mysheetnames = ExcelBll.GetSheetNames(filePath);
            //Collection<string> mysheetnames = EPPExcelHelper.GetExcelSheetNames(filePath);
            Collection<string> mysheetnames = ExcelHelper.GetSheetnames(filePath);
            listBoxAdv3.DataSource = mysheetnames;


        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            DataTable temptable = TableToOracle.GetFieldsName(comboBoxEx1.Text);
            listBoxAdv2.DataSource = temptable;
            listBoxAdv2.ValueMember = "column_name";
            listBoxAdv2.DisplayMember = "column_name";
        }

        private void ToOrclbtnX2_Click(object sender, EventArgs e)
        {
            //foreach (string mpath in myworkbook)
            //{
            //    mytable = ExcelBll.GetSheet("家庭成员信息", mpath).Tables[0];
           // DBConfig mycon = new DBConfig("61.159.180.163", "NBIGDATA", "BIGDATAUSER", "bigdatauser");
                string ID = listBoxAdv2.SelectedValue.ToString();

                //if (TableToOracle.InsertData(mytable, comboBoxEx1.Text, ID, idValuetxtB.Text, myhashtable))
                if (TableToOracle.InsertIntoOrcl(mycon, comboBoxEx1.Text, mytable, ID, idValuetxtB.Text))
                {
                    MessageBox.Show("添加数据成功~！");
                }
                else
                {

                    MessageBox.Show(LogHelper.ErrorMessage);
                }
            //}
        }

        private void buttonXGetTableName_Click(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("61.159.180.163", "NBIGDATA", "BIGDATAUSER", "bigdatauser");
            comboBoxEx1.DataSource = TableToOracle.GetTablesName(mycon);
            comboBoxEx1.DisplayMember = "table_name";
        }

        private void buttonX5_Click(object sender, EventArgs e)
        {
            DataRowView row = null;
            if (listBoxAdv2.SelectedItem != null)
            {
                row = (DataRowView)listBoxAdv2.SelectedItem;
                string ID = row.Row.ItemArray[0].ToString();
                mytable = ExcelBll.GetRepeatFields(listBoxAdv3.SelectedValue.ToString(), filePath, ID);
            }


            if (mytable.Rows.Count > 0)
            {
                System.IO.FileInfo myfilepath = new System.IO.FileInfo(filePath);

                string myfile = myfilepath.Name.Replace(".xlsx", ".xml");
                ExcelOper.DataTableToXML(mytable, myfile);
                dataGridViewX1.DataSource = mytable;
            }
            else
            {
                MessageBox.Show("共享无重复~！");
            }

        }

        private void listBoxAdv3_ItemClick(object sender, EventArgs e)
        {


            DateTime mytime = DateTime.Now;
            //mytable = ExcelOper.ExcelToDataTable(this.listBoxAdv3.SelectedValue.ToString(), filePath);
            mytable = ExcelBll.GetSheet(listBoxAdv3.SelectedValue.ToString(), filePath).Tables[0];
            //mytable = EPPExcelHelper.Import(filePath, this.listBoxAdv3.SelectedValue.ToString());
            //mytable = ExcelOp.ReadExcel(this.listBoxAdv3.SelectedValue.ToString(), filePath,1);
            //mytable = ExcelHelper.ExcelToDataTable(filePath, this.listBoxAdv3.SelectedValue.ToString());
            if (mytable.Rows.Count > 0)
            {
                ToOrclbtnX2.Enabled = true;
                btnXUpdate.Enabled = true;
                dataGridViewX1.DataSource = mytable;
            }
            else
            {
                dataGridViewX1.DataSource = null;
            }
            TimeSpan ts = DateTime.Now - mytime;
            labelX3.Text = ts.ToString();
        }

        private void btnXUpdate_Click(object sender, EventArgs e)
        {
            DataRowView row = (DataRowView)listBoxAdv2.SelectedItem;
            string ID = row.Row.ItemArray[0].ToString();

            if (TableToOracle.UpdateData(mytable, comboBoxEx1.Text, ID, idValuetxtB.Text))
            {
                MessageBox.Show("添加数据成功~！");
            }
            else
            {

                MessageBox.Show(LogHelper.ErrorMessage);
            }
        }

        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("61.159.180.163", "NBIGDATA", "BIGDATAUSER", "bigdatauser");
            if (comboBoxEx1.Text == null) 
                return;
            DataTable temptable = TableToOracle.GetFieldsProperty(comboBoxEx1.Text, mycon);
            listBoxAdv2.DataSource = temptable;
            listBoxAdv2.ValueMember = "column_name";
            listBoxAdv2.DisplayMember = "column_name";
        }

        private void buttonX3_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog myfolderdialog = new FolderBrowserDialog();
            myfolderdialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (myfolderdialog.ShowDialog() == DialogResult.OK)
            {
                textBoxXSavePath.Text = myfolderdialog.SelectedPath + @"\";
            }
        }

        private void buttonXExpAll_Click(object sender, EventArgs e)
        {
            DBConfig mydb = new DBConfig();
            mydb = DBConOP.readConfig();
            textBoxXExpInfo.Clear();
            textBoxXExpInfo.Visible = true;
            dataGridViewX2.Visible = false;
            if (textBoxXSavePath.Text != "") 
            { 
                textBoxXExpInfo.Text = DBDmpOp.OracleExp(mydb, textBoxXSavePath.Text);
            }
            else 
            {
                MessageBox.Show("请选择保存数据的目录");
                textBoxXSavePath.Focus();
            }            
        }

        private void buttonXExpTableDmp_Click(object sender, EventArgs e)
        {
            DBConfig mydb = new DBConfig();
            mydb = DBConOP.readConfig();
            textBoxXExpInfo.Clear();
            textBoxXExpInfo.Visible = true;
            dataGridViewX2.Visible = false;
            if (comboBoxExpTables.SelectedItem!=null)
            {
                textBoxXExpInfo.Text = DBDmpOp.OracleExp(mydb, textBoxXSavePath.Text, comboBoxExpTables.Text);
            }
            else 
            {
                MessageBox.Show("请选择一个导出数据的表~！");
                comboBoxExpTables.Focus();
            }
        }

        private void buttonXViewTable_Click(object sender, EventArgs e)
        {
            textBoxXExpInfo.Visible = false;
            dataGridViewX2.Visible = true;
            DataTable orcltable = new DataTable();
            if (comboBoxExpTables.SelectedItem != null)
            {
                orcltable = TableToOracle.GetData(mycon, comboBoxExpTables.Text);
            }
            else
            {
                MessageBox.Show("请选择一个表预览数据~！");
                comboBoxExpTables.Focus();
                return;
            }
            
            if (orcltable.Rows.Count>0)
            {
                dataGridViewX2.DataSource = orcltable;
                dataGridViewX2.Refresh();
            }
            else
            {
                dataGridViewX2.DataSource = null;
            }

        }

        private void buttonXGetTName_Click(object sender, EventArgs e)
        {
            comboBoxExpTables.DataSource = TableToOracle.GetTablesName(mycon);
            comboBoxExpTables.DisplayMember = "table_name";
        }

        private void buttonXExpTableXlsx_Click(object sender, EventArgs e)
        {
            textBoxXExpInfo.Clear();
            textBoxXExpInfo.Visible = true;
            dataGridViewX2.Visible = false;
            if (comboBoxExpTables.SelectedItem != null)
            {
                DataTable orcltable = new DataTable();
                string tablename = comboBoxExpTables.Text;
                orcltable = TableToOracle.GetData(mycon, tablename);
                if (orcltable.Rows.Count > 0)
                {
                    ExcelHelper.OutFileToDisk(orcltable, comboBoxExpTables.Text, textBoxXSavePath.Text + tablename + ".xlsx");
                }
                else 
                { 
                    MessageBox.Show("表中没有数据！"); 
                }
                //textBoxXExpInfo.Text = DBDmpOp.OracleExp(mydb, textBoxXSavePath.Text, comboBoxExpTables.Text);
            }
            else
            {
                MessageBox.Show("请选择一个导出数据的表~！");
                comboBoxExpTables.Focus();
            }
        }
        Hashtable myhashtable = new Hashtable();
        private void buttonXMapField_Click(object sender, EventArgs e)
        {
            DataTable maptable = TableToOracle.GetMapFd(comboBoxEx1.Text);
            foreach (DataRow myrow in maptable.Rows) 
            {
                myhashtable.Add(myrow[1].ToString(), myrow[0].ToString());
            }
        }

        private void buttonX9_Click(object sender, EventArgs e)
        {
            cmbxIMDBName.DataSource = TableToOracle.GetSynonyms("BJ_FPY");
            cmbxIMDBName.DisplayMember = "table_name";
        }

        private void btmGetID_Click(object sender, EventArgs e)
        {

        }

        private void buttonX1_Click_1(object sender, EventArgs e)
        {
            GetLngLatForm mygetform = new GetLngLatForm();
            mygetform.Show();
        }

        private void btnXBrowesCSV_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog myfolderdialog = new FolderBrowserDialog();
            myfolderdialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (myfolderdialog.ShowDialog() == DialogResult.OK)
            {
                textBoxXCsvFiles.Text = myfolderdialog.SelectedPath;
                myworkbook = IOHelper.GetFilePath(textBoxXCsvFiles.Text, "*.csv");
                listBoxAdvCsvFile.DataSource = myworkbook;
            }
        }

        private void listBoxAdvCsvFile_ItemClick(object sender, EventArgs e)
        {
            filePath = sender.ToString();

            //CsvStreamReader mycsv = new CsvStreamReader(filePath);

            //DataTable mytable = CsvHelper.OpenCSV(filePath);
            //dataGridViewXCsv.DataSource = mytable;
            DataSet meset = CsvHelper.getCsv(@"E:\精准扶贫\数据\20170328\毕节市贫困户信息", @"毕节市家庭成员信息.csv");
        }

        private void comboBoxEx2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEx1.Text == null)
                return;
            DataTable temptable = TableToOracle.GetFieldsProperty(comboBoxEx1.Text, mycon);
            listBoxAdvFields.DataSource = temptable;
            listBoxAdvFields.ValueMember = "column_name";
            listBoxAdvFields.DisplayMember = "column_name";
        }

        private void buttonX4_Click(object sender, EventArgs e)
        {
            comboBoxExCsvTableName.DataSource = TableToOracle.GetTablesName(mycon);
            comboBoxExCsvTableName.DisplayMember = "table_name";
        }

        private void buttonX7_Click(object sender, EventArgs e)
        {
            UpdateForm mygetform = new UpdateForm();
            mygetform.Show();
        }

        private void buttonX8_Click(object sender, EventArgs e)
        {
            string filepath = @"C:\app\client\OUY\product\12.1.0\client_1\Network\Admin\tnsnames.ora";
            string[] mydb;
            if (!File.Exists(filepath)) 
            {

            }
                mydb = GetOracleTnsNames(filepath);

        }
        private string GetTNSFile() 
        {
            OpenFileDialog mydlg = new OpenFileDialog();
            mydlg.FileName = "tnsnames.ora";
            string filepath = "";
            return filepath;
        }
        public static string[] GetDatabases()
        {
            #region 读取TNS文件

            string output = "";
            string fileLine;
            Stack parens = new Stack();

            // open tnsnames.ora   
            StreamReader sr;
            try
            {
                sr = new StreamReader(@"C:\app\client\OUY\product\12.1.0\client_1\Network\Admin\tnsnames.ora");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw ex;
            }
            #endregion

            // Read the first line of the file 读取文件的第一行  
            fileLine = sr.ReadLine();

            #region
            // loop through, reading each line of the file 循环，读取每一行  
            while (fileLine != null)
            {
                // if the first non whitespace character is a #, ignore the line   
                // and Go to the next line in the file 如行的第一个字符为“#”忽略这一行。直接读下一行。  
                if (fileLine.Length > 0 && fileLine.Trim().Substring(0, 1) != "#")
                {
                    // Read through the input line character by character   
                    char lineChar;
                    for (int i = 0; i < fileLine.Length; i++)
                    {
                        lineChar = fileLine[i];


                        if (lineChar == '(')
                        {
                            // if the char is a ( push it onto the stack //如果第一个字符是 "(" 整行放入 堆栈。  
                            parens.Push(lineChar);
                        }
                        else if (lineChar == ')')
                        {
                            // if the char is a ), pop the stack 如果字符是")",一个一个移出  （注：POP可在 Stack 的顶部移除一个元素）  
                            parens.Pop();
                        }
                        else
                        {
                            // if there is nothing in the stack, add the character to the output  

                            if (parens.Count == 0)
                            {
                                output += lineChar;
                            }
                        }
                    }
                }


                // Read the next line of the file   
                fileLine = sr.ReadLine();
            }

            // Close the stream reader   
            sr.Close();

            #endregion


            #region 处理=号

            // Split the output string into a string[]   
            string[] split = output.Split('=');


            // trim each string in the array 以"="号为分隔符。截掉，放入split内  
            for (int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Trim();
            }


            Array.Sort(split);


            return split;

            #endregion
        }

        public static string[] GetOracleTnsNames(string file)
        {
            try
            {
                // 查询注册表，获取oracle服务文件路径
                //RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Oracle");
                //RegistryKey mykey = Registry.LocalMachine.OpenSubKey("SOFTWARE");

                //RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE").OpenSubKey("Oracle");

                
                //string home = (string)key.GetValue("TNS_ADMIN");
                //string file = home + @"\tnsnames.ora";
                // 解析文件
                string line;
                ArrayList arr = new ArrayList();
                StreamReader sr = new StreamReader(file);
                arr.Add("--选择数据库--");
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line != "")
                    {
                        char c = line[0];
                        if (c >= 'A' && c <= 'z')
                            arr.Add(line.Substring(0, line.IndexOf(' ')));
                    }
                }
                sr.Close();
                // 返回字符串数组
                return (string[])arr.ToArray(typeof(string));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void buttonX7_Click_1(object sender, EventArgs e)
        {
            UpdateForm myform = new UpdateForm();
            myform.Show();
        }

        private void buttonX1_Click_2(object sender, EventArgs e)
        {

        }
        void getpictask() 
        {

                string filename = "savefile";
                while(true)
                {
	                Hashtable myhash = JsonTools.readhash(filename);
	                if (myhash.Count > 0)
	                {
	                    foreach (DictionaryEntry mydic in myhash)
	                    {
	                        int pageno = Convert.ToInt32(mydic.Key.ToString());
	                        int startno = Convert.ToInt32(mydic.Value.ToString());
	                        JsonTools.getobj(pageno, startno, mycon);
	                    }
	                }
	                else
	                	JsonTools.getobj(1, 0, mycon);
                }
            
        }

        void pgetpictask()
        {
            Parallel.For(1,11,(int i)=>{
            string filename = "savefile" + i;
            Hashtable myhash = JsonTools.readhash(filename);
            if (myhash.Count > 0)
            {
                foreach (DictionaryEntry mydic in myhash)
                {
                    int pageno = Convert.ToInt32(mydic.Key.ToString());
                    int startno = Convert.ToInt32(mydic.Value.ToString());
                    JsonTools.pgetobj(pageno, startno, mycon);
                }
            }
            else
                JsonTools.pgetobj(1, 0, mycon);
            });

        }


        private void buttonX10_Click(object sender, EventArgs e)
        {
        	getpictask();
            
        }
        System.Timers.Timer pTimer = new  System.Timers.Timer(5000);//每隔5秒执行一次，没用winfrom自带的
        private void MainForm_Load(object sender, EventArgs e)
        {
            
            pTimer.Elapsed += pTimer_Elapsed;//委托，要执行的方法
            pTimer.AutoReset = true;//获取该定时器自动执行            
            Control.CheckForIllegalCrossThreadCalls = false;//这个不太懂，有待研究
        }

        private void pTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。  
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            int iHour = 0;
            int iMinute = 30;
            int iSecond = 00;
            Task mytask = new Task(pgetpictask);
            if (intHour == iHour & intMinute == iMinute && intSecond == iSecond)
            {                
                mytask.Start();
            }

        }

        private void comboBoxExDataBase_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonX13_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable mytable = new DataTable();
                DataRow myrow = mytable.Rows[1];

            }catch(Exception ex)
            {
                LogHelper.WriteLog(mycon, ex);
            }
        }

        private void buttonXAutoGet_Click(object sender, EventArgs e)
        {
            pTimer.Enabled = true;
        }

        private void buttonXSelectExcelPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog myfolderdialog = new FolderBrowserDialog();
            myfolderdialog.SelectedPath = @"E:\精准扶贫\数据";
            if (myfolderdialog.ShowDialog() == DialogResult.OK)
            {
                textBoxXExcelPath.Text = myfolderdialog.SelectedPath;
                myworkbook = IOHelper.GetFilePath(textBoxXExcelPath.Text, "*.xls");
                listBoxAdvListExcelFile.DataSource = myworkbook;
            }
        }

        private void listBoxAdvListExcelFile_ItemClick(object sender, EventArgs e)
        {
            filePath = sender.ToString();

            //Collection<string> mysheetnames = ExcelOper.GetExcelSheetNames(filePath);
            //Collection<string> mysheetnames = ExcelBll.GetSheetNames(filePath);
            //Collection<string> mysheetnames = EPPExcelHelper.GetExcelSheetNames(filePath);
            Collection<string> mysheetnames = ExcelHelper.GetSheetnames(filePath);
            listBoxAdvExcelSheets.DataSource = mysheetnames;
        }

        private void listBoxAdvExcelSheets_MouseClick(object sender, MouseEventArgs e)
        {
            
            //mytable = ExcelOper.ExcelToDataTable(this.listBoxAdv3.SelectedValue.ToString(), filePath);
            //mytable = ExcelBll.GetSheet(sender.ToString(), filePath).Tables[0];
            //mytable = EPPExcelHelper.Import(filePath, this.listBoxAdv3.SelectedValue.ToString());
            //mytable = ExcelOp.ReadExcel(this.listBoxAdv3.SelectedValue.ToString(), filePath,1);
            mytable = ExcelHelper.ExcelToDataTable(filePath, sender.ToString());
            if (mytable.Rows.Count > 0)
            {
                dataGridViewXExcelSheet.DataSource = mytable;
                buttonXQueryData.Enabled = true;
            }
            else
            {
                dataGridViewXExcelSheet.DataSource = null;
            }
            
        }

        private void buttonXQueryData_Click(object sender, EventArgs e)
        {
            foreach (DataRow myrow in mytable.Rows)
            {
                string mycardid = myrow["身份证号"].ToString();
                string qrstr = @"SELECT   HHID,HD_HHNAME,HOUSE_DIC.CARDID,VID,RD_FULLNAME FROM  PEOPLE_INFO,HOUSE_DIC,REGION_DICT WHERE  (PEOPLE_INFO.CARDID LIKE '" + mycardid + "%') AND (HHID = HD_ID) AND (RD_ID = VID)";
                DataTable qrtable = TableToOracle.QueryTable(qrstr, mycon);
                if ((qrtable != null) && (qrtable.Rows.Count > 0))
                {
                    DataRow qrow = qrtable.Rows[0];
                    myrow["户编号"] = qrow["HHID"];
                    myrow["户主姓名"] = qrow["HD_HHNAME"];
                    myrow["户主身份证"] = qrow["CARDID"];
                    myrow["地址编号"] = qrow["VID"];
                    myrow["地址"] = qrow["RD_FULLNAME"];
                }
                else
                {
                    myrow["户编号"] = "数据库中查无此人";
                }
            }
            ExcelHelper.OutFileToDisk(mytable, "验证数据", textBoxXExcelPath.Text + "outfile" + ".xlsx");
        }
    }
}
