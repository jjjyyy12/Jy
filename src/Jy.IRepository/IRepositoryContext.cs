using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.IRepositories
{
    public interface IRepositoryContext : IDisposable
    {
        Guid Id { get; }

        object Session { get; }

        IRepositoryFactory RepositoryFactory { get; }

        IUnitOfWork UnitOfWork { get; }
    }

    public interface IRepositoryContext<out TSession> : IRepositoryContext
        where TSession : class
    {
        new TSession Session { get; }
    }
}
