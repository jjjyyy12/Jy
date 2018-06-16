using Jy.Cache.HashAlgorithms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Jy.Cache
{
    /// <summary>
    /// redis数据上下文
    /// </summary>
    public class RedisContext
    {
        private readonly IHashAlgorithm _hashAlgorithm;
        /// <summary>
        /// 缓存对象集合容器池
        /// </summary>
        internal Lazy<Dictionary<string, List<string>>> _cachingContextPool;

        /// <summary>
        /// 密码
        /// </summary>
        internal string _password = null;

        internal string _bucket = null;
        /// <summary>
        /// 默认缓存失效时间
        /// </summary>
        internal string _defaultExpireTime = null;

        /// <summary>
        /// 连接失效时间
        /// </summary>
        internal string _connectTimeout = null;

 

        /// <summary>
        /// 哈希节点容器
        /// </summary>
        internal ConcurrentDictionary<string, ConsistentHash<ConsistentHashNode>> dicHash;

        /// <summary>
        /// 对象池上限
        /// </summary>
        internal string _maxSize = null;

        /// <summary>
        /// 对象池下限
        /// </summary>
        internal string _minSize = null;

        #region 构造函数
        /// <summary>
        /// redis数据上下文
        /// </summary>
        /// <param name="rule">规则</param>
        /// <param name="args">参数</param>
        public RedisContext(IHashAlgorithm  hashAlgorithm, RedisCacheOptions options)
        {
             _hashAlgorithm = hashAlgorithm;
            if(!string.IsNullOrWhiteSpace(options.Configuration))
            {
                string[] connectionString = options.Configuration.Split(',');
                HashSet<string> connSet = new HashSet<string>();
                for (int i = 0, j = connectionString.Length; i < j; i++)
                {
                    var connStr = connectionString[i];
                    if (connSet.Contains(connStr) )
                        continue;
                    connSet.Add(connStr);
                     
                    var hash =
                        new ConsistentHash<HashAlgorithms.ConsistentHashNode>(_hashAlgorithm);

                        var endpoints = connStr.Split(':');
 
                        hash.Add(new ConsistentHashNode()
                        {
                            Type = CacheTargetType.Redis,
                            Host = endpoints[0],
                            Port = endpoints[1],
                            UserName = options.InstanceName,
                            Password = options.Pwd,
                            MaxSize = this._maxSize,
                            MinSize = this._minSize,
                            Db = i.ToString()
                        });
                        dicHash.GetOrAdd(CacheTargetType.Redis.ToString(), hash);
 
                }
            }
            dicHash = new ConcurrentDictionary<string, ConsistentHash<ConsistentHashNode>>();
        }
        #endregion

        #region 属性

        public string ConnectTimeout
        {
            get
            {
                return _connectTimeout;
            }
        }

        public string DefaultExpireTime
        {
            get
            {
                return _defaultExpireTime;
            }
        }
        /// <summary>
        /// 缓存对象集合容器池
        /// </summary>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public Dictionary<string, List<string>> DataContextPool
        {
            get { return _cachingContextPool.Value; }
        }
        #endregion

 

         
    }
}
