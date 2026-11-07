using Nct.Jobs;

namespace Nct.Module.Flow.JobDefs
{
    public class JobStore : IJobStore, ITransientDependency
    {
        private readonly IRepository<JobDef> _repository;
        public JobStore(IRepository<JobDef> repository)
        {
            _repository = repository;

        }

        public async Task<bool> IsEnabled(string name)
        {
            var job = await _repository.FirstOrDefaultAsync(x => x.Name == name || x.ExeMethod == name);
            return job == null || job.IsEnabled;
        }
    }
}
