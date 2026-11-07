using Nct.Authorization;
using Nct.Module.Core.Roles;

namespace Nct.Module.Core.Authorize
{
    public class RoleChecker : IRoleChecker, ITransientDependency
    {
        private readonly IRoleAppService _roleAppService;
        private readonly ISession _session;
        public RoleChecker(IRoleAppService roleAppService, ISession session)
        {
            _roleAppService = roleAppService;
            _session = session;
        }
        public bool Authorize(string[] roles)
        {
            if (!_session.IsAuthenticated) return false;

            if (_session.IsAdmin()) return true;

            var userRoles = _roleAppService.GetUserRoles(_session.UserId.Value).ConfigureAwait(false).GetAwaiter().GetResult();

            if (userRoles.Count == 0) return false;

            foreach (var role in roles)
            {
                if (userRoles.Any(m => m.Code.IgnoreCaseEqual(role)))
                    return true;
            }

            return false;

        }
    }
}
