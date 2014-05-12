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
            Bootstrap.Register(AzurePublishSettingsFile);
            var appDownloader = TinyIoCContainer.Current.Resolve<AzureManagementDownloader>();
            var appReporter = TinyIoCContainer.Current.Resolve<AzureManagementReporter>();

            //var rdpFiles = appDownloader.GetAllElasticRoleRdpFiles();
            //var result = appDownloader.DownloadRdpFiles(rdpFiles, "c:\\temp\\rdp");

            var vms = appReporter.GetAllVirtualMachineRoles();
            Console.WriteLine("--- Virtual Machines ---");
            foreach (var vm in vms.MyVirtualMachines) 
            {
                Console.WriteLine(String.Format("Virtual Machine -- Role Name: {0} -- Role Size: {1} -- Role Type: {2}",vm.RoleName, vm.RoleSize, vm.RoleType));
            }
            Console.WriteLine("---------------------------------------------");

            var webRoles = appReporter.GetAllWebRoles();
            Console.WriteLine("--- Web Roles ---");
            foreach(var role in webRoles.MyComputeRoles)
            {
                Console.WriteLine(String.Format("Web Role -- Service: {0} -- Hostname: {1} -- Instance: {2} -- Size: {3} -- Status: {4} ",role.ServiceName, role.HostName, role.InstanceName, role.InstanceSize, role.InstanceStatus));
            }
            Console.WriteLine("---------------------------------------------");

            Console.WriteLine("DONE!");
            Console.ReadKey();
        }
    }

    public static class Bootstrap 
    {
        public static void Register(string settingsFile)
        {
            IMlogger mLogger = new Mlogger();
            IAppConfiguration appConfig = new ApplicationConfiguration(settingsFile);
            IDataExporter dataExporter = new DataExporter();
            TinyIoCContainer.Current.Register<IMlogger>(mLogger);
            TinyIoCContainer.Current.Register<IAppConfiguration>(appConfig);
            TinyIoCContainer.Current.Register<IDataExporter>(dataExporter);
        }
    }
}
