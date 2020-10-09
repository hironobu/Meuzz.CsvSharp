using System;
using System.IO;
using System.Linq;
using Meuzz.Foundation;

namespace Meuzz.CsvSharp.Io
{
    public enum CsvWriterOptions
    {
        None = 0,
        QuoteAlways = 1,
    }

    public class CsvWriter : IDisposable
    {
        private StreamWriter _writer;
        private CsvWriterOptions _options;
        private char _delimiter;

        private Func<string, string> _quotefunc = null;

        public CsvWriter(string filename, char delimiter = ',', CsvWriterOptions options = CsvWriterOptions.None)
            : this(new StreamWriter(filename), delimiter, options)
        {
        }

        public CsvWriter(Stream stream, char delimiter = ',', CsvWriterOptions options = CsvWriterOptions.None)
            : this(new StreamWriter(stream), delimiter, options)
        {
        }

        public CsvWriter(StreamWriter writer, char delimiter = ',', CsvWriterOptions options = CsvWriterOptions.None)
        {
            _writer = writer;
            _delimiter = delimiter;
            _options = options;

            _quotefunc = x =>
            {
                decimal v = default(decimal);
                return decimal.TryParse(x, out v) ? x : Quote(x);
            };
            if ((_options & CsvWriterOptions.QuoteAlways) != CsvWriterOptions.None)
            {
                _quotefunc = x => Quote(x);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _writer?.Dispose();
                    _writer = null;
                }
                _disposed = true;
            }
        }

        ~CsvWriter()
        {
            Dispose(false);
        }


        private string Quote(string o)
        {
            return $"\"{o.Replace("\"", "\"\"")}\"";
        }

        public void Write(string[] row)
        {
            _writer.WriteLine(string.Join(_delimiter, row.Select(x => _quotefunc(x))));
        }
    }

    /*
    public class CsvWriter<C, T> : IDisposable
    {
        private C[] _columns;
        private StreamWriter _writer;

        public CsvWriter(Stream stream)
        {
            _writer = new StreamWriter(stream);
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
        }

        private string Quote(object o)
        {
            return $"\"{o.ToString().Replace("\"", "\"\"")}\"";
        }

        public bool Write(C[] cols, T[] rows, Func<T, C[], object[]> func)
        {
            if (!Arrays.IsNullOrEmpty(cols))
                _writer.WriteLine(string.Join(",", cols.Select(x => Quote(x))));

            foreach (var row in rows)
            {
                _writer.WriteLine(string.Join(",", func(row, cols).Select(x => Quote(x))));
            }

            return true;
        }
    }
     */
}
