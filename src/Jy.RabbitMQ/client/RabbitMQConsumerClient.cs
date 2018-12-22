using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Jy.IMessageQueue;
using RabbitMQ.Client.MessagePatterns;
using Jy.Utility.Convert;
using RabbitMQ.Client.Exceptions;
using Jy.ILog;

namespace Jy.RabbitMQ
{
    //public delegate Task ProcessEvent(string name, MessageBase msg);
    public class RabbitMQConsumerClient<T> : IConsumerClient<T> where T: MessageBase 
    {
        private readonly ILogger _logger;
        private readonly string _exchageName;
        private readonly string _queueName;
        public string QueueName { get { return _queueName; } }
        private readonly string _responseQueueName; //回复的channel
        public string ResponseQueueName { get { return _responseQueueName; } }


        private readonly RabbitMQOptions _rabbitMQOptions;

        private ConnectionPool _connectionPool;
        private IModel _channel;
        private IModel _responseChannel; //返回结果的信道
        private bool _needResponse;
        private ulong _deliveryTag;

        public event  EventHandler<T> OnMessageReceievedToOutSide;

        public event EventHandler<string> OnError;
        
        //public ProcessEvent _processEvent;

        public RabbitMQConsumerClient(string exchageName,string queueName,string responseQueueName,bool needResponse,
             ConnectionPool connectionPool,
             RabbitMQOptions options, ILogger logger)//, ProcessEvent processEvent
        {
            _queueName = queueName;
            _responseQueueName = responseQueueName;
            _needResponse = needResponse;
            _connectionPool = connectionPool;
            _rabbitMQOptions = options;
            _exchageName = exchageName ?? options.TopicExchangeName;
            //_processEvent = processEvent;
            _logger = logger;
            InitClient();
        }

        private void InitClient()
        {
            var connection = _connectionPool.Rent();

            _channel = connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: _exchageName,
                type: RabbitMQOptions.ExchangeType,
                durable: true);

            var arguments = new Dictionary<string, object> { { "x-message-ttl", (int)_rabbitMQOptions.QueueMessageExpires } };
            _channel.QueueDeclare(_queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments);

            _connectionPool.Return(connection);

            if(_needResponse)
            {
                //responsechannel
                var responseConnection = _connectionPool.Rent();
                _responseChannel = responseConnection.CreateModel();
                _responseChannel.ExchangeDeclare(
                    exchange: _exchageName,
                    type: RabbitMQOptions.ExchangeType,
                    durable: true);

                _responseChannel.QueueDeclare(_responseQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments);
                _connectionPool.Return(responseConnection);
            }
            
        }

        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics == null) throw new ArgumentNullException(nameof(topics));

            foreach (var topic in topics)
            {
                _channel.QueueBind(_queueName, _exchageName, topic);
            }
        }

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken) 
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnConsumerReceived;
            consumer.Shutdown += OnConsumerShutdown;
            _channel.BasicConsume(_queueName, false, consumer); //第二个参数值为false代表关闭RabbitMQ的自动应答机制，改为手动应答
            //while (true)
            //{
            //    Task.Delay(timeout, cancellationToken).GetAwaiter().GetResult();
            //}
        }
         
        public void Commit()
        {
            _channel.BasicAck(_deliveryTag, false);
        }

        public void Dispose()
        {
            _channel.Dispose();
            if (_needResponse)
            {
                _responseChannel.Dispose();
            }
        }

        private void OnConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            _deliveryTag = e.DeliveryTag;

            var message = ByteConvertHelper.Bytes2Object<T>(e.Body);
            if(!_needResponse)
            {
                OnMessageReceievedToOutSide?.Invoke(sender, message);
            }
            else
            {
                bool hasRejected = false;
                bool isSuccess = false; 
                try
                {
                    //_processEvent(_queueName, message);
                    OnMessageReceievedToOutSide?.Invoke(sender, message);
                    isSuccess = true;
                }
                catch(Exception ex)
                {
                    _logger.LogError($"RabbitMQConsumerClient OnConsumerReceived eooro:{message.MessageRouter}", ex);
                    _responseChannel.BasicReject(e.DeliveryTag, false); //不再重新分发
                    hasRejected = true;
                    isSuccess = true;
                    throw ex;
                }
          
                if (_needResponse)//是否需要回执
                {
                    if (isSuccess)
                    {
                        try
                        {
                            IBasicProperties props = e.BasicProperties;
                            IBasicProperties replyProps = _responseChannel.CreateBasicProperties();
                            replyProps.CorrelationId = props.CorrelationId;
                            _responseChannel.BasicPublish(_exchageName, $"{message.MessageRouter}_reply",null, ByteConvertHelper.Object2Bytes(message)); //发送消息到内容检查队列
                            if (!hasRejected)
                            {
                                _channel.BasicAck(e.DeliveryTag, false); //确认处理成功  此处与不再重新分发，只能出现一次
                            }
                        }
                        catch (Exception Ex)
                        {
                            _logger.LogError($"RabbitMQConsumerClient OnConsumerReceived _needResponse error:{message.MessageRouter}", Ex);
                            throw Ex;
                        }
                    }
                    //else
                    //{
                    //    _channel.BasicReject(e.DeliveryTag, true); //处理失败，重新分发
                    //}
                }
            }
           
        }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            OnError?.Invoke(sender, e.Cause?.ToString());
        }

        //public void ResponseTopic(string exchangeName, string queueName, string bindKeyTopic)
        //{
        //    Subscription sub = new Subscription(_channel, _queueName);
        //    SimpleRpcServer server = new SimpleRpcServer(sub);
        //    PublicationAddress replayAddress = new PublicationAddress("topic", exchangeName, bindKeyTopic);
        //    var props = _channel.CreateBasicProperties();
        //    props.ReplyTo= $"{queueName}_reply";
        //    props.ReplyToAddress = replayAddress;

        //    BasicDeliverEventArgs arg = new BasicDeliverEventArgs(queueName, _deliveryTag, true, exchangeName, bindKeyTopic, props, ByteConvertHelper.Object2Bytes("1"));
        //    server.ProcessRequest(arg);
        //}
    }
}
