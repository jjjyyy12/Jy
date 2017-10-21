
using Jy.ServicesKeep.Watch;
using Jy.Utility.Const;
using Jy.Utility.IP;
using System;
using System.Net;
using System.Net.Sockets;

namespace Jy.ServicesKeep
{
    public class CallTokenAuth
    {
        private static volatile CallServer _zk;
        private static readonly object _locker = new object();

        private static volatile TokenAuthWatcher _watcher;
        private static readonly object _watcherlocker = new object();
        public void Dispose()
        {
            _zk = null;
            _watcher = null;
        }
        public static CallServer getCallServer(string zooKeeperURL)
        {
            if (_zk == null)
            {
                lock (_locker)
                {
                    if (_zk == null)
                    {
                        _zk = new CallServer(zooKeeperURL);
                    }
                }
            }
            return _zk;
        }
        public static TokenAuthWatcher getWatcher(string zooKeeperURL)
        {
            if (_watcher == null)
            {
                lock (_watcherlocker)
                {
                    if (_watcher == null)
                    {
                        _watcher = new TokenAuthWatcher(zooKeeperURL);
                    }
                }
            }
            return _watcher;
        }
      
    }
}
