using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AppLogging;

namespace AppDataExport
{

    public class HtmlExporter : FileExporter, IDataExporter
    {
        private IHtmlWrapper Wrapper { get; set; }

        public HtmlExporter(IMlogger logger, string exportFileLocation, IHtmlWrapper wrapper )
            : base(logger, exportFileLocation)
        {
            Wrapper = wrapper;
        }

        public override void ExportHeader(IList<string> headers)
        {
            var _header = "<thead><tr>";
            foreach (var header in headers)
            {
                _header += string.Format("<th>{0}</th>", header);
            }
            _header += "</tr></thead>";
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
                foreach (var line in _ExportData)
                {
                    html.AppendLine(string.Join("", line));
                }
                var output = Wrapper.Wrap(html.ToString());
                File.WriteAllText(_ExportFile, output);
            }
            catch (Exception htmlE)
            {
                var errorMessage = string.Format("Error opening the HTML file for writes: {0}", htmlE);
                _Logger.Warn(htmlE, errorMessage);
            }
        }
    }
}
