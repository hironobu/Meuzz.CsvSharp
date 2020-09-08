using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Meuzz.CsvSharp
{
    public class Arrays
    {
        public static T[] Concat<T>(params T[][] arrs)
        {
            var result = new T[arrs.Select((x) => x.Length).Aggregate((x, y) => x + y)];
            int i = 0;
            foreach (var a in arrs)
            {
                Array.Copy(a, 0, result, i, a.Length);
                i += a.Length;
            }
            return result;
        }
    }
}
