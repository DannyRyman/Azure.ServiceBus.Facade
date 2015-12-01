using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class SubscriptionMessage<T> : SubscriptionMessage
    {
        public SubscriptionMessage(BrokeredMessage message) : base(message)
        {
        }

        public T GetBody()
        {
            using (var reader = new StreamReader(this.Message.GetBody<Stream>(), Encoding.UTF8))
            {
                string json = reader.ReadToEnd();

                if (typeof(T).IsInterface)
                {
                    // Dynamic proxy
                    return JsonConvert.DeserializeObject<T>(json, new ProxyConverter<T>(this.Message.Properties["Core.MessageType"].ToString()));
                }

                return JsonConvert.DeserializeObject<T>(json, new GuidelineJsonSerializerSettings());
            }
        }
    }

    public class SubscriptionMessage
    {
        protected readonly BrokeredMessage Message;

        public SubscriptionMessage(BrokeredMessage message)
        {
            this.Message = message;
        }

        public string ContentType
        {
            get
            {
                return this.Message.ContentType;
            }
        }

        public string CorrelationId
        {
            get
            {
                return this.Message.CorrelationId;
            }
        }

        public int DeliveryCount
        {
            get
            {
                return this.Message.DeliveryCount;
            }
        }

        public DateTime EnqueuedTimeUtc
        {
            get
            {
                return this.Message.EnqueuedTimeUtc;
            }
        }

        public DateTime ExpiresAtUtc
        {
            get
            {
                return this.Message.ExpiresAtUtc;
            }
        }

        public string Label
        {
            get
            {
                return this.Message.Label;
            }
        }

        public DateTime LockedUntilUtc
        {
            get
            {
                return this.Message.LockedUntilUtc;
            }
        }

        public Guid LockToken
        {
            get
            {
                return this.Message.LockToken;
            }
        }

        public string MessageId
        {
            get
            {
                return this.Message.MessageId;
            }

            set
            {
                this.Message.MessageId = value;
            }
        }

        public IDictionary<string, object> Properties
        {
            get
            {
                return this.Message.Properties;
            }
        }

        public string ReplyTo
        {
            get
            {
                return this.Message.ReplyTo;
            }
        }

        public string ReplyToSessionId
        {
            get
            {
                return this.Message.ReplyToSessionId;
            }
        }

        public DateTime ScheduledEnqueueTimeUtc
        {
            get
            {
                return this.Message.ScheduledEnqueueTimeUtc;
            }
        }

        public long SequenceNumber
        {
            get
            {
                return this.Message.SequenceNumber;
            }
        }

        public string SessionId
        {
            get
            {
                return this.Message.SessionId;
            }
        }

        public long Size
        {
            get
            {
                return this.Message.Size;
            }
        }

        public TimeSpan TimeToLive
        {
            get
            {
                return this.Message.TimeToLive;
            }
        }

        public string To
        {
            get
            {
                return this.Message.To;
            }
        }

        public bool IsDeadLettered
        {
            get;
            private set;
        }

        public bool IsAbandoned { get; private set; }

        public bool IsComplete { get; private set; }

        public bool IsSuspended { get; private set; }

        public bool IsActioned
        {
            get { return this.IsAbandoned || this.IsComplete || this.IsDeadLettered || this.IsSuspended; }
        }

        public void Suspend()
        {
            this.IsSuspended = true;
        }

        public void Abandon()
        {
            this.Message.Abandon();
            this.IsAbandoned = true;
        }

        public void Complete()
        {
            this.Message.Complete();
            this.IsComplete = true;
        }

        public void DeadLetter(string deadLetterReason, string deadLetterErrorDescription)
        {
            this.Message.DeadLetter(deadLetterReason, deadLetterErrorDescription);
            this.IsDeadLettered = true;
        }

        public void Dispose()
        {
            this.Message.Dispose();
        }

        public string GetBodyString()
        {
            using (var reader = new StreamReader(this.Message.GetBody<Stream>(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public string GetHeadersString()
        {
            return JsonConvert.SerializeObject(this.Properties, new GuidelineJsonSerializerSettings());
        }

        public BrokeredMessage GetMessage()
        {
            return this.Message;
        }
    }
}