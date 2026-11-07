using Microsoft.SqlServer.Management.SqlParser.Metadata;
using Nct.Identity.Entity;
using Nct.Module.Core.Contracts.Organizations;
using Nct.Module.Core.Organizations.Dtos;
using Nct.Module.Core.Users;
using Nct.Module.Core.Users.Dtos;
using System;
using System.ComponentModel;

namespace Nct.Module.Core.Organizations
{

    [RemoteService("Org")]

    public class OrganizationAppService : CrudAppService<Organization, OrganizationDto, QueryDto, CreateOrganizationDto>, IOrganizationAppService
    {

        private const int CodeLen = 4;
        private readonly IRepository<UserOrganization> _userOrgRep;
        private readonly IRepository<User> _userRep;
        private readonly IRepository<OrgManager> _orgMgrRep;
        public OrganizationAppService(IRepository<Organization> repository, IRepository<UserOrganization> userOrgRep, IRepository<User> userRep, IRepository<OrgManager> orgMgrRep) : base(repository)
        {

            _userOrgRep = userOrgRep;
            _userRep = userRep;
            _orgMgrRep = orgMgrRep;
        }

        protected override void SetPermissionNames()
        {
            CreatePermissionName = "addorg";
            DeletePermissionName = "delorg";
            UpdatePermissionName = "editorg";

        }


        public override async Task<OrganizationDto> Create(CreateOrganizationDto dto)
        {

            var entity = MapToEntity(dto);

            entity.Code = await GetNextChildCode(dto.ParentId);

            var existEntity = await Repository.GetAll().Where(m => m.Code == entity.Code).FirstOrDefaultAsync();

            if (existEntity != null)
                throw new BusinessException($"编码重复[{entity.Code}]");

            await Repository.InsertAsync(entity);

            await SetOrgManagers(dto.ManagerIds, entity.Id);

            return MapToEntityDto(entity);

        }


        public override async Task<OrganizationDto> Update(CreateOrganizationDto dto)
        {
            await SetOrgManagers(dto.ManagerIds, dto.Id);
            return await base.Update(dto);
        }

        private static string FormatCode(int code)
        {
            return code.ToString(new string('0', CodeLen));

        }

        public override async Task<int> Delete(int id)
        {
            var code = await GetCode(id);
            var num = await Repository.GetAll().Where(m => m.Code.StartsWith(code)).DeleteAsync();
            return num;

        }

        private async Task<string> GetCode(int id)
        {
            var entity = await Get(id);
            var code = entity.Code;
            return code;
        }


        private static string CalNextCode(string code)
        {

            var codes = code.Split('.');
            var lastCode = codes[^1];
            var nextCode = FormatCode(Convert.ToInt32(lastCode) + 1);
            if (codes.Length == 1)
            {
                return nextCode;

            }

            return code[..^lastCode.Length] + nextCode;


        }

        private async Task<string> GetNextChildCode(int parentId)
        {
            var lastChild = await Repository.GetAll().Where(m => m.ParentId == parentId).OrderByDescending(m => m.Code).FirstOrDefaultAsync();
            if (lastChild == null)
            {
                var childCode = FormatCode(1);
                if (parentId > 0)
                {
                    var parentCode = await GetCode(parentId);
                    return $"{parentCode}.{childCode}";

                }

                return childCode;

            }
            return CalNextCode(lastChild.Code);

        }


        [RemoteService(false)]
        public async Task<IList<OrganizationDto>> GetTree(int? pid)
        {

            var allOrgs = await GetAll();
            List<Organization> organizations;
            if (pid.HasValue)
            {

                var org = allOrgs.First(m => m.Id == pid);
                organizations = allOrgs.Where(m => m.Code.StartsWith(org.Code)).ToList();

            }
            else
            {
                organizations = allOrgs;

            }

            var orgDtos = organizations.MapTo<OrganizationDto>();

            return orgDtos.ToTree<int, OrganizationDto>();

        }


        public async Task<IList<OrganizationDto>> GetTreeForList(OrganizationDto dto)
        {

            var allOrgs = await Repository.GetAll().WhereIf(m => m.Name.Contains(dto.Name), dto.Name.IsNotEmpty()).ToListAsync();

            var orgDtos = allOrgs.MapTo<OrganizationDto>();

            return orgDtos.ToTree<int, OrganizationDto>();

        }

        [RemoteService(false)]
        public async Task<IList<UserOrganization>> GetUserOrgs(int uid)
        {

            var list = await _userOrgRep.GetAllListAsync(m => m.UserId == uid);
            return list;

        }

  
        private async Task<IList<OrgManager>> GetOrgManagers(int orgId)
        {

            var list = await _orgMgrRep.GetAllListAsync(m => m.OrgId == orgId);
            return list;

        }


        [RemoteService(false)]
        public async Task<IList<int>> GetUserOrgAndChildrenIds(int uid)
        {

            var ids = new List<int>();
            var list = await _userOrgRep.GetAllListAsync(m => m.UserId == uid);
            ids.AddRange(list.Select(m => m.OrgId));

            var orgs = await GetAll();
            var userOrgs = orgs.Where(m => ids.Contains(m.Id));

            foreach (var userOrg in userOrgs)
            {
                var subOrgs = orgs.Where(m => m.Code.StartsWith(userOrg.Code));
                foreach (var subOrg in subOrgs)
                {
                    if (!ids.Contains(subOrg.Id))
                    {
                        ids.Add(subOrg.Id);

                    }

                }

            }

            return ids;

        }


        [RemoteService(false)]

        public async Task SetUserOrgs(IList<int> orgIds, int uid)
        {

            var list = await GetUserOrgs(uid);

            if (list.Count == 0 && orgIds.Count == 0)
                return;

            if (list.Count > 0)
            {
                var deletingIds = list.Select(m => m.OrgId).Except(orgIds).ToList();
                if (deletingIds.Count > 0)
                {
                    await _userOrgRep.GetAll().Where(m => deletingIds.Contains(m.OrgId) && m.UserId == uid).DeleteAsync();
                }

            }

            foreach (int id in orgIds)
            {
                if (!list.Any(m => m.OrgId == id))
                {
                    var userOrg = new UserOrganization { OrgId = id, UserId = uid };
                    await _userOrgRep.InsertAsync(userOrg);

                }
            }


        }


        [RemoteService(false)]
        public async Task SetOrgManagers(IList<int> userIds, int orgId)
        {

            var list = await GetOrgManagers(orgId);

            if (list.Count == 0 && userIds.Count == 0)
                return;

            if (list.Count > 0)
            {
                var deletingIds = list.Select(m => m.UserId).Except(userIds).ToList();
                if (deletingIds.Count > 0)
                {
                    await _orgMgrRep.GetAll().Where(m => deletingIds.Contains(m.UserId) && m.OrgId == orgId).DeleteAsync();
                }

            }

            foreach (int id in userIds)
            {
                if (!list.Any(m => m.UserId == id))
                {
                    var model = new OrgManager { OrgId = orgId, UserId = id };
                    await _orgMgrRep.InsertAsync(model);

                }
            }


        }


        [RemoteService(false)]
        public async Task<List<Organization>> GetAll()
        {
            var list = await Repository.GetAllListAsync();
            return list;
        }

        [RemoteService(false)]
        public async Task<List<UserDto>> GetManagers(int orgId) {

            var users = _userRep.GetAll();
            var orgMgrs = _orgMgrRep.GetAll();

            var query = from user in users
                        join orgMgr in orgMgrs on user.Id equals orgMgr.UserId
                        where orgMgr.OrgId == orgId
                        orderby user.Id descending
                        select user;

            var userList = await query.ToListAsync();
            return userList.MapTo<UserDto>();

        }


        [RemoteService(false)]
        public async Task<List<UserDto>> GetManagers(int uid, int level)
        {

            var userDtos = new List<UserDto>();
            var userOrg = _userOrgRep.GetAll().Where(m => m.UserId == uid).FirstOrDefault();
            if (userOrg == null) return userDtos;

            var org = await Get(userOrg.OrgId);
            if (org == null) return userDtos;

            if (level > 0)
            {
                var codes = org.Code.Split('.');
                var deep = codes.Length;
                string orgCode;

                if (level + 1 > deep)
                {
                    orgCode = codes[0];
                }
                else
                {
                    orgCode = string.Join('.', codes.Take(deep - level));
                }

                org = Repository.GetAll().Where(m => m.Code == orgCode).FirstOrDefault();

                if (org == null) return userDtos;

            }


            userDtos = await GetManagers(org.Id);
            return userDtos;
        }


        public async Task<EditOrganizationDto> GetEditInfo(int id)
        {

            var dto = await GetDto(id);
            var editDto = dto.MapTo<EditOrganizationDto>();
            editDto.Managers= await GetManagers(id);
            return editDto;

        }

    }
}
