using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using TS.Common;

namespace EncryptConn
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请选择操作类型： 1----加密     2----解密");
            string type = "";
            while (true)
            {
                type = Console.ReadLine();
                if (type == "1" || type == "2")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("操作错误，请输入1或者2");
                }
            }

            if (type == "1")
            {
                Console.WriteLine("开始加密数据库连接串");
                EncryptConnection.EncryptConnectionString(true);
                Console.WriteLine("加密成功！");
            }
            else if (type == "2")
            {
                Console.WriteLine("开始解密数据库连接串");
                EncryptConnection.EncryptConnectionString(false);
                Console.WriteLine("解密成功！");
            }
            else
            {
                Console.WriteLine("操作错误，请输入1或者2");
            }
            Console.ReadLine();
        }
    }
}
