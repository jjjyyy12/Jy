using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jy.IMessageQueue
{
    public interface IConsumerClient<T> :IDisposable where T : MessageBase
    {
        void Subscribe(IEnumerable<string> topics);

        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        void Commit();

        event EventHandler<T> OnMessageReceievedToOutSide;

        event EventHandler<string> OnError;
    }
}
