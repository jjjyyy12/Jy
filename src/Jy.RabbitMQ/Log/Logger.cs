using Jy.Utility;
using RawRabbit.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jy.RabbitMQ
{
    public class Logger : ILogger
    {
        public void LogDebug(string format, params object[] args)
        {
            Console.WriteLine("Debug:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + string.Format(format, args));
            Log.WriteLog("D:\\LogFiles_MQ\\", "Debug", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + string.Format(format, args));
        }

        public void LogError(string message, Exception exception)
        {
            StringBuilder sberr = new StringBuilder();

            sberr.Append("\r\n---------------------------error begin :");
            sberr.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            sberr.Append(exception.Message);
            sberr.Append("\r\n  InnerException:");
            sberr.Append(exception.InnerException);
            sberr.Append("\r\n  StackTrace:");
            sberr.Append(exception.StackTrace);
            sberr.Append("\r\n---------------------------end error");

            Console.WriteLine(sberr.ToString());
            Log.WriteLog("D:\\LogFiles_MQ\\", "Error", sberr.ToString());
        }

        public void LogInformation(string format, params object[] args)
        {
            Console.WriteLine("Info:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + string.Format(format, args));
            Log.WriteLog("D:\\LogFiles_MQ\\", "Info", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + string.Format(format, args));
        }

        public void LogWarning(string format, params object[] args)
        {
            Console.WriteLine("Warning:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + string.Format(format, args));
            Log.WriteLog("D:\\LogFiles_MQ\\", "Warning", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + string.Format(format, args));
        }
    }
}
