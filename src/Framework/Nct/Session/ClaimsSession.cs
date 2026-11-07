using Nct.Ioc;
using System.Linq;
using System.Security.Claims;

namespace Nct.Session
{
    public class ClaimsSession : ISession, ITransientDependency
    {

        private IPrincipalAccessor _principalAccessor;

        public ClaimsSession(IPrincipalAccessor principalAccessor)
        {

            _principalAccessor = principalAccessor;
        }

        public int? UserId
        {

            get
            {
                var userIdClaim = _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return null;
                }

                return userId;

            }

        }

        public string UserName
        {

            get
            {
                var userNameClaim = _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (string.IsNullOrEmpty(userNameClaim?.Value))
                {
                    return null;
                }

                return userNameClaim?.Value;

            }

        }

        public bool IsAuthenticated => UserId.HasValue;
    }
}
