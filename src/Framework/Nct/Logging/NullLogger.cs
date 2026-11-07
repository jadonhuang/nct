using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.Logging
{
    public class NullLogger : ILogger
    {

        public static ILogger Instance { get; } = new NullLogger();

        public bool IsEnabled(LogLevel level)
        {
            return false;
        }

        public void Log(LogLevel level, object message)
        {
            
        }

    }
}
