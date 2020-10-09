## What is this

C# library for handling CSV files.

## Usage

We are preparing abstraction layer for Object Maping on CSV.

### Read

```
using Meuzz.CsvSharp.Io;

CsvReader csv = new CsvReader(File.Open("test.csv"));

string[] row;
while ((row = csv.Read()) != null)
{
    Console.WriteLine(string.Format("[{0}]", string.Join(",", row.Select(x => string.Format("[{0}]", x)))));
}
```

### Write

```
using Meuzz.CsvSharp.Io;

// QuoteAlways: quote every cell regardless of its value 
// CsvWriter writer = new CsvWriter(File.Create("test.csv"), CsvWriterOptions.QuoteAlways);

// None(default): quote only strings
CsvWriter writer = new CsvWriter(File.Create("test.csv"));

writer.Write(new[] { "aaa", "bbb" });
writer.Write(new[] { 111.ToString(), 222.ToString() });
writer.Write(new[] { "aa aa", "this, is, a test" });
writer.Write(new[] { "a\",a \"aa", "aaa\"\"aaa" });

// call below where you need (or "using" clause)
// writer.Dispose();
```