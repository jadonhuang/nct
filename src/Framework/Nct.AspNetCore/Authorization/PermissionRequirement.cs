using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }

        public PermissionRequirement(string permissionName)
        {
            Check.NotNull(permissionName, nameof(permissionName));
            PermissionName = permissionName;
        }

        public override string ToString()
        {
            return $"PermissionRequirement: {PermissionName}";
        }

    }
}
