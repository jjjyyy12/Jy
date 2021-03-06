﻿using Jy.IIndex;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SolrNetCore;
using SolrNetCore.Commands.Parameters;

namespace Jy.SolrIndex
{
    /// <summary>
    ///  基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    /// <typeparam name="TSolrOperations">solrOperations</typeparam>
    public abstract class IndexReadBase<TEntity, TPrimaryKey, TSolrOperations> : IIndexRead<TEntity, TPrimaryKey>
        where TEntity : Entity<TPrimaryKey>
        where TSolrOperations : ISolrReadOnlyOperations<TEntity>
    {
        //定义数据访问上下文对象
        protected readonly TSolrOperations _solrOperations;


        /// <summary>
        /// 通过构造函数注入得到数据上下文对象实例
        /// </summary>
        /// <param name="solrOperations"></param>
        public IndexReadBase(TSolrOperations solrOperations)
        {
            _solrOperations = solrOperations;
        }
         
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        public List<TEntity> GetAllList()
        {
            return _solrOperations.Query(SolrQuery.All);
            //var results = _solrOperations.Query(SolrQuery.All);
            //List<TEntity> reslist = new List<TEntity>();
            //results.ForEach(x => { reslist.Add(x); });
            //return reslist;
        }

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public List<TEntity> GetAllList(ICollection<KeyValuePair<string, string>> wheres)
        {
            if (wheres.Count > 1)
            {
                SolrQueryByField[] conds = new SolrQueryByField[wheres.Count];
                int i = 0;
                foreach(var param in wheres)
                {
                    if(!string.IsNullOrWhiteSpace(param.Key))
                        conds[i] = new SolrQueryByField(param.Key, param.Value);
                    i++;
                }
                QueryOptions qo = new QueryOptions();
                qo.AddFilterQueries(conds);
                return _solrOperations.Query(SolrQuery.All, qo);
            }
            else
            {
                SolrQueryByField qf=null;
                foreach (var param in wheres)
                        qf = new SolrQueryByField(param.Key, param.Value);
                return _solrOperations.Query(qf);
            }
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public TEntity Get(TPrimaryKey id)
        {
            return _solrOperations.Query(new SolrQueryByField("id", id.ToString()))[0];
        }

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public TEntity FirstOrDefault(ICollection<KeyValuePair<string, string>> wheres)
        {
            if (wheres.Count > 1)
            {
                SolrQueryByField[] conds = new SolrQueryByField[wheres.Count];
                int i = 0;
                foreach (var param in wheres)
                {
                    if (!string.IsNullOrWhiteSpace(param.Key))
                        conds[i] = new SolrQueryByField(param.Key, param.Value);
                    i++;
                }
                QueryOptions qo = new QueryOptions();
                qo.AddFilterQueries(conds);
                var res = _solrOperations.Query(SolrQuery.All, qo);
                if (res?.Count > 0)
                    return res[0];
                else
                    return null;
            }
            else
            {
                SolrQueryByField qf = null;
                foreach (var param in wheres)
                    qf = new SolrQueryByField(param.Key, param.Value);
                var res = _solrOperations.Query(qf);
                if (res?.Count > 0)
                    return res[0];
                else
                    return null;
            }
        }


        public void EntityToEntity<T>(T pTargetObjSrc, T pTargetObjDest)
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
        public List<TEntity> LoadPageList(int startPage, int pageSize, out int rowCount, ICollection<KeyValuePair<string, string>> wheres = null, ICollection<KeyValuePair<string, string>> orders = null)
        {
            QueryOptions options = new QueryOptions();
            //分页参数
            options.Rows = pageSize; //数据条数
            options.Start = startPage;  //开始项    
            
            // 拼接相关查询条件
            SolrQueryByField[] conds = new SolrQueryByField[wheres.Count];
            int i = 0;
            foreach (var param in wheres)
            {
                if (!string.IsNullOrWhiteSpace(param.Key))
                    conds[i] = new SolrQueryByField(param.Key, param.Value);
                i++;
            }
            options.AddFilterQueries(conds);

            //执行查询
            var results = _solrOperations.Query(SolrQuery.All, options);
            // 得到返回的数据总条数和total和 总页数 用于分页显示，
            rowCount = results.NumFound;
            return results;
        }

  
    }
    /// <summary>
    /// 主键为Guid类型的基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TSolrOperations">SolrOperations</typeparam>
    public abstract class IndexReadBase<TEntity, TSolrOperations> : IndexReadBase<TEntity, Guid, TSolrOperations> where TEntity : Entity where TSolrOperations : ISolrReadOnlyOperations<TEntity>
    {
        public IndexReadBase(TSolrOperations solrOperations) : base(solrOperations)
        {
        }
    }
}
