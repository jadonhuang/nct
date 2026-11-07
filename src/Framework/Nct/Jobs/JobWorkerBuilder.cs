using Nct.Extentions;
using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Nct.Jobs
{
    public class JobWorkerBuilder
    {

        private static readonly List<JobInfo> jobInfos = [];

        public static JobWorkerBuilder Create()
        {
            return new JobWorkerBuilder();
        }
        public static List<JobInfo> GetJobs()
        {
            return jobInfos;
        }

        public JobWorkerBuilder Register(IReadOnlyList<Type> types)
        {
            var jobTypes = types.Where(m => typeof(IJob).IsAssignableFrom(m) && m.IsClass);
            foreach (Type type in jobTypes)
            {
                var jobAttr = type.GetCustomAttribute<JobAttribute>();
                if (jobAttr == null || !jobAttr.Enabled || jobAttr.Name.IsEmpty())
                    continue;

                var jobInfo = new JobInfo
                {
                    Interval = jobAttr.Interval,
                    Name = jobAttr.Name,
                    JobType = type
                };
                jobInfos.Add(jobInfo);

            }

            return this;

        }


        public IJobWorker Build()
        {
            var worker = IocManager.Instance.Resolve<IJobWorker>();
            foreach (var job in jobInfos)
            {
                worker.Create(job);
            }

            return worker;
        }

    }
}
