
using Jy.ServicesKeep.Watch;
using Jy.Utility.Const;
using System;
namespace Jy.ServicesKeep
{
    public class KeepCallServer
    {
        private static string http = "http";
        private static string _AuthAPIAddress=""; //cache AuthAPIAddress
        private static readonly object _AuthAPIAddresslocker = new object();
        public static string getAuthAPIHostPort(string zooKeeperURL)
        {
            if (string.IsNullOrWhiteSpace(_AuthAPIAddress))
            {
                lock (_AuthAPIAddresslocker)
                {
                    if (string.IsNullOrWhiteSpace(_AuthAPIAddress))
                    {
                        _AuthAPIAddress = getAuthAPIAddress(zooKeeperURL);
                    }
                }
            }
            return _AuthAPIAddress;
        }
        public static void refreshAuthAPIHostPort(string zooKeeperURL)
        {
           _AuthAPIAddress = getAuthAPIAddress(zooKeeperURL);
        }
        private static string _TokenAuthAddress = ""; //cache TokenAuthAddress
        private static readonly object _TokenAuthAddresslocker = new object();
        public static string getTokenAuthHostPort(string zooKeeperURL)
        {
            if (string.IsNullOrWhiteSpace(_TokenAuthAddress))
            {
                lock (_TokenAuthAddresslocker)
                {
                    if (string.IsNullOrWhiteSpace(_TokenAuthAddress))
                    {
                        _TokenAuthAddress = getTokenAuthAddress(zooKeeperURL);
                    }
                }
            }
            return _TokenAuthAddress;
        }
        public static void refreshTokenAuthHostPort(string zooKeeperURL)
        {
            _TokenAuthAddress = getTokenAuthHostPort(zooKeeperURL);
        }
        public static string getAuthAPIAddress(string zooKeeperURL)
        {
            CallServer zk = CallAuthAPI.getCallServer(zooKeeperURL);
            AuthAPIWatcher watcher = CallAuthAPI.getWatcher(zooKeeperURL);
            string ipport = zk.getChild($"/{NodeName.AuthAdmin}", Guid.NewGuid().ToString(), watcher);
            if (string.IsNullOrWhiteSpace(ipport))
                return "";
            else
                return $"{http}://{ipport}";
        }
        public static string getTokenAuthAddress(string zooKeeperURL)
        {
            CallServer zk = CallTokenAuth.getCallServer(zooKeeperURL);
            TokenAuthWatcher watcher = CallTokenAuth.getWatcher(zooKeeperURL);
            string ipport = zk.getChild($"/{NodeName.TokenAuth}", Guid.NewGuid().ToString(), watcher);
            if (string.IsNullOrWhiteSpace(ipport))
                return "";
            else
                return $"{http}://{ipport}";
        }
    }
}
