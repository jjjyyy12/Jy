
using Dapper;
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Jy.DapperBase.Repositories.Extensions;
 
namespace Jy.DapperBase.Repositories
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    /// <typeparam name="TDBContext">dbcontext</typeparam>
    public abstract class DapperRepositoryReadBase<TEntity, TPrimaryKey> : RepositoryRead<TEntity, TPrimaryKey> 
        where TEntity : Entity<TPrimaryKey>
        
    {
        //定义数据访问上下文对象
        protected readonly TransactedConnection _dbContext;

        /// <summary>
        /// 通过构造函数注入得到数据上下文对象实例
        /// </summary>
        /// <param name="dbContext"></param>
        public DapperRepositoryReadBase(IRepositoryReadContext context) : base(context)
        {
            var dapperRepositoryContext = (DapperRepositoryReadContext)context;
            _dbContext = dapperRepositoryContext.Session;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        public override List<TEntity> GetAllList()
        {
            return _dbContext.connection.Query<TEntity>(StatementFactory.Select<TEntity>(Dialect.MSSQL)).ToList();
        }
        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public override List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.connection.Query<TEntity>(predicate).ToList();
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public override TEntity Get(TPrimaryKey id)
        {
            var sql = StatementFactory.Select<TEntity>(Dialect.MSSQL, new { Id = id });
            //string sql = "select * from " + typeof(TEntity).ToString() + " where id=@id";
            DynamicParameters para = new DynamicParameters();
            para.Add("id", id);
            return _dbContext.connection.Query<TEntity>(sql, para).FirstOrDefault();
        }

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public override TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.connection.Query<TEntity>(predicate).FirstOrDefault();
            //string sql = "select * from " + typeof(TEntity).ToString();
            //return _dbContext.connection.Query<TEntity>(sql).FirstOrDefault(predicate.Compile());
        }

        public DynamicParameters EntityToValue<T>(T pTargetObjSrc)
        {
            DynamicParameters para = new DynamicParameters();
            foreach (var mItem in typeof(T).GetProperties())
            {
                para.Add(mItem.Name, mItem.GetValue(pTargetObjSrc, new object[] { }));
            }
            return para;
        }
       
       
        
        public  void EntityToEntity<T>(T pTargetObjSrc, T pTargetObjDest)
        {
            foreach (var mItem in typeof(T).GetProperties())
            {
                mItem.SetValue(pTargetObjDest, mItem.GetValue(pTargetObjSrc, new object[] { }), null);
            }
        }
      
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="startPage">页码</param>
        /// <param name="pageSize">单页数据数</param>
        /// <param name="rowCount">行数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public override IQueryable<TEntity> LoadPageList(int startPage, int pageSize, out int rowCount, Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null)
        {
             
            var result = from p in _dbContext.connection.Query<TEntity>()
                         select p;
            if (where != null)
                result = result.Where(where);
            if (order != null)
                result = result.OrderBy(order);
            else
                result = result.OrderBy(m => m.Id);
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize);
        }
        
        
    }

    /// <summary>
    /// 主键为Guid类型的仓储基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TDBContext">dbcontext</typeparam>
    public abstract class DapperRepositoryReadBase<TEntity> : DapperRepositoryReadBase<TEntity, Guid> where TEntity : Entity
    {
        public DapperRepositoryReadBase(IRepositoryReadContext context) : base(context)
        {
        }
    }
}
