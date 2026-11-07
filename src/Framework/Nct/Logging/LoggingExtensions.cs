using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.Logging
{
   public static class LoggingExtensions
    {
        public static void Info(this ILogger logger ,object message)
        {

            FilteredLog(logger, LogLevel.Information,message);

        }
        public static void Warn(this ILogger logger, object message)
        {
            FilteredLog(logger, LogLevel.Warning, message);
        }

        public static void Debug(this ILogger logger, object message)
        {
            FilteredLog(logger, LogLevel.Debug, message);
        }

        public static void Error(this ILogger logger, object message)
        {
            FilteredLog(logger, LogLevel.Error, message);
        }
      
        public static void Fatal(this ILogger logger, object message)
        {
            FilteredLog(logger, LogLevel.Fatal, message);
        }
       
        private static void FilteredLog(ILogger logger, LogLevel level, object message)
        {
            if (logger.IsEnabled(level))
            {
                logger.Log(level, message);
            }
        }

    }
}
