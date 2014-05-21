using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AppLogging;

namespace AppDataExport
{

    public class HtmlExporter : FileExporter, IDataExporter
    {
        private string _TableHeader { get; set; }
        private string _TableFooter { get; set; }
        public HtmlExporter(IMlogger logger, string exportFileLocation, string docHeader, string docFooter )
            : base(logger, exportFileLocation)
        {            
            _TableHeader = docHeader;
            _TableFooter = docFooter;
        }

        public override void ExportHeader(IList<string> headers)
        {
            var _header = "<tr>";
            foreach (var header in headers)
            {
                _header += string.Format("<th>{0}</th>", header);
            }
            _header += "</tr>";
            _ExportData.Add(_header);
        }

        public override void ExportDataRow(IList<string> dataCols)
        {
            var _dataRow = "<tr>";
            foreach (var dataCol in dataCols)
            {
                _dataRow += string.Format("<td>{0}</td>", dataCol);
            }
            _dataRow += "</tr>";
            _ExportData.Add(_dataRow);
        }

        public override void Flush()
        {
            try
            {
                var html = new StringBuilder();
                html.Append(_TableHeader);
                foreach (var line in _ExportData)
                {
                    html.AppendLine(string.Join("", line));
                }
                html.Append(_TableFooter);
                File.WriteAllText(_ExportFile, html.ToString());
            }
            catch (Exception htmlE)
            {
                var errorMessage = string.Format("Error opening the HTML file for writes: {0}", htmlE);
                _Logger.Warn(htmlE, errorMessage);
            }
        }
    }
}
