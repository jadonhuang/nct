using Microsoft.AspNetCore.Builder;
using Nct.Modularity;
using Nct.Module.Core;
using Nct.Module.Form;

namespace Nct.Web
{

    [DependsOn<FlowModule, FormModule, CoreModule>]
    public class WebModule : BaseHostModule
    {
        protected override void UseMiddleware(ApplicationInitContext context)
        {
            var app = context.GetApplicationBuilder();
            app.UseMiddleware<AdminEntryMiddleware>();

        }

    }

}
