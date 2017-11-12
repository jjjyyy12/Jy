﻿using Jy.IIndex;
using SolrNetCore;
using SolrNetCore.Commands.Parameters;
using SolrNetCore.Exceptions;
using SolrNetCore.Impl;
using SolrNetCore.Mapping.Validation;
using SolrNetCore.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jy.AuthAdmin.SolrIndex
{
    public class AuthAdminSolrServer<T>: ISolrOperations<T> 
    {
        private readonly ISolrBasicOperations<T> basicServer;
        private readonly IReadOnlyMappingManager mappingManager;
        private readonly IMappingValidator _schemaMappingValidator;

        public AuthAdminSolrServer(ISolrBasicOperations<T> basicServer, IReadOnlyMappingManager mappingManager, IMappingValidator _schemaMappingValidator)
        {
            this.basicServer = basicServer;
            this.mappingManager = mappingManager;
            this._schemaMappingValidator = _schemaMappingValidator;
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options)
        {
            return basicServer.Query(query, options);
        }

        public ResponseHeader Ping()
        {
            return basicServer.Ping();
        }

        public SolrQueryResults<T> Query(string q)
        {
            return Query(new SolrQuery(q));
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders)
        {
            return Query(new SolrQuery(q), orders);
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(string q, QueryOptions options)
        {
            return basicServer.Query(new SolrQuery(q), options);
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(ISolrQuery q)
        {
            return Query(q, new QueryOptions());
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders)
        {
            return Query(query, new QueryOptions { OrderBy = orders });
        }

        /// <summary>
        /// Executes a facet field query only
        /// </summary>
        /// <param name="facet"></param>
        /// <returns></returns>
        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facet)
        {
            var r = basicServer.Query(SolrQuery.All, new QueryOptions
            {
                Rows = 0,
                Facet = new FacetParameters
                {
                    Queries = new[] { facet },
                },
            });
            return r.FacetFields[facet.Field];
        }

        public ResponseHeader BuildSpellCheckDictionary()
        {
            var r = basicServer.Query(SolrQuery.All, new QueryOptions
            {
                Rows = 0,
                SpellCheck = new SpellCheckingParameters { Build = true },
            });
            return r.Header;
        }

        public ResponseHeader AddWithBoost(T doc, double boost)
        {
            return AddWithBoost(doc, boost, null);
        }

        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters)
        {
            return ((ISolrOperations<T>)this).AddRangeWithBoost(new[] { new KeyValuePair<T, double?>(doc, boost) }, parameters);
        }

        public ExtractResponse Extract(ExtractParameters parameters)
        {
            return basicServer.Extract(parameters);
        }

        [Obsolete("Use AddRange instead")]
        public ResponseHeader Add(IEnumerable<T> docs)
        {
            return Add(docs, null);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs)
        {
            return AddRange(docs, null);
        }

        [Obsolete("Use AddRange instead")]
        public ResponseHeader Add(IEnumerable<T> docs, AddParameters parameters)
        {
            return basicServer.AddWithBoost(docs.Select(d => new KeyValuePair<T, double?>(d, null)), parameters);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters)
        {
            return basicServer.AddWithBoost(docs.Select(d => new KeyValuePair<T, double?>(d, null)), parameters);
        }

        [Obsolete("Use AddRangeWithBoost instead")]
        ResponseHeader ISolrOperations<T>.AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            return ((ISolrOperations<T>)this).AddWithBoost(docs, null);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            return ((ISolrOperations<T>)this).AddRangeWithBoost(docs, null);
        }

        [Obsolete("Use AddRangeWithBoost instead")]
        ResponseHeader ISolrOperations<T>.AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            return basicServer.AddWithBoost(docs, parameters);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            return basicServer.AddWithBoost(docs, parameters);
        }

        public ResponseHeader Delete(IEnumerable<string> ids)
        {
            return basicServer.Delete(ids, null, null);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters)
        {
            return basicServer.Delete(ids, null, parameters);
        }

        public ResponseHeader Delete(T doc)
        {
            return Delete(doc, null);
        }

        public ResponseHeader Delete(T doc, DeleteParameters parameters)
        {
            var id = GetId(doc);
            return Delete(id.ToString(), parameters);
        }

        public ResponseHeader Delete(IEnumerable<T> docs)
        {
            return Delete(docs, null);
        }

        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters)
        {
            return basicServer.Delete(docs.Select(d =>
            {
                var uniqueKey = mappingManager.GetUniqueKey(typeof(T));
                if (uniqueKey == null)
                    throw new SolrNetException(string.Format("This operation requires a unique key, but type '{0}' has no declared unique key", typeof(T)));
                return Convert.ToString(uniqueKey.Property.GetValue(d, null));
            }), null, parameters);
        }

        private object GetId(T doc)
        {
            var uniqueKey = mappingManager.GetUniqueKey(typeof(T));
            if (uniqueKey == null)
                throw new SolrNetException(string.Format("This operation requires a unique key, but type '{0}' has no declared unique key", typeof(T)));
            var prop = uniqueKey.Property;
            return prop.GetValue(doc, null);
        }

        ResponseHeader ISolrOperations<T>.Delete(ISolrQuery q)
        {
            return basicServer.Delete(null, q, null);
        }

        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters)
        {
            return basicServer.Delete(null, q, parameters);
        }

        public ResponseHeader Delete(string id)
        {
            return basicServer.Delete(new[] { id }, null, null);
        }

        public ResponseHeader Delete(string id, DeleteParameters parameters)
        {
            return basicServer.Delete(new[] { id }, null, parameters);
        }

        ResponseHeader ISolrOperations<T>.Delete(IEnumerable<string> ids, ISolrQuery q)
        {
            return basicServer.Delete(ids, q, null);
        }

        ResponseHeader ISolrOperations<T>.Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            return basicServer.Delete(ids, q, parameters);
        }

        public ResponseHeader Commit()
        {
            return basicServer.Commit(null);
        }

        /// <summary>
        /// Rollbacks all add/deletes made to the index since the last commit.
        /// </summary>
        /// <returns></returns>
        public ResponseHeader Rollback()
        {
            return basicServer.Rollback();
        }

        /// <summary>
        /// Commits posts, 
        /// blocking until index changes are flushed to disk and
        /// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
        /// </summary>
        public ResponseHeader Optimize()
        {
            return basicServer.Optimize(null);
        }

        public ResponseHeader Add(T doc)
        {
            return Add(doc, null);
        }

        public ResponseHeader Add(T doc, AddParameters parameters)
        {
            return AddRange(new[] { doc }, parameters);
        }

        public SolrSchema GetSchema()
        {
            return basicServer.GetSchema("schema.xml");
        }

        public SolrSchema GetSchema(string schemaFileName)
        {
            return basicServer.GetSchema(schemaFileName);
        }

        public IEnumerable<SolrNetCore.Mapping.Validation.ValidationResult> EnumerateValidationResults()
        {
            var schema = GetSchema();
            return _schemaMappingValidator.EnumerateValidationResults(typeof(T), schema);
        }

        /// <summary>
        /// Gets the DIH Status.
        /// </summary>
        /// <param name="options">command options</param>
        /// <returns>A XmlDocument containing the DIH Status XML.</returns>
        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options)
        {
            return basicServer.GetDIHStatus(options);
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            return basicServer.MoreLikeThis(query, options);
        }

        //public Task<SolrQueryResults<T>> QueryAsync(string q) => QueryAsync(new SolrQuery(q));

        //public Task<SolrQueryResults<T>> QueryAsync(string q, ICollection<SortOrder> orders) => QueryAsync(new SolrQuery(q), orders);
        //public Task<SolrQueryResults<T>> QueryAsync(string q, QueryOptions options) => QueryAsync(new SolrQuery(q), options);

        //public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery q) => QueryAsync(q, new QueryOptions());

        //public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, ICollection<SortOrder> orders) => QueryAsync(query, new QueryOptions() { OrderBy = orders });

        //public async Task<ICollection<KeyValuePair<string, int>>> FacetFieldQueryAsync(SolrFacetFieldQuery facet)
        //{
        //    var r = await basicServer.QueryAsync(SolrQuery.All, new QueryOptions
        //    {
        //        Rows = 0,
        //        Facet = new FacetParameters
        //        {
        //            Queries = new[] { facet },
        //        },
        //    });
        //    return r.FacetFields[facet.Field];
        //}

        //public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options) => basicServer.QueryAsync(query, options);

        //public Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options) => basicServer.MoreLikeThisAsync(query, options);

        //public Task<ResponseHeader> PingAsync() => basicServer.PingAsync();

        //public Task<SolrSchema> GetSchemaAsync(string schemaFileName) => basicServer.GetSchemaAsync(schemaFileName);

        //public Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options) => basicServer.GetDIHStatusAsync(options);

        //public Task<ResponseHeader> CommitAsync() => basicServer.CommitAsync(null);

        //public Task<ResponseHeader> RollbackAsync() => basicServer.RollbackAsync();

        //public Task<ResponseHeader> OptimizeAsync() => basicServer.OptimizeAsync(null);

        //public Task<ResponseHeader> AddAsync(T doc) => AddAsync(doc, null);

        //public Task<ResponseHeader> AddAsync(T doc, AddParameters parameters) => AddRangeAsync(new[] { doc }, parameters);

        //public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost) => AddWithBoostAsync(doc, boost, null);

        //public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost, AddParameters parameters) => AddRangeWithBoostAsync(new[] { new KeyValuePair<T, double?>(doc, boost) }, parameters);


        //public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters) => basicServer.ExtractAsync(parameters);

        //public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs) => AddRangeAsync(docs, null);

        //public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs, AddParameters parameters) => basicServer.AddWithBoostAsync(docs.Select(d => new KeyValuePair<T, double?>(d, null)), parameters);

        //public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs) => AddRangeWithBoostAsync(docs, null);

        //public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) => basicServer.AddWithBoostAsync(docs, parameters);

        //public Task<ResponseHeader> DeleteAsync(T doc) => DeleteAsync(doc, null);

        //public Task<ResponseHeader> DeleteAsync(T doc, DeleteParameters parameters) => DeleteAsync(GetId(doc).ToString(), parameters);


        //public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs) => DeleteAsync(docs, null);

        //public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs, DeleteParameters parameters) => basicServer.DeleteAsync(docs.Select(d => GetId(d).ToString()), null, parameters);

        //public Task<ResponseHeader> DeleteAsync(ISolrQuery q) => DeleteAsync(q, null);

        //public Task<ResponseHeader> DeleteAsync(ISolrQuery q, DeleteParameters parameters) => basicServer.DeleteAsync(null, q, parameters);

        //public Task<ResponseHeader> DeleteAsync(string id) => DeleteAsync(id, null);

        //public Task<ResponseHeader> DeleteAsync(string id, DeleteParameters parameters) => basicServer.DeleteAsync(new[] { id }, null, parameters);
        //public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids) => basicServer.DeleteAsync(ids, null, null);

        //public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, DeleteParameters parameters) => basicServer.DeleteAsync(ids, null, parameters);

        //public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q) => basicServer.DeleteAsync(ids, q, null);

        //public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters) => basicServer.DeleteAsync(ids, q, parameters);

        //public async Task<ResponseHeader> BuildSpellCheckDictionaryAsync()
        //{
        //    var r = await basicServer.QueryAsync(SolrQuery.All, new QueryOptions
        //    {
        //        Rows = 0,
        //        SpellCheck = new SpellCheckingParameters { Build = true },
        //    });
        //    return r.Header;
        //}

        //public async Task<IEnumerable<SolrNetCore.Mapping.Validation.ValidationResult>> EnumerateValidationResultsAsync()
        //{
        //    var schema = await basicServer.GetSchemaAsync("schema.xml");
        //    return _schemaMappingValidator.EnumerateValidationResults(typeof(T), schema);
        //}
    }
}
