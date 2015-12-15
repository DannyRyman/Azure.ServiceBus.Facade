using System;
using System.Collections.Generic;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    /// <summary>
    /// Wraps the standard batch capability of Azure Service Bus.  This subscriber is liable to return less than the batch size
    /// most of the time.  This is because (a) it will return any messages found immediately without waiting for the batch to
    /// fill up and (b) when partitioning is enabled, it is even less likely that a whole batch of messages will be available 
    /// immediately.
    /// 
    /// The AdvancedBatchSubscriber if you would prefer to wait a period of time in order to have more change for a whole batch
    /// of results to be constructed.
    /// </summary>  
    public class SimpleBatchSubscriber : BatchSubscriber
    {
        public SimpleBatchSubscriber(string topic, int batchSize, string connectionString)
            : base(topic, batchSize, connectionString)
        {
        }

        public void Subscribe(
            string subscriptionName,
            ISpecification filter,
            Action<IEnumerable<SubscriptionMessage>> messagesReceived)
        {
            this.InitialiseSubscription(subscriptionName, filter, messagesReceived);
        }

        public void Subscribe(string subscriptionName, Action<IEnumerable<SubscriptionMessage>> messagesReceived)
        {
            this.Subscribe(subscriptionName, null, messagesReceived);
        }
    }

    /// <summary>
    /// Wraps the standard batch capability of Azure Service Bus.  This subscriber is liable to return less than the batch size
    /// most of the time.  This is because (a) it will return any messages found immediately without waiting for the batch to
    /// fill up and (b) when partitioning is enabled, it is even less likely that a whole batch of messages will be available 
    /// immediately.
    /// 
    /// The AdvancedBatchSubscriber if you would prefer to wait a period of time in order to have more change for a whole batch
    /// of results to be constructed.
    /// </summary>  
    public class SimpleBatchSubscriber<T> : BatchSubscriber, IBatchSubscriber<T>
    {
        public SimpleBatchSubscriber(string topic, int batchSize, string connectionString)
            : base(topic, batchSize, connectionString)
        {
        }

        public void Subscribe(string subscriptionName, Action<IEnumerable<SubscriptionMessage<T>>> messagesReceived)
        {
            this.Subscribe(subscriptionName, messagesReceived, null);
        }

        public void Subscribe(
            string subscriptionName,
            Action<IEnumerable<SubscriptionMessage<T>>> messagesReceived,
            ISpecification filters)
        {
            this.InitialiseSubscription(subscriptionName, typeof(T), filters, messagesReceived);
        }
    }
}
