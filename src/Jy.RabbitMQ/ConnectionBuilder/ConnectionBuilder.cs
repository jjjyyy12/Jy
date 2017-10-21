using RabbitMQ.Client;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.Logging;
using System.IO;
using RawRabbit.Common;
using RawRabbit.Attributes;

namespace Jy.RabbitMQ
{
    /// <summary>
    /// 构建链接，有单例的方法和非单例2种
    /// </summary>
    public class ConnectionBuilder
    {
        private static volatile IBusClient<MessageContext> _busClient;
        private static volatile RawRabbit.vNext.Disposable.IBusClient<MessageContext> _extbusClient;

        private static readonly object _locker = new object();
        public static readonly NamingConventions Conventions = new NamingConventions();

        public void Dispose()
        {
            _busClient = null;
            _extbusClient = null;
        }

        public static IBusClient<MessageContext> getClientRawRabbit()
        {
            //  rawrabbit.readthedocs.io/en/master/configuration.html
            if (_busClient == null)
            {
                lock (_locker)
                {
                    if (_busClient == null)
                    {
                        _busClient = BusClientFactory.CreateDefault<MessageContext>(cfg => cfg.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("rawrabbit.json", optional: true, reloadOnChange: true),
                                ioc => ioc.AddSingleton<ILoggerFactory, LoggerFactory>().AddSingleton<IConfigurationEvaluator, AttributeConfigEvaluator>().AddSingleton(c => Conventions));
                    }
                }
            }
            return _busClient;
        }

        public static IBusClient<MessageContext> getExtendableClient()
        {
            if (_extbusClient == null)
            {
                lock (_locker)
                {
                    if (_extbusClient == null)
                    {
                        _extbusClient = RawRabbit.vNext.BusClientFactory.CreateDefault<MessageContext>(cfg =>
                        {
                            cfg.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("rawrabbit.json", optional: true, reloadOnChange: true);
                        },
                        conf =>
                        {
                            conf.AddSingleton<ILoggerFactory, LoggerFactory>().AddSingleton<IConfigurationEvaluator, AttributeConfigEvaluator>().AddSingleton(c => Conventions);
                        });
                    }
                }
            }
            return _extbusClient;
        }

        public static IBusClient<MessageContext> getClientRawRabbitNoSignal()
        {
            return BusClientFactory.CreateDefault<MessageContext>(cfg => cfg.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("rawrabbit.json", optional: true, reloadOnChange: true),
                           ioc => ioc.AddSingleton<ILoggerFactory, LoggerFactory>().AddSingleton<IConfigurationEvaluator, AttributeConfigEvaluator>().AddSingleton(c => Conventions));
        }

        public static RawRabbit.vNext.Disposable.IBusClient<MessageContext> getExtendableClientNoSignal()
        {
            return RawRabbit.vNext.BusClientFactory.CreateDefault<MessageContext>(cfg =>
            {
                cfg.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("rawrabbit.json", optional: true, reloadOnChange: true);
            },
            conf =>
            {
                conf.AddSingleton<ILoggerFactory, LoggerFactory>().AddSingleton<IConfigurationEvaluator, AttributeConfigEvaluator>()
                .AddSingleton(c => Conventions);
            });
        }
    }
}
