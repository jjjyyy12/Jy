//using RabbitMQ.Client;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System.IO;
//using RawRabbit.Common;
//using RawRabbit.Attributes;
//using RabbitMQ.Client.Framing.Impl;
//using System;
//using System.Collections.Generic;

//namespace Jy.RabbitMQ
//{
//    /// <summary>
//    /// 构建链接，有单例的方法和非单例2种
//    /// </summary>
//    public class AutorecoveringConnectionBuilder
//    {
//        private static volatile AutorecoveringConnection _connection;

//        private static readonly object _locker = new object();
//        public static readonly NamingConventions Conventions = new NamingConventions();

//        public void Dispose()
//        {
//            _connection = null;
//        }

//        public static AutorecoveringConnection getClientRawRabbit(TimeSpan interval, IList<string> hostnames)
//        {
//            //  rawrabbit.readthedocs.io/en/master/configuration.html
//            if (_connection == null)
//            {
//                lock (_locker)
//                {
//                    if (_connection == null)
//                    {
//                        var cf = new ConnectionFactory();
//                        cf.AutomaticRecoveryEnabled = true;
//                        // tests that use this helper will likely list unreachable hosts,
//                        // make sure we time out quickly on those
//                        cf.RequestedConnectionTimeout = 10000;
//                        cf.NetworkRecoveryInterval = interval;
//                        return (AutorecoveringConnection)cf.CreateConnection(hostnames);
//                    }
//                }
//            }
//            return _connection;
//        }

//    }
//}
