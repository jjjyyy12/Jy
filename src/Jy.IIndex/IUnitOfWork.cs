using System;

namespace Jy.IIndex
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChange();
    }
}
