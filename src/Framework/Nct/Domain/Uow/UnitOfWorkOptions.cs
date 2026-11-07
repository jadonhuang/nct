using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nct.Domain.Uow
{
    public class UnitOfWorkOptions
    {
        public IsolationLevel IsolationLevel { get; set; }

        public TimeSpan? Timeout { get; set; }

        public bool? IsTransactional { get; set; }

    }
}
