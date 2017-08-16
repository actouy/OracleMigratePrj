using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Common;
using Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace OracleMigratePrj
{
    public partial class UpdateForm : DevComponents.DotNetBar.OfficeForm
    {
        public UpdateForm()
        {
            InitializeComponent();
        }
        DBConfig mycon = new DBConfig("61.159.180.163", "NBIGDATA", "BIGDATAUSER", "bigdatauser");
        private DataTable rdidtable;
        private DataTable zerotable;

        //DBConfig mycon = new DBConfig("61.159.180.163", "0BIGDATA", "BIGDATAUSER", "bigdatauser");
        private void buttonX1_Click(object sender, EventArgs e)
        {
            string sql2 = @"CREATE TABLE POOR_FAMILY_2017_NEW(AAA001, AZC001, AZC002, AZC003, AZC004, AZC005, AZC006, AAC004, AAF001, AAF002, AAD001, AAD105, AAD107, 
                AAD108, AAE001, AAE002, AAE003, AAE004, AAE005, AAE006, AAE007, AAE008, AAE009, AAE010, AAE900, AAE011, 
                AAE012, AAE013, AAE014, AAE015, AAE016, AAE017, AAE018, AAE019, AAE020, AAE021, AAE022, AAE023, AAE024, 
                AAE025, AAE026, AAE027, AAE028, AAH007, AAH001, AAH002, AAH008, AAH003, AAH004, AAH005, AAH006, NAME1, 
                NAME2, NAME3, NAME4, NAME5, NAME6, AAE039, AAE040, AAE041, AAE042, AAE043, AAE044, AAE045, AAH009, 
                AAH010, AAH011, AAH012, AAH013, AAH014, AAH015, NAD001, NAE001, NAE002, NAE003, NAE004, NAD002, NAD003, 
                NAH001, NAH002, BNAE001, BNAE002, BAAE019, BAAE021, NAD004, NAH003, BNAD004, NAD005, NAD006, SRC_IMG, 
                ISVIRTUAL, LONGITUDE, LATITUDE, NAH004, NAH005, NAH006, NAH007, NAD007, NAD008, NAD009, NAD010, NAD011, 
                NAD012, NAD013, NAD014, NAD015, NAD016, FAMILY_CODE, OUT_POOR, NAD017, BANGFUTYPE, SPECIALPOOR, 
                BZBQF, BQDUIXIANG, GSGRH, RHLYH, YBYH, NYHZZZ, YJZDCJBT, LSJZJ, WBGYJ, JTXMY, JTJJZW, XNHCBZZJ, 
                AZC005BAK, AZC005HISBAK, AAC013, AAC014, AAC308, AAC083, AAC086, AAC085, AAA001BAK, AAC001,VID) 
				as SELECT   AAA001, AZC001, AZC002, AZC003, AZC004, AZC005, AZC006, AAC004, AAF001, AAF002, AAD001, AAD105, AAD107, 
                AAD108, AAE001, AAE002, AAE003, AAE004, AAE005, AAE006, AAE007, AAE008, AAE009, AAE010, AAE900, AAE011, 
                AAE012, AAE013, AAE014, AAE015, AAE016, AAE017, AAE018, AAE019, AAE020, AAE021, AAE022, AAE023, AAE024, 
                AAE025, AAE026, AAE027, AAE028, AAH007, AAH001, AAH002, AAH008, AAH003, AAH004, AAH005, AAH006, NAME1, 
                NAME2, NAME3, NAME4, NAME5, NAME6, AAE039, AAE040, AAE041, AAE042, AAE043, AAE044, AAE045, AAH009, 
                AAH010, AAH011, AAH012, AAH013, AAH014, AAH015, NAD001, NAE001, NAE002, NAE003, NAE004, NAD002, NAD003, 
                NAH001, NAH002, BNAE001, BNAE002, BAAE019, BAAE021, NAD004, NAH003, BNAD004, NAD005, NAD006, SRC_IMG, 
                ISVIRTUAL, LONGITUDE, LATITUDE, NAH004, NAH005, NAH006, NAH007, NAD007, NAD008, NAD009, NAD010, NAD011, 
                NAD012, NAD013, NAD014, NAD015, NAD016, FAMILY_CODE, OUT_POOR, NAD017, BANGFUTYPE, SPECIALPOOR, 
                BZBQF, BQDUIXIANG, GSGRH, RHLYH, YBYH, NYHZZZ, YJZDCJBT, LSJZJ, WBGYJ, JTXMY, JTJJZW, XNHCBZZJ, 
                AZC005BAK, AZC005HISBAK, AAC013, AAC014, AAC308, AAC083, AAC086, AAC085, AAA001BAK, AAC001,rd_newid
FROM      BJ_POOR_FAMILY_2017_NEW,REGION_MAP where BJ_POOR_FAMILY_2017_NEW.AZC005 = REGION_MAP.RD_OLDID";
            TableToOracle.ExecuteSql(sql2, mycon);
        }

        private void buttonXCheckVid_Click(object sender, EventArgs e)
        {
            string sqlstr = @"SELECT DISTINCT NAME5, NAME4, NAME3, VID, RD_SHORTNAME
FROM      POOR_FAMILY_2017_NEW, REGION_DICT
WHERE   (VID = RD_ID) AND (SUBSTR(NAME5, 1, 3) 
                <> SUBSTR(RD_SHORTNAME, 1, 3)) ORDER BY VID";
            DataTable mytable = TableToOracle.QueryTable(sqlstr, mycon);
            ExcelHelper.OutFileToDisk(mytable, "POOR_FAMILY_2017_NEW", "MY_POOR_FAMILY.xlsx");
            if (File.Exists("MY_POOR_FAMILY.xlsx"))
            {
                System.Diagnostics.Process.Start("MY_POOR_FAMILY.xlsx");
            }
        }

        private void buttonXUPdateHHID_Click(object sender, EventArgs e)
        {
            DataTable mynotable = new DataTable();
            DataAdapter myda = new DataAdapter();
            string VillID = "";
            //string qrsql = "SELECT * FROM POOR_FAMILY_2017 where substr(POOR_FAMILY_2017.HHID,1,11) <> POOR_FAMILY_2017.VID or hhid is null order by vid";
            myda = TableToOracle.GetOracleBulkData(mycon, "POOR_FAMILY_2017_NEW", "AAA001,VID,OHHID,MEMO", "substr(POOR_FAMILY_2017_NEW.OHHID,1,11) <> POOR_FAMILY_2017_NEW.VID or ohhid is null", "VID");
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
                        mynotable = TableToOracle.QueryNoTb(mycon, "HOUSE_BASE_INFO", "VID", VillID);
                        yzmax = 0;
                    }
                    string HHno = "";
                    do
                    {
                        yzmax++;
                        HHno = tempVID + yzmax.ToString().PadLeft(4, '0');
                    } while (mynotable.Select("HHID ='" + HHno + "'").Length > 0);

                    zerotable.Rows[j]["OHHID"] = HHno;
                    zerotable.Rows[j]["MEMO"] = "HHIDISERROR";
                }
                if (TableToOracle.ModiDB(myda, mycon))
                {
                    msg = "更新数据成功\n\r";
                }
            }
            MessageBox.Show(msg);
        }



        private void buttonX2_Click_1(object sender, EventArgs e)
        {
            DataAdapter myda = new DataAdapter();
            int yzmax;
            string msg = "";
            myda = TableToOracle.GetOracleBulkData(mycon, "POOR_PEOPLE_2017_NEW", "AHH002,AAA001,AAD004,VID,HHID,PBID,PGID,MEMO", "PBID is null", "VID");

            zerotable = myda.Mytable;

            
            //try
            //{


            //    zerotable = new DataTable("modidata"); 

            //    zerotable.Columns.Add("AHH002");
            //    zerotable.Columns.Add("AAA001");
            //    zerotable.Columns.Add("AAD004");
            //    zerotable.Columns.Add("VID");
            //    zerotable.Columns.Add("HHID");
            //    zerotable.Columns.Add("PBID");
            //    zerotable.Columns.Add("PGID");
            //    zerotable.ReadXml(@"D:\data.xml"); 
                
            //}
            //catch (Exception ex)
            //{
            //    string strTest = ex.Message;
               
            //}            
            
            if (zerotable.Rows.Count > 0)
            {
                for (int j = 0; j < zerotable.Rows.Count; j++)//针对每一户进行操作
                {
                    //string mno = myda.Mytable.Rows[j]["AHH002"].ToString();
                    //string zno = zerotable.Rows[j]["AHH002"].ToString();
                    //myda.Mytable.Rows[j]["PBID"] = zerotable.Rows[j]["PBID"];
                    //myda.Mytable.Rows[j]["PGID"] = zerotable.Rows[j]["PGID"];
                    string mypid = zerotable.Rows[j]["PBID"].ToString();
                    if (mypid != "")
                        continue;
                    yzmax = 0;
                    int pbno = 0;
                    string hhid = zerotable.Rows[j]["HHID"].ToString();
                    string hguid = zerotable.Rows[j]["AAA001"].ToString();
                    DataRow[] rowarry = zerotable.Select("AAA001='" + hguid + "' and PBID is null");
                    if (rowarry.Length > 0)
                    {
                        for (int k = 0; k < rowarry.Length; k++)
                        {

                            string myrl = rowarry[k]["AAD004"].ToString();
                            string pgid = rowarry[k]["PGID"].ToString();
                            if (pgid == "")
                                rowarry[k]["PGID"] = Guid.NewGuid().ToString();
                            string pbid = "";
                            switch (myrl)
                            {
                                case "户主":
                                    pbno = 1;
                                    pbid = hhid + pbno.ToString().PadLeft(2, '0');
                                    rowarry[k]["PBID"] = pbid;
                                    break;
                                case "配偶":
                                    pbno = 2;
                                    pbid = hhid + pbno.ToString().PadLeft(2, '0');
                                    rowarry[k]["PBID"] = pbid;
                                    break;
                                default:
                                    if (yzmax == 0)
                                        yzmax += 3;
                                    else
                                        yzmax++;
                                    pbid = hhid + yzmax.ToString().PadLeft(2, '0');
                                    rowarry[k]["PBID"] = pbid;
                                    break;
                            }
                        }
                    }
                }

                if (TableToOracle.ModiDB(myda, mycon))
                {
                    msg +=  "添加数据成功\n\r";
                }
            }
        }

        private void buttonXInsertPB_Click(object sender, EventArgs e)
        {
            string msg = "";
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string stable = "POOR_PEOPLE_2017";
            string sfields = @"PGID,
AAA001,
PBID,
HHID,
VID,
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
AAF003,
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
AAH005";
            string ttable = "PEOPLE_INFO";
            string tfields = @"PGID,
HGID,
PID,
HHID,
VID,
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
MEMO";

            bool rslt = TableToOracle.InsertFromOrcl(mycon, stable, sfields, ttable, tfields, " rowid  in (select max(rowid) from POOR_PEOPLE_2017 group by PGID) ", "VID,HHID");



            if (rslt)
            {
                msg += " 插入数据成功!\n\r";
            }
            else
            {
                msg += " 插入数据失败!\n\r";
            }

            //MessageBox.Show("处理结束~！");
            msg = LogHelper.ReadLog();
            MessageBox.Show(msg);
        }

        private void buttonXUpdateLng_Click(object sender, EventArgs e)
        {
            string sql2 = @"MERGE INTO (select * from POOR_FAMILY_2017_NEW) a1 
                        USING (select * from HOUSE_BASE_INFO where rowid in (SELECT   MAX(rowid) FROM HOUSE_BASE_INFO group by HHID) and  HOUSE_BASE_INFO.LATITUDE is not null)a2  
                        ON ( a1.OHHID = a2.HHID) 
                        WHEN MATCHED THEN 
                        update set 
                                    a1.LONGITUDE = a2.LONGITUDE,
                                    a1.LATITUDE = a2.LATITUDE";
            int rltnum = TableToOracle.ExecuteSql(sql2, mycon);
            if (rltnum > 0)
                MessageBox.Show("更新成功");
            else
                MessageBox.Show("更新失  败");
        }

        private void buttonXImport_Click(object sender, EventArgs e)
        {
            string msg = "";
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string stable = "POOR_FAMILY_2017";
            string sfields = @"AAA001,
HHID,
AAD001,
NAD002,
NAD003,
AAC004,
AAD107,
AAD105,
VID,
NAME6,
AAF001,
AAF002,
BANGFUTYPE,
Nad004,
NAD001,
AAD108,
AAH006,
AAH007,
AAH001,
AAH002,
AAH008,
AAH003,
AAH004,
NAH002,
NAH001,
NAH003,
0,
AAH009,
null,
SPECIALPOOR,
0,
AAH012,
null,
BQDUIXIANG,
null,
null,
0,
null,
LONGITUDE,
LATITUDE,
ALTITUDE,
null,
AAH015";
            string ttable = "HOUSE_BASE_INFO";
            string tfields = @"HGID,
HHID,
HHNAME,
CARDID,
POP,
PHONE,
PTYPE,
PSTANDARD,
VID,
VGROUP,
BANK,
ACCOUNT,
HELP_PLANTYPE,
HELPTYPE,
MAIN_REASON,
OTHER_REASON,
CHECKSTATE,
INORPHONE,
INOR,
INDATE,
MODIPHONE,
MODITOR,
MODIDATE,
CHECKPHONE,
CHECKOR,
CHECKDATE,
STATE,
OUTPOORTATE,
MEMO,
ISSPECIAL,
ISDELETE,
PLANOUTYEAR,
ISHELPPROJECT,
ISIMMIGRANT,
ISHELPPEOPLE,
ISPLAN,
ISNULL,
AREATYPE,
LONGITUDE,
LATITUDE,
ALTITUDE,
SUBREASON,
outyear";

            bool rslt = TableToOracle.InsertFromOrcl(mycon, stable, sfields, ttable, tfields, "", "VID,HHID");



            if (rslt)
            {
                msg += " 插入数据成功!\n\r";
            }
            else
            {
                msg += " 插入数据失败!\n\r";
            }

            //MessageBox.Show("处理结束~！");
            msg += LogHelper.ReadLog();
            MessageBox.Show(msg);
        }

        private void buttonXImportCon_Click(object sender, EventArgs e)
        {
            string msg = "";
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string stable = "POOR_FAMILY_2017";
            string sfields = @"AAA001,
HHID,
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
                            LONGITUDE,
                            LATITUDE,
                            ALTITUDE,
                            NAD015,
                            NAD016,
                            NAD010,
                            AAH005,
                            AAH002
";
            string ttable = "HOUSE_COND_INFO";
            string tfields = @"HGID,
HHID,
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

            bool rslt = TableToOracle.InsertFromOrcl(mycon, stable, sfields, ttable, tfields, "", "VID,HHID");



            if (rslt)
            {
                msg += " 插入数据成功!\n\r";
            }
            else
            {
                msg += " 插入数据失败!\n\r";
            }

            //MessageBox.Show("处理结束~！");
            msg += LogHelper.ReadLog();
            MessageBox.Show(msg);
        }

        private void buttonXHDIC_Click(object sender, EventArgs e)
        {
            string msg = "";
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string stable = "POOR_FAMILY_2017";
            string sfields = @"HHID,
VID,
AAD001,
NAME6,
AAA001,
NAD002
";
            string ttable = "HOUSE_DIC";
            string tfields = @"HD_ID,
HD_AREAID,
HD_HHNAME,
HD_GROUP,
HGID,
CARDID
";

            bool rslt = TableToOracle.InsertFromOrcl(mycon, stable, sfields, ttable, tfields, "", "VID,HHID");



            if (rslt)
            {
                msg += " 插入数据成功!\n\r";
            }
            else
            {
                msg += " 插入数据失败!\n\r";
            }

            //MessageBox.Show("处理结束~！");
            msg += LogHelper.ReadLog();
            MessageBox.Show(msg);
        }

        private void buttonXUpdata_Click(object sender, EventArgs e)
        {
            string msg = "";
            //DBConfig mycon = new DBConfig("219.141.106.196", "OBIGDATA", "BIGDATAUSER", "bigdatauser");
            string stable = "POOR_FAMILY_2017";
            string sfields = @"HHID,VID";
            string skeyfield = "AAA001";
            string swhere = " OHHID <> HHID ";



            string ttable = "REGION_FILE";
            string tfields = @"RF_OWNER,VID";
            string tkeyfield = "RF_UID";
            string twhere = @"";
            


            Collection<string> s1 = new Collection<string>(sfields.Split(','));
            Collection<string> s2 = new Collection<string>(tfields.Split(','));
            bool rslt = TableToOracle.UpdateFromOrcl(mycon, stable, s1, skeyfield, swhere, ttable, s2, tkeyfield, twhere, "VID");
            
            if (rslt)
            {
                msg += " 更新数据成功!\n\r";
            }
            else
            {
                msg += " 更新数据失败!\n\r";
            }

            //MessageBox.Show("处理结束~！");
            msg += LogHelper.ReadLog();
            MessageBox.Show(msg);
        }
        //更新
        private void buttonXUpdateID_Click(object sender, EventArgs e)
        {
            string sqlstr = @"UPDATE POOR_FAMILY_2017_NEW SET  OHHID = (SELECT HHID FROM HOUSE_BASE_INFO WHERE HGID = POOR_FAMILY_2017_NEW.AAA001) WHERE OHHID IS NULL";
            int updatanum = TableToOracle.ExecuteSql(sqlstr, mycon);
            this.labelXNO.Text = updatanum.ToString();
            

        }

        private void buttonXUpdatePeople_Click(object sender, EventArgs e)
        {
            string sql2 = @"CREATE TABLE POOR_PEOPLE_2017_NEW(AHH002, AAA001, AAD001, AAD002, AAD003, AAD004, AAD005, AAD006, AAD007, AAD008, AAD009, AAD010, AAD011, 
                AAD012, AAD013, AAH001, AAH002, AAH003, AAH004, AAH005, AAH006, AAH007, AAH008, AAF001, AAF002, AAF003, 
                AZC002, AZC003, AZC004, AZC005, AZC006, A1, LATITUDE, LONGITUDE, LONGITUDE_OLD, LATITUDE_OLD, NAD005, 
                AAD014, AAD015, AAD016, AAD017, AAD018, SFCJ, CJZH, HYZK, BLJHZ, LSET, XSDB, ZJXY, AZC005BAK, 
                AZC005HISBAK, AAB025, AAB026, AAB027, AAB028, AAB029, AAB030, AAB031, AAB022, AAA001BAK, VID) 
				as SELECT   AHH002, AAA001, AAD001, AAD002, AAD003, AAD004, AAD005, AAD006, AAD007, AAD008, AAD009, AAD010, AAD011, 
                AAD012, AAD013, AAH001, AAH002, AAH003, AAH004, AAH005, AAH006, AAH007, AAH008, AAF001, AAF002, AAF003, 
                AZC002, AZC003, AZC004, AZC005, AZC006, A1, LATITUDE, LONGITUDE, LONGITUDE_OLD, LATITUDE_OLD, NAD005, 
                AAD014, AAD015, AAD016, AAD017, AAD018, SFCJ, CJZH, HYZK, BLJHZ, LSET, XSDB, ZJXY, AZC005BAK, 
                AZC005HISBAK, AAB025, AAB026, AAB027, AAB028, AAB029, AAB030, AAB031, AAB022, AAA001BAK, rd_newid
FROM      BJ_POOR_FAMILY_MEMBER_2017_NEW,REGION_MAP where BJ_POOR_FAMILY_MEMBER_2017_NEW.AZC005 = REGION_MAP.RD_OLDID";
            TableToOracle.ExecuteSql(sql2, mycon);
        }

        private void buttonXUpdatePHHID_Click(object sender, EventArgs e)
        {
            
            string stable = "POOR_FAMILY_2017_NEW";
            string ttable = "POOR_PEOPLE_2017_NEW";
            string sfields = "VID,OHHID";
            string tfields = "VID,HHID";
            string skeyfield = "AAA001";
            string tkeyfield = "AAA001";
            string swhere = " OHHID is not null";
            string twhere = " HHID is null";
            string msg = "";
            Collection<string> s1 = new Collection<string>(sfields.Split(','));
            Collection<string> s2 = new Collection<string>(tfields.Split(','));
            bool rslt = TableToOracle.UpdateFromOrcl(mycon, stable, s1, skeyfield, swhere, ttable, s2, tkeyfield, twhere, "VID");
            if (rslt)
            {

                msg += "更新数据成功!\n\r";
            }
            else
            {
                msg += "更新数据失败!\n\r";
            }
            MessageBox.Show(msg);
        }

        private void buttonXUpdatePbid_Click(object sender, EventArgs e)
        {
            string ttable = "POOR_PEOPLE_2017_NEW";
            string stable = "PEOPLE_INFO";
            string sfields = "PGID,PID";
            string tfields = "PGID,PBID";
            string skeyfield = "CARDID";
            string tkeyfield = "AAD003";
            string swhere = " rowid in (select max(rowid) from PEOPLE_INFO group by CARDID) ";
            string twhere = " PGID is null";
            string msg = "";
            Collection<string> s1 = new Collection<string>(sfields.Split(','));
            Collection<string> s2 = new Collection<string>(tfields.Split(','));
            bool rslt = TableToOracle.UpdateFromOrcl(mycon, stable, s1, skeyfield, swhere, ttable, s2, tkeyfield, twhere, "");
            if (rslt)
            {

                msg += "更新数据成功!\n\r";
            }
            else
            {
                msg += "更新数据失败!\n\r";
            }
            MessageBox.Show(msg);
        }
    }
}