using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Jobs
{
    public interface IJobLogStore
    {

        Task Log(string jobName, bool success, string msg, int duration);

    }
}
