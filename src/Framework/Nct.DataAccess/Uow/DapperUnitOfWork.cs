using Nct.Domain.Uow;
using Nct.Ioc;
using Nct.DataAccess.Configuration;
using Nct.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Extentions;
using Microsoft.Extensions.Options;

namespace Nct.DataAccess.Uow
{
    public class DapperUnitOfWork : BaseUnitOfWork, ITransientDependency
    {


        private DbContext _dbContext;

        private readonly IIocResolver _iocResolver;

        private ConnectionStrings _sonnectionStrings;

        public DapperUnitOfWork(IIocResolver iocResolver, IOptions<DbConnectionOptions> connectionOptions)
        {
            _iocResolver = iocResolver;
            _sonnectionStrings = connectionOptions.Value.ConnectionStrings;
        }

        public virtual TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext
        {
            if (_dbContext == null)
            {

                _dbContext = _iocResolver.Resolve<TDbContext>(new Dictionary<string, object> { { "connectionString", _sonnectionStrings.Default } });

                if (Options != null)
                {

                    if (Options.Timeout.HasValue)
                        _dbContext.Database.Timeout = Options.Timeout.Value.TotalSeconds.ToInt();

                    if (Options.IsTransactional == true)
                        _dbContext.Database.BeginTransaction(Options.IsolationLevel);

                }
            }

            return (TDbContext)_dbContext;

        }

        public override void Dispose()
        {

            if (IsDisposed) return;

            _iocResolver.Release(_dbContext);

            base.Dispose();


        }



        public override void Complete()
        {
            if (Options.IsTransactional == true && _dbContext != null)
            {
                _dbContext.Database.Transaction.Commit();
            }
        }
    }
}
