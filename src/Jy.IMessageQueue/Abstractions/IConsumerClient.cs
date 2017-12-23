using System;
using System.Collections.Generic;
using System.Threading;

namespace Jy.IMessageQueue
{
    //消息监听
    public interface IConsumerClient<T> :IDisposable where T : MessageBase
    {
        void Subscribe(IEnumerable<string> topics);

        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        void Commit();

        event EventHandler<T> OnMessageReceievedToOutSide;

        event EventHandler<string> OnError;
    }
}
