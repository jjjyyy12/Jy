
using Jy.ServicesKeep.Watch;
using Jy.Utility.Const;
using Jy.Utility.IP;
using System;
using System.Net;
using System.Net.Sockets;

namespace Jy.ServicesKeep
{
    public class CRMAPIRegister
    {
        private static volatile Register _crmAPIzk;
        private static readonly object _locker = new object();
        public void Dispose()
        {
            _crmAPIzk = null;
        }
        public static Register getRegister(string zooKeeperURL)
        {
            if (_crmAPIzk == null)
            {
                lock (_locker)
                {
                    if (_crmAPIzk == null)
                    {
                        _crmAPIzk = new Register(zooKeeperURL);
                    }
                }
            }
            return _crmAPIzk;
        }

        public static string registerCRMAPIHostPort(string zooKeeperURL,string ip = "")
        {
            if (string.IsNullOrWhiteSpace(ip))
                ip = IPUtil.GetLocalIP();
            if (string.IsNullOrWhiteSpace(ip))
                ip = IPUtil.GetLocalIPDebug();
            Register zk = new Register(zooKeeperURL);
            return zk.createAsync($"/{NodeName.CRM}", ip, NodeName.CRMPort,new CRMAPIWatcher(zooKeeperURL)).Result;
        }
      
    }
}
