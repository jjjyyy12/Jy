
using Jy.ILog;

namespace Jy.CKafka
{
    public class LoggerFactory
    {
        private static volatile ILogger _logger;
        private static readonly object _locker = new object();
        public static ILogger CreateLogger()
        {
            if (_logger == null)
            {
                lock (_locker)
                {
                    if (_logger == null)
                        _logger = new Logger();
                }
            }
            return _logger;
        }

        public void Dispose()
        {
            _logger=null;
        }
    }
}
