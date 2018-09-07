
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jy.EntityFramewordCoreBase.Repositories
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    /// <typeparam name="TDBContext">dbcontext</typeparam>
    public abstract class EntityFrameworkRepositoryBase<TEntity, TPrimaryKey> : Repository<TEntity, TPrimaryKey> 
        where TEntity : Entity<TPrimaryKey>
        
    {
        //定义数据访问上下文对象
        protected readonly DbContext _dbContext;

        /// <summary>
        /// 通过构造函数注入得到数据上下文对象实例
        /// </summary>
        /// <param name="dbContext"></param>
        public EntityFrameworkRepositoryBase(IRepositoryContext context) : base(context)
        {
            var entityFrameworkRepositoryContext = (EntityFrameworkRepositoryContext)context;
            _dbContext = entityFrameworkRepositoryContext.Session;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        public override List<TEntity> GetAllList()
        {
            return _dbContext.Set<TEntity>().ToList();
        }

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public override List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Where(predicate).ToList();
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public override TEntity Get(TPrimaryKey id)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public override TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        /// <returns></returns>
        public override TEntity Insert(TEntity entity, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Add(entity);
            if (autoSave)
                Save();
            return entity;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public override TEntity Update(TEntity entity, bool autoSave = true)
        {
            var obj = Get(entity.Id);
            EntityToEntity(entity, obj);
            if (autoSave)
                Save();
            return entity;
        }
        public TEntity Update1(TEntity entity, bool autoSave = true)
        {
            //using (var transaction = _dbContext.Database.BeginTransaction())
            //{
            //    try
            //    {
                    _dbContext.Update<TEntity>(entity);
                    if (autoSave)
                        Save();
            //transaction.Commit();
            //    }
            //    catch (Exception e)
            //    {
            //        transaction.Rollback();
            //    }
            //}
            return entity;
        }
        
        public  void EntityToEntity<T>(T pTargetObjSrc, T pTargetObjDest)
        {
            foreach (var mItem in typeof(T).GetProperties())
            {
                mItem.SetValue(pTargetObjDest, mItem.GetValue(pTargetObjSrc, new object[] { }), null);
            }
        }
        /// <summary>
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public TEntity InsertOrUpdate(TEntity entity, bool autoSave = true)
        {
            if (!(default(Guid).Equals(entity.Id)))//if (Get(entity.Id) != null)
                return Update(entity, autoSave);
            return Insert(entity, autoSave);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public override void Delete(TEntity entity, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            if (autoSave)
                Save();
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public override void Delete(TPrimaryKey id, bool autoSave = true)
        {
            var obj = Get(id);
            if (obj == null) return;
            _dbContext.Set<TEntity>().Remove(Get(id));
            if (autoSave)
                Save();
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">lambda表达式</param>
        /// <param name="autoSave">是否自动保存</param>
        public override void Delete(Expression<Func<TEntity, bool>> where, bool autoSave = true)
        {
            //_dbContext.Set<TEntity>().RemoveRange(_dbContext.Set<TEntity>().Where(where).ToList());
            _dbContext.Set<TEntity>().Where(where).ToList().ForEach(it => _dbContext.Set<TEntity>().Remove(it));
            if (autoSave)
                Save();
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
            var result = from p in _dbContext.Set<TEntity>()
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

        public async Task<PaginatedList<TEntity>> GetPaginatedAsync(PaginationBase parameters, PropertyMapping propertyMapping)
        {
            var collectionBeforePaging = _dbContext.Set<TEntity>().ApplySort(parameters.OrderBy, propertyMapping);
            parameters.Count = await collectionBeforePaging.CountAsync();
            var items = await collectionBeforePaging.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var result = new PaginatedList<TEntity>(parameters, items);
            return result;
        }

        public async Task<PaginatedList<TEntity>> GetPaginatedAsync(PaginationBase parameters, PropertyMapping propertyMapping, Expression<Func<TEntity, bool>> criteria)
        {
            var collectionBeforePaging = _dbContext.Set<TEntity>().Where(criteria).ApplySort(parameters.OrderBy, propertyMapping);
            parameters.Count = await collectionBeforePaging.CountAsync();
            var items = await collectionBeforePaging.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var result = new PaginatedList<TEntity>(parameters, items);
            return result;
        }

        public async Task<PaginatedList<TEntity>> GetPaginatedAsync(PaginationBase parameters, PropertyMapping propertyMapping, Expression<Func<TEntity, bool>> criteria,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var collectionBeforePaging = includes
                .Aggregate(_dbContext.Set<TEntity>().Where(criteria).ApplySort(parameters.OrderBy, propertyMapping),
                    (current, include) => current.Include(include));
            parameters.Count = await collectionBeforePaging.CountAsync();
            var items = await collectionBeforePaging.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var result = new PaginatedList<TEntity>(parameters, items);
            return result;
        }
        /*
          [HttpGet]
        [Route("Paged", Name = "GetPagedVehicles")]
        public async Task<IActionResult> GetPaged(QueryViewModel parameters)
        {
            var propertyMapping = new VehiclePropertyMapping();
            PaginatedList<Vehicle> pagedList;
            if (string.IsNullOrEmpty(parameters.SearchTerm))
            {
                pagedList = await _vehicleRepository.GetPaginatedAsync(parameters, propertyMapping);
            }
            else
            {
                pagedList = await _vehicleRepository.GetPaginatedAsync(parameters, propertyMapping,
                    x => x.Model.Contains(parameters.SearchTerm) || x.Owner.Contains(parameters.SearchTerm));
            }
            var vehicleVms = Mapper.Map<IEnumerable<VehicleViewModel>>(pagedList);
            vehicleVms = vehicleVms.Select(CreateLinksForVehicle);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagedList.PaginationBase));
            var wrapper = new LinkedCollectionResourceWrapperViewModel<VehicleViewModel>(vehicleVms);
            return Ok(CreateLinksForVehicle(wrapper));
        }
             */
        /// <summary>
        /// 事务性保存
        /// </summary>
        public override void Save()
        {
            _dbContext.SaveChanges(RefreshConflict.MergeClientAndStore);
        }

        /// <summary>
        /// 根据主键构建判断表达式
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));
            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        public override void Execute(Action action)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    action();
                    transaction.Commit();
                }
            });
        }
        public override async Task ExecuteAsync(Func<Task> action)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    await action();
                    transaction.Commit();
                }
            });
        }
        //    public List<TEntity> Query<TEntity>(string sql, List<DbParameter> parms, CommandType cmdType = CommandType.Text)
        //    {
        //        //存储过程（exec getActionUrlId @name,@ID）
        //        if (cmdType == CommandType.StoredProcedure)
        //        {
        //            StringBuilder paraNames = new StringBuilder();
        //            foreach (var sqlPara in parms)
        //            {
        //                paraNames.Append($" @{sqlPara},");
        //            }
        //            sql = paraNames.Length > 0 ? $"exec {sql} {paraNames.ToString().Trim(',')}" : $"exec {sql} ";
        //        }
        //        var list = _dbContext.Database.<TEntity>(sql, parms.ToArray());
        //        var enityList = list.ToList();
        //        return enityList;
        //    }
    }

    /// <summary>
    /// 主键为Guid类型的仓储基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TDBContext">dbcontext</typeparam>
    public abstract class EntityFrameworkRepositoryBase<TEntity> : EntityFrameworkRepositoryBase<TEntity, Guid> where TEntity : Entity
    {
        public EntityFrameworkRepositoryBase(IRepositoryContext context) : base(context)
        {
        }
    }
}
