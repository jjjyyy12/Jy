using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
 
namespace Jy.EntityFramewordCoreBase.Repositories
{
    public class EntityFrameworkRepositoryContext : RepositoryContext<DbContext>
    {
        private bool disposed = false;
        public EntityFrameworkRepositoryContext(DbContext session) : base(session,(IUnitOfWork) session)
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
