using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Logging
{
    public class DefaultLogger : ILogger
    {
        public void Log(string value)
        {
            Console.Write(value ?? "null");
        }

        public void LogLine(string value)
        {
            Console.WriteLine(value ?? "null");
        }

        public void Information(string value)
        {
            LogLine(value);
        }

        public void Error(string value)
        {
            LogLine(value);
        }

        public void Warning(string value)
        {
            LogLine(value);
        }
    }
}
