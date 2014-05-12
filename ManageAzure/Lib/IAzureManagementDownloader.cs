using System;
using System.Collections.Generic;
using ManageAzure.AzureModels;

namespace ManageAzure.Lib
{
    public interface IAzureManagementDownloader
    {
        bool DownloadRdpFiles(List<RdpFileObject> rdpFiles, string downloadPath);

        List<RdpFileObject> GetAllVirtualMachineRdpFiles();

        List<RdpFileObject> GetAllElasticRoleRdpFiles();

        List<RdpFileObject> GetAllElasticRoleRdpFilesForService(string serviceName);

        List<RdpFileObject> GetAllVirtualMachineRdpFilesForService(string serviceName);
    }
}
