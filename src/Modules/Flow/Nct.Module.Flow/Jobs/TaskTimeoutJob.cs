using Nct.Jobs;
using Nct.Module.Flow.Flows;

namespace Nct.Module.Flow.Jobs
{

    [Job("任务超时提醒",1)]
    public class TaskTimeoutJob : IJob
    {
        private readonly IFlowAppService _flowAppService;
        public TaskTimeoutJob(IFlowAppService flowAppService)   
        {
           _flowAppService = flowAppService;
        
        }
        public async Task Execute(JobInfo job)
        {
            await _flowAppService.HandleTimeoutTask();
        }
    }
}
