using System;
using System.Collections;
using System.Collections.Generic;

namespace AppDataExport
{
    public interface IDataExporter
    {
        void ExportHeader(string headerValue);

        void ExportDataRow(string rowValue);

        void ExportHeader(IList<string> headers);

        void ExportDataRow(IList<string> dataCols);

        void Flush();
    }
}
