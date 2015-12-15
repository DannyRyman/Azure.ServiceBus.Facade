using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class AdvancedBatchSubscriber<T> : SimpleBatchSubscriber<T>
    {
        private readonly TimeSpan maxTimeToWaitForBatch;

        public AdvancedBatchSubscriber(string topic, int batchSize, TimeSpan maxTimeToWaitForBatch, string connectionString)
            : base(topic, batchSize, connectionString)
        {
            this.maxTimeToWaitForBatch = maxTimeToWaitForBatch;
        }

        protected override void StartProcessing<T1>(SubscriptionClient subscriptionClient, Action<IEnumerable<T1>> messagesReceived)
        {
            var batcher = new Batcher<BrokeredMessage>(messages => ProcessMessages(subscriptionClient, messagesReceived, messages), BatchSize, this.maxTimeToWaitForBatch);

            while (true)
            {
                var batch = subscriptionClient.ReceiveBatch(BatchSize);

                foreach (var brokeredMessage in batch)
                {
                    batcher.Post(brokeredMessage);
                }
            }
        }
    }
}
