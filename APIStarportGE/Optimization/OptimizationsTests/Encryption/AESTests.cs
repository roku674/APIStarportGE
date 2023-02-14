
//Created by Alexander Fields 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Optimization.Encryption.Tests
{
    [TestClass()]
    public class AESTests
    {
        [TestMethod()]
        public void DecryptStringTest()
        {
            string key = Utility.Utility.GenerateString(32);
            string encryptedString = AES.EncryptString(key, "Test");

            string decryptedString = AES.DecryptString(key, encryptedString);

            Assert.AreEqual(decryptedString, "Test");
        }

        [TestMethod()]
        public void EncryptStringTest()
        {
            string key = Utility.Utility.GenerateString(32);
            string encryptedString = AES.EncryptString(key, "Test");

            Assert.AreNotEqual(encryptedString, "Test");
        }

        [TestMethod()]
        public void GenerateKeyTest()
        {
            string key = Utility.Utility.GenerateString(32);
            Assert.IsNotNull(key);
        }
    }
}