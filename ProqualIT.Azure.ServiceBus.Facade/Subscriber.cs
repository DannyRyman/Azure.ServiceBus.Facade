using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using ProqualIT.Azure.ServiceBus.Facade.Logging;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class Subscriber
    {
        private readonly ILog logger;

        private readonly SubscriptionClientFactory subscriptionClientFactory;

        public Subscriber(
            string topic,
            string connectionString)
        {
            this.logger = LogProvider.For<Subscriber>();
            subscriptionClientFactory = new SubscriptionClientFactory(topic, connectionString);
        }

        public void Subscribe<T>(string subscriptionName, Action<SubscriptionMessage<T>> messageReceived)
        {
            this.Subscribe<T>(subscriptionName, null, messageReceived);
        }

        public void Subscribe<T>(
            string subscriptionName,
            ISpecification filters,
            Action<SubscriptionMessage<T>> messageReceived)
        {
            var subscriptionClient = this.subscriptionClientFactory.CreateAndGetSubscription(
                subscriptionName,
                typeof(T),
                filters);

            Task.Run(() =>
            {
                this.ReceiveAndProcessMessages(subscriptionClient, messageReceived);
            });
        }

        private void ReceiveAndProcessMessages<T>(
            SubscriptionClient subscriptionClient,
            Action<SubscriptionMessage<T>> messageReceived)
        {
            while (true)
            {
                BrokeredMessage message = subscriptionClient.Receive();
                if (message == null)
                {
                    continue;
                }

                var subscriptionMessage = new SubscriptionMessage<T>(message);
                try
                {
                    messageReceived.Invoke(subscriptionMessage);

                    if (!subscriptionMessage.IsActioned)
                    {
                        message.Complete();
                    }
                }
                catch (Exception ex)
                {
                    this.logger.ErrorException("Exception encountered processing message", ex);
                }
            }
        }
    }
}