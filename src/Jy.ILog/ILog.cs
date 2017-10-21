using System;

namespace Jy.ILog
{
    public interface ILogger
    {
        void LogDebug(string format, params object[] args);
        void LogError(string message, Exception exception);
        void LogInformation(string format, params object[] args);
        void LogWarning(string format, params object[] args);
    }
}
