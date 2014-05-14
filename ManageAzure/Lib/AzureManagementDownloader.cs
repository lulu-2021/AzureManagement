using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AppLogging;
using AppConfiguration;
using AppDataExport;
using ManageAzure.AzureModels;

namespace ManageAzure.Lib
{
    public class AzureManagementDownloader : AzureManagement, IAzureManagementDownloader
    {
        public AzureManagementDownloader(IMlogger logger, IAppConfiguration configuration, IDataExporter dataExporter)
            : base(logger, configuration, dataExporter)
        { }

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
                                    var rdpFileName = String.Format("rdp--{0}--{1}.rdp", cloudService.ServiceName, role.RoleName);
                                    try { 
                                        rdpFile = new RdpFileObject(rdpFileName, client.VirtualMachines.GetRemoteDesktopFile(cloudService.ServiceName, deploymentName, role.RoleName));
                                        rdpFiles.Add(rdpFile);
                                    }
                                    catch(CloudException cerdp)
                                    {
                                        Logger.Warn(cerdp, String.Format("Exception durign retrieval of Virtual Machine RDP file - possibly no endpoint? - exception: {0}", cerdp));
                                    }
                                }
                            }
                        }
                    }
                }
                return rdpFiles;
            }
            catch (CloudException ce)
            {
                Logger.Warn(ce, String.Format("Exception durign retrieval of Cloud Service List - exception: {0}", ce));
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
            try
            {
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
            }
            catch (CloudException ce) 
            {
                Logger.Warn(ce, String.Format("Exception durign retrieval of Web Role RDP files - exception: {0}", ce));
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
    }
}
