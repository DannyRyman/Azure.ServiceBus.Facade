using System;
using System.Threading;
using NUnit.Framework;
using ProqualIT.Azure.ServiceBus.Facade;
using Tests.Support;

namespace Tests
{
    [TestFixture]
    public class StronglyTypedPubSub
    {
        [Ignore("Need to supply connection string")]
        [Test]
        public void CanSubscribeToPublishedEvent()
        {
            SubscriptionMessage<SampleEvent> receivedMessage = null;
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            // Register the subscriber
            Subscriber subscriber = new Subscriber(Configuration.SampleTopicName, Configuration.AzureServiceBusConnectionString);
            subscriber.Subscribe<SampleEvent>("StronglyTypedPubSubSubscription", message =>
            {
                receivedMessage = message;
                autoResetEvent.Set();
            });

            Publisher publisher = new Publisher(Configuration.SampleTopicName, Configuration.AzureServiceBusConnectionString);
            publisher.Send(PublishMessage.Create(new SampleEvent("test"), Guid.NewGuid().ToString()));

            autoResetEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual("test", receivedMessage.GetBody().Message);
        }
    }
}
