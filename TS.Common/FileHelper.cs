﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;


namespace TS.Common
{
    public class FileHelper
    {
        const int LOCK = 500; //申请读写时间
        const int SLEEP = 10; //线程挂起时间
        static ReaderWriterLock readWriteLock = new ReaderWriterLock();
        private static readonly int SaveLog = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SaveLog"]);


        

        public static void WriteLog(string msg) //写入文件
        {
            if (SaveLog != 1)
            {
                return;
            }
            readWriteLock.AcquireWriterLock(LOCK);
            try
            {

                string path = AppDomain.CurrentDomain.BaseDirectory + "Actionlogs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                if (!File.Exists(path))
                {
                    FileStream fs1 = File.Create(path);
                    fs1.Close();
                    Thread.Sleep(10);
                }

                using (StreamWriter sw = new StreamWriter(path, true, Encoding.Default))
                {
                    sw.WriteLine("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】      " + msg
                        + "\r\n---------------------------------------------------------------\r\n");
                    sw.Flush();
                    sw.Close();
                }
                Thread.Sleep(SLEEP);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                readWriteLock.ReleaseWriterLock();
            }
        }


        public static void CreateNetworkShare(string ntShare, string loginUser, string loginPassword)
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
    }
}
