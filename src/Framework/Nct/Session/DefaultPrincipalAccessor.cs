using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nct.Ioc;

namespace Nct.Session
{
    public class DefaultPrincipalAccessor : IPrincipalAccessor, ITransientDependency
    {
        public virtual ClaimsPrincipal Principal => Thread.CurrentPrincipal as ClaimsPrincipal;
    }
}
