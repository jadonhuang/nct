using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Nct.Modularity
{
    public class ModuleInfo
    {


        public ModuleInfo([NotNull] Type type, [NotNull] BaseModule instance)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(instance, nameof(instance));

            Type = type;
            Instance = instance;
            Assembly = Type.GetTypeInfo().Assembly;
            Dependencies = new List<ModuleInfo>();

        }

        public Assembly Assembly { get; }

        public Type Type { get; }

        public BaseModule Instance { get; }

        public List<ModuleInfo> Dependencies { get; }

    }
}
