using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Module.Flow.Engine.Models;

namespace Nct.Module.Flow.Engine
{
    public class FlowConfig
    {
        public List<FlowNode> Nodes { get; set; }

        public List<FlowEdge> Edges { get; set; }


        public FlowNode GetNextNode(FlowNode node)
        {
            return GetNextNodes(node).FirstOrDefault();

        }

        public FlowNode[] GetNextNodes(FlowNode node)
        {
            var edges = Edges.Where(m => m.SourceNodeId == node.Id).ToList();
            if (edges.Count == 0) return [];
            return Nodes.Where(m => edges.Any(e => e.TargetNodeId == m.Id)).ToArray();

        }


    }
}
