using Nct.AspNetCore.Configuration;
using Nct.Modularity;
using Nct.Module.Core;
using Nct.Scripting;

namespace Nct.Module.Form
{


    [DependsOn<FormContractsModule,CoreContractsModule, DataAccessModule>]
    [DependsOn<ScriptingModule>]
    public class FormModule : BaseModule
    {
        public override void OnPreInitialize(ApplicationInitContext context)
        {
            Configuration.Module.AspNetCore().CreateControllersForAppServices(typeof(FormModule).GetAssembly(), predicate: type => {

                return true;
            });
        }
    }
}
