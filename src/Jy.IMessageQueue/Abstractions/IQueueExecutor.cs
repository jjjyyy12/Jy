using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jy.IMessageQueue
{
    //消息发送
    public interface IQueueExecutor
    {
        void ExecuteAsync(MessageBase message);
        Task<MessageBase> RequestTopic(MessageBase msg);
    }
}
