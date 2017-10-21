using RawRabbit.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.RabbitMQ
{
    public class LoggerFactory : ILoggerFactory
    {
        ILogger _logger;
        public ILogger CreateLogger(string categoryName)
        {
            if(_logger==null)
                _logger=new Logger();
            return _logger;
        }

        public void Dispose()
        {
            _logger=null;
        }
    }
}
