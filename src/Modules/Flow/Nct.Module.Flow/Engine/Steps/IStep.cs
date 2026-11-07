namespace Nct.Module.Flow.Engine.Steps
{
    public interface IStep : ITransientDependency, IDisposable
    {

        Task<IStepExecutionResult> Run(StepExecutionContext context);

    }
}
