using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;

namespace Meuzz.CsvSharp
{
    public class CsvFileLoader : CsvFileLoader<string>
    {
    }

    public class CsvFileLoader<C> where C : IConvertible
    {
        public CsvFileLoader()
        {
        }

        public CsvFileLoader(Func<string[], C[]> headerfunc)
        {
            _headerfunc = headerfunc;
        }

        private Func<string[], C[]> _headerfunc = (string[] ss) =>
        {
            return ss.Select(x => (C)Convert.ChangeType(x, typeof(C))).ToArray();
        };

        public S LoadFromFile<S, T>(string filename, Func<C[], string[], (T, C[])> func) where S : DataSet<C, T>, new()
        {
            return LoadFromStreamReader<S, T>(new StreamReader(filename), func);
        }

        public S LoadFromStreamReader<S, T>(StreamReader streamReader, Func<C[], string[], (T, C[])> func) where S : DataSet<C, T>, new()
        {
            var csv = new CsvReader(streamReader);

            C[] header = _headerfunc(csv.Read());

            if (header.GroupBy(s => s).Where(s => s.Count() > 1).Any())
                throw new Exception("columns duplicated.");

            var keys = new C[] { };
            var cols = header;
            var rows = new List<T>();
            string[] row;
            while ((row = csv.Read()) != null)
            {
                var (r, ks) = func(header, row);
                if (r == null) { continue; }
                rows.Add(r);

                if (ks != null)
                    keys = ks;
            }

            if (!rows.Any())
                throw new Exception("no data");

            return new S() { Keys = keys, Columns = cols.Where(c => !keys.Contains(c)).ToArray(), Rows = rows.ToArray() };
        }

        public bool WriteToFile<T>(string filename, DataSet<C, T> dataSet, Func<T, C, object> func)
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
