using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using Nct.AspNetCore;
using Nct.Identity.Jwt;
using Nct.Modularity;
using Nct.SignalR;
using Nct.Web.AppStart;
using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Nct.Module.Core
{

    public class BaseHostModule : BaseModule
    {

        const string DefaultPolicyName = "CorsPolicy";
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            services.AddSignalR().AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;

            });
            services.AddControllersWithViews(options =>
            {
                options.ModelValidatorProviders.Clear();
            });

            AuthenConfigurer.Configure(services, services.GetConfiguration());

            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = int.MaxValue;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(DefaultPolicyName,
                   builder => builder
                       .SetIsOriginAllowed(_ => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials()
                      );

            });


            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
            });

        }

        protected virtual void UseMiddleware(ApplicationInitContext context)
        { 
        
        
        }


        public override void OnAfterInitialize(ApplicationInitContext context)
        {

            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

                app.UseWhen(
                         context => !context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
                         a => a.UseExceptionHandler("/Error")
                     );


            }

            app.UseStatusCodePagesWithReExecute("/ErrorWithCode/{0}");

            app.UseUrlRewriter();

            UseMiddleware(context);
           
            app.UseStaticFiles();

            app.UseEmbeddedFiles();

            app.UseRouting();

            app.UseCors(DefaultPolicyName);

            app.UseAuthentication();

            app.UseJwtTokenMiddleware();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<CommonHub>("/signalr");

            });

        }
    }

}
