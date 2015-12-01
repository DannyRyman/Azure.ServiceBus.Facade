using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class GuidelineJsonSerializerSettings : JsonSerializerSettings
    {
        public GuidelineJsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver();
            TypeNameHandling = TypeNameHandling.None;
            DateFormatHandling = DateFormatHandling.IsoDateFormat;
            NullValueHandling = NullValueHandling.Ignore;
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            Converters.Add(new StringEnumConverter());
        }
    }
}