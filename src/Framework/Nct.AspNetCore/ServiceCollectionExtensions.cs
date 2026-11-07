using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Nct.AspNetCore.Authorization;
using Nct.AspNetCore.DynamicApi;
using Nct.AspNetCore.EmbeddedResources;
using Nct.AspNetCore.Validation;
using Nct.Ioc;
using Nct.Json;
using Nct.Localization;
using System;

namespace Nct.AspNetCore
{



    public static class ServiceCollectionExtensions
    {


        public static void AddNct<TStartupModule>(this IServiceCollection services) where TStartupModule : Modularity.BaseModule
        {

            var bootstrapper = Bootstrapper.Create<TStartupModule>(services);
            services.AddSingleton(bootstrapper);
            bootstrapper.PreInitialize();
            ConfigureAspNetCore(services);
          
        }


        private static void ConfigureAspNetCore(IServiceCollection services)
        {


            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddTransient<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddTransient<IAuthorizationHandler, DefaultPermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, DefaultRoleAuthorizationHandler>();

            services.AddSingleton<IStringLocalizerFactory, DefaultStringLocalizerFactory>();
            services.AddTransient(typeof(IStringLocalizer), typeof(DefaultStringLocalizer));

            //use to di to create controller
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());



            var partManager = services.GetSingletonServiceOrNull<ApplicationPartManager>();
            partManager?.FeatureProviders.Add(new AppServiceControllerFeatureProvider());

            services.Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.AddNct(services);


            });


            services.Insert(0,
               ServiceDescriptor.Singleton<IConfigureOptions<MvcRazorRuntimeCompilationOptions>>(
                   new ConfigureOptions<MvcRazorRuntimeCompilationOptions>(
                       (options) =>
                       {
                           options.FileProviders.Add(new EmbeddedResourceViewFileProvider(IocManager.Instance));
                       }
                   )
               )
           );


            services.Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DataTableConverter());

            });

            services.AddSingleton<IValidationAttributeAdapterProvider,
                 CustomValidateAttributeAdapterProvider>();


        }
    }
}
