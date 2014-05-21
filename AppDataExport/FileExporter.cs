using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AppLogging;

namespace AppDataExport
{
    public abstract class FileExporter : IDataExporter 
    {
        protected string _ExportFile { get; set; }
        protected IList<string> _ExportData { get; set; }
        protected IMlogger _Logger { get; set; }

        public FileExporter(IMlogger logger, string exportFileLocation) 
        {
            _ExportData = new List<string>();
            _ExportFile = exportFileLocation;
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

        public abstract void ExportHeader(IList<string> headers);

        public abstract void ExportDataRow(IList<string> dataCols);

        public abstract void Flush();
    }

}
