
using Jy.IMessageQueue;
using RawRabbit.Context;
using System;


namespace Jy.RabbitMQ
{
    public class MessageContextResponse :  IMessageContext
    {
        private MessageBase _message;
        public MessageBase message { get { return _message; } set { _message = value; } }
        public MessageContextResponse() { }
        public MessageContextResponse(MessageBase message)
        {
            _message = message;
        }
        public string CustomProperty { get; set; }
        public ulong DeliveryTag { get; set; }
        public Guid GlobalRequestId { get; set; }
    }
}
