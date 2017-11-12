
using System.Collections.Generic;

namespace Jy.IIndex
{
    /// <summary>
    /// 根据id，取到分区表库的dbIndex
    /// </summary>
    public interface IIndexFactory
    {
        HashSet<string> GetConnectionStrings();
        TH CreateIndex<T, TH>(string Id)
            where TH : IIndex<T>
            where T : Entity;

        TH CreateDefaultIndex<T, TH>()
            where TH : IIndex<T>
            where T : Entity;

        List<TH> CreateAllIndex<T, TH>()
            where T : Entity
            where TH : IIndex<T>;

        TH CreateIndexByConnStr<T, TH>(string ConnStr)
            where T : Entity
            where TH : IIndex<T>;
    }
}
