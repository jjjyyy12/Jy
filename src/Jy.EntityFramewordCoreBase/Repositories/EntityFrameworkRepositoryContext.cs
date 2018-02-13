using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
 
namespace Jy.EntityFramewordCoreBase.Repositories
{
    public sealed class EntityFrameworkRepositoryContext : RepositoryContext<DbContext>
    {
        private bool disposed = false;

        public EntityFrameworkRepositoryContext(DbContext session, IRepositoryFactory repositoryFactory, IUnitOfWork unitOfWork) : base(session, repositoryFactory, unitOfWork)
        {
        }
        public EntityFrameworkRepositoryContext(DbContext session, IRepositoryFactory repositoryFactory) : base(session, repositoryFactory,(IUnitOfWork) session)
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
