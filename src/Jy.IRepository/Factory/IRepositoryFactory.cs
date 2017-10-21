
using System.Collections.Generic;

namespace Jy.IRepositories
{
    /// <summary>
    /// 根据id，取到分区表库的dbrepository
    /// </summary>
    public interface IRepositoryFactory
    {
        HashSet<string> GetConnectionStrings();
        TH CreateRepository<T, TH>(string Id)
            where TH : IRepository<T>
            where T : Entity;

        TH CreateDefaultRepository<T, TH>()
            where TH : IRepository<T>
            where T : Entity;

        List<TH> CreateAllRepository<T, TH>()
            where T : Entity
            where TH : IRepository<T>;

        TH CreateRepositoryByConnStr<T, TH>(string ConnStr)
            where T : Entity
            where TH : IRepository<T>;
    }
}
