using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Module.Core.Users.Dtos;

namespace Nct.Module.Core.Users
{
    public interface IUserAppService : ICrudAppService<User, UserDto, UserDto, CreateUserDto>
    {

        Task<RetResult> Login(LoginDto dto);

        Task<CurrentUserDto> GetCurrentUser();

        Task<bool> UpdatePassword(UpdatePassowordDto dto);

        Task<bool> UpdateLastLoginTime(string userName);

        Task<bool> UpdateUserInfo(EditUserInfoDto dto);


        Task UpdateBalance(int userId, decimal amt);


        Task Reg(MemberRegDto dto);


        Task ForgotPwd(MemberRegDto dto);

        Task<int> ResetPwd(List<int> userIds);

        Task<bool> CheckRegTel(string tel);


        Task<bool> TelExist(string tel);

        Task<int?> GetUserOrgId(int uid);

        PageTableResult GetPageList(PageQuery<IDictionary<string, object>> pageQuery);

        Task<List<UserDto>> GetListByIds(int[] ids);

        Task<List<UserDto>> GetListByRoleId(int roleId);

    }
}
