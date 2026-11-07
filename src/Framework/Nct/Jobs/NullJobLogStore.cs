using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Jobs
{
    public class NullJobLogStore : IJobLogStore
    {
        public Task Log(string jobName, bool success, string msg, int duration)
        {
            return Task.CompletedTask;
        }

        public static NullJobLogStore Instance
        {
            get { return new NullJobLogStore(); }
        }
    }
}
