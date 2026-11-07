using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Http.ScriptProxy
{
    public interface IApiScriptBuilder
    {
        string Build(string module, bool addModulePrefix);
    }
}
