using Nct.Modularity;
using Nct.Module.Core;
using Nct.Module.Form;
using Nct.AspNetCore.Configuration;
using Nct.Extentions;




[DependsOn<FlowModule>]
public class HostModule : BaseHostModule
{

    public override void OnPreInitialize(ApplicationInitContext context)
    {
        Configuration.Module.AspNetCore().CreateClientProxyForAppServices(typeof(CoreContractsModule).GetAssembly());
        Configuration.Module.AspNetCore().CreateClientProxyForAppServices(typeof(FormContractsModule).GetAssembly(),"Form");
    }

}


