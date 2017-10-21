using Jy.ILog;
using Jy.Utility;
using System;
using System.Text;

namespace Jy.RabbitMQ.ProcessMessage
{
    public class Logger : ILogger
    {
        public void LogDebug(string format, params object[] args)
        {
            string content = null;
            if (args.Length == 0)
                content = format;
            else
                content = string.Format(format, args);
            Console.WriteLine("Debug:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + content);
            Log.WriteLog("D:\\LogFiles_AuthOperateLog\\", "Debug", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + content);
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
            Log.WriteLog("D:\\LogFiles_AuthOperateLog\\", "Error", sberr.ToString());
        }

        public void LogInformation(string format, params object[] args)
        {
            string content = null;
            if (args.Length == 0)
                content = format;
            else
                content = string.Format(format, args);
            Console.WriteLine("Info:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + content);
            Log.WriteLog("D:\\LogFiles_AuthOperateLog\\", "Info", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + content);
        }

        public void LogWarning(string format, params object[] args)
        {
            string content = null;
            if (args.Length == 0)
                content = format;
            else
                content = string.Format(format, args);
            Console.WriteLine("Warning:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + content);
            Log.WriteLog("D:\\LogFiles_AuthOperateLog\\", "Warning", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + content);
        }
    }
}
