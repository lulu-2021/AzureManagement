using System;
using System.Collections.Generic;

namespace ManageAzure.AzureModels
{
    public class VirtualMachine
    {
        public string RoleName { get; set; }
        public string RoleSize { get; set; }
        public string RoleType { get; set; }
        public string OsVersion { get; set; }
        public string HourlyRate { get; set; }
        public string MonthlyRate { get; set; }

        public VirtualMachine() { }
        public VirtualMachine(string roleName, string roleSize, string roleType, string osVersion, double hourlyRate) 
        {
            RoleName = roleName;
            RoleSize = roleSize;
            RoleType = roleType;
            OsVersion = osVersion;
            HourlyRate = hourlyRate.ToString();
            MonthlyRate = (hourlyRate * 24 * 31).ToString();
        }
    }

    public class VirtualMachines 
    {
        public List<VirtualMachine> MyVirtualMachines { get; set; }

        public VirtualMachines(List<VirtualMachine> vms) 
        {
            MyVirtualMachines = vms;
        }
        public void Add(VirtualMachine vm) 
        {
            MyVirtualMachines.Add(vm);
        }

        public bool Contains(string roleName) 
        {
            foreach (var vm in MyVirtualMachines) 
            {
                if (vm.RoleName == roleName)
                    return true;
            }
            return false;
        }
    }
}
