using Nct.AspNetCore;
using Nct.AspNetCore.Configuration;
using Nct.DataAccess;
using Nct.Modularity;

namespace Nct.ApiJson
{
    [DependsOn<AspNetCoreModule,DataAccessModule>]
    public class ApiJsonModule:BaseModule
    {

        public override void OnPreInitialize(ApplicationInitContext context)
        {

            Configuration.Module.AspNetCore().CreateControllersForAppServices(typeof(ApiJsonModule).GetAssembly());


        }
    }
}
