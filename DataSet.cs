using System;
using System.Collections.Generic;
using System.Linq;

namespace Meuzz.CsvSharp
{
    public class DataSet<T>
    {
        public DataSet(string[] cols, T[] rows)
        {
            Columns = cols;
            Rows = rows;
        }

        public string[] Columns { get; set; }

        public T[] Rows { get; }

        public DataSet<T> ColumnFilteredDataSet(string[] cols)
        {
            return new DataSet<T>(Columns.Where(x => cols.Contains(x)).ToArray(), Rows);
        }
        public DataSet<T> ColumnExcludingDataSet(string[] cols)
        {
            return new DataSet<T>(Columns.Where(x => !cols.Contains(x)).ToArray(), Rows);
        }
    }

    public static class DataSetExtensions
    {
        /// <summary>
        /// 2つ以上のDataSetを連結する
        /// </summary>
        /// <param name="containers"></param>
        public static DataSet<Dictionary<string, object>> Concat(this IEnumerable<DataSet<Dictionary<string, object>>> dataSets)
        {
            string[] cols = { };
            Dictionary<string, object>[] rows = { };

            if (dataSets.Any())
            {
                cols = dataSets.FirstOrDefault()?.Columns;
                rows = Arrays.Concat<Dictionary<string, object>>(dataSets.Select((x) => x.Rows).ToArray());
            }

            return new DataSet<Dictionary<string, object>>(cols, rows);
        }
    }
}