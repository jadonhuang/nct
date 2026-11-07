using System;

namespace Nct.Jobs
{


    public enum JobIntervalType
    {
        Minutes = 1,
        Daily = 2,
        Cron = 3,
    }

    public class JobInterval
    {
        public JobIntervalType Type { get; set; } 
        public TimeSpan Value { get; set; }
        public string CronExpression { get; set; }

    }
}
