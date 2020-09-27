using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;

namespace Meuzz.CsvSharp
{
    public class CsvFileLoader
    {
        public DataSet<T> LoadFromFile<T>(string filename, Func<string[], string[], (T, string[])> func)
        {
            return LoadFromStreamReader(new StreamReader(filename), func);
        }

        public DataSet<T> LoadFromStreamReader<T>(StreamReader stream, Func<string[], string[], (T, string[])> func)
        {
            var csv = new CsvReader(stream);

            var header = csv.Read();

            if (header.GroupBy(s => s).Where(s => s.Count() > 1).Any())
                throw new Exception("columns duplicated.");

            var cols = header;
            var rows = new List<T>();
            string[] row;
            while ((row = csv.Read()) != null)
            {
                var (r, newcols) = func(header, row);
                if (r == null) { continue; }
                rows.Add(r);

                if (newcols != null)
                    cols = newcols;
            }

            if (!rows.Any())
                throw new Exception("no data");

            return new DataSet<T>(cols, rows.ToArray());
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
