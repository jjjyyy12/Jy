
using Jy.IMessageQueue;
using System;


namespace Jy.QueueSerivce
{
    public interface IQueueService
    {
        IQueueOperation Queue { get; set; }
        IBigQueueOperation BigQueue { get; set; }
        string ExchangeName { set; get; } 
        string LogTopicName { set; get; }
        void PublishTopic(MessageBase msg);
        /// <summary>
        /// rpc方式的同步消息
        /// </summary>
        /// <typeparam name="T">例如：UserDTO</typeparam>
        /// <param name="dto">dto</param>
        /// <param name="msg">要发送的消息</param>
        /// <param name="replyMsg">接收到的回执消息</param>
        /// <param name="succHandle">成功后的处理方法</param>
        /// <param name="runcnt">当前重试的次数，第一次填0</param>
        void Request<T>(T dto, MessageBase msg, MessageBase replyMsg, Action<T, MessageBase> succHandle, int runcnt) where T : class;
    }
}
