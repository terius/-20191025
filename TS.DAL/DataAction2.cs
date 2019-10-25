using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Xml;
using TS.Common;
using TS.Model;

namespace TS.DAL
{


    public class MyXmlDocument : XmlDocument
    {
        private volatile static MyXmlDocument _instance = null;
        private static readonly object lockHelper = new object();
        private MyXmlDocument() { }
        public static MyXmlDocument CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new MyXmlDocument();
                }
            }
            return _instance;
        }

    }
    public class DataAction2
    {
        private static readonly int SaveLog = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SaveLog"]);
        private readonly string LocalHead = System.Configuration.ConfigurationManager.AppSettings["LocalHead"];
        //   private readonly string FilterTable = System.Configuration.ConfigurationManager.AppSettings["FilterTable"];
        //   private readonly string FilterTradeCode = System.Configuration.ConfigurationManager.AppSettings["FilterTradeCode"];
        private readonly string ChangeDecTypeRule = System.Configuration.ConfigurationManager.AppSettings["ChangeDecTypeRule"];
        private readonly string ChangeDecTypeForDefConn = System.Configuration.ConfigurationManager.AppSettings["ChangeDecTypeForDefConn"];
        private readonly string ReadPath = System.Configuration.ConfigurationManager.AppSettings["ReadPath"];
        private readonly string SavePath = System.Configuration.ConfigurationManager.AppSettings["SavePath"];
        private readonly string FailPath = System.Configuration.ConfigurationManager.AppSettings["FailPath"];
        private readonly string HGPath = System.Configuration.ConfigurationManager.AppSettings["HGPath"];
        private readonly string OtherPath = System.Configuration.ConfigurationManager.AppSettings["OtherPath"];
        private readonly string CheckBillSql = "select count(1) from {0} where bill_no = @bill_no";
        private string UpdateSql = "";
        private string InsertSql = "";

        private readonly string GetDBTimeSQL = "select getdate()";

        // private ConnCollection ConnList;
        private Hashtable DecTranDic;
        //  private string NewConnString;
        //    private string NewTable;

        public DateTime GetDBTime()
        {

            DateTime dtnow = DateTime.Now;
            try
            {
                object time = DbHelperSQL.GetSingle(GetDBTimeSQL);
                return DateTime.Parse(time.ToString());
            }
            catch
            {

                return dtnow;
            }

        }

        public void ReadFiles()
        {
            DirectoryInfo di = new DirectoryInfo(HGPath);
            FileInfo newFile = null;
            MyXmlDocument doc = MyXmlDocument.CreateInstance();

            //try
            //{
            //    while (true)
            //    {
            //        di = new DirectoryInfo(HGPath);
            //        bool rs = di.Exists;
            //        if (rs)
            //        {
            //            break;
            //        }
            //        Thread.Sleep(60000);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Loger.LogMessage(ex.ToString());
            //}
            // FileHelper.WriteLog("开始获取文件");
            S();
            FileInfo[] files = di.GetFiles();

            //  int icount = Math.Min(50, files.Length);
            int icount = files.Length;
            E("", "从海关服务器读取文件");
            FileHelper.WriteLog("从海关服务上获取到" + icount + "个文件");
            DateTime dtS = DateTime.Now;
            string dueTime, dueTxt;
            string newFilePath = "";
            for (int i = 0; i < icount; i++)
            {
                try
                {
                    dtS = DateTime.Now;
                    S();
                    newFilePath = MoveFileToPath(files[i], ReadPath);
                    CopyFileToPath(newFilePath, OtherPath);

                    E(files[i].Name, "移动到Read文件夹");
                    S();
                    newFile = new FileInfo(newFilePath);
                    //  XmlDocument doc = new XmlDocument();
                    doc.Load(newFile.FullName);
                    XmlNodeList node = doc.SelectNodes("DATA/ENTRY_HEAD");
                    IList<EHS_ENTRY_HEAD> infos = new List<EHS_ENTRY_HEAD>();

                    foreach (XmlNode item in node)
                    {
                        EHS_ENTRY_HEAD info = XmlHelper.Deserialize<EHS_ENTRY_HEAD>(item.OuterXml);
                        infos.Add(info);
                    }
                    E(newFile.Name, "读取文件");
                    S();
                    int rs = 0;
                    foreach (var info in infos)
                    {
                        rs += SaveInfo(info);
                    }
                    E(newFile.Name, "存储");
                    S();
                    if (rs > 0)
                    {
                        MoveFileToPath(newFile, true);
                    }
                    else
                    {
                        MoveFileToPath(newFile, false);
                    }
                    E(newFile.Name, "移动到Save文件夹");


                    dueTime = DateTime.Now.Subtract(dtS).TotalSeconds.ToString("0.00");
                    dueTxt = (newFile == null ? "未知文件" : newFile.Name) + "处理完成，用时：" + dueTime + "秒";
                    FileHelper.WriteLog(dueTxt);
                    Console.WriteLine(dueTxt);

                }
                catch (Exception ex)
                {
                    MoveFileToPath(newFile, false);
                    Console.WriteLine((newFile == null ? "未知文件" : newFile.Name) + "处理失败,原因:" + ex.Message);
                    Loger.LogMessage(ex.ToString());
                }
            }

        }

        DateTime dtt = DateTime.Now;
        private void S()
        {
            if (SaveLog == 1)
            {
                dtt = DateTime.Now;
            }
        }

        private void E(string fileName, string msg)
        {
            if (SaveLog == 1)
            {
                string due = DateTime.Now.Subtract(dtt).TotalSeconds.ToString("0.00");
                FileHelper.WriteLog(fileName + msg + "用时：" + due + "秒");
            }
        }

        private int SaveInfo(EHS_ENTRY_HEAD info)
        {
            if (info == null)
            {
                return 0;
            }
            int rs = 0;
            try
            {
                //S();
                string newDecType = ChangeDecType(info.TRADE_CODE, info.DEC_TYPE);
                info.DEC_TYPE = newDecType;
                bool billIsExist = CheckBillExist(info.BILL_NO);
                // E("", "判断");
                // S();
                if (billIsExist)
                {
                    rs = UpdateHeadInfo(info);
                    //  FileHelper.WriteLog("更新 BILL_NO:" + info.BILL_NO + (rs > 0 ? "成功" : "失败"));

                }
                else
                {
                    rs = InsertHeadInfo(info);
                    //   FileHelper.WriteLog("新增 BILL_NO:" + info.BILL_NO + (rs > 0 ? "成功" : "失败"));
                }
                //   E("", "数据库存储");
            }
            catch (Exception ex)
            {
                FileHelper.WriteLog("发生错误！ BILL_NO:" + info.BILL_NO + "\r\n错误原因：" + ex.ToString());

            }

            return rs;
        }

        private void MoveFileToPath(FileInfo fileInfo, bool isMoveToBackUp)
        {
            try
            {
                string path = isMoveToBackUp ? SavePath : FailPath;
                string backupPath = path.Trim('\\') + "\\" + DateTime.Now.ToString("yyyyMMdd");
                CheckPathExist(backupPath);
                string backupFile = backupPath + "\\" + fileInfo.Name;
                if (File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }
                FileHelper.WriteLog(fileInfo.FullName + "   ====》  " + backupFile);
                fileInfo.MoveTo(backupFile);

                //   FileHelper.WriteLog(fileInfo.Name + (isMoveToBackUp ? " 移动到备份文件夹" : "移动到失败文件夹"));
            }
            catch (Exception ex)
            {

                Console.WriteLine((fileInfo == null ? "未知文件" : fileInfo.Name) + "移动失败,原因:" + ex.Message);
                Loger.LogMessage(ex.ToString());
            }


        }

        private string MoveFileToPath(FileInfo fileInfo, string path)
        {
            CheckPathExist(path);
            string descFile = Path.Combine(path, fileInfo.Name);
            if (File.Exists(descFile))
            {
                File.Delete(descFile);
            }
            FileHelper.WriteLog(fileInfo.FullName + "   ====》  " + descFile);
            fileInfo.MoveTo(descFile);

            return descFile;
        }

        private string CopyFileToPath(string fileName, string path)
        {
            var fileInfo = new FileInfo(fileName);
            CheckPathExist(path);
            string descFile = Path.Combine(path, fileInfo.Name);
            if (File.Exists(descFile))
            {
                File.Delete(descFile);
            }
            FileHelper.WriteLog(fileInfo.FullName + "   ====》  " + descFile);
            fileInfo.CopyTo(descFile);

            return descFile;
        }



        private void CheckPathExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private int InsertHeadInfo(EHS_ENTRY_HEAD info)
        {
            Hashtable ht = CreateHtParam(info);
            return InsertTable(ht);
        }

        private int UpdateHeadInfo(EHS_ENTRY_HEAD info)
        {
            Hashtable ht = CreateHtParam(info);
            return UpdateTable(ht);

        }

        private Hashtable CreateHtParam(EHS_ENTRY_HEAD info)
        {
            Hashtable ht = new Hashtable();
            ht["SHIP_ID"] = info.SHIP_ID;
            ht["VOYAGE_NO"] = info.VOYAGE_NO;
            ht["BILL_NO"] = info.BILL_NO;
            ht["I_E_FLAG"] = info.I_E_FLAG;
            ht["I_E_PORT"] = info.I_E_PORT;
            ht["L_D_PORT"] = info.L_D_PORT;
            ht["TRAF_NAME"] = info.TRAF_NAME;
            ht["SEND_NAME"] = info.SEND_NAME;
            ht["OWNER_NAME"] = info.OWNER_NAME;
            ht["SEND_COUNTRY"] = info.SEND_COUNTRY;
            ht["SEND_CITY"] = info.SEND_CITY;
            ht["TRADE_CODE"] = info.TRADE_CODE;
            ht["TRADE_NAME"] = info.TRADE_NAME;
            ht["MAIN_G_NAME"] = info.MAIN_G_NAME;
            ht["PACK_NO"] = info.PACK_NO;
            ht["GROSS_WT"] = info.GROSS_WT;
            ht["TOTAL_VALUE"] = info.TOTAL_VALUE;
            ht["CURR_CODE"] = info.CURR_CODE;
            ht["I_E_DATE"] = info.I_E_DATE;
            ht["D_DATE"] = info.D_DATE;
            ht["ENTRY_TYPE"] = info.ENTRY_TYPE;
            ht["DEC_TYPE"] = info.DEC_TYPE;
            ht["DEC_ER"] = info.DEC_ER;
            ht["READ_DATE"] = info.READ_DATE;
            ht["DEC_DATE"] = info.DEC_DATE;
            ht["M_CHECKH"] = info.CHK_MARK;
            ht["READ_FLAG"] = "0";
            return ht;
        }

        private string ChangeDecType(string trade_code, string dec_type)
        {
            string new_Dec_Type = "";
            GetDecTranDic();
            if (ChangeDecTypeForDefConn == "1")
            {
                new_Dec_Type = getNewDecType(dec_type);
            }

            if (string.IsNullOrEmpty(new_Dec_Type))
            {
                new_Dec_Type = dec_type;
            }

            return new_Dec_Type;
        }

        //private string ChangeDecType(string trade_code, string dec_type)
        //{
        //    string new_Dec_Type = "";
        //    GetConnList();
        //    GetDecTranDic();
        //    ConnInfo info = GetConnInfo(trade_code);

        //    if (info != null)
        //    {
        //        if (info.IsChangeDecType == 1)
        //        {
        //            new_Dec_Type = getNewDecType(dec_type);
        //        }
        //    }
        //    else
        //    {
        //        if (ChangeDecTypeForDefConn == "1")
        //        {
        //            new_Dec_Type = getNewDecType(dec_type);
        //        }
        //    }
        //    if (new_Dec_Type == null)
        //    {
        //        new_Dec_Type = dec_type;
        //    }

        //    return new_Dec_Type;
        //}

        private string getNewDecType(string old_dec_type)
        {
            foreach (string key in DecTranDic.Keys)
            {
                if (key == old_dec_type)
                {
                    return DecTranDic[key].ToString();
                }
            }
            return null;
        }

        private void GetDecTranDic()
        {
            if (DecTranDic == null)
            {
                string[] list = ChangeDecTypeRule.Split(',');
                DecTranDic = new Hashtable();
                foreach (string item in list)
                {
                    string[] sp = item.Split('-');
                    DecTranDic[sp[0]] = sp[1];
                }
            }

        }

        //private ConnInfo GetConnInfo(string trade_code)
        //{
        //    if (ConnList != null)
        //    {
        //        foreach (ConnInfo item in ConnList)
        //        {
        //            if (item.Trade_Code == trade_code)
        //            {
        //                NewConnString = item.ConnString;
        //                NewTable = item.Table;
        //                return item;
        //            }
        //        }
        //    }
        //    NewConnString = null;
        //    NewTable = LocalHead;
        //    return null;
        //}

        //private void GetConnList()
        //{
        //    if (ConnList == null)
        //    {
        //        ConnSectionGroup group = (ConnSectionGroup)ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).SectionGroups["ConnGroup"];
        //        ConnsSection simple = group.ConnStringList;
        //        ConnList = simple.conns;
        //    }
        //}


        IList<SqlParameter> paramList;
        private int UpdateTable(Hashtable ht)
        {
            //string NewTable = GetNewTable(ht);
            if (UpdateSql == "")
            {
                StringBuilder sb = new StringBuilder();
                foreach (DictionaryEntry item in ht)
                {
                    sb.AppendFormat(" {0} = @{0},", item.Key);
                }
                UpdateSql = string.Format("update {0} set {1} where BILL_NO=@BILL_NO", LocalHead, sb.ToString().Trim(','));
            }
            // string NewUpdateSql = UpdateSql.Replace("newtable", NewTable);
            if (paramList == null || paramList.Count <= 0)
            {
                paramList = ConvertToListParam(ht);
            }
            else
            {
                foreach (SqlParameter parm in paramList)
                {
                    parm.Value = ht[parm.ParameterName.Substring(1)];
                }
            }
            // DbHelperSQL.CreateConn(NewConnString);
            return DbHelperSQL.ExecuteSql(UpdateSql, paramList);
        }

        //private string GetNewTable(Hashtable ht)
        //{
        //    if (string.IsNullOrEmpty(FilterTradeCode))
        //    {
        //        return LocalHead;
        //    }
        //    string[] codeList = FilterTradeCode.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string item in codeList)
        //    {
        //        if (item == ht["TRADE_CODE"].ToString())
        //        {
        //            ht["BILL_NO"] = ENHelper.RSAEncrypt(ht["BILL_NO"].ToString());
        //            return FilterTable;
        //        }
        //    }

        //    return LocalHead;
        //}


        private int InsertTable(Hashtable ht)
        {
            //  string NewTable = GetNewTable(ht);
            if (InsertSql == "")
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sbVal = new StringBuilder();
                foreach (DictionaryEntry item in ht)
                {
                    sb.AppendFormat("{0},", item.Key);
                    sbVal.AppendFormat("@{0},", item.Key);
                }

                InsertSql = string.Format("insert into {0}({1}) values({2})", LocalHead, sb.ToString().Trim(','), sbVal.ToString().Trim(','));
            }
            //    string NewInsertSql = InsertSql.Replace("newtable", NewTable);
            IList<SqlParameter> paramList = ConvertToListParam(ht);
            //  DbHelperSQL.CreateConn(NewConnString);
            return DbHelperSQL.ExecuteSql(InsertSql, paramList);
        }

        private bool CheckBillExist(string billno)
        {
            SqlParameter[] ps = new SqlParameter[]{
                 new SqlParameter("@bill_no",billno)
            };
            // DbHelperSQL.CreateConn(NewConnString);
            return DbHelperSQL.Exists(string.Format(CheckBillSql, LocalHead), ps);
        }

        private IList<SqlParameter> ConvertToListParam(Hashtable HTParamList)
        {
            if (HTParamList == null || HTParamList.Count < 1)
            {
                return null;
            }
            IList<SqlParameter> oracleParamList = new List<SqlParameter>();
            IDictionaryEnumerator enumerator = HTParamList.GetEnumerator();
            string keyStr = "";
            while (enumerator.MoveNext())
            {
                keyStr = enumerator.Key.ToString().Trim();
                if (keyStr.IndexOf("@") != 0)
                {
                    keyStr = "@" + keyStr;
                }
                oracleParamList.Add(new SqlParameter(keyStr, enumerator.Value));
            }


            return oracleParamList;
        }

        //public int DeBillNO()
        //{
        //    string sql = string.Format("update {0} set bill_no =@bill_no where bill_no=@oldbill_no", FilterTable);
        //    DataSet ds = DbHelperSQL.Query(string.Format("select bill_no from {0}", FilterTable));
        //    int rs = 0;
        //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        string deBill_no = "";

        //        foreach (DataRow item in ds.Tables[0].Rows)
        //        {
        //            deBill_no = ENHelper.RSADecrypt(item["bill_no"].ToString());
        //            SqlParameter[] ps = new SqlParameter[]{
        //                new SqlParameter("@bill_no",deBill_no),
        //                new SqlParameter("@oldbill_no",item["bill_no"].ToString())
        //          };
        //            rs += DbHelperSQL.ExecuteSql(sql, ps);
        //        }
        //    }
        //    return rs;
        //}

    }
}
