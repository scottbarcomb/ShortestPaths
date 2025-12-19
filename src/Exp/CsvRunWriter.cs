using CsvHelper;
using System.Globalization;

namespace Exp
{
    // Writes the captured metric data to a CSV file
    // CsvHelper is a .NET package that must be installed
    public sealed class CsvRunWriter : IDisposable
    {
        private readonly CsvWriter _csv;
        private bool _headerWritten;

        public CsvRunWriter(string path)
        {
            var writer = new StreamWriter(path, append: true);
            _csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            _headerWritten = writer.BaseStream.Length > 0;
        }

        public void Write(RunRow row)
        {
            if (!_headerWritten)
            {
                _csv.WriteHeader<RunRow>();
                _csv.NextRecord();
                _headerWritten = true;
            }

            _csv.WriteRecord(row);
            _csv.NextRecord();
            _csv.Flush();
        }

        public void Dispose() => _csv.Dispose();
    }
}
