using Microsoft.AspNetCore.Authentication;
using Nct.OAuth;
using Nct.OAuth.Weixin;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class WeixinAuthenticationExtensions
    {
        public static AuthenticationBuilder AddWeixin(
       [NotNull] this AuthenticationBuilder builder,  
       [NotNull] Action<WeixinAuthenticationOptions> configuration)
        {
            return builder.AddOAuth<WeixinAuthenticationOptions, WeixinAuthenticationHandler>(WeixinAuthenticationDefaults.AuthenticationScheme, configuration);
        }
    }
}
