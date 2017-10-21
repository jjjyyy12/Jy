
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Jy.Kafka
{
    public class ConnectionBuilder  
    {
        private static volatile string _borkerList;
        private static readonly object _locker = new object();
        public IConfigurationRoot Configuration { get;}
        public static string getBorkerList()
        {
            if (_borkerList == null)
            {
                lock (_locker)
                {
                    if (_borkerList == null)
                    {
                        var confbuilder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                           .AddEnvironmentVariables();
                        _borkerList = confbuilder.Build().GetSection("KafkaConfig").GetValue<string>("BorkerList");
                    }
                }
            }
            return _borkerList;
        }
        public void Dispose()
        {
            _borkerList = null;
        }
    }
}
