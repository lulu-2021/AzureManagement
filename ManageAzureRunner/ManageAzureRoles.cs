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
            var arguments = "/AzureSettingsFile c:\\temp\\Azure_Publish_Settings\\test.publishsettings /CostDataFile c:\\temp\\Azure_Publish_Settings\\AzureCosts.xml /CsvExportFile c:\\temp\\test-csv-export.csv /RdpFilesDir c:\\temp\\rdp /Report /DownloadRdp";           
            args = arguments.Split();            
#endif

            var command = Args.Configuration.Configure<CommandObject>().CreateAndBind(args);

#if DEBUG
            Bootstrap.Register(command.AzureSettingsFile, command.CostDataFile, command.CsvExportFile);
#endif

            if (command.Help)
            {
                Console.WriteLine("The commandline arguments for the reporting tool are:");
                Console.WriteLine("");
                Console.WriteLine("/AzureSettingsFile \"Location of the Azure Publishsettingsfile\" ");
                Console.WriteLine("");
                Console.WriteLine("/CsvExportFile \"Location of the csv file to export the data to\"");
                Console.WriteLine("");
                Console.WriteLine("/RdpFilesDir \"Location of the directory where to dump the RDP files\"");
                Console.WriteLine("");
                Console.WriteLine("/DownloadRdp \"if omitted it will not do the RDP file download\"");
                Console.WriteLine("");
                Console.WriteLine("/Report \"if omitted it will not do the report\"");
            }
            else 
            {

                if (command.AzureSettingsFile != null && command.CsvExportFile != null)
                {
                    Bootstrap.Register(command.AzureSettingsFile, command.CostDataFile, command.CsvExportFile);
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
        public string CsvExportFile { get; set; }
        public string RdpFilesDir { get; set; }
        public bool DownloadRdp { get; set; }
        public bool Report { get; set; }
        public bool Help { get; set; }
    }

    public static class Bootstrap 
    {
        public static void Register(string settingsFile, string costDataFile, string csvExportFile)
        {
            IMlogger mLogger = new Mlogger();
            IAppConfiguration appConfig = new ApplicationConfiguration(settingsFile, costDataFile);
            IDataExporter consoleExporter = new ConsoleWriter();
            IDataExporter csvWriter = new CsvExporter(mLogger, csvExportFile);
            TinyIoCContainer.Current.Register<IMlogger>(mLogger);
            TinyIoCContainer.Current.Register<IAppConfiguration>(appConfig);
            TinyIoCContainer.Current.Register<IDataExporter>(csvWriter);
        }
    }
}