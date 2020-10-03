using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using Meuzz.Foundation;
using Meuzz.CsvSharp.Io;

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

        public S LoadFromStream<S, T>(Stream stream, Func<C[], string[], (T, C[])> func) where S : DataSet<C, T>, new()
        {
            return LoadFromStreamReader<S, T>(new StreamReader(stream), func);
        }

        public S LoadFromStreamReader<S, T>(StreamReader streamReader, Func<C[], string[], (T, C[])> func) where S : DataSet<C, T>, new()
        {
            var csv = new CsvReader(streamReader);

            C[] cols = _headerfunc(csv.Read());

            if (cols.GroupBy(s => s).Where(s => s.Count() > 1).Any())
                throw new Exception("columns duplicated.");

            var keys = new C[] { };
            var rows = new List<T>();
            string[] row;
            while ((row = csv.Read()) != null)
            {
                var (r, ks) = func(cols, row);
                if (r == null) { continue; }
                rows.Add(r);

                if (ks != null)
                    keys = ks;
            }

            if (!rows.Any())
                throw new Exception("no data");

            return new S() { KeyColumns = keys, Columns = cols.Where(c => !keys.Contains(c)).ToArray(), Rows = rows.ToArray() };
        }

        public bool WriteToFile<T>(string filename, DataSet<C, T> dataSet, Func<T, C[], string[]> func)
        {
            return WriteToStreamWriter<T>(new StreamWriter(filename), dataSet, func);
        }

        public bool WriteToStream<T>(Stream stream, DataSet<C, T> dataSet, Func<T, C[], string[]> func)
        {
            return WriteToStreamWriter<T>(new StreamWriter(stream), dataSet, func);
        }

        public bool WriteToStreamWriter<T>(StreamWriter streamWriter, DataSet<C, T> dataSet, Func<T, C[], string[]> func)
        {
            CsvWriter writer = new CsvWriter(streamWriter);

            writer.Write(dataSet.Columns.Select(c => c.ToString()).ToArray());

            foreach (var row in dataSet.Rows)
            {
                writer.Write(func(row, dataSet.Columns));
            }

            return true;
        }
    }
}
