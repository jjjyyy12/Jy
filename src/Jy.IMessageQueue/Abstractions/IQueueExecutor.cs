using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.IMessageQueue
{
    interface IQueueExecutor
    {
        void ExecuteAsync(MessageBase message);
    }
}
