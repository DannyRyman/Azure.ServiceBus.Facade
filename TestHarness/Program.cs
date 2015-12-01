using System;
using ProqualIT.Azure.ServiceBus.Facade;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                "ConnectionStringGoesHere";

            const string topicName = "shopify-notifications";

            Publisher publisher = new Publisher(topicName, connectionString);
            
            Subscriber subscriber = new Subscriber(topicName, connectionString);

            subscriber.Subscribe<SampleEvent>("SampleEvents", message => Console.WriteLine($"received message with id \"{message.MessageId}\" and content \"{message.GetBody().Message}\""));

            while (true)
            {
                Console.WriteLine("Write something to send a message:");
                string message = Console.ReadLine();

                if (!string.IsNullOrEmpty(message))
                {
                    var publishMessage = PublishMessage.Create(new SampleEvent(message), Guid.NewGuid().ToString());
                    publisher.Send(publishMessage);
                }
            }
        }

        public class SampleEvent
        {
            public string Message { get; }

            public SampleEvent(string message)
            {
                Message = message;
            }
        }
    }
}
