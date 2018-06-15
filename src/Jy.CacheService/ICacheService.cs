using Jy.ICache;

namespace Jy.CacheService
{
    public interface ICacheService
    {
        ICached Cached { get; set; }
        IHttpCached HttpCached { get; set; }
        string Key { set; get; }

    }
}
