using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AppLogging;

namespace AppDataExport
{

    public class CsvExporter : FileExporter, IDataExporter
    {
        public CsvExporter(IMlogger logger, string exportFileLocation)
            : base(logger, exportFileLocation)
        {
        }

        public override void ExportHeader(IList<string> headers)
        {
            var _header = string.Empty;
            foreach (var header in headers)
            {
                _header += string.Format("{0}{1}", header, ",");
            }
            _ExportData.Add(_header);
        }

        public override void ExportDataRow(IList<string> dataCols)
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
        public override void Flush()
        {
            try
            {
                var csv = new StringBuilder();
                foreach (var line in _ExportData)
                {
                    csv.AppendLine(string.Join(",", line));
                }
                File.WriteAllText(_ExportFile, csv.ToString());
            }
            catch (Exception exE)
            {
                var errorMessage = string.Format("Error opening the Export file for writes: {0}", exE);
                _Logger.Warn(exE, errorMessage);
            }
        }
    }
}
