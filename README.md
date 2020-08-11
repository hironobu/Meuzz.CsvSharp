## What is this

C# library for handling CSV files.

## Usage

```
StreamReader reader = new StreamReader("test2.csv");
CsvReader csv = new CsvReader(reader);

string[] row;
while ((row = csv.Read()) != null)
{
    Console.WriteLine(string.Format("[{0}]", string.Join(",", row.Select(x => string.Format("[{0}]", x)))));
}
```