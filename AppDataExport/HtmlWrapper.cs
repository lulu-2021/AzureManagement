using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDataExport
{
    public class HtmlWrapper : IHtmlWrapper
    {
        private string DocHeader { get; set; }
        private string DocFooter { get; set; }

        public HtmlWrapper()
        {
            var tableHtml = new StringBuilder();
            var tableCss = "class=\"table table-striped\"";
            var bootStrapHead = "<head><link rel=\"stylesheet\" href=\"http://installer.janison.com.au/assets/css/bootstrap.css\" /></head>";
            tableHtml.Append(string.Format("<html>{0}", bootStrapHead));
            tableHtml.Append(string.Format("<body><table {0}>", tableCss));

            DocHeader = tableHtml.ToString();
            var bootStrapFoot = "<script src=\"http://installer.janison.com.au/assets/js/bootstrap.min.js\"></script>";
            var tableFooter = string.Format("</table>{0}</body></html>", bootStrapFoot);
            DocFooter = tableFooter;
        }

        public string Wrap(string data)
        {
            var htmlDoc = new StringBuilder();
            htmlDoc.Append(DocHeader);
            htmlDoc.Append(data);
            htmlDoc.Append(DocFooter);
            return htmlDoc.ToString();
        }
    }
}
