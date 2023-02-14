using APIStarportGE.Controllers;

//Created by Alexander Fields 
using APILoggingTests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optimization.Objects.Logging;
using MongoDB.Bson.IO;
using System.Collections.Generic;

namespace APIStarportGE.Controllers.Tests
{
    [TestClass()]
    public class LoggingControllerTests : ControllerBase
    {
        [TestMethod()]
        public void PostLogAzureTestSuccess()
        {
            new SettingsTests().CreateSettings();

            var controller = new LoggingController();
            LogMessage.MessageSourceSetter = "APILoggingTests";
            List<LogMessage> logMessages = new List<LogMessage>
            {
                new LogMessage(0, System.DateTime.Now, "Test Error", MessageType.Error, "Test"),
                new LogMessage(1, System.DateTime.Now, "Test Message", MessageType.Message, "Test"),
                new LogMessage(2, System.DateTime.Now, "Test Info", MessageType.Informational, "C:\\Temp\\06864W-ChainMerchantDifference.csv successfully uploaded!"),
                new LogMessage(3, System.DateTime.Now, "Test Warning", MessageType.Warning, "Test"),
                //new LogMessage(5, System.DateTime.Now, "Test Crit", MessageType.Critical, "Crit d20"),
                new LogMessage(4, System.DateTime.Now, "Test Success", MessageType.Success, "Test")
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(logMessages);
       
            IActionResult result = controller.PostLogAzure(logMessages);

            Assert.IsTrue(result != null);
        }

        [TestMethod()]
        public void PostLogTestSuccess()
        {
            new SettingsTests().CreateSettings();

            var controller = new LoggingController();
            LogMessage.MessageSourceSetter = "APILoggingTests";
            for (int i = 0; i < 12; i++)
            {


                List<LogMessage> logMessages = new List<LogMessage>
            {
                new LogMessage(0, System.DateTime.Now, "Test Error", MessageType.Error, "Test"),
                new LogMessage(1, System.DateTime.Now, "Test Message", MessageType.Message, "Test"),
                new LogMessage(2, System.DateTime.Now, "Test Info", MessageType.Informational, "Test"),
                new LogMessage(3, System.DateTime.Now, "Test Warning", MessageType.Warning, "Test"),
                new LogMessage(5, System.DateTime.Now, "Test Crit", MessageType.Critical, "Crit d20"),
                new LogMessage(4, System.DateTime.Now, "Test Success", MessageType.Success, "Test")
            };

                IActionResult result = controller.PostLog(logMessages);
            }

            //Assert.IsTrue(result != null);
        }

        [TestMethod()]
        public void LoggingControllerTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PingTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostLogAzureTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostLogAzureTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostLogTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostLogTest1()
        {
            Assert.Fail();
        }
    }
}