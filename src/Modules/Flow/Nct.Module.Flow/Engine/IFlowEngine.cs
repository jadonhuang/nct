using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.FlowDefs;
using Nct.Module.Flow.Flows;


namespace Nct.Module.Flow.Engine
{
    public interface IFlowEngine : ITransientDependency, IDisposable
    {


        Task<bool> Run(StepExecutionContext context);

        List<IStepExecutionResult> Results { get; }

    }
}
