using System;
using System.Collections.Generic;

namespace ManageAzure.AzureModels
{
    public class CloudService
    {
        public string ServiceName { get; set; }
        public string Uri { get; set; }

        public CloudService() { }
        public CloudService(string serviceName, string uri) 
        {
            ServiceName = serviceName;
            Uri = uri;
        }
    }

    public class CloudServices 
    {
        public List<CloudService> MyCloudServices { get; set; }
        public CloudServices(List<CloudService> services) 
        {
            MyCloudServices = services;
        }
        public void Add(CloudService service) 
        {
            MyCloudServices.Add(service);
        }

        public bool Contains(string serviceName) 
        {

            foreach (var service in MyCloudServices) 
            {
                if (service.ServiceName == serviceName)
                    return true;
            }
            return false;
        }
    }

}
