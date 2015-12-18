using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using ProqualIT.Azure.ServiceBus.Facade.Logging;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class AdvancedBatchSubscriber<T> : SimpleBatchSubscriber<T>
    {
        private readonly TimeSpan maxTimeToWaitForBatch;
        private readonly ILog log;

        public AdvancedBatchSubscriber(string topic, int batchSize, TimeSpan maxTimeToWaitForBatch, string connectionString)
            : base(topic, batchSize, connectionString)
        {
            this.log = LogProvider.For<AdvancedBatchSubscriber<T>>();
            this.maxTimeToWaitForBatch = maxTimeToWaitForBatch;
        }

        protected override void StartProcessing<T1>(SubscriptionClient subscriptionClient, Action<IEnumerable<T1>> messagesReceived)
        {
            log.Info($"Initialising batcher. BatchSize={BatchSize}, MaxTimeToWaitForBatch={maxTimeToWaitForBatch}");
            var batcher = new Batcher<BrokeredMessage>(messages => ProcessMessages(subscriptionClient, messagesReceived, messages), BatchSize, this.maxTimeToWaitForBatch);
            log.Info("Batcher initialised");

            while (true)
            {
                log.Info("Waiting to receive batch");

                var batch = subscriptionClient.ReceiveBatch(BatchSize);

                var brokeredMessages = batch as BrokeredMessage[] ?? batch.ToArray();

                log.Info($"Received batch of {brokeredMessages.Length} messages");

                foreach (var brokeredMessage in brokeredMessages)
                {
                    batcher.Post(brokeredMessage);
                }
            }
        }
    }
}
