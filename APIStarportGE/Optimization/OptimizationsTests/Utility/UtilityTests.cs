
//Created by Alexander Fields 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optimization.Objects.Data;
using Optimization.Objects.Logging;
using OptimizedPaymentsTests.Objects;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Optimization.Utility.Tests
{
    [TestClass()]
    public class UtilityTests
    {
        [TestMethod()]
        public void ConvertDataTableToHTMLTest()
        {
            MockObjects mockObjects = new MockObjects();
            DataTable dataTable = mockObjects.CreateRandomDataTable();

            string result = Utility.ConvertDataTableToHTML(dataTable, 0, 0, 0, null);

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void ConvertDataTableToListTest()
        {
            MockObjects mockObjects = new MockObjects();
            DataTable dataTable = Utility.ConvertCSVtoDataTable("C:\\Users\\Alexander.DESKTOP-DSA8VMT\\Documents\\totalImported.csv");

            List<TotalImports> result = Utility.ConvertDataTableToList<TotalImports>(dataTable);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void ConvertDataTableToListTestFast()
        {
            MockObjects mockObjects = new MockObjects();
            DataTable dataTable = mockObjects.CreateLogMessagesDt();

            List<LogMessage> result = Utility.ConvertDataTableToListFast<LogMessage>(dataTable, 100);

            foreach(LogMessage logMessage in result)
            {
                System.Console.WriteLine(logMessage.Message);
            }

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void ConvertListToDataTableTest()
        {
            List<int> ints = new List<int>();
            System.Random random = new System.Random();
            int randomAmt = random.Next(0, 100);

            for (int i = 0; i < randomAmt; i++)
            {
                ints.Add(random.Next());
            }

            DataTable result = Utility.ConvertListToDataTable<int>(ints);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void CreateCsvFromDataTableTest()
        {
            MockObjects mockObjects = new MockObjects();
            DataTable dataTable = mockObjects.CreateRandomDataTable();

            string testCsvPath = Directory.GetCurrentDirectory() + "/createcsvtest.csv";
            Utility.CreateCsvFromDataTable(dataTable, testCsvPath);
            Utility.CreateCsvFromDataTableNoHeap(dataTable, testCsvPath);
            string csv = File.ReadAllText(testCsvPath);
            Assert.IsNotNull(csv);
            File.Delete(testCsvPath);
        }

        [TestMethod()]
        public void ConvertCSVtoDataTableTest()
        {
            MockObjects mockObjects = new MockObjects();
            DataTable dataTable = mockObjects.CreateRandomDataTable();

            string testCsvPath = Directory.GetCurrentDirectory() + "/convertcsvtest.csv";
            Utility.CreateCsvFromDataTable(dataTable, testCsvPath);

            DataTable dataTableFromCsv = Utility.ConvertCSVtoDataTable(testCsvPath);

            Assert.IsNotNull(dataTableFromCsv);
            //Assert.AreEqual(dataTable, dataTableFromCsv);
            File.Delete(testCsvPath);
        }

        [TestMethod()]
        public void DecodeStringTest()
        {
            string result = Utility.DecodeString("VGVzdA==");
            Assert.AreEqual(result, "Test");
        }

        [TestMethod()]
        public void EncodeStringTest()
        {
           string result = Utility.EncodeString("Test");
           Assert.AreEqual("VGVzdA==", result);
        }

        [TestMethod()]
        public void GetObjectFromDataRowTest()
        {
            MockObjects mockObjects = new MockObjects();
            DataTable dataTable = mockObjects.CreateRandomDataTable();

            int num = Utility.GetObjectFromDataRow<int>(dataTable.Rows[0]);
        }

        [TestMethod()]
        public void GetReadableTimeStringTest()
        {
            string time = Utility.GetReadableTimeString(new System.TimeSpan());
            Assert.IsNotNull(time);
        }

        [TestMethod()]
        public void IfStringContainsOneOfTrueTest()
        {
            string[] strings =
            {
                "t", "e"
            };
            bool result = Utility.IfStringContainsOneOf("test", "t,e", ',');
            bool result2 = Utility.IfStringContainsOneOf("test", strings);
            bool result3 = Utility.IfStringContainsOneOf("test", strings.ToList());

            Assert.IsTrue(result);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);


        }

        [TestMethod()]
        public void IfStringContainsOneOfFalseTest()
        {
            string[] strings =
            {
                "d", "f"
            };
            bool result = Utility.IfStringContainsOneOf("test", "d,f", ',');
            bool result2 = Utility.IfStringContainsOneOf("test", strings);
            bool result3 = Utility.IfStringContainsOneOf("test", strings.ToList());

            Assert.IsFalse(result);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
        }

        [TestMethod()]
        public void IsBase64StringTest()
        {
            bool result = Utility.IsBase64String("VGVzdA==");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void WhereContainsTest()
        {
            string[] strings =
            {
                "T", "E"
            };
            IEnumerable<string> result = Utility.WhereContains("test", strings, true);
            IEnumerable<string> result2 = Utility.WhereContains("test", strings.ToList(), true);

            IEnumerable<string> result3 = Utility.WhereContains("test", strings, false);
            IEnumerable<string> result4 = Utility.WhereContains("test", strings.ToList(), false);


            Assert.AreEqual("T", result.ToList().FirstOrDefault());
            Assert.AreEqual("T", result2.ToList().FirstOrDefault());

            Assert.IsTrue(result3.ToList().Count == 0);
            Assert.IsTrue(result4.ToList().Count == 0);

        }     
    }
}