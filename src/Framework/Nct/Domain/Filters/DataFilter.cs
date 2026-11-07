using Nct.Ioc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Nct.Domain.Filters
{


    public class DataFilter : IDataFilter, ISingletonDependency
    {

       
        private readonly ConcurrentDictionary<Type, object> _filters;

        private readonly IIocResolver _iocResolver;

        public DataFilter(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
            _filters = new ConcurrentDictionary<Type, object>();
        }

        public IDisposable Enable<TFilter>() where TFilter : class
        {
            return GetFilter<TFilter>().Enable();
        }

        public IDisposable Disable<TFilter>() where TFilter : class
        {
            return GetFilter<TFilter>().Disable();
        }

        public bool IsEnabled<TFilter>() where TFilter : class
        {
            return GetFilter<TFilter>().IsEnabled;
        }

        private IDataFilter<TFilter> GetFilter<TFilter>() where TFilter : class
        {
            return _filters.GetOrAdd(
                typeof(TFilter),
                t =>
                {
                    return _iocResolver.Resolve<IDataFilter<TFilter>>();
                }
            ) as IDataFilter<TFilter>;
        }
    }

    public class DataFilter<TFilter> : IDataFilter<TFilter>, ISingletonDependency where TFilter : class
    {
        public bool IsEnabled
        {
            get
            {
                EnsureInitialized();
                return _filter.Value.IsEnabled;
            }
        }


        private readonly AsyncLocal<DataFilterState> _filter;

        public DataFilter()
        {
            _filter = new AsyncLocal<DataFilterState>();
        }

        public IDisposable Enable()
        {
            if (IsEnabled)
            {
                return DisposeAction.Empty;
            }

            _filter.Value.IsEnabled = true;

            return new DisposeAction(() => Disable());
        }

        public IDisposable Disable()
        {
            if (!IsEnabled)
            {
                return DisposeAction.Empty;
            }

            _filter.Value.IsEnabled = false;

            return new DisposeAction(() => Enable());
        }

        private void EnsureInitialized()
        {
            if (_filter.Value != null)
            {
                return;
            }

            _filter.Value = new DataFilterState(true);
        }
    }
}
