using Nct.Authorization;
using Nct.Domain.Entities;
using Nct.Identity.Entity;
using Nct.Module.Core.Menus;
using Nct.Module.Core.Roles.Dtos;
using System.Data;

namespace Nct.Module.Core.Roles
{




    public class RoleAppService : CrudAppService<Role, RoleDto, RoleDto, CreateRoleDto>, IRoleAppService
    {
        private readonly IRepository<UserRole> _userRoleRep;

        private const string UserRoleCacheKey = "userrole_";
        public RoleAppService(IRepository<Role> repository, IRepository<UserRole> userRoleRep) : base(repository)
        {
            _userRoleRep = userRoleRep;

        }


        protected override void SetPermissionNames()
        {
            CreatePermissionName = "addrole";
            DeletePermissionName = "delrole";
            UpdatePermissionName = "editrole";

        }


    
        protected override IQueryable<Role> CreateQuery(RoleDto dto)
        {
            return base.CreateQuery(dto).WhereIf(m => m.Code.Contains(dto.Code), !dto.Code.IsEmpty())
              .WhereIf(m => m.Name.Contains(dto.Name), !dto.Name.IsEmpty())
             .WhereIf(m => m.Descpt.Contains(dto.Descpt), !dto.Descpt.IsEmpty());
        }


        public override Task<int> Delete(int id)
        {
            Cache.Clear(UserRoleCacheKey);
            return base.Delete(id);
        }


        public async override Task<RoleDto> Create(CreateRoleDto input)
        {
            using (DataFilter.Disable<ISoftDelete>())
            {
                var user = await Repository.FirstOrDefaultAsync(m => m.Code == input.Code);
                if (user != null)
                    throw new BusinessException($"角色编号[{input.Code}]已存在");
            }

            var dto = await base.Create(input);
            return dto;
        }


 
        public async override Task<RoleDto> Update(CreateRoleDto input)
        {


            using (DataFilter.Disable<ISoftDelete>())
            {
                var user = await Repository.FirstOrDefaultAsync(m => m.Code == input.Code && m.Id != input.Id);
                if (user != null)
                    throw new BusinessException($"角色编号[{input.Code}]已存在");
            }

            var dto = await base.Update(input);

            Cache.Clear(UserRoleCacheKey);
            Cache.Clear(MenuAppService.UserPermissionsCacheKey);

            return dto;
        }


        protected override void MapToEntity(CreateRoleDto updateInput, Role entity)
        {
            var code = entity.Code;
            base.MapToEntity(updateInput, entity);
            entity.Code = code;
        }



        public async Task<List<RoleShortInfo>> GetShortInfos(int? userId)
        {

            var list = (await Repository.GetAllListAsync()).MapTo<RoleShortInfo>();

            if (userId.HasValue)
            {
                var userRoles = _userRoleRep.GetAllList(m => m.UserId == userId.Value);
                if (userRoles.Count > 0)
                {
                    foreach (var role in list)
                    {
                        if (userRoles.Any(m => m.RoleId == role.Id))
                            role.Checked = true;


                    }

                }

            }

            return list;


        }


        public async Task<List<RoleShortInfo>> GetAll()
        {

            var list = (await Repository.GetAllListAsync()).MapTo<RoleShortInfo>();

            return list;


        }

        [AuthorizeEx("editrole")]
        public async Task<bool> SetUserRoles(int userId, List<int> roleIds)
        {


            var list = await _userRoleRep.GetAll().Where(m => m.UserId == userId).ToListAsync();

            if (list.Count == 0 && roleIds.Count == 0)
                return true;

            if (list.Count > 0)
            {
                var deletingIds = list.Select(m => m.RoleId).Except(roleIds).ToList();
                if (deletingIds.Count > 0)
                {
                    await _userRoleRep.GetAll().Where(m => deletingIds.Contains(m.RoleId) && m.UserId == userId).DeleteAsync();
                }

            }

            foreach (int id in roleIds)
            {
                if (!list.Any(m => m.RoleId == id))
                {
                    var userRole = new UserRole { RoleId = id, UserId = userId };
                    await _userRoleRep.InsertAsync(userRole);

                }
            }

            Cache.Remove(UserRoleCacheKey + userId);
            Cache.Remove(MenuAppService.UserPermissionsCacheKey + userId);
            return true;

        }

        [RemoteService(false)]
        public Task<List<Role>> GetUserRoles(int userId)
        {

            return Task.FromResult(Cache.Get(UserRoleCacheKey + userId, () =>
            {


                using (DataFilter.Disable<ISoftDelete>())
                {
                    var roles = Repository.GetAll();
                    var userRoles = _userRoleRep.GetAll();


                    var query = from role in roles
                                join userRole in userRoles on role.Id equals userRole.RoleId
                                where userRole.UserId == userId
                                select role;

                    return query.ToList();

                }


            }));



        }


    }
}
