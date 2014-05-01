using System;
using TinyIoC;
using ManageAzure.Lib;
using AppLogging;
using AppConfiguration;

namespace ManageAzureRunner
{
    public class ManageAzureRoles
    {
        const string AzurePublishSettingsFile = "c:\\temp\\Azure_Publish_Settings\\test.publishsettings";

        static void Main(string[] args)
        {
            Bootstrap.Register(AzurePublishSettingsFile);
            var application = TinyIoCContainer.Current.Resolve<AzureManagement>(); 


        }
    }

    public static class Bootstrap 
    {
        public static void Register(string settingsFile)
        {
            IMlogger mLogger = new Mlogger();
            IAppConfiguration appConfig = new ApplicationConfiguration(settingsFile);
            TinyIoCContainer.Current.Register<IMlogger>(mLogger);
            TinyIoCContainer.Current.Register<IAppConfiguration>(appConfig);
        }
    }
}
