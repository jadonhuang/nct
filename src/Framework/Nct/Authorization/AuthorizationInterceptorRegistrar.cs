using System;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Nct.Helpers;
using Nct.Ioc;

namespace Nct.Authorization
{
    
    internal static class AuthorizationInterceptorRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            iocManager.Container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;            
        }

        private static void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            if (ReflectionHelper.HasAnyAttribute<AuthorizeExAttribute>(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AuthorizationInterceptor))); 
            }
        }

    
    }
}