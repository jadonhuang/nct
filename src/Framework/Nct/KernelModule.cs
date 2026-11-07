using Castle.MicroKernel.Registration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nct.Async;
using Nct.Cache.Redis;
using Nct.Domain.Uow;
using Nct.EmbeddedResources;
using Nct.Event;
using Nct.Http.ClientProxy;
using Nct.Http.ContentRequest;
using Nct.Ioc;
using Nct.Jobs;
using Nct.MessageQueue;
using Nct.Modularity;
using Nct.ObjectMapper;
using System.Collections.Generic;
using System.Linq;

namespace Nct
{
    public class KernelModule : BaseModule
    {


        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<RedisOptions>();
            Configure<ClientProxyOptions>();
            context.Services.AddHttpClient(ContentRequest.ClientName);
        }

        private void RegisterAllAssembliesByConvention()
        {

            foreach (var module in IModuleManager.Instance.Modules)
            {
                IocManager.RegisterAssemblyByConvention(module.Assembly);

            }

        }

        public override void OnPreInitialize(ApplicationInitContext context)
        {
            IocManager.Register<ClientProxyAssebleySettings>();
            IocManager.Container.Register(Component.For<IMediator>().ImplementedBy<Mediator>());

            IocManager.Container.Register(Component.For<ServiceFactory>().UsingFactoryMethod<ServiceFactory>(k => (type =>
            {
                var enumerableType = type
                    .GetInterfaces()
                    .Concat(new[] { type })
                    .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                return enumerableType != null ? k.ResolveAll(enumerableType.GetGenericArguments()[0]) : k.Resolve(type);
            })));

            AddInterceptorRegistrars();

        }


        private void AddInterceptorRegistrars()
        {
            UowInterceptorRegistrar.Register(IocManager);

        }

        public override void OnInitialize(ApplicationInitContext context)
        {

            foreach (var action in Configuration.ServiceReplaceActions.Values)
            {
                action();

            }

            var allTypes = IModuleManager.Instance.GetTypesOfAllAssemblies();
            EventRegister.RegisteAll(allTypes);
            AutoMapperRegister.RegisteAll(allTypes);

            AddRedis();

            RegisterAllAssembliesByConvention();

            RegisterInterceptors();

        }


        private void AddRedis()
        {

            if (IocManager.Resolve<IOptions<RedisOptions>>().Value.Redis != null)
            {
                Configuration.Caching.UseRedis();
            }

        }


        private void RegisterInterceptors()
        {
            IocManager.Register(typeof(AsyncDeterminationInterceptor<UnitOfWorkInterceptor>), DependencyLifeStyle.Transient);

        }


        public override void OnAfterInitialize(ApplicationInitContext context)
        {

            IocManager.Resolve<IEmbeddedResourceManager>().SetResourceAssemblies(IModuleManager.Instance.Modules.Select(m => m.Assembly).ToList());

            var allTypes = IModuleManager.Instance.GetTypesOfAllAssemblies();

            Configuration.Job.UseQuartz(allTypes);

            MessageQueueServiceBuilder.Create().Register(allTypes).Build();
            ClientProxyInterceptorRegister.RegisteAll(IocManager.Resolve<ClientProxyAssebleySettings>());

        }


        public override void OnUnload()
        {

            IocManager.ResolveOrNull<IJobWorker>()?.Stop();

        }

    }
}
