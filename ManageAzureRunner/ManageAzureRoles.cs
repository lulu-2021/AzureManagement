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
        const string AzurePublishSettingsFile = "c:\\temp\\Azure_Publish_Settings\\test.publishsettings";

        static void Main(string[] args)
        {
            var csvExportFile = "c:\\temp\\test-csv-export.csv";
            Bootstrap.Register(AzurePublishSettingsFile, csvExportFile);
            var appDownloader = TinyIoCContainer.Current.Resolve<AzureManagementDownloader>();
            var appReporter = TinyIoCContainer.Current.Resolve<AzureManagementReporter>();

            //var rdpFiles = appDownloader.GetAllElasticRoleRdpFiles();
            //var result = appDownloader.DownloadRdpFiles(rdpFiles, "c:\\temp\\rdp");

            appReporter.GetAllVirtualMachineRoles();            
            appReporter.ExportAllWebRoles();
            appReporter.Exporter.Flush();

            Console.WriteLine("DONE!");
            Console.ReadKey();
        }
    }

    public static class Bootstrap 
    {
        public static void Register(string settingsFile, string csvExportFile)
        {
            IMlogger mLogger = new Mlogger();
            IAppConfiguration appConfig = new ApplicationConfiguration(settingsFile);
            IDataExporter consoleExporter = new ConsoleWriter();
            IDataExporter csvWriter = new CsvExporter(mLogger, csvExportFile);
            TinyIoCContainer.Current.Register<IMlogger>(mLogger);
            TinyIoCContainer.Current.Register<IAppConfiguration>(appConfig);
            TinyIoCContainer.Current.Register<IDataExporter>(csvWriter);
        }
    }
}
