using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Ioc
{
    public class DisposableObjectWrapper<T> : IDisposable
    {


        private readonly IIocResolver _iocResolver;
        public T Object { get; private set; }

        public DisposableObjectWrapper(IIocResolver iocResolver, T obj)
        {
            _iocResolver = iocResolver;
            Object = obj;
        }

        public void Dispose()
        {
            _iocResolver.Release(Object);
        }
    }

    public class DisposableObjectWrapper : DisposableObjectWrapper<object>
    {

        public DisposableObjectWrapper(IIocResolver iocResolver, object obj) : base(iocResolver, obj)
        {
        }

    }

}
