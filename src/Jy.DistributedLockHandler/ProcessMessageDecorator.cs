
using Jy.IMessageQueue;
using Jy.DistributedLock;
using System;

namespace Jy.DistributedLockHandler
{
    /// <summary>
    /// //分布式锁装饰类,主要服务于IProcessMessage
    /// </summary>
    public class ProcessMessageDecorator<TMessageBase> : IProcessMessage<TMessageBase>  where TMessageBase:MessageBase
    { 
        private IProcessMessage<TMessageBase> _messageProcessor;
        private ProcessMessageLockHandler _distributedLockHandler;
        //装饰者模式必须要传入上一个装饰者对象，如果没有则传null ，这里是IProcessMessage
        public ProcessMessageDecorator(IProcessMessage<TMessageBase> messageProcessor, ProcessMessageLockHandler distributedLockHandler)
        {
            _messageProcessor = messageProcessor;
            _distributedLockHandler = distributedLockHandler;
        }
 
        public void ProcessMsg(TMessageBase msg)
        {
            var mp = _messageProcessor.GetType(); //靠反射识别attribute属性执行，性能略受影响
            object[] args = { msg };
            _distributedLockHandler.RunLock<DistributedLockAttribute, TMessageBase>(_messageProcessor, mp, args); //_messageProcessor.ProcessMsg(msg);
        }

    }
}
