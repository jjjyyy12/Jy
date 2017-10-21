using System;

namespace Jy.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChange();
    }
}
