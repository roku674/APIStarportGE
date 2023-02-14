
//Created by Alexander Fields 

using Optimization.Objects.Logging;
using Optimization.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace OptimizedPaymentsTests.Objects
{
    public class MockObjects
    {
        public DataTable CreateRandomDataTable() {
            DataTable dataTable = new DataTable();

            System.Random random = new System.Random();
            int randomAmt = random.Next(1,100);

            for (int i = 0; i < randomAmt; i++)
            {
                dataTable.Columns.Add(new DataColumn("Column" + i));
            }
            for(int i =0; i < dataTable.Columns.Count; i++)
            {
                DataRow row = dataTable.NewRow();
                object[] ints = new object[dataTable.Columns.Count];
                for (int j = 0;j < ints.Length; j++)
                {
                    ints[j] = random.Next();
                }
                row.ItemArray = ints;
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public List<LogMessage> CreateLogMessages()
        {
            List<LogMessage> messages = new List<LogMessage>
            {
                new LogMessage(0, DateTime.Now, "Test", MessageType.Message, "Test"),
                new LogMessage(0, DateTime.Now, "Test", MessageType.Message, "Test2"),
                new LogMessage(0, DateTime.Now, "Test", MessageType.Message, "Test3"),
                new LogMessage(0, DateTime.Now, "Test", MessageType.Message, "Test4"),
                new LogMessage(0, DateTime.Now, "Test", MessageType.Message, "Test5")
            };

            return messages;
        }

        public DataTable CreateLogMessagesDt()
        {
            return Utility.ConvertListToDataTable(CreateLogMessages());
        }
    }
}
