using Jy.DapperBase;
using Jy.IRepositories;
using System.Data.Common;

namespace Jy.DapperBase.Repositories
{
    public class DapperRepositoryContext : RepositoryContext<TransactedConnection>
    {
        private bool disposed = false;
        public DapperRepositoryContext(TransactedConnection session) : base(session,(IUnitOfWork) session)
        {
        }
        public override void Dispose()
        {
            if (!disposed)
            {
                this.Session.Dispose();
                disposed = true;
            }
        }
    }
}
