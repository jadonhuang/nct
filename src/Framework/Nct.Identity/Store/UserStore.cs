using Microsoft.AspNetCore.Identity;
using Nct.Identity.Entity;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Nct.Identity.Store
{
    public class UserStore<TUser, TRole> :
        IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        // IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IQueryableUserStore<TUser>
        where TUser : UserBase
        where TRole : RoleBase
    {

        public IQueryable<TUser> Users => UserRepository.GetAll();
        public IRepository<TUser> UserRepository { get; }

        private readonly IRepository<TRole> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IRepository<UserToken> _userTokenRepository;
        public UserStore(IRepository<TUser> userRepository,
            IRepository<TRole> roleRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<UserLogin> userLoginRepository,
            IRepository<UserToken> userTokenRepository

            )
        {
            UserRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userLoginRepository = userLoginRepository;
            _userTokenRepository = userTokenRepository;

        }



        public async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default)
        {

            cancellationToken.ThrowIfCancellationRequested();

            await _userLoginRepository.InsertAsync(new UserLogin(user.Id, login.LoginProvider, login.ProviderKey));

        }


        protected Task<TRole> FindRoleAsync(string roleName)
        {
            return _roleRepository.FirstOrDefaultAsync(m => m.Code == roleName);

        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await IsInRoleAsync(user, roleName, cancellationToken))
            {
                return;
            }

            var roleEntity = await _roleRepository.FirstOrDefaultAsync(r => r.Code == roleName);

            var userRoleEntity = await _userRoleRepository.FirstOrDefaultAsync(m => m.RoleId == roleEntity.Id && m.UserId == user.Id);
            if (userRoleEntity == null)
            {
                await _userRoleRepository.InsertAsync(new UserRole(user.Id, roleEntity.Id));

            }

        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default)
        {

            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.InsertAsync(user);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.DeleteAsync(user);

            return IdentityResult.Success;
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return UserRepository.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var uidInt = userId.To<int>();
            return UserRepository.FirstOrDefaultAsync(u => u.Id == uidInt);
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {


            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            var query = from userLogin in _userLoginRepository.GetAll()
                        join user in UserRepository.GetAll() on userLogin.UserId equals user.Id
                        where userLogin.LoginProvider == loginProvider &&
                              userLogin.ProviderKey == providerKey

                        select user;

            return query.FirstOrDefaultAsync();


        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return UserRepository.FirstOrDefaultAsync(u => u.UserName == normalizedUserName);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }



        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsEmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsLockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            if (!user.LockoutEndDate.HasValue)
            {
                return Task.FromResult<DateTimeOffset?>(null);
            }

            return Task.FromResult<DateTimeOffset?>(new DateTimeOffset(user.LockoutEndDate.Value));
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return (await _userLoginRepository.GetAllListAsync(l => l.UserId == user.Id)).Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.LoginProvider)).ToList();
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Password);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Tel);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsTelConfirmed);
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            var query = from userRole in _userRoleRepository.GetAll()
                        join role in _roleRepository.GetAll() on userRole.RoleId equals role.Id
                        where userRole.UserId == user.Id
                        select role.Code;

            return await query.ToListAsync();
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }


        public async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return (await _userTokenRepository.FirstOrDefaultAsync(u => u.UserId == user.Id && u.LoginProvider == loginProvider && u.Name == name))?.Value;

        }



        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.UserName);
        }



        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var role = await _roleRepository.FirstOrDefaultAsync(r => r.Code == roleName);

            if (role == null)
            {
                return new List<TUser>();
            }

            var query = from userrole in _userRoleRepository.GetAll()
                        join user in UserRepository.GetAll() on userrole.UserId equals user.Id
                        where userrole.RoleId.Equals(role.Id)
                        select user;

            return await query.ToListAsync();
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Password != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);

        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();


            Check.NotNull(user, nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(nameof(roleName) + " can not be null or whitespace");
            }

            var role = await _roleRepository.FirstOrDefaultAsync(r => r.Code == roleName);
            if (role == null)
            {
                return false;
            }

            return await _userRoleRepository.FirstOrDefaultAsync(m => m.RoleId == role.Id) != null;


        }



        public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(nameof(roleName) + " can not be null or whitespace");
            }

            if (!await IsInRoleAsync(user, roleName, cancellationToken))
            {
                return;
            }

            var role = await _roleRepository.FirstOrDefaultAsync(r => r.Code == roleName);
            if (role == null)
            {
                return;
            }

            await _userRoleRepository.DeleteAsync(m => m.UserId == user.Id && m.RoleId == role.Id);

        }

        public async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            await _userLoginRepository.DeleteAsync(m => m.UserId == user.Id && m.LoginProvider == loginProvider && m.ProviderKey == providerKey);
        }

        public async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            await _userTokenRepository.DeleteAsync(m => m.UserId == user.Id && m.LoginProvider == loginProvider && m.Name == name);
        }



        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.Email = email;

            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.IsEmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.IsLockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.LockoutEndDate = lockoutEnd?.UtcDateTime;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.Email = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken = default)
        {
            //cancellationToken.ThrowIfCancellationRequested();

            //Check.NotNull(user, nameof(user));
            //user.UserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.Password = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.Tel = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.IsTelConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken = default)
        {

            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            var userToken = await _userTokenRepository.FirstOrDefaultAsync(m => m.UserId == user.Id && m.LoginProvider == loginProvider && m.Name == name);

            if (userToken == null)
            {
                userToken = new UserToken { UserId = user.Id, LoginProvider = loginProvider, Name = name, Value = value };
                await _userTokenRepository.InsertAsync(userToken);
            }
            else
            {
                userToken.Value = value;
                await _userTokenRepository.UpdateAsync(userToken);
            }


        }



        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UserRepository.UpdateAsync(user);

            return IdentityResult.Success;

        }
    }


}
