using Microsoft.Extensions.Options;
using Nct.DataAccess.Core;
using Nct.Ioc;
using System;

namespace Nct.DataAccess.Configuration
{
    public class DbConfiguration
    {


        private readonly IIocManager _iocManager;

        public Type DbContextType { get; private set; }

        public DbConfiguration(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        public void AddDbContext<TDbContext>(Action<DbConnectionOptions> action) where TDbContext : DbContext
        {
            _iocManager.RegisterIfNot<DbContext, TDbContext>(DependencyLifeStyle.Transient);
            DbContextType = typeof(TDbContext);
            var options = _iocManager.Resolve<IOptions<DbConnectionOptions>>();
            action(options.Value);

        }

        public void AddDbContext<TDbContext>() where TDbContext : DbContext
        {
            _iocManager.RegisterIfNot<DbContext, TDbContext>(DependencyLifeStyle.Transient);
            DbContextType = typeof(TDbContext);

        }


    }
}
