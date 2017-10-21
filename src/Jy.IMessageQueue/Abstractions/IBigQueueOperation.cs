using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.IMessageQueue
{
    /// <summary>
    /// 非amqp，kafka
    /// </summary>
    public interface IBigQueueOperation
    {
        /// <summary>
        /// 发送消息
        /// </summary>
         void PublishTopic(MessageBase msg, string queueName, string borkerList);
        /// <summary>
        /// 接收消息
        /// </summary>
        void SubscribeTopic<T,TH>(Func<TH> handler, string borkerList, string groupID, List<string> topics) 
            where TH : IProcessMessage<T>
            where T : MessageBase;
        /// <summary>
        /// 异常消息处理
        /// </summary>
        void ErrorSubscribe();
    }
}
