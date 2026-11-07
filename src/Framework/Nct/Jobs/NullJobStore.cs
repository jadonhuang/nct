using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Jobs
{
    public class NullJobStore : IJobStore
    {
        public Task<bool> IsEnabled(string name)
        {
            return Task.FromResult(true);
        }

        public static NullJobStore Instance
        {
            get { return new NullJobStore(); }
        }
    }
}
