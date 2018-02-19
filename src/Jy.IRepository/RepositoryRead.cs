using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jy.IRepositories
{
    public abstract class RepositoryRead<TEntity, TPrimaryKey> : IRepositoryRead<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        public IRepositoryReadContext Context { get; }
        public RepositoryRead(IRepositoryReadContext context)
        {
            this.Context = context;
        }

        public abstract TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        public abstract TEntity Get(TPrimaryKey id);

        public abstract List<TEntity> GetAllList();

        public abstract List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        public abstract IQueryable<TEntity> LoadPageList(int startPage, int pageSize, out int rowCount, Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> order);

    }
}
