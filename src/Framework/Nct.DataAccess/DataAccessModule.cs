using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Options;
using Nct.DataAccess.Configuration;
using Nct.DataAccess.Core;
using Nct.DataAccess.Log;
using Nct.Domain.Filters;
using Nct.Domain.Repositories;
using Nct.Modularity;

namespace Nct.DataAccess
{

    [DependsOn<KernelModule>]
    public class DataAccessModule : BaseModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<DbConnectionOptions>();
            Configure<DbLogOptions>();
        }


        public override void OnPreInitialize(ApplicationInitContext context)
        {
            IocManager.Container.Register(Component.For(typeof(IDataFilter<>)).ImplementedBy(typeof(DataFilter<>)).LifestyleSingleton());
            IocManager.Register<DbConfiguration>();
            IocManager.Register<RepositoryMatchingStrategy>();
            Configuration.Module.Db().AddDbContext<DapperDbContext>();
        }


        public override void OnInitialize(ApplicationInitContext context)
        {


            if (IocManager.Resolve<IOptions<DbLogOptions>>().Value.DbLog != null)
            {
                DbInterception.Add(IocManager.Resolve<IDbCommandInterceptor>());
            }

            var repMatchingStrategy = IocManager.Resolve<RepositoryMatchingStrategy>();

            IocManager.Container.Register(
                       Component
                           .For(typeof(IRepository<>))
                           .ImplementedBy(typeof(Repository<,>), repMatchingStrategy)
                           .LifestyleTransient()
                   );

            IocManager.Container.Register(
                      Component
                          .For(typeof(IRepository<,>))
                          .ImplementedBy(typeof(Repository<,,>), repMatchingStrategy)
                          .LifestyleTransient()
                  );


        }

    }
}
