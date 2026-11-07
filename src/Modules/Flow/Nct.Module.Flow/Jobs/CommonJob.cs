using Nct.Jobs;
using Nct.Module.Flow.JobDefs;

namespace Nct.Module.Flow.Jobs
{

   // [Job("自定义通用任务",1)]
    public class CommonJob : IJob
    {
        private readonly IJobDefAppService _jobDefAppService;
        public CommonJob(IJobDefAppService jobDefAppService)   
        {
           _jobDefAppService = jobDefAppService;
        
        }
        public async Task Execute(JobInfo job )
        {
            var id = (int)job.Data;
            await _jobDefAppService.Run(id,false);
        }
    }
}
