using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{

    public class JobAiModelNodeProps : NodeProps
    {
        public string ModelType { get; set; }

        public string ApiKey { get; set; }

        public string ApiUrl { get; set; }

        public string Prompt { get; set; }

        public double Temperature { get; set; } = 0.7;

        public int MaxTokens { get; set; } = 2000;

    
        public RetryConfig RetryConfig { get; set; }

        public override List<Variable> GetOutputVariables()
        {
            return [
                new() { Name = "text", Type = Consts.VariableType.String },
              ];
        }
    }
}
