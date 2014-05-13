using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AppLogging;

namespace AppDataExport
{
    public class CsvExporter : IDataExporter
    {        
        private string _CsvFile { get; set; }
        private IList<string> _ExportData { get; set; }
        private IMlogger _Logger { get; set; }

        public CsvExporter(IMlogger logger, string csvFileLocation) 
        {
            _ExportData = new List<string>();
            _CsvFile = csvFileLocation;
            _Logger = logger;
        }

        public void ExportHeader(string headerValue)
        {
            _ExportData.Add(headerValue);
        }

        public void ExportDataRow(string rowValue)
        {
            _ExportData.Add(rowValue);
        }

        public void ExportHeader(IList<string> headers)
        {
            var _header = string.Empty;
            foreach (var header in headers) 
            {
                _header += string.Format("{0}{1}", header, ",");
            }
            _ExportData.Add(_header);
        }

        public void ExportDataRow(IList<string> dataCols)
        {
            var _dataRow = string.Empty;
            foreach (var dataCol in dataCols) 
            {
                _dataRow += string.Format("{0}{1}", dataCol, ",");
            }
            _ExportData.Add(_dataRow);
        }

        /// <summary>
        /// flush the existing header and data rows out to the file!
        /// </summary>
        public void Flush() 
        {
            try
            {
                var csv = new StringBuilder();
                foreach (var line in _ExportData)
                {
                    csv.AppendLine(string.Join(",", line));
                }
                File.WriteAllText(_CsvFile, csv.ToString());
            }
            catch (Exception csvE)
            {
                var errorMessage = string.Format("Error opening the CSV file for writes: {0}", csvE);
                _Logger.Warn(csvE, errorMessage);
            }
        }


        /// <summary>
        /// clear the csv cache, release file handles and clean up..
        /// </summary>
        public void Dispose()
        {
        }
    }

    public class ConsoleWriter : IDataExporter
    {
        public ConsoleWriter(){}

        public void ExportHeader(string headerValue)
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine(string.Format("Headers: {0}", headerValue));
        }

        public void ExportDataRow(string rowValue)
        {
            Console.WriteLine(string.Format("Rowdata: {0}", rowValue));
        }

        public void ExportHeader(IList<string> headerColumns)
        {
            Console.WriteLine("--------------------------------------");
            var headerRow = string.Empty;
            foreach (var header in headerColumns) 
            {
                headerRow += string.Format("{0}{1}", header, ",");
            }
            Console.WriteLine(string.Format("Headers: {0}", headerRow));
        }

        public void ExportDataRow(IList<string> dataColumns)
        {
            var headerRow = string.Empty;
            foreach (var header in dataColumns)
            {
                headerRow += string.Format("{0}{1}", header, ",");
            }
            Console.WriteLine(string.Format("Rowdata: {0}", headerRow));
        }

        public void Flush() 
        {
            Console.WriteLine("--------------------------------");
        }

    }
}
