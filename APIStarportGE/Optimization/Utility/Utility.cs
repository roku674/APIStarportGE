
//Created by Alexander Fields 
using Microsoft.AspNetCore.Http;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Optimization.Utility
{
    /// <summary>
    /// Class with reusable code
    /// </summary>
    public static class Utility
    {
        //public code section

        /// <summary>
        /// Converts System.DataTable to Html Table with
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="cellSpacing">default at 0</param>
        /// <param name="cellPadding">default at 0</param>
        /// <param name="border">default at 0</param>
        /// <param name="backgroundColor">if null or empty defaults to white</param>
        /// <returns>string that will appear as a table in HTML</returns>
        public static string ConvertDataTableToHTML(DataTable dataTable, int cellSpacing, int cellPadding, int border, string backgroundColor)
        {
            if (string.IsNullOrWhiteSpace(backgroundColor))
            {
                backgroundColor = "#ffffff";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<table cellspacing={cellSpacing} cellpadding={cellPadding} border={border} bgcolor={backgroundColor}>");

            //add header row
            sb.AppendLine("<tr>");
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                sb.AppendLine($"<td>{dataTable.Columns[i].ColumnName}</td>");
            }
            sb.AppendLine("</tr>");

            //add rows
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                sb.AppendLine("<tr>");
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    sb.AppendLine($"<td>{dataTable.Rows[i][j]}</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// Generic converter for DataTable to List of Objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns>Generic List</returns>
        public static List<T> ConvertDataTableToList<T>(DataTable dataTable)
        {
            List<T> data = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                T item = GetObjectFromDataRow<T>(row);
                data.Add(item);
            }
            return data;
        }

        /// <summary>
        /// Generic converter for DataTable to List of Objects This is faster, but may construct the list in a different order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <param name="rowsAtOnce">amount of rows at once you'd like to convert if you do more than 500-100 you're asking for problems</param>
        /// <returns>Generic List</returns>
        public static List<T> ConvertDataTableToListFast<T>(DataTable dataTable, int rowsAtOnce)
        {
            List<T> data = new List<T>();

            List<Task<T>> pool = new List<Task<T>>();

            foreach (DataRow row in dataTable.Rows)
            {
                while (pool.Count >= rowsAtOnce)
                {
                    Task.WaitAll(pool.ToArray());

                    foreach (Task<T> tempTask in pool)
                    {
                        data.Add(tempTask.Result);
                    }

                    pool.RemoveAll(tempTask => tempTask.IsCompleted);
                }

                Task<T> task = Task.Run(() => GetObjectFromDataRow<T>(row));
                pool.Add(task);
            }

            Task.WaitAll(pool.ToArray());

            foreach(Task<T> task in pool)
            {
                data.Add(task.Result);
            }

            return data;
        }

        /// <summary>
        /// Converts a List of objects to a DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ConvertListToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        /// <summary>
        /// Converts a DataTable into a list of strings seperated by commas. The first row will be the headers
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns>List of strings representing each line in a csv file</returns>
        public static List<string> ConvertDataTableToListString(DataTable dataTable)
        {
            List<string> lines = new List<string>();

            string[] columnNames = dataTable.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();

            string header = string.Join(",", columnNames.Select(name => $"\"{name}\""));
            lines.Add(header);

            EnumerableRowCollection<string> valueLines = dataTable.AsEnumerable()
                .Select(row => string.Join(",", row.ItemArray.Select(val => $"\"{val}\"")));

            lines.AddRange(valueLines);

            return lines;
        }

        /// <summary>
        /// Converts a DataTable to a csv and writes it to the path given
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="csvPath"></param>
        public static void CreateCsvFromDataTable(DataTable dataTable, string csvPath)
        {
            StringBuilder csv = new StringBuilder();

            string[] columnNames = dataTable.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();

            string header = string.Join(",", columnNames.Select(name => $"\"{name}\""));
            csv.AppendLine(header);

            object[] rowValues = dataTable.AsEnumerable().Select(row => row.ItemArray).ToArray();
            foreach (object[] values in rowValues)
            {
                string line = string.Join(",", values.Select(val => $"\"{val}\""));
                csv.AppendLine(line);
            }

            System.IO.File.WriteAllText(csvPath, csv.ToString());
        }

        /// <summary>
        /// Converts a DataTable to a csv and writes it to the path gaven with little to no heap allocations
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="csvPath"></param>
        public static void CreateCsvFromDataTableNoHeap(DataTable dataTable, string csvPath)
        {
            StringBuilder csv = new StringBuilder();

            int columnCount = dataTable.Columns.Count;
            for (int i = 0; i < columnCount; i++)
            {
                csv.Append('"');
                csv.Append(dataTable.Columns[i].ColumnName);
                csv.Append('"');

                if (i < columnCount - 1)
                {
                    csv.Append(',');
                }
            }

            csv.AppendLine();

            int rowCount = dataTable.Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                DataRow row = dataTable.Rows[i];
                for (int j = 0; j < columnCount; j++)
                {
                    csv.Append('"');
                    csv.Append(row[j]);
                    csv.Append('"');

                    if (j < columnCount - 1)
                    {
                        csv.Append(',');
                    }
                }
                csv.AppendLine();
            }

            System.IO.File.WriteAllText(csvPath, csv.ToString());
        }

        /// <summary>
        /// Converts a DataTable to a csv and writes it to the path given with no headers
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="csvPath"></param>
        public static void CreateCsvFromDataTableNoHeaders(DataTable dataTable, string csvPath)
        {
            StringBuilder csv = new StringBuilder();
            
            object[] rowValues = dataTable.AsEnumerable().Select(row => row.ItemArray).ToArray();
            foreach (object[] values in rowValues)
            {
                string line = string.Join(",", values.Select(val => $"\"{val}\""));
                csv.AppendLine(line);
            }

            System.IO.File.WriteAllText(csvPath, csv.ToString());
        }

        /// <summary>
        /// Read a csv from the path given and turn it into a DataTable
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns>System.DataTable</returns>
        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dataTable = new DataTable();
            using (StreamReader streamReader = new StreamReader(strFilePath))
            {
                string[] headers = streamReader.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header);
                }
                while (!streamReader.EndOfStream)
                {
                    string[] rows = System.Text.RegularExpressions.Regex.Split(streamReader.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    DataRow dr = dataTable.NewRow();
                    for (int i = 0;i < headers.Length;i++)
                    {
                        string field = rows[i].Trim();
                        if (field.StartsWith("\"") && field.EndsWith("\""))
                        {
                            field = field.Substring(1, field.Length - 2);
                        }
                        dr[i] = field;
                    }
                    dataTable.Rows.Add(dr);
                }
            }
            return dataTable;
        }

        /// <summary>
        /// Decodes a string from base 64
        /// </summary>
        /// <param name="encodedString"></param>
        /// <returns>decoded string</returns>
        public static string DecodeString(string encodedString)
        {
            byte[] bytes = System.Convert.FromBase64String(encodedString);
            string decoded = System.Text.Encoding.ASCII.GetString(bytes);

            return decoded;
        }

        /// <summary>
        /// Encodes a string into base 64
        /// </summary>
        /// <param name="stringToEncode"></param>
        /// <returns>encoded string</returns>
        public static string EncodeString(string stringToEncode)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(stringToEncode);
            string encoded = System.Convert.ToBase64String(bytes);

            return encoded;
        }

        /// <summary>
        /// If the length is less than or equal to 0 then it will assign a random length between 7 and 255
        /// </summary>
        /// <param name="length">Length of the key</param>
        /// <returns>Base 62 string using the alphabet and numbers</returns>
        public static string GenerateString(int length)
        {
            System.Random random = new System.Random();

            string pool =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
               + "abcdefghijklmnopqrztuvwxyz"
               + "0123456789";

            if (length <= 0)
            {
                length = random.Next(7, 255);
            }

            IEnumerable<char> ieChars = Enumerable.Range(0, length)
                .Select(x => pool[random.Next(0, pool.Length)]);

            return new string(ieChars.ToArray());
        }

        /// <summary>
        /// Converts a DataRow to an Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <returns>Generic Object</returns>
        public static T GetObjectFromDataRow<T>(DataRow dataRow)
        {
            System.Type temp = typeof(T);
            T obj = System.Activator.CreateInstance<T>();

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                foreach (PropertyInfo prop in temp.GetProperties())
                {                 
                    if (prop.Name == column.ColumnName)
                    {                        
                        object convertedValue = Convert.ChangeType(dataRow[column.ColumnName], prop.PropertyType);
                        prop.SetValue(obj, convertedValue, null);                     
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Turns a Timespan into a 0h:0m:0s
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns>a nice string to read</returns>
        public static string GetReadableTimeString(System.TimeSpan timeSpan)
        {
            return timeSpan.Hours + "h:" + timeSpan.Minutes + "m:" + timeSpan.Seconds + "s";
        }

#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// if string contains it and is not null empty or whitespace (Regex is technically faster)
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="strings"></param>
        /// <returns>true if found</returns>
        public static bool IfStringContainsOneOf(string compare, string[] strings)
        {
            if (strings.Length == 0) return false;

            return strings.Any(s => compare.Contains(s));
        }

        /// <summary>
        /// if string contains it and is not null empty or whitespace (Regex is technically faster)
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="strings"></param>
        /// <returns>true if found</returns>
        public static bool IfStringContainsOneOf(string compare, List<string> strings)
        {
            if (strings.Count == 0) return false;

            return strings.Any(s => compare.Contains(s));
        }

        /// <summary>
        /// if string contains it and is not null empty or whitespace (Regex is technically faster)
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="stringsDelimited"></param>
        /// <param name="delimiter"></param>
        /// <returns>true if found</returns>
        public static bool IfStringContainsOneOf(string compare, string stringsDelimited, string delimiter)
        {
            if (string.IsNullOrEmpty(stringsDelimited))
            {
                return false;
            }

            string[] stringsArr = stringsDelimited.Split(delimiter);
            return IfStringContainsOneOf(compare, stringsArr);
        }
#endif

        /// <summary>
        /// if string contains it and is not null empty or whitespace (Regex is technically faster)
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="stringsDelimited"></param>
        /// <param name="delimiter"></param>
        /// <returns>true if found</returns>
        public static bool IfStringContainsOneOf(string compare, string stringsDelimited, char delimiter)
        {
            if (string.IsNullOrEmpty(stringsDelimited))
            {
                return false;
            }

            string[] stringsArr = stringsDelimited.Split(delimiter);

            return stringsArr.Any(s => compare.Contains(s));
        }

#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER

        /// <summary>
        /// determines if a string is base 64 or not
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static bool IsBase64String(string base64)
        {
            System.Span<byte> buffer = new System.Span<byte>(new byte[base64.Length]);
            return System.Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
        }
        /// <summary>
        /// Converts a two dimmensional Array to a jagged array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="twoDimensionalArray"></param>
        /// <returns></returns>
        public static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
        {
            int rows = twoDimensionalArray.GetLength(0);
            int columns = twoDimensionalArray.GetLength(1);
            T[][] jaggedArray = new T[rows][];

            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new T[columns];
                for (int j = 0; j < columns; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }

            return jaggedArray;
        }
        /// <summary>
        /// Chonky
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> enumerable, int chunkSize)
        {
            int itemsReturned = 0;
            var list = enumerable.ToList(); // Prevent multiple execution of IEnumerable.
            int count = list.Count;
            while (itemsReturned < count)
            {
                int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);
                yield return list.GetRange(itemsReturned, currentChunkSize);
                itemsReturned += currentChunkSize;
            }
        }
#endif

        /// <summary>
        /// Recursively delete a directory and its contents
        /// </summary>
        /// <param name="baseDir">Root Directory</param>
        public static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
                return;

            Parallel.ForEach(baseDir.EnumerateDirectories(), dir => RecursiveDelete(dir));

            FileInfo[] files = baseDir.GetFiles();
            Parallel.ForEach(files, file =>
            {
                file.IsReadOnly = false;
                file.Delete();
            });

            baseDir.Delete();
        }

        /// <summary>
        /// Finds whether a string contains one of the substrings passed to it
        /// </summary>
        /// <param name="stringToCompare"></param>
        /// <param name="subStrings"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static IEnumerable<string> WhereContains(string stringToCompare, List<string> subStrings, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(stringToCompare) || subStrings == null)
            {
                return Enumerable.Empty<string>();
            }

            if (ignoreCase)
            {
#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER

                return subStrings.Where(substring => stringToCompare.Contains(substring, System.StringComparison.CurrentCultureIgnoreCase));

#else
                return subStrings.Where(substring => stringToCompare.Contains(substring));
#endif
            }

            return subStrings.Where(substring => stringToCompare.Contains(substring));
        }

        /// <summary>
        /// Finds whether a string contains one of the substrings passed to it
        /// </summary>
        /// <param name="stringToCompare"></param>
        /// <param name="subStrings"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static IEnumerable<string> WhereContains(string stringToCompare, string[] subStrings, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(stringToCompare) || subStrings == null)
            {
                return Enumerable.Empty<string>();
            }

            if (ignoreCase)
            {
#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                return subStrings.Where(substring => stringToCompare.Contains(substring, System.StringComparison.CurrentCultureIgnoreCase));

#else
                return subStrings.Where(substring => stringToCompare.Contains(substring));
#endif
            }

            return subStrings.Where(substring => stringToCompare.Contains(substring));
        }

        //internal code section

        //protected code section

        //private code section

    } // Utility
} // Namespace