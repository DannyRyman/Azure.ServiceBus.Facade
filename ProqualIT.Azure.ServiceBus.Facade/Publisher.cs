using System.Collections.Generic;
using Microsoft.ServiceBus.Messaging;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class Publisher : IPublisher
    {
        private readonly TopicClient client;

        public Publisher(string topicName, string connectionString)
        {
            this.client = TopicClient.CreateFromConnectionString(connectionString, topicName);
        }

        public void SendBatch(IEnumerable<PublishMessage> messages)
        {
            var brokeredMessages = new List<BrokeredMessage>();

            foreach (var message in messages)
            {
                var brokeredMessage = new BrokeredMessage(message.MessageBody) { MessageId = message.MessageId };
                AppendCustomProperties(message.MessageType, message.Properties, brokeredMessage);
                brokeredMessages.Add(brokeredMessage);
            }

            this.client.SendBatch(brokeredMessages);
        }

        public void Send(PublishMessage message)
        {
            var brokeredMessage = new BrokeredMessage(message.MessageBody) { MessageId = message.MessageId };
            AppendCustomProperties(message.MessageType, message.Properties, brokeredMessage);
            this.client.Send(brokeredMessage);
        }

        private static void AppendCustomProperties(
            string messageType,
            IEnumerable<KeyValuePair<string, string>> properties,
            BrokeredMessage brokeredMessage)
        {
            brokeredMessage.Properties.Add("Core.MessageType", messageType);
            brokeredMessage.Properties.Add("Core.DeadLetterResubmitSubscription", string.Empty);

            if (properties == null)
            {
                return;
            }

            foreach (var property in properties)
            {
                brokeredMessage.Properties.Add(property.Key, property.Value);
            }
        }
    }
}
