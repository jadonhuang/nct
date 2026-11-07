using Nct.Extentions;
using System.Collections.Generic;

namespace Nct.Http.ApiDescription
{
    public class ApplicationApiDescription
    {
        //不可以set的话无法反序列化??
        public Dictionary<string, ModuleApiDescription> Modules { get; set; }

        public ApplicationApiDescription()
        {

            Modules = new Dictionary<string, ModuleApiDescription>();
        }

        public ModuleApiDescription GetOrAddModule(string name)
        {
            return Modules.GetOrAdd(name, t => new ModuleApiDescription(name));

        }

    }
}
