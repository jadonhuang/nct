using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Nct.Extentions
{
    public static class ServiceCollectionExtensions
    {

        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(T));
            if (serviceDescriptor != null)
            {
                return (T)(serviceDescriptor.IsKeyedService ? serviceDescriptor.KeyedImplementationInstance : serviceDescriptor.ImplementationInstance);
            }

            return default;
               
        }

        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {

         
            WebHostBuilderContext webHostBuilderContext = services.GetSingletonInstanceOrNull<WebHostBuilderContext>();
            if (webHostBuilderContext?.Configuration != null)
            {
                return webHostBuilderContext.Configuration as IConfigurationRoot;
            }


            HostBuilderContext singletonInstanceOrNull = services.GetSingletonInstanceOrNull<HostBuilderContext>();
            if (singletonInstanceOrNull?.Configuration != null)
            {
                return singletonInstanceOrNull.Configuration as IConfigurationRoot;
            }
       
            return services.GetSingletonInstanceOrNull<IConfiguration>();

        }

        public static IWebHostEnvironment GetHostingEnvironment(this IServiceCollection services)
        {

            return services.GetSingletonInstanceOrNull<IWebHostEnvironment>();
         
        }

    }
}
