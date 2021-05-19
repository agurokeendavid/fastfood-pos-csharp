﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastFoodPOS
{
    class Util
    {
        public static string GetConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetHashSHA256(string text)
        {
            SHA256Managed hashstring = new SHA256Managed();
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            byte[] hash = hashstring.ComputeHash(bytes);
            string result = string.Empty;
            foreach (byte x in hash)
            {
                result += String.Format("{0:x2}", x);
            }
            return result;
        }

        public static bool VerifyHash(string text, string hashedtext)
        {
            return GetHashSHA256(text).Equals(hashedtext);
        }

        public static string CopyImage(string source, string filename)
        {
            CreateImageDirectory();
            filename = Path.GetFileName(filename);
            string destination = Path.Combine("images", filename);
            if (File.Exists(destination))
            {
                File.Replace(source, destination, destination+".bak");
            }
            else
            {
                File.Copy(source, destination);
            }
            return destination;
        }

        public static string GetFullPath(string path)
        {
            return Path.Combine(Application.StartupPath, path);
        }

        private static void CreateImageDirectory()
        {
            if(!Directory.Exists("images")){
                Directory.CreateDirectory("images");
            }
        }

        public static bool IsEmail(string text)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(text);
                return addr.Address == text;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
