using Quartz;
using System.Threading.Tasks;

namespace Nct.Jobs.Qz
{
    public class QuartzJob : Quartz.IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var executor = context.MergedJobDataMap["executor"] as JobExecutor;
            await executor.Execute();

        }
    }
}
