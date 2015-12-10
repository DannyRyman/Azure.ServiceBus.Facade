# Azure.ServiceBus.Facade

A lightweight wrapper on top of the Azure Service Bus. 

Allows for really simple publish and subscribe of objects or raw messages.  

Supports a simple ISpecification based technique for filtering messages based on message headers.

## Usage

Publishing a single message
```c#
Publisher publisher = new Publisher(topicName, connectionString);
var publishMessage = PublishMessage.Create(new SampleEvent(message), Guid.NewGuid().ToString());
publisher.Send(publishMessage);
```

Subscribing to a single message
```c#
Subscriber subscriber = new Subscriber(topicName, connectionString);
subscriber.Subscribe<SampleEvent>("SampleEvents", message => Console.WriteLine(
	String.Format("received message with id \"{0}\" and content \"{1}\"", message.MessageId,
        message.GetBody().Message)));
```

## Additional Information

Nuget package:
https://www.nuget.org/packages/ProqualIT.Azure.ServiceBus.Facade/
