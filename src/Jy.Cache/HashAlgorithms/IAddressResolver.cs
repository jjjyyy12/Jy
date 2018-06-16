using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jy.Cache.HashAlgorithms
{
    public interface IAddressResolver
    {
        ValueTask<ConsistentHashNode> Resolver(string cacheId, string item);
    }
}
