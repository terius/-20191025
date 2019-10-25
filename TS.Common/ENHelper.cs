using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TS.Common
{
    public class ENHelper
    {
        #region RSA加解密
        //加密算法  
        public static string RSAEncrypt(string encryptString)
        {
            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "teriusyouareveryniubihahaha";

            RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider(csp);
            byte[] encryptBytes = RSAProvider.Encrypt(ASCIIEncoding.ASCII.GetBytes(encryptString), true);
            string str = "";
            foreach (byte b in encryptBytes)
            {
                str = str + string.Format("{0:x2}", b);
            }
            return str;
        }


        //解密算法  
        public static string RSADecrypt(string decryptString)
        {
            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "teriusyouareveryniubihahaha";
            RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider(csp);
            int length = (decryptString.Length / 2);
            byte[] decryptBytes = new byte[length];
            for (int index = 0; index < length; index++)
            {
                string substring = decryptString.Substring(index * 2, 2);
                decryptBytes[index] = Convert.ToByte(substring, 16);
            }
            decryptBytes = RSAProvider.Decrypt(decryptBytes, true);
            return ASCIIEncoding.ASCII.GetString(decryptBytes);
        }

        #endregion
    }
}
