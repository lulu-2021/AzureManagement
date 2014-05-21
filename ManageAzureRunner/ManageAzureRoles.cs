using System;
using TinyIoC;
using ManageAzure.Lib;
using AppLogging;
using AppConfiguration;
using AppDataExport;

namespace ManageAzureRunner
{
    public class ManageAzureRoles
    {
        static void Main(string[] args)
        {
#if DEBUG
            var arguments = "/AzureSettingsFile c:\\temp\\Azure_Publish_Settings\\test.publishsettings /CostDataFile c:\\temp\\Azure_Publish_Settings\\AzureCosts.xml /ExportFile c:\\temp\\test-html-export.html /RdpFilesDir c:\\temp\\rdp /Report /DownloadRdp /html";       
            args = arguments.Split();            
#endif
            var command = Args.Configuration.Configure<CommandObject>().CreateAndBind(args);

            if (command.Help)
            {
                Console.WriteLine("The commandline arguments for the reporting tool are:");
                Console.WriteLine("");
                Console.WriteLine("/AzureSettingsFile \"Location of the Azure Publishsettingsfile\" ");
                Console.WriteLine("");
                Console.WriteLine("/ExportFile \"Location of the file to export the data to\"");
                Console.WriteLine("");
                Console.WriteLine("/RdpFilesDir \"Location of the directory where to dump the RDP files\"");
                Console.WriteLine("");
                Console.WriteLine("/DownloadRdp \"if omitted it will not do the RDP file download\"");
                Console.WriteLine("");
                Console.WriteLine("/Report \"if omitted it will not do the report\"");
                Console.WriteLine("");
                Console.WriteLine("/Csv \" if this flag and html is omitted it will go to the console\"");
                Console.WriteLine("");
                Console.WriteLine("/Html \" if this flag and Csv is omitted it will go to the console\"");
            }
            else 
            {

                if (command.AzureSettingsFile != null && command.ExportFile != null)
                {

                    var exportType = Bootstrap.EnExportType.Console;
                    if (command.Csv) { exportType = Bootstrap.EnExportType.Csv; }
                    else if (command.Html) { exportType = Bootstrap.EnExportType.Html; }

                    // Boot strap the process with the right configuration and reporting processes..
                    Bootstrap.Register(command.AzureSettingsFile, command.CostDataFile, command.ExportFile, exportType);

                    if (command.RdpFilesDir != null)
                    {
                        var RdpFilesDir = command.RdpFilesDir;
                    }
                    var appDownloader = TinyIoCContainer.Current.Resolve<AzureManagementDownloader>();
                    var appReporter = TinyIoCContainer.Current.Resolve<AzureManagementReporter>();

                    // download the RDP files - currently defaults to all..
                    if (command.DownloadRdp && command.RdpFilesDir != null)
                    {
                        // vm roles
                        var vmRdpFiles = appDownloader.GetAllVirtualMachineRdpFiles();
                        appDownloader.DownloadRdpFiles(vmRdpFiles, command.RdpFilesDir);
                        // web and worker roles
                        var webRdpFiles = appDownloader.GetAllElasticRoleRdpFiles();
                        appDownloader.DownloadRdpFiles(webRdpFiles, command.RdpFilesDir);

                    }

                    // run the reports - currently defaults to all roles..
                    if (command.Report)
                    {
                        appReporter.ExportAllVirtualMachineRoles();
                        appReporter.ExportAllWebRoles();
                        appReporter.Exporter.Flush();
                    }

#if DEBUG
                    Console.ReadKey();
#endif

                }
                else 
                {
                    Console.WriteLine("AzureSettingsFile and CsvExportFile are mandatory");
                }
            }

        }
    }



    public class CommandObject
    {
        public string AzureSettingsFile { get; set; }
        public string CostDataFile { get; set; }
        public string ExportFile { get; set; }
        public string RdpFilesDir { get; set; }
        public bool DownloadRdp { get; set; }
        public bool Report { get; set; }
        public bool Csv { get; set; }
        public bool Html { get; set; }
        public bool Help { get; set; }
    }

    public static class Bootstrap 
    {
        public enum EnExportType 
        {
            Console,
            Csv,
            Html
        }

        public static void Register(string settingsFile, string costDataFile, string exportFile = "", EnExportType exportType = EnExportType.Console)
        {
            var tableHeader = "<html><body><table>";
            var tableFooter = "</table></body></html>";

            IMlogger mLogger = new Mlogger();
            IAppConfiguration appConfig = new ApplicationConfiguration(settingsFile, costDataFile);
            TinyIoCContainer.Current.Register<IMlogger>(mLogger);
            TinyIoCContainer.Current.Register<IAppConfiguration>(appConfig);
            IDataExporter exporter = null;
            switch (exportType) 
            {
                case EnExportType.Console:
                    exporter = new ConsoleWriter();
                    break;
                case EnExportType.Csv:
                    exporter = new CsvExporter(mLogger, exportFile);
                    break;
                case EnExportType.Html:
                    exporter = new HtmlExporter(mLogger, exportFile, tableHeader, tableFooter);
                    break;
            }
            TinyIoCContainer.Current.Register<IDataExporter>(exporter);
        }
    }
}