using System.Threading.Tasks;

namespace Nct.Jobs
{
    public interface IJobWorker
    {


        Task Create(JobInfo job);

        Task Start();

        Task Stop();

        Task<bool> Remove(JobInfo job);

    }
}
