using Nct.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.AspNetCore.Configuration
{
   public static class AspNetCoreConfigurationExtensions
    {
        public static AspNetCoreConfiguration AspNetCore(this ModuleConfigurations configurations)
        {
            return configurations.Configuration.Get<AspNetCoreConfiguration>();
        }
    }
}
