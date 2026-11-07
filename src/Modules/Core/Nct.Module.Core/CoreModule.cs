using Microsoft.Extensions.Configuration;
using Nct.ApiJson;
using Nct.AspNetCore.Configuration;
using Nct.DataAccess;
using Nct.Modularity;

namespace Nct.Module.Core
{


    [DependsOn<CoreContractsModule, DataAccessModule>]
    [DependsOn<ApiJsonModule>]
    public class CoreModule : BaseModule
    {

        public override void OnPreInitialize(ApplicationInitContext context)
        {

            Configuration.Module.AspNetCore().CreateControllersForAppServices(typeof(CoreModule).GetAssembly());

        }

        public override void OnInitialize(ApplicationInitContext context)
        {

            ((IConfigurationRoot)IocManager.Resolve<IConfiguration>()).Reload();
            
        }




    }




}
