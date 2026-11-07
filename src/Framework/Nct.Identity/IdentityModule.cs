using Nct.AspNetCore;
using Nct.DataAccess;
using Nct.Identity.Jwt.Configuration;
using Nct.Modularity;

namespace Nct.Identity
{
    [DependsOn<DataAccessModule,AspNetCoreModule>]
    public class IdentityModule:BaseModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AuthenOptions>();
        }
    }
}
