using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Ioc
{
   public static class IocResolverExtensions
    {

        public static DisposableObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver, IDictionary<string, object> args)
        {
            return new DisposableObjectWrapper<T>(iocResolver, iocResolver.Resolve<T>(args));
        }


        public static DisposableObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver, Type type)
        {
            return new DisposableObjectWrapper<T>(iocResolver, (T)iocResolver.Resolve(type));
        }

        public static DisposableObjectWrapper ResolveAsDisposable(this IIocResolver iocResolver, Type type)
        {
            return new DisposableObjectWrapper(iocResolver, iocResolver.Resolve(type));
        }

        public static DisposableObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver)
        {
            return new DisposableObjectWrapper<T>(iocResolver, iocResolver.Resolve<T>());
        }


        public static void Using<T>(this IIocResolver iocResolver, Action<T> action)
        {
            using (var wrapper = iocResolver.ResolveAsDisposable<T>())
            {
                action(wrapper.Object);
            }
        }

    
        public static TReturn Using<TService, TReturn>(this IIocResolver iocResolver, Func<TService, TReturn> func)
        {
            using (var obj = iocResolver.ResolveAsDisposable<TService>())
            {
                return func(obj.Object);
            }
        }


    }
}
