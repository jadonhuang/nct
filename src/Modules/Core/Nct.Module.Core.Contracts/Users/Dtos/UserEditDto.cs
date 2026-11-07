using Nct.Module.Core.Organizations.Dtos;
using Nct.Module.Core.Roles.Dtos;

namespace Nct.Module.Core.Users.Dtos
{
    public class UserEditDto
    {
        public UserDto User { get; set; }

        public IList<OrganizationDto> Orgs { get; set; }

        public List<int> UserOrgIds { get; set; } = [];

        public List<RoleShortInfo> Roles { get; set; }

        public List<int> UserRoleIds { get; set; } = [];


    }
}
