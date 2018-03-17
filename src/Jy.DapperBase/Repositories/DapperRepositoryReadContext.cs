using Jy.IRepositories;
using System.Data.Common;

namespace Jy.DapperBase.Repositories
{
    public class DapperRepositoryReadContext : RepositoryReadContext<DbConnection>
    {
        private bool disposed = false;
        public DapperRepositoryReadContext(DbConnection session) : base(session)
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
