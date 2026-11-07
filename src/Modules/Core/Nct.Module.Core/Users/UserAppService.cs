using Microsoft.AspNetCore.Identity;
using Nct.Authorization;
using Nct.DataAccess;
using Nct.DataAccess.Json;
using Nct.Domain.Entities;
using Nct.Identity.Entity;
using Nct.Identity.Jwt;
using Nct.Module.Core.Logs;
using Nct.Module.Core.Menus;
using Nct.Module.Core.Menus.Dtos;
using Nct.Module.Core.Organizations;
using Nct.Module.Core.Roles;
using Nct.Module.Core.Users.Dtos;
using Nct.SignalR;
using System;

namespace Nct.Module.Core.Users
{


    public class UserAppService : CrudAppService<User, UserDto, UserDto, CreateUserDto>, IUserAppService
    {

        public const string DefaultPwd = "123456";

        private readonly SignInManager<UserBase> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogAppService _logAppService;
        private readonly IRoleAppService _roleAppService;
        private readonly IMenuAppService _menuAppService;
        private readonly IOrganizationAppService _orgAppService;
        private readonly IRepository<UserOrganization> _userOrgRep;
        private readonly IRepository<Organization> _orgRep;
        private readonly IRepository<UserRole> _userRoleRep;

        public UserAppService(IRoleAppService roleAppService,
        SignInManager<UserBase> signInManager, IJwtTokenGenerator jwtTokenGenerator,
        IRepository<User> repository, ILogAppService logAppService,
            IMenuAppService menuAppService,
            IOrganizationAppService orgAppService,
            IRepository<UserOrganization> userOrgRep,
             IRepository<Organization> orgRep,
               IRepository<UserRole> userRoleRep

            ) : base(repository)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _logAppService = logAppService;
            _signInManager = signInManager;
            _roleAppService = roleAppService;
            _menuAppService = menuAppService;
            _orgAppService = orgAppService;
            _userOrgRep = userOrgRep;
            _orgRep = orgRep;
            _userRoleRep = userRoleRep;

        }


        public static string GetPwd(string pwd)
        {
            return pwd.ToMd5();
        }

        protected override IQueryable<User> CreateQuery(UserDto dto)
        {
            return base.CreateQuery(dto).Where(m => m.UserType == 0)
                .WhereIf(m => m.UserName.Contains(dto.UserName) || m.RealName.Contains(dto.UserName), dto.UserName.IsNotEmpty());
 
        }


        public override async Task<PageResult<UserDto>> GetList(PageQuery<UserDto> pageQuery)
        {

            if (pageQuery.Query.Id == 0)
                return await base.GetList(pageQuery);


            var users = CreateQuery(pageQuery.Query);
            var orgs = _orgRep.GetAll();
            var userOrgs = _userOrgRep.GetAll();

            var orgModel = await _orgAppService.Get(pageQuery.Query.Id);
            var orgCode = orgModel.Code;

            var query = from user in users
                        join userOrg in userOrgs on user.Id equals userOrg.UserId
                        join org in orgs on userOrg.OrgId equals org.Id
                        where org.Code.StartsWith(orgCode) && user.UserType == 1
                        orderby user.Id descending
                        select user;


            var queryCount = from user in users
                             join userOrg in userOrgs on user.Id equals userOrg.UserId
                             join org in orgs on userOrg.OrgId equals org.Id
                             where org.Code.StartsWith(orgCode) && user.UserType == 1
                             select user;

                            



            long count = await queryCount.LongCountAsync();

            query = query.Distinct().PageBy(pageQuery);

            var rows = await query.ToListAsync();

            var pageResult = new PageResult<UserDto>
            {
                Total = count,
                Rows = rows.MapTo<UserDto>(),
                PageIndex = pageQuery.PageIndex,
                PageSize = pageQuery.PageSize


            };
            return pageResult;




        }

        [RemoteService(false)]
        public override async Task<int> MultiDelete(int[] ids)
        {

            int rows = await Repository.DeleteAsync(m => ids.Contains(m.Id) && m.UserName != Constants.AdminUserName);
            return rows;
        }



        public Task<int> BatchDelete(string ids)
        {
            var intIds = ids.SplitString().Select(m => m.ToInt()).ToArray();
            return MultiDelete(intIds);
        }


        [RemoteService(false)]
        public PageTableResult GetPageList(PageQuery<IDictionary<string, object>> pageQuery)
        {


            if (pageQuery.OrderBy.IsEmpty())
                pageQuery.OrderBy = " id desc";


            string where = pageQuery.Query.GetOrDefault<string>("_where");
            if (where.IsEmpty())
            {
                where = "where isdeleted=0 and UserType=0";
            }
            else
            {
                where = where.Replace("t.[Id] = @_p0", "1=1");
                where += " and isdeleted=0 and UserType=0";
            }

            var orgId = pageQuery.Query.GetOrDefault<int>("Id");
            if (orgId > 0)
            {

                var org = _orgRep.GetAll().Where(m => m.Id == orgId).FirstOrDefault();
                where += $" and id in (select userid from  Core_UserOrganization uo join Core_Organization ou on uo.OrgId=ou.Id and ou.Code like '{org.Code}%')";
            }

            string fields;
            var ctxFileds = pageQuery.Query.GetOrDefault<string>("_fields");
            if (ctxFileds != null)
            {
                fields = string.Join(",", ctxFileds.SplitString().Where(m => m.IsName() && m != "CreatorId"));
            }
            else
            {
                fields = "Id,UserName,IsActive,RealName,LastLoginTime,Tel,Sex,Address,Birthday,CreateTime";
            }
            var sql = $"select {fields} from (select *,(select top 1 Name from Core_UserOrganization uo join  Core_Organization ou on uo.OrgId=ou.Id and uo.UserId=t.id) as OrgName from core_user t {where}) tb";
            pageQuery.Query.Remove("_query");
            pageQuery.Query.Remove("_where");
            pageQuery.Query.Remove("_fields");

            return Db.QueryTable(sql, pageQuery, pageQuery.Query);


        }




        [Anonymous]
        public async Task<RetResult> Login(LoginDto dto)
        {


            string userName = dto.UserName;
            string password = dto.Password;

            var result = CheckCodeHelper.Verify(dto.LoginId, dto.CheckCode);
            if (!result)
                return Error("验证码错误");

            var user = await _signInManager.UserManager.FindByNameAsync(userName);
            var success = false;
            if (user != null)
            {
                var ret = await _signInManager.CheckPasswordSignInAsync(user, GetPwd(password), user.IsLockoutEnabled);

                if (ret.Succeeded)
                {
                    success = true;

                    IocManager.Instance.Using<IRealTimeNotifier>(nf =>
                    {
                        nf.SendNotifications(new UserNotification { Data = new { user.UserName, Time = DateTime.Now.ToShortTimeString(), Msg = "登录了" } });
                    });


                }
            }

            await _logAppService.CreateLoginLog(user?.UserName ?? userName, success);

            if (success)
            {
                var token = _jwtTokenGenerator.Create(user);
                return Success(token);
            }

            return Error("用户名或密码错误");

        }




        [RemoteService(false)]
        public override Task<User> Get(int id)
        {
            return base.Get(id);
        }


        [AuthorizeEx("edituser")]
        public override async Task<UserDto> GetDto(int id)
        {
            var dto = await base.GetDto(id);
            return dto;
        }


        [RemoteService(false)]
        public async Task<int?> GetUserOrgId(int uid)
        {
            var orgs = await _orgAppService.GetUserOrgs(uid);
            return orgs.FirstOrDefault()?.Id;

        }

        private async Task CheckUserName(string userName, int userId)
        {

            using (DataFilter.Disable<ISoftDelete>())
            {

                var user = await Repository.FirstOrDefaultAsync(m => m.UserName == userName && m.Id != userId);
                if (user != null)
                    throw new BusinessException($"用户名[{userName}]已存在");


            }

        }


        private async Task CheckTel(string tel, int userId)
        {
            if (tel.IsEmpty()) return;

            using (DataFilter.Disable<ISoftDelete>())
            {
                var user = await Repository.FirstOrDefaultAsync(m => m.Tel == tel && m.Id != userId);
                if (user != null)
                    throw new BusinessException($"手机号[{tel}]已存在");
            }

        }


        public async Task<bool> CheckRegTel(string tel)
        {
            try
            {
                await CheckTel(tel, 0);
                await CheckUserName(tel, 0);
            }
            catch
            {
                return false;
            }

            return true;
        }


        [AuthorizeEx("adduser")]
        public override async Task<UserDto> Create(CreateUserDto dto)
        {


            await CheckTel(dto.Tel, 0);

            User user = dto.MapTo<User>();
            user.IsActive = true;
            var ret = await _signInManager.UserManager.CreateAsync(user, GetPwd(dto.Password));
            CheckIdentityResult(ret);
            if (dto.RoleIds != null)
            {
                await _roleAppService.SetUserRoles(user.Id, dto.RoleIds);

            }


            if (dto.OrgId.HasValue)
            {
                var orgIds = new List<int> { dto.OrgId.Value };
                await _orgAppService.SetUserOrgs(orgIds, user.Id);
            }

            return user.MapTo<UserDto>();


        }

        [AuthorizeEx("edituser")]
        public override async Task<UserDto> Update(CreateUserDto dto)
        {

            await CheckTel(dto.Tel, dto.Id);

            var user = await Get(dto.Id);
            dto.UserName = user.UserName;
            if (dto.Password.IsNotEmpty())
            {
                var hasher = _signInManager.UserManager.PasswordHasher;

                dto.Password = hasher.HashPassword(user, GetPwd(dto.Password));

            }
            else
            {
                dto.Password = user.Password;
            }

            var result = await base.Update(dto);

            await _roleAppService.SetUserRoles(result.Id, dto.RoleIds ?? []);

            var orgIds = new List<int>();
            if (dto.OrgId.HasValue)
            {
                orgIds.Add(dto.OrgId.Value);
            }
            await _orgAppService.SetUserOrgs(orgIds, result.Id);


            return result;
        }

        [AuthorizeEx("edituser,adduser")]
        public async Task<UserEditDto> GetEditInfo(int? id)
        {

            var userEditDto = new UserEditDto();
            var roles = await _roleAppService.GetAll();
            var orgs = await _orgAppService.GetTree(null);

            userEditDto.Roles = roles;
            userEditDto.Orgs = orgs;

            if (id.HasValue && id.Value > 0)
            {
                var userOrgs = await _orgAppService.GetUserOrgs(id.Value);
                userEditDto.UserOrgIds = userOrgs.Select(m => m.OrgId).ToList();

                var userRoles = await _roleAppService.GetUserRoles(id.Value);
                userEditDto.UserRoleIds = userRoles.Select(m => m.Id).ToList();

                userEditDto.User = await GetDto(id.Value);
            }

            return userEditDto;

        }


        protected override void MapToEntity(CreateUserDto dto, User entity)
        {

            var userName = entity.UserName;
            base.MapToEntity(dto, entity);

            entity.UserName = userName;

            if (entity.UserName.IgnoreCaseEqual(Constants.AdminUserName))
                entity.IsActive = true;

        }


        [AuthorizeEx("deluser")]
        public override async Task<int> Delete(int id)
        {
            var user = await Get(id);
            if (user.UserName.IgnoreCaseEqual(Constants.AdminUserName))
                return 0;

            return await base.Delete(id);
        }


        public async Task<CurrentUserDto> GetCurrentUser()
        {

            if (Session.UserId.HasValue)
            {
                var model = Repository.FirstOrDefault(m => m.Id == Session.UserId.Value);
                if (model != null)
                {
                    var dto = model.MapTo<CurrentUserDto>();
                    var menus = _menuAppService.GetCurrentUserPermissions();
                    var ps = menus.MapTo<ShortMenuDto>();
                    dto.Permissions = ps;
                    var roles = await _roleAppService.GetUserRoles(Session.UserId.Value);
                    var roleCodes = roles.Select(m => m.Code).ToList();
                    dto.Roles = roleCodes;
                    return dto;
                }
            }

            return null;
        }

        public async Task<bool> UpdatePassword(UpdatePassowordDto dto)
        {

            var hasher = _signInManager.UserManager.PasswordHasher;
            var user = await Get(Session.UserId.Value);
            if (hasher.VerifyHashedPassword(user, user.Password, GetPwd(dto.OldPassword)) != PasswordVerificationResult.Success)
                throw new BusinessException("旧密码错误");

            user.Password = hasher.HashPassword(user, GetPwd(dto.NewPassword));

            await Repository.UpdateAsync(user);

            return true;

        }

        public async Task<bool> UpdateUserInfo(EditUserInfoDto dto)
        {

            await CheckTel(dto.Tel, Session.UserId.Value);
            var user = await Get(Session.UserId.Value);
            user.MapFrom(dto);
            await Repository.UpdateAsync(user);
            return true;
        }



        [RemoteService(false)]
        public async Task UpdateBalance(int userId, decimal amt)
        {

            await Repository.GetAll().Where(m => m.Id == userId).UpdateAsync(m => new User { Balance = m.Balance + amt });

        }


        private static void CheckIdentityResult(IdentityResult result)
        {

            if (!result.Succeeded)
            {


                var errMsg = result.Errors.First().Code switch
                {
                    "DuplicateUserName" => "用户名重复",
                    "InvalidUserName" => "用户名格式不正确",
                    _ => "密码格式不正确",

                };

                throw new BusinessException(errMsg);


            }
        }

        [RemoteService(false)]
        public async Task Reg(MemberRegDto dto)
        {
            var verifyCode = Cache.Get<string>(dto.UserName.ToMd5());

            if (verifyCode == null || verifyCode != dto.VerifyCode)
            {
                throw new BusinessException("验证码错误");
            }


            await CheckTel(dto.UserName, 0);

            User user = dto.MapTo<User>();
            user.UserType = 1;
            user.IsActive = true;
            user.RealName = "user";
            user.Tel = dto.UserName;
            user.IsTelConfirmed = true;

            var ret = await _signInManager.UserManager.CreateAsync(user, GetPwd(dto.Password));
            CheckIdentityResult(ret);

            Cache.Remove(dto.UserName.ToMd5());


        }

        [AuthorizeEx(Roles = "admin")]
        public async Task<int> ResetPwd(List<int> userIds)
        {

            string pwd = _signInManager.UserManager.PasswordHasher.HashPassword(null, GetPwd(DefaultPwd));
            return await Repository.GetAll().Where(m => userIds.Contains(m.Id)).UpdateAsync(m => new User { Password = pwd, LockoutEndDate = null, AccessFailedCount = 0 });

        }

        public async Task ForgotPwd(MemberRegDto dto)
        {

            var verifyCode = Cache.Get<string>(dto.UserName.ToMd5());

            if (verifyCode == null || verifyCode != dto.VerifyCode)
            {
                throw new BusinessException("验证码错误");
            }

            var user = await Repository.FirstOrDefaultAsync(m => m.Tel == dto.UserName);
            if (user == null)
            {
                throw new BusinessException("用户不存在");
            }
            string pwd = _signInManager.UserManager.PasswordHasher.HashPassword(null, GetPwd(dto.Password));
            await Repository.GetAll().Where(m => m.Id == user.Id).UpdateAsync(m => new User { Password = pwd });
            Cache.Remove(dto.UserName.ToMd5());
        }

        [RemoteService(false)]
        public async Task<bool> TelExist(string tel)
        {
            try
            {
                await CheckTel(tel, 0);
            }
            catch
            {
                return true;
            }

            return false;
        }

        [RemoteService(false)]
        public async Task<bool> UpdateLastLoginTime(string userName)
        {
            var ret = await Repository.GetAll().Where(m => m.UserName == userName).UpdateAsync(m => new User { LastLoginTime = DateTime.Now });
            return ret > 0;
        }

        public async Task<object> GetShortList()
        {

            var list = await Repository.GetAll().Select(m => new { m.Id, m.UserName, m.RealName, m.Tel, m.Sex }).ToListAsync();
            return list;
        }

        [AuthorizeEx("edituser,adduser")]
        public async Task<bool> SetActive(int uid, bool active)
        {
            var ret = await Repository.GetAll().Where(m => m.Id == uid).UpdateAsync(m => new User { IsActive = active });
            return ret > 0;

        }


        [RemoteService(false)]
        public async Task<List<UserDto>> GetListByRoleId(int roleId)
        {

            var users = Repository.GetAll();
            var userRoles = _userRoleRep.GetAll();

            var query = from user in users
                        join userRole in userRoles on user.Id equals userRole.UserId
                        where userRole.RoleId == roleId
                        orderby user.Id descending
                        select user;

            var userLst = await query.ToListAsync();

            return userLst.MapTo<UserDto>();


        }

        [Post]
        public async Task<List<UserDto>> GetListByIds(int[] ids)
        {

            var users = await Repository.GetAllListAsync(m => ids.Contains(m.Id));
            return users.MapTo<UserDto>();

        }






    }
}

