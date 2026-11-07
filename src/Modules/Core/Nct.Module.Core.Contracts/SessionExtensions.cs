using Nct.Module.Core.Users;
using Nct.Module.Core.Users.Dtos;

namespace Nct.Module.Core
{
    public static class SessionExtensions
    {
       
        public static bool IsAdmin(this ISession session)
        {
            return session.IsAuthenticated && session.UserName.IgnoreCaseEqual(Constants.AdminUserName);

        }

        public static async Task<CurrentUserDto> ToUser(this ISession _)
        {

            using var obj = IocManager.Instance.ResolveAsDisposable<IUserAppService>();

            return await obj.Object.GetCurrentUser();

        }

    }
}
