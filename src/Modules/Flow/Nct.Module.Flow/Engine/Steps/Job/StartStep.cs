using Nct.Module.Flow.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Steps.Job
{
    public class StartStep : Step
    {
        public override Task<IStepExecutionResult> Run(StepExecutionContext context)
        {
            var ctx = (FlowJobStepExecutionContext)context;
            var props = context.Node.Properties.To<JobStartNodeProps>();
            var result = Success(context.FlowConfig.GetNextNodes(context.Node));

            foreach (var variable in props.GetOutputVariables())
            {
                object value;
                if (variable.Name == "sys.user_id")
                {
                    value = ISession.Session.UserId;
                }
                else if (variable.Name == "sys.user_name")
                {
                    value = ISession.Session.UserName;
                }
                else if (variable.Name == "sys.job_id")
                {
                    value = ctx.JobId;
                }
                else
                {
                    value = variable.Value;
                }
                var variableName = variable.Name;
                result.Outputs[variableName] = value;
            }
            return Task.FromResult(result);
        }

    }
}
