using System;
using System.Linq;
using System.Threading.Tasks;
using RawRabbit.Extensions.MessageSequence;
using RawRabbit.Configuration.Exchange;
using RawRabbit.ErrorHandling;
using RawRabbit.Common;
using RawRabbit.Attributes;
using RawRabbit.Logging;
using RawRabbit.vNext;
using System.Threading;
using RawRabbit.Extensions.BulkGet;
using Jy.IMessageQueue;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Reflection.Emit;
using RawRabbit;

namespace Jy.RabbitMQ
{
 
    public class QueueOperationRawRabbit : IQueueOperationRawRabbit
    {
        private readonly ILogger<QueueOperationRawRabbit> _logger;
        private readonly IQueueOperationSubscriptionsManager _subsManager;
        private Dictionary<string,IBusClient<MessageContext>> busClientList = new Dictionary<string,IBusClient<MessageContext>>();
        private Dictionary<string,RawRabbit.vNext.Disposable.IBusClient<MessageContext>> extbusClientList = new Dictionary<string,RawRabbit.vNext.Disposable.IBusClient<MessageContext>>();
        public QueueOperationRawRabbit(ILogger<QueueOperationRawRabbit> logger, IQueueOperationSubscriptionsManager subsManager)
        {
            _logger = logger;
            _subsManager = subsManager ?? new InMemorySubscriptionsManager();
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }
        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
             Dispose(eventName);
        }
        public void Dispose(string queueName)
        {
            if (busClientList.ContainsKey(queueName))
            {
                busClientList[queueName].ShutdownAsync();
            }
            if (extbusClientList.ContainsKey(queueName))
            {
                extbusClientList[queueName].Dispose();
            }
            _subsManager.Clear();
        }
        public void Dispose()
        {
            foreach(KeyValuePair<string, RawRabbit.IBusClient<MessageContext>> temp1 in busClientList)
            {
                temp1.Value.ShutdownAsync();
            }
            foreach (KeyValuePair<string, RawRabbit.vNext.Disposable.IBusClient<MessageContext>> temp2 in extbusClientList)
            {
                temp2.Value.Dispose();
            }
        }
        /// <summary>
        /// 推送topic消息
        /// </summary>
        /// <param name="msg"></param>
        public void PublishTopic(MessageBase msg)
        {
            var client = ConnectionBuilder.getClientRawRabbit();
            client.PublishAsync<MessageBase>(msg, Guid.NewGuid(), conf => {
                if(!string.IsNullOrWhiteSpace(msg.exchangeName))
                    conf.WithExchange(e => e.WithName(msg.exchangeName).WithType(ExchangeType.Topic));
                conf.WithRoutingKey(msg.MessageRouter);
            });
             
        }
        /// <summary>
        /// 订阅监听topic类型的queue，往总线中添加事件和事件处理，第一次的事件加mq的监听
        /// </summary>
        /// <param name="handler">处理函数，func返回一个IProcessMessage</param>
        /// <param name="subscriberId">监听程序id</param>
        /// <param name="exchangeName">rabbitmq交换机名</param>
        /// <param name="queueName">队列名</param>
        /// <param name="bindKeyTopic">监听过滤topic的表达式，#.#.#.rpc</param>
        public void SubscribeTopic<T, TH>(Func<TH>handler, string subscriberId,string exchangeName,string queueName, string bindKeyTopic) 
            where TH : IProcessMessage<T>
            where T : MessageBase
        {
            if (handler == null) return;

            var eventName = typeof(T).Name;
            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();

            if (!containsKey)
            {
                var client = ConnectionBuilder.getClientRawRabbitNoSignal();
                if (!busClientList.ContainsKey(queueName))
                    busClientList.Add(queueName, client);

                client.SubscribeAsync<T>(async (message, context) => {
                   //throw new Exception("Oh oh!SubscribeTopic");
                   await ProcessEvent(message.MessageRouter, message);
                }
                , conf => {
                    if (!string.IsNullOrWhiteSpace(exchangeName))
                        conf.WithExchange(e => e.WithName(exchangeName).WithType(ExchangeType.Topic));
                    conf.WithQueue(q => q.WithName(queueName)
                            .WithArgument(QueueArgument.MessageTtl, (int)TimeSpan.FromSeconds(2).TotalMilliseconds)
                            .WithArgument(QueueArgument.DeadLetterExchange, ConnectionBuilder.Conventions.DeadLetterExchangeNamingConvention())
                            .WithDurability(true));
                    if (!string.IsNullOrWhiteSpace(subscriberId))
                        conf.WithSubscriberId(subscriberId);
                    conf.WithRoutingKey(bindKeyTopic);
                    conf.WithPrefetchCount(1);
                });
                
            }
            _subsManager.AddSubscription<T, TH>(handler);
        }
      
        /// <summary>
        /// 同步rpc形式的推送消息到queue
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns>response处理的回传结果</returns>
        public async Task<MessageBase> RequestTopic(MessageBase msg)
        {
            var client = ConnectionBuilder.getExtendableClient();
            MessageContext nmsg = new MessageContext()
            {
                message = msg,
                GlobalRequestId = msg.Id
            };

            var response = await client.RequestAsync<MessageContext, MessageContextResponse>(nmsg, Guid.NewGuid()
                , conf => {
                    if (!string.IsNullOrWhiteSpace(msg.exchangeName))
                        conf.WithExchange(e => e.WithName(msg.exchangeName).WithType(ExchangeType.Topic));
                    conf.WithRoutingKey(msg.MessageRouter);//.WithReplyQueue(q=>q.WithName(replyQueue));
                });
            return response.message;
        }
        /// <summary>
        /// 同步rpc形式的监听queue，往总线中添加事件和事件处理，第一次的事件加mq的监听，执行处理函数，返回给推送者，
        /// </summary>
        /// <param name="handler">处理函数，func返回一个IProcessMessage</param>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="bindKeyTopic"></param>
        public void ResponseTopic<T, TH>(Func<TH> handler,string exchangeName,string queueName, string bindKeyTopic)
            where TH : IProcessMessage<T>
            where T : MessageBase
        {
            if (handler == null) return;

            var eventName = typeof(T).Name;
            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                var client = ConnectionBuilder.getExtendableClientNoSignal();
                if (!extbusClientList.ContainsKey(queueName))
                    extbusClientList.Add(queueName, client);

                client.RespondAsync<MessageContext, MessageContextResponse>(async (request,context) =>
                {
                    //throw new Exception("Oh oh!ResponseTopic");
                    await ProcessEvent(request.message.MessageRouter, request.message);
              
                    return (new MessageContextResponse()
                    {
                        message = request.message,
                        GlobalRequestId = context.GlobalRequestId
                    });
                }
                , conf =>
                {
                    if (!string.IsNullOrWhiteSpace(exchangeName))
                        conf.WithExchange(e => e.WithName(exchangeName).WithType(ExchangeType.Topic));
                    conf.WithQueue(q => q.WithName(queueName)
                    .WithArgument(QueueArgument.MessageTtl, (int)TimeSpan.FromSeconds(2).TotalMilliseconds)
                    .WithArgument(QueueArgument.DeadLetterExchange, ConnectionBuilder.Conventions.DeadLetterExchangeNamingConvention()));
                    conf.WithRoutingKey(bindKeyTopic);
                });
               
            }
            _subsManager.AddSubscription<T, TH>(handler);

        }

        /// <summary>
        /// 消费端的错误处理，生产者端不知道，SubscribeTopic
        /// http://rawrabbit.readthedocs.io/en/master/error-handling.html
        /// </summary>
        /// <param name="ipro"></param>
        /// <param name="errorExchange"></param>
        public void ErrorSubscribe()
        {
            var client = ConnectionBuilder.getExtendableClientNoSignal();
            client.SubscribeAsync<HandlerExceptionMessage>(async (message, context) =>
            {
                var originalMsg = message.Message;
                var originalContext = context;
                var unhandled = message.Exception;
                await HandleAsync(message);
            }, c => c
                .WithExchange(e => e.WithName(ConnectionBuilder.Conventions.ErrorExchangeNamingConvention()).WithDurability(false))
                .WithQueue(q => q.WithArgument(QueueArgument.MessageTtl, (int)TimeSpan.FromSeconds(100).TotalMilliseconds)
                        .WithArgument(QueueArgument.DeadLetterExchange,ConnectionBuilder.Conventions.DeadLetterExchangeNamingConvention())
                        .WithAutoDelete())
                .WithRoutingKey("#"));//监听所有异步queue的异常，也可根据需要再建立异常处理方法，监听不同的topic表达式
        }
        //重发
        private Task<MessageBase> HandleAsync(HandlerExceptionMessage message)
        {
            MessageBase originalMsg = message.Message as MessageBase;
            if (originalMsg.MessageRouter.IndexOf("_normal") < 0) return Task.FromResult<MessageBase>(null); //非normal 不予处理
            originalMsg.FailedTimes++;

            Thread.Sleep(2000* originalMsg.FailedTimes);
            return Task.Run(() =>
            {
                //errorHandle
                if(originalMsg.FailedTimes!=4)  //4次机会重发,每次等待2*失败次数的时间
                    PublishTopic(originalMsg);
                return originalMsg;
            });
        }

        public void BulkSubscribeTopic(IProcessMessage ipro, string subscriberId, string exchangeName, string queueName, string bindKeyTopic)
        {
            var client = ConnectionBuilder.getExtendableClientNoSignal();
            if (!extbusClientList.ContainsKey(queueName))
                extbusClientList.Add(queueName, client);
            var bulk = client.GetMessages(cfg => cfg
                    .ForMessage<MessageBase>(msg => msg
                        .FromQueues(queueName)
                        .WithBatchSize(4))
                    );
            var basics = bulk.GetMessages<MessageBase>().ToList();

        }

        public void Unsubscribe<T, TH>()
        where TH : IProcessMessage<T>
        where T : MessageBase
        {
            _subsManager.RemoveSubscription<T, TH>();
        }
        private static Func<IProcessMessage> FindHandlerByType(Type handlerType, IEnumerable<Func<IProcessMessage>> handlers)
        {
            foreach (var func in handlers)
            {
                if (func.GetMethodInfo().ReturnType == handlerType)
                {
                    return func;
                }
            }

            return null;
        }
        //执行总线中该事件的所有handler
        private async Task ProcessEvent<T>(string eventName, T message) where T:MessageBase
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var eventType = _subsManager.GetEventTypeByName(eventName);
                var handlers = _subsManager.GetHandlersForEvent(eventName);

                await ExecuteHandlersSync(handlers, eventType, message);
            }
        }
        //并行
        private async Task ExecuteHandlersAsync<T>(IEnumerable<Delegate> handlers,Type eventType,T message) where T : MessageBase
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
        private void DynamicInvoke<T>(Type eventType, Delegate ihandler ,T message) where T : MessageBase
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


        //public void PublishTopicRPC(MessageBase msg)
        //{
        //    var client = ConnectionBuilder.getExtendableClient();

        //    MessageContext nmsg = new MessageContext()
        //    {
        //        message = msg,
        //        GlobalRequestId = msg.Id
        //    };
        //    var sequence = client.ExecuteSequence(c =>
        //                        c.PublishAsync<MessageContext>(nmsg)
        //                        .When<MessageContext>((message, context) => Task.FromResult(true))
        //                        .Complete<MessageContextResponse>()
        //                            );
        //}
        //public void SubscribeTopicRPC<T>(IProcessMessage<T> ipro, string subscriberId, string queueName, string bindKeyTopic) where T : MessageBase
        //{
        //    var client = ConnectionBuilder.getExtendableClientNoSignal();
        //    if (!extbusClientList.ContainsKey(queueName))
        //        extbusClientList.Add(queueName, client);
        //    client.SubscribeAsync<MessageContext>(async (message, context) => {
        //        ipro.ProcessMsg((T)message.message);

        //        await client.PublishAsync<MessageContextResponse>(new MessageContextResponse()
        //        {
        //            message = message.message,
        //            GlobalRequestId = context.GlobalRequestId
        //        }//,default(Guid),conf=>conf.WithRoutingKey(message.message.MessageReturnRouter)
        //        );
        //    }
        //    , conf =>
        //    {
        //        conf.WithQueue(q => q.WithName(queueName).WithDurability(true));
        //        conf.WithSubscriberId(subscriberId);
        //    });
        //}

        //public void Publish(MessageBase msg)
        //{
        //    throw new NotImplementedException();
        //    //var client = ConnectionBuilder.getClientRawRabbit();
        //    //client.PublishAsync<MessageBase>(msg, Guid.NewGuid(), conf=> { conf.WithRoutingKey(msg.MessageRouter); });
        //}
        //public void Subscribe(MessageBase msg, IProcessMessage ipro)
        //{
        //    //var client = ConnectionBuilder.getClientRawRabbit();
        //    //client.SubscribeAsync<MessageBase>(async (message, context) =>{  ipro.ProcessMsg(message); }
        //    //,conf=>{
        //    //    conf.WithRoutingKey(msg.MessageRouter);
        //    //});
        //}
        //public void PublishDirect(MessageBase msg, string queueName)
        //{
        //    throw new NotImplementedException();
        //    //var client = ConnectionBuilder.getClientRawRabbit();
        //    //client.PublishAsync<MessageBase>(msg, Guid.NewGuid(), conf => { conf.WithRoutingKey(queueName); });
        //}
        //public void SubscribeDirect(IProcessMessage ipro, string exchangeName, string queueName)
        //{
        //    throw new NotImplementedException();
        //    //var client = ConnectionBuilder.getClientRawRabbit();
        //    //client.SubscribeAsync<MessageBase>(async (message, context) => { ipro.ProcessMsg(message); }
        //    //, conf => {
        //    //    conf.WithExchange(exchange => exchange.WithName(exchangeName));
        //    //    conf.WithQueue(queue=>queue.WithName(queueName));
        //    //});
        //}
        //public void PublishFanout(MessageBase msg, string exchangeName)
        //{
        //    throw new NotImplementedException();
        //    //var client = ConnectionBuilder.getClientRawRabbit();
        //    //client.PublishAsync<MessageBase>(msg, Guid.NewGuid(), conf => { conf.WithExchange(exchange => { exchange.WithName(exchangeName); }); });
        //}

        //public void SubscribeFanout(IProcessMessage ipro, string exchangeName, string queueName)
        //{
        //    throw new NotImplementedException();
        //    //var client = ConnectionBuilder.getClientRawRabbit();
        //    //client.SubscribeAsync<MessageBase>(async (message, context) => { ipro.ProcessMsg(message); }
        //    //, conf => {
        //    //    conf.WithExchange(exchange => exchange.WithName(exchangeName));
        //    //    conf.WithQueue(queue => queue.WithName(queueName));
        //    //});
        //}
    }
}
