using Jy.IIndex;
namespace Jy.IndexService
{
    public interface IIndexService
    {
        IIndexRead indexRead { get; set; }
        Jy.IIndex.IIndex index { get; set; }
    }
}
