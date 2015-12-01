using System;
using Newtonsoft.Json.Converters;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class ProxyConverter<T> : CustomCreationConverter<T>
    {
        private readonly string typeName;

        public ProxyConverter(string typeName)
        {
            this.typeName = typeName.Split(';')[0];
        }

        public override T Create(Type objectType)
        {
            var type = Type.GetType(this.typeName);
            if (type == null)
            {
                throw new InvalidOperationException(string.Format("Unable to create type: {0}", this.typeName));
            }

            return (T)Activator.CreateInstance(type);
        }
    }
}