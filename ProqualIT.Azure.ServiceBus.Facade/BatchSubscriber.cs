using Microsoft.ServiceBus.Messaging;
using ProqualIT.Azure.ServiceBus.Facade.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public abstract class BatchSubscriber
    {
        protected readonly int BatchSize;
        private readonly ILog log;

        private readonly SubscriptionClientFactory subscriptionClientFactory;

        protected BatchSubscriber(
            string topic,
            int batchSize,
            string connectionString)
        {
            this.BatchSize = batchSize;
            this.log = LogProvider.For<BatchSubscriber>();
            this.subscriptionClientFactory = new SubscriptionClientFactory(topic, connectionString);
        }

        public void ProcessMessages<T>(
            SubscriptionClient subscriptionClient,
            Action<IEnumerable<T>> messagesReceived,
            IEnumerable<BrokeredMessage> messages) where T : SubscriptionMessage
        {
            var brokeredMessages = messages as BrokeredMessage[] ?? messages.ToArray();

            if (brokeredMessages.Any())
            {
                var wrappedMessages = brokeredMessages.Select(x => (T)Activator.CreateInstance(typeof(T), x));
                var brokeredMessageWrappers = wrappedMessages as T[] ?? wrappedMessages.ToArray();

                try
                {
                    messagesReceived.Invoke(brokeredMessageWrappers);

                    var completionTokens = brokeredMessageWrappers.Where(x => !x.IsActioned).Select(x => x.LockToken);

                    var lockTokens = completionTokens as Guid[] ?? completionTokens.ToArray();

                    if (lockTokens.Any())
                    {
                        subscriptionClient.CompleteBatch(lockTokens);
                    }
                }
                catch (Exception ex)
                {
                    this.log.ErrorException("Exception encountered processing messages", ex);
                }
            }
        }

        protected virtual void StartProcessing<T>(SubscriptionClient subscriptionClient, Action<IEnumerable<T>> messagesReceived)
            where T : SubscriptionMessage
        {
            while (true)
            {
                var messages = subscriptionClient.ReceiveBatch(this.BatchSize);
                this.ProcessMessages(subscriptionClient, messagesReceived, messages);
            }
        }

        protected void InitialiseSubscription<T>(string subscriptionName, ISpecification filter, Action<IEnumerable<T>> messagesReceived) where T : SubscriptionMessage
        {
            var subscriptionClient = this.subscriptionClientFactory.CreateAndGetSubscription(subscriptionName, filter);
            Task.Run(() => this.StartProcessing(subscriptionClient, messagesReceived));
        }

        protected void InitialiseSubscription<T>(string subscriptionName, Type type, ISpecification filter, Action<IEnumerable<T>> messagesReceived) where T : SubscriptionMessage
        {
            var subscriptionClient = this.subscriptionClientFactory.CreateAndGetSubscription(subscriptionName, type, filter);
            Task.Run(() => this.StartProcessing(subscriptionClient, messagesReceived));
        }
    }
}
