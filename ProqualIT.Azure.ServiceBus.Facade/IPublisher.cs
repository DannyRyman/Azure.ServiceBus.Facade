using System.Collections.Generic;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public interface IPublisher
    {
        void SendBatch(IEnumerable<PublishMessage> messages);

        void Send(PublishMessage message);
    }
}