using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Nct.Identity.Entity;
using Nct.Identity.Jwt.Configuration;
using Nct.Identity.Store;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Web.AppStart
{
    public static class AuthenConfigurer
    {

        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {


            services.TryAddScoped<UserStore<UserBase, RoleBase>>();
            services.TryAddScoped(typeof(IUserStore<UserBase>), provider => provider.GetService(typeof(UserStore<UserBase, RoleBase>)));
            services.TryAddScoped<RoleStore<RoleBase>>();
            services.TryAddScoped(typeof(IRoleStore<RoleBase>), provider => provider.GetService(typeof(RoleStore<RoleBase>)));
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<UserBase>>();

            services
          .AddIdentity<UserBase, RoleBase>(options =>
          {

              options.Password.RequireDigit = false;
              options.Password.RequiredLength = 6;
              options.Password.RequireNonAlphanumeric = false;
              options.Password.RequireUppercase = false;
              options.Password.RequireLowercase = false;
              options.Password.RequiredUniqueChars = 0;
              options.Lockout.MaxFailedAccessAttempts = 5;
              options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
              options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Name;

          })

          .AddDefaultTokenProviders();

            var authenConfiguration = configuration.Get<AuthenOptions>().Authentication.JwtBearer.ToJwtConfigurationModel();

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {

                    options.Audience = authenConfiguration.Audience;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = authenConfiguration.SecurityKey,


                        ValidateIssuer = true,
                        ValidIssuer = authenConfiguration.Issuer,


                        ValidateAudience = true,
                        ValidAudience = authenConfiguration.Audience,

                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {

                            if (context.Request.Cookies.TryGetValue("token", out string token))
                            {
                                context.Token = token;
                            }
                            else if (context.Request.Query.TryGetValue("token", out StringValues tokens))
                            {
                                context.Token = tokens.ToString();
                            }
                            return Task.CompletedTask;

                        }
                    };

                });



        }




    }
}
