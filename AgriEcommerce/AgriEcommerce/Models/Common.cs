using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AgriEcommerce.Models
{
    public class Common
    {
        public static string ToMD5(string pwd)
        {
            string result = string.Empty;
            byte[] buffer = Encoding.UTF8.GetBytes(pwd);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            buffer = md5.ComputeHash(buffer);
            foreach (var item in buffer)
            {
                result += item.ToString("x2");
            }
            return result;
        }
    }
}