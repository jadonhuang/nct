using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Jobs
{
    public interface IJobStore
    {
        Task<bool> IsEnabled(string name);

    }
}
