using Nct.Module.Flow.Flows;

namespace Nct.Module.Flow.Engine.Steps
{
    public class StartStep : Step
    {

        public IRepository<FlowTask> TaskRep { get; set; }

        public override async Task<IStepExecutionResult> Run(StepExecutionContext ctx)
        {

            var context = (ApprovalStepExecutionContext)ctx;
            var node = context.Node;
            var task = new FlowTask
            {
                NodeName = "开始",
                NodeId = node.Id,
                FlowId = context.Flow.Id,
                Status = Consts.TaskStatus.Passed,
            };

            await TaskRep.InsertAsync(task);

            var nextNode = context.FlowConfig.GetNextNode(node);
             return Success(nextNode);



        }
    }
}
