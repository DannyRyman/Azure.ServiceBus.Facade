using System;
using ProqualIT.Azure.ServiceBus.Facade;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                "Endpoint=sb://mporium-dev-insights.servicebus.windows.net/;SharedAccessKeyName=insights;SharedAccessKey=oSVZuPFS87HSOoe0GWO6979luQ6T+vZ7Pefu9bVK8Nk=";

            const string topicName = "shopify-notifications";

            Publisher publisher = new Publisher(topicName, connectionString);
            
            Subscriber subscriber = new Subscriber(topicName, connectionString);

            subscriber.Subscribe<SampleEvent>("SampleEvents", message => Console.WriteLine(
                String.Format("received message with id \"{0}\" and content \"{1}\"", message.MessageId,
                    message.GetBody().Message)));

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
            public string Message { get; private set; }

            public SampleEvent(string message)
            {
                Message = message;
            }
        }
    }
}
