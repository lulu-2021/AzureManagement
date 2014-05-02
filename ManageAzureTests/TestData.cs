using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ManageAzureTests
{
    public class TestData
    {
        public TestData() { }

        [XmlElement("SubscriptionId")]
        public string SubscriptionId { get; set; }

        [XmlElement("PublishSettingsFile")]
        public string PublishSettingsFile { get; set; }

        [XmlElement("DownloadResourcePath")]
        public string DownloadResourcePath { get; set; }

        [XmlElement("TestCloudServices")]
        public TestCloudServices _CloudServices { get; set; }

        [XmlElement("TestVirtualMachines")]
        public TestVirtualMachines _VirtualMachines { get; set; }

        [XmlElement("TestComputeRoles")]
        public TestComputeRoles _ComputeRoles { get; set; }
    }

    public class TestCloudServices 
    {
        [XmlElement("TestCloudService")]
        public List<TestCloudService> _CloudServices = new List<TestCloudService>();
    }

    public class TestVirtualMachines 
    {
        [XmlElement("TestVirtualMachine")]
        public List<TestVirtualMachine> _VirtualMachineRoles = new List<TestVirtualMachine>();
    }

    public class TestComputeRoles 
    {
        [XmlElement("TestRole")]
        public List<TestRole> _ComputeRoles = new List<TestRole>();
    }

    public class TestCloudService 
    {
        public TestCloudService() { }

        [XmlElement("CloudServiceName")]
        public string CloudServiceName { get; set; }
    }

    public class TestVirtualMachine
    {
        public TestVirtualMachine() { }

        [XmlElement("VmName")]
        public string VmName { get; set; }
    }

    public class TestRole 
    {
        public TestRole() { }

        [XmlElement("ServiceName")]
        public string ServiceName { get; set; }

        [XmlElement("InstanceName")]
        public string InstanceName { get; set; }
    }
}
