using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using ProqualIT.Azure.ServiceBus.Facade.Logging;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class SubscriptionClientFactory
    {
        private readonly string topic;
        private readonly NamespaceManager namespaceManager;
        private readonly string connectionString;
        private readonly ILog log;

        public SubscriptionClientFactory(string topic, 
            string connectionString)
        {
            this.topic = topic;
            this.connectionString = connectionString;
            this.log = LogProvider.For<SubscriptionClientFactory>();
            this.namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);            
        }

        public SubscriptionClient CreateAndGetSubscription(string subscriptionName, ISpecification customFilter = null)
        {
            string subscriptionFullName = string.Concat(subscriptionName);

            var baseFilter = CreateDeadLetterFilter(subscriptionFullName); 

            // Join the custom and base filter if necessary
            var filter = customFilter != null ? new AndSpecification(baseFilter, customFilter) : baseFilter;

            return this.CreateAndGetSubscriptionInternal(subscriptionFullName, filter);
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

            log.Info($"Checking subscription for path {subscriptionName} exists");

            if (!this.namespaceManager.SubscriptionExists(this.topic, subscriptionName))
            {
                log.Info("Creating subscription as it does not currently exist");

                var subscriptionDescription = new SubscriptionDescription(this.topic, subscriptionName)
                {
                    LockDuration = TimeSpan.FromMinutes(5)
                };

                this.namespaceManager.CreateSubscription(subscriptionDescription, filter);

                log.Info("Subscription created");
            }

            log.Info("Creating subscription client");

            var client = SubscriptionClient.CreateFromConnectionString(
                connectionString, this.topic, subscriptionName);

            log.Info("Subscription client created");

            return client;
        }

        private void EnsureSubscriptionNameIsValid(string subscriptionFullName)
        {
            log.Info("Validating subscription name");

            if (subscriptionFullName.Length > 50)
            {
                throw new Exception("The entity path/name of subscription '" + subscriptionFullName +
                                    "' exceeds the 50 character limit.");
            }

            log.Info("Subscription name is valid");
        }
    }
}