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
            BrokeredMessage[] brokeredMessages) where T : SubscriptionMessage
        {            
            if (brokeredMessages.Any())
            {
                log.Info("Wrapping messages for consumption");

                var wrappedMessages = brokeredMessages.Select(x => (T)Activator.CreateInstance(typeof(T), x));
                var brokeredMessageWrappers = wrappedMessages as T[] ?? wrappedMessages.ToArray();

                try
                {
                    log.Info("Calling message consumer");

                    messagesReceived.Invoke(brokeredMessageWrappers);

                    log.Info("Checking completion status");

                    var completionTokens = brokeredMessageWrappers.Where(x => !x.IsActioned).Select(x => x.LockToken);

                    var lockTokens = completionTokens as Guid[] ?? completionTokens.ToArray();

                    if (lockTokens.Any())
                    {
                        log.Info("Completing batch");
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
            log.Info("Starting processing");

            while (true)
            {
                log.Info($"Waiting for message batch.  BatchSize = {this.BatchSize}");
                var messages = subscriptionClient.ReceiveBatch(this.BatchSize);

                var brokeredMessages = messages as BrokeredMessage[] ?? messages.ToArray();

                log.Info($"Received {brokeredMessages.Count()} messages");
                this.ProcessMessages(subscriptionClient, messagesReceived, brokeredMessages);
            }
        }

        protected void InitialiseSubscription<T>(string subscriptionName, ISpecification filter, Action<IEnumerable<T>> messagesReceived) where T : SubscriptionMessage
        {
            log.Info($"Initialising subscription.  Name={subscriptionName}, filter = {filter}");
            var subscriptionClient = this.subscriptionClientFactory.CreateAndGetSubscription(subscriptionName, filter);
            log.Info("Subscription initialised.");
            Task.Run(() => this.StartProcessing(subscriptionClient, messagesReceived));
        }

        protected void InitialiseSubscription<T>(string subscriptionName, Type type, ISpecification filter, Action<IEnumerable<T>> messagesReceived) where T : SubscriptionMessage
        {
            log.Info($"Initialising subscription.  Name={subscriptionName}, Type = {type}, filter = {filter}");
            var subscriptionClient = this.subscriptionClientFactory.CreateAndGetSubscription(subscriptionName, type, filter);
            log.Info("Subscription initialised.");
            Task.Run(() => this.StartProcessing(subscriptionClient, messagesReceived));
        }
    }
}
