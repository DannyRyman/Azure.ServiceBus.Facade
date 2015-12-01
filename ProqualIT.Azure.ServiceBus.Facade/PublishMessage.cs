using System.Collections.Generic;
using System.IO;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class PublishMessage
    {
        private PublishMessage(
            string messageType,
            Stream messageBody,
            string messageId,
            IDictionary<string, string> properties)
        {
            this.MessageType = messageType;
            this.MessageBody = messageBody;
            this.MessageId = messageId;
            this.Properties = properties;
        }

        public string MessageType { get; private set; }

        public Stream MessageBody { get; private set; }

        public string MessageId { get; private set; }

        public IDictionary<string, string> Properties { get; private set; }

        public static PublishMessage Create<T>(T body, string messageId, IDictionary<string, string> properties = null)
        {
            var messageType = Reflect.GetTypeNameOfConcreteAndParentTypes(body.GetType());
            var bodyStream = new JsonSerializer().Serialize(body);
            return new PublishMessage(messageType, bodyStream, messageId, properties);
        }

        public static PublishMessage Create(
            string messageType,
            Stream bodyRaw,
            string messageId,
            IDictionary<string, string> properties = null)
        {
            return new PublishMessage(messageType, bodyRaw, messageId, properties);
        }
    }
}