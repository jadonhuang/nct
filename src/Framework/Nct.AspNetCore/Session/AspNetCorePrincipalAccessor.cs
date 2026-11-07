using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Nct.Session;
using Microsoft.AspNetCore.Http;

namespace Nct.AspNetCore.Session
{
    public class AspNetCorePrincipalAccessor : DefaultPrincipalAccessor
    {

        public override ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User ?? base.Principal;


        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCorePrincipalAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
