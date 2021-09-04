using RemoteInvoke.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke
{
    public static class LoggingExtensions
    {
        public static void Log(this ILogger logger, object value, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1)
        {
            logger.Log($"{value} ({memberName} in {filePath}[{lineNumber}])");
        }

        public static void LogLine(this ILogger logger, object value, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1)
        {
            logger.LogLine($"{value} ({memberName} in {filePath}[{lineNumber}])");
        }

        public static void Information(this ILogger logger, object value, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1)
        {
            logger.Information($"{value} ({memberName} in {filePath}[{lineNumber}])");
        }

        public static void Warning(this ILogger logger, object value, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1)
        {
            logger.Warning($"{value} ({memberName} in {filePath}[{lineNumber}])");
        }

        public static void Error(this ILogger logger, object value, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1)
        {
            logger.Error($"{value} ({memberName} in {filePath}[{lineNumber}])");
        }
    }
}
