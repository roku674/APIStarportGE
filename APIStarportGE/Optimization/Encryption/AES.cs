
//Created by Alexander Fields 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Optimization.Encryption
{
    /// <summary>
    /// The key must be of size 16, 24, or 32 if not it will assign a crappy key which I dont recommend using for anything important
    /// </summary>
    public class AES
    {
        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cipherText">Must be a base64 String</param>
        /// <returns>decryped base 64 string</returns>
        public static string DecryptString(string key, string cipherText)
        {
            key = ResizeKey(key);
            byte[] iv = new byte[16];
            byte[] buffer = System.Convert.FromBase64String(cipherText);
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
                return "Failed to Decrypt";
            }
        }

        /// <summary>
        /// Creates a base 64 string using the text to encrypt and a key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="plainText"></param>
        /// <returns>Base64 string</returns>
        public static string EncryptString(string key, string plainText)
        {
            key = ResizeKey(key);
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return System.Convert.ToBase64String(array);
        }

        private static string ResizeKey(string key)
        {
            string crappyKey = "TheyWillNeverFindThisKey";

            if (key == null)
            {
                return crappyKey;
            }
            else if (key.Length > 32)
            {
                return key.Substring(0, 32);
            }
            else if (key.Length > 24 && key.Length < 32)
            {
                return key.Substring(0, 24);
            }
            else if (key.Length > 16 && key.Length < 24)
            {
                return key.Substring(0, 16);
            }
            else if (key.Length < 16)
            {
                return crappyKey;
            }
            return key; //should only return if no change (32,24,16)
        }
    }
}