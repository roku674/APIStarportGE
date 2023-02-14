
//Created by Alexander Fields 

using System.Linq;
using System.Text;

namespace Optimization.Encryption
{
    /// <summary>
    /// Rivest–Shamir–Adleman crypto algorithm
    /// </summary>
    public class RSA
    {
        /// <summary>
        /// strings to hold key
        /// </summary>
        private string publicKey, privateKey;

        private UnicodeEncoding encoder = new UnicodeEncoding();

        /// <summary>
        /// RSA Constructor
        /// </summary>
        public RSA()
        {
            System.Security.Cryptography.RSACryptoServiceProvider myRSA = new System.Security.Cryptography.RSACryptoServiceProvider();
            privateKey = myRSA.ToXmlString(true);
            publicKey = myRSA.ToXmlString(false);
        }

        /// <summary>
        /// Pass in the password to encrypt and it returns an array with password publicKey and pk
        /// </summary>
        /// <param name="text">Text to encrypt</param>
        /// <returns></returns>
        public string[] Encrypt(string text)
        {
            System.Security.Cryptography.RSACryptoServiceProvider myRSA = new System.Security.Cryptography.RSACryptoServiceProvider();
            //Set up the cryptoServiceProvider with the proper key
            myRSA.FromXmlString(publicKey);
            //Encode the data to encrypt as a byte array
            byte[] dataToEncrypt = encoder.GetBytes(text);

            //Encrypt the byte array
            byte[] encryptedByteArray = myRSA.Encrypt(dataToEncrypt, false).ToArray();

            int length = encryptedByteArray.Count();
            int item = 0;
            StringBuilder sb = new StringBuilder();

            //Change each byte in the encrypted byte array to text

            foreach (byte x in encryptedByteArray)
            {
                item++;
                sb.Append(x);
                if (item < length) sb.Append(",");
            }
            text = sb.ToString();

            string[] stringArr = new string[3];
            stringArr[0] = text;
            stringArr[1] = publicKey;
            stringArr[2] = privateKey;

            return stringArr;
        }

        /// <summary>
        /// Decrypts the string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public string Decrypt(string text, string privateKey)
        {
            System.Security.Cryptography.RSACryptoServiceProvider myRSA = new System.Security.Cryptography.RSACryptoServiceProvider();
            //Split the data into an array
            string[] dataArray = text.Split(new char[] { ',' });

            //Convert to bytes
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++) dataByte[i] = System.Convert.ToByte(dataArray[i]);

            //Decrypt the byte array
            myRSA.FromXmlString(privateKey);
            byte[] decryptedBytes = myRSA.Decrypt(dataByte, false);

            //Place into text
            text = encoder.GetString(decryptedBytes);

            return text;
        }
    }
}