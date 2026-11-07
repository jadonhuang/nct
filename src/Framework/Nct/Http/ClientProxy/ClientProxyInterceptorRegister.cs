using Castle.MicroKernel.Registration;
using Nct.Application;
using Nct.Async;
using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Http.ClientProxy
{
    public static class ClientProxyInterceptorRegister
    {

        public static void RegisteAll(ClientProxyAssebleySettings settings)
        {
  
            foreach (var setting in settings.Values)
            {

                foreach (var type in setting.Assembly.GetTypes().Where(setting.TypePredicate).Where(IsSuitableForClientProxy))
                {

                    var interceptorType = typeof(ClientProxyInterceptor<>).MakeGenericType(type);
                    IocManager.Instance.Register(interceptorType, DependencyLifeStyle.Transient);
                    var interceptorAdapterType = typeof(AsyncDeterminationInterceptor<>).MakeGenericType(interceptorType);
                    IocManager.Instance.Register(interceptorAdapterType, DependencyLifeStyle.Transient);                  
                    IocManager.Instance.Container.Register(Component.For(type).Interceptors(interceptorAdapterType).LifestyleTransient());

                }
                
            
            }
        
        }

        private static bool IsSuitableForClientProxy(Type type)
        {

            return type.IsInterface
                && type.IsPublic
                && !type.IsGenericType
                && typeof(IApplicationService).IsAssignableFrom(type);

        }



    }
}
