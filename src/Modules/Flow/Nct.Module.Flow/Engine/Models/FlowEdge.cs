using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{
    public class FlowEdge : FlowElement
    {
        public string SourceNodeId { get; set; }

        public string TargetNodeId { get; set; }


    }
}
