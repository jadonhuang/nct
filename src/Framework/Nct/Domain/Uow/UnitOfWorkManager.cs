using Nct.Ioc;
using System;
using System.Threading;


namespace Nct.Domain.Uow
{
    public class UnitOfWorkManager : IUnitOfWorkManager, ITransientDependency
    {

        private readonly IIocResolver _iocResolver;

        private readonly static AsyncLocal<IUnitOfWork> AsyncLocalUow = new AsyncLocal<IUnitOfWork>();

        public IUnitOfWork Current => GetCurrentUow();

        public UnitOfWorkManager(IIocResolver iocResolver)
        {

            _iocResolver = iocResolver;
        }



        private static IUnitOfWork GetCurrentUow()
        {
            var uow = AsyncLocalUow.Value;
            if (uow == null)
            {
                return null;
            }

            if (uow.IsDisposed)
            {
                AsyncLocalUow.Value = null;
                return null;
            }

            return uow;
        }



        public IUnitOfWork Begin(UnitOfWorkOptions options)
        {
            var current = Current;
            if (current != null)
            {
                if (current.Options.IsTransactional == true || options.IsTransactional == false)
                    return new InnerUnitOfWork();
            }

            var uow = _iocResolver.Resolve<IUnitOfWork>();

            uow.Begin(options);

            uow.OnDisposed += (sender, e) =>
            {
                _iocResolver.Release(uow);
            };

            AsyncLocalUow.Value = uow;
            return uow;

        }

        public IUnitOfWork Begin(int? timeout = null)
        {

            var options = new UnitOfWorkAttribute().CreateOptions();
            if (timeout.HasValue)
            {
                options.Timeout = TimeSpan.FromSeconds(timeout.Value);
            }
            options.IsTransactional = true;

            return this.Begin(options);

        }
    }
}
