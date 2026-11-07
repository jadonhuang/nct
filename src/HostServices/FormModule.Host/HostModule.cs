using Nct.Modularity;
using Nct.Module.Core;
using Nct.Module.Form;
using Nct.AspNetCore.Configuration;
using Nct.Extentions;

[DependsOn<FormModule>]
public class HostModule : BaseHostModule
{
    public override void OnPreInitialize(ApplicationInitContext context)
    {
        Configuration.Module.AspNetCore().CreateClientProxyForAppServices(typeof(CoreContractsModule).GetAssembly());

    }

}


