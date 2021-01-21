using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Meuzz.CsvSharp.Io
{
    public class CsvReader
    {
        private const char DefaultDelimiter = ',';
        private const char DefaultQuote = '"';

        private char _delimiter = DefaultDelimiter;
        private char _quote = DefaultQuote;

        private StreamReader _reader;
        private bool _inQuote = false;

        private CsvReaderContext _context;

        public CsvReader(string filename, char quote = DefaultQuote, char delim = DefaultDelimiter)
            : this(new StreamReader(filename), quote, delim)
        {
        }

        public CsvReader(Stream stream, char quote = DefaultQuote, char delim = DefaultDelimiter)
            : this(new StreamReader(stream), quote, delim)
        {
        }

        public CsvReader(StreamReader reader, char quote = DefaultQuote, char delim = DefaultDelimiter)
        {
            _reader = reader;
            _delimiter = delim;
            _quote = quote;

            _context = new CsvReaderContext();
            _inQuote = false;
        }

        public string[] Read()
        {
            _context.Reset();
            
            do
            {
                string line = _reader.ReadLine();
                if (line == null)
                {
                    if (_context.IsEmpty())
                        return null;
                    else
                        break;
                }

                // _context.SetSource(line);

                int i = 0, i0 = 0;
                char[] charr = line.ToCharArray();
                while (i < charr.Length)
                {
                    char c = charr[i];

                    if (_inQuote)
                    {
                        if (c == '\\')
                        {
                            _context.Append(line, i0, i);
                            i++;
                            i0 = i;
                        }
                        else if (c == _quote)
                        {
                            _context.Append(line, i0, i);
                            i0 = i + 1;
                            if (i0 < charr.Length && charr[i0] == _quote)
                            {
                                i++;
                            }
                            else
                            {
                                _inQuote = false;
                            }
                        }
                    }
                    else
                    {
                        if (c == _delimiter)
                        {
                            _context.Append(line, i0, i);
                            _context.Push();
                            i0 = i + 1;
                        }
                        else if (c == _quote)
                        {
                            _context.Append(line, i0, i);
                            _inQuote = true;
                            i0 = i + 1;
                        }
                    }

                    i++;
                }

                _context.Append(line, i0, i);
                if (_inQuote)
                {
                    _context.Append(Environment.NewLine);
                }

            } while (_inQuote);

            _context.Push();

            return _context.GetStrings();
        }

        class CsvReaderContext
        {
            private List<string> _strings;
            private StringBuilder _builder;

            // private string _source;

            public CsvReaderContext()
            {
                _strings = new List<string>();
                _builder = new StringBuilder();
            }

            /*public void SetSource(string src)
            {
                _source = src;
            }*/

            public void Append(string s, int i0, int i)
            {
                _builder.Append(s, i0, i - i0);
            }

            public void Append(string s)
            {
                _builder.Append(s);
            }

            public void Push()
            {
                _strings.Add(_builder.ToString());
                _builder.Clear();
            }

            public string[] GetStrings()
            {
                return _strings.ToArray();
            }

            public void Reset()
            {
                _strings.Clear();
                _builder.Clear();
            }

            public bool IsEmpty()
            {
                return _strings.Count == 0 && _builder.Length == 0;
            }
        }
    }
}
