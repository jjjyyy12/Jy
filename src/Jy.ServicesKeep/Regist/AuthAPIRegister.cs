
using Jy.ServicesKeep.Watch;
using Jy.Utility.Const;
using Jy.Utility.IP;
using System;
using System.Net;
using System.Net.Sockets;

namespace Jy.ServicesKeep
{
    public class AuthAPIRegister
    {
        private static volatile Register _authAPIzk;
        private static readonly object _locker = new object();

        private static volatile AuthAPIRegisterWatcher _authAPIwatcher;
        private static readonly object _watcherlocker = new object();
        public void Dispose()
        {
            _authAPIzk = null;
            _authAPIwatcher = null;
        }
        public static Register getRegister(string zooKeeperURL)
        {
            if (_authAPIzk == null)
            {
                lock (_locker)
                {
                    if (_authAPIzk == null)
                    {
                        _authAPIzk = new Register(zooKeeperURL);
                    }
                }
            }
            return _authAPIzk;
        }
        public static AuthAPIRegisterWatcher getWatcher(string zooKeeperURL)
        {
            if (_authAPIwatcher == null)
            {
                lock (_watcherlocker)
                {
                    if (_authAPIwatcher == null)
                    {
                        _authAPIwatcher = new AuthAPIRegisterWatcher(zooKeeperURL);
                    }
                }
            }
            return _authAPIwatcher;
        }
        public static string registerAuthAPIHostPort(string zooKeeperURL,string ip = "")
        {
            if (string.IsNullOrWhiteSpace(ip))
                ip = IPUtil.GetLocalIP();
            if (string.IsNullOrWhiteSpace(ip))
                ip = IPUtil.GetLocalIPDebug();
            Register zk = getRegister(zooKeeperURL);
            return zk.createAsync($"/{NodeName.AuthAdmin}", ip, NodeName.AuthAdminPort, getWatcher(zooKeeperURL)).Result; //getWatcher(zooKeeperURL)
        }
      
    }
}
