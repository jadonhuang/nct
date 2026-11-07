using Nct.Http.ClientProxy;
using System;
using System.Reflection;

namespace Nct.AspNetCore.Configuration
{
    public class AspNetCoreConfiguration 
    {
    
        public ControllerAssemblySettingList ControllerAssemblySettings { get; }
        public ClientProxyAssebleySettings ClientProxyAssebleySettings { get;set; }

        public AspNetCoreConfiguration()
        {        
            ControllerAssemblySettings = [];

        }

        public void CreateControllersForAppServices(Assembly assembly, string moduleName = ControllerAssemblySetting.DefaultServiceModuleName, Func<Type, bool> predicate = null)
        {

            var setting = new ControllerAssemblySetting(moduleName, assembly);
            if (predicate != null)
            {
                setting.TypePredicate = predicate;

            }
            ControllerAssemblySettings.Add(setting);

        }


        public void CreateClientProxyForAppServices(Assembly assembly, string configName = ClientProxyAssemblySetting.DefaultName, Func<Type, bool> predicate = null)
        {

            var setting = new ClientProxyAssemblySetting(configName, assembly);
            if (predicate != null)
            {
                setting.TypePredicate = predicate;

            }
            ClientProxyAssebleySettings.TryAdd(assembly,setting);

        }

    }
}
