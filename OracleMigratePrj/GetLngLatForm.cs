using AppClass;
using Common;
using DevComponents.DotNetBar;
using Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace OracleMigratePrj
{
    public partial class GetLngLatForm :  OfficeForm
    {
        public GetLngLatForm()
        {
            InitializeComponent();
        }
        DBConfig mycon = new DBConfig("61.159.180.163", "NBIGDATA", "BIGDATAUSER", "bigdatauser");
        //DBConfig mycon = new DBConfig("61.159.180.163", "0BIGDATA", "BIGDATAUSER", "bigdatauser");
        private void buttonX1_Click(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            this.comboBoxEx1.DataSource = TableToOracle.GetData(mycon,"REGION_DICT","RD_ID","52");
            comboBoxEx1.ValueMember = "RD_ID";
            comboBoxEx1.DisplayMember = "RD_FULLNAME";
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {

        }
        public double GetRandomNumber(double minimum, double maximum, int Len)   //Len小数点保留位数
        {
            Random random = new Random();
            return Math.Round(random.NextDouble() * maximum + minimum, Len);
        }
        #region  获取数据库
        private void buttonX3_Click(object sender, EventArgs e)
        {
            DataTable mytable = TableToOracle.GetSynonyms("BJ_FPY");
            foreach (DataRow myrow in mytable.Rows) 
            {
                string tablename = myrow["table_name"].ToString();
                DataTable fieldtable = TableToOracle.GetFieldsProperty(tablename, "FPY");

                //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
                //DataTable fieldtable = TableToOracle.GetFieldsProperty(tablename, mycon);
                if (fieldtable.Rows.Count > 0)
                {
                    ExcelHelper.OutFileToDisk(fieldtable, tablename, textBoxXSavePath.Text + DateTime.Today.ToString("yyyyMMdd") +tablename + ".xlsx");

                }
                else
                {
                    MessageBox.Show("表中没有对应的字段！");
                    
                }
            }
            MessageBox.Show("导出对应字段结束！");
        }
       

        private void buttonX5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog myfolderdialog = new FolderBrowserDialog();
            myfolderdialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (myfolderdialog.ShowDialog() == DialogResult.OK)
            {
                textBoxXSavePath.Text = myfolderdialog.SelectedPath + @"\";
            }
        } 
        #endregion

        private void buttonX4_Click(object sender, EventArgs e)
        {
            DataTable mytable = TableToOracle.GetSynonyms("BJ_FPY");
            int havedatanum = 0;
            //DBConfig mycon = new DBConfig("61.159.180.163", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            foreach (DataRow myrow in mytable.Rows)
            {
                string tablename = myrow["table_name"].ToString();
                if (havedatanum < 8)
                {
                    havedatanum++;
                    continue;
                }
                DataTable fieldtable = TableToOracle.GetFieldsProperty(tablename, "FPY");
                //TableToOracle.DropTable(tablename, mycon);
                if (fieldtable.Rows.Count > 0)
                {
                    string mysql = TableToOracle.GetOrclStr(fieldtable);
                    int myint = TableToOracle.CreateTable(mysql, mycon);
                }
                else
                {
                    MessageBox.Show("表中没有对应的字段！");
                }
            }
        }

        private void buttonX6_Click(object sender, EventArgs e)
        {
            DataTable mytable = TableToOracle.GetSynonyms("BJ_FPY");
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string msg = "";
            //int havedatanum = 0;
            foreach (DataRow myrow in mytable.Rows)
            {
                string tablename = myrow["table_name"].ToString();
                //if (havedatanum<19)
                //{
                //    havedatanum++;
                //    continue;
                //}
                int rowscount = TableToOracle.CountData(tablename, mycon);
                if (rowscount > 0)  //根据数据量分表，每个表的数据行不超过10万行
                {
                    //DataTable dt = TableToOracle.GetAllData(tablename);
                    //if (dt.Rows.Count > 0)
                    //{
                    //    dt.TableName = tablename;
                    //    if (TableToOracle.OracleBulkCopy(dt, mycon))
                    //    {
                    //        msg += tablename + "添加数据成功~！\n\r";//MessageBox.Show("添加数据成功~！");
                    //    }
                    //    else
                    //    {

                    //        msg += tablename + "添加数据失败~！\n\r";
                    //    }
                    //}
                }
                else 
                {
                    //DataSet myset = TableToOracle.GetDataSet(tablename, rowscount);
                    //if (myset.Tables.Count > 0)
                    //{
                    //    myset.DataSetName = tablename;
                    //    if (TableToOracle.OracleBulkCopy(myset, mycon))
                    //    {
                    //        msg += tablename + "添加数据成功~！\n\r";//MessageBox.Show("添加数据成功~！");
                    //    }
                    //    else
                    //    {

                    //        msg += tablename + "添加数据失败~！\n\r";
                    //    }
                    //}
                    rowscount = TableToOracle.CountData(tablename);
                    if (rowscount > 0) 
                    {
                        int i;
                        DataTable dt = TableToOracle.QueryTable("Select * from FPY.CFG_BASIC_AREA_BIJIE where area_level=3");
                        for (i = 0; i < dt.Rows.Count; i++)
                        {

                            string townid = dt.Rows[i]["OID"].ToString();
                            string townname = dt.Rows[i]["SHORT_NAME"].ToString();
                            DataTable towntable = TableToOracle.QueryTable("Select * from FPY." + tablename + " where AZC004='" + townid + "'");
                            int towndatanumber = towntable.Rows.Count;
                            if (towndatanumber > 0)
                            {
                                towntable.TableName = tablename;
                                if (TableToOracle.OracleBulkCopy(towntable, mycon))
                                {
                                    msg += tablename + " 镇名" + townname + " 添加 " + towndatanumber + " 条数据成功~！\n\r";//MessageBox.Show("添加数据成功~！");
                                }
                                else
                                {

                                    msg += tablename + " 镇名" + townname + "添加数据失败~！\n\r";
                                }
                            }
                        }
                    }
                    else 
                    {
                        msg += tablename + "原始表无数据~！\n\r";
                    }
                    
                    
                }
                //LogHelper.Log(msg);
                //msg = "";
            }

            MessageBox.Show(msg);
        }

        DataTable rdidtable = new DataTable();
        DataTable zerotable = new DataTable();
        private void btnUPFMName_Click(object sender, EventArgs e)
        {
            //this.cmbxEMDBName.DataSource = TableToOracle.GetTablesName();
            //cmbxEMDBName.DisplayMember = "table_name";
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            rdidtable = TableToOracle.GetVillTb(mycon);
            DataTable mynotable = new DataTable();
            DataTable ownertable = new DataTable();
            int yzmax;
            for (int i = 3404; i < rdidtable.Rows.Count; i++)
            {
                string tempVilID = rdidtable.Rows[i]["RD_ID"].ToString();
                zerotable = TableToOracle.QueryZeroNoFromFPY(mycon,tempVilID);

                if (zerotable.Rows.Count > 0)
                {
                    mynotable = TableToOracle.QueryNoTb(mycon,tempVilID);
                    ownertable = TableToOracle.QueryOwnerTb(mycon, tempVilID);
                    ownertable.PrimaryKey = new DataColumn[] {ownertable.Columns[0]};
                    DataTable pbtable = TableToOracle.GetHHIDFromPB(mycon, tempVilID);
                    //pbtable.PrimaryKey = new DataColumn[] { pbtable.Columns[1] };
                    int[] idarr = new Int32[5000];
                    //建立村的户表
                    for (int j = 0; j < mynotable.Rows.Count; j++)
                    {
                        idarr[Int32.Parse(mynotable.Rows[j][0].ToString())] = Int32.Parse(mynotable.Rows[j][0].ToString());
                    }
                    //遍历村表
                    yzmax = 0;
                    for (int j = 0; j < zerotable.Rows.Count; j++)
                    {
                        string cardid = zerotable.Rows[j]["CARDID"].ToString();
                        string hguid = zerotable.Rows[j]["HGID"].ToString();
                        //检验在户表中有没有编号，如果有，则取出编号，检查与idarr中是否冲突，则修改
                        //否则自定义编号开始，在IDARR找未用的编号

                        

                        bool conflict = true;
                        int hbh = 0;
                        DataRow[] myrow = pbtable.Select("cardid='" + cardid + "'");
                        if (myrow.Length>0)
                        {
                            string pbno = myrow[0]["HHID"].ToString(); ;
                            for(int k=1;k<myrow.Length;k++)
                            {   
                                if (ownertable.Rows.Contains(pbno))
                                     break;
                                pbno = myrow[k]["HHID"].ToString();
                            }
                            int tempint = Convert.ToInt32(pbno);
                            if (idarr[tempint] == 0)
                            {
                                idarr[tempint] = tempint;
                                hbh = tempint;
                                conflict = false;
                            }
                        }
                        if (conflict)
                        {
                            while (true)
                            {
                                for (int k = yzmax + 1; k < 5000; k++)
                                    if (idarr[k] == 0) { hbh = yzmax = k; break; }
                                //检查ID在图片是否出在，标记
                                string tempid = tempVilID + hbh.ToString().PadLeft(4, '0'); ;
                                //if (!TableToOracle.DBExist(mycon,tempid))
                                //    break;
                                //string expression = "SHHID =" + hbh;
                                if (!ownertable.Rows.Contains(hbh))
                                    break;
                                //如果不存在，确认编号，退出
                            }
                        }
                        //写入数据库处理
                        string hhid = tempVilID + hbh.ToString().PadLeft(4, '0');
                        Int64 hhno = Convert.ToInt64(hhid);
                        string sql1 = @"Update HOUSE_BASE_INFO_FPY set HHID=" + hhno +
                            "  where HGID = '" + hguid +"'";

                        int reslt = TableToOracle.ExecuteSql(sql1,mycon);

                    }
                }

            }

            string sql2 = @"Truncate table HOUSE_DIC";
            TableToOracle.ExecuteSql(sql2, mycon);
            string sql3 = @"insert into HOUSE_DIC(HD_ID, HD_AREAID, HD_HHNAME, HD_GROUP, HGID)
select HHID,VID,HHNAME,VGROUP,HGID
FROM      HOUSE_BASE_INFO_FPY";
            TableToOracle.ExecuteSql(sql3, mycon);
            TableToOracle.CloseCon();
          }

        private void buttonX7_Click(object sender, EventArgs e)
        {

        }

        private void buttonXUpdatePeople_Click(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            rdidtable = TableToOracle.GetRDIDTbByType(mycon,"3");
            //string ttable = "PEOPLE_INFO_BJ";
            //string stable = "HOUSE_BASE_INFO_BJ";
            //string sfields = "VID,HHID";
            //string tfields = "VID,HHID";
            //string skeyfield = "HGID";
            //string tkeyfield = "HGID";
            //string swhere = " HHID is not null";
            //string twhere = " HHID is null";
            //Collection<string> s1 = new Collection<string>(sfields.Split(','));
            //Collection<string> s2 = new Collection<string>(tfields.Split(','));
            //bool rslt = TableToOracle.UpdateFromOrcl(mycon, stable, s1, skeyfield, swhere, ttable, s2, tkeyfield, twhere, "VID");
            //if (rslt)
            //{

            //    textBoxXOutPutrst.Text += "更新数据成功!\n\r";
            //}
            //else
            //{
            //    textBoxXOutPutrst.Text += "更新数据失败!\n\r";
            //}

            

            DataAdapter myda = new DataAdapter();
            int yzmax;
            string msg = "";
            for (int i = 0; i < rdidtable.Rows.Count; i++)
            {
                string tempID = rdidtable.Rows[i]["RD_ID"].ToString();
                myda = TableToOracle.QueryNULLPID(mycon, "PEOPLE_INFO_BJ", "VID", tempID);
                zerotable = myda.Mytable;

                if (zerotable.Rows.Count > 0)
                {
                    for (int j = 0; j < zerotable.Rows.Count; j++)//针对每一户进行操作
                    {
                        string mypid = zerotable.Rows[j]["PID"].ToString();
                        if (mypid != "")                            
                            continue;
                        yzmax = 0;
                        int pbno = 0;
                        string hhid = zerotable.Rows[j]["HHID"].ToString();
                        string hguid = zerotable.Rows[j]["HGID"].ToString();
                        DataRow[] rowarry = zerotable.Select("HGID='" + hguid + "' and PID is null");
                        if (rowarry.Length > 0) 
                        {
                            for (int k = 0; k < rowarry.Length; k++)
                            {

                                string myrl = rowarry[k]["RELATION"].ToString();
                                string pbid = "";
                                switch (myrl) 
                                {
                                    case "户主":
                                        pbno = 1;
                                        pbid = hhid + pbno.ToString().PadLeft(2, '0');
                                        rowarry[k]["PID"] = pbid;
                                        break;
                                    case "配偶":
                                        pbno = 2;
                                        pbid = hhid + pbno.ToString().PadLeft(2, '0');
                                        rowarry[k]["PID"] = pbid;
                                        break;
                                    default:
                                        if (yzmax==0)
                                            yzmax += 3;
                                        else
                                            yzmax++;
                                        pbid = hhid + yzmax.ToString().PadLeft(2, '0');
                                        rowarry[k]["PID"] = pbid;
                                        break;
                                    }
                                }                            
                            }
                        }

                        if (TableToOracle.ModiDB(myda, mycon)) 
                        {
                            msg += tempID + "添加数据成功\n\r";
                        }
                }
            }
            string sql2 = @"Truncate table PEOPLE_INFO";
            TableToOracle.ExecuteSql(sql2, mycon);
            string sql3 = @"insert into PEOPLE_INFO
SELECT   PGID, HGID, PID, HHID, VID, PPNAME, SEX, CARDID, RELATION, NATION, DEGREE, SCHOOSTATE, HEALTH, WORKABIL, 
                WORKSTATE, WORKTIME, ISNRCMS, RESIDENPENS, ADDREATION, SUBREASON, POLITICAL, FAITH, HHGROUP, 
                PPSTATE, HELPSTATE, ISDEL, ISWORKOUT, ISSOLDER, ISOLDMANMEDI, READSCHOOL, ISHIGHSCHOOL, 
                ISDISABILITY, DISABILNO, MARRYSTATE, ISLEGALMARR, ISLEFTCHILD, ISBASICALLOWNCE, ISDROPOUT, 
                CHECKDATE, CHECKOR, INPUTOR, CHECKPHONE, INPUTPHONE, GDATE, MEMO, ISLABORTRANC, ISSERVICEMAN
FROM      PEOPLE_INFO_BJ";
            TableToOracle.ExecuteSql(sql3, mycon);
            TableToOracle.CloseCon();

            MessageBox.Show(msg);
        }

        private void buttonX10_Click(object sender, EventArgs e)
        {
            DataTable mytable = TableToOracle.GetSynonyms("BJ_FPY");
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string msg = "";
            foreach (DataRow myrow in mytable.Rows)
            {
                string tablename = myrow["table_name"].ToString();

                int rowscount = TableToOracle.CountData(tablename, mycon);
                if (rowscount <= 0)  //根据数据量分表，每个表的数据行不超过10万行
                {
                    rowscount = TableToOracle.CountData(tablename);
                    if (rowscount > 0)
                    {
                        int i;
                        DataTable dt = TableToOracle.QueryTable("Select * from FPY.CFG_BASIC_AREA_BIJIE where area_level=3");
                        for (i = 0; i < dt.Rows.Count; i++)
                        {

                            string townid = dt.Rows[i]["OID"].ToString();
                            string townname = dt.Rows[i]["SHORT_NAME"].ToString();
                            DataTable towntable = TableToOracle.QueryTable("Select * from FPY." + tablename + " where AZC004='" + townid + "'");
                            int towndatanumber = towntable.Rows.Count;
                            if (towndatanumber > 0)
                            {
                                towntable.TableName = tablename;
                                if (TableToOracle.OracleBulkCopy(towntable, mycon))
                                {
                                    msg += tablename + " 镇名" + townname + " 添加 " + towndatanumber + " 条数据成功~！\n\r";//MessageBox.Show("添加数据成功~！");
                                }
                                else
                                {

                                    msg += tablename + " 镇名" + townname + "添加数据失败~！\n\r";
                                }
                            }
                        }
                    }
                    else
                    {
                        msg += tablename + "原始表无数据~！\n\r";
                    }
                }

                //LogHelper.Log(msg);
                //msg = "";
            }

            MessageBox.Show(msg);
        }

        private void buttonX11_Click(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            rdidtable = TableToOracle.GetRDIDTbByType(mycon, "4");
            DataTable maprd = TableToOracle.QueryTable(mycon, "REGION_MAP", "", "", "");
            string msg = "";
            for (int i = 0; i < rdidtable.Rows.Count; i++)
            {
                string tempID = rdidtable.Rows[i]["RD_ID"].ToString();
                string rdname = rdidtable.Rows[i]["RD_SHORTNAME"].ToString();
                string sql = @"MERGE INTO (select * from HOUSE_BASE_INFO where HOUSE_BASE_INFO.VID like '"  + tempID + @"%') a1 
                        USING (select * from TEMP_HOUSE_NINFO where rowid in (SELECT   MAX(rowid) FROM TEMP_HOUSE_NINFO group by HHID) and  TEMP_HOUSE_NINFO.VID like '" + tempID + @"%')a2  
                        ON ( a1.HHID = a2.HHID) 
                        WHEN MATCHED THEN 
                        update set 
                                    a1.LONGITUDE = a2.LONGITUDE,
                                    a1.LATITUDE = a2.LATITUDE" ;
                int rslnum = TableToOracle.ExecuteSql(sql, mycon);
                if (rslnum > 0)
                {

                    msg = rdname + " " + tempID + " 更新经纬度成功!\n\r";
                }
                else
                {
                    msg = rdname + " " + tempID + " 更新经纬度失败或者经纬度已经更新!\n\r";
                }
                DataRow[] oldidarr = maprd.Select("RD_NEWID ='" + tempID + "'");
                string oldID = "";
                if (oldidarr.Length > 0)
                {
                    oldID = oldidarr[0]["RD_OLDID"].ToString();
                    oldID = oldID.Substring(0, 6);


                    string sql2 = @"MERGE INTO (select * from HOUSE_BASE_INFO where HOUSE_BASE_INFO.VID like '" + tempID + @"%' and PLANOUTYEAR is null)a1
                                    USING (select * from ARC_POOR_FAMILY_BIJIE where AZC005 like '" + oldID + @"%'and AAH015 is not null)a2
                                    ON ( a1.HGID = a2.AAA001) 
                                    WHEN MATCHED THEN 
                                    UPDATE SET
                                           a1.OUTYEAR = a2.AAH015";
                    int rslnum2 = TableToOracle.ExecuteSql(sql2, mycon);
                    if (rslnum2 > 0)
                    {

                        msg += rdname + " " + tempID + " 更新脱贫年度成功!\n\r";
                    }
                    else
                    {
                        msg += rdname + " " + tempID + " 更新脱贫年度失败或者已更新!\n\r";
                    }
                }
                //LogHelper.WriteLog(msg);
            }
            TableToOracle.CloseCon();

            textBoxXOutPut.Text = LogHelper.ReadLog();
            
            MessageBox.Show("处理完毕！");
        }

        private void buttonX8_Click(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            //DBConfig mycon = new DBConfig("61.159.180.163", "NBIGDATA", "BIGDATAUSER", "bigdatauser");
            DBConfig mytestcon = new DBConfig("61.159.180.163", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            DataTable mynotable = new DataTable();
            
            int[] idarr = new Int32[4];
            int lblbo = 0;
            zerotable = TableToOracle.GetRepeatFamiTb(mytestcon);
            for (int i = 0; i < zerotable.Rows.Count; i++)
            {
                string tempVilID = zerotable.Rows[i]["VID"].ToString();
                if (i > 0)
                {
                    if (tempVilID != zerotable.Rows[i - 1]["VID"].ToString())
                    {
                        lblbo = 0;
                        mynotable = TableToOracle.GetFmNoTb(mytestcon, tempVilID);
                        idarr = new Int32[5000];
                        //建立村的户表
                        for (int j = 0; j < mynotable.Rows.Count; j++)
                        {
                            idarr[Int32.Parse(mynotable.Rows[j][0].ToString())] = Int32.Parse(mynotable.Rows[j][0].ToString());
                        }
                    }
                }
                else
                {
                    lblbo = 0;
                    mynotable = TableToOracle.GetFmNoTb(mytestcon, tempVilID);
                    idarr = new Int32[5000];
                    //建立村的户表
                    for (int j = 0; j < mynotable.Rows.Count; j++)
                    {
                        idarr[Int32.Parse(mynotable.Rows[j][0].ToString())] = Int32.Parse(mynotable.Rows[j][0].ToString());
                    }
                }
                
                //遍历村表


                //string cardid = zerotable.Rows[i]["CARDID"].ToString();
                string hguid = zerotable.Rows[i]["HGID"].ToString();
                string rhhid = zerotable.Rows[i]["HHID"].ToString();

                int hbh = 0;


                for (int k = lblbo + 1; k < 5000; k++)
                    if (idarr[k] == 0) 
                    { 
                        hbh = lblbo = k;
                        idarr[k] = 1;
                        break; }
                //写入数据库处理
                string hhid = tempVilID + hbh.ToString().PadLeft(4, '0');
                Int64 hhno = Convert.ToInt64(hhid);
                if (TableToOracle.DBExistByHHID(mytestcon, rhhid)) 
                { 
                    //LogHelper.WriteLog("新号:" + hhid + "老号：" + rhhid + "\r\n");
                    string sqlstr = @"insert into  REGION_FILE( RF_FID, RF_PATH, RF_OWNER, RF_CHECKID, RF_ISCHECK, RF_CKDATE, RF_UID, RF_MEMO, VID, GDATE)  
select RF_FID, RF_PATH, " + hhno + ", RF_CHECKID, RF_ISCHECK, RF_CKDATE, '" + hguid + "', RF_MEMO, VID, GDATE from REGION_FILE where REGION_FILE.RF_OWNER = " + rhhid;
                    //int rlt1 = TableToOracle.ExecuteSql(sqlstr, mycon);
                    int rlt2 = TableToOracle.ExecuteSql(sqlstr, mytestcon);

                }
                ArrayList sqllist = new ArrayList();
                string sql1 = @"Update HOUSE_BASE_INFO set HOUSE_BASE_INFO.HHID=" + hhno +
                    "  where HOUSE_BASE_INFO.HGID='" + hguid + "'";
                string sql2 = @"Update HOUSE_COND_INFO set HOUSE_COND_INFO.HHID=" + hhno +
                    "  where HOUSE_COND_INFO.HGID='" + hguid + "'";
                string sql3 = @"Update HOUSE_DIC set HOUSE_DIC.HD_ID=" + hhno +
                    "  where HGID = '" + hguid + "'";
                string sql4 = @"Update HOUSE_FOUR_FIVE set HOUSE_FOUR_FIVE.HHID=" + hhno +
                    "  where HOUSE_FOUR_FIVE.HGUID = '" + hguid + "'";
                string sql5 = @"Update HOUSE_HELP_INFO set HOUSE_HELP_INFO.HHID=" + hhno +
                    "  where HOUSE_HELP_INFO.HGID = '" + hguid + "'";
                string sql6 = @"Update HOUSE_MOVE set HOUSE_MOVE.HHID=" + hhno +
                    "  where HOUSE_MOVE.HGID = '" + hguid + "'";
                string sql7 = @"Update HOUSE_OUT_INFO set HOUSE_OUT_INFO.HHID=" + hhno +
                    @"where HOUSE_OUT_INFO.HGID = '" + hguid + "'";
                string sql8 = @"Update PEOPLE_INFO set PEOPLE_INFO.HHID=" + hhno +
                    ",PID= to_number(to_char('" + hhno + "')||substr(PID,-2)) where PEOPLE_INFO.HGID = '" + hguid + "'";
                
                sqllist.Add(sql1);
                sqllist.Add(sql2);
                sqllist.Add(sql3);
                sqllist.Add(sql4);
                sqllist.Add(sql5);
                sqllist.Add(sql6);
                sqllist.Add(sql7);
                sqllist.Add(sql8);

                TableToOracle.ExecuteSqlTran(sqllist, mytestcon);

            }

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

        private void buttonXUpDateHHID_Click(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");

            DataTable mynotable = new DataTable();
            DataAdapter myda = new DataAdapter();
            string VillID = "";
            myda = TableToOracle.GetNOHHIDTable(mycon, "HOUSE_BASE_INFO_BJ");
            int yzmax = 0;
            zerotable = myda.Mytable;
            string msg = "";
            if (zerotable.Rows.Count > 0)
            {
                for (int j = 0; j < zerotable.Rows.Count; j++)
                //针对每一户进行操作
                {
                    string tempVID = zerotable.Rows[j]["VID"].ToString();                       
                    if (VillID != tempVID) 
                    {
                        VillID = tempVID;
                        mynotable = TableToOracle.QueryNoTb(mycon, "HOUSE_BASE_INFO_BJ", "VID", VillID);
                        yzmax = 0;
                    }
                    string pbno = "";
                    do
                    {
                        yzmax++;
                        pbno = tempVID + yzmax.ToString().PadLeft(4, '0');
                    } while (mynotable.Select("HHID='" + pbno + "'").Length > 0);

                    zerotable.Rows[j]["HHID"] = pbno;
                    zerotable.Rows[j]["MEMO"] = "HHIDISERROR";
                }
                if (TableToOracle.ModiDB(myda, mycon))
                {
                    msg = "更新数据成功\n\r";
                }
            }
            MessageBox.Show(msg);

        }

        private void buttonXUpdateHC_Click(object sender, EventArgs e)
        {
            string msg = "";
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string stable = "BJ_POOR_FAMILY";
            string sfields = @"AAA001,
                            AAE001,
                            AAE002,
                            AAE039,
                            AAE040,
                            AAE003,
                            AAE004,
                            AAE005,
                            AAE006,
                            AAE007,
                            AAE008,
                            AAE009,
                            AAE044,
                            AAE010,
                            GSGRH,
                            YBYH,
                            AAE045,
                            AAE900,
                            AAE011,
                            AAE012,
                            AAE017,
                            RHLYH,
                            NYHZZZ,
                            AAE013,
                            AAE014,
                            AAE015,
                            AAE016,
                            AAE043,
                            BZBQF,
                            AAE041,
                            AAE042,
                            AAE043,
                            AAE018,
                            NAE002,
                            AAE019,
                            NAE003,
                            NAE003,
                            NAE001,
                            AAE021,
                            NAE004,
                            AAE022,
                            AAE023,
                            AAE024,
                            AAE025,
                            AAE026,
                            AAE027,
                            AAE028,
                            Yjzdcjbt,
                            Lsjzj,
                            jtxmy,
                            Xnhcbzzj,
                            Wbgyj,
                            jtjjzw,
                            NAD007,
                            NAD008,
                            NAD012,
                            NAD013,
                            NAD014,
                            NAD015,
                            NAD016,
                            NAD010,
                            AAH005,
                            AAH002
                            ";
            string ttable = "HOUSE_COND_INFO";
            string tfields = @"HGID,
                            A17,
                            A17A,
                            A17B,
                            A17C,
                            A18,
                            A18A,
                            A18B,
                            A19,
                            A20,
                            A21,
                            A22,
                            WATERSTATE,
                            A23,
                            ISWARTERNET,
                            ISHARDYARD,
                            CONSUMES,
                            A24,
                            A25,
                            A26,
                            A31,
                            ISHARDROAD,
                            ISWTO,
                            A27,
                            A28,
                            A29,
                            A30,
                            ISONMOVEPLAN,
                            ISHELPHOUSE,
                            HOMEDATE,
                            HOMESTRCTURE,
                            MOVESTATE,
                            A32,
                            N02,
                            A33,
                            A34,
                            N03,
                            N01,
                            A35,
                            N04,
                            A36,
                            A36A,
                            A36B,
                            A36C,
                            A36D,
                            A36E,
                            A36F,
                            DISAIDMONEY,
                            TEMPHELPMONEY,
                            HANINCOME,
                            NRCMSMONEY,
                            FIVEMONEY,
                            HECOINCOME,
                            ECONAREA,
                            PRODAREA,
                            LONGITUDE,
                            LATITUDE,
                            ALTITUDE,
                            DISTANCETOWNS,
                            ISARMYFAMILY,
                            ISDROPOUTSTU,
                            MEMO,
                            GDATE
                            ";
            
            bool rslt = TableToOracle.InsertFromOrcl(mycon, stable, sfields, ttable, tfields, "", "AZC005");

            

            if (rslt)
            {
                
                MessageBox.Show(" 插入数据成功!\n\r");
            }
            else
            {
                MessageBox.Show(" 插入数据失败!\n\r");
            }
            stable = "HOUSE_BASE_INFO_BJ";
            sfields = "HHID,HGID";
            tfields = "HHID,HGID";
            rslt = TableToOracle.UpdateFromOrcl(mycon, stable, sfields, ttable, tfields, "a1.HGID = a2.HGID", "OLDVID");
            if (rslt)
            {

                MessageBox.Show(" 更新数据成功!\n\r");
            }
            else
            {
                MessageBox.Show(" 更新数据失败!\n\r");
            }
            msg = LogHelper.ReadLog();
            textBoxXOutPutrst.Text = msg;
        }

        private void buttonXAddPeople_Click(object sender, EventArgs e)
        {
            string msg = "";
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string stable = "BJ_POOR_FAMILY_MEMBER";
            string sfields = @"AHH002,
AAA001,
AAD001,
AAD002,
AAD003,
AAD004,
AAD005,
AAD006,
AAD007,
AAD008,
AAD009,
AAD010,
AAD011,
AAD012,
AAD013,
AAF001,
AAF002,
AAD015,
zjxy,
AZC006,
AAH006,
NAD005,
0,
AAD014,
AAD016,
AAD017,
aad018,
cjzh,
hyzk,
bljhz,
lset,
xsdb,
AAH004,
AAH003,
AAH001,
AAH008,
AAH007,
AAH002,
AAH005,
azc005";
            string ttable = "PEOPLE_INFO_BJ";
            string tfields = @"PGID,
HGID,
PPNAME,
SEX,
CARDID,
RELATION,
NATION,
DEGREE,
SCHOOSTATE,
HEALTH,
WORKABIL,
WORKSTATE,
WORKTIME,
ISNRCMS,
RESIDENPENS,
ADDREATION,
SUBREASON,
POLITICAL,
FAITH,
HHGROUP,
ppstate,
HELPSTATE,
ISDEL,
ISWORKOUT,
ISSOLDER,
ISOLDMANMEDI,
READSCHOOL,
DISABILNO,
MARRYSTATE,
ISLEGALMARR,
ISLEFTCHILD,
ISBASICALLOWNCE,
CHECKDATE,
CHECKOR,
INPUTOR,
CHECKPHONE,
INPUTPHONE,
GDATE,
MEMO,
oldvid";

            bool rslt = TableToOracle.InsertFromOrcl(mycon, stable, sfields, ttable, tfields, "", "AZC005");



            if (rslt)
            {
                textBoxXOutPutrst.Text += " 插入数据成功!\n\r";
            }
            else
            {
                textBoxXOutPutrst.Text += " 插入数据失败!\n\r" ;
            }

            MessageBox.Show("处理结束~！");
            msg = LogHelper.ReadLog();
            textBoxXOutPutrst.Text += msg;
        }

        private void btnXUpdateTBase_Click(object sender, EventArgs e)
        {

            string msg = "";
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string stable = "TOWN_BASE_INFO";
            string sfields = @"TID, LONGITUDE, LATITUDE, ALTITUDE, 
            TOWNNAME, ISDEL";
            string ttable = "TOWN_BASE_INFO_BJ";
            //string stable = "TOWN_BASE_INFO_BJ";
            //string sfields = "VID,HHID";
            string tfields = @"TID, LONGITUDE, LATITUDE, ALTITUDE, 
            TOWNNAME, ISDEL";
            string skeyfield = "TOWNGID";
            string tkeyfield = "TOWNGID";

            Collection<string> s1 = new Collection<string>(sfields.Split(','));
            Collection<string> s2 = new Collection<string>(tfields.Split(','));
            bool rslt = TableToOracle.UpdateFromOrcl(mycon, stable, s1, skeyfield,  ttable, s2, tkeyfield,  "TID");
            string sql = "insert into " + ttable + " select * from " + stable + " where " + skeyfield + " <> " + tkeyfield;
            int rsltint = TableToOracle.ExecuteSql(sql, mycon);

            if (rslt)
            {
                textBoxXOutPutrst.Text += " 插入数据成功!\n\r";
            }
            else
            {
                textBoxXOutPutrst.Text += " 插入数据失败!\n\r";
            }

            MessageBox.Show("处理结束~！");
            msg = LogHelper.ReadLog();
            textBoxXOutPutrst.Text += msg;

        }
        Collection<string> myworkbook = new Collection<string>();
        private void btnXBrowesExcel_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog myfolderdialog = new FolderBrowserDialog();
            myfolderdialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (myfolderdialog.ShowDialog() == DialogResult.OK)
            {
                textBoxXCsvFiles.Text = myfolderdialog.SelectedPath;
                myworkbook = IOHelper.GetFilePath(textBoxXCsvFiles.Text, "*.xls");
                listBoxAdv1.DataSource = myworkbook;
            }
        }

        private void listBoxAdv1_ItemClick(object sender, EventArgs e)
        {
            filePath = sender.ToString();

            //Collection<string> mysheetnames = ExcelOper.GetExcelSheetNames(filePath);
            Collection<string> mysheetnames = ExcelBll.GetSheetNames(filePath);
            //Collection<string> mysheetnames = EPPExcelHelper.GetExcelSheetNames(filePath);
            listBoxAdv3.DataSource = mysheetnames;
        }



        string filePath;

        private void listBoxAdv3_Click(object sender, EventArgs e)
        {
            DateTime mytime = DateTime.Now;
            //mytable = ExcelOper.ExcelToDataTable(this.listBoxAdv3.SelectedValue.ToString(), filePath);
            mytable = ExcelBll.GetSheet(this.listBoxAdv3.SelectedValue.ToString(), filePath).Tables[0];
            //mytable = EPPExcelHelper.Import(filePath, this.listBoxAdv3.SelectedValue.ToString());
            //mytable = ExcelOp.ReadExcel(this.listBoxAdv3.SelectedValue.ToString(), filePath,1);
            if (mytable.Rows.Count > 0)
            {
                //this.ToOrclbtnX2.Enabled = true;
                //this.btnXUpdate.Enabled = true;
                this.dataGridViewXExcel.DataSource = mytable;
            }
            else
            {
                this.dataGridViewXExcel.DataSource = null;
            }
            TimeSpan ts = DateTime.Now - mytime;
            this.labelX3.Text = ts.ToString();
        }

        DataTable mytable = new DataTable();

        private void btnXUpdate_Click(object sender, EventArgs e)
        {
            if (mytable.Rows.Count > 0)
            {
                //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
                foreach (DataRow myrow in mytable.Rows) 
                {
                    string hgid = myrow["HGID"].ToString();
                    string hhid = myrow["HHID"].ToString();
                    ArrayList sqllist = new ArrayList();
                    string sql1 = @"Update HOUSE_BASE_INFO set HOUSE_BASE_INFO.HHID = " + hhid +
                        "  where HOUSE_BASE_INFO.HGID='" + hgid + "'";
                    string sql2 = @"Update PEOPLE_INFO set PEOPLE_INFO.HHID =" + hhid +
                        " , PEOPLE_INFO.PID = to_number(to_char(" + hhid + ")||substr(to_char(PID),-2))  where PEOPLE_INFO.HGID='" + hgid + "'";
                    string sql3 = @"Update HOUSE_DIC set HOUSE_DIC.HD_ID=" + hhid +
                        "  where HOUSE_DIC.HGID = '" + hgid + "'";
                    string sql4 = @"Update HOUSE_COND_INFO set HOUSE_COND_INFO.HHID =" + hhid +
                        "  where HOUSE_COND_INFO.HGID = '" + hgid + "'";
                    sqllist.Add(sql1);
                    sqllist.Add(sql2);
                    sqllist.Add(sql3);
                    sqllist.Add(sql4);
                    TableToOracle.ExecuteSqlTran(sqllist, mycon);

                }
            }
        }

        private void ToOrclbtnX2_Click(object sender, EventArgs e)
        {
            if (mytable.Rows.Count > 0)
            {
                //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
                foreach (DataRow myrow in mytable.Rows)
                {
                    string hgid = myrow["HGID"].ToString();
                    //string hhid = myrow["HHID"].ToString();
                    ArrayList sqllist = new ArrayList();
                    string sql1 = @"delete from HOUSE_BASE_INFO " +
                        "  where HOUSE_BASE_INFO.HGID='" + hgid + "'";
                    string sql2 = @"delete from PEOPLE_INFO " +
                        "where PEOPLE_INFO.HGID='" + hgid + "'";
                    string sql3 = @"delete from  HOUSE_DIC"  +
                        "  where HOUSE_DIC.HGID = '" + hgid + "'";
                    string sql4 = @"delete from HOUSE_COND_INFO "  +
                        "  where HOUSE_COND_INFO.HGID = '" + hgid + "'";
                    sqllist.Add(sql1);
                    sqllist.Add(sql2);
                    sqllist.Add(sql3);
                    sqllist.Add(sql4);
                    TableToOracle.ExecuteSqlTran(sqllist, mycon);

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            villtable = TableToOracle.GetRDIDTbLikeID(mycon, "5224");
            
            DataTable noidtable = new DataTable("NOID");
            if (this.rBtnYes.Checked) 
            {
                

                string sql2 = @"Truncate table GROUPBYREASON";
                TableToOracle.ExecuteSql(sql2, mycon);

            }


            for (int i = 0; i < villtable.Rows.Count; i++)
            {
                string rgid = villtable.Rows[i]["RD_ID"].ToString();

                myidtable = TableToOracle.GetRSByID(mycon, rgid);
                noidtable = myidtable.Copy();
                noidtable.Rows.Clear();
                for (int j = 0; j < myidtable.Rows.Count; j++)
                {
                    DataRow mydr = myidtable.Rows[j];

                    string mydg = mydr["REASON"].ToString();
                    string mynum = mydr["num"].ToString();
                    if (mydg == "")
                    {
                        mydg = "未知";
                    }
                    Int64 hhno = Convert.ToInt64(rgid);
                    Int64 cmun = Convert.ToInt64(mynum);
                    string sql1 = @"insert into GROUPBYREASON values(" + hhno + ",'" + mydg + "'," + cmun + ")";
                    int oraclersl = TableToOracle.ExecuteSql(sql1, mycon);
                    if (oraclersl < 1)
                        noidtable.ImportRow(mydr);

                }
                if (noidtable.Rows.Count > 0)
                    XmlHelper.DataTableToXML(noidtable, @"D:\result\" + rgid + "_REASON.xml");

            }
            Button mybtn = (Button)sender;
            if (mybtn.Name != "btnTotalCount")
                MessageBox.Show("数据处理完毕！");
        }

        public DataTable villtable { get; set; }

        public DataTable myidtable { get; set; }

        private void button4_Click(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            villtable = TableToOracle.GetRDIDTbLikeID(mycon, "5224");
            DataTable noidtable = new DataTable("NOID");
            if (this.rBtnYes.Checked)
            {
                string sql2 = @"Truncate table GROUPBYEDU";
                TableToOracle.ExecuteSql(sql2, mycon);
            }

            for (int i = 0; i < villtable.Rows.Count; i++)
            {
                string rgid = villtable.Rows[i]["RD_ID"].ToString();

                myidtable = TableToOracle.GetDGByID(mycon,rgid);
                noidtable = myidtable.Copy();
                noidtable.Rows.Clear();
                for (int j = 0; j < myidtable.Rows.Count; j++)
                {
                    DataRow mydr = myidtable.Rows[j];

                    string mydg = mydr["DEGREE"].ToString();
                    string mynum = mydr["num"].ToString();
                    if (mydg == "")
                    {
                        mydg = "未知";
                    }
                    Int64 hhno = Convert.ToInt64(rgid);
                    Int64 cmun = Convert.ToInt64(mynum);
                    string sql1 = @"insert into GROUPBYEDU values(" + hhno + ",'" + mydg + "'," + cmun + ")";
                    int oraclersl = TableToOracle.ExecuteSql(sql1, mycon);
                    if (oraclersl < 1)
                        noidtable.ImportRow(mydr);

                }
                if (noidtable.Rows.Count > 0)
                    XmlHelper.DataTableToXML(noidtable, @"D:\result\" + rgid + "_DEGREE.xml");

            }

            Button mybtn = (Button)sender;
            if (mybtn.Name != "btnTotalCount")
                MessageBox.Show("数据处理完毕！");
        }
        DataTable rdtable = new DataTable();
        DataTable newrdtable = new DataTable();
        private void buttonX13_Click(object sender, EventArgs e)
        {
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");

            rdtable = TableToOracle.GetRDIDTbByType(mycon, "3");
            newrdtable = TableToOracle.GetRDIDTbByType(mycon, "3");
            this.comboBoxEx4.DataSource = rdtable;
            comboBoxEx4.ValueMember = "RD_ID";
            comboBoxEx4.DisplayMember = "RD_SHORTNAME";
            this.comboBoxEx5.DataSource = newrdtable;
            comboBoxEx5.ValueMember = "RD_ID";
            comboBoxEx5.DisplayMember = "RD_SHORTNAME";
        }

        private void btnTotalCount_Click(object sender, EventArgs e)
        {
            if (this.rBtnYes.Checked)
            {

                if (MessageBox.Show("的确要请除所有数据库数据吗？", "警告", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    this.rBtnNo.Checked = true;
                }
            }
            this.button3_Click(sender, e);
            this.button4_Click(sender, e);
            
            MessageBox.Show("数据处理完毕！");
        }

        private void buttonX12_Click(object sender, EventArgs e)
        {

        }

        private void buttonXGetTableName_Click(object sender, EventArgs e)
        {

        }

        private void btnXCreateData_Click(object sender, EventArgs e)
        {
           //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            DataAdapter myda = new DataAdapter();
            villtable = TableToOracle.GetRDIDTbLikeID(mycon, "5224");
            //DataTable noidtable = new DataTable("NOID");
            //noidtable = villtable.Copy();
            //noidtable.Rows.Clear();
            for (int i = 0; i < villtable.Rows.Count; i++)
            {
                string rgid = villtable.Rows[i]["RD_ID"].ToString();

                
               
//                string sql1 = @"update HELP_STU set (pid,pgid,hhid,hgid,vid,LEFTCHILD,ISDISABILITY,ISBASICALLOWNCE) 
//                                =
//                                (select pid,pgid,hhid,hgid,vid,LEFTCHILD,ISDISABILITY,ISBASICALLOWNCE from PEOPLE_INFO where PEOPLE_INFO.CARDID = HELP_STU.CARDID and PEOPLE_INFO.VID =" 
//                    + rgid + @") where pid is null";

                string sql1 = @"update HELP_STU set (PREASON,HPOP,PTYPE) 
                                                    =
                                                (select MAIN_REASON,POP,PTYPE from HOUSE_BASE_INFO where HOUSE_BASE_INFO.HHID = HELP_STU.HHID and rowid  in (select min(rowid) from HOUSE_BASE_INFO group by HOUSE_BASE_INFO.HHID) and VID ="
                    + rgid + @") where VID = " + rgid;
                int oraclersl = TableToOracle.ExecuteSql(sql1, mycon);
                //if (oraclersl > 1)
                //    noidtable.ImportRow(villtable.Rows[i]);                
            } 
            //if (noidtable.Rows.Count > 0)
            //    XmlHelper.DataTableToXML(noidtable, @"D:\result\HELP_STU.xml");
            //string sqlstr = @"update HELP_STU set ISTRUE = 1 where VID is not null";
            //int orlrst = TableToOracle.ExecuteSql(sqlstr, mycon);
            //sqlstr = @"update HELP_STU set ISTRUE = 0 where VID is null";
            //orlrst = TableToOracle.ExecuteSql(sqlstr, mycon);
            //sqlstr = @"update HELP_STU set (PREASON,HPOP,PTYPE) = (select MAIN_REASON,POP,PTYPE from HOUSE_BASE_INFO where HOUSE_BASE_INFO.HHID = HELP_STU.HHID ) where VID is not null";
            //orlrst = TableToOracle.ExecuteSql(sqlstr, mycon);
            //sqlstr = @"update HELP_STU set HINCOME = (select A33 from HOUSE_COND_INFO where HOUSE_COND_INFO.HHID = HELP_STU.HHID )  where VID is not null";
            //orlrst = TableToOracle.ExecuteSql(sqlstr, mycon);
   
            //myda = TableToOracle.GetNoVidInHelpStu(mycon);
            //myidtable = myda.Mytable;
            //noidtable = myidtable;
            //noidtable.Rows.Clear();
            //foreach(DataRow mydr in myidtable.Rows)
            //{
            //    string Fieldsname = " COUNTY,ADDRESS,ISTRUE,VID,SGID";
            //    string myaddr = mydr["ADDRESS"].ToString();
            //    string mycounty = mydr["COUNTY"].ToString();
            //    if (myaddr == "")
            //    {
            //        mydr["VID"] = "0";
            //        noidtable.ImportRow(mydr); 
            //    }
            //    else 
            //    {
            //        string villname = myaddr.IndexOf("");
            //        DataRow[] myrow = villtable.Select("RD_SHORTNAME='" + mycounty + "'");
            //        string mycid = "";
            //    }          

            //}

            ButtonX mybtn = (ButtonX)sender;
            if (mybtn.Name != "btnTotalCount")
                MessageBox.Show("数据处理完毕！");
        }

        private void buttonXPicUP_Click(object sender, EventArgs e)
        {
            //先导入命名空间：using System.IO;
            string[] line = File.ReadAllLines(@"D:\C#\OracleMigratePrj\OracleMigratePrj\bin\Debug\201612140007.log");
            //DBConfig mycon = new DBConfig("61.159.180.163", "NBIGDATA", "BIGDATAUSER", "bigdatauser");
            //遍历第10行
             //Console.WriteLine(line[9]);
            //遍历所有行
            string hhid = "";
            string pichhid = "";
            for (int i = 1; i < line.Length; i+=4)
            {
                Console.WriteLine(line[i]);
                int hhidindex = line[i].IndexOf("新号:");
                int picidindex = line[i].IndexOf("老号：");
                hhid = line[i].Substring(hhidindex + 3, 15);
                pichhid = line[i].Substring(picidindex + 3, 15);
                string hguid = "";
                DataTable hhidtable = TableToOracle.QueryNoTb(mycon, "HOUSE_BASE_INFO", "HHID", hhid);
                hguid = hhidtable.Rows[0]["HGID"].ToString();
                Console.WriteLine(hhid);
                Console.WriteLine(pichhid);
                string sqlstr = @"insert into  REGION_FILE( RF_FID, RF_PATH, RF_OWNER, RF_CHECKID, RF_ISCHECK, RF_CKDATE, RF_UID, RF_MEMO, VID, GDATE)  
select RF_FID, RF_PATH, " + hhid + ", RF_CHECKID, RF_ISCHECK, RF_CKDATE, '" + hguid + "', RF_MEMO, VID, GDATE from REGION_FILE where REGION_FILE.RF_OWNER = " + pichhid;
                //int rlt1 = TableToOracle.ExecuteSql(sqlstr, mycon);
                int rlt2 = TableToOracle.ExecuteSql(sqlstr, mycon);
                Console.WriteLine(rlt2);
            }
        }

        private void btnUpdateCountRs_Click(object sender, EventArgs e)
        {
            villtable = TableToOracle.GetRDIDTbLikeID(mycon, "5224");
             if (this.rBtnYes.Checked) 
            {
                

                string sql2 = @"Truncate table GROUPBYREASON";
                TableToOracle.ExecuteSql(sql2, mycon);

            }


             for (int i = 0; i < villtable.Rows.Count; i++)
             {
                 string rgid = villtable.Rows[i]["RD_ID"].ToString();
                 TableToOracle.CountAllRs(mycon, rgid);
             }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            villtable = TableToOracle.GetRDIDTbLikeID(mycon, "5224");
            if (this.rBtnYes.Checked)
            {


                string sql2 = @"Truncate table GROUPBYREASON";
                TableToOracle.ExecuteSql(sql2, mycon);

            }


            for (int i = 0; i < villtable.Rows.Count; i++)
            {
                string rgid = villtable.Rows[i]["RD_ID"].ToString();
                TableToOracle.CountRs(mycon, rgid);
            }
        }

        private void listBoxAdvCsvFile_ItemClick(object sender, EventArgs e)
        {
            filePath = sender.ToString();

            //CsvStreamReader mycsv = new CsvStreamReader(filePath);

            DataTable mytable = CsvHelper.OpenCSV(filePath);
            dataGridViewXCsv.DataSource = mytable;

            //Collection<string> mysheetnames = ExcelOper.GetExcelSheetNames(filePath);
            //Collection<string> mysheetnames = ExcelBll.GetSheetNames(filePath);
            //Collection<string> mysheetnames = EPPExcelHelper.GetExcelSheetNames(filePath);
            //listBoxAdv3.DataSource = mysheetnames;
        }

    }
}
