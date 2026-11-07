using Nct.Ioc;
using Nct.Logging;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Jobs
{
    public class JobExecutor : ITransientDependency
    {

        private readonly ILogger _logger;
        public IJobLogStore JobLogStore { get; set; }
        public IJobStore JobStore { get; set; }

        public JobExecutor(ILogger logger)
        {
            JobLogStore = NullJobLogStore.Instance;
            JobStore = NullJobStore.Instance;
            _logger = logger;

        }

        public JobInfo JobDetail { get; set; }

        public async Task<bool> Execute()
        {
            if (JobDetail.IsBusy) return false;

            JobDetail.IsBusy = true;
            var jobTypeName = JobDetail.JobType.FullName;
            var enabled = await JobStore.IsEnabled(jobTypeName);
            if (!enabled) return false;

            var logStr = new StringBuilder();
            logStr.AppendLine($"任务：{JobDetail.Name} 开始执行");
            var stopwatch = Stopwatch.StartNew();
            var success = true;
            string message;
            try
            {

                using (var disposableObject = IocManager.Instance.ResolveAsDisposable<IJob>(JobDetail.JobType))
                {
                    await disposableObject.Object.Execute(JobDetail);
                }

                logStr.AppendLine($"{DateTime.Now} 执行成功");
                message = "执行成功";
            }
            catch (Exception ex)
            {
                success = false;
                if (ex.InnerException != null) ex = ex.InnerException;

                _logger.Error(ex);
                logStr.AppendLine($"{DateTime.Now} 执行失败{Environment.NewLine}{ex.Message}{Environment.NewLine}");
                message = ex.Message;
            }

            _logger.Info(logStr.ToString());

            if (!JobDetail.LogIgnore)
            {
                _ = JobLogStore.Log(jobTypeName, success, message, (int)stopwatch.ElapsedMilliseconds);
            }
            JobDetail.IsBusy = false;
            return success;

        }











    }
}
