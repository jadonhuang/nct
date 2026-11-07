using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Nct.Modularity
{
   public interface IModuleManager
    {
        ModuleInfo StartupModule { get; }

        IReadOnlyList<ModuleInfo> Modules { get; }

        void Initialize(Type startupModule, ServiceConfigurationContext context);

        void Start(ApplicationInitContext context);

        void Unload();

        IReadOnlyList<Type> GetTypesOfAllAssemblies();

        public static IModuleManager Instance 
        {

            get 
            { 
               return IocManager.Instance.Resolve<IModuleManager>();
            
            }
        }



    }
}
