using System;
using System.Collections.Generic;

namespace ManageAzure.AzureModels
{
    public class ComputeRole
    {
        public string ServiceName { get; set; }
        public string InstanceName { get; set; }
        public string RoleName { get; set; }
        public string InstanceSize { get; set; }
        public string InstanceStatus { get; set; }
        public string OsVersion { get; set; }
        public string HourlyRate { get; set; }
        public string MonthlyRate { get; set; }

        public ComputeRole(string serviceName, string instanceName, string roleName, string instanceSize, string instanceStatus, string osVersion, double hourlyRate) 
        {
            ServiceName = serviceName;
            InstanceName = instanceName;
            RoleName = roleName;
            InstanceSize = instanceSize;
            InstanceStatus = instanceStatus;
            OsVersion = osVersion;
            HourlyRate = hourlyRate.ToString();
            MonthlyRate = (hourlyRate * 24 * 31).ToString();
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
