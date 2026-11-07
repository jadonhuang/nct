using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nct.Logging
{
    public enum LogLevel
    {
        Debug,
        Information,
        Warning,
        Error,
        Fatal
    }

    public interface ILogger {

        bool IsEnabled(LogLevel level);
        void Log(LogLevel level, object message);

    }
}
