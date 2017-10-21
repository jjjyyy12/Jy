using Jy.IMessageQueue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RdKafka;
using Jy.ILog;
using System.Threading;
using System.Reflection.Emit;
using System.Reflection;

namespace Jy.Kafka
{
    public class QueueOperationRdKafka : IBigQueueOperation
    {
        private ILogger _logger;
        private readonly IQueueOperationSubscriptionsManager _subsManager;
        public QueueOperationRdKafka( IQueueOperationSubscriptionsManager subsManager)
        {
            _subsManager = subsManager ?? new InMemorySubscriptionsManager();
            _logger = LoggerFactory.CreateLogger();
        }
        public void ErrorSubscribe()
        {
            throw new NotImplementedException();
        }

        public void PublishTopic(MessageBase msg,string queueName, string borkerList)
        {
            if (string.IsNullOrWhiteSpace(borkerList)) borkerList=ConnectionBuilder.getBorkerList();
            var topicConfig = new TopicConfig
            {
                CustomPartitioner = (top, key, cnt) =>
                {
                    cnt = 2;
                    key = Encoding.UTF8.GetBytes(queueName);
                    var kt = (key != null) ? Encoding.UTF8.GetString(key, 0, key.Length) : "(null)";
                    int partition = (key?.Length ?? 0) % cnt;
                    bool available = top.PartitionAvailable(partition);
                    _logger.LogInformation("Partitioner topic: {0} key: {1} partition count: {2} -> {3} {4}", top.Name, kt, cnt, partition, available);
                    return partition;
                }
            };

            using (Producer producer = new Producer(borkerList))
            using (Topic topic = producer.Topic(queueName, topicConfig))//topicConfig
            {
                DeliveryReport deliveryReport = topic.Produce(msg.MessageBodyByte).Result;
                _logger.LogInformation("发送到分区：{0}, Offset 为: {1}", deliveryReport.Partition, deliveryReport.Offset);
            }
        }
        
        public void SubscribeTopic<T, TH>(Func<TH> handler, string borkerList, string groupID, List<string> topics) 
            where TH : IProcessMessage<T>
            where T : MessageBase
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                
                Task.Run(() =>
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(borkerList)) borkerList = ConnectionBuilder.getBorkerList();
                        bool enableAutoCommit = false;

                        var config = new Config()
                        {
                            GroupId = groupID,
                            EnableAutoCommit = enableAutoCommit,
                            StatisticsInterval = TimeSpan.FromSeconds(60)
                        };

                        EventConsumer consumer = new EventConsumer(config, borkerList);

                        consumer.OnMessage += async (obj, msgs) => {
                            string text = Encoding.UTF8.GetString(msgs.Payload, 0, msgs.Payload.Length);
                            _logger.LogInformation("Topic: {0} Partition: {1} Offset: {2} {3}", msgs.Topic, msgs.Partition, msgs.Offset, text);

                            MessageBase mb = new MessageBase()
                            {
                                MessageBodyByte = msgs.Payload,
                                MessageRouter = msgs.Topic
                            };

                            await ProcessEvent(mb.MessageRouter, mb);
                            if (!enableAutoCommit && msgs.Offset % 10 == 0)
                            {
                                _logger.LogInformation($"Committing offset");
                                consumer.Commit(msgs).Wait();
                                _logger.LogInformation($"Committed offset");
                            }
                        };

                        consumer.OnConsumerError += (obj, errorCode) =>
                        {
                            _logger.LogWarning("Consumer Error: {0}", errorCode);
                        };

                        consumer.OnEndReached += (obj, end) => {
                            _logger.LogInformation("Reached end of topic {0} partition {1}, next message will be at offset {2}", end.Topic, end.Partition, end.Offset);
                        };

                        consumer.OnError += (obj, error) => {
                            _logger.LogWarning("Error: {0} {1}", error.ErrorCode, error.Reason);
                        };

                        if (enableAutoCommit)
                        {
                            consumer.OnOffsetCommit += (obj, commit) => {
                                if (commit.Error != ErrorCode.NO_ERROR)
                                {
                                    _logger.LogInformation("Failed to commit offsets: {0}", commit.Error);
                                }
                                _logger.LogInformation("Successfully committed offsets: [{0}]", string.Join(", ", commit.Offsets));
                            };
                        }

                        consumer.OnPartitionsAssigned += (obj, partitions) => {
                            _logger.LogInformation("Assigned partitions: [{0}], member id: {1}", string.Join(", ", partitions), consumer.MemberId);
                            consumer.Assign(partitions);
                        };

                        consumer.OnPartitionsRevoked += (obj, partitions) => {
                            _logger.LogInformation("Revoked partitions: [{0}]", string.Join(", ", partitions));
                            consumer.Unassign();
                        };

                        consumer.OnStatistics += (obj, json) => {
                            _logger.LogInformation("Statistics: {0}", json);
                        };

                        consumer.Subscribe(topics);
                        consumer.Start();

                        _logger.LogInformation("Assigned to: [{0}]", string.Join(", ", consumer.Assignment));
                        _logger.LogInformation("Subscribed to: [{0}]", string.Join(", ", consumer.Subscription));
                        _logger.LogInformation("Started consumer, press enter to stop consuming");

                        while (true)
                        {
                            Thread.Sleep(1);
                        }
                        // Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("SubscribeTopicAsync", ex);
                        return ex.Message;
                    }
                });//----------end run

            }//-----------end if
            _subsManager.AddSubscription<T, TH>(handler);
        }//-----------end SubscribeTopic


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
