using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Domain.Entities
{
    public abstract class AuditEntity : AuditEntity<int>
    {

    }

    public abstract class AuditEntity<T> : CreationAuditEntity<T>, IHasUpdateTime, IHasUpdateUser
    {
        public DateTime UpdateTime { get; set; }
        public string UpdateUser { get; set; }

    }
}
