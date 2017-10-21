using Jy.ICache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace Jy.Cache
{
    public class RedisCacheRepository: ICached
    {
        protected IDatabase _cache;

        private ConnectionMultiplexer _connection;

        private readonly string _instance;
        public TimeSpan _expTime { get; set; } = new TimeSpan(0, 10, 0);

        public RedisCacheRepository(RedisCacheOptions options, int database = 0)
        {
            _connection = ConnectionMultiplexer.Connect(options.Configuration);
            _cache = _connection.GetDatabase(database);
            _instance = options.InstanceName;
            _expTime = options.expTime;
        }

        public string GetKeyForRedis(string key)
        {
            return _instance + key;
        }
        private IServer GetServer(EndPoint point)
        {
            return _connection.GetServer(point);
        }
        /// <summary>
        /// getallkeys by pattern
        /// </summary>
        /// <param name="keypattern">"*key*"</param>
        /// <returns></returns>
        public List<string> GetKeys(string keypattern)
        {
            var endpoint = _connection.GetEndPoints();
            List<string> rlist = new List<string>();
            IEnumerable<RedisKey> data = null;
            foreach (var point in endpoint)
            {
                var server = GetServer(point);
                data = server.Keys(pattern: keypattern);
                if(data!=null)
                    rlist.AddRange(data.Select(k => k.ToString()));
            }
            return rlist;
        }

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.KeyExists(GetKeyForRedis(key));
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public bool Add(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.StringSet(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间,Redis中无效）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public bool Add(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (expiressAbsoulte == default(TimeSpan))
                expiressAbsoulte = this._expTime;
            return _cache.StringSet(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiressAbsoulte);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间,Redis中无效）</param>
        /// <returns></returns>
        public bool Add(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (expiresIn == default(TimeSpan))
                expiresIn = this._expTime;
            return _cache.StringSet(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiresIn);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.KeyDelete(GetKeyForRedis(key));
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            keys.ToList().ForEach(item => Remove(GetKeyForRedis(item)));
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = _cache.StringGet(GetKeyForRedis(key));

            if (!value.HasValue)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(value);
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = _cache.StringGet(GetKeyForRedis(key));

            if (!value.HasValue)
            {
                return null;
            }
            return JsonConvert.DeserializeObject(value);
        }
        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            var dict = new Dictionary<string, object>();

            keys.ToList().ForEach(item => dict.Add(item, Get(GetKeyForRedis(item))));

            return dict;
        }

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <returns></returns>
        public bool Replace(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (Exists(key))
                if (!Remove(key))
                    return false;

            return Add(key, value);

        }
        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public bool Replace(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (Exists(key))
                if (!Remove(key))
                    return false;

            return Add(key, value, expiresSliding, expiressAbsoulte);
        }
        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        public bool Replace(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (Exists(key))
                if (!Remove(key)) return false;

            return Add(key, value, expiresIn, isSliding);
        }
        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task<bool> ExistsAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddAsync(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.StringSetAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }

        public Task<bool> AddAsync(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (expiressAbsoulte == default(TimeSpan))
                expiressAbsoulte = this._expTime;
            return _cache.StringSetAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)),expiressAbsoulte);

        }

        public Task<bool> AddAsync(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.StringSetAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiresIn);

        }

        public Task<bool> RemoveAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.KeyDeleteAsync(GetKeyForRedis(key));
        }

        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            if (keys == null || keys.Count()==0)
            {
                throw new ArgumentNullException(nameof(keys));
            }
 
            RedisKey[] inKeys = new RedisKey[keys.Count()];
            int i = 0;
            foreach(var it in keys)
            {
                inKeys[i] = GetKeyForRedis(it);
                i++;
            }
            return _cache.KeyDeleteAsync(inKeys);
        }

        public Task<T> GetAsync<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }
        public async Task<object> GetAsync2(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            RedisValue value = await _cache.StringGetAsync(GetKeyForRedis(key));
            return value;
        }
        public T Get<T>(Func<T> handler, string key, TimeSpan expiressAbsoulte = default(TimeSpan))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (expiressAbsoulte == default(TimeSpan))
                expiressAbsoulte = this._expTime;
            T obj;
            var value = _cache.StringGet(GetKeyForRedis(key));

            if (!value.HasValue)
            {
                obj = handler();
                if (obj != null)
                    Add(key, obj, expiressAbsoulte);
                return obj;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

        }
        //-------------------------------zset 分页数据
        public List<T> GetSortList<T>(Func<List<T>> handler, string key, TimeSpan expiresIn = default(TimeSpan))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
           
            var value = _cache.SortedSetRangeByRank(GetKeyForRedis(key));
            if (value?.Length==0)
            {
                List<T> obj = handler();
                if (obj != null)
                {
                    SortedSetEntry[] vals = new SortedSetEntry[obj.Count];
                    for (int i = 0, j = obj.Count; i < j; i++)
                    {
                        vals[i] = new SortedSetEntry(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj[i])),i);
                    }
                    _cache.SortedSetAdd(GetKeyForRedis(key), vals);
                    if (expiresIn == default(TimeSpan))
                        expiresIn = this._expTime;
                    _cache.KeyExpire(GetKeyForRedis(key), expiresIn);
                }
                return obj;
            }
            else
            {
                List<T> rlist = new List<T>();
                foreach(var item in value)
                {
                    rlist.Add(JsonConvert.DeserializeObject<T>(item));
                }
                return rlist;
            }
        }
        public long SetSortList<T>(string key, List<T> rlist)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
 
            SortedSetEntry[] vals = new SortedSetEntry[rlist.Count];
            for (int i = 0, j = rlist.Count; i < j; i++)
            {
                 vals[i] = new SortedSetEntry(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rlist[i])), i);
            }
            return _cache.SortedSetAdd(GetKeyForRedis(key), vals);
        }
        public bool SetSortSingal(string key, object value, double score)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.SortedSetAdd(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), score);
        }
        public List<T> SortedSetRangeByRank<T>(string key,long start=0,long end = -1)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var zlist = _cache.SortedSetRangeByRank(GetKeyForRedis(key), start,end);
            List<T> rlist = new List<T>();
            for (int i = 0, j = zlist.Length; i < j; i++)
            {
                rlist.Add(JsonConvert.DeserializeObject<T>(zlist[i]));
            }
            return rlist;
        }
        public bool SortedSetUpdate<T>(string key,T inobj, Func<T, bool> findHandle,bool delFlag=false)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (inobj == null)
                throw new ArgumentNullException(nameof(inobj));

            string okey = GetKeyForRedis(key);
            var zlist = _cache.SortedSetRangeByRankWithScores(okey);
            bool isNew = true;
            for (int i = 0, j = zlist.Length; i < j; i++)
            {
                if (findHandle(JsonConvert.DeserializeObject<T>(zlist[i].Element)))
                {
                    isNew = false;
                    string sobj = JsonConvert.SerializeObject(inobj);
                    double score = zlist[i].Score;
                    _cache.SortedSetRemove(okey, zlist[i].Element);
                    if(!delFlag)//delete
                        _cache.SortedSetAdd(okey, Encoding.UTF8.GetBytes(sobj), score);//update
                }
            }
            if (isNew)//insert
            {
                string sobj = JsonConvert.SerializeObject(inobj);
                _cache.SortedSetAdd(okey, Encoding.UTF8.GetBytes(sobj), 1);
            }
            return true;
        }
        //------------------------------------------------zset
        public Task<object> GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<string, object>> GetAllAsync(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceAsync(string key, object value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceAsync(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceAsync(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            throw new NotImplementedException();
        }
        public void SetAdd(string key,object[] values)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            RedisValue[] vals = new RedisValue[values.Length];
            for(int i = 0, j = values.Length; i < j; i++)
            {
                vals[i] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(values[i]));
            }
            _cache.SetAdd(GetKeyForRedis(key), vals);
        }
        public void SetAdd(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _cache.SetAdd(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }
        public List<T> SetMembers<T>(string key) where T : class
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = _cache.SetMembers(GetKeyForRedis(key));

            if (value==null||value.Length<=0)
            {
                return null;
            }
            List<T> rlist = new List<T>(value.Length);
            for(int i=0,j=value.Length;i<j;i++)
            {
                rlist.Add(JsonConvert.DeserializeObject<T>(value[i]));
            }
            return rlist;
        }
        public bool SetContains(string key,object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.SetContains(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }
        public void HashSet(string key, object field,object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _cache.HashSet(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(field)), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }
        public  T HashGet<T>(string key,object field) where T : class
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = _cache.HashGet(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(field)));

            if (!value.HasValue)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(value);
        }
        public void KeyExpire(string key, TimeSpan expiresIn)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _cache.KeyExpire(GetKeyForRedis(key), expiresIn);
        }

    }
}
