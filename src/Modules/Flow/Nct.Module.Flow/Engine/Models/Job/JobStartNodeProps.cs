using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{
    public class JobStartNodeProps : NodeProps
    {

        public List<Variable> Variables { get; set; } = [];


        public override List<Variable> GetOutputVariables()
        {
            return [

                  new() { Name = "sys.user_id", Type = Consts.VariableType.Number },
                new() { Name = "sys.user_name", Type = Consts.VariableType.String },
                new() { Name = "sys.job_id", Type = Consts.VariableType.Number },
                .. Variables
                ];
        }


    }
}
