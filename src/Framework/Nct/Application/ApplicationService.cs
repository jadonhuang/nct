using Nct.Cache;
using Nct.Domain.Filters;
using Nct.Domain.Uow;
using Nct.Ioc;
using Nct.Logging;
using Nct.Session;


namespace Nct.Application
{
    public abstract class ApplicationService : IApplicationService, ITransientDependency,IUnitOfWorkEnabled
    {

        public static readonly string[] Suffixs = ["AppService", "ApplicationService"];

        public ILogger Logger { get; set; }

        public ICache Cache { get; set; }

        //[DoNotWire]
        public ISession Session { get; set; }


        public IDataFilter DataFilter { get; set; }


        public virtual RetResult Success(object data = null, string msg = null)
        {
            return new RetResult().Success(data, msg);

        }

        public virtual RetResult Error(string msg = null, int statusCode = 500)
        {
            return new RetResult().Error(msg, statusCode);
        }

    }
}
