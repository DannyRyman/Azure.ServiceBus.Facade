using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using ProqualIT.Azure.ServiceBus.Facade.Logging;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class Subscriber
    {
        private readonly ILog logger;

        private readonly SubscriptionClientFactory subscriptionClientFactory;

        private List<SubscriptionClient> clientRegistry; 

        public Subscriber(
            string topic,
            string connectionString)
        {
            this.logger = LogProvider.For<Subscriber>();
            subscriptionClientFactory = new SubscriptionClientFactory(topic, connectionString);
            clientRegistry = new List<SubscriptionClient>();
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

            clientRegistry.Add(subscriptionClient);

            OnMessageOptions options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromMinutes(1)
            };

            subscriptionClient.OnMessage((message) =>
            {
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
            } , options);
        }
    }
}