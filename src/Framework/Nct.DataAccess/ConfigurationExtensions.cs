using Castle.MicroKernel.Registration;
using Nct.Configuration;
using Nct.Ioc;
using Nct.DataAccess.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.DataAccess
{
    public static class ConfigurationExtensions
    {
        public static DbConfiguration Db(this ModuleConfigurations configurations)
        {
            return configurations.Configuration.Get<DbConfiguration>(); 
        }

       
    }
}
