using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.Configuration
{
    public class ModuleConfigurations 
    {
        public StartupConfiguration Configuration { get; private set; }

        public ModuleConfigurations(StartupConfiguration configuration)
        {

            Configuration = configuration;
        }
    }
}
