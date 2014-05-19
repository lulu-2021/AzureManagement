using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AppConfiguration
{
    /// <summary>
    /// class structure to deserialise the Azure CostSettings file that contains the costings - this is so that costs can be married to each exportable item
    /// </summary>
    public class CostData
    {
        public CostData() { }

        [XmlElement("RoleHourlyRates")]
        public RoleHourlyRates Rates { get; set; }

        public double GetMyRate(string roleSize) 
        {
            var rateStr = GetMyRateAsString(roleSize);
            double rate = 0.0;
            double.TryParse(rateStr, out rate);
            return rate;
        }

        private string GetMyRateAsString(string roleType)
        {            
            switch (roleType)
            {
                case "ExtraSmall":
                    return Rates.ExtraSmall;

                case "Small":
                    return Rates.Small;

                case "Medium":
                    return Rates.Medium;

                case "Large":
                    return Rates.Large;

                case "ExtraLarge":
                    return Rates.ExtraLarge;

                case "A6":
                    return Rates.A6;

                case "A7":
                    return Rates.A7;

                default:
                    return "undefined";    
            }
        }

        public class RoleHourlyRates
        {
            RoleHourlyRates() { }

            [XmlElement("ExtraSmall")]
            public string ExtraSmall { get; set; }

            [XmlElement("Small")]
            public string Small { get; set; }

            [XmlElement("Medium")]
            public string Medium { get; set; }

            [XmlElement("Large")]
            public string Large { get; set; }

            [XmlElement("ExtraLarge")]
            public string ExtraLarge { get; set; }

            [XmlElement("A6")]
            public string A6 { get; set; }

            [XmlElement("A7")]
            public string A7 { get; set; }

        }
    }
}
