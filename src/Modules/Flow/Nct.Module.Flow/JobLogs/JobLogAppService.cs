using Nct.Module.Flow.JobLogs.Dtos;

namespace Nct.Module.Flow.JobLogs
{


    public class JobLogAppService : CrudAppService<JobLog, JobFlowLogDto, JobLogQueryDto, CreateJobFlowLogDto>, IJobLogAppService
    {

        public JobLogAppService(IRepository<JobLog> repository) : base(repository)
        {


        }

        protected override IQueryable<JobLog> CreateQuery(JobLogQueryDto dto)
        {
            return base.CreateQuery(dto).Where(m => m.JobName == dto.JobName)
                .WhereIf(m => m.IsSuccess == dto.Success.Value, dto.Success.HasValue)
                 .WhereIf(m => m.CreateTime >= dto.DateRange[0].Value, dto.DateRange != null && dto.DateRange[0].HasValue)
               .WhereIf(m => m.CreateTime <= dto.DateRange[1].Value.ToDayEnd(), dto.DateRange != null && dto.DateRange[1].HasValue);

            ;
        }

        public async Task<bool> ClearLog(string jobName)
        { 
        
            await  Repository.DeleteAsync(x => x.JobName == jobName);
            return true;
        }


        private async Task<bool> Test(int a) 
        { 
           // await Task.Delay(2000);
           return true;
        
        }

    }
}
