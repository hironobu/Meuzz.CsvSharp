using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;

namespace Meuzz.CsvSharp
{
    public class CsvFileLoader
    {
        public DataSet<T> LoadFromFile<T>(string filename, Func<string[], string[], T> func)
        {
            var reader = new StreamReader(filename);
            var csv = new CsvReader(reader);

            var header = csv.Read();
            if (header.GroupBy(s => s).Where(s => s.Count() > 1).Any())
            {
                throw new Exception("duplicated columns exist.");
            }

            var rows = new List<T>();
            string[] row;
            while ((row = csv.Read()) != null)
            {
                rows.Add(func(header, row));
            }
            if (!rows.Any())
            {
                throw new Exception("empty rows");
            }

            return new DataSet<T>(header, rows.ToArray());
        }

        public bool WriteToFile<T>(string filename, DataSet<T> dataSet, Func<T, string, object> func)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            using (var sw = new StreamWriter(filename, false))
            {
                sw.WriteLine(string.Join(",", dataSet.Columns.Select(x => string.Format("\"{0}\"", x))));

                foreach (var row in dataSet.Rows)
                {
                    sw.WriteLine(string.Join(",", dataSet.Columns.Select(x => string.Format("\"{0}\"", func(row, x)))));
                }
            }

            return true;
        }
    }
}
