using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;

namespace EMS_Security
{
    public static class Encryption
    {
        private static readonly byte[] key = Encoding.Default.GetBytes("FD2Se#3$32r2asQ4#($sjcaqD&Z8A*(F");
        private static readonly byte[] IV = Encoding.Default.GetBytes("sda&@!31433Qad$#");

        public static string Encrypt(string plainText)
        {
            byte[] bytes = EncryptToBytes(plainText);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static byte[] EncryptToBytes(string plainText)
        {
            byte[] encryptedArray;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV;

                ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream stream = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(plainText);
                        }
                    }
                    encryptedArray = ms.ToArray();
                }
            }

            return encryptedArray;
        }


        public static string Decrypt(string cipherText)
        {
            int cipherLength = cipherText.Length;
            byte[] plainText = new byte[cipherLength / 2];
            for (int i = 0; i < cipherLength; i = i + 2)
            {
                plainText[i / 2] = Convert.ToByte(cipherText.Substring(i, 2), 16);
            }

            return DecryptBytes(plainText);
        }


        public static string DecryptBytes(byte[] cipherText)
        {
            string plainText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV;

                ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    using (CryptoStream stream = new CryptoStream(ms, transform, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            plainText = reader.ReadToEnd();
                        }
                    }
                }
            }

            return plainText;
        }
        
        //public static void EncryptConnectionString()
        //{
        //    string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        //    Regex regex = new Regex("[^0-9A-Z]");

        //    if (!regex.Match(connStr).Success)
        //    {
        //        connStr = Encrypt(connStr);
        //        ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["connStr"];

        //        FieldInfo info = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

        //        info.SetValue(settings, false);

        //        settings.ConnectionString = connStr;
        //    }

        //}
    }
}
