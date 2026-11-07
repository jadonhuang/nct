using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{
    public class JobScriptNodeProps : NodeProps
    {

        public List<Variable> Variables { get; set; } = [];

        public List<Variable> Outputs { get; set; } = [];

        public RetryConfig RetryConfig { get; set; }

        public string Code { get; set; }


        public override List<Variable> GetOutputVariables()
        {
            return Outputs;
        }



    }
}
