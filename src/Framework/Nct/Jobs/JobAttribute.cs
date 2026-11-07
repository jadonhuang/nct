using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Jobs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobAttribute : Attribute
    {
       
        public string Name { get; }


        public JobInterval Interval { get; }

        public bool Enabled { get; set; } = true;

        public JobAttribute(string name, int minutes)
        {
            this.Name = name;
            this.Interval = new JobInterval
            {
                Type = JobIntervalType.Minutes,
                Value = new TimeSpan(0, minutes, 0)
            };
        }


        public JobAttribute(string name, int hours, int minutes)
        {
            this.Name = name;
            this.Interval = new JobInterval
            {
                Type = JobIntervalType.Daily,
                Value = new TimeSpan(hours, minutes, 0),
            };
        }


        public JobAttribute(string name, string cronExpression)
        {
            this.Name = name;
            this.Interval = new JobInterval
            {
                Type = JobIntervalType.Cron,
                CronExpression = cronExpression
            };
        }
    }
}
