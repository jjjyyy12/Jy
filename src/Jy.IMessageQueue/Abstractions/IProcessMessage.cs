using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.IMessageQueue
{
    //消息处理，消费
    public interface IProcessMessage<in TMessageBase>: IProcessMessage
         where TMessageBase : MessageBase
    {
        void ProcessMsg(TMessageBase msg);

    }
    public interface IProcessMessage
    {
    }
}
