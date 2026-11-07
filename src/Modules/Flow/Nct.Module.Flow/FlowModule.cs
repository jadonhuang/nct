using Nct.AspNetCore.Configuration;
using Nct.Modularity;
using Nct.Module.Core;
using Nct.Module.Flow.JobDefs;
using Nct.Scripting;

namespace Nct.Module.Form
{

    [DependsOn<FormContractsModule,CoreContractsModule, DataAccessModule>]
    [DependsOn<ScriptingModule>]
    public class FlowModule : BaseModule
    {
        public override void OnPreInitialize(ApplicationInitContext context)
        {
            Configuration.Module.AspNetCore().CreateControllersForAppServices(typeof(FlowModule).GetAssembly(), predicate: type => {

                return true;
            });
        }

        public override void OnAfterInitialize(ApplicationInitContext context)
        {
            base.IocManager.Using<IJobDefAppService>(app =>
            {
               app.RegisterAllJobs();
               app.SyncSystemJobToDb();

            });

        }
    }
}
