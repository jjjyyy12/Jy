using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.IMessageQueue
{
    //消息
    public class MessageBase
    {
        public MessageBase()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public DateTime CreationDate { get; }

        public byte[] MessageBodyByte { get; set; }

        public byte[] MessageBodyReturnByte { get; set; }

        public string MessageRouter { get; set; }
        public int FailedTimes { get; set; } = 0;

        public string exchangeName { get; set; }
        public string queueName { get; set; }
    }
}
