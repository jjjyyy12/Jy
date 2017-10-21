using Jy.ICache;

namespace Jy.CacheService
{
    public interface ICacheService
    {
        ICached Cached { get; set; }
        string Key { set; get; }

    }
}
