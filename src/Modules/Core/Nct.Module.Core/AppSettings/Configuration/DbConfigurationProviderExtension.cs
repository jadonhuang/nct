using Microsoft.Extensions.Configuration;
using Nct.DataAccess;
using Nct.DataAccess.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Core.AppSettings
{
   public static class DbConfigurationProviderExtension
    {
        public static IConfigurationBuilder AddDbConfiguration(this IConfigurationBuilder builder)
        {
            return builder.Add(new DbConfigurationSource());
        }

    }
}
