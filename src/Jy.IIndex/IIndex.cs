﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jy.IIndex
{
    /// <summary>
    /// 仓储接口定义
    /// </summary>
    public interface IIndex
    {
        IUnitOfWork UnitOfWork { get; }
    }
    /// <summary>
    /// 定义泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public interface IIndex<TEntity, TPrimaryKey> : IIndex where TEntity : Entity<TPrimaryKey>
    {
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        List<TEntity> GetAllList();

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
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
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        TEntity FirstOrDefault(ICollection<KeyValuePair<string, string>> wheres);

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        /// <returns></returns>
        TEntity Insert(TEntity entity, bool autoSave = true);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        TEntity Update(TEntity entity, bool autoSave = true);

        /// <summary>
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        TEntity InsertOrUpdate(TEntity entity, bool autoSave = true);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        void Delete(TEntity entity, bool autoSave = true);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <param name="autoSave">是否立即执行保存</param>
        void Delete(TPrimaryKey id, bool autoSave = true);

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">lambda表达式</param>
        /// <param name="autoSave">是否自动保存</param>
        void Delete(ICollection<KeyValuePair<string, string>> wheres, bool autoSave = true);

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

        void Save();

        
    }

    /// <summary>
    /// 默认Guid主键类型索引
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IIndex<TEntity> : IIndex<TEntity, Guid> where TEntity : Entity
    {

    }

}
