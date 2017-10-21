using Jy.ILog;
using Jy.Utility;
using System;
using System.Text;

namespace Jy.TokenAuth
{
    public class Logger : ILogger
    {
        public void LogDebug(string format, params object[] args)
        {
            Console.WriteLine("Debug:" + string.Format(format, args));
            Log.WriteLog("D:\\LogFiles_TokenAuth\\", "Debug", string.Format(format, args));
        }

        public void LogError(string message, Exception exception)
        {
            StringBuilder sberr = new StringBuilder();

            sberr.Append("\r\n---------------------------error begin :");
            sberr.Append(exception.Message);
            sberr.Append("\r\n  InnerException:");
            sberr.Append(exception.InnerException);
            sberr.Append("\r\n  StackTrace:");
            sberr.Append(exception.StackTrace);
            sberr.Append("\r\n---------------------------end error");

            Console.WriteLine("Error:" + sberr.ToString());
            Log.WriteLog("D:\\LogFiles_TokenAuth\\", "Error", sberr.ToString());
        }

        public void LogInformation(string format, params object[] args)
        {
            Console.WriteLine("Info:" + string.Format(format, args));
            Log.WriteLog("D:\\LogFiles_TokenAuth\\", "Info", string.Format(format, args));
        }

        public void LogWarning(string format, params object[] args)
        {
            Console.WriteLine("Warning:" + string.Format(format, args));
            Log.WriteLog("D:\\LogFiles_TokenAuth\\", "Warning", string.Format(format, args));
        }
    }
}
