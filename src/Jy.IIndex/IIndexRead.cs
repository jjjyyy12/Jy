using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Jy.IIndex
{
    public interface IIndexRead
    {
    }
    /// <summary>
    /// 定义泛型仓索引储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public interface IIndexRead<TEntity, TPrimaryKey> : IIndexRead where TEntity : Entity<TPrimaryKey>
    {
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        List<TEntity> GetAllList();

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="wheres">条件</param>
        /// <returns></returns>
        List<TEntity> GetAllList(ICollection<KeyValuePair<string, string>> wheres);

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        TEntity Get(TPrimaryKey id);

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="wheres">条件</param>
        /// <returns></returns>
        TEntity FirstOrDefault(ICollection<KeyValuePair<string, string>> wheres);

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面条目</param>
        /// <param name="rowCount">数据总数</param>
        /// <param name="where">查询条件</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        List<TEntity> LoadPageList(int startPage, int pageSize, out int rowCount, ICollection<KeyValuePair<string, string>> wheres = null, ICollection<KeyValuePair<string, string>> orders = null);

    }

    /// <summary>
    /// 默认Guid主键类型索引
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IIndexRead<TEntity> : IIndexRead<TEntity, Guid> where TEntity : Entity
    {

    }
}
