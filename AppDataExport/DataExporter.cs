using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDataExport
{
    public class DataExporter : IDataExporter
    {
        public DataExporter()
        {
        }

        public void ExportHeader(string headerValue)
        {
            Console.WriteLine(string.Format("Headers: {0}", headerValue));
        }

        public void ExportDataRow(string rowValue)
        {
            Console.WriteLine(string.Format("Rowdata: {0}", rowValue));
        }

        public void ExportHeader(IList<string> headerColumns)
        {
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
    }
}
