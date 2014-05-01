using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ManageAzure.AzureModels;
using AppLogging;
using AppConfiguration;

namespace ManageAzure.Lib
{
    public class AzureManagement
    {
        public SubscriptionCloudCredentials MyCloudCredentials { get; set; }
        public IMlogger Logger { get; set; }
        public IAppConfiguration Configuration { get; set;}

        public AzureManagement(IMlogger logger, IAppConfiguration configuration) 
        {
            Logger = logger;
            Configuration = configuration;
            var subscriptionId = configuration.SubscriptionId();
            var base64EncodedCertificate = configuration.Base64EncodedManagementCertificate();
            MyCloudCredentials = getCredentials(subscriptionId, base64EncodedCertificate);            
        }

        /// <summary>
        ///  Returns a list of Cloud Cervice objects for a given subscription. Each cloud service contains some metadata of the actual cloud service
        /// </summary>
        /// <returns>A list of Cloud Serviec objects</returns>
        public CloudServices GetAllCloudServices()
        {
            var subscriptionId = Configuration.SubscriptionId();
            var certificate = Configuration.Base64EncodedManagementCertificate();

            ComputeManagementClient client = new ComputeManagementClient(getCredentials(subscriptionId,certificate));
            var cloudServiceList = client.HostedServices.List();
            CloudServices services = new CloudServices(new List<CloudService>());
            CloudService service = null;
            foreach (var cloudService in cloudServiceList)
            {               
                service = new CloudService(cloudService.ServiceName, cloudService.Uri.ToString());
                services.Add(service);
            }
            return services;
        }

        /// <summary>
        /// Returns a list of Permanent Virtual Machine roles. Each Virtual Machine object contains some metadata of the actual virtual machine
        /// </summary>
        /// <returns>A list of Permanent Virtual Machine Roles</returns>
        public VirtualMachines GetAllVirtualMachineRoles()
        {
            ComputeManagementClient client = new ComputeManagementClient(MyCloudCredentials);
            var hostedServices = client.HostedServices.List();
            foreach (var service in hostedServices)
            {
                var deployment = GetAzureDeyployment(service.ServiceName, DeploymentSlot.Production);
                if (deployment != null)
                {
                    if (deployment.Roles.Count > 0)
                    {
                        VirtualMachines vms = new VirtualMachines(new List<VirtualMachine>());
                        VirtualMachine vm = null;
                        foreach (var role in deployment.Roles)
                        {
                            if (role.RoleType == VirtualMachineRoleType.PersistentVMRole.ToString())
                            {
                                vm = new VirtualMachine(role.RoleName, role.RoleSize, role.RoleType);
                                vms.Add(vm);
                            }

                        }
                        return vms;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a list of Compute Roles for all cloud services in a given subscription. The Compute Role list will include Web and Worker Roles.
        /// </summary>
        /// <returns>A list of Compute Roles</returns>
        public ComputeRoles GetAllWebRoles()
        {
            ComputeManagementClient client = new ComputeManagementClient(MyCloudCredentials);
            var hostedServices = client.HostedServices.List();
            foreach (var service in hostedServices)
            {
                var instances = client.Deployments.GetBySlot(service.ServiceName, DeploymentSlot.Production).RoleInstances;
                if (instances.Count > 0) 
                {
                    ComputeRoles roles = new ComputeRoles(new List<ComputeRole>());
                    ComputeRole role = null;
                    foreach (RoleInstance instance in instances) 
                    {
                        role = new ComputeRole(service.ServiceName, instance.HostName, instance.InstanceName, instance.RoleName, instance.InstanceSize, instance.InstanceStatus);
                        roles.Add(role);                        
                    }
                    return roles;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a list of Deployment objects for a given subscription
        /// </summary>
        /// <param name="serviceName">The name of the cloud service</param>
        /// <param name="slot">The slot being either Production or Staging</param>
        /// <returns></returns>
        private DeploymentGetResponse GetAzureDeyployment(string serviceName, DeploymentSlot slot)
        {            
            ComputeManagementClient client = new ComputeManagementClient(MyCloudCredentials);
            try
            {
                return client.Deployments.GetBySlot(serviceName, slot);
            }
            catch (CloudException ex)
            {
                if (ex.ErrorCode == "ResourceNotFound")
                {
                    Logger.Warn(ex, ex.ErrorCode);
                    return null;
                }
                else
                {
                    throw ex;
                }
            }
        } 

        /// <summary>
        /// Returns a Credential object for the given subscription and Management Certificate that was extracted from the Azure Publish Settings file
        /// </summary>
        /// <param name="subscriptionId">The subscription Identifier</param>
        /// <param name="base64EncodedCertificate">The Base 64 Encoded version of the Management Certificate</param>
        /// <returns></returns>
        private SubscriptionCloudCredentials getCredentials(string subscriptionId, string base64EncodedCertificate)
        {
            return new CertificateCloudCredentials(subscriptionId, new X509Certificate2(Convert.FromBase64String(base64EncodedCertificate)));
        }

    }
}
