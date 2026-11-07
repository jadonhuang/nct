using Nct.Module.Core.Organizations.Dtos;
using Nct.Module.Core.Users.Dtos;

namespace Nct.Module.Core.Organizations
{

    public interface IOrganizationAppService : ICrudAppService<Organization, OrganizationDto, QueryDto, CreateOrganizationDto>
    {

        Task<IList<OrganizationDto>> GetTree(int? pid);

        Task SetUserOrgs(IList<int> orgIds, int uid);

        Task<IList<UserOrganization>> GetUserOrgs(int uid);

        Task<List<Organization>> GetAll();

        Task<IList<int>> GetUserOrgAndChildrenIds(int uid);

        Task SetOrgManagers(IList<int> userIds, int orgId);

        Task<List<UserDto>> GetManagers(int uid, int level);

        Task<List<UserDto>> GetManagers(int orgId);
        


        }
}
