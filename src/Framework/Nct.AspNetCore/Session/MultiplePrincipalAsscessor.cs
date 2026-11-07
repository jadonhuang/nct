using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Nct.AspNetCore.Session
{
    public class MultiplePrincipalAsscessor : AspNetCorePrincipalAccessor
    {


        public override ClaimsPrincipal Principal
        {

            get
            {
                ClaimsPrincipal claimsPrincipal = null;
                try
                {
                    claimsPrincipal = _authenticationStateProvider.GetAuthenticationStateAsync().Result.User;
                }
                catch { }
                return claimsPrincipal ?? base.Principal;

            }

        }



        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public MultiplePrincipalAsscessor(IHttpContextAccessor httpContextAccessor, AuthenticationStateProvider authenticationStateProvider) : base(httpContextAccessor)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

    }
}
