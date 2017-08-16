using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Model;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
namespace Common
{
    public static class JsonTools
    {
        // 从一个对象信息生成Json串
        public static string ObjectToJson(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            return Encoding.UTF8.GetString(dataBytes);
        }
        // 从一个Json串生成对象信息
        public static object JsonToObject(string jsonString, object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return serializer.ReadObject(mStream);
        }
        public static string UserMd5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("x2");

            }
            return pwd;
        }
        public static string GetJson(string url)
        {
            //访问https需加上这句话 
            // ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            //访问http（不需要加上面那句话） 

            string Md5scr = "bijieUniversity" + DateTime.Now.ToString("yyyyMMddHHmm"); //加密前数据
            string verifyCode = UserMd5(Md5scr).ToUpper(); //加密后数据

            string postString = "verifyCode=" + verifyCode + "&page=1&pageSize=10&type=all";//这里即为传递的参数，可以用工具抓包分析，也可以自己分析，主要是form里面每一个name都要加进来  
            byte[] postData = Encoding.UTF8.GetBytes(postString);//编码，尤其是汉字，事先要看下抓取网页的编码方式  

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
            byte[] responseData = webClient.UploadData(url, "POST", postData);//得到返回字符流
            //webClient.DownloadFile(url + postString, @"D:/test.jpg");


            string srcString = Encoding.UTF8.GetString(responseData);//解码  

            //string returnText = wc.DownloadString(url);

            //if (srcString.Contains("errcode"))
            //{
            //    //可能发生错误 
            //}
            ////Response.Write(returnText); 
            return srcString;
        }

        public static string GetJson(string url,int pageno ,int pagesize)
        {
 

            string Md5scr = "bijieUniversity" + DateTime.Now.ToString("yyyyMMddHHmm"); //加密前数据
            string verifyCode = UserMd5(Md5scr).ToUpper(); //加密后数据

            string condstr = "&page=" + pageno.ToString() + "&pageSize=" + pagesize + "&type=all";
            string postString = "verifyCode=" + verifyCode + condstr;//这里即为传递的参数，可以用工具抓包分析，也可以自己分析，主要是form里面每一个name都要加进来  
            byte[] postData = Encoding.UTF8.GetBytes(postString);//编码，尤其是汉字，事先要看下抓取网页的编码方式  

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
            try
            {
                byte[] responseData = webClient.UploadData(url, "POST", postData);//得到返回字符流



                string srcString = Encoding.UTF8.GetString(responseData);//解码
                
                return srcString;
            }
            catch (Exception ex) 
            {
                return "";
            }
            finally
            {
            	webClient.Dispose();
            }
            
        }
        public static void GetPic(string url,string filepath)
        {
            string Md5scr = "bijieUniversity" + DateTime.Now.ToString("yyyyMMddHHmm"); //加密前数据
            string verifyCode = UserMd5(Md5scr).ToUpper(); //加密后数据

            string postString = "&verifyCode=" + verifyCode;//这里即为传递的参数，可以用工具抓包分析，也可以自己分析，主要是form里面每一个name都要加进来  

            WebClient webClient = new WebClient();
            try
            {
                //webClient.DownloadFile(url + postString, filepath);
                while(true)
                {
                	webClient.DownloadFileTaskAsync(url + postString, filepath);
                	FileInfo myinfo = new FileInfo(filepath);
                	if(myinfo.Length>0)
                		break;
                	System.Threading.Thread.Sleep(100);
                }
                webClient.Dispose();
            }
            catch (Exception ex) 
            {

            }
        }
        static Hashtable writehash = new Hashtable();
        static void writefile(string filename, Hashtable writehash) 
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, writehash);
                fs.Close();
            }
            catch (Exception ex)
            {

            }  
        }
        public static Hashtable readhash(string filename) 
        {
            Hashtable aa = new Hashtable();
            try
            {
                FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
                BinaryFormatter bf = new BinaryFormatter();
                aa = (Hashtable)bf.Deserialize(fs);
                fs.Close();

            }
            catch (Exception ex)
            {

            }

            return aa;
        }
		public static void getobj(int pageno,int pagestartno,DBConfig mycon) 
        {
            int pagesize = 10;
            string getjsonurl = "http://www.hengdafuping.cn:8091/web/V1/getPoorInfo.json";
            int j = pageno;
            int i = pagestartno;
            string path = @"E:\fileup\UpLoadFile\";
            while ((j >= 0)&(j<29000))
            {
                string json = GetJson(getjsonurl, j, pagesize);
                if (json != "")//如果获取不到数据则停止
                {
                    JsonParser jptest = JsonConvert.DeserializeObject<JsonParser>(json);

                    List<Data> mylistdata = jptest.result.data;

                    if (mylistdata != null)
                    {   
                    	int listint = mylistdata.Count;
                        while (i < listint)
                        {
                            writehash.Add(j, i);

                            string cardid = mylistdata[i].identityID;
                            string qrstr = @"SELECT   HD_ID, HD_AREAID, HD_HHNAME, HD_GROUP, HGID FROM  BIGDATAUSER.HOUSE_DIC WHERE  CARDID LIKE '" + cardid + "%'";
                            DataTable mytable = TableToOracle.QueryTable(qrstr, mycon);
                            if ((mytable != null)&&(mytable.Rows.Count>0))
                            {
                            	
                            	string HHID = mytable.Rows[0]["HD_ID"].ToString();
	                            string HGID = mytable.Rows[0]["HGID"].ToString();
	                            string VID = mytable.Rows[0]["HD_AREAID"].ToString();	
                            	foreach (ImageData myimage in mylistdata[i].upImageList)
                                	{   
	                                    string fNickName = myimage.fNickName;
	                                    string filetype = gettype(fNickName);
	                                    string furl = myimage.fUrl;
	                                    
	                                    
	                                    string fullname = path+ @"noindata\"+ cardid + @"\" +fNickName;
	                                    
	                                    
	                                    string filepath = getpath(HHID);
	                                    string fullpath = path + filepath;
	                                    
	                                    string imagefilename = HHID +"_1707_" + filetype + myimage.fName.Substring(myimage.fName.Length - 4);
	                                    fullname = fullpath + imagefilename;
	                                    if (!File.Exists(fullname))//检测文件是否存在，不存在则创建
	                                    {
	                                        creatpath(fullpath); //创建目录
	                                        
	                                        GetPic(furl.Replace(":8080",":8091"), fullname);//获取图片，                                        
	                                        //GetPic(furl, fullname);//获取图片
	                                    }
	                                    if (!TableToOracle.DBExistByRF(mycon, imagefilename)) 
	                                    {
	                                        string insertstr = @"INSERT INTO REGION_FILE (RF_FID, RF_PATH, RF_OWNER, RF_UID, RF_MEMO, VID, GDATE) VALUES ('"
	                                            + imagefilename + "','"
	                                            + "http://61.159.180.167:8888/" + filepath.Replace(@"\","/") + "',"
	                                            + HHID + ",'"
	                                            + HGID + "','"
	                                            + fNickName + "','"
	                                            + VID + "',to_date('"
	                                            + DateTime.Now.ToString("yyyy-MM-dd") + "','YYYY-MM-DD'))"; //更新户的图片数据库
	                                        TableToOracle.ExecuteSql(insertstr, mycon);
	                                    }

                                	}

                                	string upstr = @"update HOUSE_BASE_INFO set LONGITUDE = " + mylistdata[i].lng + " , LATITUDE = " + mylistdata[i].lat + " where HHID = '" + HHID + "' AND LATITUDE is null";
                                
                                	TableToOracle.ExecuteSql(upstr, mycon); //更新户的经纬度,获取户编号，获取图片类型，生成存储路径与文件名
                                	mytable.Dispose();
                            	
                            }
                            else 
                            {
                                foreach (ImageData myimage in mylistdata[i].upImageList)
                                {
                                    string fName = myimage.fName;
                                    string fNickName = myimage.fNickName;
                                    string filetype = gettype(fNickName);
                                    string furl = myimage.fUrl;                                    
                                   
                                    string fullpath = path + @"notin\" + cardid + @"\";
                                    string fullname = fullpath + filetype + "_" + fName;
                                    if (!File.Exists(fullname))//检测文件是否存在，不存在则创建
                                    {
                                        creatpath(fullpath); //创建目录

                                        GetPic(furl.Replace(":8080", ":8091"), fullname);//获取图片，                                        
                                        //GetPic(furl, fullname);//获取图片
                                    }
                                }
                            	string rslstr =  XmlHelper.WriteXML<Data>(mylistdata[i], @"Notin.xml");
                            }
                            //当中断执行的时候，需要记住i值和当前页数
                            string filename = "savefile";
                            writefile(filename, writehash);
                            writehash.Clear();
                            i++;
                        }
                        mylistdata.Clear();
                    }
                    else 
                    {
                        writehash.Add(j, i);
                        string filename = "savefile";
                        writefile(filename, writehash);
                        writehash.Clear();
                        System.Threading.Thread.Sleep(5000);
                        continue;
                    }
                    j++; 
                    i = 0;
                }
                else
                {
                    writehash.Add(j, i);
                    string filename = "savefile";
                    writefile(filename, writehash);
                    writehash.Clear();
                    System.Threading.Thread.Sleep(5000);
                    continue;
                }
            }
        }



        public static void pgetobj(int pageno, int pagestartno, DBConfig mycon)
        {
            int pagesize = 10;
            string getjsonurl = "http://www.hengdafuping.cn:8091/web/V1/getPoorInfo.json";
            int j = pageno;
            int i = pagestartno;
            int savefileno = 0;
            string path = @"F:\HDFile\";
            while ((j >= 0) & (j < 29000))
            {
                string json = GetJson(getjsonurl, j, pagesize);
                if (json != "")//如果获取不到数据则停止
                {
                    JsonParser jptest = JsonConvert.DeserializeObject<JsonParser>(json);

                    List<Data> mylistdata = jptest.result.data;

                    if (mylistdata != null)
                    {
                        int listint = mylistdata.Count;
                        while (i < listint)
                        {
                            writehash.Add(j, i);

                            string cardid = mylistdata[i].identityID;
                            string qrstr = @"SELECT   HD_ID, HD_AREAID, HD_HHNAME, HD_GROUP, HGID FROM  BIGDATAUSER.HOUSE_DIC WHERE  CARDID LIKE '" + cardid + "%'";
                            DataTable mytable = TableToOracle.QueryTable(qrstr, mycon);
                            if ((mytable != null) && (mytable.Rows.Count > 0))
                            {

                                string HHID = mytable.Rows[0]["HD_ID"].ToString();
                                string HGID = mytable.Rows[0]["HGID"].ToString();
                                string VID = mytable.Rows[0]["HD_AREAID"].ToString();
                                foreach (ImageData myimage in mylistdata[i].upImageList)
                                {
                                    string fNickName = myimage.fNickName;
                                    string filetype = gettype(fNickName);
                                    string furl = myimage.fUrl;


                                    string fullname = path + @"noindata\" + cardid + @"\" + fNickName;


                                    string filepath = getpath(HHID);
                                    string fullpath = path + filepath;

                                    string imagefilename = HHID + "_1707_" + filetype + myimage.fName.Substring(myimage.fName.Length - 4);
                                    fullname = fullpath + imagefilename;
                                    if (!File.Exists(fullname))//检测文件是否存在，不存在则创建
                                    {
                                        creatpath(fullpath); //创建目录

                                        GetPic(furl.Replace(":8080", ":8091"), fullname);//获取图片，                                        
                                        //GetPic(furl, fullname);//获取图片
                                    }
                                    if (!TableToOracle.DBExistByRF(mycon, imagefilename))
                                    {
                                        string insertstr = @"INSERT INTO REGION_FILE (RF_FID, RF_PATH, RF_OWNER, RF_UID, RF_MEMO, VID, GDATE) VALUES ('"
                                            + imagefilename + "','"
                                            + "http://61.159.180.167:8888/" + filepath.Replace(@"\", "/") + "',"
                                            + HHID + ",'"
                                            + HGID + "','"
                                            + fNickName + "','"
                                            + VID + "',to_date('"
                                            + DateTime.Now.ToString("yyyy-MM-dd") + "','YYYY-MM-DD'))"; //更新户的图片数据库
                                        TableToOracle.ExecuteSql(insertstr, mycon);
                                    }

                                }

                                string upstr = @"update HOUSE_BASE_INFO set LONGITUDE = " + mylistdata[i].lng + " , LATITUDE = " + mylistdata[i].lat + " where HHID = '" + HHID + "' AND LATITUDE is null";

                                TableToOracle.ExecuteSql(upstr, mycon); //更新户的经纬度,获取户编号，获取图片类型，生成存储路径与文件名
                                mytable.Dispose();

                            }
                            else
                            {
                                foreach (ImageData myimage in mylistdata[i].upImageList)
                                {
                                    string fName = myimage.fName;
                                    string fNickName = myimage.fNickName;
                                    string filetype = gettype(fNickName);
                                    string furl = myimage.fUrl;

                                    string fullpath = path + @"notin\" + cardid + @"\";
                                    string fullname = fullpath + filetype + "_" + fName;
                                    if (!File.Exists(fullname))//检测文件是否存在，不存在则创建
                                    {
                                        creatpath(fullpath); //创建目录

                                        GetPic(furl.Replace(":8080", ":8091"), fullname);//获取图片，                                        
                                        //GetPic(furl, fullname);//获取图片
                                    }
                                }
                                string rslstr = XmlHelper.WriteXML<Data>(mylistdata[i], @"Notin.xml");
                            }
                            //当中断执行的时候，需要记住i值和当前页数
                            savefileno = i % 10;
                            string filename = "savefile" + savefileno.ToString();
                            writefile(filename, writehash);
                            writehash.Clear();
                            i++;
                        }
                        mylistdata.Clear();
                    }
                    else
                    {
                        writehash.Add(j, i);
                        savefileno = i % 10;
                        string filename = "savefile" + savefileno.ToString();
                        
                        writefile(filename, writehash);
                        writehash.Clear();
                        System.Threading.Thread.Sleep(5000);
                        continue;
                    }
                    j++;
                    i = 0;
                }
                else
                {
                    writehash.Add(j, i);
                    savefileno = i % 10;
                    string filename = "savefile" + savefileno.ToString();
                    writefile(filename, writehash);
                    writehash.Clear();
                    System.Threading.Thread.Sleep(5000);
                    continue;
                }
            }
        }
        private static string gettype(string fname) 
        {
            string ftype = "H1";
            string h2str = "内";
            string h3str = "查";
            string h4str = "合";
            if (fname.Contains(h2str)) 
            {
                ftype = "H2";
                return ftype;
            }
            if (fname.Contains(h3str))
            {
                ftype = "H3";
                return ftype;
            }
            if (fname.Contains(h4str))
            {
                ftype = "H4";
                return ftype;
            }
            return ftype;
        }
        private static string getpath(string OwnerID)
        {
            string UpPath = OwnerID.Substring(0, 4) + @"\" + OwnerID.Substring(4, 2) + @"\" + OwnerID.Substring(6, 3) + @"\" + OwnerID.Substring(9, 2) + @"\" + OwnerID.Substring(11, 4) + @"\";
          
            return UpPath;
        }
        static void creatpath(string fullpath) 
        {
            if (!Directory.Exists(fullpath))
                Directory.CreateDirectory(fullpath);
        }
    }
}
