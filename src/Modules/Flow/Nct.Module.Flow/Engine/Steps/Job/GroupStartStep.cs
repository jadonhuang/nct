namespace Nct.Module.Flow.Engine.Steps.Job
{
    public class GroupstartStep : Step
    {
        public override Task<IStepExecutionResult> Run(StepExecutionContext context)
        {
            var result = Success(context.FlowConfig.GetNextNodes(context.Node));
            return Task.FromResult(result);
        }

    }
}
