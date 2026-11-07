using Nct.Module.Flow.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine
{
    public class StepExecutionResultArgs
    {

        public string StepName { get; set; }

        public string StepId { get; set; }

        public bool Success { get; set; } = true;

        public string Exception { get; set; }

        public Dictionary<string, object> Outputs { get; set; } 

        public Dictionary<string, object> Inputs { get; set; } 
      
        public List<StepExecutionResultArgs> Children { get; set; } = [];




    }
}
