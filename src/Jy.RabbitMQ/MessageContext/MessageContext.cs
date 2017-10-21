
using Jy.IMessageQueue;
using RawRabbit.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.RabbitMQ
{
    public class MessageContext :  IMessageContext
    {
        private MessageBase _message;
        public MessageBase message { get { return _message; } set { _message = value; } }
        public MessageContext() { }
        public MessageContext(MessageBase message)
        {
            _message = message;
        }
        public string CustomProperty { get; set; }
        public ulong DeliveryTag { get; set; }
        public Guid GlobalRequestId { get; set; }
    }
}
