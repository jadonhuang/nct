using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Nct.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Authorization
{
    public class DefaultPermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {

        private readonly IPermissionChecker _permissionChecker;

        public DefaultPermissionAuthorizationHandler(IPermissionChecker permissionChecker)
        {
            _permissionChecker = permissionChecker;


        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            if (_permissionChecker.Authorize(requirement.PermissionName))
            {
                context.Succeed(requirement);
            }
       
            return Task.CompletedTask;
        }
    }


    public class DefaultRoleAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {

        private readonly IRoleChecker _roleChecker;

        public DefaultRoleAuthorizationHandler(IRoleChecker roleChecker)
        {
            _roleChecker = roleChecker;


        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {

            if (_roleChecker.Authorize(requirement.AllowedRoles.ToArray()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

 

}
