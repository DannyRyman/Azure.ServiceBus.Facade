using System;
using System.Collections.Generic;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public interface IBatchSubscriber<T>
    {
        void Subscribe(string subscriptionName, Action<IEnumerable<SubscriptionMessage<T>>> messagesReceived);

        void Subscribe(
            string subscriptionName,
            Action<IEnumerable<SubscriptionMessage<T>>> messagesReceived,
            ISpecification filters);
    }
}
