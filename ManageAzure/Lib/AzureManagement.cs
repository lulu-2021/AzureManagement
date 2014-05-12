using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ManageAzure.AzureModels;
using AppLogging;
using AppConfiguration;
using AppDataExport;


namespace ManageAzure.Lib
{
    public class AzureManagement
    {
        public SubscriptionCloudCredentials MyCloudCredentials { get; set; }
        public IMlogger Logger { get; set; }
        public IAppConfiguration Configuration { get; set;}
        public IDataExporter Exporter { get; set; }

        public AzureManagement(IMlogger logger, IAppConfiguration configuration, IDataExporter dataExporter) 
        {            
            Logger = logger;
            Configuration = configuration;
            var subscriptionId = configuration.SubscriptionId();
            var base64EncodedCertificate = configuration.Base64EncodedManagementCertificate();
            MyCloudCredentials = getCredentials(subscriptionId, base64EncodedCertificate);
            Exporter = dataExporter;
        }

        /// <summary>
        /// Returns a list of Deployment objects for a given subscription
        /// </summary>
        /// <param name="serviceName">The name of the cloud service</param>
        /// <param name="slot">The slot being either Production or Staging</param>
        /// <returns></returns>
        protected DeploymentGetResponse GetAzureDeyployment(string serviceName, DeploymentSlot slot)
        {            
            ComputeManagementClient client = new ComputeManagementClient(MyCloudCredentials);
            try 
            {
                try
                {
                    return client.Deployments.GetBySlot(serviceName, slot);
                }
                catch (CloudException ex)
                {
                    if (ex.ErrorCode == "ResourceNotFound")
                    {
                        Logger.Warn(ex, String.Format("Resource not found during retrieval of Deployment object for service: {0}, {1}", serviceName, ex.ErrorCode));
                        return null;
                    }
                    else
                    {
                        Logger.Warn(ex, String.Format("Exception during retrieval of Deployment objects for the service: {0}, Errorcode: {1}", serviceName, ex.ErrorCode));
                        return null;
                    }
                }
            }
            catch (Exception e) 
            {
                Logger.Warn(e, String.Format("Exception during retrieval of Deployment objects for the service: {0}", serviceName));
                return null;
            }
        } 

        /// <summary>
        /// Returns a Credential object for the given subscription and Management Certificate that was extracted from the Azure Publish Settings file
        /// </summary>
        /// <param name="subscriptionId">The subscription Identifier</param>
        /// <param name="base64EncodedCertificate">The Base 64 Encoded version of the Management Certificate</param>
        /// <returns></returns>
        protected SubscriptionCloudCredentials getCredentials(string subscriptionId, string base64EncodedCertificate)
        {
            return new CertificateCloudCredentials(subscriptionId, new X509Certificate2(Convert.FromBase64String(base64EncodedCertificate)));
        }

    }

}
