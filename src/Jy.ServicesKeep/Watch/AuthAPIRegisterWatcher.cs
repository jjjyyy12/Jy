﻿using Jy.Utility;
using org.apache.zookeeper;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Jy.ServicesKeep.Watch
{
    public class AuthAPIRegisterWatcher : Watcher
    {
        //internal readonly BlockingCollection<WatchedEvent> events =
        //new BlockingCollection<WatchedEvent>();

        private readonly string _zooKeeperURL;
        public AuthAPIRegisterWatcher(string zooKeeperURL)
        {
            _zooKeeperURL = zooKeeperURL;
        }
        public override Task process(WatchedEvent @event)
        {
            var state = @event.getState();
            if (Event.KeeperState.Disconnected == state)
            {
                AuthAPIRegister.registerAuthAPIHostPort(_zooKeeperURL);
            }
            var type = @event.get_Type();
            if (type != Event.EventType.None)
            {
                //events.Add(@event);
            }
            
            return default(Task);
        }
    }
}
