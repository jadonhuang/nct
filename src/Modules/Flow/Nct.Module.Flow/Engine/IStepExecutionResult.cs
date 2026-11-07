using Nct.Module.Flow.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine
{
    public interface IStepExecutionResult
    {

        [JsonIgnore]
        FlowNode[] NextNodes { get; set; }
        bool Success { get; set; }
        string Exception { get; set; }
        Dictionary<string, object> Outputs { get; set; }

        Dictionary<string, object> Inputs { get; set; }

        List<IStepExecutionResult> SubResults { get; set; }

        string StepName { get; set; }

        int Duration { get; set; }


    }
}
