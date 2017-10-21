using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jy.ServicesKeep
{
    public interface ICallServer
    {
        string GetServer(string node, Watcher watcher);
        string getChild(string node, string id, Watcher watcher);
    }
}
