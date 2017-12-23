using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.IMessageQueue
{
    //消息发送
    interface IQueueExecutor
    {
        void ExecuteAsync(MessageBase message);
    }
}
