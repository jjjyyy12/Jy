
using System;
using System.Collections.Generic;


namespace Jy.IMessageQueue
{
    //消息总线
    public interface IQueueOperationSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<T, TH>(Func<TH> handler)
           where T : MessageBase
           where TH : IProcessMessage<T>;

        void RemoveSubscription<T, TH>()
             where TH : IProcessMessage<T>
             where T : MessageBase;
        bool HasSubscriptionsForEvent<T>() where T : MessageBase;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<Delegate> GetHandlersForEvent<T>() where T : MessageBase;
        IEnumerable<Delegate> GetHandlersForEvent(string eventName);
    }
}
