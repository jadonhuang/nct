using Nct.Module.Core.Roles.Dtos;

namespace Nct.Module.Core.Roles
{

    public interface IRoleAppService: ICrudAppService<Role,RoleDto, RoleDto, CreateRoleDto>
    {


        Task<List<RoleShortInfo>> GetShortInfos(int? userId);


        Task<bool> SetUserRoles(int userId,List<int> roleIds);


        Task<List<Role>> GetUserRoles(int userId);

        Task<List<RoleShortInfo>> GetAll();

    }
}
