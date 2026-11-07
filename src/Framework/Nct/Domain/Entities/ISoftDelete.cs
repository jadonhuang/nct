using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.Domain.Entities
{
   public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
