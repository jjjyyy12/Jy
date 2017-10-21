
using Jy.ServicesKeep.Watch;
using Jy.Utility.Const;
using Jy.Utility.IP;
using System;
using System.Net;
using System.Net.Sockets;

namespace Jy.ServicesKeep
{
    public class TokenAuthRegister
    {
        private static volatile Register _tokenAuthzk;
        private static readonly object _locker = new object();
        private static volatile TokenAuthRegisterWatcher _tokenAuthwatcher;
        private static readonly object _watcherlocker = new object();
        public void Dispose()
        {
            _tokenAuthzk = null;
            _tokenAuthwatcher = null;
        }
        public static Register getRegister(string zooKeeperURL)
        {
            if (_tokenAuthzk == null)
            {
                lock (_locker)
                {
                    if (_tokenAuthzk == null)
                    {
                        _tokenAuthzk = new Register(zooKeeperURL);
                    }
                }
            }
            return _tokenAuthzk;
        }
        public static TokenAuthRegisterWatcher getWatcher(string zooKeeperURL)
        {
            if (_tokenAuthwatcher == null)
            {
                lock (_watcherlocker)
                {
                    if (_tokenAuthwatcher == null)
                    {
                        _tokenAuthwatcher = new TokenAuthRegisterWatcher(zooKeeperURL);
                    }
                }
            }
            return _tokenAuthwatcher;
        }
        public static string registerTokenAuthHostPort(string zooKeeperURL)
        {
            string name = IPUtil.GetLocalIPDebug();//GetLocalIP()
            Register zk = getRegister(zooKeeperURL);
            string res = zk.createAsync($"/{NodeName.TokenAuth}", name, NodeName.TokenAuthPort, getWatcher(zooKeeperURL)).Result;//getWatcher(zooKeeperURL)
            return res;
        }
      
    }
}
