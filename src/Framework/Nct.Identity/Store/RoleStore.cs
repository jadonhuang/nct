using Microsoft.AspNetCore.Identity;
using Nct.Identity.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Nct.Identity.Store
{
    public class RoleStore<TRole> : IRoleStore<TRole> where TRole : RoleBase
    {

        private readonly IRepository<TRole> _roleRepository;
        public RoleStore(IRepository<TRole> roleRepository)
        {
            _roleRepository = roleRepository;

        }
        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));
            await _roleRepository.InsertAsync(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));
            await _roleRepository.DeleteAsync(role);
            return IdentityResult.Success;
        }

        public void Dispose()
        {

        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var idInt = roleId.To<int>();
            return _roleRepository.FirstOrDefaultAsync(m => m.Id == idInt);

        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _roleRepository.FirstOrDefaultAsync(m => m.Code == normalizedRoleName);

        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Code);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Code);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            role.Code = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            role.Code = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(role, nameof(role));
            await _roleRepository.UpdateAsync(role);
            return IdentityResult.Success;
        }
    }
}
