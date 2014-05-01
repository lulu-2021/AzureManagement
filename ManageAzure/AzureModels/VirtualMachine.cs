using System;
using System.Collections.Generic;

namespace ManageAzure.AzureModels
{
    public class VirtualMachine
    {
        public string RoleName { get; set; }
        public string RoleSize { get; set; }
        public string RoleType { get; set; }

        public VirtualMachine() { }
        public VirtualMachine(string roleName, string roleSize, string roleType) 
        {
            RoleName = roleName;
            RoleSize = roleSize;
            RoleType = roleType;
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
