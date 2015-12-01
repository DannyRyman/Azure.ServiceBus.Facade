using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class SubscriptionClientFactory
    {
        private readonly string topic;
        private readonly NamespaceManager namespaceManager;
        private readonly string connectionString;

        public SubscriptionClientFactory(string topic, 
            string connectionString)
        {
            this.topic = topic;
            this.connectionString = connectionString;
            this.namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
        }

        public SubscriptionClient CreateAndGetSubscription(string subscriptionName, Type type, ISpecification customFilter = null)
        {
            string subscriptionFullName = string.Concat(subscriptionName, ".", type.Name);

            var baseFilter = 
                new AndSpecification(
                   CreateDeadLetterFilter(subscriptionFullName),
                   CreateFilterOnMessageType(type)
                );

            // Join the custom and base filter if necessary
            var filter = customFilter != null ? new AndSpecification(baseFilter, customFilter) : baseFilter;

            return this.CreateAndGetSubscriptionInternal(subscriptionFullName, filter);
        }

        private static ISpecification CreateDeadLetterFilter(string subscriptionFullName)
        {
            ISpecification deadLetterSpecification =
                new OrSpecification(
                        new EqualSpecification("Core.DeadLetterResubmitSubscription", string.Empty),
                        new EqualSpecification("Core.DeadLetterResubmitSubscription", subscriptionFullName)
                    );

            return deadLetterSpecification;
        }

        private static LikeSpecification CreateFilterOnMessageType(Type type)
        {
            var messageSpecification = new LikeSpecification("Core.MessageType", type.Name);
            return messageSpecification;
        }

        private SubscriptionClient CreateAndGetSubscriptionInternal(string subscriptionName, ISpecification filterSpecification)
        {
            var filter = new SqlFilter(filterSpecification.Result());
            EnsureSubscriptionNameIsValid(subscriptionName);

            if (!this.namespaceManager.SubscriptionExists(this.topic, subscriptionName))
            {
                var subscriptionDescription = new SubscriptionDescription(this.topic, subscriptionName)
                {
                    LockDuration = TimeSpan.FromMinutes(5)
                };

                this.namespaceManager.CreateSubscription(subscriptionDescription, filter);
            }

            var client = SubscriptionClient.CreateFromConnectionString(
                connectionString, this.topic, subscriptionName);

            return client;
        }

        private static void EnsureSubscriptionNameIsValid(string subscriptionFullName)
        {
            if (subscriptionFullName.Length > 50)
            {
                throw new Exception("The entity path/name of subscription '" + subscriptionFullName +
                                    "' exceeds the 50 character limit.");
            }
        }
    }
}