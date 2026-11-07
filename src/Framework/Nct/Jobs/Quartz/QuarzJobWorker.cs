using Nct.Ioc;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Nct.Jobs.Qz
{
    public class QuartzJobWorker : IJobWorker
    {

        private readonly IScheduler scheduler;
        private readonly ISchedulerFactory factory;

        public QuartzJobWorker()
        {

            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            factory = new StdSchedulerFactory(props);
            scheduler = factory.GetScheduler().Result;
        }

        public async Task Create(JobInfo info)
        {
            var job = BuildJob(info);
            var trigger = BuildTrigger(info);
            await scheduler.ScheduleJob(job, trigger);
        }


        private static string GetJobKey(JobInfo job)
        {
            return $"{job.Name}_{job.JobType?.FullName}";
        }

        private static IJobDetail BuildJob(JobInfo job)
        {
            var executor = IocManager.Instance.Resolve<JobExecutor>();
            executor.JobDetail = job;
            return JobBuilder.Create<QuartzJob>()
                .WithIdentity(GetJobKey(job), job.Group)
                .SetJobData(new JobDataMap {
                    { "executor", executor }
                })
                .Build();
        }

        private static ITrigger BuildTrigger(JobInfo job)
        {
            var interval = job.Interval;
            var builder = TriggerBuilder.Create()
                .WithIdentity(GetJobKey(job) + "Trigger", job.Group);

            if (interval.Type == JobIntervalType.Minutes)
            {
                builder.WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(Convert.ToInt32(interval.Value.TotalMinutes))
                    .RepeatForever());
            }
            else if (interval.Type == JobIntervalType.Daily)
            {
                builder.WithDailyTimeIntervalSchedule(x =>
                x.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(interval.Value.Hours, interval.Value.Minutes))
                .EndingDailyAfterCount(1)
                .WithIntervalInHours(1));
            }
            else if (interval.Type == JobIntervalType.Cron)
            {
                builder.WithCronSchedule(interval.CronExpression);
            }
            return builder.Build();
        }




        public async Task Start()
        {
            await scheduler.Start();
        }

        public async Task Stop()
        {
            await scheduler?.Shutdown(true);
        }

        public async Task<bool> Remove(JobInfo job)
        {
            var jobKey = new JobKey(GetJobKey(job), job.Group);
            var success= await scheduler.DeleteJob(jobKey);
            return success;
        }
    }
}
