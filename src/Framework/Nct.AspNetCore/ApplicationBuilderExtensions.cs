using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nct.AspNetCore.EmbeddedResources;
using Nct.AspNetCore.Rewrite;
using Nct.Ioc;


namespace Nct.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNct(this IApplicationBuilder app)
        {
            var bootstrapper = app.ApplicationServices.GetRequiredService<Bootstrapper>();
            IocManager.Instance.Container.Register(Component.For<IApplicationBuilder>().Instance(app));
            bootstrapper.SetApplicationInitContext(new Modularity.ApplicationInitContext(app.ApplicationServices));
            bootstrapper.Initialize();

            var applicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(bootstrapper.Dispose);

            return app;

        }


        public static void UseEmbeddedFiles(this IApplicationBuilder app)
        {
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new EmbeddedResourceFileProvider(
                        app.ApplicationServices.GetRequiredService<IIocResolver>()
                    )
                }
            );
        }

        public static void UseUrlRewriter(this IApplicationBuilder app)
        {

            var options = app.ApplicationServices.GetRequiredService<RewriteOptionsProvider>().GetOptions();

            if (options == null) return;

            app.UseRewriter(options);


        }

        public static IHostBuilder UseCastleWindsor(this IHostBuilder hostBuilder)
        {

            return hostBuilder.ConfigureServices(services=>
            {
                services.AddSingleton(IocManager.Instance.Container);
            }).UseServiceProviderFactory(new WindsorServiceProviderFactory());
        }

    }
}
