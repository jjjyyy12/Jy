using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
 
namespace Jy.EntityFramewordCoreBase.Repositories
{
    public class EntityFrameworkRepositoryReadContext : RepositoryReadContext<DbContext>
    {
        private bool disposed = false;
        public EntityFrameworkRepositoryReadContext(DbContext session) : base(session)
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
