using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine
{
    public class FlowJobStepExecutionContext : StepExecutionContext
    {

       
        public int JobId { get; set;}

        public override string GetStepNamespace()
        {
            return base.GetStepNamespace()+".Job";
        }



    }
}
