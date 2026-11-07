using Microsoft.Extensions.DependencyInjection;
using Nct.Configuration;
using Nct.Extentions;
using Nct.Ioc;
using Nct.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nct.Modularity
{
    public abstract class BaseModule
    {

        public ILogger Logger { get; set; }

        public IIocManager IocManager { get; set; }


        public StartupConfiguration Configuration { get; set; }


        public ServiceConfigurationContext ServiceConfigurationContext { get; set; }

     

        public static bool IsModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(BaseModule).IsAssignableFrom(type);

        }


        protected void Configure<TOptions>(Action<TOptions> configureOptions) where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure(configureOptions);
        }

    
        protected void Configure<TOptions>() where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure<TOptions>(ServiceConfigurationContext.Services.GetConfiguration());
        }


        public virtual void PreConfigureServices(ServiceConfigurationContext context)
        {
            
        }


        public virtual void ConfigureServices(ServiceConfigurationContext  context)
        {
           
        }

        public virtual void OnPreInitialize(ApplicationInitContext context)
        {

        }


        public virtual void OnInitialize(ApplicationInitContext context)
        {

        }

        public virtual void OnAfterInitialize(ApplicationInitContext context)
        {

        }


        public virtual void OnUnload()
        {


        }


        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsModule(moduleType))
            {
                throw new ArgumentException($"{moduleType.AssemblyQualifiedName} is not an module");
            }

            var moduleTypes = new List<Type>();

            var attrs = moduleType.GetCustomAttributes();

            foreach (var attr in attrs)
            {

                if (attr is DependsOnAttribute depends)
                {
                    var dependsTypes = depends.DependedModuleTypes.Where(IsModule).ToList();
                    if (dependsTypes.Count != 0)
                    {
                        moduleTypes.AddRange(dependsTypes);
                    }
                }
                else
                {
                    var attrType = attr.GetType();

                    if (attrType.IsGenericType && (attrType.GetGenericTypeDefinition() == typeof(DependsOnAttribute<>)
                        || attrType.GetGenericTypeDefinition() == typeof(DependsOnAttribute<,>)
                        || attrType.GetGenericTypeDefinition() == typeof(DependsOnAttribute<,,>)
                        ))
                    {

                        var genericTypes = attrType.GetGenericArguments();
                        moduleTypes.AddRange(genericTypes);
                    }


                }


            }

          
            return moduleTypes;
        }

        public static List<Type> FindDependedModuleTypesRecursively(Type moduleType)
        {

            if (!IsModule(moduleType))
            {
                throw new ArgumentException($"{moduleType.AssemblyQualifiedName} is not an module");
            }

            var moduleTypes = new List<Type>();
            AddModuleAndDependenciesRecursively(moduleTypes, moduleType);

            moduleTypes.AddIfNotContains(typeof(KernelModule));
            return moduleTypes;
        }

        private static void AddModuleAndDependenciesRecursively(List<Type> moduleTypes, Type module)
        {

            if (moduleTypes.Contains(module))
            {
                return;
            }

            moduleTypes.Add(module);
            var dependedModules = FindDependedModuleTypes(module);
            foreach (var dependedModule in dependedModules)
            {
                AddModuleAndDependenciesRecursively(moduleTypes, dependedModule);
            }
        }



    }
}
