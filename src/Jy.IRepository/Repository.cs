using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jy.IRepositories
{
    public abstract class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        public IRepositoryContext Context { get; }
        public IUnitOfWork UnitOfWork { get; }
        public Repository(IRepositoryContext context)
        {
            this.Context = context;
            this.UnitOfWork = context.UnitOfWork;
        }



        public abstract void Delete(TEntity entity, bool autoSave = true);


        public abstract void Delete(TPrimaryKey id, bool autoSave = true);


        public abstract void Delete(Expression<Func<TEntity, bool>> where, bool autoSave = true);


        public abstract void Execute(Action action);


        public abstract Task ExecuteAsync(Func<Task> action);

        public abstract TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        public abstract TEntity Get(TPrimaryKey id);

        public abstract List<TEntity> GetAllList();

        public abstract List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        public abstract TEntity Insert(TEntity entity, bool autoSave = true);

        public virtual TEntity InsertOrUpdate(TEntity entity, bool autoSave = true)
        {
            if (!(default(Guid).Equals(entity.Id)))//if (Get(entity.Id) != null)
                return Update(entity, autoSave);
            return Insert(entity, autoSave);
        }

        public abstract IQueryable<TEntity> LoadPageList(int startPage, int pageSize, out int rowCount, Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> order);

        public abstract void Save();

        public virtual TEntity Update(TEntity entity, bool autoSave = true)
        {
            var obj = Get(entity.Id);
            EntityToEntity(entity, obj);
            if (autoSave)
                Save();
            return entity;
        }
        public void EntityToEntity<T>(T pTargetObjSrc, T pTargetObjDest)
        {
            foreach (var mItem in typeof(T).GetProperties())
            {
                mItem.SetValue(pTargetObjDest, mItem.GetValue(pTargetObjSrc, new object[] { }), null);
            }
        }
    }
}
