
using Jy.ServicesKeep.Watch;
using Jy.Utility.Const;
using System;
namespace Jy.ServicesKeep
{
    public class KeepCallServer
    {
        private static volatile CallServer _zkAuthAPI;

        private static readonly object _zkAuthAPIlocker = new object();

        public static string AuthAPIAddress=""; //cache AuthAPIAddress
        private static readonly object _AuthAPIAddresslocker = new object();
        public static string getAuthAPIHostPort(string zooKeeperURL)
        {
            if (string.IsNullOrWhiteSpace(AuthAPIAddress))
            {
                lock (_AuthAPIAddresslocker)
                {
                    if (string.IsNullOrWhiteSpace(AuthAPIAddress))
                    {
                        AuthAPIAddress = getAuthAPIAddress(zooKeeperURL);
                    }
                }
            }
            return AuthAPIAddress;
        }
         
        public static string TokenAuthAddress = ""; //cache TokenAuthAddress
        private static readonly object _TokenAuthAddresslocker = new object();
        public static string getTokenAuthHostPort(string zooKeeperURL)
        {
            if (string.IsNullOrWhiteSpace(TokenAuthAddress))
            {
                lock (_TokenAuthAddresslocker)
                {
                    if (string.IsNullOrWhiteSpace(TokenAuthAddress))
                    {
                        TokenAuthAddress = getTokenAuthAddress(zooKeeperURL);
                    }
                }
            }
            return TokenAuthAddress;
        }

        public static string getAuthAPIAddress(string zooKeeperURL)
        {
            CallServer zk = CallAuthAPI.getCallServer(zooKeeperURL);
            AuthAPIWatcher watcher = CallAuthAPI.getWatcher(zooKeeperURL);
            string ipport = zk.getChild($"/{NodeName.AuthAdmin}", Guid.NewGuid().ToString(), watcher);
            if (string.IsNullOrWhiteSpace(ipport))
                return "";
            else
                return $"http://{ipport}";
        }
        public static string getTokenAuthAddress(string zooKeeperURL)
        {
            CallServer zk = CallTokenAuth.getCallServer(zooKeeperURL);
            TokenAuthWatcher watcher = CallTokenAuth.getWatcher(zooKeeperURL);
            string ipport = zk.getChild($"/{NodeName.TokenAuth}", Guid.NewGuid().ToString(), watcher);
            if (string.IsNullOrWhiteSpace(ipport))
                return "";
            else
                return $"http://{ipport}";
        }
    }
}
