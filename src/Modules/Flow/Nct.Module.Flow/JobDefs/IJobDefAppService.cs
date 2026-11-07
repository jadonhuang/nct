using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Module.Flow.JobDefs.Dtos;

namespace Nct.Module.Flow.JobDefs
{    

    public interface IJobDefAppService: ICrudAppService<JobDef,JobDefDto, JobDefQueryDto,CreateJobDefDto>
    {
        Task<string> GetFlowConfig(int id);
        Task<RetResult> Run(int id, bool notify = true);
        Task RegisterAllJobs();

        Task SyncSystemJobToDb();

    }
}
