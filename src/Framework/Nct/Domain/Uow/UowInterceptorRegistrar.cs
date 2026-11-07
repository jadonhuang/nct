using System;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Nct.Async;
using Nct.Helpers;
using Nct.Ioc;

namespace Nct.Domain.Uow
{

    internal static class UowInterceptorRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            iocManager.Container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        private static void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            var imp = handler.ComponentModel.Implementation;

            if (ShouldIntercept(imp))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AsyncDeterminationInterceptor<UnitOfWorkInterceptor>)));
            }
        }

        private static bool ShouldIntercept(Type type)
        {
            return UnitOfWorkHelper.IsUnitOfWorkType(type.GetTypeInfo());
        }




    }
}