using Nct.Jobs;

namespace Nct.Module.Flow.JobLogs
{
    public class JobLogStore : IJobLogStore, ITransientDependency
    {
        private readonly IRepository<JobLog> _repository;
        public JobLogStore(IRepository<JobLog> repository)
        {
            _repository = repository;

        }
        public async Task Log(string jobName, bool success, string msg, int duration)
        {
            await _repository.InsertAsync(new JobLog
            {
                JobName = jobName,
                IsSuccess = success,
                Message = msg,
                ElapsedTime = duration,
                CreateTime = DateTime.Now,
                LogType = 3,
            });
        }
    }
}
