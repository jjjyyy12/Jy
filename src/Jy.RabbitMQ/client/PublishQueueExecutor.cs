using System;
using System.Text;
using Jy.ILog;
using RabbitMQ.Client;
using System.Threading.Tasks;
using Jy.IMessageQueue;
using RabbitMQ.Client.MessagePatterns;
using Jy.Utility.Convert;
using System.Threading;
using RabbitMQ.Client.Events;

namespace Jy.RabbitMQ
{
    public class PublishQueueExecutor
    {
        private readonly ILogger _logger;
        private readonly ConnectionPool _connectionPool;
        private readonly RabbitMQOptions _rabbitMQOptions;

        public PublishQueueExecutor(
            ConnectionPool connectionPool,
            RabbitMQOptions rabbitMQOptions,
            ILogger logger)
        {
            _logger = logger;
            _connectionPool = connectionPool;
            _rabbitMQOptions = rabbitMQOptions;
        }

        public void Publish(MessageBase msg)
        {
            var connection = _connectionPool.Rent();

            try
            {
                using (var channel = connection.CreateModel())
                {
                    //channel.ExchangeDeclare(msg.exchangeName, RabbitMQOptions.ExchangeType, true, true, null);
                    channel.BasicPublish(exchange: msg.exchangeName,
                                         routingKey: msg.MessageRouter,
                                         basicProperties: null,
                                         body: ByteConvertHelper.Object2Bytes(msg));

                    _logger.LogDebug($"rabbitmq topic message [{msg.Id}] has been published.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"rabbitmq topic message [{msg.Id}] has benn raised an exception of sending. the exception is: {ex.Message}",ex);
            }
            finally
            {
                _connectionPool.Return(connection);
            }
        }
      
        public Task<MessageBase> RequestTopic(MessageBase msg)
        {
            return RequestPublish(msg, new TimeSpan(0, 60, 0), CancellationToken.None);
        }

        static Object obj = new Object();
        public Task<MessageBase> RequestPublish(MessageBase msg,TimeSpan timeout, CancellationToken cancellationToken)
        {
            var connection = _connectionPool.Rent();
            try
            {
                using (var channel = connection.CreateModel())
                {
                    var replyQueueName = $"{msg.MessageRouter}_reply";

                    var responseConnection = _connectionPool.Rent();
                    var _responseChannel = responseConnection.CreateModel();
                    _connectionPool.Return(responseConnection);
                    _responseChannel.QueueBind(replyQueueName, msg.exchangeName, $"{msg.MessageRouter}_reply");
                    var consumer = new EventingBasicConsumer(_responseChannel);
                    consumer.Received += OnReplyConsumerReceived;
                    _responseChannel.BasicConsume(replyQueueName, false,consumer);

                    
                    channel.BasicPublish(exchange: msg.exchangeName,
                                         routingKey: msg.MessageRouter,
                                         basicProperties: null,
                                         body: ByteConvertHelper.Object2Bytes(msg));

                    //此处等待OnReplyConsumerReceived执行完再记日志，需要同步
                    Monitor.Enter(obj);
                    Monitor.Wait(obj);
                    _logger.LogDebug($"rabbitmq topic message [{msg.Id}] has been published.");
                    _responseChannel.BasicAck(_deliveryTag, false); //确认处理成功  此处与不再重新分发，只能出现一次
                }
              
                return Task.FromResult(returnMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError($"rabbitmq topic message [{msg.Id}] has benn raised an exception of sending. the exception is: {ex.Message}", ex);
            }
            finally
            {
                _connectionPool.Return(connection);
                Monitor.Pulse(obj);
                Monitor.Exit(obj);
            }
            return Task.FromResult(msg);
        }

        private ulong _deliveryTag;
        public MessageBase returnMsg; //返回的msg
        private void OnReplyConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            Monitor.Enter(obj);
            
            var res = ByteConvertHelper.Bytes2Object<MessageBase>(e.Body);
            returnMsg = res;
            if (res.MessageBodyByte.Length>0)
                _logger.LogDebug($"rabbitmq topic message [{res.Id}] has been published. and rpc received");

            _deliveryTag = e.DeliveryTag;
            Monitor.Pulse(obj);
            Monitor.Exit(obj);
        }


        //public Task<MessageBase> RequestTopic0(MessageBase msg)
        //{
        //    var connection = _connectionPool.Rent();
        //    try
        //    {
        //        using(var channel = connection.CreateModel())
        //        using (var client = new SimpleRpcClient(channel, msg.exchangeName, RabbitMQOptions.ExchangeType, msg.MessageRouter))
        //        {
        //            PublicationAddress replayAddress = new PublicationAddress(RabbitMQOptions.ExchangeType, msg.exchangeName, msg.MessageRouter);
        //            var props = channel.CreateBasicProperties();
        //            props.ReplyTo = $"{msg.queueName}_reply";
        //            props.ReplyToAddress = replayAddress;

        //            var res = ByteConvertHelper.Bytes2Object<MessageBase>(client.Call(props, ByteConvertHelper.Object2Bytes(msg)).Body);
        //            if (res.MessageBodyByte.Length > 0)
        //                _logger.LogDebug($"rabbitmq topic message [{res.Id}] has been published. and rpc received");
        //        }
        //        return Task.FromResult(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"rabbitmq topic message [{msg.Id}] has benn raised an exception of sending. the exception is: {ex.Message}", ex);
        //    }
        //    finally
        //    {
        //        _connectionPool.Return(connection);
        //    }
        //    return Task.FromResult(msg);
        //}
    }
}
