using Jy.ILog;
using Jy.IMessageQueue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jy.QueueSerivce
{
    //消息发送的公共服务
    public class QueueSerivce: IQueueService
    {
        private IQueueOperation _queue;
        //稳定消息队列接口rabbitmq
        public IQueueOperation Queue { set { _queue = value; } get { return _queue; } }
        private IBigQueueOperation _bigQueue;
        //高并发消息队列接口kafaka
        public IBigQueueOperation BigQueue { set { _bigQueue = value; } get { return _bigQueue; } }

        private string _exchangeName = "auth.exchange";
        public string ExchangeName { set { _exchangeName = value; } get { return _exchangeName; } }
        //记录到kafka的topicname
        private string _logTopicName = "auth.operate";
        public string LogTopicName { set { _logTopicName = value; } get { return _logTopicName; } }
        private readonly ILogger _logger;
        public QueueSerivce( IQueueOperation queue, IBigQueueOperation bigQueue, ILogger logger)
        {
            _queue = queue;
            _bigQueue = bigQueue;
            _logger = logger;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void PublishTopic(MessageBase msg)
        {
            _queue.PublishTopic(msg);
            _bigQueue.PublishTopic(msg, _logTopicName, null);
        }
        /// <summary>
        /// rpc方式的同步消息
        /// </summary>
        /// <typeparam name="T">例如：UserDTO</typeparam>
        /// <param name="dto">dto</param>
        /// <param name="msg">要发送的消息</param>
        /// <param name="replyMsg">接收到的回执消息</param>
        /// <param name="succHandle">成功后的处理方法</param>
        /// <param name="runcnt">当前重试的次数，第一次填0</param>
        public async void Request<T>(T dto, MessageBase msg, MessageBase replyMsg, Action<T, MessageBase> succHandle, int runcnt) where T : class
        {
            try
            {
                replyMsg = await Queue.RequestTopic(msg);
                if (replyMsg.MessageBodyReturnByte != null)
                {
                    succHandle(dto, replyMsg);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(string.Format("QueueSerivce.Request error: {0},{1}", e.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")), e);
                if (runcnt <= 4) //4次机会重发,每次等待2*失败次数的时间
                {
                    runcnt++;
                    Thread.Sleep(2000 * runcnt);
                    Request<T>(dto, msg, replyMsg, succHandle, runcnt);
                }
            }
            try
            {
                _bigQueue.PublishTopic(msg, _logTopicName, null);
            }catch(Exception ex)
            {
                _logger.LogError("RequestLogError:"+ msg.Id, ex);
            }
           

        }
    }
}
