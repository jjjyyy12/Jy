using org.apache.zookeeper;
using System.Threading.Tasks;

namespace Jy.ServicesKeep
{
    public interface IRegister
    {
        Task<string> createAsync(string node, string name, string port, Watcher watcher);
        string removeAsync(string node, Watcher watcher);
    }
}
