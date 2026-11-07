using Nct.Application;
using Nct.AspNetCore.Configuration;
using Nct.Helpers;
using Nct.Ioc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace Nct.AspNetCore.DynamicApi
{
    public class AppServiceControllerFeatureProvider : ControllerFeatureProvider
    {



        protected override bool IsController(TypeInfo typeInfo)
        {

            if (!base.IsController(typeInfo))
            {
                var type = typeInfo.AsType();

                if (!typeof(IApplicationService).IsAssignableFrom(type) || !typeInfo.IsPublic || typeInfo.IsAbstract || typeInfo.IsGenericType)
                {
                    return false;
                }

                var remoteServiceAttr = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(typeInfo);

                if (remoteServiceAttr != null && !remoteServiceAttr.IsEnabled)
                {
                    return false;
                }

                var configuration = IocManager.Instance.Resolve<AspNetCoreConfiguration>().ControllerAssemblySettings.GetOrNull(type);
                return configuration != null && configuration.TypePredicate(type);
            }

            return true;

        }

    }
}
