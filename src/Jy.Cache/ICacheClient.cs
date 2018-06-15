using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jy.Cache
{
    public interface ICacheClient<T>
    {
        T GetClient(CacheEndpoint info, int connectTimeout);

        Task<bool> ConnectionAsync(CacheEndpoint endpoint, int connectTimeout);

    }
}
