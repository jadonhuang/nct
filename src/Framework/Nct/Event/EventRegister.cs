using MediatR;
using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nct.Event
{
    public static class EventRegister
    {
        public static void RegisteAll(IReadOnlyList<Type> allTypes)
        {

  
            var notifyHandlerType = typeof(INotificationHandler<>);

            var requestHandlerType = typeof(IRequestHandler<,>);

            foreach (var type in allTypes)
            {

                if (type.IsGenericType) continue;

                var interfaces = type.GetInterfaces();

                if (interfaces.Length == 0) continue;

                DependencyLifeStyle lifeStyle = typeof(ISingletonDependency).IsAssignableFrom(type) ? DependencyLifeStyle.Singleton : DependencyLifeStyle.Transient;

                Type baseType = interfaces.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == notifyHandlerType);

                if (baseType != null)
                {
                    var genericType = notifyHandlerType.MakeGenericType(baseType.GetGenericArguments()[0]);
                    IocManager.Instance.RegisterIfNot(genericType, type, lifeStyle);
                    continue;
                }


                baseType = interfaces.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == requestHandlerType);

                if (baseType != null)
                {
                    var args = baseType.GetGenericArguments();
                    var genericType = requestHandlerType.MakeGenericType(args[0], args[1]);
                    IocManager.Instance.RegisterIfNot(genericType, type, lifeStyle);

                }




            }





        }


    }
}
