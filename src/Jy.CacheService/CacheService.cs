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

        public CacheService(ICached cache)
        {
            _cache = cache;
        }
    }
}
