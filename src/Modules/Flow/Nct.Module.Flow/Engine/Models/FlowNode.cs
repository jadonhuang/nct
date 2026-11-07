using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{

    public enum NodeStatus
    {
        Pending,
        Running,
        Completed,
        Failed
    }

    public class FlowNode:FlowElement
    {

        [JsonIgnore]
        public bool IsSlient { get; set; } = false;

        [JsonIgnore]
        public List<FlowNode> PreNodes { get; set; } = [];

        public void LinkToPreNodes(params FlowNode[] nodes)
        {
            foreach (var node in nodes)
            {
                node.PreNodes.Add(this);
            }
        }




    }
}
