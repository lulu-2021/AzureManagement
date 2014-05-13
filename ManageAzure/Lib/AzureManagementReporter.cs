﻿using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;
using System;
using System.Collections.Generic;
using AppLogging;
using AppConfiguration;
using AppDataExport;
using ManageAzure.AzureModels;

namespace ManageAzure.Lib
{
    public class AzureManagementReporter :AzureManagement, IAzureManagementReporter
    {
        public AzureManagementReporter(IMlogger logger, IAppConfiguration configuration, IDataExporter dataExporter)
            : base(logger, configuration, dataExporter)
        { }

        /// <summary>
        /// Exports a list of Cloud Service objects for a given subscription. Each cloud service contains some metadata of the actual cloud service
        /// Since the "Exporter" was injected - we do not care how this is actually implemented - that decision is left to the exporter!
        /// </summary>
        public void ExportAllCloudServices() 
        {
            // first get the correct header names and export these
            var _serviceNameHeader = MemberUtils.GetPropertyName<CloudService>(cs => cs.ServiceName);
            var _serviceUriHeader = MemberUtils.GetPropertyName<CloudService>(cs => cs.Uri);
            IList<string> dataHeaders = new List<string> { _serviceNameHeader, _serviceUriHeader };
            Exporter.ExportHeader(dataHeaders);

            // then retrieve the data values and export these
            foreach (var cloudService in GetAllCloudServices().MyCloudServices) 
            {
                IList<string> dataCols = new List<string> { cloudService.ServiceName, cloudService.Uri };
                Exporter.ExportDataRow(dataCols);
            }
        }

        /// <summary>
        ///  Returns a list of Cloud Cervice objects for a given subscription. Each cloud service contains some metadata of the actual cloud service
        /// </summary>
        /// <returns>A list of Cloud Serviec objects</returns>
        public CloudServices GetAllCloudServices()
        {
            var subscriptionId = Configuration.SubscriptionId();
            var certificate = Configuration.Base64EncodedManagementCertificate();
            ComputeManagementClient client = new ComputeManagementClient(getCredentials(subscriptionId, certificate));
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
        /// Exports a list of Virtual Machine Role objects for a given subscription. Each role object contains some metadata of the actual virtual machine role
        /// Since the "Exporter" was injected - we do not care how this is actually implemented - that decision is left to the exporter!
        /// </summary>
        public void ExportAllVirtualMachineRoles() 
        {
            // first get the correct header names and export these
            var _roleNameHeader = MemberUtils.GetPropertyName<VirtualMachine>(vm => vm.RoleName);
            var _roleSizeHeader = MemberUtils.GetPropertyName<VirtualMachine>(vm => vm.RoleSize);
            var _roleTypeHeader = MemberUtils.GetPropertyName<VirtualMachine>(vm => vm.RoleType);
            IList<string> dataHeaders = new List<string> { _roleNameHeader, _roleSizeHeader, _roleTypeHeader };
            Exporter.ExportHeader(dataHeaders);

            // then retrieve the data values and export these
            foreach (var vm in GetAllVirtualMachineRoles().MyVirtualMachines )
            {
                IList<string> dataCols = new List<string> { vm.RoleName, vm.RoleSize, vm.RoleType };
                Exporter.ExportDataRow(dataCols);
            }
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
        /// Exports a list of Web Role objects for a given subscription. Each role object contains some metadata of the actual web role
        /// Since the "Exporter" was injected - we do not care how this is actually implemented - that decision is left to the exporter!
        /// </summary>
        public void ExportAllWebRoles() 
        {
            // first get the correct header names and export these
            var _hostNameHeader = MemberUtils.GetPropertyName<ComputeRole>(cr => cr.HostName);
            var _instanceNameHeader = MemberUtils.GetPropertyName<ComputeRole>(cr => cr.InstanceName);
            var _instanceSizeHeader = MemberUtils.GetPropertyName<ComputeRole>(cr => cr.InstanceSize);
            var _instanceStatusHeader = MemberUtils.GetPropertyName<ComputeRole>(cr => cr.InstanceStatus);
            var _roleNameHeader = MemberUtils.GetPropertyName<ComputeRole>(cr => cr.RoleName);
            var _serviceNameHeader = MemberUtils.GetPropertyName<ComputeRole>(cr => cr.ServiceName);
            IList<string> dataHeaders = new List<string> { _hostNameHeader, _instanceNameHeader, _instanceSizeHeader, _instanceStatusHeader, _roleNameHeader, _serviceNameHeader};
            Exporter.ExportHeader(dataHeaders);

            // then retrieve the data values and export these
            foreach (var cr in GetAllWebRoles().MyComputeRoles)
            {
                IList<string> dataCols = new List<string> { cr.HostName, cr.InstanceName, cr.InstanceSize, cr.InstanceStatus, cr.RoleName, cr.ServiceName };
                Exporter.ExportDataRow(dataCols);
            }
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
                Logger.Warn(ce, String.Format("Exception during retrieval of Web Roles Exception: {0}", ce));
            }
            return roles;
        }
    }
}