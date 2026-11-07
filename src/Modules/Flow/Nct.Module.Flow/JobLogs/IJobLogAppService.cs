using Nct.Module.Flow.JobLogs.Dtos;

namespace Nct.Module.Flow.JobLogs
{

    public interface IJobLogAppService: ICrudAppService<JobLog,JobFlowLogDto, JobLogQueryDto,CreateJobFlowLogDto>
    {


    }
}
