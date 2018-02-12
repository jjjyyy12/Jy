using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jy.EntityFramewordCoreBase.Repositories
{
    public sealed class EntityFrameworkRepositoryContext : RepositoryContext<DbContext>
    {
        private bool disposed = false;

        public EntityFrameworkRepositoryContext(DbContext session, IRepositoryFactory repositoryFactory, IUnitOfWork unitOfWork) : base(session, repositoryFactory, unitOfWork)
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
