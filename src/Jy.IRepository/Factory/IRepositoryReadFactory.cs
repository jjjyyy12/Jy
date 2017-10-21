
namespace Jy.IRepositories
{
    /// <summary>
    /// 根据id，取到分区表库的dbrepositoryread
    /// </summary>
    public interface IRepositoryReadFactory
    {
        TH CreateRepository<T, TH>(string Id)
            where TH : IRepositoryRead<T>
            where T : Entity;
    }
}
