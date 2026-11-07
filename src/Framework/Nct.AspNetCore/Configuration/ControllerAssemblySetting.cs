using System;
using System.Reflection;

namespace Nct.AspNetCore.Configuration
{
    public class ControllerAssemblySetting
    {
        public const string DefaultServiceModuleName = "";

        public string ModuleName { get; }

        public Assembly Assembly { get; }

       

        public Func<Type, bool> TypePredicate { get; set; }

   

        public ControllerAssemblySetting(string moduleName, Assembly assembly)
        {
            ModuleName = moduleName;
            Assembly = assembly;
            TypePredicate = type => true;
        }

    }
}
