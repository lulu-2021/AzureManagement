using Microsoft.WindowsAzure.Management.Compute.Models;
using System;

namespace ManageAzure.AzureModels
{
    public class RdpFileObject
    {
        public string RdpFileName { get; set; }
        public VirtualMachineGetRemoteDesktopFileResponse RdpObject { get; set; }

        public RdpFileObject(string rdpFileName, VirtualMachineGetRemoteDesktopFileResponse rdpObject)
        {
            RdpFileName = rdpFileName;
            RdpObject = rdpObject;
        }
    }
}
