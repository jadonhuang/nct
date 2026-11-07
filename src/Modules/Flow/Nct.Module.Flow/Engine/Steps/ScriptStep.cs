using Nct.Module.Flow.Engine.Models;
using Nct.Scripting;

namespace Nct.Module.Flow.Engine.Steps
{
    public class ScriptStep(Script script) : Step
    {
        private readonly Script _script = script;

        public override Task<IStepExecutionResult> Run(StepExecutionContext ctx)
        {
            var context = (ApprovalStepExecutionContext)ctx;
            var props = context.Node.Properties.To<ScriptNodeProps>();
            var flag = _script.Evaluate<bool>(props.Code, context);
            if (!flag)
            {
                return null;
            }

            var nextNode = context.FlowConfig.GetNextNode(context.Node);
            var result = Success(nextNode);
            return Task.FromResult(result);

        }
    }
}
