using System;
using System.Collections.Generic;

namespace AppDataExport
{
    public class ConsoleWriter : IDataExporter
    {
        public ConsoleWriter() { }

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

