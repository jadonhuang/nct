using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nct.Modularity
{
    public class ServiceConfigurationContext
    {
        public IServiceCollection Services { get; set; }

        public ServiceConfigurationContext(IServiceCollection services)
        {
            Services = services;

        }

    }
}
