using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{
    public class JobIterationNodeProps : NodeProps
    {

        public string Input { get; set; }

        public string Output  { get; set; }
      
       public string ErrHandleMode { get; set; }

       public string[] Children { get; set; }


        public override List<Variable> GetOutputVariables()
        {
            return [new Variable { Name = "item", Type = Consts.VariableType.ArrayString }
              ,new Variable { Name = "index", Type = Consts.VariableType.Number }

            ];
        }

    }
}
