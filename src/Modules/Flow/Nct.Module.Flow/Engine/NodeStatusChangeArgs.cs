using Nct.Module.Flow.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine
{
    public class NodeStatusChangeArgs
    {

        public NodeStatusChangeArgs(string nodeId, NodeStatus status)
        {
            NodeId = nodeId;
            Status = status;
        }

        public string NodeId { get; set; }
        public NodeStatus Status { get; set; }
        public object Data { get; set; }

   

    }
}
