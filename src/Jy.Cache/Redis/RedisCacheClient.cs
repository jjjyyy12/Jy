using Jy.Cache.HashAlgorithms;
using Jy.Cache.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jy.Cache
{ 
    public class RedisCacheClient<T> : ICacheClient<T>
        where T : class

    {
        private static readonly ConcurrentDictionary<string, ObjectPool<T>> _pool =
            new ConcurrentDictionary<string, ObjectPool<T>>();

        public RedisCacheClient()
        {

        }

        public async Task<bool> ConnectionAsync(CacheEndpoint endpoint, int connectTimeout)
        {
            try
            {
                var info = endpoint as ConsistentHashNode;
                var point = string.Format("{0}:{1}", info.Host, info.Port);
                var conn = await ConnectionMultiplexer.ConnectAsync(new ConfigurationOptions()
                {
                    EndPoints = { { point } },
                    ServiceName = point,
                    Password = info.Password,
                    ConnectTimeout = connectTimeout
                });
                return conn.IsConnected;
            }
            catch (Exception e)
            {
                throw new CacheException(e.Message); 
            }
        }

        public T GetClient(CacheEndpoint endpoint, int connectTimeout)
        {
            try
            {
                var info = endpoint as RedisEndpoint;
                Check.NotNull(info, "endpoint");
                var key = string.Format("{0}{1}{2}{3}", info.Host, info.Port, info.Password, info.DbIndex);
                if (!_pool.ContainsKey(key))
                {
                    var objectPool = new ObjectPool<T>(() =>
                    {
                        var point = string.Format("{0}:{1}", info.Host, info.Port);
                        var redisClient = ConnectionMultiplexer.Connect(new ConfigurationOptions()
                        {
                            EndPoints = { { point } },
                            ServiceName = point,
                            Password = info.Password,
                            ConnectTimeout = connectTimeout
                        });
                        return redisClient.GetDatabase(info.DbIndex) as T;
                    }, info.MinSize, info.MaxSize);
                    _pool.GetOrAdd(key, objectPool);
                    return objectPool.GetObject();
                }
                else
                {
                    return _pool[key].GetObject();
                }
            }
            catch (Exception e)
            {
                throw new CacheException(e.Message);
            }
        }
    }
}
