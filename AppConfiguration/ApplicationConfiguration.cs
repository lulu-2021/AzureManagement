using System;
using System.IO;
using System.Xml.Serialization;

namespace AppConfiguration
{
    public interface IAppConfiguration 
    {
        string SubscriptionId();
        string Base64EncodedManagementCertificate();
        CostData GetAzureRates();
    }

    public class ApplicationConfiguration : IAppConfiguration
    {
        private PublishData AzurePublishData { get; set; }
        protected CostData AzureCostData { get; set; }

        public ApplicationConfiguration(string settingsFile, string costDataFile)
        {
            AzurePublishData = ReadAzurePublishSettingsFile(settingsFile);
            AzureCostData = ReadAzureCostDataFile(costDataFile);
        }

        public string SubscriptionId()
        {
            return AzurePublishData._Profile._Subcription.Id;
        }

        public string Base64EncodedManagementCertificate()
        {
            return AzurePublishData._Profile._Subcription.ManagementCertificate;
        }

        public CostData GetAzureRates() 
        {
            if (AzureCostData != null) 
            {
                return AzureCostData;
            }
            // fall back to just passing an empty object so that the export proces does not fail!
            return new CostData();
        }

        /// <summary>
        /// Deserialises the Azure Publish Settings File into a tree of objects that hold the Subscription Id and the Base 64 encoded Management Certificate
        /// </summary>
        /// <param name="settingsPath">Full path and file name of the Azure Publish Settings File</param>
        /// <returns>A PublishData object containing the Subscription Id and the Management Certificate</returns>
        private PublishData ReadAzurePublishSettingsFile(string settingsPath)
        {
            TextReader reader = null;
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(PublishData));
                reader = new StreamReader(@settingsPath);
                object obj = deserializer.Deserialize(reader);
                PublishData XmlData = (PublishData)obj;
                return XmlData;
            }
            catch (InvalidOperationException ioe)
            {
                var message = String.Format("Error Reading the Azure Publish Settings File{0}", ioe);
                throw ioe;
            }
            finally
            {
                reader.Close();
            }
        }

        private CostData ReadAzureCostDataFile(string costDataPath)
        {
            TextReader reader = null;
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(CostData));
                reader = new StreamReader(@costDataPath);
                object obj = deserializer.Deserialize(reader);
                CostData XmlData = (CostData)obj;
                return XmlData;
            }
            catch (InvalidOperationException ioe)
            {
                var message = String.Format("Error Reading the Azure Cost Data File{0}", ioe);
                throw ioe;
            }
            finally
            {
                reader.Close();
            }
        }

    }

}
