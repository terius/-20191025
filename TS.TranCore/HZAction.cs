using TS.Common;
using TS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using System.Text;
using System.Threading;
using System.Net;


namespace TS.TranCore
{
    public class HZAction
    {
        DataAction2 da = new DataAction2();
        private volatile bool isRun = false;
        private readonly int LoopTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LoopTime"]);
        private readonly string loginUser = System.Configuration.ConfigurationManager.AppSettings["RemotePathLoginName"];
        private readonly string loginPassword = System.Configuration.ConfigurationManager.AppSettings["RemotePathPassword"];
        private readonly string HGPath = System.Configuration.ConfigurationManager.AppSettings["HGPath"];

        public void BeginRun()
        {
            //if (CheckTimeOut())
            //{
            //    isRun = false;
            //    Loger.LogMessage("授权已过期，请联系服务厂商");
            //    throw new TimeoutException("授权已过期，请联系服务厂商");
            //}
            FileHelper.WriteLog("-------------------------服务开始启动---------------------");
            isRun = true;
            CreateNetworkShare(HGPath);
            ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdateData));//更新数据
        }


        private void CreateNetworkShare(string ntShare)
        {
            try
            {
                NetworkCredential theNetworkCredential = new NetworkCredential(loginUser, loginPassword);
                Uri uri = new Uri(ntShare);
                CredentialCache theNetCache = new CredentialCache();
                theNetCache.Add(uri, "Basic", theNetworkCredential);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private bool CheckTimeOut()
        {
            DateTime timeEnd = DateTime.Parse("2016-07-25");
            DateTime dtNow = da.GetDBTime();
            if (dtNow >= timeEnd)
            {
                return true;
            }
            return false;
        }

        readonly object obUpdateData = new object();
        private void UpdateData(Object stateInfo)
        {

            lock (obUpdateData)
            {
                int i = 0;
                while (isRun)
                {
                    try
                    {
                        da.ReadFiles();
                    }
                    catch (Exception ex)
                    {
                        if (i <= 100)
                        {
                            Loger.LogMessage(ex);
                            i++;
                        }

                    }
                    Thread.Sleep(LoopTime * 1000);
                }
            }
        }



        public void EndRun()
        {
            FileHelper.WriteLog("------------------------------服务中止！-------------------------------\r\n\r\n\r\n");
            isRun = false;
        }
    }
}
