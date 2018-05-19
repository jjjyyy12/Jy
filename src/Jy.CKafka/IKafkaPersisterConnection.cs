using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CKafka
{
    public interface IKafkaPersisterConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        Object CreateConnect();
    }
}
