using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.IMessageQueue
{
    /// <summary>
    /// AMQP协议下的消息操作接口，exchangename，queuename，topic
    /// </summary>
    public interface IQueueOperationRawRabbit
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


        //void PublishTopicRPC(MessageBase msg);
        //void SubscribeTopicRPC<T>(IProcessMessage<T> ipro, string subscriberId, string queueName, string bindKeyTopic) where T : MessageBase;

        ///// <summary>
        ///// 发送消息
        ///// </summary>
        //void Subscribe(MessageBase msg, IProcessMessage ipro);
        ///// <summary>
        ///// 接收消息
        ///// </summary>
        //void Publish(MessageBase msg);
        ///// <summary>
        ///// 发送消息 一个exchange 对多个queue
        ///// 所有发送到Fanout Exchange的消息都会被转发到与该Exchange 绑定(Binding)的所有Queue上。
        /////Fanout Exchange  不需要处理RouteKey 。只需要简单的将队列绑定到exchange 上。这样发送到exchange的消息都会被转发到与该交换机绑定的所有队列上。类似子网广播，每台子网内的主机都获得了一份复制的消息。
        //　　    ///所以，Fanout Exchange 转发消息是最快的。
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="exchangeName"></param>
        //void PublishFanout(MessageBase msg, string exchangeName);

        ///// <summary>
        ///// 接收消息，一个exchange 对多个queue，多个queue同时消费一次
        ///// </summary>
        //void SubscribeFanout(IProcessMessage ipro, string exchangeName, string queueName);

        ///// <summary>
        ///// 发送消息  所有发送到Direct Exchange的消息被转发到RouteKey中指定的Queue。
        ///// Direct模式,可以使用rabbitMQ自带的Exchange：default Exchange 。所以不需要将Exchange进行任何绑定(binding)操作 。
        ///// 消息传递时，RouteKey必须完全匹配，才会被队列接收，否则该消息会被抛弃。
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="queueName"></param>
        // void PublishDirect(MessageBase msg, string queueName);
        ///// <summary>
        ///// 接收消息，所有发送到Direct Exchange的消息被转发到RouteKey中指定的Queue
        ///// </summary>
        // void SubscribeDirect(IProcessMessage ipro, string exchangeName, string queueName);
    }
}
