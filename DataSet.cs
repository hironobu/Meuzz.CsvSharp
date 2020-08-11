using System;
using System.Collections.Generic;
using System.Text;

namespace Meuzz.CsvSharp
{
    public class DataSet<T>
    {
        public DataSet(string[] cols, T[] rows)
        {
            Columns = cols;
            Rows = rows;
        }

        public string[] Columns { get; }

        public T[] Rows { get; }
    }
}
