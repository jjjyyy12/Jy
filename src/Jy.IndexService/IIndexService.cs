using Jy.IIndex;
namespace Jy.IndexService
{
    public interface IIndexService
    {
         IIndexReadFactory IndexReadFactory { set ;  get; }
         IIndexFactory IndexFactory { set; get; }
    }
}
