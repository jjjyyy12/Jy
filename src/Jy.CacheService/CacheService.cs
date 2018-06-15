using Jy.ICache;


namespace Jy.CacheService
{
    public class CacheService: ICacheService
    {
        //key perfix
        private string key = "";
        public string Key { set { key = value; } get { return key; } }

        private ICached _cache;
        //缓存接口
        public ICached Cached { set { _cache = value; } get { return _cache; } }

        private IHttpCached _httpCache;
        public IHttpCached HttpCached { set { _httpCache = value; } get { return _httpCache; } }

        public CacheService(ICached cache, IHttpCached httpCache)
        {
            _cache = cache;
            _httpCache = httpCache;
        }
    }
}
