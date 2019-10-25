using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using TS.TranCore;

namespace ServerTran
{
    class Program
    {
        //[DllImport("User32.dll", EntryPoint = "FindWindow")]
        //private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //[DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
        //private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        //[DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
        //private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        //[DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
        //private static extern bool ShowWindow(IntPtr hWnd, int type);

        //const string Id = "af49d266-e4f4-4a63-b73c-f62c1144b584";
        static void Main(string[] args)
        {

            //bool thisInstance;
            //using (var semaphore = new Semaphore(0, 1, Id, out thisInstance))
            //{
            //    if (thisInstance)
            //    {
            //        Console.WriteLine("This is the first instance!");
            //        Console.ReadLine();

            //        // Release resource
            //        semaphore.Release();
            //    }
            //    else
            //    {
            //        Console.WriteLine("There is another instance running.");

            //        // Wait for resource
            //        semaphore.WaitOne();
            //        Console.WriteLine("There is now the only instance.");
            //        Console.ReadLine();
            //    }
            //}

            //Console.Title = "MyConsoleApp";
            //IntPtr ParenthWnd = new IntPtr(0);
            //IntPtr et = new IntPtr(0);
            //ParenthWnd = FindWindow(null, "MyConsoleApp");

            //ShowWindow(ParenthWnd, 2);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
            Console.WriteLine("程序版本：2016-11-09");
            Console.WriteLine("请勿关闭此窗口，谢谢：）");

            HZAction action = new HZAction();
            action.BeginRun();
            Console.ReadLine();
        }
    }
}
