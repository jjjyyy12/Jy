using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jy.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChange();
        Task SaveChangeAsync(CancellationToken cancellationToken = default(CancellationToken));
        
    }
}
