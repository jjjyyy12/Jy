using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.IRepositories
{
    public interface IRepositoryReadContext : IDisposable
    {
        Guid Id { get; }

        object Session { get; }
    }

    public interface IRepositoryReadContext<out TSession> : IRepositoryReadContext
        where TSession : class
    {
        new TSession Session { get; }
    }
}
