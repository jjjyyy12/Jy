
namespace Jy.IIndex
{
    /// <summary>
    /// 根据id，取到分区表库的dbrepositoryread
    /// </summary>
    public interface IIndexReadFactory
    {
        TH CreateIndex<T, TH>(string Id, string coreIndex)
            where TH : IIndexRead<T>
            where T : Entity;
    }
}
