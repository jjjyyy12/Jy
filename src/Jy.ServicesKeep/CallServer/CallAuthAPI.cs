
using Jy.ServicesKeep.Watch;
using Jy.Utility.Const;
using Jy.Utility.IP;
using System;
using System.Net;
using System.Net.Sockets;

namespace Jy.ServicesKeep
{
    public class CallAuthAPI
    {
        private static volatile CallServer _authAPIzk;
        private static readonly object _locker = new object();

        private static volatile AuthAPIWatcher _authAPIwatcher;
        private static readonly object _watcherlocker = new object();
        public void Dispose()
        {
            _authAPIzk = null;
            _authAPIwatcher = null;
        }
        public static CallServer getCallServer(string zooKeeperURL)
        {
            if (_authAPIzk == null)
            {
                lock (_locker)
                {
                    if (_authAPIzk == null)
                    {
                        _authAPIzk = new CallServer(zooKeeperURL);
                    }
                }
            }
            return _authAPIzk;
        }
        public static AuthAPIWatcher getWatcher(string zooKeeperURL)
        {
            if (_authAPIwatcher == null)
            {
                lock (_watcherlocker)
                {
                    if (_authAPIwatcher == null)
                    {
                        _authAPIwatcher = new AuthAPIWatcher(zooKeeperURL);
                    }
                }
            }
            return _authAPIwatcher;
        }
      
    }
}
