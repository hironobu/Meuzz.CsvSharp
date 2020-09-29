using System;
using System.Collections.Generic;
using System.Linq;

namespace Meuzz.CsvSharp
{
    public class DataSet<C, T>
    {
        public DataSet()
        {
        }

        public DataSet(C[] keys, C[] cols, T[] rows)
        {
            Keys = keys;
            Columns = cols;
            Rows = rows;
        }

        public C[] Keys { get; set; }

        public C[] Columns { get; set; }

        public T[] Rows { get; set; }

        public static T1 CreateWithFilteringColumns<T1>(T1 source, C[] cols) where T1 : DataSet<C, T>, new()
        {
            return new T1() { Keys = source.Keys, Columns = source.Columns.Where(x => cols.Contains(x)).ToArray(), Rows = source.Rows };
        }
        public static T1 CreateWithExcludingColumns<T1>(T1 source, C[] cols) where T1 : DataSet<C, T>, new()
        {
            return new T1() { Keys = source.Keys, Columns = source.Columns.Where(x => !cols.Contains(x)).ToArray(), Rows = source.Rows };
        }

        /// <summary>
        /// 2つ以上のDataSetを連結する
        /// </summary>
        /// <param name="dataSets"></param>
        public static T1 CreateFromDataSets<T1>(IEnumerable<T1> dataSets) where T1 : DataSet<C, T>, new()
        {
            C[] cols = { };
            T[] rows = { };

            if (dataSets.Any())
            {
                cols = dataSets.FirstOrDefault()?.Columns;
                rows = Arrays.Concat<T>(dataSets.Select((x) => x.Rows).ToArray());
            }

            return new T1() { Keys = dataSets.FirstOrDefault()?.Keys, Columns = cols, Rows = rows };
        }

    }

    public static class DataSetExtensions
    {
        [Obsolete]
        /// <summary>
        /// 2つ以上のDataSetを連結する
        /// </summary>
        /// <param name="dataSets"></param>
        public static T2 Concat<C, T, T2>(this IEnumerable<T2> dataSets) where T2 : DataSet<C, T>, new()
        {
            C[] cols = { };
            T[] rows = { };

            if (dataSets.Any())
            {
                cols = dataSets.FirstOrDefault()?.Columns;
                rows = Arrays.Concat<T>(dataSets.Select((x) => x.Rows).ToArray());
            }

            return new T2() { Keys = dataSets.FirstOrDefault()?.Keys, Columns = cols, Rows = rows };
        }
    }
}