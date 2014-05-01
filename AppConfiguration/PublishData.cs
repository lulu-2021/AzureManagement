using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AppConfiguration
{
    /// <summary>
    /// class structure to deserialise the Azure PublishSettings file format to obtain the subscription ID and the Management Certificate in Base 64 Encoded format
    /// </summary>
    public class PublishData
    {
        PublishData() { }

        [XmlElement("PublishProfile")]
        public PublishProfile _Profile { get; set; }

        public class PublishProfile
        {
            PublishProfile() { }

            [XmlElement("Subscription")]
            public SubscriptionData _Subcription { get; set; }
        }
        public class SubscriptionData
        {
            SubscriptionData() { }

            [XmlAttribute("ServiceManagementUrl")]
            public string ServiceManagementUrl { get; set; }

            [XmlAttribute("Id")]
            public string Id { get; set; }

            [XmlAttribute("Name")]
            public string Name { get; set; }

            [XmlAttribute("ManagementCertificate")]
            public string ManagementCertificate { get; set; }
        }
    }
}
