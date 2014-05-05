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
using System.IO;

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
            try 
            {
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
            catch (CloudException ce) 
            { 
                Logger.Warn(ce, String.Format("Exception during retrieval of Cloud Services Exception: {0}", ce)); 
            }
            return null;
        }

        /// <summary>
        /// Write all the Remote Desktop File objects out to disk
        /// </summary>
        /// <param name="rdpFiles">A list of the RDP objects that each contain a byte[] and a name</param>
        /// <param name="downloadPath">the OS disk path where the RDP files should be stored</param>
        /// <returns>boolean value of whether the download was successful</returns>
        public bool DownloadRdpFiles(List<RdpFileObject> rdpFiles, string downloadPath) 
        {
            try 
            {
                foreach (var rdpFile in rdpFiles)
                {
                    File.WriteAllBytes(downloadPath + "\\" + rdpFile.RdpFileName, rdpFile.RdpObject.RemoteDesktopFile.ToArray());                    
                }
                return true;
            }
            catch (Exception e) 
            {
                Logger.Warn(e, String.Format("Exception during the download of the RDP resource files - exception: {0}", e));
                return false;
            }
        }

        /// <summary>
        /// Downloads an RDP file for each of the Virtual Machine Roles in a given subscription
        /// </summary>
        /// <returns>List of RdpFileObject containing the RDP file name and a byte[] with the RDP file</returns>
        public List<RdpFileObject> GetAllVirtualMachineRdpFiles() 
        {
            ComputeManagementClient client = new ComputeManagementClient(MyCloudCredentials);
            try 
            {
                var rdpFiles = new List<RdpFileObject>();
                RdpFileObject rdpFile = null;
                var cloudServiceList = client.HostedServices.List();
                foreach (var cloudService in cloudServiceList)
                {
                    var deployment = GetAzureDeyployment(cloudService.ServiceName, DeploymentSlot.Production);
                    if (deployment != null)
                    {
                        if (deployment.Roles.Count > 0)
                        {
                            var deploymentName = deployment.Name;
                            foreach (var role in deployment.Roles) 
                            {
                                if (role.RoleType == VirtualMachineRoleType.PersistentVMRole.ToString())
                                {
                                    var rdpFileName = String.Format("rdp--{0}--{1}.rdp",cloudService.ServiceName,role.RoleName);
                                    rdpFile = new RdpFileObject(rdpFileName,client.VirtualMachines.GetRemoteDesktopFile(cloudService.ServiceName, deploymentName, role.RoleName));
                                    rdpFiles.Add(rdpFile);
                                }
                            }
                        }
                    }                    
                }
                return rdpFiles; 
            }
            catch (CloudException ce) 
            { 
                Logger.Warn(ce, String.Format("Exception durign retrieval of Virtual Machine RDP files - exception: {0}", ce)); 
            }
            return null;
        }

        /// <summary>
        /// Downloads an RDP file for each of the Elastic (Web and Worker based) Roles in a given subscription
        /// </summary>
        /// <returns>List of RdpFileObject containing the RDP file name and a byte[] with the RDP file</returns>
        public List<RdpFileObject> GetAllElasticRoleRdpFiles() 
        {
            ComputeManagementClient client = new ComputeManagementClient(MyCloudCredentials);
            var rdpFiles = new List<RdpFileObject>();
            RdpFileObject rdpFile = null;

            var hostedServices = client.HostedServices.List();
            if (hostedServices.Count() > 0) 
            {
                foreach (var service in hostedServices)
                {
                    var deployment = GetAzureDeyployment(service.ServiceName, DeploymentSlot.Production);
                    if (deployment != null)
                    {
                        var instances = client.Deployments.GetBySlot(service.ServiceName, DeploymentSlot.Production).RoleInstances;
                        if (instances != null)
                        {
                            if (instances.Count > 0)
                            {
                                foreach (RoleInstance instance in instances)
                                {
                                    var rdpFileName = String.Format("rdp--{0}--{1}--{2}.rdp", service.ServiceName, deployment.Name, instance.InstanceName);
                                    rdpFile = new RdpFileObject(rdpFileName, client.VirtualMachines.GetRemoteDesktopFile(service.ServiceName, deployment.Name, instance.InstanceName));
                                    rdpFiles.Add(rdpFile);
                                }
                            }
                        }
                    }
                }
                return rdpFiles;
            }
            return null;                
        }

        /// <summary>
        /// Downloads an RDP file for each of the Elastic (Web and Worker based) Roles in a given subscription
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns>List of RdpFileObject containing the RDP file name and a byte[] with the RDP file</returns>
        public List<RdpFileObject> GetAllElasticRoleRdpFilesForService(string serviceName)
        {
            ComputeManagementClient client = new ComputeManagementClient(MyCloudCredentials);
            var rdpFiles = new List<RdpFileObject>();
            RdpFileObject rdpFile = null;
            var deployment = GetAzureDeyployment(serviceName, DeploymentSlot.Production);
            if (deployment != null)
            {
                var deployments = client.Deployments.GetBySlot(serviceName, DeploymentSlot.Production);
                if (deployments != null) 
                {
                    var instances = deployments.RoleInstances;
                    if (instances != null)
                    {
                        if (instances.Count > 0)
                        {
                            foreach (RoleInstance instance in instances)
                            {
                                var rdpFileName = String.Format("rdp--{0}--{1}--{2}.rdp", serviceName, deployment.Name, instance.InstanceName);
                                rdpFile = new RdpFileObject(rdpFileName, client.VirtualMachines.GetRemoteDesktopFile(serviceName, deployment.Name, instance.InstanceName));
                                rdpFiles.Add(rdpFile);
                            }
                        }
                        return rdpFiles;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Downloads an RDP file for each of the Virtual Machine Roles in a given subscription & Cloud Service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns>List of RdpFileObject containing the RDP file name and a byte[] with the RDP file</returns>
        public List<RdpFileObject> GetAllVirtualMachineRdpFilesForService(string serviceName) 
        {
            ComputeManagementClient client = new ComputeManagementClient(MyCloudCredentials);
            try 
            {
                var rdpFiles = new List<RdpFileObject>();
                RdpFileObject rdpFile = null;
                var deployment = GetAzureDeyployment(serviceName, DeploymentSlot.Production);
                if (deployment != null)
                {
                    if (deployment.Roles.Count > 0)
                    {
                        var deploymentName = deployment.Name;
                        foreach (var role in deployment.Roles)
                        {
                            if (role.RoleType == VirtualMachineRoleType.PersistentVMRole.ToString())
                            {
                                var rdpFileName = String.Format("rdp--{0}--{1}.rdp", serviceName, role.RoleName);
                                rdpFile = new RdpFileObject(rdpFileName, client.VirtualMachines.GetRemoteDesktopFile(serviceName, deploymentName, role.RoleName));
                                rdpFiles.Add(rdpFile);
                            }
                        }
                        return rdpFiles;
                    }                    
                }                
            }
            catch (CloudException ce) 
            {
                Logger.Warn(ce, String.Format("Exception durign retrieval of Virtual Machine RDP files - exception: {0}", ce));
            }
            return null;
        }


        /// <summary>
        /// Returns a list of Permanent Virtual Machine roles. Each Virtual Machine object contains some metadata of the actual virtual machine
        /// </summary>
        /// <returns>A list of Permanent Virtual Machine Roles</returns>
        public VirtualMachines GetAllVirtualMachineRoles()
        {
            ComputeManagementClient client = new ComputeManagementClient(MyCloudCredentials);
            try 
            {
                VirtualMachines vms = new VirtualMachines(new List<VirtualMachine>());
                var hostedServices = client.HostedServices.List();
                foreach (var service in hostedServices)
                {
                    var deployment = GetAzureDeyployment(service.ServiceName, DeploymentSlot.Production);
                    if (deployment != null)
                    {
                        if (deployment.Roles.Count > 0)
                        {
                            VirtualMachine vm = null;
                            foreach (var role in deployment.Roles)
                            {                                
                                if (role.RoleType == VirtualMachineRoleType.PersistentVMRole.ToString())
                                {
                                    vm = new VirtualMachine(role.RoleName, role.RoleSize, role.RoleType);
                                    vms.Add(vm);
                                }
                            }
                        }
                    }
                }
                return vms;
            }
            catch (CloudException ce) 
            { 
                Logger.Warn(ce, String.Format("Exception during retrieval of Virtual Machine Roles Exception: {0}", ce)); 
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
            ComputeRoles roles = null;
            try 
            {
                roles = new ComputeRoles(new List<ComputeRole>());
                var hostedServices = client.HostedServices.List();
                foreach (var service in hostedServices)
                {
                    var deployments = client.Deployments.GetBySlot(service.ServiceName, DeploymentSlot.Production);
                    if (deployments != null) 
                    {
                        var instances = deployments.RoleInstances;
                        if (instances.Count > 0)
                        {
                            ComputeRole role = null;
                            foreach (RoleInstance instance in instances)
                            {
                                role = new ComputeRole(service.ServiceName, instance.HostName, instance.InstanceName, instance.RoleName, instance.InstanceSize, instance.InstanceStatus);
                                roles.Add(role);
                            }
                        }
                    }
                }
            }
            catch (CloudException ce) 
            {
                Logger.Warn(ce, String.Format("Exception during retrieval of Web Roles Exception: {0}",ce));
            }
            return roles;
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
        private SubscriptionCloudCredentials getCredentials(string subscriptionId, string base64EncodedCertificate)
        {
            return new CertificateCloudCredentials(subscriptionId, new X509Certificate2(Convert.FromBase64String(base64EncodedCertificate)));
        }

    }

}
