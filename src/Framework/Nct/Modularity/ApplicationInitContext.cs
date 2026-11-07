using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nct.Modularity
{
    public class ApplicationInitContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public ApplicationInitContext(IServiceProvider serviceProvider)
        {
           // Check.NotNull(serviceProvider, nameof(serviceProvider));
            ServiceProvider = serviceProvider;
        }

    }
    public static class ApplicationInitContextExtension
    {
        public static IApplicationBuilder GetApplicationBuilder(this ApplicationInitContext context)
        {
            return context.ServiceProvider.GetRequiredService<IApplicationBuilder>();
        }

        public static IWebHostEnvironment GetEnvironment(this ApplicationInitContext context)
        {
            return context.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        }
    }

}
