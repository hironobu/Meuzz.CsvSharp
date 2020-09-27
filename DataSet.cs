using System;
using System.Collections.Generic;
using System.Linq;

namespace Meuzz.CsvSharp
{
    public class DataSet<T>
    {
        public DataSet()
        {
        }

        public DataSet(string[] cols, T[] rows)
        {
            Columns = cols;
            Rows = rows;
        }

        public string[] Columns { get; set; }

        public T[] Rows { get; set; }

        public T2 ColumnFilteredDataSet<T2>(string[] cols) where T2 : DataSet<T>, new()
        {
            return new T2() { Columns = Columns.Where(x => cols.Contains(x)).ToArray(), Rows = Rows };
        }
        public T2 ColumnExcludingDataSet<T2>(string[] cols) where T2 : DataSet<T>, new()
        {
            return new T2() { Columns = Columns.Where(x => !cols.Contains(x)).ToArray(), Rows = Rows };
        }

        /// <summary>
        /// 2つ以上のDataSetを連結する
        /// </summary>
        /// <param name="dataSets"></param>
        public static T1 CreateFromDataSets<T1>(IEnumerable<T1> dataSets) where T1 : DataSet<T>, new()
        {
            string[] cols = { };
            T[] rows = { };

            if (dataSets.Any())
            {
                cols = dataSets.FirstOrDefault()?.Columns;
                rows = Arrays.Concat<T>(dataSets.Select((x) => x.Rows).ToArray());
            }

            return new T1() { Columns = cols, Rows = rows };
        }

    }

    public static class DataSetExtensions
    {
        [Obsolete]
        /// <summary>
        /// 2つ以上のDataSetを連結する
        /// </summary>
        /// <param name="dataSets"></param>
        public static T2 Concat<T, T2>(this IEnumerable<T2> dataSets) where T2 : DataSet<T>, new()
        {
            string[] cols = { };
            T[] rows = { };

            if (dataSets.Any())
            {
                cols = dataSets.FirstOrDefault()?.Columns;
                rows = Arrays.Concat<T>(dataSets.Select((x) => x.Rows).ToArray());
            }

            return new T2() { Columns = cols, Rows = rows };
        }
    }
}