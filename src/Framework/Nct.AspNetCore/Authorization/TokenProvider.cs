using Microsoft.AspNetCore.Http;
using Nct.Authorization;
using Nct.Ioc;

namespace Nct.AspNetCore.Authorization
{
    public class TokenProvider : ITokenProvider,ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers.Authorization;
        }
    }
}
