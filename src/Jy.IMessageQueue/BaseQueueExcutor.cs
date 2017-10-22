﻿using Jy.ILog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jy.IMessageQueue
{
    //可处理重发
    public abstract class BaseQueueExcutor : IQueueExecutor
    {
        private readonly ILogger _logger;

        protected BaseQueueExcutor(ILogger logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        public abstract void PublishTopic(MessageBase msg);

        public void ExecuteAsync(MessageBase message)
        {
            try
            {
                var sp = Stopwatch.StartNew();
                  PublishTopic(message);
                sp.Stop();

                if (sp.Elapsed.TotalSeconds > 4000)
                {
                    _logger.LogInformation("BaseQueueExcutor ExecuteAsync too long time:{0},{1}", message.MessageRouter, message.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BaseQueueExcutor ExecuteAsync error:{message.Id},failtimes:{message.FailedTimes}", ex);
                if (message.FailedTimes <= 4) //4次机会重发,每次等待2*失败次数的时间
                    HandleAsync(message);
            }
        }
        private Task<MessageBase> HandleAsync(MessageBase originalMsg)
        {
            if (originalMsg.MessageRouter.IndexOf("_normal") < 0) return Task.FromResult<MessageBase>(null); //非normal 不予处理
            originalMsg.FailedTimes++;
            Thread.Sleep(2000 * originalMsg.FailedTimes);
            return Task.Run(() =>
            {
                //errorHandle
                ExecuteAsync(originalMsg);
                return originalMsg;
            });
        }
    }
}
