using Nct.Authorization;
using Nct.Module.Core.Menus;

namespace Nct.Module.Core.Authorize
{
    public class PermissionChecker : IPermissionChecker, ITransientDependency
    {

        private readonly IMenuAppService _menuAppService;
        private readonly ISession _session;
        public PermissionChecker(IMenuAppService menuAppService, ISession session)
        {
            _menuAppService = menuAppService;
            _session = session;
        }

        public bool Authorize(string[] permissions)
        {

            if (!_session.IsAuthenticated) return false;

            if (_session.IsAdmin()) return true;

            var userPermissions = _menuAppService.GetCurrentUserPermissions();

            if (userPermissions.Count == 0) return false;

            foreach (var p in permissions)
            {
                if (userPermissions.Any(m => m.Code.IgnoreCaseEqual(p)))
                    return true;
            
            }
            return false;
        }
    }
}
