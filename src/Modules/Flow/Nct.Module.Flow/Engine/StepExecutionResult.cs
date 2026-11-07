using Nct.Module.Flow.Engine.Models;
using System.Text.Json.Serialization;

namespace Nct.Module.Flow.Engine
{
    public class StepExecutionResult : IStepExecutionResult
    {
        public bool Success { get; set; } = true;

        public Dictionary<string, object> Outputs { get; set; } =[ ];
         
        public Dictionary<string, object> Inputs { get; set; } =[];

        public FlowNode[] NextNodes { get; set; } = [];

        public string Exception { get; set; }

        public List<IStepExecutionResult> SubResults { get; set; } = [];

        public string StepName { get; set; }

        public int Duration { get; set; }


    }
}
