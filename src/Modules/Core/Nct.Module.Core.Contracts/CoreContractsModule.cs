using Nct.AspNetCore;
using Nct.Identity;
using Nct.Modularity;
using Nct.SignalR;

namespace Nct.Module.Core
{


    [DependsOn<AspNetCoreModule,SignalRModule>]
    [DependsOn<IdentityModule>]
    public class CoreContractsModule : BaseModule
    {

      


    }




}
