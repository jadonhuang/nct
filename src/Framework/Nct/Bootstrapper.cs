using Microsoft.Extensions.DependencyInjection;
using Nct.Configuration;
using Nct.Extentions;
using Nct.Ioc;
using Nct.Logging;
using Nct.Modularity;
using System;

namespace Nct
{
    public class Bootstrapper : IDisposable
    {

        public Type StartupModule { get; }

        private readonly ILogger _logger;

        private bool isDisposed = false;

        private readonly IModuleManager _moduleManager;

        private readonly StartupConfiguration _startupConfiguration;

        private ApplicationInitContext  _applicationInitContext;
        private ServiceConfigurationContext _serviceConfigurationContext;

        public Bootstrapper(Type startupModule, IServiceCollection services)
        {

            Check.NotNull(startupModule, nameof(startupModule));
            StartupModule = startupModule;

            _serviceConfigurationContext = new ServiceConfigurationContext(services);

            _logger = new NLogLogger();
            services.AddSingleton(typeof(ILogger), _logger);

            _moduleManager = new ModuleManager(_logger);
            services.AddSingleton(typeof(IModuleManager), _moduleManager);

            _startupConfiguration = [];
            services.AddSingleton(_startupConfiguration);
          
        }

        public void SetApplicationInitContext(ApplicationInitContext context)
        {
            _applicationInitContext = context;
        }

        public static Bootstrapper Create<TStartupModule>(IServiceCollection services) where TStartupModule : BaseModule
        {

            return new Bootstrapper(typeof(TStartupModule), services);


        }


        public virtual void PreInitialize()
        {
            _moduleManager.Initialize(StartupModule, _serviceConfigurationContext);
       
        }



        public virtual void Initialize()
        {

            _logger.Debug("Modules Initializing...");

            try
            {
       
                _moduleManager.Start(_applicationInitContext);
                _logger.Debug("Modules Initialize Finish");

            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.ToString());
                throw;
            }

        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;

            _moduleManager.Unload();
        }
    }
}
