using Nct.Module.Flow.Flows;


namespace Nct.Module.Flow.Engine
{
    public class ApprovalStepExecutionContext:StepExecutionContext
    {

        public FlowDefs.FlowDef FlowDef { get; set; }

        public Flows.Flow Flow { get; set; }

        public TaskItem TaskItem { get; set; }

        public override FlowConfig FlowConfig
        {
            get
            {
                return FlowDef.GetConfig();

            }
        }


    }
}
