using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Http.ClientProxy
{
    public class ClientProxyAssemblySetting
    {

        public const string DefaultName = "Default";

        public string ConfigName { get; }

        public Assembly Assembly { get; }


        public Func<Type, bool> TypePredicate { get; set; }


        public ClientProxyAssemblySetting(string configName, Assembly assembly)
        {
            ConfigName = configName;
            Assembly = assembly;
            TypePredicate = type => true;
        }

    }
}
