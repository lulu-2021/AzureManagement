using System;
using ManageAzure.AzureModels;

namespace ManageAzure.Lib
{
    public interface IAzureManagementReporter
    {
        CloudServices GetAllCloudServices();

        VirtualMachines GetAllVirtualMachineRoles();

        ComputeRoles GetAllWebRoles();
    }
}
