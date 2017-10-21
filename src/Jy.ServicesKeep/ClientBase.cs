using Jy.Utility.Const;
using org.apache.zookeeper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.ServicesKeep
{
    public abstract class ClientBase : IDisposable
    {
        protected static class Arrays
        {
            public static List<T> asList<T>(params T[] objs)
            {
                return new List<T>(objs);
            }
        }

        public const int CONNECTION_TIMEOUT = 400000;
        private readonly string m_currentRoot;

        private readonly string hostPort;

        private readonly ConcurrentBag<ZooKeeper> allClients = new ConcurrentBag<ZooKeeper>();


        protected ZooKeeper createClient(string chroot = null, int timeout = CONNECTION_TIMEOUT)
        {
            return createClient(NullWatcher.Instance, chroot, timeout);
        }

        protected ZooKeeper createClient(Watcher watcher, string chroot = null, int timeout = CONNECTION_TIMEOUT)
        {
            if (watcher == null) watcher = NullWatcher.Instance;
            var zk = new ZooKeeper(hostPort + m_currentRoot + chroot, timeout, watcher);
            allClients.Add(zk);
            return zk;
        }
        protected ZooKeeper createClientForCall(string chroot = null, int timeout = CONNECTION_TIMEOUT)
        {
            return createClientForCall(NullWatcher.Instance, chroot, timeout);
        }
        protected ZooKeeper createClientForCall(Watcher watcher, string chroot = null, int timeout = CONNECTION_TIMEOUT)
        {
            if (watcher == null) watcher = NullWatcher.Instance;
            return new ZooKeeper(hostPort + m_currentRoot + chroot, timeout, watcher);
        }

        protected ClientBase(string hostPort)
        {
            this.hostPort = hostPort;
            m_currentRoot = "/";//createNode().Result;
        }


        public void Dispose()
        {
            deleteNode(m_currentRoot).Wait();
            Task.WaitAll(allClients.Select(c => c.closeAsync()).ToArray());
        }

        public Task deleteNode(string path)
        {
            return ZooKeeper.Using(hostPort, CONNECTION_TIMEOUT, NullWatcher.Instance, zk =>
            {
                return ZKUtil.deleteRecursiveAsync(zk, path);
            });
        }

        private Task<string> createNode()
        {
            return ZooKeeper.Using(hostPort, CONNECTION_TIMEOUT, NullWatcher.Instance, async zk =>
            {
                string newNode = await zk.createAsync("/", null, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT_SEQUENTIAL);
                return newNode;
            });
        }

        /// <summary>
        ///     In general don't use this. Only use in the special case that you
        ///     want to ignore results (for whatever reason) in your test. Don't
        ///     use empty watchers in real code!
        /// </summary>
        public class NullWatcher : Watcher
        {
            public static readonly NullWatcher Instance = new NullWatcher();
            private NullWatcher() { }
            public override Task process(WatchedEvent @event)
            {
                return default(Task);
                // nada
            }
        }
        
    }
}
