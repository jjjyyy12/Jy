using Jy.IMessageQueue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
 
using Jy.ILog;
using System.Threading;
using System.Reflection.Emit;
using System.Reflection;
using Confluent.Kafka;
using Newtonsoft.Json;
using Polly.Retry;
using Polly;

namespace Jy.CKafka
{
    public class QueueOperationCKafka : IBigQueueOperation
    {
        private ILogger _logger;
        private readonly IQueueOperationSubscriptionsManager _subsManager;
        private readonly IKafkaPersisterConnection _producerConnection;
        private readonly IKafkaPersisterConnection _consumerConnection;
        private readonly Func<string, IKafkaPersisterConnection> _connectionAccessor;
        const int commitPeriod = 5;
        public QueueOperationCKafka(IQueueOperationSubscriptionsManager subsManager, Func<string, IKafkaPersisterConnection> connectionAccessor)
        {
            _subsManager = subsManager ?? new InMemorySubscriptionsManager();
            _connectionAccessor = connectionAccessor;
            _producerConnection = _connectionAccessor("KafkaProducer");
            _consumerConnection = _connectionAccessor("KafkaConsumer"); 
            _logger = LoggerFactory.CreateLogger();
            subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }
        public void ErrorSubscribe()
        {
        }
        public void Dispose()
        {
            _producerConnection.Dispose();
            _consumerConnection.Dispose();
        }
        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_consumerConnection.IsConnected)
            {
                _consumerConnection.TryConnect();
            }

            using (var channel = _consumerConnection.CreateConnect() as IConsumer<string, MessageBase>)
            {
                channel.Unsubscribe();
                if (_subsManager.IsEmpty)
                {
                    _consumerConnection.Dispose();
                }
            }
        }
        public void Unsubscribe<T, TH>()
           where T : MessageBase
           where TH : IProcessMessage<T>
        {
            _subsManager.RemoveSubscription<T, TH>();
        }
        public void PublishTopic(MessageBase msg ,string queueName, string borkerList)
        {
            if (!_producerConnection.IsConnected)
            {
                _producerConnection.TryConnect();
            }

            var eventName = msg.GetType()
                   .Name;
            Message<string, MessageBase> message = new Message<string, MessageBase>() { Key = eventName, Value = msg };

            var policy = RetryPolicy.Handle<KafkaException>()
               .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
               {
                   _logger.LogWarning(ex.ToString());
               });

            using (var conn = _producerConnection.CreateConnect() as IProducer<string, MessageBase>)
            {
                policy.Execute(() =>
                {
                    conn.ProduceAsync(eventName, message);
                });
            }

            
        }
        
        public void SubscribeTopic<T, TH>(Func<TH> handler, string borkerList, string groupID, List<string> topics) 
            where TH : IProcessMessage<T>
            where T : MessageBase
        {
            var eventName = typeof(T).Name;
            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                if (!_consumerConnection.IsConnected)
                {
                    _consumerConnection.TryConnect();
                }
                _subsManager.AddSubscription<T, TH>(handler);

                using (var consumer = _consumerConnection.CreateConnect() as IConsumer<string, MessageBase>)
                {
                    consumer.Subscribe(eventName);

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume();
                                if (consumeResult.IsPartitionEOF)
                                {
                                    _logger.LogInformation(
                                        $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                                    continue;
                                }

                                _logger.LogInformation($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");

                                ConsumerClient_OnMessage(null, consumeResult);

                                if (consumeResult.Offset % commitPeriod == 0)
                                {
                                    try
                                    {
                                        consumer.Commit(consumeResult);
                                    }
                                    catch (KafkaException e)
                                    {
                                        _logger.LogError($"Commit error: {e.Error.Reason}",e);
                                    }
                                }
                            }
                            catch (ConsumeException e)
                            {
                                _logger.LogError($"Consume error: {e.Error.Reason}",e);
                            }
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        _logger.LogError("Closing consumer.",e);
                        consumer.Close();
                    }
                }
            }
        
            
        }

        private void ConsumerClient_OnMessage(object sender, ConsumeResult<string, MessageBase> e)
        {
            ProcessEvent(e.Key, e.Value).Wait();
        }
        //执行总线中该事件的所有handler
        private async Task ProcessEvent<T>(string eventName, T message) where T : MessageBase
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var eventType = _subsManager.GetEventTypeByName(eventName);
                var handlers = _subsManager.GetHandlersForEvent(eventName);

                await ExecuteHandlersSync(handlers, eventType, message);
            }
        }
        //并行
        private async Task ExecuteHandlersAsync<T>(IEnumerable<Delegate> handlers, Type eventType, T message) where T : MessageBase
        {
            foreach (var handlerfactory in handlers)
            {
                await Task.Run(() => {
                    DynamicEmitInvoke(eventType, handlerfactory, message); //emit
                });
            }
        }
        //串行
        private async Task ExecuteHandlersSync<T>(IEnumerable<Delegate> handlers, Type eventType, T message) where T : MessageBase
        {
            await Task.Run(() => {
                foreach (var handlerfactory in handlers)
                {
                    DynamicEmitInvoke(eventType, handlerfactory, message); //emit
                }
            });
        }
        //reflect调用
        private void DynamicInvoke<T>(Type eventType, Delegate ihandler, T message) where T : MessageBase
        {
            var handler = ihandler.DynamicInvoke();
            var concreteType = typeof(IProcessMessage<>).MakeGenericType(eventType);
            concreteType.GetMethod("ProcessMsg").Invoke(handler, new object[] { message });
        }
        //reflect+emit调用
        private void DynamicEmitInvoke<T>(Type eventType, Delegate ihandler, T message) where T : MessageBase
        {
            var handler = ihandler.DynamicInvoke();
            var concreteType = typeof(IProcessMessage<>).MakeGenericType(eventType);
            var dynamicMethod = new DynamicMethod(concreteType.Name + ihandler.GetType().Name, null, new Type[] { concreteType, typeof(T) });
            var method = concreteType.GetMethod("ProcessMsg");
            //方法IL
            ILGenerator il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); //put IProcessMessage<> arg into stack
            il.Emit(OpCodes.Ldarg_1); //put T : MessageBase arg into stack
            if (eventType == method.DeclaringType) //call IProcessMessage.ProcessMsg
                il.Emit(OpCodes.Call, method);
            else
                il.Emit(OpCodes.Callvirt, method);
            il.Emit(OpCodes.Ret);//return
            dynamicMethod.Invoke(new object(), new object[] { handler, message });//动态调用Delegate ihandler中的PROCESSMSG，参数是paras[0]
        }
    }
}
