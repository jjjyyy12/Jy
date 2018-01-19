
using Jy.ILog;
using Jy.IMessageQueue;

namespace Jy.RabbitMQ.ProcessMessage
{
    /// <summary>
    /// kafka log操作处理
    /// </summary>
    public class ProcessOperateLog : IProcessMessage<MessageBase>
    {
        private ILogger _logger;
        public ProcessOperateLog()
        {
            _logger = LoggerFactory.CreateLogger();
        }
        public void ProcessMsg(MessageBase msg)
        {
            _logger.LogInformation(msg.MessageRouter+System.Text.Encoding.UTF8.GetString(msg.MessageBodyByte));
        }

    }
}
