using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Nct.AspNetCore.Configuration;
using Nct.AspNetCore.Session;
using Nct.Configuration;
using Nct.Extentions;
using Nct.Modularity;
using Nct.Session;
using System.Linq;

namespace Nct.AspNetCore
{


    [DependsOn<KernelModule>]
    public class AspNetCoreModule : BaseModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var webHostEnvironment = context.Services.GetHostingEnvironment();
            GlobalConfiguration.WebRootPath = webHostEnvironment.WebRootPath;
            GlobalConfiguration.ContentRootPath = webHostEnvironment.ContentRootPath;


        }

        public override void OnPreInitialize(ApplicationInitContext context)
        {

            IocManager.Register<AspNetCoreConfiguration>();
            Configuration.ReplaceService<IPrincipalAccessor, AspNetCorePrincipalAccessor>(Ioc.DependencyLifeStyle.Transient);

        }


        public override void OnAfterInitialize(ApplicationInitContext context)
        {
            var configuration = IocManager.Resolve<AspNetCoreConfiguration>();
            AddApplicationParts(configuration.ControllerAssemblySettings);

        }



        private void AddApplicationParts(ControllerAssemblySettingList controllerAssemblySettings)
        {

            var partManager = IocManager.Resolve<ApplicationPartManager>();
            var controllerAssemblies = controllerAssemblySettings.Select(s => s.Assembly).Distinct();
            foreach (var controllerAssembly in controllerAssemblies)
            {
                partManager.ApplicationParts.Add(new AssemblyPart(controllerAssembly));
            }


        }

    }
}
