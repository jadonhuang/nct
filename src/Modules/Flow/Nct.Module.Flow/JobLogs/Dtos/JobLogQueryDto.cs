using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.JobLogs.Dtos
{
   public class JobLogQueryDto
    {
        public string JobName { get; set; }

        public DateTime?[] DateRange { get; set; }

        public bool? Success { get; set; }

    }
}
