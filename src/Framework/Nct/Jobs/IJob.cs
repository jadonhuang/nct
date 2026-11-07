using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace Nct.Jobs
{
    public interface IJob : ITransientDependency
    {
             
        Task Execute(JobInfo job);

    }
}