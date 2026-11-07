using Nct.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Http.ApiDescription
{
    public class ModuleApiDescription
    {
        public Dictionary<string, ControllerApiDescription> Controllers { get; set; }

        public string Name { get; }

        public ModuleApiDescription(string name)
        {
            Name = name;
            Controllers = new Dictionary<string, ControllerApiDescription>();
        }

        public ControllerApiDescription GetOrAddController(string name, string[] interfaces)
        {
            return Controllers.GetOrAdd(name, t => new ControllerApiDescription(name, interfaces));

        }

    }
}
