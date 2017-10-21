using Jy.ILog;
using Jy.IMessageQueue;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace Jy.RabbitMQ
{
    public class QueueOperation : IQueueOperation
    {
        private readonly ILogger _logger;
        private readonly IQueueOperationSubscriptionsManager _subsManager;
        private readonly PublishQueueExecutor _publishQueueExecutor;

        private Dictionary<string, object> busClientList = new Dictionary<string, object>();
        private readonly ConnectionPool _connectionPool;
        private readonly RabbitMQOptions _rabbitMQOptions;
        public QueueOperation(ILogger logger, ConnectionPool connectionPool, RabbitMQOptions rabbitMQOptions, IQueueOperationSubscriptionsManager subsManager, PublishQueueExecutor publishQueueExecutor)
        {
            _logger = logger;
            _subsManager = subsManager ?? new InMemorySubscriptionsManager();
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
            _publishQueueExecutor = publishQueueExecutor;
            _connectionPool = connectionPool;
            _rabbitMQOptions = rabbitMQOptions;
        }

        private void SubsManager_OnEventRemoved (object sender, string eventName)
        {
            Dispose(eventName);
        }

        public void Dispose(string queueName)
        {
            if (busClientList.ContainsKey(queueName))
            {
                var obj = ( RabbitMQConsumerClient <MessageBase>) busClientList[queueName];
                obj.Dispose();
            }
        }

        public void Dispose()
        {
            foreach (var temp1 in busClientList)
            {
                var obj = (RabbitMQConsumerClient<MessageBase>)temp1.Value;
                obj.Dispose();
            }
        }

        public void ErrorSubscribe()
        {
        }

        public void PublishTopic(MessageBase msg)
        {
            _publishQueueExecutor.Publish(msg);
        }

        public void SubscribeTopic<T, TH>(Func<TH> handler, string subscriberId, string exchangeName, string queueName, string bindKeyTopic)
            where T : MessageBase
            where TH : IProcessMessage<T>
        {
            if (handler == null) return;
            var eventName = typeof(T).Name;
            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                //var processEvent = new ProcessEvent(
                //   async (x, y) => { await ProcessEvent(x, y); }
                //    );
                var client = new RabbitMQConsumerClient<T>(exchangeName,queueName, "", false,_connectionPool, _rabbitMQOptions);//, processEvent
                if (!busClientList.ContainsKey(queueName))
                    busClientList.Add(queueName, client);

                client.Subscribe(new List<string>(1) { bindKeyTopic });
                client.Listening(new TimeSpan(0,0,2),CancellationToken.None);
                client.OnMessageReceievedToOutSide += async (x, y) => {  await ProcessEvent(queueName, y); };
                }
            _subsManager.AddSubscription<T, TH>(handler);
        }
        public Task<MessageBase> RequestTopic(MessageBase msg)
        {
            return _publishQueueExecutor.RequestTopic(msg);
        }

        public void ResponseTopic<T, TH>(Func<TH> handler, string exchangeName, string queueName, string bindKeyTopic)
            where T : MessageBase
            where TH : IProcessMessage<T>
        {
            if (handler == null) return;
            var eventName = typeof(T).Name;
            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                //var processEvent = new ProcessEvent(
                //   async (x, y) => { await ProcessEvent(x, y); }
                //    );
                var client = new RabbitMQConsumerClient<T>(exchangeName,queueName, $"{queueName}_reply", true, _connectionPool, _rabbitMQOptions);
                if (!busClientList.ContainsKey(queueName))
                    busClientList.Add(queueName, client);
                client.Subscribe(new List<string>(1) { bindKeyTopic });
                client.Listening(new TimeSpan(0, 0, 2), CancellationToken.None);
                client.OnMessageReceievedToOutSide +=  (x, y) => {
                     ProcessRequestEvent(queueName, y);
                };
                //client.ResponseTopic(exchangeName, queueName, bindKeyTopic);
            }
            _subsManager.AddSubscription<T, TH>(handler);
        }


        public void Unsubscribe<T, TH>()
            where T : MessageBase
            where TH : IProcessMessage<T>
        {
            _subsManager.RemoveSubscription<T, TH>();
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
        private void ProcessRequestEvent<T>(string eventName, T message) where T : MessageBase
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var eventType = _subsManager.GetEventTypeByName(eventName);
                var handlers = _subsManager.GetHandlersForEvent(eventName);
                ExecuteHandlers(handlers, eventType, message);
            }
        }
        //异步并行
        private async Task ExecuteHandlersAsync<T>(IEnumerable<Delegate> handlers, Type eventType, T message) where T : MessageBase
        {
            foreach (var handlerfactory in handlers)
            {
                await Task.Run(() => {
                    DynamicEmitInvoke(eventType, handlerfactory, message); //emit
                });
            }
        }
        //异步串行
        private async Task ExecuteHandlersSync<T>(IEnumerable<Delegate> handlers, Type eventType, T message) where T : MessageBase
        {
            await Task.Run(() => {
                foreach (var handlerfactory in handlers)
                {
                    DynamicEmitInvoke(eventType, handlerfactory, message); //emit
                }
            });
        }
        //同步串行
        private void ExecuteHandlers<T>(IEnumerable<Delegate> handlers, Type eventType, T message) where T : MessageBase
        { 
            foreach (var handlerfactory in handlers)
            {
                DynamicEmitInvoke(eventType, handlerfactory, message); //emit
            }
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
