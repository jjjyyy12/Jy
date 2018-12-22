using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.IMessageQueue
{
    /// <summary>
    /// AMQP协议下的消息操作接口，exchangename，queuename，topic
    /// </summary>
    public interface IQueueOperation 
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        void PublishTopic(MessageBase msg);
        /// <summary>
        /// 监听处理消息
        /// </summary>
        void SubscribeTopic<T, TH>(Func<TH> handler, string subscriberId, string exchangeName, string queueName, string bindKeyTopic)
            where TH : IProcessMessage<T>
            where T : MessageBase;
        /// <summary>
        /// RPC发送消息，带response
        /// </summary>
        Task<MessageBase> RequestTopic(MessageBase msg);
        /// <summary>
        /// RPC监听处理消息
        /// </summary>
        void ResponseTopic<T, TH>(Func<TH> handler, string exchangeName, string queueName, string bindKeyTopic) 
            where TH : IProcessMessage<T>
            where T : MessageBase;

        void Unsubscribe<T, TH>() where TH : IProcessMessage<T> where T : MessageBase;

        void ErrorSubscribe();

        void Dispose(string queueName);
        void Dispose();

    }
}
