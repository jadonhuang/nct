using Nct.Configuration;
using Nct.Extentions;
using Nct.Ioc;
using Nct.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nct.Modularity
{
    public class ModuleManager : IModuleManager
    {


        public ModuleInfo StartupModule { get; private set; }

        public IReadOnlyList<ModuleInfo> Modules => _modules.ToImmutableList();

        private ModuleCollection _modules;

        private Lazy<List<Type>> _allTypes;


        private readonly ILogger _logger;

       

        public ModuleManager(ILogger logger)
        {
            _logger = logger;
            _allTypes = new Lazy<List<Type>>(() =>
            {
                return Modules.SelectMany(m => m.Assembly.GetTypes()).ToList();

            });
        }


        public virtual void Initialize(Type startupModuleType, ServiceConfigurationContext context)
        {
            _modules = new ModuleCollection(startupModuleType);
            LoadAllModules (context);
            ConfigureServices(context);

        }

        public virtual void Unload()
        {

            _logger.Debug("Modules Unload");

            var sortedModules = _modules.GetSortedListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(m => m.Instance.OnUnload());
        }


        private void ConfigureServices(ServiceConfigurationContext context)
        {
            var sortedModules = _modules.GetSortedListByDependency();
            sortedModules.ForEach(m => m.Instance.PreConfigureServices(context));
            sortedModules.ForEach(m => m.Instance.ConfigureServices(context));

        }


        public virtual void Start(ApplicationInitContext context)
        {

            var sortedModules = _modules.GetSortedListByDependency();
            sortedModules.ForEach(m => m.Instance.OnPreInitialize(context));
            sortedModules.ForEach(m => m.Instance.OnInitialize(context));
            sortedModules.ForEach(m => m.Instance.OnAfterInitialize(context));
            sortedModules.ForEach(m =>
            {
                _logger.Debug(m.Type.Name + " started");

            });

        }

        private void LoadAllModules(ServiceConfigurationContext context)
        {

            _logger.Debug("Loading  modules...");

            var moduleTypes = BaseModule.FindDependedModuleTypesRecursively(_modules.StartupModuleType).Distinct().ToList();

            _logger.Debug($"Found {moduleTypes.Count} modules in total.");

            var startupConfiguration = context.Services.GetSingletonInstanceOrNull<StartupConfiguration>();
                
            foreach (var type in moduleTypes)
            {

                var instance = (BaseModule)Activator.CreateInstance(type);
                instance.ServiceConfigurationContext = context;
                instance.IocManager=IocManager.Instance;
                instance.Configuration = startupConfiguration;
                instance.Logger = _logger;

                var moduleInfo = new ModuleInfo(type, instance);

                _modules.Add(moduleInfo);

                if (type == _modules.StartupModuleType)
                {
                    StartupModule = moduleInfo;
                }

            }

            _modules.EnsureKernelModuleToBeFirst();
            _modules.EnsureStartupModuleToBeLast();

            SetDependencies();

            _logger.Debug("Modules Loaded");


        }

        private void SetDependencies()
        {
            foreach (var module in _modules)
            {
                foreach (var dependedModuleType in BaseModule.FindDependedModuleTypes(module.Type))
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);

                    if (!module.Dependencies.Any(dm => dm.Type == dependedModuleType))
                    {
                        module.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }

        public IReadOnlyList<Type> GetTypesOfAllAssemblies()
        {
            return _allTypes.Value.ToImmutableList();
        }


    }
}
