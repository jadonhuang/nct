using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.AspNetCore.ScriptProxy
{
    public class ScriptProxyRequestDto
    {
        public string Module {  get; set; }

        public bool AddModulePrefix { get; set; }

    }
}
