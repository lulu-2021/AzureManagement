using System;
using System.Collections.Generic;

namespace ManageAzure.AzureModels
{
    public class ComputeRole
    {
        public string ServiceName { get; set; }
        public string HostName { get; set; }
        public string InstanceName { get; set; }
        public string RoleName { get; set; }
        public string InstanceSize { get; set; }
        public string InstanceStatus { get; set; }
        public int HourlyRate { get; set; }
        public int MonthlyRate { get; set; }

        public ComputeRole(string serviceName, string hostName, string instanceName, string roleName, string instanceSize, string instanceStatus, int hourlyRate) 
        {
            ServiceName = serviceName;
            HostName = hostName;
            InstanceName = instanceName;
            RoleName = roleName;
            InstanceSize = instanceSize;
            InstanceStatus = instanceStatus;
            HourlyRate = hourlyRate;
            MonthlyRate = hourlyRate * 24 * 31;
        }
    }
    public class ComputeRoles
    {
        public List<ComputeRole> MyComputeRoles { get; set; }

        public ComputeRoles(List<ComputeRole> roles) 
        {
            MyComputeRoles = roles;
        }
        public void Add(ComputeRole role) 
        {
            MyComputeRoles.Add(role);
        }

        public bool Contains(string instanceName, string serviceName) 
        {
            foreach (var role in MyComputeRoles) 
            {
                if ((role.InstanceName == instanceName) && (role.ServiceName == serviceName))
                    return true;
            }
            return false;
        }
    }
}
