using System.Configuration;

namespace Tests.Support
{
    public static class Configuration
    {
        public static string SampleTopicName => ConfigurationManager.AppSettings["TopicName"];

        public static string AzureServiceBusConnectionString => ConfigurationManager.AppSettings["AzureServiceBusConnectionString"];
    }
}
